using System.ComponentModel.DataAnnotations;

namespace JiwaCustomerPortal.Models;

/// <summary>
/// Request body used to prepare a browser-to-blob upload.
/// </summary>
public sealed class PrepareUploadRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string? RootCompanyName { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false, ErrorMessage = "Support Ticket Number is required.")]
    public string SupportTicket { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}

/// <summary>
/// Response returned after SAS generation has been prepared.
/// </summary>
public sealed class PrepareUploadResponse
{
    public Guid UploadId { get; set; }
    public string BlobUri { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime UploadDateUtc { get; set; }
}

/// <summary>
/// Finalization payload once the browser upload is complete.
/// </summary>
public sealed class CompleteUploadRequest
{
    public Guid UploadId { get; set; }
    public string BlobPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? CompanyName { get; set; }
    public string? RootCompanyName { get; set; }
    public string? ContactName { get; set; }
    public string? EmailAddress { get; set; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "Support Ticket Number is required.")]
    public string SupportTicket { get; set; } = string.Empty;
}

/// <summary>
/// Success payload returned after upload finalization.
/// </summary>
public sealed class CompleteUploadResponse
{
    public Guid UploadId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime UploadDateUtc { get; set; }
}
