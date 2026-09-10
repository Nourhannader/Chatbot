using chatbot.Api.Middlewares;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;
using chatbot.Ef.Background;
using chatbot.Ef.Data;
using chatbot.Ef.Repositories;
using chatbot.Ef.Services;
using chatbot.Ef.Services.Providers;
using chatbot.Ef.UnitOfWork;
using chatbot.Ef.ValidatorService;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using ServiceStack;
using StackExchange.Redis;


namespace chatbot.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services,IConfiguration Configuration)
        {
            
            Services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis"))!);

            Services.AddExceptionHandler<GlobalExceptionHandler>();
            Services.AddProblemDetails();
            Services.AddValidatorsFromAssemblyContaining<SendMessageValidator>();
            Services.AddTransient<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IAuthRepository, AuthRepository>();
            Services.AddScoped<IAuthService, AuthService>();
            Services.AddScoped<IJwtService, JwtService>();
            Services.AddScoped<IMailService, MailService>();
            Services.AddScoped<IChatService, ChatService>();
            Services.AddScoped<IMessageService, MessageService>();
            Services.AddScoped<IMessageEncryptionService, MessageEncryptionService>();  
            Services.AddScoped<IConversationService, ConversationService>();
            Services.AddScoped<IConversationSettingsService, ConversationSettingsService>();
            Services.AddScoped<IReactionService, ReactionService>();
            Services.AddScoped<IUserDeviceService, UserDeviceService>();
            Services.AddScoped<INotificationService,NotificationService>();
            Services.AddScoped<IPushNotificationService, FirebaseNotificationService>();
            Services.AddScoped<IPresenceService, PresenceService>();
            Services.AddScoped<IMessageStatusService, MessageStatusService>();
            Services.AddScoped<ITypingService, TypingService>();
            Services.AddScoped<IForwardService, ForwardService>();
            Services.AddScoped<ISearchService, SearchService>();
            Services.AddScoped<ICacheService, RedisCacheService>();
            Services.AddScoped<IStorageService, StorageService>();
            Services.AddScoped<IFileValidationService, FileValidationService>();
            Services.AddScoped<IFileProcessorService, ImageProcessorService>();
            Services.AddScoped<IStorageProvider, LocalStorageProvider>();
            Services.AddScoped<IStorageProvider, AzureBlobStorageProvider>();
            Services.AddScoped<IStorageService, StorageService>();
            Services.AddScoped<IVoiceNoteService, VoiceNoteService>();
            Services.AddScoped<IMediaMessageService, MediaMessageService>();
            Services.AddScoped<IChunkUploadService, ChunkUploadService>();
            Services.AddScoped<IStickerService, StickerService>();
            Services.AddScoped<ITokenHashService,TokenHashService>();
            Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            Services.AddSingleton<TypingRepository>();


            //background services
            Services.AddHostedService<FileCleanupBackgroundService>();
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
            //rate limiting
            Services.AddRateLimiter( options =>
            {
                options.AddFixedWindowLimiter(
                  "chat",
                  limiterOptions =>
                  {
                   limiterOptions.PermitLimit = 30;

                   limiterOptions.Window =  TimeSpan.FromMinutes(1);

                   limiterOptions.QueueLimit = 0;
                  });
                //login rate limit
                options.AddFixedWindowLimiter(
                  "login",
                  limiterOptions =>
                  {
                   limiterOptions.PermitLimit = 5;

                   limiterOptions.Window =TimeSpan.FromMinutes(1);

                   limiterOptions.QueueLimit = 0;
                  });
            });


            return Services;
        }
    }
}
