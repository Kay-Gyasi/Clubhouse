using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Clubhouse.Business.Contracts.Requests;

public class CreateMemberRequest
{
    public string Username { get; set; }
    public UserType Type { get; set; } = UserType.Customer;

    [Phone]
    public string PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string Password { get; set; }
}

public enum UserType
{
    Manager,
    BackOfficer,
    Customer
}

public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequest>
{
    public CreateMemberRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("username is required")
            .NotNull();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("phoneNumber is required")
            .NotNull();
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("password is required")
            .NotNull();
    }
}