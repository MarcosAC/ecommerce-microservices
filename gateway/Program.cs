using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);

var jwtKey = builder.Configuration["Jwt:Key"] ?? "MINHA_CHAVE_SECRETA_MUITO_SECRETA_123456";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "ecommerce.local";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("TestKey", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidAudience = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services
    .AddOcelot(builder.Configuration)
    .AddPolly();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
