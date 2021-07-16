using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.RouteAnalyzer;
using Confluent.Kafka;
using customer_microservice.Datamodels;
using EasyCaching.Core.Configurations;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MySqlConnector;

namespace customer_microservice
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            using ILoggerFactory loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    }));
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_SERVER")) ||
                String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_PORT")) ||
                String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_DATABASE")) ||
                String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_USER")) ||
                String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_PASSWORD")))
            {
                Console.WriteLine("Missing Environmental Variables!!!");
                Environment.Exit(-1);
            }
            else
            {
                logger.LogInformation("MYSQL Details: {0}@{1}:{2}/{3}", Environment.GetEnvironmentVariable("MYSQL_USER"), Environment.GetEnvironmentVariable("MYSQL_SERVER"), uint.Parse(Environment.GetEnvironmentVariable("MYSQL_PORT")), Environment.GetEnvironmentVariable("MYSQL_DATABASE"));
            }

            services.AddLogging();

            var builder = new MySqlConnectionStringBuilder
            {
                Server = Environment.GetEnvironmentVariable("MYSQL_SERVER"),
                Port = uint.Parse(Environment.GetEnvironmentVariable("MYSQL_PORT")),
                Database = Environment.GetEnvironmentVariable("MYSQL_DATABASE"),
                UserID = Environment.GetEnvironmentVariable("MYSQL_USER"),
                Password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD"),
            };

            //string mySqlConnectionStr = "server=192.168.0.248; port=3306; database=customer_microservice; user=root; password=VMware1!; Persist Security Info=False; Connect Timeout=300";
            //services.AddDbContextPool<CustomerDBContext>(options => options.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString)).
            //    AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
            //    services.AddEFSecondLevelCache(options =>
            //    {
            //        options.UseMemoryCacheProvider().DisableLogging(true);
            //        options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
            //    });


            bool useRedis;
            bool.TryParse(Environment.GetEnvironmentVariable("REDIS_CACHE_USE"), out useRedis);

            services.AddDbContextPool<CustomerDBContext>((serviceProvider, optionsBuilder) =>
                optionsBuilder.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString))
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));

            services.AddEFSecondLevelCache(options =>
            {
                if (useRedis)
                {
                    options.UseEasyCachingCoreProvider("Redis2").DisableLogging(true);
                }
                else
                {
                    options.UseMemoryCacheProvider().DisableLogging(true);
                }
                options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
            });
            if (useRedis)
            {
                if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("REDIS_CACHE_HOST")) ||
                String.IsNullOrEmpty(Environment.GetEnvironmentVariable("REDIS_CACHE_PORT")) ||
                String.IsNullOrEmpty(Environment.GetEnvironmentVariable("REDIS_CACHE_PASSWORD")))
                {
                    Console.WriteLine("Missing Redis Environmental Variables!!!");
                    Environment.Exit(-1);
                }
                else
                {
                    logger.LogInformation("Redis Cache Details: {1}:{2}", Environment.GetEnvironmentVariable("REDIS_CACHE_HOST"), uint.Parse(Environment.GetEnvironmentVariable("REDIS_CACHE_PORT")));
                }

                services.AddEasyCaching(option =>
                {
                    option.UseRedis(config => {
                        config.DBConfig.Password = Environment.GetEnvironmentVariable("REDIS_CACHE_PASSWORD");
                        config.MaxRdSecond = 120;
                        config.EnableLogging = false;
                        config.LockMs = 5000;
                        config.SleepMs = 300;
                        config.DBConfig.IsSsl = false;
                        config.DBConfig.AllowAdmin = true;
                        config.DBConfig.Endpoints.Add(
                            new ServerEndPoint(Environment.GetEnvironmentVariable("REDIS_CACHE_HOST"),
                            int.Parse(Environment.GetEnvironmentVariable("REDIS_CACHE_PORT"))));
                        }, "Redis2"
                    );
                });
            }
            //Kafka
            //// start kafka consumer    
            var consumerConfig = new ConsumerConfig();
            Configuration.Bind("consumer", consumerConfig);
            services.AddSingleton<ConsumerConfig>(consumerConfig);
            //services.AddTransient<interface.IMyBusinessServices, Implementations.MyBusinessServices>();  
            //services.AddHostedService<MyKafkaConsumer>(); //important



            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomerMicroservice", Version = "v1" });
            });


        }

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IHost host, IApplicationBuilder app, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {

            CreateDbIfNotExists(host);
            //Global error handling
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                await context.Response.WriteAsJsonAsync(new { error = exception.Message });
            }));

            //Redirect empty URI to Swagger
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);


            //app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerMicroservice v1"));


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });




        }
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CustomerDBContext>();
                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                    //DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}
