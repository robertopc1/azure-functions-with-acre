using Redis.API.Application;
using Redis.API.Application.Queries;
using Redis.API.Infrastructure.Extensions;
using Redis.API.Infrastructure.Helpers;
using Redis.Domain.AggregatesModel.ProductAggregate;
using Redis.Infrastructure.Repositories;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace Redis.API;

public class Startup
{
     public IConfiguration Configuration { get; }
     public Startup(IConfiguration configuration)
     {
          var builder = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true) // Add this line
               .AddEnvironmentVariables();

          Configuration = builder.Build();
     }

     public void ConfigureServices(IServiceCollection services)
     {
          var settings = services.AddOptions<Settings>()
               .Bind(Configuration);
          
          services
               .AddCustomDbContext(Configuration);
       
          var redisConnection = ConnectionMultiplexer
               .Connect(Configuration.GetConnectionString("RedisConnectionString"));
          
          
          var provider = new RedisConnectionProvider(redisConnection);
          
          //Create index in Redis
          provider.Connection.CreateIndex(typeof(RedisProduct));

          //Register AutoMapper
          services.AddAutoMapper(typeof(Program));

          // Add services to the container.
          services.AddSingleton<IConnectionMultiplexer>(redisConnection);
          services.AddSingleton<IRedisConnectionProvider>(provider);
          services.AddTransient<IProductRepository, ProductRepository>();

          // Register the MediatR request handlers
          var useWriteBehind = Configuration.GetValue<bool>("UseWriteBehind");
          services.AddCustomMediatR(useWriteBehind);
          
          
          services.AddControllers();
          // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
          services.AddEndpointsApiExplorer();
          services.AddSwaggerGen();
          
     }

     public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
     {
          if (env.IsDevelopment())
          {
               app.UseDeveloperExceptionPage();
               app.UseSwagger();
               app.UseSwaggerUI();
          }
          else
          {
               app.UseExceptionHandler("/Home/Error");
               app.UseHsts();
          }

          app.UseHttpsRedirection();
          app.UseStaticFiles();

          app.UseRouting();

          app.UseAuthorization();

          app.UseEndpoints(endpoints =>
          {
               endpoints.MapControllers(); // Map attribute-routed controllers
          });
     }
}