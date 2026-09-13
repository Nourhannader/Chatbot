using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.UnitOFWork;

namespace chatbot.Ef.Services.Helper
{
    public class SystemMessageHelper
    {
        private readonly IUnitOfWork unitOfWork;

        public SystemMessageHelper(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // Change the access modifier to public to fix CS0122  
        public async Task<string> BuildContentAsync(SystemMessageType type, Guid? actorId, Guid? targetUserId)
        {
            // Implementation of the method  
            return await Task.FromResult("Content based on type and IDs");
        }

        public async Task<string> GetUserNameAsync(Guid? userId)
        {
            // Implementation of the method
            if (!userId.HasValue)
                return "Someone";

            var user = await unitOfWork.Auth.GetByIdAsync(userId.Value);

            if (user == null)
                return "Someone";

            return string.IsNullOrWhiteSpace(user.UserName)
                ? "Someone"
                : user.UserName;
        }
    }
}
