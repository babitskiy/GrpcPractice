using Calculation;
using Grpc.Core;
using static Calculation.CalculationService;

namespace server
{
    public class CalculationServiceImpl : CalculationServiceBase
    {
        public override Task<CalculationResponse> Add(CalculationRequest request, ServerCallContext context)
            => Task.FromResult(new CalculationResponse() 
            { Result = request.FirstParam + request.SecondParam });

        public override Task<CalculationResponse> Multiply(CalculationRequest request, ServerCallContext context)
            => Task.FromResult(new CalculationResponse()
            { Result = request.FirstParam * request.SecondParam });

        public override Task<CalculationResponse> Divide(CalculationRequest request, ServerCallContext context)
            => Task.FromResult(new CalculationResponse()
            { Result = request.FirstParam / request.SecondParam});

        public override async Task PrimeNumberDecomposition(PrimeNumberDecompositionRequest request, IServerStreamWriter<CalculationResponse> responseStream, ServerCallContext context)
        {
            double divider = 2;
            double primeNumber = request.PrimeNumber;

            while (primeNumber > 1)
            {
                if (primeNumber % divider == 0)
                {
                    await responseStream.WriteAsync(new CalculationResponse() { Result = divider });
                    primeNumber /= divider;
                }
                else
                    divider++;

                await Task.Delay(500);
            }
        }

        public override async Task<ComputeAverageResponse> ComputeAverage(IAsyncStreamReader<ComputeAverageRequest> requestStream, ServerCallContext context)
        {
            int count = 0;
            double sum = 0;

            while (await requestStream.MoveNext())
            {
                sum += requestStream.Current.Number;
                count++;
            }

            return new ComputeAverageResponse() { AverageNumber = sum / count };
        }
    }
}