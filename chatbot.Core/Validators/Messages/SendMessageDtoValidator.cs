using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Enums;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class SendMessageDtoValidator : AbstractValidator<SendMessageDto>
    {
        public SendMessageDtoValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(5000);

            RuleFor(x => x.MessageType)
                .IsInEnum();

            RuleFor(x => x)
                .Must(x =>
                    x.ReplyToMessageId != Guid.Empty)
                .When(x =>
                    x.ReplyToMessageId.HasValue)
                .WithMessage(
                    "Invalid ReplyToMessageId.");
        }
    }
}
