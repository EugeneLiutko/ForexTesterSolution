# Базовий образ з ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Білд-образ з .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копіюємо лише .csproj для відновлення залежностей
COPY ["UserService.API/UserService.API.csproj", "UserService.API/"]
COPY ["UserService.Application/UserService.Application.csproj", "UserService.Application/"]
COPY ["UserService.Domain/UserService.Domain.csproj", "UserService.Domain/"]
COPY ["UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService.Infrastructure/"]
COPY ["UserService.Tests/UserService.Tests.csproj", "UserService.Tests/"]
RUN dotnet restore "UserService.API/UserService.API.csproj"

# Копіюємо весь код
COPY . .

# Білд
RUN dotnet build "UserService.API/UserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публікація
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.API/UserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Фінальний образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]
