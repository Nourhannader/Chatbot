using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Authorization.System
{
    public static class SystemRolePermissions
    {
        public static readonly Dictionary<string, HashSet<string>> Permissions = new()
        {
            [SystemRoles.User] = new()
            {
            },

            [SystemRoles.Admin] = new()
        {
            SystemPermissions.UsersView,
            SystemPermissions.UsersManage,
            SystemPermissions.UsersBlock,

            SystemPermissions.ReportsView
        },

            [SystemRoles.SuperAdmin] = new()
        {
            SystemPermissions.UsersView,
            SystemPermissions.UsersManage,
            SystemPermissions.UsersBlock,

            SystemPermissions.ReportsView,
            SystemPermissions.ReportsManage,

            SystemPermissions.RolesManage,
            SystemPermissions.SettingsManage
        }
        };
    }
}
