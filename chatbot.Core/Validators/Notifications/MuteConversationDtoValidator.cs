using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Notifications;
using FluentValidation;

namespace chatbot.Core.Validators.Notifications
{
    public class MuteConversationDtoValidator : AbstractValidator<MuteConversationDto>
    {
        public MuteConversationDtoValidator()
        {
            RuleFor(x => x.Duration)
                .IsInEnum()
                .WithMessage(
                    "Invalid mute duration.");
        }
    }
}
