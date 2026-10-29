using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.API.Data;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestionOrdinateurs",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {token}"
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.MetadataAddress =
            builder.Configuration["Keycloak:MetadataAddress"];

        options.Audience = builder.Configuration["Keycloak:Audience"];

        options.RequireHttpsMetadata = false;

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Keycloak:Authority"],
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            NameClaimType = "preferred_username",
            RoleClaimType = "role"
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Authentification");

                logger.LogInformation(
                    "Token valide pour {Utilisateur}",
                    context.Principal?.Identity?.Name);

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Authentification");

                logger.LogWarning(
                    context.Exception,
                    "Échec de la validation du token");

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContextFactory<ParcInformatiqueContext>();

builder.Services.AddScoped<ReparationService>();
builder.Services.AddSingleton<StockService>();
builder.Services.AddSingleton<JournalService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5083", "http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ParcInformatiqueContext>();

    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User
            {
                Login = "User1",
                Role = RoleEnum.Admin
            },
            new User
            {
                Login = "User2",
                Role = RoleEnum.Gestionnaire
            },
            new User
            {
                Login = "User3",
                Role = RoleEnum.Gestionnaire
            }
        );
        db.SaveChanges();
    }
}

app.UseRouting();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();