using Microsoft.OpenApi.Models;

namespace Talabat.API.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Talabat.API", Version = "v1" });

                var securitySchema = new OpenApiSecurityScheme()
                {
                    Description = "JWT Auth Bearer Schema",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityReqirement = new OpenApiSecurityRequirement() { { securitySchema, new[] { "Bearer" } } };

                c.AddSecurityRequirement(securityReqirement);
            });

            return services;
        }
    }
}
