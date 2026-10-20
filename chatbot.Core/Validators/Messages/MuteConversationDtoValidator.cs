using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class MuteConversationDtoValidator : AbstractValidator<MuteConversationDto>
    {
        public MuteConversationDtoValidator()
        {
            RuleFor(x => x.Duration)
                .IsInEnum();
        }
    }
}
