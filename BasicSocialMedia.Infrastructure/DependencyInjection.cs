using BasicSocialMedia.Application;
using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Application.Services;
using BasicSocialMedia.Application.Settings;
using BasicSocialMedia.Application.Settings.CloudinaryService;
using BasicSocialMedia.Infrastructure.Database;
using BasicSocialMedia.Infrastructure.Repositories;
using BasicSocialMedia.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BasicSocialMedia.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, IConfiguration configuration)
        {
            //UOW
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<IDirectMessageChatService, DirectMessageChatService>();
            services.AddScoped<IMessageService, MessageService>();

            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddSingleton<IEncryptionService, AesEncryptionService>();

            services.AddMemoryCache();
            services.AddLogging();

            // Repo
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<IDirectMessageChatRepository, DirectMessageChatRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();


            // Cloudinary
            services.Configure<CloudinarySetting>(configuration.GetSection("CloudinarySetting"));

            // Email Provider - Mailjet
            //services.Configure<SentEmailSettings>(
            //    configuration.GetSection(SentEmailSettings.SectionName));

            // AES Encryption
            services.Configure<EncryptionSettings>(
                configuration.GetSection(EncryptionSettings.SectionName));

            // Database Postgres
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );

            // Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!)
            );

            // Hangfire DB
            //services.AddHangfire(options =>
            //{
            //    options.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            //});


            return services;
        }
    }
}
