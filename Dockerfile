# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY src/ForexRateAlerter.Api/*.csproj ./ForexRateAlerter.Api/
COPY src/ForexRateAlerter.Core/*.csproj ./ForexRateAlerter.Core/
COPY src/ForexRateAlerter.Infrastructure/*.csproj ./ForexRateAlerter.Infrastructure/

# Restore dependencies
RUN dotnet restore ./ForexRateAlerter.Api/

# Copy source code
COPY src/ ./

# Build application
RUN dotnet publish ./ForexRateAlerter.Api/ -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install SQL Server tools for migrations (optional)
RUN apt-get update && apt-get install -y curl

# Copy built application
COPY --from=build /app/out ./

# Expose port
EXPOSE 80
EXPOSE 443

# Set entry point
ENTRYPOINT ["dotnet", "ForexRateAlerter.Api.dll"]
