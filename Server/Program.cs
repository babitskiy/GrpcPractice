using Grpc.Core;

const int Port = 50051;

Server server = null;

try
{
	server = new Server()
	{
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
		await server.ShutdownAsync().wai;
}