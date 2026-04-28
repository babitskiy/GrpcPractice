using Calculation;
using Greet;
using Grpc.Core;
using Sqrt;

const string Target = "127.0.0.1:50051";

Channel channel = new(Target, ChannelCredentials.Insecure);

await channel.ConnectAsync();
Console.WriteLine("The client connected successfully");

#region GreetingService
var greetClient = new GreetingService.GreetingServiceClient(channel);

// Unary calls
//await DoGreet(greetClient);
//await DoGreetManyTimes(greetClient);

// Client streaming
//await DoLongGreet(greetClient);

// Bidirectional streaming
//await DoGreetEveryone(greetClient);
#endregion

#region CalculationService
var calculationClient = new CalculationService.CalculationServiceClient(channel);

// Unary calls
//await DoAdd(calcClient);
//await DoMultiply(calcClient);
//await DoDivide(calcClient);

// Server streaming
//await DoPrimeNumberDecomposition(calcClient);

// Client streaming
//await DoComputeAverage(calcClient);

// Bidirectional streaming
await DoFindMaximum(calculationClient);
#endregion

#region SqrtService
var sqrtClient = new SqrtService.SqrtServiceClient(channel);
await DoCalculateSqrt(sqrtClient);
#endregion

await channel.ShutdownAsync();
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
#region GreetingService Methods

static async Task DoGreet(GreetingService.GreetingServiceClient client)
{
    var greeting = new Greeting { FirstName = "John", LastName = "Rush" };
    var request = new GreetingRequest { Greeting = greeting };

    var response = await client.GreetAsync(request);
    Console.WriteLine(response?.Result);
}

static async Task DoGreetManyTimes(GreetingService.GreetingServiceClient client)
{
    var greeting = new Greeting { FirstName = "John", LastName = "Rush" };
    var request = new GreetManyTimesRequest { Greeting = greeting };

    var stream = client.GreetManyTimes(request);

    while (await stream.ResponseStream.MoveNext())
    {
        Console.WriteLine(stream.ResponseStream.Current.Result);
        await Task.Delay(500);
    }
}

static async Task DoLongGreet(GreetingService.GreetingServiceClient client)
{
    var stream = client.LongGreet();

    foreach (var i in Enumerable.Range(1, 10))
    {
        var greeting = new Greeting { FirstName = "John", LastName = "Rush" };
        var request = new LongGreetRequest { Greeting = greeting };

        await stream.RequestStream.WriteAsync(request);
        Console.WriteLine($"Sent greeting #{i}");
    }

    await stream.RequestStream.CompleteAsync();
    var response = await stream.ResponseAsync;

    Console.WriteLine($"Server response:\n{response.Result}");
}

static async Task DoGreetEveryone(GreetingService.GreetingServiceClient client)
{
    var stream = client.GreetEveryone();

    var responseReaderTask = Task.Run(async () =>
    {
        while (await stream.ResponseStream.MoveNext())
            Console.WriteLine("Received: " + stream.ResponseStream.Current.Result);
    });

    Greeting[] greetings =
    {
        new() { FirstName = "John", LastName = "Rush" },
        new() { FirstName = "Clement", LastName = "Doe" },
        new() { FirstName = "Emilia", LastName = "White" }
    };

    foreach (var greeting in greetings)
    {
        await stream.RequestStream.WriteAsync(new GreetEveryoneRequest { Greeting = greeting });
        Console.WriteLine($"Sent: {greeting.FirstName} {greeting.LastName}");
    }

    await stream.RequestStream.CompleteAsync();
    await responseReaderTask;
}

#endregion

#region CalculationService Methods

static async Task DoAdd(CalculationService.CalculationServiceClient client)
{
    var request = new CalculationRequest { FirstParam = 5, SecondParam = 10 };
    var response = await client.AddAsync(request);

    Console.WriteLine($"5 + 10 = {response.Result}");
}

static async Task DoMultiply(CalculationService.CalculationServiceClient client)
{
    var request = new CalculationRequest { FirstParam = 5, SecondParam = 10 };
    var response = await client.MultiplyAsync(request);

    Console.WriteLine($"5 * 10 = {response.Result}");
}

static async Task DoDivide(CalculationService.CalculationServiceClient client)
{
    var request = new CalculationRequest { FirstParam = 5, SecondParam = 10 };
    var response = await client.DivideAsync(request);

    Console.WriteLine($"5 / 10 = {response.Result}");
}

static async Task DoPrimeNumberDecomposition(CalculationService.CalculationServiceClient client)
{
    var request = new PrimeNumberDecompositionRequest { PrimeNumber = 120 };

    var primeNumberDecomposition = client.PrimeNumberDecomposition(request);
    while (await primeNumberDecomposition.ResponseStream.MoveNext())
        Console.WriteLine("Part of prime number decomposition: {0}", primeNumberDecomposition.ResponseStream.Current.Result);
}

static async Task DoComputeAverage(CalculationService.CalculationServiceClient client)
{
    var stream = client.ComputeAverage();

    int[] numbers = { 10, 15, 18, 20, 22, 30 };

    Console.Write("Calculating average of: ");
    foreach (var number in numbers)
    {
        await stream.RequestStream.WriteAsync(new ComputeAverageRequest { Number = number });
        Console.Write($"{number} ");
    }

    Console.WriteLine();

    await stream.RequestStream.CompleteAsync();
    var response = await stream.ResponseAsync;

    Console.WriteLine($"Average = {response.AverageNumber:F2}");
}

static async Task DoFindMaximum(CalculationService.CalculationServiceClient client)
{
    var stream = client.FindMaximum();
    var responseReaderTask = Task.Run(async () =>
    {
        while (await stream.ResponseStream.MoveNext())
            Console.WriteLine("Current maximum is " + stream.ResponseStream.Current.MaxNumber);
    });

    foreach (var number in new int[] { 1, 5, 3, 6, 2, 20 })
        await stream.RequestStream.WriteAsync(new FindMaximumRequest() { Number = number });

    await stream.RequestStream.CompleteAsync();
    await responseReaderTask;
}

#endregion

#region SqrtService Methods
async Task DoCalculateSqrt(SqrtService.SqrtServiceClient client)
{
    int number = -1;

    try
    {
        var response = client.sqrt(new SqrtRequest() { Number = number });

        Console.WriteLine(response.SquareRoot);
    }
    catch (RpcException e)
    {
        Console.WriteLine("Error: " + e.Status.Detail);
    }
}
#endregion