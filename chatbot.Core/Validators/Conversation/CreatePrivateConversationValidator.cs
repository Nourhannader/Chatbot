using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using FluentValidation;

namespace chatbot.Core.Validators.Conversation
{
    public class CreatePrivateConversationValidator:AbstractValidator<CreatePrivateConversationDto>
    {
        public CreatePrivateConversationValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User id is required");
        }
    }
}
