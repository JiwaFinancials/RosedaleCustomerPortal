# Products Feature

## Overview

The Products feature provides a searchable customer-facing product catalogue in the portal.

Main page:
- Route: `/Products`
- Component: `Components/Pages/Products.razor`

## Implemented Features

### 1. Product list and paging
- Loads products from Jiwa inventory query endpoint.
- Uses server paging with `Take` and `Skip`.
- Handles API page-size caps by incrementing `Skip` by returned row count.
- Attempts to load at least 200 records when available.

### 2. Dual search model
- Search Products: runs a new database query.
- Filter Results: client-side filtering over currently loaded results.

Search Products supports:
- Search field selector:
  - Description (`DescriptionContains`)
  - Part No (`PartNoContains`)
- Enter key to execute search.
- Search button and Reset Results button.

AND logic in DB query:
- `IN_LogicalID` AND chosen contains filter.

### 3. Category filters
- Category 1, 2, and 3 dropdown filters.
- Category options are generated from currently loaded result set.

### 4. Product details
- Shows selected product details:
  - Part No
  - Description
  - Classification
  - Category path
  - Stock on Hand
  - Available Stock
  - List Price
  - Your Price (customer-specific)
  - RRP
  - Logical and Physical warehouse

### 5. Customer-specific pricing (async)
- Loads on product selection.
- Uses async endpoint call and displays loading text while fetching.
- Uses current debtor context from signed-in session.

### 6. Image support
- Primary image from inventory `Picture`.
- Additional images loaded from `/Inventory/{InventoryID}/Images`.
- Multi-image selector panel in details area.
- Base64/data URI preferred; URL source fallback.
- URL images are converted server-side to data URI when possible.
- Per-product image caching is used.

### 7. CSV export
- Exports currently filtered products to CSV.

## Jiwa API Endpoints (Permissions Checklist)

Grant the following endpoints to the **Customer Web Portal** user group in Jiwa.

1. `GET /Queries/InventoryItemList`
- Used by product catalogue load, paging, DB search, and projected image lookup (`Fields=InventoryID,Picture`).
- Query parameters used by the feature:
  - `IN_LogicalID`
  - `OrderBy`
  - `Fields`
  - `Take`
  - `Skip`
  - `DescriptionContains`
  - `PartNoContains`
  - `InventoryID`

2. `GET /Inventory/{InventoryID}`
- Used as fallback for primary image (`Picture`) retrieval.

3. `GET /Inventory/{InventoryID}/Images`
- Used for additional gallery images in product details.
- Endpoint may return URL-only image sources (for example `WebStore_Image_src`) and title metadata (`Title`, `Caption`, `Description`, `WebStore_Image_name`, `AltText`).

4. `GET /Inventory/{InventoryID}/Pricing/{DebtorID}/{IN_LogicalID}/{Date}/{Quantity}`
- Used for async customer-specific pricing (`Your Price`).
- Current call pattern:
  - `Date = today` (`yyyy-MM-dd`)
  - `Quantity = 1`

## Authentication Behavior

For API calls, the feature uses:
1. Session cookie (`ss-id`) when available.
2. API key bearer token fallback when needed.

## Configuration Requirements

App configuration keys used:
- `JiwaAPIURL`
- `JiwaAPIKey`
- `IN_LogicalID`
- `IN_PhysicalID`
- `LogicalWarehouseDescription`
- `PhysicalWarehouseDescription`

Startup also reads Customer Web Portal settings API and falls back to appsettings warehouse values when API settings are missing.

## Performance Notes

- Inventory list query uses field projection (`Fields`) to reduce payload size.
- Additional images and customer specific pricing are loaded asynchronously after product selection.
- Image results and picture bytes are cached per `InventoryID`.

## Known Operational Notes

- Some image records may reference external URLs rather than embedded base64.
- If no debtor is available in session context (eg Staff Login), `Your Price` may show as unavailable.
