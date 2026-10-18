using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class AddReactionDtoValidator : AbstractValidator<AddReactionDto>
    {
        public AddReactionDtoValidator()
        {
            RuleFor(x => x.MessageId)
                .NotEmpty();

            RuleFor(x => x.ReactionType)
                .IsInEnum();
        }
    }
}
