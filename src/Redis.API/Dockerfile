FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY /Redis.API Redis.API/
COPY /Redis.Infrastructure Redis.Infrastructure/
COPY /Redis.Domain Redis.Domain/

RUN dotnet restore "Redis.API/Redis.API.csproj"

RUN dotnet build "Redis.API/Redis.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Redis.API/Redis.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy styles.csv to the appropriate location
COPY --from=datafile . /app/infra/sql

ENTRYPOINT ["dotnet", "Redis.API.dll"]
