#!/usr/bin/env bash
set -euo pipefail

APP_ROOT="${OPENBREW_APP_ROOT:-/app}"
WEB_ROOT="${OPENBREW_WEB_ROOT:-${BREWGR_WEB_ROOT:-$APP_ROOT/Openbrew.Web}}"
HOST_PORT="${OPENBREW_HOST_PORT:-${BREWGR_HOST_PORT:-8085}}"
HOST_NAME="${OPENBREW_HOST_NAME:-${BREWGR_HOST_NAME:-0.0.0.0}}"
SQL_HOST="${OPENBREW_SQL_HOST:-${BREWGR_SQL_HOST:-db}}"
SQL_PORT="${OPENBREW_SQL_PORT:-${BREWGR_SQL_PORT:-1433}}"
DB_NAME="${OPENBREW_DB_NAME:-${BREWGR_DB_NAME:-Brewgr_DEV}}"
SQL_ADMIN_USER="${OPENBREW_SQL_ADMIN_USER:-${BREWGR_SQL_ADMIN_USER:-sa}}"
ROOT_URL="${OPENBREW_ROOT_URL:-http://${HOST_NAME}:${HOST_PORT}}"
ROOT_URL_SECURE="${OPENBREW_ROOT_URL_SECURE:-${ROOT_URL/http:/https:}}"

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
load_env_from_file OPENBREW_SA_PASSWORD
load_env_from_file Google_ApplicationKey
load_env_from_file Google_ApplicationSecret

: "${OPENBREW_SA_PASSWORD:?OPENBREW_SA_PASSWORD or OPENBREW_SA_PASSWORD_FILE must be configured}"
SA_PASSWORD="$OPENBREW_SA_PASSWORD"

export OPENBREW_ROOT_URL="$ROOT_URL"
export OPENBREW_ROOT_URL_SECURE="$ROOT_URL_SECURE"
export OPENBREW_STATIC_ROOT_URL="${OPENBREW_STATIC_ROOT_URL:-$ROOT_URL}"
export OPENBREW_STATIC_ROOT_URL_SECURE="${OPENBREW_STATIC_ROOT_URL_SECURE:-$ROOT_URL_SECURE}"
export OPENBREW_MEDIA_PHYSICAL_ROOT="${OPENBREW_MEDIA_PHYSICAL_ROOT:-/data/media}"
export OPENBREW_CONNECTION_STRING="${OPENBREW_CONNECTION_STRING:-Server=${SQL_HOST},${SQL_PORT};Database=${DB_NAME};User Id=${SQL_ADMIN_USER};Password=${SA_PASSWORD};TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True}"
export OPENBREW_BLOG_CONNECTION_STRING="${OPENBREW_BLOG_CONNECTION_STRING:-$OPENBREW_CONNECTION_STRING}"
export OPENBREW_SQL_HOST="$SQL_HOST"
export OPENBREW_SQL_PORT="$SQL_PORT"
export OPENBREW_DB_NAME="$DB_NAME"
export OPENBREW_SQL_ADMIN_USER="$SQL_ADMIN_USER"
export OPENBREW_SA_PASSWORD="$SA_PASSWORD"
export OPENBREW_DB_INIT_SCRIPT="${OPENBREW_DB_INIT_SCRIPT:-${BREWGR_DB_INIT_SCRIPT:-$APP_ROOT/Setup/Database/Build.20150807/20150807_initial.sql}}"

mkdir -p "$OPENBREW_MEDIA_PHYSICAL_ROOT"

if [ "${OPENBREW_SKIP_DB_INIT:-0}" != "1" ]; then
  DB_INIT_EXE="$APP_ROOT/Openbrew.DbInit/bin/Release/Openbrew.DbInit.exe"
  if [ ! -f "$DB_INIT_EXE" ]; then
    DB_INIT_EXE="$APP_ROOT/Openbrew.DbInit/bin/Debug/Openbrew.DbInit.exe"
  fi

  if [ -f "$DB_INIT_EXE" ]; then
    echo "Running database bootstrap..."
    mono "$DB_INIT_EXE"
  else
    echo "Database bootstrap executable not found; skipping."
  fi
fi

cd "$WEB_ROOT"
exec xsp4 \
  --address=0.0.0.0 \
  --port="$HOST_PORT" \
  --nonstop \
  --applications=/:.
