# Ingredient catalogs

Catalog JSON files are the versioned source of truth for OpenBrew's public ingredient defaults. They are deliberately separate from user-created ingredients and recipes.

## Hops

`hops.catalog.json` contains reviewed defaults: a canonical name, aliases, representative alpha acid percentage, category, and a short description. Alpha acid data is a planning default; brewers should still use the percentage printed on their specific hop lot.

Import it into a running SQL Server database with:

```bash
bash scripts/import-hop-catalog.sh
```

The importer is idempotent. It matches canonical names and aliases case-insensitively, updates only active public system hops, inserts missing hops, and never changes user-created hops (`CreatedByUserId IS NOT NULL`). It is safe to rerun after catalog edits.

The legacy `Hop` row is a **variety default**, not a supplier lot. `hop-products.catalog.json` is deliberately separate and has no fabricated rows: supplier, crop year, AA/beta acid, form, and lot data must originate from a supplier sheet. The additive `HopProduct` database table is ready for those records.

Before adding a release, validate the variety against breeder or supplier data and record the source in the JSON file. Do not scrape or copy a third-party catalog wholesale; normalize and curate the fields OpenBrew actually stores.

## Yeasts

`yeasts.catalog.json` uses the same pattern and keeps manufacturer and product code in the canonical name (for example, `Wyeast 1056 American Ale`). It currently curates public strains from Wyeast, White Labs, Omega Yeast, Mangrove Jack's, Fermentis, and Lallemand. This is necessary because the legacy `Yeast` table does not have separate producer and product-code columns.

Each listed producer also supplies a `country` (the producer's home country) and `websiteUrl` (its official culture catalog). The import stores both as first-class yeast fields, so they can be shown in the ingredient catalog without modifying tasting descriptions.

```bash
bash scripts/import-yeast-catalog.sh
```

The importer matches aliases, updates only public system records, and can rename an existing system yeast to its current producer/code/name. Recipe references use the numeric yeast ID, so those corrections do not break existing recipes.
