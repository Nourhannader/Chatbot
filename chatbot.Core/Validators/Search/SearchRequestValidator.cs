using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using FluentValidation;

namespace chatbot.Core.Validators.Search
{
    public sealed class SearchRequestValidator  : AbstractValidator<SearchRequestDto>
    {
        public SearchRequestValidator()
        {
            RuleFor(x => x.Keyword)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x)
                .Must(x => !x.From.HasValue ||
                           !x.To.HasValue ||
                           x.From < x.To)
                .WithMessage("'From' must be earlier than 'To'.");
        }
    }
}
