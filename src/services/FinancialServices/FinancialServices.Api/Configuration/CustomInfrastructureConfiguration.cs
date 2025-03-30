﻿using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Infrastructure.Data.Contract;
using FinancialServices.Infrastructure.Data.Factory;
using FinancialServices.Infrastructure.Data.Repository;
using FinancialServices.Infrastructure.Enum;
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
            var databaseToUse = Environment.GetEnvironmentVariable("CustomSettings__DatabaseToUse") ?? throw new System.Exception("CustomSettings__DatabaseToUse is not set");            
            var databaseName = Environment.GetEnvironmentVariable("CustomSettings__DatabaseName") ?? throw new System.Exception("CustomSettings__DatabaseName is not set");
            var connectionStrng = Environment.GetEnvironmentVariable("CustomSettings__ConnectionStrings__Default") ?? throw new System.Exception("CustomSettings__ConnectionStrings__Default is not set");
            var dbType = Enum.Parse<DatabaseTypeEnum>(databaseToUse, true);

            builder.Services.AddSingleton<IDatabaseAdapter>(sp =>
            {
                return new DatabaseAdapterFactory()
                    .CreateDatabaseAdapter(dbType, connectionStrng, databaseName);
            });

            #region Configurações relacionadas ao Mongo DB
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            var conventionPack = new ConventionPack{
                new CamelCaseElementNameConvention()
            };

            ConventionRegistry.Register(
                "CamelCase",
                conventionPack,
                t => true // aplica para todas as classes
            );
            #endregion 

            builder.Services.AddSingleton<IRepository<UserEntity>, Repository<UserEntity>>();

            return builder;
                
        }

    }
}
