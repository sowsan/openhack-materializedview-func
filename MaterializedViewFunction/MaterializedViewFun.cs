using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MaterializedViewFunction
{
    public static class MaterializedViewFun
    {
        private static readonly string _endpointUrl = "https://team33contoso.documents.azure.com:443/";
        private static readonly string _primaryKey = "<Inser the Azure Cosmos DB Key here>";
        private static readonly string _databaseId = "MoviesV2";
        //private static readonly string _containerId = "MoviewSales";
        private static CosmosClient cosmosClient = new CosmosClient(_endpointUrl, _primaryKey);

        [FunctionName("MaterializedViewFun")]
        public static async Task RunAsync([CosmosDBTrigger(
            databaseName: "MoviesV2",
            collectionName: "OrderEvents",
            ConnectionStringSetting = "DBConnection",
            CreateLeaseCollectionIfNotExists = true,           
            LeaseCollectionName = "materializedViewLeases")]IReadOnlyList<Document> input, ILogger log)
        {
            var db = cosmosClient.GetDatabase(_databaseId);
           // var container = db.GetContainer(_containerId);
            var movieContainer = db.GetContainer("Movies");
            var movieSalesContainer = db.GetContainer("MovieSales");

           // var tasks = new List<Task>();

            if (input != null && input.Count > 0)
            {
                var stateDict = new Dictionary<string, List<int>>();

                foreach (var doc in input)
                {
                    var movieOrder = JsonConvert.DeserializeObject<MoviewOrder>(doc.ToString());

                    foreach (Detail detail in movieOrder.Details)
                    {
                      
                        List<Movie> movies = new List<Movie>();
                        string queryText = "SELECT distinct c.ProductName,c.ItemId From Movies as c where c.ItemId=" + detail.ProductId;

                        // 0 maximum parallel tasks, effectively serial execution
                        QueryRequestOptions options = new QueryRequestOptions() { MaxBufferedItemCount = 100 };
                        options.MaxConcurrency = 0;
                        FeedIterator<Movie> query = movieContainer.GetItemQueryIterator<Movie>(
                            queryText,
                            requestOptions: options);
                        while (query.HasMoreResults)
                        {
                            foreach (Movie movie in await query.ReadNextAsync())
                            {
                                movies.Add(movie);
                            }
                        }
                                              

                        foreach (Movie movie in movies)
                        {
                            List<MovieSale> movieSales = new List<MovieSale>();
                            string querySalesText = "Select * from MovieSales m where m.id='" + movie.ItemId.ToString() + "'";
                        
                            // 0 maximum parallel tasks, effectively serial execution
                            QueryRequestOptions querySalesOptions = new QueryRequestOptions() { MaxBufferedItemCount = 100 };
                            options.MaxConcurrency = 0;

                            FeedIterator<MovieSale> querySales = movieSalesContainer.GetItemQueryIterator<MovieSale>(
                                querySalesText,
                                requestOptions: querySalesOptions);

                            //while (querySales.HasMoreResults)
                            //{
                            //    foreach (MovieSale sale in await querySales.ReadNextAsync())
                            //    {
                            //       // movies.Add(movie);
                            //    }
                            //}

                            if (querySales.HasMoreResults)
                            {
                                var movieSale = (await querySales.ReadNextAsync()).FirstOrDefault();
                                //MovieSale movieSale = null;

                                if (movieSale != null)
                                {
                                    movieSale.NumberOfSales = movieSale.NumberOfSales + 1;
                                }
                                else
                                {
                                    movieSale = new MovieSale();
                                    movieSale.Name = movie.ProductName;
                                    movieSale.MovieId = movie.ItemId.ToString();
                                    //movieSale.SalesId = movie.ItemId;
                                    movieSale.NumberOfSales = 1;
                                }
                               

                                log.LogInformation("Upserting materialized view document");
                               // await movieSalesContainer.CreateItemAsync(movieSale, new Microsoft.Azure.Cosmos.PartitionKey(movieSale.SalesId.ToString()));
                                var newSale =  await movieSalesContainer.UpsertItemAsync<MovieSale>(movieSale, new Microsoft.Azure.Cosmos.PartitionKey(movieSale.MovieId.ToString()));
                            }
                        }

                    }

                }

            }
        }
    }
}
