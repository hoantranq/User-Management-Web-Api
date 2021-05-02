using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Linq;
using UserManagement_Backend.Helpers;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend
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
            services.ConfigureCors();

            services.ConfigureLoggerService();

            services.ConfigureUserService();

            services.ConfigureAuthService();

            services.ConfigureRoleService();

            services.ConfigureUserRolesService();

            services.ConfigurePolicyService();

            services.ConfigureJwtAuthentication(Configuration);

            services.ConfigureDbContext(Configuration);

            services.AddAutoMapper(typeof(Startup));

            services.Configure<JWT>(Configuration.GetSection("JWT"));

            services.AddControllers().ConfigureApiBehaviorOptions(options => 
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errs = new List<string>();

                    foreach (var modelState in context.ModelState)
                    {
                        errs = modelState.Value.Errors.Select(e => e.ErrorMessage).ToList();
                    }

                    var response = new BaseApiResponse
                    { 
                        Succeeded = false,
                        Message = "An error occurred when attempting to make a request.",
                        Data = null,
                        Errors = errs
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagement_Backend", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagement_Backend v1"));
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
