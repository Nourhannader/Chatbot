using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.MessageStatus;
using FluentValidation;

namespace chatbot.Core.Validators.MessageStatus
{
    public class MarkReadDtoValidator : AbstractValidator<MarkReadDto>
    {
        public MarkReadDtoValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty();

            RuleFor(x => x.LastMessageId)
                .NotEmpty();
        }
    }
}
