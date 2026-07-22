# Base image with only the .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage with the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef

# Ensure the dotnet tools path is available
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy and restore dependencies
COPY ["BasicSocialMedia.API/BasicSocialMedia.API.csproj", "BasicSocialMedia.API/"]
COPY ["BasicSocialMedia.Application/BasicSocialMedia.Application.csproj", "BasicSocialMedia.Application/"]
COPY ["BasicSocialMedia.Domain/BasicSocialMedia.Domain.csproj", "BasicSocialMedia.Domain/"]
COPY ["BasicSocialMedia.Infrastructure/BasicSocialMedia.Infrastructure.csproj", "BasicSocialMedia.Infrastructure/"]
RUN dotnet restore "./BasicSocialMedia.API/BasicSocialMedia.API.csproj"

COPY . .
WORKDIR "/src/BasicSocialMedia.API"
RUN dotnet build "./BasicSocialMedia.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BasicSocialMedia.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage (no SDK, only runtime)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure the EF tools are still available (optional, for debugging)
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "BasicSocialMedia.API.dll"]
