# Base image with ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["todos.api/todos.api.csproj", "todos.api/"]
RUN dotnet restore "./todos.api/todos.api.csproj"

# Copy the rest of the application source code and build it
COPY . .
WORKDIR "/src/todos.api"
RUN dotnet build "./todos.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./todos.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final

# Copy certificate for HTTPS (make sure this exists in your local directory)
COPY certs/aspnetapp.pfx /https/aspnetapp.pfx

# Copy the published application to the final container	
COPY --from=publish /app/publish .

# Switch to non-root user for better security
USER app

# Start the application
ENTRYPOINT ["dotnet", "todos.api.dll"]
	