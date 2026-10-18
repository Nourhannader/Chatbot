using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class ForwardMessageDtoValidator
     : AbstractValidator<ForwardMessageDto>
    {
        public ForwardMessageDtoValidator()
        {
            RuleFor(x => x.MessageId)
                .NotEmpty();

            RuleFor(x => x.ConversationIds)
                .NotEmpty()
                .WithMessage(
                    "At least one target conversation is required.");

            RuleForEach(x => x.ConversationIds)
                .NotEmpty();

            RuleFor(x => x.ConversationIds)
                .Must(ids =>
                    ids.Distinct().Count()
                    == ids.Count)
                .WithMessage(
                    "Duplicate conversations are not allowed.");
        }
    }
}
