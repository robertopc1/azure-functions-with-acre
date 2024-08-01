using System.Reflection;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Redis.API.Application.Behaviors;
using Redis.API.Application.Commands;
using Redis.API.Application.Queries;
using Redis.Infrastructure;

namespace Redis.API.Infrastructure.Extensions;

public static class CustomExtensionMethods
{
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkSqlServer()
            .AddDbContext<ProductContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("SQLConnectionString"),
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                },
                ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

        // services.AddDbContext<IntegrationEventLogContext>(options =>
        // {
        //     options.UseSqlServer(configuration["ConnectionString"],
        //         sqlServerOptionsAction: sqlOptions =>
        //         {
        //             sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
        //             //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
        //             sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        //         });
        // });

        return services;
    }
    
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services, bool useWriteBehind)
    {
        // Register MediatR
        services.AddMediatR(typeof(GetProductByIdQuery).Assembly);

        // Register the Command classes (they implement IRequestHandler)
    
        if (useWriteBehind)
        {
            services.AddTransient<IRequestHandler<CreateProductCommand, bool>, CreateProductWBCommandHandler>();
        }
        else
        {
            services.AddTransient<IRequestHandler<CreateProductCommand, bool>, CreateProductEFCommandHandler>();
        }

        // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
        //services.AddMediatR(typeof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler).GetTypeInfo().Assembly);

        // Register the Command's Validators (Validators based on FluentValidation library)
        //services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).GetTypeInfo().Assembly);

        // Register ServiceFactory
        services.AddTransient<ServiceFactory>(provider => t => provider.GetService(t));

        // Register the pipeline behaviors
       services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
       // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
       services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

        return services;
    }
    
    public static IHost MigrateDbContext<TContext>(this IHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                    
                    var retries = 10;
                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(
                            retryCount: retries,
                            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (exception, timeSpan, retry, ctx) =>
                            {
                                logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(TContext), exception.GetType().Name, exception.Message, retry, retries);
                            });

                    //if the sql server container is not created on run docker compose this
                    //migration can't fail for network related exception. The retry options for DbContext only 
                    //apply to transient exceptions
                    // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                    retry.Execute(() => InvokeSeeder(seeder, context, services));
                    

                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                }
            }

            return webHost;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
}