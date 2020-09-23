using Access.Auth.Service.Host.Extension;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Access.Auth.Service.Domain.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Tracing;
using Access.Auth.Service.Business.Logging;
using Elastic.Apm.All;
using System;
using Newtonsoft.Json;


namespace Access.Auth.Service.Host
{
	public class Startup
    {
        private readonly IHostingEnvironment environment;

        public IConfiguration configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {			
			services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IAuthConfiguration, AuthConfiguration>();

            var sp = services.BuildServiceProvider();
            var authConfiguration = sp.GetService<IAuthConfiguration>();

            services.AddCors();

            services.AddMvc(options =>
			{
				
			});

			services.AddIdentityServer(o => {
                o.IssuerUri = authConfiguration.IdentityServerBaseAddress;
                o.PublicOrigin = authConfiguration.IdentityServerBaseAddress;
            })
            .AddSigningCredential(new SigningConfigurations().SigningCredentials)
            .AddMongoRepository()
            .AddClients()
            .AddUsers()
            .AddResourceOwnerValidation()
            .AddClientCredentialsValidation()
            .AddProfileService()
            .AddIdentityApiResources()
            .AddExternalValidation(authConfiguration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            app.UseElasticApm(configuration);

			logger.LogInformation(EventIds.Information.ServerStarted, "Server Started in {Environment}", env.EnvironmentName);

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            ConfigureMongoDriver2IgnoreExtraElements();
        }

        private async Task WriteResponse(HttpStatusCode httpStatus, string messageContent, HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int)httpStatus;
            var messageJson = JsonConvert.SerializeObject(new { message = messageContent });
            await httpContext.Response.WriteAsync(messageJson);
        }

        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
	}
}
