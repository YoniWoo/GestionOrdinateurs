var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("GestionOrdinateurs", client =>
{
    var adrese = builder.Configuration["Apis:GestionOrdinateurs"]
        ?? throw new InvalidOperationException(
            "L'adresse de l'API des gestion est manquante");

    client.BaseAddress = new Uri(adrese);
});

var cleJwt = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(cleJwt))
{
    throw new InvalidOperationException(
        "Jwt:Key ne doit pas être vide.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5083")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowBlazorClient");
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
