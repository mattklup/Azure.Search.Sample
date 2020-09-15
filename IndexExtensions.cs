using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure;
using Azure.Core.Extensions;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace Azure.Search.Sample
{
    static class IndexExtensions
    {
        // Based off of https://docs.microsoft.com/en-us/rest/api/searchservice/addupdate-or-delete-documents#request-headers
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task UploadSampleDocumentAsync(this SearchIndexClient index, string indexName, string apiKey)
        {
            Console.WriteLine($"Update Doc: to {index.ServiceName} with index {indexName} and key {apiKey}");
            Uri serviceEndpoint = new Uri($"https://{index.ServiceName}.search.windows.net/indexes/{indexName}/docs/index?api-version=2020-06-30");
            httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

            StringContent content = new StringContent(JsonSerializer.Serialize(
                new
                {
                    value = new[]
                    {
                        new IndexDocument
                        {
                            SearchAction = "upload",
                            Name = "todo.test",
                            Key = "todotest",
                        }
                    }
                }
            ));
            content.Headers.ContentType.MediaType = "application/json";

            var response = await httpClient.PostAsync(serviceEndpoint, content);
            response.EnsureSuccessStatusCode();
        }

        public static async Task DeleteDocumentAsync(this SearchIndexClient index, string indexName, string documentName, string apiKey)
        {
            Uri serviceEndpoint = new Uri($"https://{index.ServiceName}.search.windows.net/indexes/{indexName}/docs/index?api-version=2020-06-30");
            httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

            StringContent content = new StringContent(JsonSerializer.Serialize(
                new
                {
                    value = new[]
                    {
                        new IndexDocument
                        {
                            SearchAction = "delete",
                            Key = documentName,
                        }
                    }
                }
            ));
            content.Headers.ContentType.MediaType = "application/json";

            var response = await httpClient.PostAsync(serviceEndpoint, content);
            response.EnsureSuccessStatusCode();
        }

        private class IndexDocument
        {
            [JsonPropertyName("@search.action")]
            public string SearchAction { get; set; }

            [JsonPropertyName("metadata_storage_name")]
            public string Name { get; set; }

            [JsonPropertyName("metadata_storage_path")]
            public string Key { get; set; }
        }
    }
}