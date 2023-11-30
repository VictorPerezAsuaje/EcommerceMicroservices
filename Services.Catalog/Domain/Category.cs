using System.ComponentModel.DataAnnotations;

namespace Services.Catalog.Domain;

public class Category
{
    public const string Default = "Uncategorized";

    [Key]
    public string Name { get; private set; }

    protected Category() { /* EF Required */ }

    public Category(string name)
    {
        Name = name;
    }
}
