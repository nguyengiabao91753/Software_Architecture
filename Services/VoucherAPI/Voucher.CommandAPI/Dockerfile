# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7004

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/VoucherAPI/Voucher.CommandAPI/Voucher.CommandAPI.csproj", "Services/VoucherAPI/Voucher.CommandAPI/"]
COPY ["Services/VoucherAPI/Voucher.Application/Voucher.Application.csproj", "Services/VoucherAPI/Voucher.Application/"]
COPY ["Services/VoucherAPI/Voucher.Shared/Voucher.Shared.csproj", "Services/VoucherAPI/Voucher.Shared/"]
COPY ["Services/VoucherAPI/Voucher.Infrastructure/Voucher.Infrastructure.csproj", "Services/VoucherAPI/Voucher.Infrastructure/"]
COPY ["Services/VoucherAPI/Voucher.Messaging/Voucher.Messaging.csproj", "Services/VoucherAPI/Voucher.Messaging/"]
COPY ["Integrations/Integrations.Messaging/Integrations.Messaging.csproj", "Integrations/Integrations.Messaging/"]
RUN dotnet restore "./Services/VoucherAPI/Voucher.CommandAPI/Voucher.CommandAPI.csproj"
COPY . .
WORKDIR "/src/Services/VoucherAPI/Voucher.CommandAPI"
RUN dotnet build "./Voucher.CommandAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Voucher.CommandAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Voucher.CommandAPI.dll"]