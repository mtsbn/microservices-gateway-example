using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ApiGateway
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //auth logic

            string textKey = "this is the secret key to add some default jwt token, lets see how it works";

            var key = Encoding.ASCII.GetBytes(textKey);

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(textKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = "chandra",
                ValidateAudience = true,
                ValidAudience = "enduser",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer("TestKey", x =>
              {
                  x.RequireHttpsMetadata = false;
                  x.TokenValidationParameters = tokenValidationParameters;
              });

            services.AddOcelot(configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void
            Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync("API GATEWAY FUNCIONANDO!");
                });
            });

            app.UseOcelot().Wait();
        }
    }
}
