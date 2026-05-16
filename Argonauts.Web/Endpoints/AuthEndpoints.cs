using Argonauts.Application.Dto;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Web.Auth.Abstractions;
using Argonauts.Web.Requests;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class AuthEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth").WithTags("Auth");

        group.MapPost("/sign-in", async (
            [FromServices] IAuthService authService,
            [FromServices] IPlayerService playerService,
            [FromServices] IEmailService emailService,
            HttpContext httpContext,
            IValidator<SignInRequest> validator,
            [FromServices] ILogger<AuthEndpoints> logger,
            SignInRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new SignInResponse
                {
                    Success = false,
                    Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)]
                });
            }
            if (await playerService.PlayerLoginExistsAsync(request.Login))
            {
                return Results.BadRequest(new SignInResponse
                {
                    Success = false,
                    Errors = ["Пользователь с таким логином уже существует"]
                });
            }

            logger.LogInformation("Attempt to sign up from {@Request}", request);

            var user = await authService.SignInNewPlayerAsync(
                name: request.Name, login: request.Login, email: request.Email, password: request.Password);

            var token = authService.GetJwtTokenForUser(user);

            await authService.SetRefreshToken(user, httpContext);

            await SendWelcomeEmailSafe(emailService, user, logger);

            return Results.Ok(new SignInResponse
            {
                Success = true,
                Token = token
            });
        }).WithSummary("Регистрация");

        group.MapPost("/log-in", async (
            [FromServices] IAuthService authService,
            HttpContext httpContext,
            IValidator<LogInRequest> validator,
            [FromServices] ILogger<AuthEndpoints> logger,
            LogInRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new SignInResponse
                {
                    Success = false,
                    Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)]
                });
            }

            logger.LogInformation("Attempt to log in from {@Request}", request);

            var user = await authService.LogInAsync(
                login: request.Login,
                password: request.Password);

            if (user == null)
            {
                return Results.BadRequest(new SignInResponse
                {
                    Success = false,
                    Errors = ["Пользователь не найден"]
                });
            }

            var token = authService.GetJwtTokenForUser(user);

            await authService.SetRefreshToken(user, httpContext);

            return Results.Ok(new SignInResponse
            {
                Success = true,
                Token = token
            });
        }).WithSummary("Вход");

        group.MapPost("/refresh", async (
            [FromServices] IAuthService authService,
            HttpContext httpContext,
            [FromServices] ILogger<AuthEndpoints> logger) =>
        {
            var refreshToken = httpContext.Request.Cookies["spaceApp_refreshToken"];
            if (refreshToken == null)
            {
                return Results.Unauthorized();
            }
            var user = authService.LogInWithRefreshTokenAsync(refreshToken);

            var token = authService.GetJwtTokenForUser(user);

            await authService.SetRefreshToken(user, httpContext);

            return Results.Ok(new RefreshResponse
            {
                Token = token
            });
        }).WithSummary("Обновление токена")
        .Produces(401);

        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            httpContext.Response.Cookies.Append("spaceApp_refreshToken", "", new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true
            });
            return Results.Ok();
        }).WithSummary("Выход");
    }

    private static async Task SendWelcomeEmailSafe(IEmailService emailService, UserDto user, ILogger<AuthEndpoints> logger)
    {
        try
        {
            await emailService.SendWelcomeEmailAsync(user.Email, user.Name);
            logger.LogInformation("Welcome email sent to {Email}", user.Email);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send welcome email to {Email}", user.Email);
        }
    }
}