
using System.Text;
using chatbot.Api.Extensions;
using chatbot.Api.Hubs;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using chatbot.Ef.Repositories;
using chatbot.Ef.Services;
using chatbot.Ef.UnitOfWork;
using chatbot.Ef.ValidatorService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace chatbot.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationServices(builder.Configuration);


            builder.Services.AddSignalR();

            
        
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            //add jwtAuthentication
            builder.Services.AddJWTConfiguration(builder.Configuration);
              

            builder.Services.AddControllers();
            // Swagger Services
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //enable swagger only in development environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRateLimiter();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("AllowAll");
           
            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.MapHub<ChatHub>("/chathub");

            app.MapControllers();

            app.Run();
        }
    }
}
