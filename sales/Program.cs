using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Sales.Api.Data;
using Sales.Api.Services;
using Serilog;
using Inventory.Api.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
          .WriteTo.File("logs/sales-log-.txt", rollingInterval: RollingInterval.Day);
});

var configuration = builder.Configuration;

builder.Services.AddDbContext<SalesDbContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHttpClient("inventory"); 
builder.Services.AddScoped<PedidoService>();

var jwtKey = configuration["Jwt:Key"] ?? "CHAVE_SUPER_SECRETA_FALLBACK";
var jwtIssuer = configuration["Jwt:Issuer"] ?? "sales.local";

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
    try
    {
        db.Database.Migrate();
        Log.Information("Migrações aplicadas com sucesso no banco de dados SalesDbContext.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Erro ao aplicar migrações no SalesDbContext.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
