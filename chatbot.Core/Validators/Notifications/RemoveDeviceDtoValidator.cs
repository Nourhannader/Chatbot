using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Notifications;
using FluentValidation;

namespace chatbot.Core.Validators.Notifications
{
    public class RemoveDeviceDtoValidator
    : AbstractValidator<RemoveDeviceDto>
    {
        public RemoveDeviceDtoValidator()
        {
            RuleFor(x => x.PushToken)
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(x => x.Provider)
                .IsInEnum();
        }
    }
}
