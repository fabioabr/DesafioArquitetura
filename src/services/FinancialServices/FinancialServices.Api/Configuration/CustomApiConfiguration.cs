using FinancialServices.Api.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.NetworkInformation;
using System.Reflection;

namespace FinancialServices.Api.Configuration
{
    public static class CustomApiConfiguration
    {

        public static WebApplicationBuilder AddCustomApiConfiguration(this WebApplicationBuilder builder)
        {
            return builder
                .AddEndpointDocuments();
        }
        private static WebApplicationBuilder AddEndpointDocuments(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "FinancialServices API (v1)", Version = "v1" });
                options.SwaggerDoc("v2", new() { Title = "FinancialServices API (v2)", Version = "v2" });

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var tags = apiDesc.ActionDescriptor?.EndpointMetadata?
                        .OfType<TagsAttribute>()
                        .FirstOrDefault();

                    return tags?.Tags.Contains(docName) == true;
                });
            });

            return builder;
        }
        public static WebApplication UseApplicationEndpoints(this WebApplication app)
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
                        string version = attribute.Version.ToString();
                        string path = $"/api/v{version}/" + attribute.Path?.Replace("/", string.Empty) ?? "";
                        string routeGroup = attribute.RouteGroup ?? "Endpoints";

                        if (!groupBuilders.ContainsKey(path))
                            groupBuilders.Add(path, app.MapGroup(path));

                        endpoint
                            .Map(groupBuilders[path])
                            .WithOpenApi()
                            .WithName(attribute.Name + $" - (v{version})")
                            .WithTags($"v{version}");

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
        public static WebApplication UseCustomSwagger(this WebApplication app)
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FinancialServices API v1");
                    options.SwaggerEndpoint("/swagger/v2/swagger.json", "FinancialServices API v2");
                });

            }

            return app;
        }
        public static WebApplication UseCustomRedoc(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                // Configuração do ReDoc via Swashbuckle
                app.UseReDoc(c =>
                {
                    c.RoutePrefix = "redoc"; // A URL para acessar o ReDoc será /redoc
                    c.DocumentTitle = "Documentação da API - ReDoc";
                    c.SpecUrl("/swagger/v1/swagger.json"); // URL do JSON do Swagger
                    c.ExpandResponses("all"); // Expande todas as respostas por padrão
                    c.HideDownloadButton(); // Remove o botão de baixar o JSON

                    c.InjectStylesheet("/redoc/default-theme.css");
                });
            }
            return app;
        }
    }
}
