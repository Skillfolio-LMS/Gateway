# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution file
COPY Gateway.sln .

# Copy project files
COPY src/Gateway.Api/Gateway.Api.csproj src/Gateway.Api/
COPY src/Gateway.Application/Gateway.Application.csproj src/Gateway.Application/
COPY src/Gateway.Domain/Gateway.Domain.csproj src/Gateway.Domain/
COPY src/Gateway.Infrastructure/Gateway.Infrastructure.csproj src/Gateway.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish src/Gateway.Api/Gateway.Api.csproj -c Release -o /app/publish --no-restore

# Use the official .NET 9.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]