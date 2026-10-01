# Multi-stage Dockerfile for VerifyGH Backend API (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for caching restore layer
COPY ["src/VerifyGH.Shared/VerifyGH.Shared.csproj", "src/VerifyGH.Shared/"]
COPY ["src/VerifyGH.Server/VerifyGH.Server.csproj", "src/VerifyGH.Server/"]
RUN dotnet restore "src/VerifyGH.Server/VerifyGH.Server.csproj"

# Copy source code and build
COPY src/ src/
WORKDIR "/src/src/VerifyGH.Server"
RUN dotnet publish "VerifyGH.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Cloud container environment
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DatabaseProvider=Sqlite
ENV ConnectionStrings__DefaultConnection="Data Source=verifygh.db"

EXPOSE 10000
ENTRYPOINT ["dotnet", "VerifyGH.Server.dll"]
