using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Kentico.Kontent.Delivery;
using Microsoft.Extensions.Configuration;
using JukeBox.Core;
using System.Linq;

namespace JukeBoxFunctions
{
    public static class Jukebox
    {
        [FunctionName("SongList")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var config = new ConfigurationBuilder()
                            .SetBasePath(context.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            IDeliveryClient client = DeliveryClientBuilder.WithProjectId(config["KontentProjectId"]).Build();

            DeliveryItemListingResponse<Song> listingResponse = await client.GetItemsAsync<Song>();
            

            var songs = listingResponse.Items.Select(x=> x.Title).ToArray();

            return new OkObjectResult(songs);
        }
    }
}
