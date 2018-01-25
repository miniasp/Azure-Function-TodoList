using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace TodoFunction
{
    public static class SlackBot
    {
        [FunctionName("SlackBot")]
        public static void Run([QueueTrigger("slack-queue")]string slackQueue, TraceWriter log)
        {
            log.Info($"New Todo: {slackQueue}");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "AzureFunctions");
                var uri = "https://hooks.slack.com/services/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";
                var msg = new SlackHook { text = $"{slackQueue}" };

                var slackMsg = new StringContent(JsonConvert.SerializeObject(msg));
                HttpResponseMessage response = client.PostAsync(uri, slackMsg).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                log.Info($"Slack Response: {responseString}");
            }
        }
    }

    public class SlackHook
    {
        public string text { get; set; }
        public string icon_emoji { get; set; }
    }
}
