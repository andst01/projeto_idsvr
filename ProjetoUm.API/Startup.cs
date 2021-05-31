using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace ProjetoUm.API
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

            services.AddControllers();


            services.AddMvcCore()
                  .AddAuthorization()
                  .AddNewtonsoftJson(o =>
                  {
                      o.SerializerSettings.Converters.Add(new StringEnumConverter());
                      o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                  });

            ////
            /// Configuração da Autenticação da API 
            /// lembrando que o resourceApiUm é a configuração do ApiResource, ou seja,
            /// ele pode acessar a ApiUm e ApiDois conforme configuração no IdentityServer
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:5001/";
                    options.RequireHttpsMetadata = false;
                    options.ApiSecret = "apium";
                    options.ApiName = "resourceApiUm";
                });


            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });


            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {

                    Title = "Api Um",
                    Version = "v1"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        
                        ClientCredentials = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri("https://localhost:5001/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:5001/connect/token"),

                            Scopes = new Dictionary<string, string>
                            {
                                { "scopeApiUm", "scopeApiUm" },
                                { "scopeApiDois", "scopeApiDois" },

                            }

                        }

                    }

                }); ;


                options.OperationFilter<CheckAuthorizeOperationFilter>();
            });

            services.AddResponseCaching();

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            services.AddDistributedMemoryCache();
           
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

            services.AddCors();
            
            IdentityModelEventSource.ShowPII = true;

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
              
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjetoUm.API v1");

                    options.OAuthClientId("projeto.api.um");
                    options.OAuthClientSecret("mvc1");
                    options.OAuthAppName("Swagger API Teste 1 UI");


                });

            }

            app.UseHttpsRedirection();
            app.UseRouting();

            //Sempre colocar UseAuthentication na frente do UseAuthorization
            /// pois ele autentica primeiro depois autoriza
            app.UseAuthentication();
            app.UseAuthorization();
          

            app.UseCookiePolicy();
           
            app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           
        }

        internal class CheckAuthorizeOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {

                var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

                if (hasAuthorize)
                {
                    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                    operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme {Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"}
                            }
                        ] = new[] { "scopeApi" }
                    }
                };
                };


            }


        }

    }
}
