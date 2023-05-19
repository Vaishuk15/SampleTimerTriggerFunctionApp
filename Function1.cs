using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TestFunctionApp
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("*/1 * * * *")]TimerInfo myTimer, ILogger log, [DurableClient] IDurableOrchestrationClient starter)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Orchestration", null);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            await starter.TerminateAsync(instanceId, "terminated");
        }

        [FunctionName("Orchestration")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            
            await context.CallActivityAsync<string>(nameof(GetTime), "get time");
            await context.CallActivityAsync<string>(nameof(GetMessage), "get message");
            return;
        }

        [FunctionName(nameof(GetTime))]
        public Task<string> GetTime([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Running GetTime Activity: {name}.");
            return Task.FromResult($"Now the time is :{DateTime.Now}");
        }

        [FunctionName(nameof(GetMessage))]
        public Task<string> GetMessage([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Running GetMessage Activity: {name}.");
            return Task.FromResult($"hello");
        }
    }
}
