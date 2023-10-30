using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Clubhouse.Business.Contracts.Requests;

public class UpdateMemberRequest
{
    public string Id { get; set; }   
    public string Username { get; set; }
    public UserType Type { get; set; } = UserType.Customer;

    [Phone]
    public string PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}

public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequest>
{
    public UpdateMemberRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("username is required")
            .NotNull();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("phoneNumber is required")
            .NotNull();
    }
}