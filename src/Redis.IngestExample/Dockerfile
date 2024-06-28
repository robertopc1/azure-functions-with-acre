FROM mcr.microsoft.com/dotnet/sdk:8.0 AS installer-env

# Define build arguments for the connection strings and other settings
ARG AzureWebJobsStorage
ARG FUNCTIONS_WORKER_RUNTIME
ARG SQLConnectionString
ARG RedisConnectionString

# Set environment variables using the build arguments
ENV AzureWebJobsStorage=$AzureWebJobsStorage
ENV FUNCTIONS_WORKER_RUNTIME=$FUNCTIONS_WORKER_RUNTIME
ENV ConnectionStrings__SQLConnectionString=$SQLConnectionString
ENV ConnectionStrings__RedisConnectionString=$RedisConnectionString

WORKDIR /src
COPY Redis.IngestExample/ Redis.IngestExample/
COPY Redis.Functions.DataAccess/ Redis.Functions.DataAccess/
COPY Redis.Functions.Shared/ Redis.Functions.Shared/

RUN dotnet publish "Redis.IngestExample/Redis.IngestExample.csproj" --output /home/site/wwwroot

EXPOSE 80
# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]
