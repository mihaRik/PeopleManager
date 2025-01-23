using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PeopleManager.Domain.Entities;
using PeopleManager.Repository.Client;
using UriBuilder = PeopleManager.Repository.Utils.UriBuilder;

namespace PeopleManager.Repository;

public class PeopleRepository : IPeopleRepository
{
    private const string PeopleSegmentUrl = "people";
    private readonly string[] _properties = ["UserName", "FirstName", "LastName"];
    private readonly IODataHttpClient _httpClient;

    public PeopleRepository(IODataHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Person>> GetPeopleAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var uri = new UriBuilder(PeopleSegmentUrl)
            .WithSelect(_properties)
            .WithPagination(page, pageSize)
            .Build();
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        
        var response = await _httpClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);

        var wrapper = await response.Content.ReadFromJsonAsync<ODataWrapper<Person>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        return wrapper?.Value ?? Array.Empty<Person>();
    }

    public async Task<int> GetPeopleCountAsync(string searchQuery = null!, CancellationToken cancellationToken = default)
    {
        var uriBuilder = new UriBuilder(PeopleSegmentUrl);
        if (searchQuery != null)
        {
            var uri = uriBuilder
                .WithFilterByProperties(searchQuery, _properties)
                .WithSelect("UserName")
                .Build();
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            
            var response = await _httpClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);
            
            var wrapper = await response.Content.ReadFromJsonAsync<ODataWrapper<Person>>(cancellationToken: cancellationToken).ConfigureAwait(false);

            return wrapper?.Value?.Count() ?? 0;
        }
        else
        {
            var uri = uriBuilder.WithCount().Build();
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            
            var response = await _httpClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            return int.TryParse(content, out var count) ? count : 0;
        }
    }

    public async Task<Person> GetPersonByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var uri = new UriBuilder(PeopleSegmentUrl).WithFilterById(username).Build();
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        
        var response = await _httpClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);
        
        return await response.Content.ReadFromJsonAsync<Person>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Person>> SearchPeopleAsync(string searchQuery,int page, int pageSize, CancellationToken cancellationToken)
    {
        var uri = new UriBuilder(PeopleSegmentUrl)
            .WithSelect(_properties)
            .WithPagination(page, pageSize)
            .WithFilterByProperties(searchQuery, _properties)
            .Build();
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        
        var response = await _httpClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);

        var wrapper = await response.Content.ReadFromJsonAsync<ODataWrapper<Person>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        return wrapper?.Value ?? Array.Empty<Person>();
    }

    public async Task<Person> UpdatePersonAsync(string username, Person person, CancellationToken cancellationToken)
    {
        var uri = new UriBuilder(PeopleSegmentUrl)
            .WithFilterById(username)
            .Build();

        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(person), MediaTypeHeaderValue.Parse("application/json")),
        };
        var a = JsonContent.Create(person);
        var f = await a.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var response = await _httpClient.MakeRequestAsync(request, cancellationToken).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<Person>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}