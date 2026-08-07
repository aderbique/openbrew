#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CATALOG_PATH="${1:-$ROOT/Setup/Catalogs/yeasts.catalog.json}"
SQL_SERVER="${OPENBREW_SQL_SERVER:-host.docker.internal,1433}"
SQL_DATABASE="${OPENBREW_DB_NAME:-${BREWGR_DB_NAME:-Brewgr_DEV}}"
SQL_PASSWORD="${OPENBREW_SA_PASSWORD:-${BREWGR_SA_PASSWORD:-Brewgr_dev_123!}}"
SQL_IMAGE="${SQLCMD_DOCKER_IMAGE:-mcr.microsoft.com/mssql-tools}"

if [ ! -f "$CATALOG_PATH" ]; then
  echo "Yeast catalog not found: $CATALOG_PATH" >&2
  exit 1
fi

python3 "$ROOT/scripts/generate-yeast-catalog-sql.py" "$CATALOG_PATH" |
  docker run --rm -i "$SQL_IMAGE" /opt/mssql-tools/bin/sqlcmd \
    -S "$SQL_SERVER" -U sa -P "$SQL_PASSWORD" -C -d "$SQL_DATABASE" -b -i /dev/stdin

echo "Imported yeast catalog: $CATALOG_PATH"
