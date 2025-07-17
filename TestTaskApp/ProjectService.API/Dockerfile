FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ProjectService.API/ProjectService.API.csproj", "ProjectService.API/"]
COPY ["ProjectService.Application/ProjectService.Application.csproj", "ProjectService.Application/"]
COPY ["ProjectService.Domain/ProjectService.Domain.csproj", "ProjectService.Domain/"]
COPY ["ProjectService.Infrastructure/ProjectService.Infrastructure.csproj", "ProjectService.Infrastructure/"]
COPY ["ProjectService.Tests/ProjectService.Tests.csproj", "ProjectService.Tests/"]
RUN dotnet restore "ProjectService.API/ProjectService.API.csproj"
COPY . .

RUN dotnet build "ProjectService.API/ProjectService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectService.API/ProjectService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectService.API.dll"]
