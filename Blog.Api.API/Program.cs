// Program.cs - Configuração completa do ASP.NET Core Web API

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;
using Blog.Api.Authentication.Context;
using Blog.Api.Infrastructure.Context;
using Blog.Api.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // Adicionado para referência a ClaimTypes, se necessário

// Usings específicos do MediatR e do Core
using Blog.Api.Authentication.Handlers;
using Blog.Api.Application.Handlers.Post.Cadastrar;
using Blog.Api.Application.Handlers.Post.Editar;
using Blog.Api.Application.Handlers.Post.Excluir;
using Blog.Api.Application.Handlers.Post.Listar;
using Blog.Api.Application.Handlers.User.Listar;
using Blog.Api.Application.Handlers.User.Editar;
using Blog.Api.Application.Handlers.User.Excluir;

// Usings específicos de Injeção de Dependência
using Blog.Api.Application.Interfaces.Repositories;
using Blog.Api.Infrastructure.Repositories;
using Blog.Api.Application.Interfaces.Data;
using Blog.Api.Application.Interfaces.Services;
using Blog.Api.Infrastructure.Services;
using Blog.Api.Infrastructure.Seed;


// ==============================================================================
// 🎯 CONFIGURAÇÃO CRÍTICA DO JWT
// Limpa os mapeamentos de claims para evitar que o .NET mude 'role' para um URL longo.
// Isso garante que as claims que o JwtTokenService insere (rol, role, email, etc.) 
// permaneçam com seus nomes curtos.
// ==============================================================================
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();      
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();     

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 1. Adição de Controllers e Serialização
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Garante que o JSON de saída esteja em camelCase (padrão de frontend)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// 2. Configuração do Entity Framework Core (DbContex)
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)));

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(BlogDbContext).Assembly.FullName)));

// 3. Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            // Permite acesso do frontend local
            .WithOrigins("http://localhost:5173") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// 4. Configuração do ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    // Adicione outras opções de senha/lockout aqui, se necessário
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// 5. Configuração do JWT Authentication (Bearer Token)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var keyString = jwtSettings["Key"] ?? throw new Exception("JwtSettings:Key não encontrada.");
var key = Encoding.UTF8.GetBytes(keyString);

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
        // Validação da Chave
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Validação de Emissor (Issuer)
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        // Validação de Público (Audience)
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        // Validação de Tempo de Vida
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // Não adiciona margem de tempo ao expiratio

        // GARANTIA DE ROLES NO PRINCIPAL:
        // O pipeline do ASP.NET Core deve procurar a role na claim "rol" (IANA)
        RoleClaimType = "rol", 
        // O pipeline deve procurar o nome do usuário na claim "unique_name" (padrão JWT)
        NameClaimType = JwtRegisteredClaimNames.UniqueName 
    };
});

// 6. Configuração do MediatR
builder.Services.AddMediatR(
    typeof(RegistrarUsuarioHandler).Assembly,
    typeof(CadastrarPostagemHandler).Assembly,
    typeof(EditarPostagemHandler).Assembly,
    typeof(ExcluirPostagemHandler).Assembly,
    typeof(ListarPostagensHandler).Assembly,
    typeof(ListarUsuariosHandler).Assembly,
    typeof(EditarUsuarioHandler).Assembly,
    typeof(ExcluirUsuarioHandler).Assembly
);

// 7. Injeção de Dependência (Services e Repositórios)
builder.Services.AddScoped<JwtTokenService>(); // Seu serviço de JWT
builder.Services.AddScoped<IPostagemRepository, PostagemRepository>();
builder.Services.AddScoped<IUnityOfWork, BlogDbContext>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

// 8. Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Blog API",
        Version = "v1"
    });

    // Configuração para permitir JWT no Swagger UI
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe apenas o token JWT."
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

var app = builder.Build();

// ==============================================================================
// 🚀 PIPELINE DE REQUESTS
// ==============================================================================

// Aplica Migrações e Seeds
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var authContext = services.GetRequiredService<AuthDbContext>();
    await authContext.Database.MigrateAsync();

    var blogContext = services.GetRequiredService<BlogDbContext>();
    await blogContext.Database.MigrateAsync();

    await AuthDbSeeder.SeedAsync(services);
    await BlogDbSeeder.SeedAsync(services);
}

// Configurações em Desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

// ORDEM IMPORTANTE: Authentication DEVE vir antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();