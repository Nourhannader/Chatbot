using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using chatbot.Ef.Repositories;
using chatbot.Ef.Services;
using chatbot.Ef.UnitOfWork;
using chatbot.Ef.ValidatorService;
using Microsoft.AspNetCore.Identity;


namespace chatbot.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services,IConfiguration Configuration)
        {
            Services.Configure<JwtSettings>(Configuration.GetSection("JWT"));
            Services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            Services.AddTransient<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IAuthRepository, AuthRepository>();
            Services.AddScoped<IAuthService, AuthService>();
            Services.AddScoped<IJwtService, JwtService>();
            Services.AddScoped<IMailService, MailService>();
            Services.AddScoped<IChatService, ChatService>();
            Services.AddScoped<IMessageService, MessageService>();
            Services.AddScoped<IConversationService, ConversationService>();
            Services.AddScoped<IReactionService, ReactionService>();
            Services.AddScoped<IUserDeviceService, UserDeviceService>();
            Services.AddScoped<INotificationsService, NotificationsService>();
            Services.AddScoped<IPresenceService, PresenceService>();
            Services.AddScoped<IMessageStatusService, MessageStatusService>();
            Services.AddScoped<ITypingService, TypingService>();
            Services.AddScoped<IForwardService, ForwardService>();
            Services.AddScoped<ISearchService, SearchService>();
            Services.AddScoped<IStorageService, LocalStorageService>();
            Services.AddScoped<IFileValidationService, FileValidationService>();

            return Services;
        }
    }
}
