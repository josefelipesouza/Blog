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

// Handlers principais da aplicação
using Blog.Api.Authentication.Handlers;
using Blog.Api.Application.Handlers.Post.Cadastrar;
using Blog.Api.Application.Handlers.Post.Editar;
using Blog.Api.Application.Handlers.Post.Excluir;
using Blog.Api.Application.Handlers.Post.Listar;

// Repositórios e serviços
using Blog.Api.Application.Interfaces.Repositories;
using Blog.Api.Infrastructure.Repositories;
using Blog.Api.Application.Interfaces.Data;
using Blog.Api.Application.Interfaces.Services;
using Blog.Api.Infrastructure.Services;
using Blog.Api.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)));

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(typeof(BlogDbContext).Assembly.FullName)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

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
    options.RequireHttpsMetadata = false;
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

// CORREÇÃO DO MEDIATR
// Registrar apenas assemblies de handlers (correto)
// MediatR - registrar assemblies de HANDLERS (corrigido)
builder.Services.AddMediatR(
    typeof(Blog.Api.Authentication.Handlers.RegistrarUsuarioHandler).Assembly,
    typeof(Blog.Api.Application.Handlers.Post.Cadastrar.CadastrarPostagemHandler).Assembly,
    typeof(Blog.Api.Application.Handlers.Post.Editar.EditarPostagemHandler).Assembly,
    typeof(Blog.Api.Application.Handlers.Post.Excluir.ExcluirPostagemHandler).Assembly,
    typeof(Blog.Api.Application.Handlers.Post.Listar.ListarPostagensHandler).Assembly
);

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IPostagemRepository, PostagemRepository>();
builder.Services.AddScoped<IUnityOfWork, BlogDbContext>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Blog API",
        Version = "v1"
    });

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

var app = builder.Build();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
