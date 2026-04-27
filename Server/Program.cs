using Calculation;
using Greet;
using Grpc.Core;
using server;
using Server;

const int Port = 50051;

Grpc.Core.Server server = null;

try
{
	server = new Grpc.Core.Server()
	{
		Services = {	GreetingService.BindService(new GreetingServiceImpl())
						, CalculationService.BindService(new CalculationServiceImpl())
						},
		Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
	};

	server.Start();

    Console.WriteLine("The server is listening on the port " + Port);
	Console.ReadKey();
}
catch (IOException e)
{
    Console.WriteLine("The server failed to start " + e.Message);
    throw;
}
finally
{
	if (server is not null)
		await server.ShutdownAsync();
}