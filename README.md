# Caching Patterns (Ingest & Write-Behind) with Azure Functions + ACRE + Azure SQL

## Summary
This a demo of how can you implement the Ingest and Write-Behind caching patterns using the Azure Functions Redis Extensions,
Redis Enterprise & Azure SQL.

At the moment we haven't implemented the Ingest Pattern with Redis Output Binding because Azure Cache for Redis Enterprise (ACRE)
is running version 2.4 of RediSearch and JSON.MSET support is at 2.6. We will update this soon since upgrade to 7.2 is in public preview. We also haven't implemented the Write-Behind pattern using
Redis Input Binding because at the moment the Redis Input Bindings only support single output commands [here](https://github.com/Azure/azure-functions-redis-extension/blob/main/src/Microsoft.Azure.WebJobs.Extensions.Redis/Bindings/RedisAsyncConverter.cs#L63). We are using Redis OM .Net to update our data.

## Features

- CQRS
- Ingest & Write-Behind patterns
- Use Redis OM .Net to update data in ACRE
- Use Pub/Sub Trigger from Azure Functions Redis Extensions to listen to key event notifications for Write-Behind pattern
  - *** Will update to use streams instead
- Use SQL Trigger to listen for changes in Ingest pattern

## Architecture
![Local Deployment](./images/local-deployment.png)
![Azure Deployment](./images/azure-deployment.png)

## Prerequisites

- VS Code/Visual Studio/Rider
- .Net 8
- OSX or Windows
- Azure SQL
  - Configuration steps [here](https://learn.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal)
- Azure Cache for Redis Enterprise
  - Configuration steps [here](https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/quickstart-create-redis-enterprise)
- Redis Insight [here](https://redis.io/insight/)
- Docker

## Installation

### Azure SQL

Option 1: Run SQL Script to create table:

1. Copy the contents of the script named "CreateStylesTable.sql". The file is located inside the SQL folder located in the Solution Items folder.

2. Open the query editor of your preferred database tool (Azure Data Studio or SQL Server Management Studio) and paste the SQL script copied during Step 1.

3. Run the script to create the table.

Option 2: When the application starts it will use EF migrations to create schema, database and load the sample data if table is empty

## Quickstart

1. Clone the git repository

```sh
git clone https://github.com/robertopc1/azure-functions-with-acre
```

2. Open it with your Visual Studio Code, Visual Studio or Rider

3. Update local.settings.json
    - Replace "--SECRET--" with the real connection strings for Azure SQL and Redis

    ```text
    "ConnectionStrings": {
      "SQLConnectionString": "--SECRET--",
      "RedisConnectionString": "--SECRET--"
    }
    ```

4. Run Azure Function (Ingest)
    - You can test the Ingest pattern by:

    ```sh
    cd Redis.IngestExample
    func start
    ```

    After running the Azure function, update any column on the styles table using your preferred Database management application.
    The Azure Function will trigger and push the updated into ACRE.


5. Run Azure Function (Write-Behind)
    - You can test the Write-Behind pattern by:

    ```sh
    cd Redis.WriteBehindExample
    func start
    ```
   
    After running the Azure function, you can make an update to any of the Redis JSON document using Redis Insight or Redis CLI.
    The Azure Function will trigger and push the updated record into Azure SQL.

## Run with Docker Compose

1. Create a .env file with the variables below and populate them with the correct information
```text
AzureWebJobsStorage=UseDevelopmentStorage=true
FUNCTIONS_WORKER_RUNTIME=dotnet-isolated
SQLConnectionString=
RedisConnectionString=
MSSQL_SA_PASSWORD=
Sql_Trigger_MaxBatchSize=1000
```

2. Run 
 ```sh
    docker compose up
```

Docker compose will the following containerized service:
- SQLServer 2022
- Redis.API (Sample Api)
- Azurite
- Ingest Function App
