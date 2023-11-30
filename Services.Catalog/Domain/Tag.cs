using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Domain;

public class Tag
{
    public const string Default = "Untagged";

    [Key]
    public string Name { get; private set; }

    protected Tag() { /* EF Required */ }

    public Tag(string name)
    {
        Name = name;
    }
}
