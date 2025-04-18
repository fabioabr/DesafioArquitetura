﻿using FinancialServices.Domain.Core.Attributes;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Infrastructure.Data.Contract;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Transactions;

namespace FinancialServices.Infrastructure.Data.Adapter
{
    public class MongoDBDatabaseAdapter : IDatabaseAdapter
    {
        private readonly MongoClient client;
        private readonly string databaseName;
        
        public MongoDBDatabaseAdapter(string connectionString, string databaseName)
        {
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
            
            client = new MongoClient(settings);
            this.databaseName = databaseName;            
        }
        private IMongoCollection<T> GetCollection<T>() where T : IEntity
        {
            var type = typeof(T);
            var tableAttr = type.GetCustomAttribute<EntitySetNameAttribute>();

            if (tableAttr == null)
                throw new InvalidOperationException("EntitySetNameAttribute is required for MongoDB collections");

            var collectionNamespace =  tableAttr?.Name;
            return client.GetDatabase(databaseName).GetCollection<T>(collectionNamespace);
        }
        public IQueryable<T> Query<T>() where T : IEntity
        {
            return GetCollection<T>().AsQueryable();
        }
        public async Task AddAsync<T>(T entity) where T : IEntity
        {
            await GetCollection<T>().InsertOneAsync(entity);            
        }
        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : IEntity
        {
            await GetCollection<T>().InsertManyAsync(entities);
        }         
        public async Task RemoveAsync<T>(T entity) where T : IEntity
        {
            await GetCollection<T>().DeleteOneAsync(p=>p.Id == entity.Id);            
        }        
        public async Task RemoveRangeAsync<T>(IEnumerable<T> entities) where T : IEntity
        {            
            await GetCollection<T>().DeleteManyAsync(p => entities.Any(x => x.Id == p.Id));
        }         
        public async Task UpdateAsync<T>(T entity, object updateFields) where T : IEntity
        {
            // Usar uma abordagem de adapter com interface genérica para permitir a utilização de diferentes tipos de banco de dados
            // pode causar um problema relacionado ao mongodb pois neste caso temos 2 tipo de atualização de registros...
            // 1. Substituicao completa do registro
            // 2. Atualização de campos específicos

            // No caso do metodo genérico de Update, o MongoDB irá substituir o registro completo, o que pode ser um problema quando
            // estivermos enfrentando concorrencia de updates. Neste caso, seria melhor utilizar o método ReplaceOneAsync, que permite
            // a atualizção de campos específicos. E é seguro para concorrência.

            // Para resolver este problema, podemos criar um novo método UpdateAsync que aceita um objeto com os campos a serem atualizados
            // e utiliza o método ReplaceOneAsync para atualizar apenas os campos necessários.
            // passando um objeto Anonimo como parametro^.
            // Exemplo : 
            // await UpdateAsync(entity, new { Name = "New Name" });

            var update = new BsonDocument("$set", updateFields.ToBsonDocument());

            var filter = Builders<T>.Filter.Eq("_id", entity.Id);
            
            await GetCollection<T>().UpdateOneAsync(filter, update);

        }
        public async Task InsertOrUpdate<T>(T entity) where T : IEntity
        {
            // Este metodo diferentemente do outro, faz o replace do registro no banco.
            // Se o registro não existir, ele será criado.

            var filter = Builders<T>.Filter.Eq(t => t.Id, entity.Id);

            var options = new ReplaceOptions { IsUpsert = true };
            
            await GetCollection<T>().ReplaceOneAsync(filter, entity, options);

        }


    }
}
