using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace chatbot.Ef.Attribute
{
    public class UniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
            var dbContext = validationContext.GetService<ApplicationDbContext>();
            if (dbContext == null)
                throw new InvalidOperationException("DbContext not available");

            var email = value.ToString();
            var entityType = validationContext.ObjectType;
            var propertyName = validationContext.MemberName;

            var exists=dbContext.Users.Any(u => u.Email == email);
            if (exists)
                return new ValidationResult($"{propertyName} must be unique.");

            return ValidationResult.Success;
        }
    }
}
