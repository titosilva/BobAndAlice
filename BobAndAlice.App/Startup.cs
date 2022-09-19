using BobAndAlice.App.Configuration;
using BobAndAlice.App.Database;
using BobAndAlice.App.Filters;
using BobAndAlice.App.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BobAndAlice.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(options => options.Filters.Add<AppExceptionFilter>())
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddCors(o => o.AddPolicy("CORSPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddDbContext<AppDbContext>(options =>
            {
                var folder = Environment.SpecialFolder.Desktop;
                var path = Environment.GetFolderPath(folder);
                var dataSource = System.IO.Path.Join(path, "bobandalice.db");
                options.UseSqlite($"Data Source={dataSource}");
            });

            // Configuration
            services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

            // Repositories
            services.AddScoped<UserRepository>();
            services.AddScoped<UserKeyRepository>();

            // Services
            services.AddScoped<ModelConverter>();
            services.AddScoped<UserService>();
            services.AddScoped<UserKeyService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CORSPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
