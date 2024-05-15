using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoDefinitions;

namespace ApiApplication
{
    public class ApiClientGrpc
    {
        public async Task<showListResponse> GetAll()
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("X-Apikey", "68e5fbda-9ec9-4858-97b2-4a8349764c63");
                return Task.CompletedTask;
            });

            var channel =
                GrpcChannel.ForAddress("https://localhost:7443", new GrpcChannelOptions()
                {
                    HttpHandler = httpHandler,
                    Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
                });
            var client = new MoviesApi.MoviesApiClient(channel);

            var all = await client.GetAllAsync(new Empty());
            all.Data.TryUnpack<showListResponse>(out var data);
            return data;
        }
    }
}