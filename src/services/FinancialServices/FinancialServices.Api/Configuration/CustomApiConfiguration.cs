using FinancialServices.Api.Attributes;
using FinancialServices.Api.Contract;
using FinancialServices.Api.Middleware;
using FinancialServices.Api.Utils.Seed;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
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
                .AddCustomJobRunningConfiguration()
                ;

        }
        public static WebApplication UseCustomApiConfiguration(this WebApplication app)
        {
            app.UseApplicationEndpoints();
            app.UseCustomMiddlewares();
            app.UseCustomSwagger();

            app.UseAuthentication();
            app.UseAuthorization();

            var settings = app.Services.GetRequiredService<IOptions<ApplicationSettingsModel>>().Value;

            if (app.Environment.IsDevelopment())
            {
                if (settings.UseDevelopmentTransactionSeed)
                    app.UseDevelopmentSeed();
            }

            if (settings.UseSubscriptions)
                app.UseSubscriptons();

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
                    policy.RequireRole("credito", "debito"));
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
        private static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ApiKeyRequestHandlerMiddleware>();
            return app;
        }

        private static void UseDevelopmentSeed(this WebApplication app)
        {
            var transactionRepository = app.Services.GetRequiredService<IRepository<TransactionEntity>>();

            // Se ja existe registro no banco nao efetua a criação dos registros
            if (transactionRepository.Query().Any())
                return;


            var days = Enumerable
                .Range(0, 5)
                .Select(p => DateTime.UtcNow.AddDays(p * -1).Date)
                .OrderBy(p => p)
                .ToList()
                ;

            foreach (var dt in days)
            {
                TransactionEntityGenerator.GenerateDailyTransactions(dt, new TransactionEntityGeneratorRequest()
                {
                    EasyMomentTransactionCreationFactor = 10, // 20% do total de transações do dia
                    EasyBorderDurationFactor = 20, // 70% do tempo do Momento calmo é transição da borda do momento
                    EasyBorderMomentTransactionCreationFactor = 60, // 70% do total de transações do periodo calmo    
                    EasyMomentStart = new TimeSpan(2, 0, 0), // Horário de início do momento calmo (incluindo a borda)
                    EasyMomentEnd = new TimeSpan(9, 30, 0), // Horário de término do momento calmo (incluindo a borda)

                    PeakMomentTransactionCreationFactor = 50, // 50% do total de transações do dia
                    PeakBorderDurationFactor = 20, // 20% do tempo do Momento de pico é transição da borda do momento
                    PeakBorderMomentTransactionCreationFactor = 35, // 20% do total de transações do periodo de pico
                    PeakMomentStart = new TimeSpan(18, 15, 0), // Horário de início do momento de pico (incluindo a borda)
                    PeakMomentEnd = new TimeSpan(22, 0, 0), // Horário de término do momento de pico (incluindo a borda)

                    TransactionCountAvgPerMinute = 30 // Média de 30 transações por minuto

                }, app);
            }

            // Cria o relatório
            var createConsolidatedReportsUseCase = app.Services.GetRequiredService<ICreateConsolidatedReportsUseCase>();

            // Cria os grupos
            createConsolidatedReportsUseCase.CreateTransactionGroups([TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo")]);


        }

    }

}
