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
var greetClient = new GreetingService.GreetingServiceClient(channel);

var greeting = new Greeting() { FirstName = "John", LastName = "Rush" };

var greetingRequest = new GreetingRequest() { Greeting = greeting };
var greetingResponse = greetClient.Greet(greetingRequest);
Console.WriteLine(greetingResponse?.Result);

var greetManyTimesResponse = greetClient.GreetManyTimes(new GreetManyTimesRequest() { Greeting = greeting });
while (await greetManyTimesResponse.ResponseStream.MoveNext())
{
    Console.WriteLine(greetManyTimesResponse.ResponseStream.Current.Result);
    await Task.Delay(500);
}

var longGreetRequest = new LongGreetRequest() { Greeting = greeting };
var longGreetStream = greetClient.LongGreet();
foreach (var i in Enumerable.Range(1, 10))
    await longGreetStream.RequestStream.WriteAsync(longGreetRequest);

await longGreetStream.RequestStream.CompleteAsync();
var longGreetResponse = await longGreetStream.ResponseAsync;
Console.WriteLine(longGreetResponse.Result);
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

var computeAverageStream = calcClient.ComputeAverage();
foreach (var number in new int[] { 10, 15, 18, 20, 22, 30 })
    await computeAverageStream.RequestStream.WriteAsync(new ComputeAverageRequest() { Number = number});

await computeAverageStream.RequestStream.CompleteAsync();
var computeAverageResult = await computeAverageStream.ResponseAsync;
Console.WriteLine(computeAverageResult.AverageNumber);
#endregion CalculationService

    await channel.ShutdownAsync();
Console.ReadKey();