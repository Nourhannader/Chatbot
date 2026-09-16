using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using FluentValidation;

namespace chatbot.Core.Validators.Groups
{
    public class CreateGroupDtoValidator:AbstractValidator<CreateGroupDto>
    {
        public CreateGroupDtoValidator()
        {
            RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Group title is required.")
            .MinimumLength(2)
            .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => x.Description != null);

            RuleFor(x => x.MemberIds)
                .NotNull();

            RuleFor(x => x.MemberIds.Count)
                .LessThanOrEqualTo(200)
                .WithMessage("A group cannot have more than 100 members.");
        }
    }
}
