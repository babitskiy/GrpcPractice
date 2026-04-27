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
    }
}