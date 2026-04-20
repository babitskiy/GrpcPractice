using Dummy;
using Grpc.Core;

const string Target = "127.0.0.1:50051";

Channel channel = new(Target, ChannelCredentials.Insecure);

channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

var client = new DummyService.DummyServiceClient(channel);

channel.ShutdownAsync().WaitAsync();
Console.ReadKey();