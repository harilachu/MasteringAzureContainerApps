using ERP.Common.Core;
using ERP.Common.Domain;
using ERP.Infrastructure.Core.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Infrastructure.Core
{
    public class BaseCosmosRepository<T> : IBaseCosmosRepository<T> where T : class
    {
        private readonly Container container;
        private readonly ILogger<BaseCosmosRepository<T>> logger;
        private readonly IOptionsMonitor<AppConfig> appConfig;

        private AppConfig AppConfig => appConfig.CurrentValue;

        public BaseCosmosRepository(CosmosClient cosmosClient, ILogger<BaseCosmosRepository<T>> logger, IOptionsMonitor<AppConfig> appConfig)
        {
            this.logger = logger;
            this.appConfig = appConfig;
            var databaseName = AppConfig.Cosmos.DatabaseName;
            var containerName = typeof(T).Name + "s";
            this.container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<Result> AddItemAsync<T>(T item, string partitionKey, CancellationToken cancellationToken)
        {
            try
            {
                await container.CreateItemAsync(item, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding item with partition key '{PartitionKey}'", partitionKey);
                return Result.Failure(new Error("InternalError", "An error occurred while adding the item."));
            }
        }

        public async Task<Result> DeleteItemAsync(string id, string partitionKey, CancellationToken cancellationToken)
        {
            try
            {
                await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting item with id '{Id}' and partition key '{PartitionKey}'", id, partitionKey);
                return Result.Failure(new Error("InternalError", "An error occurred while deleting the item."));
            }
        }

        public async Task<Result<T>> GetItemAsync<T>(string id, string partitionKey, CancellationToken cancellationToken)
        {
            try
            {
                var response = await container.ReadItemAsync<T>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
                return Result<T>.Success(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogWarning("Item with id '{Id}' and partition key '{PartitionKey}' not found", id, partitionKey);
                return Result<T>.Failure(new Error("NotFound", $"Item with id '{id}' and partition key '{partitionKey}' not found."));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving item with id '{Id}' and partition key '{PartitionKey}'", id, partitionKey);
                return Result<T>.Failure(new Error("InternalError", "An error occurred while retrieving the item."));
            }
        }

        public async Task<Result<List<T>>> GetItemsByQueryAsync<T>(QueryDefinition queryDefinition, CancellationToken cancellationToken)
        {
            try
            {
                using FeedIterator<T> feed = container.GetItemQueryIterator<T>(
                    queryDefinition
                );

                List<T> items = new List<T>();
                while (feed.HasMoreResults)
                {
                    FeedResponse<T> response = await feed.ReadNextAsync();
                    foreach (T item in response)
                    {
                        items.Add(item);
                    }
                }

                return Result<List<T>>.Success(items);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogWarning("Item not found for given Query: {Query}", queryDefinition.QueryText);
                return Result<List<T>>.Failure(new Error("NotFound", "Item not found for the given query."));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving item for given Query: {Query}", queryDefinition.QueryText);
                return Result<List<T>>.Failure(new Error("InternalError", "An error occurred while retrieving the item."));
            }
        }

        public async Task<Result> UpdateItemAsync<T>(string id, T item, CancellationToken cancellationToken)
        {
            try
            {
                await container.UpsertItemAsync(item, new PartitionKey(id), cancellationToken: cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating item with id '{Id}'", id);
                return Result.Failure(new Error("InternalError", "An error occurred while updating the item."));
            }
        }
    }
}
