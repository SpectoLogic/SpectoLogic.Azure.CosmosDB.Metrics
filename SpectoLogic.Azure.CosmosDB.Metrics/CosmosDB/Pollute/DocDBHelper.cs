using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Threading.Tasks;

namespace SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Pollute
{
    public class DocDBHelper
    {
        private string _cosmosDBAccount = null;
        private string _cosmosDBDatabaseName = null;
        private string _cosmsoDBCollectionName = null;
        private string _cosmosDBKey = null;

        private DocumentClient _client = null;

        private string _colSelfLink = null;

        public DocDBHelper(string account, string database, string collection, string key)
        {
            _cosmosDBKey = key;
            _cosmosDBAccount = account;
            _cosmosDBDatabaseName = database;
            _cosmsoDBCollectionName = collection;
            _client = new DocumentClient(new Uri($"https://{account}.documents.azure.com:443/"), _cosmosDBKey);
        }

        public async Task Connect()
        {
            Database db = new Database()
            {
                Id = _cosmosDBDatabaseName
            };
            db = await _client.CreateDatabaseIfNotExistsAsync(db);
            DocumentCollection col = new DocumentCollection()
            {
                Id = _cosmsoDBCollectionName
            };
            col = await _client.CreateDocumentCollectionIfNotExistsAsync(db.SelfLink, col);
            _colSelfLink = col.SelfLink;
        }

        public async Task AddData()
        {
            Task[] mTasks = new Task[100];
            for (int i = 0; i < 100; i++)
            {
                mTasks[i] = CreateDocs();
            }
            await Task.WhenAll(mTasks);
        }

        private async Task CreateDocs()
        {
            for (int i = 0; i < 2000; i++)
            {
                await _client.CreateDocumentAsync(_colSelfLink, new DocDBTestObject());
            }
        }
    }
}
