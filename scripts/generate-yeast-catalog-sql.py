#!/usr/bin/env python3
"""Generate idempotent SQL Server upserts from OpenBrew's yeast catalog JSON."""
from __future__ import print_function

import json
import sys


def sql_string(value):
    return "N'{}'".format(value.replace("'", "''"))


def normalized(value):
    return value.strip().lower()


def main(path):
    with open(path, 'r') as catalog_file:
        catalog = json.load(catalog_file)
    if catalog.get('schemaVersion') != 1 or not isinstance(catalog.get('yeasts'), list):
        raise ValueError('Unsupported yeast catalog schema')
    manufacturers = catalog.get('manufacturers', {})

    print('SET NOCOUNT ON;')
    print('BEGIN TRANSACTION;')
    for yeast in catalog['yeasts']:
        manufacturer = yeast['manufacturer'].strip()
        product_code = yeast['productCode'].strip()
        canonical_name = '{} {} {}'.format(manufacturer, product_code, yeast['name'].strip())
        aliases = [canonical_name] + [alias.strip() for alias in yeast.get('aliases', []) if alias.strip()]
        attenuation = float(yeast['attenuation'])
        category = yeast.get('category', manufacturer).strip()
        description = yeast.get('description', '').strip()
        producer = manufacturers.get(manufacturer, {})
        country = yeast.get('country', producer.get('country', '')).strip()
        website_url = yeast.get('websiteUrl', producer.get('websiteUrl', '')).strip()
        alias_sql = ', '.join(sql_string(normalized(alias)) for alias in aliases)
        match = "LOWER(LTRIM(RTRIM(Name))) IN ({})".format(alias_sql)
        print("IF EXISTS (SELECT 1 FROM dbo.Yeast WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND {})".format(match))
        print('BEGIN')
        print("  UPDATE dbo.Yeast SET Name = {0}, Attenuation = {1}, Category = {2}, Description = {3}, Country = {4}, WebsiteUrl = {5} WHERE IsActive = 1 AND IsPublic = 1 AND CreatedByUserId IS NULL AND {6};".format(sql_string(canonical_name), attenuation, sql_string(category), sql_string(description), sql_string(country), sql_string(website_url), match))
        print('END')
        print('ELSE')
        print('BEGIN')
        print("  INSERT INTO dbo.Yeast (Name, Attenuation, Description, IsActive, IsPublic, DateCreated, Category, Country, WebsiteUrl) VALUES ({0}, {1}, {2}, 1, 1, GETDATE(), {3}, {4}, {5});".format(sql_string(canonical_name), attenuation, sql_string(description), sql_string(category), sql_string(country), sql_string(website_url)))
        print('END;')
    print('COMMIT TRANSACTION;')


if __name__ == '__main__':
    if len(sys.argv) != 2:
        raise SystemExit('Usage: generate-yeast-catalog-sql.py <catalog.json>')
    main(sys.argv[1])
