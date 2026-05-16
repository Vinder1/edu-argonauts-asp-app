using Argonauts.Web.Validation;
using FluentValidation;

namespace Argonauts.Web.Extensions;

/// <summary>
/// 
/// </summary>
public static class ValidationConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<SignInRequestValidator>();
    }
}