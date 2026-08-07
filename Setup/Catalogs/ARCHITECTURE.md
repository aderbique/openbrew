# Ingredient catalog architecture

OpenBrew keeps its original recipe ingredient tables as stable, generic defaults. The additive catalog tables make those defaults traceable and allow more precise supplier data without changing existing recipes.

| Catalog | Legacy recipe default | Additive product / supporting data |
| --- | --- | --- |
| Hops | `Hop` is the variety (Cascade, Citra, Mosaic) | `HopProduct` holds supplier, crop year, alpha/beta acids, form, lot, URL |
| Fermentables | `Fermentable` is the generic calculation default | `MaltType` identifies a conceptual malt; `MaltProduct` identifies a branded malt and optional legacy mapping |
| Yeast | `Yeast` is the culture default | producer country, official catalog URL, and `IngredientSource` fields; a future `YeastProduct` can follow the same pattern |
| Miscellaneous | `Adjunct` | supplier-specific products can use the same provenance table now, then receive a product table when the UI needs it |
| Water additions | none in the legacy schema | `WaterAddition` is ready for salts, acids, and treatment agents |
| Beer styles | `BjcpStyle` / current BA catalog | guideline edition remains a separate, versioned catalog |

## Provenance

`IngredientSource` records a value's field name, source URL/name, retrieval date, and confidence. It is intentionally field-level: a hop's aroma can be backed by one source while its alpha-acid planning default is backed by another. Imports upsert only the exact `(ingredient, field, source URL)` record, preserving audit history from other sources.

## Recipe behavior

Existing recipes retain the `HopId`, `FermentableId`, and frozen per-recipe AA/PPG/Lovibond values they already use. A later product-aware recipe editor can add nullable `HopProductId` and `MaltProductId` references to recipe ingredient rows. That must be an explicit UI migration, not an automatic conversion: an old recipe has no trustworthy lot or supplier to infer.
