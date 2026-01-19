namespace IssueRunner.Models;

/// <summary>
/// Repository configuration model.
/// </summary>
public record RepositoryConfig 
{
    public RepositoryConfig()
    {
        
    }

    public RepositoryConfig(string owner, string name)
    {
        Owner = owner;
        Name = name;
    }

    [JsonPropertyName("owner")] public string? Owner { get; set; } = "";

    [JsonPropertyName("name")] public string? Name { get; set; } = "";

    public override string ToString()
    {
        return $"{Owner}/{Name}";
    }
}





