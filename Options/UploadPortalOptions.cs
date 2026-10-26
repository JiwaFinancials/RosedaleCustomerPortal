namespace JiwaCustomerPortal.Options;

/// <summary>
/// Application settings for the customer upload portal.
/// </summary>
public sealed class UploadPortalOptions
{
    /// <summary>
    /// The configuration section name used to bind this options type.
    /// </summary>
    public const string SectionName = "UploadPortalOptions";

    /// <summary>
    /// Azure Storage account name hosting the uploads container.
    /// </summary>
    public string StorageAccountName { get; set; } = "jiwafiledownloads";

    /// <summary>
    /// Azure Blob Storage container name for customer uploads.
    /// </summary>
    public string ContainerName { get; set; } = "customeruploads";

    /// <summary>
    /// Number of hours a generated SAS token remains valid. Large uploads should allow more time
    /// than a typical web request because browsers upload directly to Azure Storage and can take
    /// several hours on slower connections.
    /// </summary>
    public int SasExpiryHours { get; set; } = 24;

    /// <summary>
    /// Maximum file size allowed for an upload, configured in gigabytes.
    /// </summary>
    public int MaxUploadSizeGB { get; set; } = 500;

    /// <summary>
    /// File extensions accepted for upload.
    /// </summary>
    public string[] AllowedFileExtensions { get; set; } = [
        ".bak",
        ".bacpac",
        ".zip",
        ".rar",
        ".7z",
        ".tar",
        ".gz",
        ".tgz",
        ".bz2",
        ".xz",
        ".cab"
    ];

    /// <summary>
    /// Gets the maximum upload size in bytes.
    /// </summary>
    public long MaxUploadSizeBytes => (long)MaxUploadSizeGB * 1024L * 1024L * 1024L;
}
