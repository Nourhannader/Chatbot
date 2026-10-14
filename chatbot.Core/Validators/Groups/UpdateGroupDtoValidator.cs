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
    public class UpdateGroupDtoValidator:AbstractValidator<UpdateGroupDto>
    {
        private readonly IFileValidationService _validationService;
        public UpdateGroupDtoValidator(IFileValidationService validationService)
        {
            _validationService = validationService;

            RuleFor(x => x.Title)
            .Must(title =>
                title == null ||
                !string.IsNullOrWhiteSpace(title))
            .WithMessage(
                "Group title cannot be empty.");

            RuleFor(x => x.Title)
                .MaximumLength(100)
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => x.Description != null);

            RuleFor(x => x)
                .Must(x =>
                    !(x.Image != null &&
                      x.RemoveImage))
                .WithMessage(
                    "Cannot upload and remove the group image at the same time.");

            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.Image!)
                    .CustomAsync(async (image, context, cancellationToken) =>
                    {
                        await _validationService.ValidateImageAsync(image);
                    });
            });


            RuleFor(x => x.Description)
                 .MaximumLength(500)
                 .When(x => x.Description != null);
           
        }
    }
}
