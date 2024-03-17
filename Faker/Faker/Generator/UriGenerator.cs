using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoGenerator.Generator;

public class UriGenerator : IGenerator
{
    private readonly Random random;

    private string GetRandomScheme()
    {
        string[] schemes = { "http", "https", "ftp", "file" };
        return schemes[random.Next(schemes.Length)];
    }

    private string GetRandomHost()
    {
        string[] hosts = { "example.com", "google.com", "stackoverflow.com", "github.com" };
        return hosts[random.Next(hosts.Length)];
    }

    private string GetRandomPath()
    {
        string[] paths = { "path1", "path2", "path3", "path4" };
        return paths[random.Next(paths.Length)];
    }

    private string GetRandomQuery()
    {
        string[] queries = { "key1=value1", "key2=value2", "key3=value3" };
        return queries[random.Next(queries.Length)];
    }

    public UriGenerator()
    {
        random = new Random();
    }

    public object Generate(Type t, Faker faker)
    {
        string scheme = GetRandomScheme();
        string host = GetRandomHost();
        string path = GetRandomPath();
        string query = GetRandomQuery();

        string uriString = $"{scheme}://{host}/{path}?{query}";
        return new Uri(uriString);
    }
}
