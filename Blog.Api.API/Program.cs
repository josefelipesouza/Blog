using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;
using Blog.Api.Authentication.Context; // Para AuthDbContext
using Blog.Api.Infrastructure.Context; // Para BlogDbContext
using Blog.Api.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;
using Blog.Api.Authentication.Requests.Register;
using Blog.Api.Application.Handlers.Post.Cadastrar;
using Blog.Api.Application.Handlers.Post.Editar;
using Blog.Api.Application.Handlers.Post.Excluir;
using Blog.Api.Application.Handlers.Post.Listar;
using Blog.Api.Application.Interfaces.Repositories;
using Blog.Api.Infrastructure.Repositories;
using Blog.Api.Application.Interfaces.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ==========================================================
// 1. Configuração dos Serviços (Injeção de Dependência)
// ==========================================================

// Controllers
builder.Services.AddControllers();

// AuthDbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)));

// BlogDbContext
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(BlogDbContext).Assembly.FullName)));

// ASP.NET Core Identity (ApplicationUser)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// ----------------------------------------------------------
// JWT
// ----------------------------------------------------------
// Lê a seção JwtSettings (mesma que no appsettings.json)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var keyString = jwtSettings["Key"] ?? throw new Exception("JwtSettings:Key não encontrada no configuration.");
var key = Encoding.UTF8.GetBytes(keyString);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // true em prod
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

// MediatR
// MediatR
builder.Services.AddMediatR(
    typeof(Program).Assembly,
    typeof(RegisterUserRequest).Assembly,
    typeof(CadastrarPostagemHandler).Assembly,
    typeof(EditarPostagemHandler).Assembly,
    typeof(ExcluirPostagemHandler).Assembly,
    typeof(ListarPostagensHandler).Assembly
);

// Serviços
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddScoped<IPostagemRepository, PostagemRepository>();
builder.Services.AddScoped<IUnityOfWork, BlogDbContext>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Blog API",
        Version = "v1"
    });

    // 🔐 Adiciona suporte a JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    Description = "Informe apenas o token JWT. O prefixo 'Bearer ' será adicionado automaticamente."
});


    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ==========================================================
// 2. Pipeline
// ==========================================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
