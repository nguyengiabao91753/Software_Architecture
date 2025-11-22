# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7033

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Orders/Orders.Orchestrator/Orders.Orchestrator.csproj", "Services/Orders/Orders.Orchestrator/"]
COPY ["Integrations/Integrations.Messaging/Integrations.Messaging.csproj", "Integrations/Integrations.Messaging/"]
RUN dotnet restore "./Services/Orders/Orders.Orchestrator/Orders.Orchestrator.csproj"
COPY . .
WORKDIR "/src/Services/Orders/Orders.Orchestrator"
RUN dotnet build "./Orders.Orchestrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orders.Orchestrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.Orchestrator.dll"]