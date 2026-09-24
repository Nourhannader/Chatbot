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
                    "Users.View",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.AddRequirements(
                            new SystemPermissionRequirement(
                                SystemPermissions.UsersView));
                    });

                options.AddPolicy(
                    "Users.Manage",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.AddRequirements(
                            new SystemPermissionRequirement(
                                SystemPermissions.UsersManage));
                    });
            });

            return Services;
        }
    }
}
