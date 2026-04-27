using Dummy;
using Greet;
using Grpc.Core;

const string Target = "127.0.0.1:50051";

Channel channel = new(Target, ChannelCredentials.Insecure);

channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

//var client = new DummyService.DummyServiceClient(channel);
var client = new GreetingService.GreetingServiceClient(channel);

var greeting = new Greeting() { FirstName = "John", LastName = "Rush" };

var request = new GreetingRequest() { Greeting = greeting };
var response = client.Greet(request);

Console.WriteLine(response?.Result);

await channel.ShutdownAsync();
Console.ReadKey();