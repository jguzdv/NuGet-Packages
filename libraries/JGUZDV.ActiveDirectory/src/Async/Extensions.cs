using System.DirectoryServices.Protocols;

namespace JGUZDV.ActiveDirectory.Async
{
    public static class LdapConnectionExtensions
    {
        extension(LdapConnection connection)
        {
            public async Task<T?> SendRequestAsync<T>(DirectoryRequest request, CancellationToken ct)
                where T : DirectoryResponse
            {
                var asyncResult = connection.BeginSendRequest(request, PartialResultProcessing.NoPartialResultSupport, null, null);
                using var ctr = ct.Register(() => connection.Abort(asyncResult));

                return await Task.Factory.FromAsync(
                    asyncResult,
                    connection.EndSendRequest
                ) as T;
            }
        }
    }
}
