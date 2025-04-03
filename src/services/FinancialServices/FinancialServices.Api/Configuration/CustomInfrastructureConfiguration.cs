using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Model;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Data.Factory;
using FinancialServices.Infrastructure.Data.Repository;
using FinancialServices.Infrastructure.Events.Adapter;
using FinancialServices.Infrastructure.Events.Publishers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace FinancialServices.Api.Configuration
{
    public static class CustomInfrastructureConfiguration
    {
        public static WebApplicationBuilder AddCustomInfrastructureConfiguration(this WebApplicationBuilder builder)
        {    
            builder.AddDatabaseConfiguration();
            builder.AddEventBusConfiguration();

            return builder;

        }
        public static WebApplicationBuilder AddDatabaseConfiguration(this WebApplicationBuilder builder)
        {
            var settings = builder.Configuration.GetSection("CustomSettings").Get<ApplicationSettingsModel>()!;

            var dbType = settings.DatabaseToUse;
            
            builder.Services.AddSingleton<IDatabaseAdapter>(sp =>
            {
                return new DatabaseAdapterFactory(settings)
                    .CreateDatabaseAdapter();
            });

            #region Configurações relacionadas ao Mongo DB

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            ConventionRegistry.Register("EnumAsString", new ConventionPack{ new CamelCaseElementNameConvention() }, t => true);
            ConventionRegistry.Register("CamelCase", new ConventionPack{ new EnumRepresentationConvention(BsonType.String) }, t => true );
            
            #endregion 

            builder.Services.AddSingleton<IRepository<UserEntity>, Repository<UserEntity>>();
            builder.Services.AddSingleton<IRepository<TransactionEntity>, Repository<TransactionEntity>>();
            builder.Services.AddSingleton<IRepository<TransactionGroupingEntity>, Repository<TransactionGroupingEntity>>();
           
            return builder;
        }
        public static WebApplicationBuilder AddEventBusConfiguration(this WebApplicationBuilder builder)
        {
            var settings = builder.Configuration.GetSection("CustomSettings").Get<ApplicationSettingsModel>()!;

            


            builder.Services.AddSingleton<IEventPublisherAdapter>(sp =>
            {
                return new EventPublisherAdapterFactory(settings)
                    .CreateEventPublisherAdapter(sp.GetRequiredService<ILogger>());
            });



            builder.Services.AddSingleton<IEventPublisher<TransactionCreatedEventModel>, EventPublisher<TransactionCreatedEventModel>>();

            return builder;
        }
    }
}
