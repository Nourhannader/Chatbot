using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class EditMessageValidator: AbstractValidator<EditMessageDto>
    {
        public EditMessageValidator()
        {
            RuleFor(x => x.MessageId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(5000);
        }
    }

}
