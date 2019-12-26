using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace HG.IdentityServer4.MvcClient
{
    public class Startup
    {
        // oauth 2.0
        // https://www.cnblogs.com/tianyamoon/p/9490953.html
        // http://www.ruanyifeng.com/blog/2014/05/oauth_2_0.html
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // ensure not change any return Claims from Authorization Server
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc"; // oidc => open ID connect
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                //options.Authority = $"https://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}";
                //options.Authority = "https://www.tview.cn:8090";
                options.Authority = "https://198.198.198.176:8090";

                options.RequireHttpsMetadata = false; // please use https in production env
                options.ClientId = "code.015cd5b0-5b1c-406d-8b8e-fca63ad6d7e5";
                options.ClientSecret = "secret.ce9028f2-b3d0-4e9f-9043-ec40f945332a";

                //options.ClientId = "code123456789";
                //options.ClientSecret = "secret123456789";

                options.ResponseType = OpenIdConnectResponseType.Code; // allow to return access token
                options.SaveTokens = true;

                options.Scope.Add("api1");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
