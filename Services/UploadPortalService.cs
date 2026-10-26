using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using JiwaCustomerPortal.Models;
using JiwaCustomerPortal.Options;
using Microsoft.Extensions.Options;

namespace JiwaCustomerPortal.Services;

/// <summary>
/// Creates SAS-based upload URLs and persists upload metadata.
/// </summary>
public sealed class UploadPortalService
{
    private readonly UploadPortalOptions _options;
    private readonly ILogger<UploadPortalService> _logger;

    /// <summary>
    /// Creates a new upload portal service.
    /// </summary>
    /// <param name="options">Strongly typed upload portal configuration.</param>
    /// <param name="logger">Logger instance.</param>
    public UploadPortalService(
        IOptions<UploadPortalOptions> options,
        ILogger<UploadPortalService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Validates an upload request and generates a browser-ready SAS URL for Azure Blob Storage.
    /// </summary>
    /// <param name="request">The upload request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The prepared upload details.</returns>
    public async Task<PrepareUploadResponse> PrepareUploadAsync(PrepareUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        ValidateRequest(request);

        var uploadId = Guid.NewGuid();
        var companyFolder = SanitizePathSegment(request.CompanyName);
        var rootCompanyFolder = string.IsNullOrWhiteSpace(request.RootCompanyName)
            ? null
            : SanitizePathSegment(request.RootCompanyName);

        var storageFolder = string.IsNullOrWhiteSpace(rootCompanyFolder)
            ? companyFolder
            : $"{rootCompanyFolder}/{companyFolder}";

        var safeFileName = SanitizeFileName(request.FileName);
        var fileNameWithTicket = AppendTicketNumberToFileName(safeFileName, request.SupportTicket);
        var blobFileName = await GetUniqueBlobFileNameAsync(storageFolder, fileNameWithTicket, cancellationToken);
        var blobPath = $"{storageFolder}/{blobFileName}";
        var sasExpiryHours = GetEffectiveSasExpiryHours(request.FileSizeBytes);
        var blobUri = await GenerateSasUriAsync(blobPath, sasExpiryHours, cancellationToken);

        return new PrepareUploadResponse
        {
            UploadId = uploadId,
            BlobUri = blobUri,
            BlobPath = blobPath,
            OriginalFileName = request.FileName,
            FileSizeBytes = request.FileSizeBytes,
            UploadDateUtc = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marks an upload as complete and applies blob metadata directly to Azure Storage.
    /// </summary>
    /// <param name="request">Upload completion payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload completion summary.</returns>
    public async Task<CompleteUploadResponse> CompleteUploadAsync(CompleteUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.BlobPath))
        {
            throw new ValidationException("The blob path is required.");
        }

        if (string.IsNullOrWhiteSpace(request.SupportTicket))
        {
            throw new ValidationException("Support Ticket Number is required.");
        }

        var uploadDateUtc = DateTime.UtcNow;
        var metadata = new Dictionary<string, string>
        {
            ["CompanyName"] = string.IsNullOrWhiteSpace(request.CompanyName) ? string.Empty : request.CompanyName.Trim(),
            ["ContactName"] = string.IsNullOrWhiteSpace(request.ContactName) ? string.Empty : request.ContactName.Trim(),
            ["EmailAddress"] = string.IsNullOrWhiteSpace(request.EmailAddress) ? string.Empty : request.EmailAddress.Trim(),
            ["SupportTicket"] = string.IsNullOrWhiteSpace(request.SupportTicket) ? string.Empty : request.SupportTicket.Trim(),
            ["OriginalFileName"] = string.IsNullOrWhiteSpace(request.FileName) ? Path.GetFileName(request.BlobPath) : request.FileName.Trim(),
            ["UploadDateUtc"] = uploadDateUtc.ToString("O"),
            ["UploadId"] = request.UploadId.ToString()
        };

        var blobClient = await GetBlobClientAsync(request.BlobPath, cancellationToken);
        await blobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);

        _logger.LogInformation("Upload {UploadId} completed for {CompanyName} ({FileName})", request.UploadId, metadata["CompanyName"], metadata["OriginalFileName"]);

        return new CompleteUploadResponse
        {
            UploadId = request.UploadId,
            FileName = metadata["OriginalFileName"],
            FileSizeBytes = request.FileSizeBytes,
            UploadDateUtc = uploadDateUtc
        };
    }

