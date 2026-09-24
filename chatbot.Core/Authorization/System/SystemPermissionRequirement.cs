using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace chatbot.Core.Authorization.System
{
    public class SystemPermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public SystemPermissionRequirement(
            string permission)
        {
            Permission = permission;
        }
    }
}
