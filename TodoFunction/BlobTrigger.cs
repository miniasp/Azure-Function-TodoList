using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace TodoFunction
{
    public static class BlobTrigger
    {
        [FunctionName("BlobTrigger")]
        public static void Run(
            [BlobTrigger("images/{name}")]Stream myBlob, string name, TraceWriter log, 
            [Queue("slack-queue")] out string myQueue)
        {
            myQueue = $"¤W¶ÇÀÉ®× {name}, size:{myBlob.Length}";
        }
    }
}
