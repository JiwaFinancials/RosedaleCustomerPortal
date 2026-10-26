using System.ComponentModel.DataAnnotations;
using JiwaCustomerPortal.Models;
using JiwaCustomerPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace JiwaCustomerPortal.Controllers;

/// <summary>
/// API endpoints for issuing SAS tokens and completing direct browser uploads.
/// </summary>
[ApiController]
[Route("api/uploads")]
public sealed class UploadController : ControllerBase
{
    private readonly UploadPortalService _uploadPortalService;
    private readonly ILogger<UploadController> _logger;

    /// <summary>
    /// Creates a new upload controller.
    /// </summary>
    /// <param name="uploadPortalService">Upload portal service.</param>
    /// <param name="logger">Logger instance.</param>
    public UploadController(UploadPortalService uploadPortalService, ILogger<UploadController> logger)
    {
        _uploadPortalService = uploadPortalService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a database record and SAS token for a browser-initiated upload.
    /// </summary>
    /// <param name="request">The upload request payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Prepared upload session information.</returns>
    [HttpPost("prepare")]
    public async Task<ActionResult<PrepareUploadResponse>> PrepareAsync([FromBody] PrepareUploadRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _uploadPortalService.PrepareUploadAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Upload validation failed.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Preparing a direct upload failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to prepare the upload request. Please try again." });
        }
    }

    /// <summary>
    /// Marks the browser upload as complete and updates blob metadata.
    /// </summary>
    /// <param name="request">Upload completion payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload summary.</returns>
    [HttpPost("complete")]
    public async Task<ActionResult<CompleteUploadResponse>> CompleteAsync([FromBody] CompleteUploadRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _uploadPortalService.CompleteUploadAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Upload completion validation failed.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Uploading content to Azure Storage failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "The upload finished but could not be recorded. Please contact support." });
        }
    }
}
