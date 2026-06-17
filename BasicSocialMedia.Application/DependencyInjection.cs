using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Application.Mappers;
using BasicSocialMedia.Application.Services;
using BasicSocialMedia.Application.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BasicSocialMedia.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<ICurrentTime, CurrentTime>();
            services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, TokenGenerators>();
            services.AddScoped<TokenGenerators>();

            services.AddAutoMapper(configuration =>
            {
                configuration.AddProfile<MapperConfigurationsProfile>();
            });
            return services;
        }
    }
}
