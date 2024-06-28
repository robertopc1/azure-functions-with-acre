using Redis.API.Application.Queries;
using Redis.API.Application.Services;
using Redis.API.Infrastructure.Extensions;
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
          Configuration = configuration;
     }

     public void ConfigureServices(IServiceCollection services)
     {
          services
               .AddCustomDbContext(Configuration);

          var redisConnection = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnectionString"));
          var provider = new RedisConnectionProvider(redisConnection);
          provider.Connection.CreateIndex(typeof(RedisProduct));

          //Register AutoMapper
         services.AddAutoMapper(typeof(Program));

          // Add services to the container.
          services.AddSingleton<IRedisConnectionProvider>(provider);
          services.AddTransient<IProductRepository, ProductRepository>();
          services.AddTransient<IRedisService, RedisService>();

          // Register the MediatR request handlers
          services.AddCustomMediatR();
          
          
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