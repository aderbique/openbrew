#!/usr/bin/env python3
"""Generate idempotent SQL Server upserts from OpenBrew's hop catalog JSON."""
from __future__ import print_function

import json
import sys


def sql_string(value):
    return "N'{}'".format(value.replace("'", "''"))


def normalized(value):
    return value.strip().lower()


def country_for(hop):
    if hop.get('country'):
        return hop['country'].strip()
    category = hop.get('category', '')
    prefixes = {
        'US ': 'United States',
        'Australian ': 'Australia',
        'New Zealand ': 'New Zealand',
        'UK ': 'United Kingdom',
        'German ': 'Germany',
        'Czech ': 'Czech Republic',
    }
    for prefix, country in prefixes.items():
        if category.startswith(prefix):
            return country
    return ''

def emit_provenance(ingredient_match, field_name, value, source):
    source_url = source.get('url', '').strip()
    source_name = source.get('name', '').strip()
    if not source_url or not source_name:
        return
    confidence = source.get('confidence', 'medium').strip() or 'medium'
    system_hop_match = "IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND {}".format(ingredient_match)
    condition = "IngredientTypeId = 20 AND IngredientId = (SELECT TOP 1 HopId FROM dbo.Hop WHERE {}) AND FieldName = {} AND SourceUrl = {}".format(system_hop_match, sql_string(field_name), sql_string(source_url))
    print("IF EXISTS (SELECT 1 FROM dbo.IngredientSource WHERE {})".format(condition))
    print("  UPDATE dbo.IngredientSource SET FieldValue = {0}, SourceName = {1}, RetrievedAt = GETDATE(), Confidence = {2} WHERE {3};".format(sql_string(str(value)), sql_string(source_name), sql_string(confidence), condition))
    print("ELSE")
    print("  INSERT INTO dbo.IngredientSource (IngredientTypeId, IngredientId, FieldName, FieldValue, SourceUrl, SourceName, RetrievedAt, Confidence) SELECT 20, HopId, {0}, {1}, {2}, {3}, GETDATE(), {4} FROM dbo.Hop WHERE {5};".format(sql_string(field_name), sql_string(str(value)), sql_string(source_url), sql_string(source_name), sql_string(confidence), system_hop_match))


def main(path):
    with open(path, 'r') as catalog_file:
        catalog = json.load(catalog_file)

    if catalog.get('schemaVersion') != 1 or not isinstance(catalog.get('hops'), list):
        raise ValueError('Unsupported hop catalog schema')

    print('SET NOCOUNT ON;')
    print('BEGIN TRANSACTION;')
    default_source = catalog.get('defaultSource', {})
    for hop in catalog['hops']:
        name = hop['name'].strip()
        aliases = [name] + [alias.strip() for alias in hop.get('aliases', []) if alias.strip()]
        if not name or not aliases:
            raise ValueError('Every hop needs a name')
        alpha = float(hop['alphaAcid'])
        category = hop.get('category', '').strip()
        description = hop.get('description', '').strip()
        country = country_for(hop)
        source = hop.get('source', default_source)
        alias_sql = ', '.join(sql_string(normalized(alias)) for alias in aliases)
        match = "LOWER(LTRIM(RTRIM(Name))) IN ({})".format(alias_sql)
        print("IF EXISTS (SELECT 1 FROM dbo.Hop WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND {})".format(match))
        print('BEGIN')
        print("  UPDATE dbo.Hop SET AA = {0}, Category = {1}, Country = {2}, Description = {3} WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND {4};".format(alpha, sql_string(category), sql_string(country), sql_string(description), match))
        print('END')
        print('ELSE')
        print('BEGIN')
        print("  INSERT INTO dbo.Hop (Name, AA, Description, IsActive, IsPublic, DateCreated, Country, Category) VALUES ({0}, {1}, {2}, 1, 1, GETDATE(), {3}, {4});".format(sql_string(name), alpha, sql_string(description), sql_string(country), sql_string(category)))
        print('END;')
        emit_provenance(match, 'alpha_acid_default', alpha, source)
        emit_provenance(match, 'description', description, source)
        emit_provenance(match, 'country', country, source)
    print('COMMIT TRANSACTION;')


if __name__ == '__main__':
    if len(sys.argv) != 2:
        raise SystemExit('Usage: generate-hop-catalog-sql.py <catalog.json>')
    main(sys.argv[1])
