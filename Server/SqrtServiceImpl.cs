using Grpc.Core;
using Sqrt;
using static Sqrt.SqrtService;

namespace server
{
    public class SqrtServiceImpl : SqrtServiceBase
    {
        public override async Task<SqrtResponse> sqrt(SqrtRequest request, ServerCallContext context)
        {
            int number = request.Number;

            if (number > 0)
                return new SqrtResponse() { SquareRoot = number };
            else
                throw new RpcException(new Status(StatusCode.InvalidArgument, "number < 0"));
        }
    }
}