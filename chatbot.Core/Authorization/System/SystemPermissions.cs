using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Authorization.System
{
    public static class SystemPermissions
    {
        public const string UsersView ="system.users.view";

        public const string UsersManage ="system.users.manage";

        public const string UsersBlock ="system.users.block";

        public const string ReportsView ="system.reports.view";

        public const string ReportsManage ="system.reports.manage";

        public const string RolesManage ="system.roles.manage";

        public const string SettingsManage ="system.settings.manage";
    }
}
