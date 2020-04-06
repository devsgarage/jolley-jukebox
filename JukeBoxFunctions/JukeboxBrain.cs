using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Jukebox.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Nexmo.Api;

namespace JukeBoxFunctions
{
    public static class JukeboxBrain
    {
        [FunctionName("JukeboxBrain")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
            { 
            var config = new ConfigurationBuilder()
                            .SetBasePath(context.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();
            var command = ParseMessage(req.Query["text"]);

            switch (command.command)
            {
                case ReadOnlyMemory<char> t when t.Span.Equals("list".AsSpan(), StringComparison.OrdinalIgnoreCase):
                    await SendSongList(req.Query["msisdn"], req.Query["to"], config);
                    break;
                case ReadOnlyMemory<char> t when t.Span.Equals("play".AsSpan(), StringComparison.OrdinalIgnoreCase):
                    await PlaySong(req.Query["msisdn"], req.Query["to"], Convert.ToInt32(command.parameter.ToString()), config);
                    break;

            }

            return new OkObjectResult("200");

            (ReadOnlyMemory<char> command, ReadOnlyMemory<char> parameter) ParseMessage(string messageText)
            {
                var command = messageText.AsMemory();
                var commandParameter = ReadOnlyMemory<char>.Empty;
                var splitLocation = command.Span.IndexOf(' ');
                if (splitLocation != -1)
                {
                    commandParameter = command.Slice(splitLocation + 1);
                    command = command.Slice(0, splitLocation);
                }
                return (command, commandParameter);
            }
        }

        private static async Task PlaySong(StringValues stringValues1, StringValues stringValues2, int songNumber, IConfigurationRoot config)
        {
            var song = await JukeboxHelpers.GetSong(config["KontentProjectId"], songNumber);
        }

        private static async Task SendSongList(string toNumber, string fromNumber, IConfigurationRoot config)
        {
            var songlist = await JukeboxHelpers.GetStringifiedSongList(config["KontentProjectId"]);
            string API_KEY = config["NEXMO_API_KEY"];
            string API_SECRET = config["NEXMO_API_SECRET"];

            var nexmoClient = new Client(creds: new Nexmo.Api.Request.Credentials(
               nexmoApiKey: API_KEY, nexmoApiSecret: API_SECRET));

            var results = nexmoClient.SMS.Send(new SMS.SMSRequest
            {
                from = fromNumber,
                to = toNumber,
                text = songlist
            });
        }
    }
}