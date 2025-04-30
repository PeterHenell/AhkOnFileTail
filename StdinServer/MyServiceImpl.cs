using Grpc.Core;
using System.Threading.Tasks;

namespace StdinServer
{
    public class MyServiceImpl : MyService.MyServiceBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello, {request.Name}"
            });
        }
    }
}
