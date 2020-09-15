using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Extensions;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace Azure.Search.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string serviceName = "";
            string indexName = "";
            string apiKey = "";

            if (String.IsNullOrEmpty(apiKey)) throw new InvalidOperationException("Update with your settings");

            // Create a SearchIndexClient to send create/delete index commands
            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(apiKey);
            SearchIndexClient index = new SearchIndexClient(serviceEndpoint, credential);

            // Create a SearchClient to load and query documents
            SearchClient queryClient = new SearchClient(serviceEndpoint, indexName, credential);

            var count = await queryClient.GetDocumentCountAsync();

            Console.WriteLine($"Found {count} docs");

            SearchIndexerClient indexer = new SearchIndexerClient(serviceEndpoint, credential);

            (await indexer.GetIndexerNamesAsync()).Value
                .ToList()
                .ForEach((item) => Console.WriteLine($"Index: {item}"));

            // await index.UploadSampleDocumentAsync(indexName, apiKey);
            // await index.DeleteDocumentAsync(indexName, "todo", apiKey);
        }
    }
}
