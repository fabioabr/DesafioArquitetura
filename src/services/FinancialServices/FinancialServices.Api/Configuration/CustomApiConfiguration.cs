using FinancialServices.Api.Attributes;
using FinancialServices.Api.Contract;
using FinancialServices.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace FinancialServices.Api.Configuration
{
    public static class CustomApiConfiguration
    {

        public static WebApplicationBuilder AddCustomApiConfiguration(this WebApplicationBuilder builder)
        {

            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return builder
                .AddCustomApplicationSettingsConfiguration()
                .AddCustomLoggingConfiguration()
                .AddCustomMappingConfiguration()
                .AddEndpointDocuments()
                .AddSecurityConfiguration()
                .AddCustomApplicationConfiguration()
                .AddCustomInfrastructureConfiguration()                
                ;

        }
        public static WebApplication UseCustomApiConfiguration(this WebApplication app)
        {
            app.UseApplicationEndpoints();
            app.UseCustomMiddlewares();
            app.UseCustomSwagger();
            app.UseCustomRedoc();
                        
            app.UseAuthentication();
            app.UseAuthorization();
            
            return app;
        }
        
        
        
        private static WebApplicationBuilder AddEndpointDocuments(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "FinancialServices API", Version = "v1" });
                options.SwaggerDoc("v2", new() { Title = "FinancialServices API", Version = "v2" });

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var path = apiDesc.RelativePath?.ToLowerInvariant();

                    return path != null && path.StartsWith($"api/{docName.ToLower()}");
                });

            });

            return builder;
        }        
        private static WebApplicationBuilder AddSecurityConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("PremiumUsers", policy =>
                    policy.RequireRole("credito","debito")); 
            });

            return builder;
        }
        
        private static WebApplication UseApplicationEndpoints(this WebApplication app)
        {

            var endpoints = new List<IEndpoint>();
            var groupBuilders = new Dictionary<string, RouteGroupBuilder>();

            // Obter o assembly atual
            var assembly = Assembly.GetExecutingAssembly();

            // Obter todas as classes que implementam IEndpoint
            var endpointTypes = assembly.GetTypes().Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface);

            // Instanciar cada classe encontrada
            foreach (var endpointType in endpointTypes)
            {
                var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;

                if (endpoint != null)
                {
                    var attribute = endpointType.GetCustomAttribute<RouteBuilderConfigurationAttribute>();

                    if (attribute != null)
                    {
                        string version = $"v{attribute.Version}";
                        string routeGroupName = attribute.RouteGroup.Replace("/", string.Empty);
                        string routeGroupPath = attribute.Path.Replace("/", string.Empty);
                        string groupPath = $"/api/{version}/{routeGroupPath}";

                        // Adiciona um grupo de Endpoints, ja considerando a versão
                        if (!groupBuilders.ContainsKey(groupPath))
                            groupBuilders
                                .Add(groupPath, app
                                    .MapGroup(groupPath)
                                    //.WithTags($"{routeGroupName} - {version}")
                                    
                            );

                        endpoint
                            .Map(groupBuilders[groupPath])
                            .WithOpenApi()
                            .WithName($"{attribute.Name} ({version})")
                            .WithTags($"{routeGroupName}");

                        Console.WriteLine($"[DEBUG] Registrando endpoint: {endpointType.FullName} - Versão: v{version}");


                    }
                    else
                    {
                        throw new InvalidOperationException($"Classe '{endpoint.GetType().Name}' implementa IEndpoint mas não possui o atributo [RouteBuilderConfigurationAttribute].");

                    }


                }



            }

            return app;
        }
        private static WebApplication UseCustomSwagger(this WebApplication app)
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FinancialServices API v1");
                    options.SwaggerEndpoint("/swagger/v2/swagger.json", "FinancialServices API v2");
                });

            }

            return app;
        }
        private static WebApplication UseCustomRedoc(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseReDoc(c =>
                {
                    c.RoutePrefix = "redoc"; // A URL será /redoc/v2
                    c.DocumentTitle = "Documentação da API - ReDoc v2";
                    c.SpecUrl("/swagger/v2/swagger.json");
                    c.ExpandResponses("all");
                    c.HideDownloadButton(); // Remove o botão de baixar o JSON
                    c.InjectStylesheet("/default-theme-app.css");

                });
                
                app.UseStaticFiles();


                /*
                app.UseReDoc(c =>
                {
                    c.RoutePrefix = "redoc/v1"; // A URL será /redoc/v1
                    c.DocumentTitle = "Documentação da API - ReDoc v1";
                    c.SpecUrl("/swagger/v1/swagger.json");
                    c.ExpandResponses("all");
                    c.HideDownloadButton(); // Remove o botão de baixar o JSON
                    c.InjectStylesheet("/default-theme-app.css");

                });

                app.UseReDoc(c =>
                {
                    c.RoutePrefix = "redoc/v2"; // A URL será /redoc/v2
                    c.DocumentTitle = "Documentação da API - ReDoc v2";
                    c.SpecUrl("/swagger/v2/swagger.json");
                    c.ExpandResponses("all");
                    c.HideDownloadButton(); // Remove o botão de baixar o JSON
                    c.InjectStylesheet("/default-theme-app.css");

                });
                */
            }
            return app;
        }
        private static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ApiKeyRequestHandlerMiddleware>();
            return app;
        }
    }
}
