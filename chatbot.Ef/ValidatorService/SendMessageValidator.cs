using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using FluentValidation;

namespace chatbot.Ef.ValidatorService
{
    public class SendMessageValidator:AbstractValidator<SendMessageDto>
    {
        public SendMessageValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(4000);
            RuleFor(x => x.ConversationId)
                .NotEmpty();

        }
    }
}
