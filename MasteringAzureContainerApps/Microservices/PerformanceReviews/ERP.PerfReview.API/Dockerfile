# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/PerformanceReviews/ERP.PerfReview.API/ERP.PerfReview.API.csproj", "Microservices/PerformanceReviews/ERP.PerfReview.API/"]
COPY ["Common/ERP.Common/ERP.Common.csproj", "Common/ERP.Common/"]
COPY ["Microservices/PerformanceReviews/ERP.PerfReview.Application/ERP.PerfReview.Application.csproj", "Microservices/PerformanceReviews/ERP.PerfReview.Application/"]
COPY ["Microservices/PerformanceReviews/ERP.PerfReview.Infrastructure/ERP.PerfReview.Infrastructure.csproj", "Microservices/PerformanceReviews/ERP.PerfReview.Infrastructure/"]
COPY ["Common/ERP.Infrastructure.Core/ERP.Infrastructure.Core.csproj", "Common/ERP.Infrastructure.Core/"]
RUN dotnet restore "./Microservices/PerformanceReviews/ERP.PerfReview.API/ERP.PerfReview.API.csproj"
COPY . .
WORKDIR "/src/Microservices/PerformanceReviews/ERP.PerfReview.API"
RUN dotnet build "./ERP.PerfReview.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ERP.PerfReview.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ERP.PerfReview.API.dll"]