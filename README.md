# Rosedale Customer Portal

A fork of the [Jiwa Customer Portal](https://github.com/JiwaFinancials/JiwaCustomerPortal/) which adds licences and downloads, as well as other functions specific to our own use cases.

This is the project which powers [portal.jiwa.com.au](https://portal.jiwa.com.au/).

## Azure configuration for direct browser uploads

The Uploads page uses Azure Blob Storage for direct browser-to-storage uploads. The web application never streams the file contents through the web server itself. Instead, the server issues a short-lived SAS token and the browser uploads the file directly to Azure Storage using block/blob upload APIs.

### Browser SDK bundle

The browser loads the checked-in `wwwroot\js\vendor\azureStorageBlob.js` bundle locally. Do not replace it with a CDN `+esm` import: independently resolved Azure SDK dependencies can use incompatible versions, dropping required request headers and failing to reject unsuccessful HTTP responses.

After changing the JavaScript dependencies or `Scripts\azureStorageBlob.js`, regenerate the bundle with Node.js and npm, then commit the updated bundle and lockfile:

```powershell
npm ci
npm run build:upload-sdk
```

Normal .NET builds and deployments use the checked-in bundle and do not require Node.js.

### Storage settings

Default configuration values are already included in `appsettings.json`:

- Storage account name: `jiwafiledownloads`
- Container name: `customeruploads`
- SAS expiry: 24 hours (supports large backup uploads on slower connections)
- Maximum upload size: 500 GB

Example configuration:

```json
{
  "UploadPortalOptions": {
    "StorageAccountName": "jiwafiledownloads",
    "ContainerName": "customeruploads",
    "SasExpiryHours": 24,
    "MaxUploadSizeGB": 500,
    "AllowedFileExtensions": [
      ".bak",
      ".zip",
      ".7z",
      ".gz",
      ".rar",
      ".sql",
      ".dmp",
      ".ndf",
      ".ldf"
    ]
  }
}
```

You can override these values with environment variables using the ASP.NET Core configuration system. For example, `UploadPortalOptions__StorageAccountName`, `UploadPortalOptions__ContainerName`, and `UploadPortalOptions__MaxUploadSizeGB` are valid overrides.

The application uses `DefaultAzureCredential` and therefore expects an Azure identity to be available at runtime. In Azure App Service, Azure Container Apps, Azure Functions, or a VM, configure a managed identity and grant it the required storage permissions. Locally, sign in with `az login` or set the standard Azure environment variables before running the project.

### Local developer setup for direct uploads

Before testing the Uploads page on a developer machine, ensure the local Azure identity has access to the storage account. The app does not upload through the web server; it generates a SAS token and the browser calls Azure Blob Storage directly, so the machine running Visual Studio must be able to authenticate to the storage account.

1. Sign in to the correct tenant and subscription:

```bash
az login --tenant 50ea7a76-23fa-44a5-91e7-7187ef2fa97c
az account set --subscription "Jiwa Financials"
```

2. Grant the local developer identity the required storage roles on the storage account:

```bash
$sub = "/subscriptions/0cd37710-98b7-4c41-ac4d-19a7f1f6cc0c"
$scope = "$sub/resourceGroups/JiwaFileDownloads/providers/Microsoft.Storage/storageAccounts/jiwafiledownloads"

az role assignment create --assignee scottp@jiwa.com.au --role "Storage Blob Data Contributor" --scope $scope
az role assignment create --assignee scottp@jiwa.com.au --role "Storage Blob Delegator" --scope $scope
```

3. Allow the browser origin used for local development via CORS:

```powershell
az storage cors add `
  --account-name jiwafiledownloads `
  --services b `
  --methods PUT OPTIONS `
  --origins "http://localhost:5010" "https://localhost:5010" `
  --allowed-headers "Content-Type" "x-ms-*" `
  --exposed-headers "ETag" "x-ms-request-id" "x-ms-version" `
  --max-age 86400
```

Configure this rule on the Azure Storage blob service, not in the Blazor application's CORS middleware. The SDK sends headers such as `x-ms-version` and `x-ms-client-request-id` in addition to blob type and metadata headers, so allowing only `x-ms-blob-type` and `x-ms-meta-*` rejects its preflight requests. Pass allowed headers as separate arguments. For production, add a rule with the exact portal origin rather than a wildcard origin.

If rules already exist for these origins and methods, update their allowed headers too: Azure evaluates rules in order, so appending a corrected rule after a restrictive matching rule is insufficient.

4. If the app still fails to authenticate while debugging in Visual Studio, temporarily switch the upload service to `DefaultAzureCredential` instead of `DefaultAzureCredential` to match the signed-in Azure CLI context. This is a local debugging aid only; production should continue to use `DefaultAzureCredential` with managed identity.

## Creating the `customeruploads` container

The project defaults to the `customeruploads` container. It can be created with Azure CLI or in the Azure portal.

Using Azure CLI:

```bash
az storage container create \
  --account-name jiwafiledownloads \
  --name customeruploads \
  --auth-mode login
```

The app can also create the container automatically if it does not already exist when a SAS token is generated, but creating it up front is recommended.

## Required Azure RBAC permissions

The application identity needs the ability to create user-delegation SAS tokens and write blob metadata for the upload container.

Recommended role assignments on the storage account:

- `Storage Blob Data Contributor`
- `Storage Blob Delegator`

These roles allow the application identity to create the container when required, generate user-delegation SAS tokens, and write blob metadata for browser-driven uploads. If you use a stricter environment, ensure the identity still has permission to:

- write blobs and metadata
- create containers
- generate user delegation keys / SAS tokens
- read and list blob metadata when required

If you are using a managed identity, assign that identity the `Storage Blob Data Contributor` and `Storage Blob Delegator` roles on the storage account or the containing resource group in Azure RBAC.

When the application is running locally, the logged-in developer identity must also be granted the same permissions to test uploads successfully. A browser `Failed to fetch` error can also indicate that Azure Storage rejected the CORS preflight; check the browser Network tab for an `OPTIONS` request returning `403 CorsPreflightFailure` and review the blob service CORS rule above.

## Upload layout and blob metadata

The application writes files using the following structure inside the `customeruploads` container:

```text
customeruploads/
    Acme Pty Ltd/
        2026-10-20/
            4f5a0bc8-database.bak
```

Blob metadata is stored with the following values:

- CompanyName
- ContactName
- EmailAddress
- SupportTicket
- OriginalFileName
- UploadDateUtc
- UploadId
