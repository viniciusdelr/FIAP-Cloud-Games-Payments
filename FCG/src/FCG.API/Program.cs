using FCG.Infrastructure.Data;
using JWT_Example;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
//using static System.Runtime.InteropServices.JavaScript.JSType;
using FCG.Middlewares;
using Serilog;
using System;
using Microsoft.OpenApi.Models;
using MassTransit;
using FCG.API.Consumers;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))

    };

    options.Events = new JwtBearerEvents
    {
        
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                mensagem = "Token inválido ou ausente."
            });
            return context.Response.WriteAsync(result);
        },

        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                mensagem = "Você não possui permissão para esta ação. Contate um administrador."
            });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT assim: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.WebHost.UseUrls("http://0.0.0.0:80");

//builder.Services.AddMassTransit(x =>
//{
//    // 1. Registra o consumidor
//    x.AddConsumer<UserCreatedConsumer>();

//    x.UsingAzureServiceBus((context, cfg) =>
//    {
//        cfg.Host(builder.Configuration.GetConnectionString("ServiceBusConnection"));

//        cfg.ReceiveEndpoint("user-created-queue", e =>
//        {
//            e.ConfigureConsumer<UserCreatedConsumer>(context);
//        });
//    });
//});

//builder.Services.AddMassTransit(x => {
//    x.AddConsumer<GamePurchasedConsumer>(); // O Payments ouve o pedido de compra
//    x.UsingAzureServiceBus((context, cfg) => {
//        cfg.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));
//        cfg.ConfigureEndpoints(context);
//    });
//});

builder.Services.AddMassTransit(x =>
{
    // 1. Registre TODOS os seus consumidores aqui em cima
    x.AddConsumer<UserCreatedConsumer>();
    x.AddConsumer<GamePurchasedConsumer>();

    x.UsingAzureServiceBus((context, cfg) =>
    {
        // Use apenas uma string de conexão (garanta que o nome no appsettings seja igual)
        cfg.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));

        // 2. Configuração Automática (Recomendado)
        // O ConfigureEndpoints cria as filas automaticamente para todos os consumers registrados acima
        cfg.ConfigureEndpoints(context);

        // 3. Configuração Manual (Opcional)
        // Se você quiser dar um nome específico para a fila do usuário, pode manter isso:
        cfg.ReceiveEndpoint("user-created-queue", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
        });
    });
});
var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        if (httpReq.Headers.ContainsKey("X-Forwarded-Host"))
        {
            var serverUrl = $"{httpReq.Scheme}://{httpReq.Headers["X-Forwarded-Host"]}/payments";
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
        }
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("./v1/swagger.json", "API Users V1");
});

app.UseMiddleware<ExceptionMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();

        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao rodar as migrations.");
    }
}

app.Run();

