using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using FluentValidation;

namespace chatbot.Core.Validators.Notifications
{
    public class RegisterDeviceDtoValidator:AbstractValidator<RegisterDeviceDto>
    {
        public RegisterDeviceDtoValidator()
        {
            RuleFor(x => x.PushToken)
            .NotEmpty()
            .MaximumLength(1000);

            RuleFor(x => x.Provider)
                .IsInEnum();

            RuleFor(x => x.DeviceType)
                .IsInEnum();

            RuleFor(x => x.DeviceName)
                .MaximumLength(100);
        }
    }
}
