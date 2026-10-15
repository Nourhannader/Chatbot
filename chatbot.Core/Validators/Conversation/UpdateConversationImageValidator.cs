using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using FluentValidation;

namespace chatbot.Core.Validators.Conversation
{
    public sealed class UpdateConversationImageValidator
    : AbstractValidator<UpdateConversationImageDto>
    {
        public UpdateConversationImageValidator()
        {
            RuleFor(x => x.Image)
                .NotNull();

            RuleFor(x => x.Image.Length)
                .GreaterThan(0)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .When(x => x.Image is not null);
        }
    }
}
