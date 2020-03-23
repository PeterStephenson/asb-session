using System;
using System.Text;
using System.Threading.Tasks;
using contracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace publisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var asbDemoKey = configuration["AsbDemoKey"];

            var connectionString = new ServiceBusConnectionStringBuilder();
            connectionString.Endpoint = "launchb-techsession.servicebus.windows.net";
            connectionString.SasKeyName = "RootManageSharedAccessKey";
            connectionString.SasKey = asbDemoKey;
            connectionString.EntityPath = "user";

            var topicClient = new TopicClient(connectionString);

            while (true)
            {
                Console.WriteLine("Enter a name to send");
                var name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name)) continue;

                var message = new Message();
                message.ContentType = "UserCreated";
                message.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new UserCreated { UserName = name }));
                await topicClient.SendAsync(message);

                Console.WriteLine("Send another? (Y/N)");
                var key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Y) break;
            }

            await topicClient.CloseAsync();
        }
    }
}