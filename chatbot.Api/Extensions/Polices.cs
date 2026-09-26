using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Authorization.System;
using Microsoft.Extensions.Options;

namespace chatbot.Api.Extensions
{
    public static class Polices
    {
        public static IServiceCollection AddApplicationPolices(this IServiceCollection Services)
        {
            //Cros
            Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            //policies
            Services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("ChatUser",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
                    options.AddPolicy("AdminOnly",
                    policy =>
                    {
                        policy.RequireRole("Admin");
                    });

                });



            Services.AddAuthorization(
                options =>
                {
                    options.AddPolicy(
                        "Conversation.SendMessage",
                         policy =>
                         {
                             policy.RequireAuthenticatedUser();

                             policy.AddRequirements(
                          new PermissionRequirement(
                          ConversationPermissions.SendMessage));
                         });

                    options.AddPolicy(
                        "Conversation.DeleteMessage",
                        policy =>
                        {
                            policy.RequireAuthenticatedUser();

                            policy.AddRequirements(
                          new PermissionRequirement(
                              ConversationPermissions.DeleteMessage));
                        });

                    options.AddPolicy(
                       "Conversation.AddMember",
                        policy =>
                        {
                            policy.RequireAuthenticatedUser();

                            policy.AddRequirements(
                          new PermissionRequirement(
                              ConversationPermissions.AddMember));
                        });
                });

            //systempolicy
           Services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "UsersView",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(
                            new SystemPermissionRequirement(
                                SystemPermissions.UsersView));
                    });

                options.AddPolicy(
                    "UsersManage",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(
                            new SystemPermissionRequirement(
                                SystemPermissions.UsersManage));
                    });

                options.AddPolicy(
                    "UsersBlock",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(
                            new SystemPermissionRequirement(
                                SystemPermissions.UsersBlock));
                    });

                options.AddPolicy(
                    "ReportsView",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(
                            new SystemPermissionRequirement(
                                SystemPermissions.ReportsView));
                    });

                options.AddPolicy(
                    "ReportsManage",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(
                            new SystemPermissionRequirement(
                                SystemPermissions.ReportsManage));
                    });

                options.AddPolicy(
                    "RolesManage",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(
                            new SystemPermissionRequirement(
                                SystemPermissions.RolesManage));
                    });

                options.AddPolicy(
                    "SettingsManage",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(
                            new SystemPermissionRequirement(
                                SystemPermissions.SettingsManage));
                    });
            });
            return Services;
        }
    }
}
