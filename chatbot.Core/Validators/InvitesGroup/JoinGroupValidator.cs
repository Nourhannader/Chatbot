using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.InviteGroup;
using FluentValidation;

namespace chatbot.Core.Validators.InvitesGroup
{
    public class JoinGroupValidator:AbstractValidator<JoinGroupDto>
    {
        public JoinGroupValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
