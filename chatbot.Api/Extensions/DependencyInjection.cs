using chatbot.Api.BackgroundServices;
using chatbot.Api.Hubs;
using chatbot.Api.Middlewares;
using chatbot.Api.Services;
using chatbot.Core.Authorization;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Events.Messages;
using chatbot.Core.Events.Notifications;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Outbox;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.Helper;
using chatbot.Core.Interfaces.Services.Media;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Mapping;
using chatbot.Core.Models;
using chatbot.Core.Validators.Auth;
using chatbot.Ef.Authorization;
using chatbot.Ef.Data;
using chatbot.Ef.Events.Handlers.Conversations;
using chatbot.Ef.Events.Handlers.Messages;
using chatbot.Ef.Events.Handlers.Notifications;
using chatbot.Ef.Events.Publishing;
using chatbot.Ef.Jobs;
using chatbot.Ef.Outbox;
using chatbot.Ef.Repositories;
using chatbot.Ef.Services;
using chatbot.Ef.Services.Helper;
using chatbot.Ef.Services.Media;
using chatbot.Ef.Services.Providers;
using chatbot.Ef.UnitOfWork;
using chatbot.Ef.ValidatorService;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceStack;
using StackExchange.Redis;


namespace chatbot.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services,IConfiguration Configuration)
        {
            Services.AddHttpContextAccessor();
            
            Services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis"))!);

            Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            Services.AddProblemDetails();
            Services.AddValidatorsFromAssemblyContaining<SendMessageValidator>();
            Services.AddTransient<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IAuthRepository, AuthRepository>();
            Services.AddScoped<IAuthService, AuthService>();
            Services.AddScoped<IJwtService, JwtService>();
            Services.AddScoped<IUserService, UserService>();
            Services.AddScoped<IMailService, MailService>();
            Services.AddScoped<IChatService, ChatService>();
            Services.AddScoped<IMessageService, MessageService>();
            Services.AddScoped<IMessageReactionService,MessageReactionService>();
            Services.AddScoped<IMessageEncryptionService, MessageEncryptionService>();  
            Services.AddScoped<IConversationService, ConversationService>();
            Services.AddScoped<IRolePermissionService,RolePermissionService>();
            Services.AddScoped<IConversationAuthorizationService,ConversationAuthorizationService>();
            Services.AddScoped<IAuthorizationHandler, SystemPermissionHandler>();
            Services.AddScoped<IAuthorizationHandler,ConversationPermissionHandler>();
            Services.AddScoped<IGroupService, GroupService>();
            Services.AddScoped<IGroupInviteService, GroupInviteService>();
            Services.AddScoped<ISystemMessageService, SystemMessageService>();
            Services.AddScoped<IConversationSettingsService, ConversationSettingsService>();
            Services.AddScoped<IUserDeviceService, UserDeviceService>();
            Services.AddScoped<INotificationService,NotificationService>();
            Services.AddScoped<INotificationDecisionService, NotificationDecisionService>();
            Services.AddScoped<IPushNotificationFactory, PushNotificationFactory>();
            Services.AddScoped<FirebaseNotificationService>();
            Services.AddHttpClient<OneSignalPushNotificationService>();
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
            Services.AddScoped<IRealtimeNotificationService, SignalRNotificationService>();
            Services.AddSingleton<TypingRepository>();
            Services.AddScoped<IUserConnectionService, UserConnectionService>();
            Services.AddScoped<IFfmpegService,FfmpegService>();
            Services.AddScoped< ITemporaryFileService, TemporaryFileService>();
            Services.AddScoped<IImageProcessorService,ImageProcessorService>();
            Services.AddScoped<IVideoProcessorService,VideoProcessorService>();
            Services.AddScoped< IAudioProcessorService,AudioProcessorService>();
            Services.AddScoped<IFileProcessorFactory,FileProcessorFactory>();
            Services.AddScoped<ICurrentUser, CurrentUser>();


            //outbox
            Services.AddScoped< IEventPublisher, EventPublisher>();
            
            Services.AddScoped< IOutboxProcessor,OutboxProcessor>();

            Services.AddScoped<IEventHandler<MessageCreatedEvent>, MessageCreatedSignalRHandler>();

            Services.AddScoped<IEventHandler<MessageCreatedEvent>,MessageCreatedNotificationHandler>();

            Services.AddScoped< IEventHandler<MessageEditedEvent>, MessageEditedSearchHandler>();

            Services.AddScoped< IEventHandler<MessageDeletedEvent>,MessageDeletedSearchHandler>();

            Services.AddScoped<IEventHandler<MessageCreatedEvent>, MessageCreatedSignalRHandler>();

            Services.AddScoped<IEventHandler<MessageCreatedEvent>,MessageCreatedSearchHandler>();

            Services.AddScoped<IEventHandler<MessageCreatedEvent>,MessageCreatedNotificationHandler>();

            Services.AddScoped<IEventHandler<MessageEditedEvent>, MessageEditedSignalRHandler>();

            Services.AddScoped< IEventHandler<MessageEditedEvent>,MessageEditedSearchHandler>();

            Services.AddScoped<IEventHandler<MessageDeletedEvent>,MessageDeletedSignalRHandler>();

            Services.AddScoped<IEventHandler<MessageDeletedEvent>, MessageDeletedSearchHandler>();

            Services.AddScoped<IEventHandler<MessageReactedEvent>,MessageReactedSignalRHandler>();

            Services.AddScoped<IEventHandler<MessageReactedEvent>, MessageReactedNotificationHandler>();

            Services.AddScoped<IEventHandler<MessageReplyCreatedEvent>, MessageReplyNotificationHandler>();

            Services.AddScoped<IEventHandler<MessageForwardedEvent>,MessageForwardedSignalRHandler>();

            Services.AddScoped<IEventHandler<MessageRecipientStatusChangedEvent>,MessageRecipientStatusChangedSignalRHandler>();

            Services.AddScoped<IEventHandler<ConversationCreatedEvent>, ConversationCreatedSignalRHandler>();

            Services.AddScoped<IEventHandler<MemberAddedEvent>, MemberAddedSignalRHandler>();

            Services.AddScoped<IEventHandler<MemberAddedEvent>,MemberAddedNotificationHandler>();

            Services.AddScoped<IEventHandler<MemberRemovedEvent>, MemberRemovedSignalRHandler>();

            Services.AddScoped<IEventHandler<MemberRemovedEvent>, MemberRemovedNotificationHandler>();

            Services.AddScoped<IEventHandler<MemberRoleChangedEvent>, MemberRoleChangedSignalRHandler>();

            Services.AddScoped<IEventHandler<MemberRoleChangedEvent>, MemberRoleChangedNotificationHandler>();

            Services.AddScoped<IEventHandler<NotificationCreatedEvent>, NotificationCreatedSignalRHandler>();

            Services.AddScoped<IEventHandler<NotificationCreatedEvent>, NotificationCreatedPushHandler>();

            Services.AddHostedService< OutboxBackgroundService>();

            Services.AddSingleton<IEventTypeRegistry, EventTypeRegistry>();

            Services.AddSingleton<OutboxMessageFactory>();


            //jobSchedular
            Services.AddScoped<FileCleanupJob>();
            Services.AddScoped<NotificationCleanupJob>();
            Services.AddScoped<SessionCleanupJob>();
            Services.AddScoped<MessageCleanupJob>();

            
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
