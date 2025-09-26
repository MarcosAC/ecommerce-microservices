using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Inventory.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
          .WriteTo.File("logs/inventory-log-.txt", rollingInterval: RollingInterval.Day);
});

var configuration = builder.Configuration;

builder.Services.AddDbContext<InventoryDbContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("Default")));

var jwtKey = configuration["Jwt:Key"] ?? "CHAVE_SUPER_SECRETA_FALLBACK";
var jwtIssuer = configuration["Jwt:Issuer"] ?? "inventory.local";
var jwtAudience = configuration["Jwt:Audience"] ?? jwtIssuer;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    try
    {
        db.Database.Migrate();
        Log.Information("Migrações aplicadas com sucesso no banco de dados InventoryDbContext.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Erro ao aplicar migrações no InventoryDbContext.");
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