    /// <summary>
    /// Validates the upload request before generating a SAS token.
    /// </summary>
    public void ValidateRequest(PrepareUploadRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName))
        {
            throw new ValidationException("Company Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new ValidationException("Please select a file to upload.");
        }

        if (string.IsNullOrWhiteSpace(request.SupportTicket))
        {
            throw new ValidationException("Support Ticket Number is required.");
        }

        if (request.FileSizeBytes <= 0)
        {
            throw new ValidationException("The selected file is empty.");
        }

        var maxBytes = _options.MaxUploadSizeBytes;
        if (request.FileSizeBytes > maxBytes)
        {
            throw new ValidationException($"The selected file exceeds the maximum allowed size of {_options.MaxUploadSizeGB} GB.");
        }

        var extension = Path.GetExtension(request.FileName);
        if (!string.IsNullOrWhiteSpace(extension) && !_options.AllowedFileExtensions.Any(x => string.Equals(x, extension, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException($"The file extension '{extension}' is not supported. Allowed extensions are: {string.Join(", ", _options.AllowedFileExtensions)}.");
        }
    }

    private async Task<string> GenerateSasUriAsync(string blobPath, int sasExpiryHours, CancellationToken cancellationToken)
    {
        var accountName = _options.StorageAccountName;
        var credential = new DefaultAzureCredential();
        var containerClient = new BlobContainerClient(new Uri($"https://{accountName}.blob.core.windows.net/{_options.ContainerName}"), credential);
        //await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var startsOn = DateTimeOffset.UtcNow.AddMinutes(-5);
        var expiresOn = DateTimeOffset.UtcNow.AddHours(sasExpiryHours);
        var blobClient = containerClient.GetBlobClient(blobPath);
        var userDelegationKey = await blobClient.GetParentBlobContainerClient().GetParentBlobServiceClient()
            .GetUserDelegationKeyAsync(startsOn, expiresOn, cancellationToken);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _options.ContainerName,
            BlobName = blobPath,
            Resource = "b",
            StartsOn = startsOn,
            ExpiresOn = expiresOn,
            Protocol = SasProtocol.Https
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write | BlobSasPermissions.Add);
        var sasQuery = sasBuilder.ToSasQueryParameters(userDelegationKey, accountName).ToString();

        return $"{blobClient.Uri}?{sasQuery}";
    }

    private int GetEffectiveSasExpiryHours(long fileSizeBytes)
    {
        var minimumExpiryHours = fileSizeBytes switch
        {
            >= 100L * 1024 * 1024 * 1024 => 72,
            >= 25L * 1024 * 1024 * 1024 => 48,
            _ => 24
        };

        var effectiveExpiryHours = Math.Max(_options.SasExpiryHours, minimumExpiryHours);
        if (effectiveExpiryHours != _options.SasExpiryHours)
        {
            _logger.LogInformation(
                "Extended direct-upload SAS expiry from {ConfiguredSasExpiryHours}h to {EffectiveSasExpiryHours}h for {FileSizeBytes} bytes.",
                _options.SasExpiryHours,
                effectiveExpiryHours,
                fileSizeBytes);
        }

        return effectiveExpiryHours;
    }

    private Task<BlobClient> GetBlobClientAsync(string blobPath, CancellationToken cancellationToken)
    {
        var accountName = _options.StorageAccountName;
        var serviceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new DefaultAzureCredential());
        var containerClient = serviceClient.GetBlobContainerClient(_options.ContainerName);
        return Task.FromResult(containerClient.GetBlobClient(blobPath));
    }

    private async Task<string> GetUniqueBlobFileNameAsync(string companyFolder, string fileName, CancellationToken cancellationToken)
    {
        var directoryPath = $"{companyFolder}/";
        var extension = Path.GetExtension(fileName);
        var fileStem = string.IsNullOrEmpty(extension) ? fileName : fileName[..^extension.Length];
        var suffix = 1;
        var candidate = fileName;

        while (await BlobExistsAsync($"{directoryPath}{candidate}", cancellationToken))
        {
            candidate = $"{fileStem}_{suffix}{extension}";
            suffix++;
        }

        return candidate;
    }

    private async Task<bool> BlobExistsAsync(string blobPath, CancellationToken cancellationToken)
    {
        var accountName = _options.StorageAccountName;
        var serviceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new DefaultAzureCredential());
        var containerClient = serviceClient.GetBlobContainerClient(_options.ContainerName);
        var blobClient = containerClient.GetBlobClient(blobPath);
        return await blobClient.ExistsAsync(cancellationToken);
    }

    private static string SanitizePathSegment(string value)
    {
        var cleaned = Regex.Replace(value.Trim(), @"[^a-zA-Z0-9 _-]", "-");
        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
        cleaned = cleaned.Replace('/', '-').Replace('\\', '-');
        return string.IsNullOrWhiteSpace(cleaned) ? "customer" : cleaned;
    }

    private static string SanitizeFileName(string fileName)
    {
        var trimmed = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return "upload.bin";
        }

        return string.Concat(trimmed.Select(character => char.IsLetterOrDigit(character) || character is '-' or '_' or '.' ? character : '-'));
    }

    private static string AppendTicketNumberToFileName(string fileName, string? supportTicket)
    {
        if (string.IsNullOrWhiteSpace(supportTicket))
        {
            return fileName;
        }

        var sanitizedTicket = string.Concat(supportTicket.Trim().Select(character => char.IsLetterOrDigit(character) || character is '-' or '_' ? character : '-'));
        sanitizedTicket = sanitizedTicket.Trim('-').Trim();
        if (string.IsNullOrWhiteSpace(sanitizedTicket))
        {
            return fileName;
        }

        var extension = Path.GetExtension(fileName);
        var nameWithoutExtension = string.IsNullOrEmpty(extension) ? fileName : fileName[..^extension.Length];
        return $"{nameWithoutExtension}_{sanitizedTicket}{extension}";
    }

    }
