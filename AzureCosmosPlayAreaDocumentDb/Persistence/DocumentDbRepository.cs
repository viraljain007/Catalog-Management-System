﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace AzureCosmosPlayAreaDocumentDb.Persistence
{
    public class DocumentDbRepository<T> where T : class
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["DatabaseId"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["CollectionId"];
        private static DocumentClient _documentClient;

        public static void Initialize()
        {
            Uri serviceEndpoint = new Uri(ConfigurationManager.AppSettings["CosmosDbUri"]);
            string authKey = ConfigurationManager.AppSettings["CosmosDbKey"];

            _documentClient = new DocumentClient(serviceEndpoint, authKey);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                Uri documentCollectionLink = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
                await _documentClient.ReadDocumentCollectionAsync(documentCollectionLink);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<T> GetItemAsync(Guid id, string collectionId)
        {
            try
            {
                Document document = await _documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, collectionId, id.ToString()));

                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<T> GetItemAsync(Guid id)
        {
            try
            {
                Document document = await _documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id.ToString()));

                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<IList<T>> GetDropDownAsync(string collectionid)
        {
            try
            {
                var db = (await _documentClient.ReadDatabaseFeedAsync()).Single(d => d.Id == "Catalog");
                var col = (await _documentClient.ReadDocumentCollectionFeedAsync(db.CollectionsLink)).Single(c => c.Id == collectionid);
              
                IList<T> docs = _documentClient.CreateDocumentQuery<T>(col.DocumentsLink).ToList();
           
                foreach (var document in docs)
                {

                }

                return docs;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<IList<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _documentClient.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public static async Task<Document> CreateItemAsync(T item, string CollectionId)
        {
            return await _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public static async Task<Document> UpdateItemAsync(Guid id, T item , string CollectionId)
        {
            return await _documentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id.ToString()), item);
        }

        public static async Task DeleteItemAsync(Guid id ,string CollectionId)
        {
            await _documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id.ToString()));
        }
    }
}