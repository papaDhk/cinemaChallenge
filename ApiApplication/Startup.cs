using System;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Database;
using ApiApplication.Database.Repositories;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Middlewares;
using ApiApplication.Services.Auditorium;
using ApiApplication.Services.Movies;
using ApiApplication.Services.ReservationService;
using ApiApplication.Services.Showtimes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProtoDefinitions;

namespace ApiApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            ConfigureCustomsService(services);
        }

        public void ConfigureCustomsService(IServiceCollection services)
        {
            services.AddTransient<IShowtimesRepository, ShowtimesRepository>();
            services.AddTransient<ITicketsRepository, TicketsRepository>();
            services.AddTransient<IAuditoriumsRepository, AuditoriumsRepository>();
            services.AddTransient<IMoviesRepository, MoviesRepository>();
            services.AddTransient<IMoviesService, MoviesService>();
            services.AddTransient<IShowtimesService, ShowtimeService>();
            services.AddTransient<IAuditoriumService, AuditoriumService>();
            services.AddTransient<IReservationService, ReservationService>();


            services.AddDbContext<CinemaContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            }, ServiceLifetime.Transient);

            services.AddHttpClient();
            services.AddGrpcClient<MoviesApi.MoviesApiClient>(o =>
                {
                    o.ChannelOptionsActions.Add(channelOptions =>
                    {
                        var httpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                        channelOptions.HttpHandler = httpHandler;
                    });
                    o.Address = new Uri(Configuration.GetValue<string>("MoviesApi:Address"));
                })
                .AddCallCredentials((context, metadata) =>
                {
                    metadata.Add("X-Apikey", Configuration.GetValue<string>("MoviesApi:ApiKey"));
                    return Task.CompletedTask;
                });
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "CachingInstance1";
            });
            
            services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0", new OpenApiInfo{
                    Version = "v0",
                    Title = "Cinema challenge api",
                });
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ResponseTimeTrackingMiddleware>();
            app.UseMiddleware<CustomExceptionsHandlerMiddleware>();
            
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>{
                c.SwaggerEndpoint("/swagger/v0/swagger.json", "Cinema challenge v0");
            });
            
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            SampleData.Initialize(serviceScope.ServiceProvider);
        }      
    }
}
