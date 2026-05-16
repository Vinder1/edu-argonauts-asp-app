// Validators/SignInRequestValidator.cs

using Argonauts.Web.Requests;
using FluentValidation;

namespace Argonauts.Web.Validation;

/// <summary>
/// Валидатор для запроса регистрации/входа пользователя.
/// Проверяет корректность имени, логина и пароля.
/// </summary>
public class SignInRequestValidator : AbstractValidator<SignInRequest>
{

    /// <summary>
    /// 
    /// </summary>
    public SignInRequestValidator()
    {
        RuleFor(x => x.Name) // Остановить цепочку после первой ошибки для этого поля
            .NotEmpty()
                .WithMessage("Имя пользователя обязательно")
            .MinimumLength(2)
                .WithMessage("Имя должно содержать минимум 2 символа")
            .MaximumLength(50)
                .WithMessage("Имя не должно превышать 50 символов")
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ0-9\s\-_']+$")
                .WithMessage("Имя может содержать только буквы, цифры, пробелы, дефис, подчёркивание и апостроф")
            .WithName("Имя");  // Отображаемое имя поля в сообщениях об ошибках

        RuleFor(x => x.Login)
            .NotEmpty()
                .WithMessage("Логин обязателен")
            .MinimumLength(3)
                .WithMessage("Логин должен содержать минимум 3 символа")
            .MaximumLength(30)
                .WithMessage("Логин не должен превышать 30 символов")
            .Matches(@"^[a-zA-Z0-9_]+$")
                .WithMessage("Логин может содержать только латинские буквы, цифры и подчёркивание")
            .WithName("Логин");

        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email обязателен")
            .EmailAddress()
                .WithMessage("Некорректный email адрес")
            .MaximumLength(254)
                .WithMessage("Email не должен превышать 254 символа")
            .WithName("Email");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Пароль обязателен")
            .MinimumLength(8)
                .WithMessage("Пароль должен содержать минимум 8 символов")
            .MaximumLength(100)
                .WithMessage("Пароль не должен превышать 100 символов")
            .Matches(@"[A-Z]+")
                .WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Matches(@"[a-z]+")
                .WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Matches(@"[0-9]+")
                .WithMessage("Пароль должен содержать хотя бы одну цифру")
            .WithName("Пароль");
    }
}