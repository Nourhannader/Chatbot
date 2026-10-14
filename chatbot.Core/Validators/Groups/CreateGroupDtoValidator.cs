using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Validators;
using FluentValidation;

namespace chatbot.Core.Validators.Groups
{
    public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
    {
        private readonly IFileValidationService _validationService;

        public CreateGroupDtoValidator(IFileValidationService validationService)
        {
            _validationService = validationService;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Group title is required.")
                .MaximumLength(100)
                .WithMessage("Group title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.MemberIds)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("Duplicate members are not allowed.");

            RuleForEach(x => x.MemberIds)
                .NotEmpty()
                .WithMessage("Invalid member id.");

            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.Image!)
                    .CustomAsync(async (image, context, cancellationToken) =>
                    {
                        await _validationService.ValidateImageAsync(image);
                    });
            });
        }
    }
}
