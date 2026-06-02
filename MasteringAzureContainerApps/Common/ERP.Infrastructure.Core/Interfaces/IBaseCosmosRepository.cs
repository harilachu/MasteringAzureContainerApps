using ERP.Common.Core;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Core.Interfaces
{
    public interface IBaseCosmosRepository<T> where T : class
    {
        Task<Result<T>> GetItemAsync<T>(string id, string partitionKey, CancellationToken cancellationToken);
        Task<Result<List<T>>> GetItemsByQueryAsync<T>(QueryDefinition queryDefinition, CancellationToken cancellationToken);
        Task<Result> AddItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken);
        Task<Result> UpdateItemAsync<T>(string id, T item, CancellationToken cancellationToken);
        Task<Result> DeleteItemAsync(string id, string partitionKey, CancellationToken cancellationToken);
    }
}
