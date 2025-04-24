using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace RO.DevTest.Domain.Exception;

/// <summary>
/// Represents the base for exceptions that return an HTTP status code
/// </summary>
public abstract class ApiException : System.Exception
{
    /// <summary>
    /// The <see cref="HttpStatusCode"/> of the <see cref="ApiException"/>
    /// </summary>
    public abstract HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Simple list of raw error messages
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Structured semantic errors for client consumption
    /// </summary>
    public List<ApiError> SemanticErrors { get; set; } = [];

    protected ApiException() : base() { }

    protected ApiException(string error) : this()
    {
        Errors = new List<string> { error };
        SemanticErrors = new List<ApiError> { new ApiError(error) };
    }

    /// <summary>
    /// Initializes a new <see cref="ApiException"/> from a <see cref="ValidationResult"/>
    /// </summary>
    /// <param name="validationResult">The <see cref="ValidationResult"/> that caused the exception</param>
    protected ApiException(ValidationResult validationResult) : this()
    {
        Errors = ExtractErrors(validationResult);
        SemanticErrors = ExtractSemanticErrors(validationResult);
    }

    /// <summary>
    /// Initializes a new <see cref="ApiException"/> from a <see cref="IdentityResult"/>
    /// </summary>
    /// <param name="identityResult">The <see cref="IdentityResult"/> that caused the exception</param>
    protected ApiException(IdentityResult identityResult) : this()
    {
        Errors = ExtractErrors(identityResult);
        SemanticErrors = ExtractSemanticErrors(identityResult);
    }

    protected static List<string> ExtractErrors(ValidationResult validationResult) =>
        validationResult.Errors.Select(e => e.ErrorMessage).ToList();

    protected static List<string> ExtractErrors(IdentityResult identityResult) =>
        identityResult.Errors.Select(e => e.Description).ToList();

    protected static List<ApiError> ExtractSemanticErrors(ValidationResult validationResult) =>
        validationResult.Errors.Select(e => new ApiError(e.ErrorMessage, null, e.PropertyName)).ToList();

    protected static List<ApiError> ExtractSemanticErrors(IdentityResult identityResult) =>
        identityResult.Errors.Select(e => new ApiError(e.Description, e.Code)).ToList();
}

/// <summary>
/// Represents a semantic API error with optional field and code
/// </summary>
/// <param name="Message">Error message</param>
/// <param name="Code">Optional error code</param>
/// <param name="Field">Field related to the error (optional)</param>
public record ApiError(string Message, string? Code = null, string? Field = null);
