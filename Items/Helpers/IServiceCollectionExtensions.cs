using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Items.Abstractions.Queries.Factories;
using Items.Queries.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Items.Helpers;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IItemQueryHandlerFactory, ItemQueryHandlerFactory>()
            .AddTransient<IItemsPageQueryHandlerFactory, ItemsPageQueryHandlerFactory>()
            .AddTransient<IItemListQueryHandlerFactory, ItemListQueryHandlerFactory>()
            .AddTransient<ICategoriesQueryHandlerFactory, CategoriesQueryHandlerFactory>()
            .AddTransient<IOrdersQueryHanlderFactory, OrdersQueryHandlerFactory>();

        return serviceCollection;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key must be specified.");

        serviceCollection
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        return serviceCollection;
    }

    public static IServiceCollection AddConfiguredSwaggerGen(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = Assembly.GetExecutingAssembly().GetName().Name,
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = """
                    JWT Authorization header using the Bearer scheme.
                    Enter 'Bearer' [space] and then your token in the text input below.
                    Example: "Bearer ***.***.***"
                    """,
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        serviceCollection.ConfigureSwaggerGen(opt =>
        {
            opt.UseOneOfForPolymorphism();

            opt.SelectDiscriminatorNameUsing(_ => "$type");
            opt.SelectDiscriminatorValueUsing(subType => subType.BaseType!
                .GetCustomAttributes<JsonDerivedTypeAttribute>()
                .FirstOrDefault(x => x.DerivedType == subType)?
                .TypeDiscriminator!.ToString());
        });

        return serviceCollection;
    }
}
