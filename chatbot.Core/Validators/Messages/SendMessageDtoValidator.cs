using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class SendMessageDtoValidator: AbstractValidator<CreateMessageDto>
    {
        public SendMessageDtoValidator()
        {
            RuleFor(x => x.ConversationId)
            .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .When(x => x.Type == MessageType.Text);

            RuleFor(x => x.File)
                .NotNull()
                .When(x =>
                    x.Type == MessageType.Image ||
                    x.Type == MessageType.Video ||
                    x.Type == MessageType.Audio);
        }
    }
}
