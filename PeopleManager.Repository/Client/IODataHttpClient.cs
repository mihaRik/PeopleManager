namespace PeopleManager.Repository.Client;

public interface IODataHttpClient : IDisposable
{
    Task<HttpResponseMessage> MakeRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}