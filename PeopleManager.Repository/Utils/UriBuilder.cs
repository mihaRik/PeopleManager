using System.Text;

namespace PeopleManager.Repository.Utils;

public class UriBuilder
{
    private StringBuilder _baseUri = new();
    private bool questionMarkUsed; 

    public UriBuilder(string segment)
    {
        _baseUri.Append(segment);
    }

    public UriBuilder WithPagination(int page, int pageSize)
    {
        CheckQuestionMark();
        _baseUri.Append($"&$top={pageSize}&$skip={(page - 1) * pageSize}");

        return this;
    }

    public UriBuilder WithSelect(params string[] select)
    {
        CheckQuestionMark();
        _baseUri.Append($"&$select={string.Join(',', select)}");

        return this;
    }
    
    public UriBuilder WithCount()
    {
        _baseUri.Append("/$count");

        return this;
    }
    
    public UriBuilder WithFilterById(string id)
    {
        _baseUri.Append($"('{id}')");

        return this;
    }

    public UriBuilder WithFilterByProperties(string filter, params string[] properties)
    {
        CheckQuestionMark();
        _baseUri.Append($"&$filter={string.Join(" or ", properties.Select(p => $"contains({p}, '{filter}')"))}");

        return this;
    }

    public string Build()
    {
        return _baseUri.ToString();
    }

    private void CheckQuestionMark()
    {
        if (questionMarkUsed) return;
        _baseUri.Append('?');
        questionMarkUsed = true;
    }
}