using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Notifications;
using FluentValidation;

namespace chatbot.Core.Validators.Notifications
{
    public class RegisterDeviceDtoValidator:AbstractValidator<RegisterDeviceDto>
    {
        public RegisterDeviceDtoValidator()
        {
            RuleFor(x => x.PushToken)
                .NotEmpty()
                .WithMessage(
                    "Push token is required.")
                .MaximumLength(2000)
                .WithMessage(
                    "Push token must not exceed 2000 characters.");


            RuleFor(x => x.DeviceType)
                .IsInEnum()
                .WithMessage(
                    "Invalid device type.");


            RuleFor(x => x.Provider)
                .IsInEnum()
                .WithMessage(
                    "Invalid push provider.");


            RuleFor(x => x.DeviceName)
                .MaximumLength(200)
                .WithMessage(
                    "Device name must not exceed 200 characters.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(
                        x.DeviceName));
        }
    }
}
