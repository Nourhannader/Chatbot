using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using FluentValidation;

namespace chatbot.Core.Validators.Groups
{
    public class AddMembersValidator : AbstractValidator<AddMembersDto>
    {
        public AddMembersValidator()
        {
            RuleFor(x => x.UserIds)
                .NotEmpty()
                .WithMessage(
                    "At least one user is required.");

            RuleFor(x => x.UserIds)
                .Must(ids =>
                    ids.Distinct().Count() ==
                    ids.Count)
                .WithMessage(
                    "Duplicate users are not allowed.");

            RuleForEach(x => x.UserIds)
                .NotEmpty()
                .WithMessage(
                    "Invalid user id.");
        }
    }
}
