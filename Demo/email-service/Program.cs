using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using contracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var asbDemoKey = configuration["AsbDemoKey"];

            var connectionString = new ServiceBusConnectionStringBuilder();
            connectionString.Endpoint = "launchb-techsession.servicebus.windows.net";
            connectionString.SasKeyName = "RootManageSharedAccessKey";
            connectionString.SasKey = asbDemoKey;
            connectionString.EntityPath = "email-service";

            var queueClient = new QueueClient(connectionString);

            queueClient.RegisterMessageHandler(HandleMessage, ExceptionHandler);

            Console.WriteLine("Press ESC to close");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape) break;
            }
        }

        private static Task HandleMessage(Message message, CancellationToken cancellationToken)
        {
            var body = JsonConvert.DeserializeObject<UserCreated>(Encoding.UTF8.GetString(message.Body));
            Console.WriteLine($"Sending welcome email to {body.UserName}");
            return Task.CompletedTask;
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Exception: {arg.Exception}");
            return Task.CompletedTask;
        }
    }
}
