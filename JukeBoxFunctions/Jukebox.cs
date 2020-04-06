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
using Nexmo.Api;
using System.Linq;
using System.Text;
using System.Collections.Generic;

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

            string API_KEY = config["NEXMO_API_KEY"];
            string API_SECRET = config["NEXMO_API_SECRET"];
            
             var nexmoClient = new Client(creds: new Nexmo.Api.Request.Credentials(
                nexmoApiKey: API_KEY, nexmoApiSecret: API_SECRET));

            var results = nexmoClient.SMS.Send(new SMS.SMSRequest
            {
                from = req.Query["to"],
                to = req.Query["msisdn"],
                text = ConvertToNumberedList(songs)
            });


            return new OkObjectResult(songs);

            string ConvertToNumberedList(IEnumerable<string> songs)
            {
                StringBuilder sb = new StringBuilder();
                int songNumber = 1;
                foreach(var s in songs)
                {
                    sb.AppendLine($"{songNumber++} - {s}");
                }
                return sb.ToString();
            }
        }
    }
}

