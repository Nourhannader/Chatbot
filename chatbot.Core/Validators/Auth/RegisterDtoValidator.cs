using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Auth;
using FluentValidation;

namespace chatbot.Core.Validators.Auth
{
    public class RegisterDtoValidator: AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Firstname is required.")
            .MinimumLength(3)
            .WithMessage("Firstname must be at least 3 characters.")
            .MaximumLength(50)
            .WithMessage("Fisrtname cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Lastname is required.")
            .MinimumLength(3)
            .WithMessage("Lastname must be at least 3 characters.")
            .MaximumLength(50)
            .WithMessage("Lastname cannot exceed 50 characters.");


            RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50)
            .WithMessage("Username cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email address.");

            RuleFor(x => x.Password)
               .NotEmpty()
               .WithMessage("Password is required.")
               .MinimumLength(8)
               .WithMessage("Password must be at least 8 characters.")
               .Matches("[A-Z]")
               .WithMessage("Password must contain at least one uppercase letter.")
               .Matches("[a-z]")
               .WithMessage("Password must contain at least one lowercase letter.")
               .Matches("[0-9]")
               .WithMessage("Password must contain at least one number.")
               .Matches("[^a-zA-Z0-9]")
               .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

        }
    }
}
