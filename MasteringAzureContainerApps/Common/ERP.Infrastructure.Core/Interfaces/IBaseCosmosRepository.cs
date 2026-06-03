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
        Task<Result<T>> AddItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken);
        Task<Result<T>> UpdateItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken);
        Task<Result<T>> DeleteItemAsync(string id, string partitionKey, CancellationToken cancellationToken);
    }
}
