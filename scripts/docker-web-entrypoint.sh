#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="${OPENBREW_REPO_ROOT:-${BREWGR_REPO_ROOT:-/workspace/brewgr}}"
WEB_ROOT="${OPENBREW_WEB_ROOT:-${BREWGR_WEB_ROOT:-$REPO_ROOT/Openbrew.Web}}"
HOST_PORT="${OPENBREW_HOST_PORT:-${BREWGR_HOST_PORT:-8085}}"
HOST_NAME="${OPENBREW_HOST_NAME:-${BREWGR_HOST_NAME:-localhost}}"
DB_NAME="${OPENBREW_DB_NAME:-${BREWGR_DB_NAME:-Brewgr_DEV}}"
SA_PASSWORD="${OPENBREW_SA_PASSWORD:-${BREWGR_SA_PASSWORD:-Brewgr_dev_123!}}"

ROOT_URL="${OPENBREW_ROOT_URL:-http://${HOST_NAME}:${HOST_PORT}}"
ROOT_URL_SECURE="${OPENBREW_ROOT_URL_SECURE:-$ROOT_URL}"

load_env_from_file() {
  local name="$1"
  local file_var="${name}_FILE"
  local file_path="${!file_var:-}"

  if [ -n "$file_path" ] && [ -f "$file_path" ]; then
    export "$name"="$(tr -d '\r\n' < "$file_path")"
  fi
}

load_env_from_file SMTP_HOST
load_env_from_file SMTP_PORT
load_env_from_file SMTP_USERNAME
load_env_from_file SMTP_PASSWORD
load_env_from_file OPENBREW_CONTACT_EMAIL_ADDRESS
export OPENBREW_ROOT_URL="$ROOT_URL"
export OPENBREW_ROOT_URL_SECURE="$ROOT_URL_SECURE"
export OPENBREW_STATIC_ROOT_URL="${OPENBREW_STATIC_ROOT_URL:-$ROOT_URL}"
export OPENBREW_STATIC_ROOT_URL_SECURE="${OPENBREW_STATIC_ROOT_URL_SECURE:-$ROOT_URL_SECURE}"
export OPENBREW_MEDIA_PHYSICAL_ROOT="${OPENBREW_MEDIA_PHYSICAL_ROOT:-$WEB_ROOT/Media}"
export OPENBREW_CONNECTION_STRING="${OPENBREW_CONNECTION_STRING:-Server=db,1433;Database=${DB_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True}"
export OPENBREW_BLOG_CONNECTION_STRING="${OPENBREW_BLOG_CONNECTION_STRING:-$OPENBREW_CONNECTION_STRING}"

mkdir -p "$OPENBREW_MEDIA_PHYSICAL_ROOT"
rm -f "$WEB_ROOT/bin/Brewgr.Web.dll" "$WEB_ROOT/bin/Brewgr.Web.pdb" \
  "$WEB_ROOT/obj/Debug/Brewgr.Web.dll" "$WEB_ROOT/obj/Debug/Brewgr.Web.pdb"

MONO_GAC_PREFIX=/usr xbuild "$REPO_ROOT/Openbrew.Web.sln" /p:Configuration=Debug /verbosity:minimal

echo "Starting Brewgr web server on ${ROOT_URL}"
cd "$WEB_ROOT"
exec xsp4 \
  --address=0.0.0.0 \
  --port="$HOST_PORT" \
  --nonstop \
  --applications=/:.
