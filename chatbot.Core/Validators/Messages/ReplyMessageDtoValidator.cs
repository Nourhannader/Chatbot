using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class ReplyMessageDtoValidator : AbstractValidator<ReplyMessageDto>
    {
        public ReplyMessageDtoValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty();

            RuleFor(x => x.MessageId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(5000);

            RuleFor(x => x.Type)
                .IsInEnum();
        }
    }
}
