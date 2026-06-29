using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CoreBanking.Application.Common.Behaviors;
using CoreBanking.Infrastructure;
using CoreBanking.Infrastructure.Identity;
using CoreBanking.Application.Accounts.Commands;
using System.Text.Json.Serialization;
using CoreBanking.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using StackExchange.Redis;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
    options.OperationFilter<BearerSecurityOperationFilter>();
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateAccountCommand).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "CoreBanking_";
});

builder.Services.AddSingleton<IDistributedLockFactory>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("Redis") ??
                           throw new InvalidOperationException("Redis Connection string is not configured!");;

    // Singleton Redis connection
    var multiplexer = ConnectionMultiplexer.Connect(connectionString);

    // RedLock with single instance (only for development!)
    return RedLockFactory.Create(new List<RedLockMultiplexer>
    {
        new RedLockMultiplexer(multiplexer)
    });
});
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var bankingDb = scope.ServiceProvider.GetRequiredService<CoreBanking.Infrastructure.Persistence.BankingDbContext>();
    bankingDb.Database.EnsureCreated();

    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    identityDb.Database.EnsureCreated();

    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.UseCors();

app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;
        
        var (statusCode, message) = exception switch 
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            BadRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
        
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { error = message });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class BearerSecurityOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(Microsoft.OpenApi.OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        operation.Security ??= [];
        operation.Security.Add(new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", context.Document),
                new List<string>()
            }
        });
    }
}
