using FirebaseAdmin;

namespace chatbot.Api.Extensions
{
    public static class FirebaseExtensions
    {
        public static IServiceCollection AddFirebase(this IServiceCollection services)
        {
            FirebaseApp.Create();

            return services;
        }
    }
}
