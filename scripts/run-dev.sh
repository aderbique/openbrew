#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# Local credentials stay outside version-controlled deployment manifests.
# Parse this optional file as dotenv-style data rather than sourcing it, so SMTP
# passwords containing shell characters such as &, $, or ! are preserved.
if [ -f "$ROOT/.openbrew.dev.env" ]; then
  while IFS='=' read -r env_name env_value || [ -n "$env_name" ]; do
    env_name="${env_name%$'\r'}"
    env_value="${env_value%$'\r'}"
    case "$env_name" in
      GOOGLE_APPLICATION_KEY|GOOGLE_APPLICATION_SECRET|SMTP_HOST|SMTP_PORT|SMTP_USERNAME|SMTP_USER|SMTP_PASSWORD|OPENBREW_CONTACT_EMAIL_ADDRESS)
        if [ "${env_value#\"}" != "$env_value" ] && [ "${env_value%\"}" != "$env_value" ]; then env_value="${env_value#\"}"; env_value="${env_value%\"}"; fi
        if [ "${env_value#\'}" != "$env_value" ] && [ "${env_value%\'}" != "$env_value" ]; then env_value="${env_value#\'}"; env_value="${env_value%\'}"; fi
        export "$env_name=$env_value"
        ;;
    esac
  done < "$ROOT/.openbrew.dev.env"
fi

SA_PASSWORD="${OPENBREW_SA_PASSWORD:?Set OPENBREW_SA_PASSWORD in .openbrew.dev.env or the environment.}"
DB_NAME="${OPENBREW_DB_NAME:-${BREWGR_DB_NAME:-Brewgr_DEV}}"
HOST_PORT="${OPENBREW_HOST_PORT:-${BREWGR_HOST_PORT:-8085}}"
HOST_NAME="${OPENBREW_HOST_NAME:-${BREWGR_HOST_NAME:-localhost}}"
SQLCMD_DOCKER_IMAGE="${SQLCMD_DOCKER_IMAGE:-mcr.microsoft.com/mssql-tools}"
SMTP_HOST="${SMTP_HOST:-}"
SMTP_PORT="${SMTP_PORT:-}"
SMTP_USERNAME="${SMTP_USERNAME:-}"
SMTP_PASSWORD="${SMTP_PASSWORD:-}"

run_sqlcmd() {
  docker run --rm \
    -v "$ROOT:/repo" \
    "$SQLCMD_DOCKER_IMAGE" \
    /opt/mssql-tools/bin/sqlcmd "$@"
}

if ! command -v docker >/dev/null 2>&1; then
  echo "docker not found on PATH. Install Docker or set up an equivalent container runtime." >&2
  exit 1
fi

mkdir -p "$ROOT/Openbrew.Web/Media"
rm -f "$ROOT/Openbrew.Web/bin/Brewgr.Web.dll" "$ROOT/Openbrew.Web/bin/Brewgr.Web.pdb" \
  "$ROOT/Openbrew.Web/obj/Debug/Brewgr.Web.dll" "$ROOT/Openbrew.Web/obj/Debug/Brewgr.Web.pdb"

if docker inspect brewgr-sql >/dev/null 2>&1; then
  docker start brewgr-sql >/dev/null
else
  docker run -d \
    --name brewgr-sql \
    --network brewgr-dev \
    --network-alias db \
    -e ACCEPT_EULA=1 \
    -e MSSQL_SA_PASSWORD="$SA_PASSWORD" \
    -p 1433:1433 \
    -v brewgr-sql-data:/var/opt/mssql \
    --restart unless-stopped \
    mcr.microsoft.com/azure-sql-edge:latest >/dev/null
fi

docker network connect brewgr-dev brewgr-sql >/dev/null 2>&1 || true

echo "Waiting for SQL Server..."
for _ in $(seq 1 60); do
  if run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d master -Q "SELECT 1" >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

if ! run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d master -h -1 -W -Q "SET NOCOUNT ON; IF DB_ID(N'${DB_NAME}') IS NOT NULL SELECT 1;" | grep -qx '1'; then
  run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -Q "CREATE DATABASE [${DB_NAME}];"
fi

if ! run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" -h -1 -W -Q "SET NOCOUNT ON; IF OBJECT_ID(N'dbo.IngredientCategory', N'U') IS NOT NULL SELECT 1;" | grep -qx '1'; then
  echo "Resetting and seeding database from initial script..."
  run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -Q "IF DB_ID(N'${DB_NAME}') IS NOT NULL BEGIN ALTER DATABASE [${DB_NAME}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [${DB_NAME}]; END"
  run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -Q "CREATE DATABASE [${DB_NAME}];"
  run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" -i "/repo/Setup/Database/Build.20150807/20150807_initial.sql"
