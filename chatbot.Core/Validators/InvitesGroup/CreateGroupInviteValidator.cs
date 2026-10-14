using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.InviteGroup;
using FluentValidation;

namespace chatbot.Core.Validators.InvitesGroup
{
    public class CreateGroupInviteValidator:AbstractValidator<CreateGroupInviteDto>
    {
        public CreateGroupInviteValidator(TimeProvider timeProvider)
        {
            RuleFor(x => x.ExpiresAt)
                .Must(value =>
                    !value.HasValue ||value.Value >
                    timeProvider.GetUtcNow().UtcDateTime)
                .WithMessage( "Expiration date must be in the future.");

            RuleFor(x => x.MaxUses)
                .GreaterThan(0)
                .When(x =>x.MaxUses.HasValue);

            RuleFor(x => x.MaxUses)
                .LessThanOrEqualTo(1000)
                .When(x => x.MaxUses.HasValue);
        }
    }
}
