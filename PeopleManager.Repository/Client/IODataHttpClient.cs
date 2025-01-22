namespace PeopleManager.Repository.Client;

public interface IODataHttpClient
{
    Task<HttpResponseMessage> MakeRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}