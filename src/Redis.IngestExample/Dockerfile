# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# Define build arguments for the connection strings and other settings
ARG AzureWebJobsStorage
ARG FUNCTIONS_WORKER_RUNTIME
ARG SQLConnectionString
ARG RedisConnectionString

# Copy source code and restore dependencies
COPY Redis.IngestExample/ Redis.IngestExample/
COPY Redis.Functions.DataAccess/ Redis.Functions.DataAccess/
COPY Redis.Functions.Shared/ Redis.Functions.Shared/

# Publish the app
RUN dotnet publish "Redis.IngestExample/Redis.IngestExample.csproj" -c Debug -o /home/site/wwwroot

# Runtime stage
# Use the Azure Functions isolated image
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0

# Set environment variables for the function runtime
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    DOTNET_EnableWriteXorExecute=0

# Set environment variables for connection strings and function runtime
ENV AzureWebJobsStorage=$AzureWebJobsStorage
ENV FUNCTIONS_WORKER_RUNTIME=$FUNCTIONS_WORKER_RUNTIME
ENV Sql_Trigger_MaxBatchSize=$Sql_Trigger_MaxBatchSize
ENV ConnectionStrings__SQLConnectionString=$SQLConnectionString
ENV ConnectionStrings__RedisConnectionString=$RedisConnectionString

# Install debugging tools (vsdbg)
RUN apt-get update && \
    apt-get install -y unzip curl && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Copy the app from the build stage
COPY --from=build-env /home/site/wwwroot /home/site/wwwroot

# Expose necessary ports for Azure Functions runtime and debugging
EXPOSE 80 7071 9229