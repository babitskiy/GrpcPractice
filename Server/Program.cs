using Calculation;
using Greet;
using Grpc.Core;
using server;
using Server;
using Sqrt;

const int Port = 50051;

Grpc.Core.Server server = null;

try
{
	var serverCert = File.ReadAllText("ssl/server.crt");
	var serverKey = File.ReadAllText("ssl/server.key");
	var keypair = new KeyCertificatePair(serverCert, serverKey);

	var cacert = File.ReadAllText("ssl/ca.crt");

	var credentials = new SslServerCredentials([keypair], cacert, true);

	server = new Grpc.Core.Server()
	{
		Services = {	  GreetingService.BindService(new GreetingServiceImpl())
						, CalculationService.BindService(new CalculationServiceImpl())
						, SqrtService.BindService(new SqrtServiceImpl())
                        },
		Ports = { new ServerPort("localhost", Port, credentials) }
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