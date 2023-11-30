using Services.Catalog.Application.Tags;

namespace Services.Catalog.Application.Reviews;

public class ReviewGetDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public double Rating { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}
