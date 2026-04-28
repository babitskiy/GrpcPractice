using Calculation;
using Greet;
using Grpc.Core;

const string Target = "127.0.0.1:50051";

Channel channel = new(Target, ChannelCredentials.Insecure);

await channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

#region GreetingService
var client = new GreetingService.GreetingServiceClient(channel);

var greeting = new Greeting() { FirstName = "John", LastName = "Rush" };

var request = new GreetingRequest() { Greeting = greeting };
var response = client.Greet(request);
Console.WriteLine(response?.Result);

var greetManyTimesResponse = client.GreetManyTimes(new GreetManyTimesRequest() { Greeting = greeting });
while (await greetManyTimesResponse.ResponseStream.MoveNext())
{
    Console.WriteLine(greetManyTimesResponse.ResponseStream.Current.Result);
    await Task.Delay(500);
}
#endregion GreetingService

#region CalculationService
var calcClient = new CalculationService.CalculationServiceClient(channel);

var calcAddResponse = calcClient.Add(new CalculationRequest() { FirstParam = 5, SecondParam = 10 });
Console.WriteLine(calcAddResponse.Result);

var calcMultipyResponse = calcClient.Multiply(new CalculationRequest() { FirstParam = 5, SecondParam = 10});
Console.WriteLine(calcMultipyResponse.Result);

var calcDivideResponse = calcClient.Divide(new CalculationRequest() { FirstParam = 5, SecondParam = 10});
Console.WriteLine(calcDivideResponse.Result);

var primeNumberDecomposition = calcClient.PrimeNumberDecomposition(new PrimeNumberDecompositionRequest() { PrimeNumber = 120});
while (await primeNumberDecomposition.ResponseStream.MoveNext())
    Console.WriteLine("Part of prime number decomposition: {0}", primeNumberDecomposition.ResponseStream.Current.Result);
#endregion CalculationService

await channel.ShutdownAsync();
Console.ReadKey();