namespace PeopleManager.Repository.Client;

public class ODataHttpClient : IODataHttpClient, IDisposable
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _httpClient?.Dispose();
        }
    }

    ~ODataHttpClient()
    {
        Dispose(false);
    }
}