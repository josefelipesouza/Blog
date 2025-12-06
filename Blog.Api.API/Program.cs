using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Context; // Para AuthDbContext
using Blog.Api.Infrastructure.Context; // Para BlogDbContext
using Microsoft.AspNetCore.Authentication.JwtBearer; // Para JwtBearerDefaults
using Microsoft.IdentityModel.Tokens;               // Para TokenValidationParameters, SymmetricSecurityKey
using System.Text;                                  // Para Encoding

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ==========================================================
// 1. Configuração dos Serviços (Injeção de Dependência)
// ==========================================================

// Adiciona os Controllers (necessário para a API Web padrão)
builder.Services.AddControllers();

// 1.1. Configuração do AuthDbContext (Identity)
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)));

// 1.2. Configuração do BlogDbContext (Domínio/Infraestrutura)
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(BlogDbContext).Assembly.FullName)));

// 1.3. Configuração do ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configurações de senha para desenvolvimento/teste
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AuthDbContext>() // Usa o AuthDbContext para persistência
.AddDefaultTokenProviders();

// 1.4. Configuração do JWT Bearer
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Mudar para true em produção
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// ==========================================================
// 2. Configuração do Pipeline (Middleware)
// ==========================================================

// Configurar o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Adicionar Autenticação e Autorização (Deve vir antes do MapControllers)
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os controllers que foram adicionados (sua API REST)
app.MapControllers();

// Remove a rota de exemplo WeatherForecast (opcional, mas limpa o projeto)
/*
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
*/

app.Run();

// Remove o record de exemplo
/*
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/