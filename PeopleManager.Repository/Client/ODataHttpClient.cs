namespace PeopleManager.Repository.Client;

public class ODataHttpClient : IODataHttpClient
{
    private readonly HttpClient _httpClient;

    public ODataHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<HttpResponseMessage> MakeRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _httpClient.SendAsync(request, cancellationToken);
    }
}