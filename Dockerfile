# Etapa 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia arquivos de projeto (csproj)
COPY Blog.Api.sln .
COPY Blog.Api.API/*.csproj Blog.Api.API/
COPY Blog.Api.Application/*.csproj Blog.Api.Application/
COPY Blog.Api.Infrastructure/*.csproj Blog.Api.Infrastructure/
COPY Blog.Api.Domain/*.csproj Blog.Api.Domain/
COPY Blog.Api.Authentication/*.csproj Blog.Api.Authentication/

# Restaura dependências
RUN dotnet restore

# Copia todo o código
COPY . .

# Compila
WORKDIR /src/Blog.Api.API
RUN dotnet publish -c Release -o /app/publish

# Etapa 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Blog.Api.API.dll"]
