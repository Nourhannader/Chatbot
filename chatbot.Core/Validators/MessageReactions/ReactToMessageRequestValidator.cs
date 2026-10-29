using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Reactions;
using FluentValidation;

namespace chatbot.Core.Validators.MessageReactions
{
    public class ReactToMessageRequestValidator : AbstractValidator<ReactToMessageRequest>
    {
        public ReactToMessageRequestValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum();
        }
    }
}
