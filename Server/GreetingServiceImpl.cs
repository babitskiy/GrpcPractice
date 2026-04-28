using Greet;
using Grpc.Core;
using System.Text;
using static Greet.GreetingService;

namespace Server
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            string result = String.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            return Task.FromResult(new GreetingResponse() { Result = result });
        }

        public override async Task GreetManyTimes(GreetManyTimesRequest request, IServerStreamWriter<GreetManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request : ");
            Console.WriteLine(request.ToString());

            string result = String.Format(" hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            foreach (var i in Enumerable.Range(1, 10))
                await responseStream.WriteAsync(new GreetManyTimesResponse() { Result = i + result});
        }

        public override async Task<LongGreetResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("The client connected successfully");
            StringBuilder result = new();

            while (await requestStream.MoveNext())
                result.AppendLine($"Hello {requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName}");

            return new LongGreetResponse() { Result = result.ToString() };
        }

        public override async Task GreetEveryone(IAsyncStreamReader<GreetEveryoneRequest> requestStream, IServerStreamWriter<GreetEveryoneResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var result = String.Format("Hello {0} {1}",
                                            requestStream.Current.Greeting.FirstName,
                                            requestStream.Current.Greeting.LastName);

                Console.WriteLine("Sending : " + result);
                await responseStream.WriteAsync(new GreetEveryoneResponse() { Result = result });
            }
        }
    }
}