fi

# Idempotent application migrations keep existing local data intact.
run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" -i "/repo/Setup/Database/20260807_add_newsletter_confirmation.sql"
run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" -i "/repo/Setup/Database/20260807_add_user_login_ip_address.sql"
run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" -i "/repo/Setup/Database/20260807_add_yeast_catalog_metadata.sql"
run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" -i "/repo/Setup/Database/20260807_add_ingredient_catalog_foundation.sql"
run_sqlcmd -S host.docker.internal,1433 -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" -i "/repo/Setup/Database/20260807_refresh_adjunct_and_mash_catalog.sql"

MONO_GAC_PREFIX=/opt/homebrew xbuild "$ROOT/Openbrew.Web.sln" /p:Configuration=Debug /verbosity:minimal

export OPENBREW_WEB_ROOT="${OPENBREW_WEB_ROOT:-${BREWGR_WEB_ROOT:-$ROOT/Openbrew.Web}}"
export OPENBREW_HOST_PORT="$HOST_PORT"
export OPENBREW_HOST_NAME="$HOST_NAME"
export OPENBREW_ROOT_URL="http://${HOST_NAME}:${HOST_PORT}"
export OPENBREW_ROOT_URL_SECURE="http://${HOST_NAME}:${HOST_PORT}"
export OPENBREW_STATIC_ROOT_URL="http://${HOST_NAME}:${HOST_PORT}"
export OPENBREW_STATIC_ROOT_URL_SECURE="http://${HOST_NAME}:${HOST_PORT}"
export OPENBREW_MEDIA_PHYSICAL_ROOT="${OPENBREW_MEDIA_PHYSICAL_ROOT:-$ROOT/Openbrew.Web/Media}"
export OPENBREW_CONNECTION_STRING="Server=brewgr-sql,1433;Database=${DB_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True"
export OPENBREW_BLOG_CONNECTION_STRING="${OPENBREW_BLOG_CONNECTION_STRING:-$OPENBREW_CONNECTION_STRING}"
export OPENBREW_CONTACT_EMAIL_ADDRESS="${OPENBREW_CONTACT_EMAIL_ADDRESS:-austinjderbique@gmail.com}"

docker network inspect brewgr-dev >/dev/null 2>&1 || docker network create brewgr-dev >/dev/null

docker build -t brewgr-web -f "$ROOT/Dockerfile.web" "$ROOT" >/dev/null

if docker inspect brewgr-web >/dev/null 2>&1; then
  docker rm -f brewgr-web >/dev/null
fi

existing_web_containers="$(docker ps -aq --filter publish="$HOST_PORT")"
if [ -n "$existing_web_containers" ]; then
  docker rm -f $existing_web_containers >/dev/null
fi

docker run -d \
  --name brewgr-web \
  --network brewgr-dev \
  -p "${HOST_PORT}:${HOST_PORT}" \
  -e OPENBREW_REPO_ROOT=/workspace/brewgr \
  -e OPENBREW_WEB_ROOT=/workspace/brewgr/Openbrew.Web \
  -e OPENBREW_HOST_PORT="$HOST_PORT" \
  -e OPENBREW_HOST_NAME="$HOST_NAME" \
  -e OPENBREW_DB_NAME="$DB_NAME" \
  -e OPENBREW_SA_PASSWORD="$SA_PASSWORD" \
  -e OPENBREW_CONNECTION_STRING="$OPENBREW_CONNECTION_STRING" \
  -e OPENBREW_BLOG_CONNECTION_STRING="$OPENBREW_BLOG_CONNECTION_STRING" \
  -e OPENBREW_CONTACT_EMAIL_ADDRESS="$OPENBREW_CONTACT_EMAIL_ADDRESS" \
  -e SmtpHost="$SMTP_HOST" \
  -e SmtpPort="$SMTP_PORT" \
  -e SmtpUserName="$SMTP_USERNAME" \
  -e SmtpPassword="$SMTP_PASSWORD" \
  -e Google_ApplicationKey="${GOOGLE_APPLICATION_KEY:-}" \
  -e Google_ApplicationSecret="${GOOGLE_APPLICATION_SECRET:-}" \
  -v "$ROOT:/workspace/brewgr" \
  -v "$ROOT/../packages:/workspace/packages" \
  --restart unless-stopped \
  brewgr-web >/dev/null

echo "Starting Brewgr at http://${HOST_NAME}:${HOST_PORT}"
exec docker logs -f brewgr-web
