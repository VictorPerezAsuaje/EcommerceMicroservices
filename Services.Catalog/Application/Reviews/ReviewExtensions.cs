using Services.Catalog.Domain;

namespace Services.Catalog.Application.Reviews;

public static class ReviewExtensions
{
    public static List<ReviewGetDTO> ToListGetDTO(this List<Review> reviews)
        => reviews.Select(ToGetDTO).ToList();
    public static ReviewGetDTO ToGetDTO(this Review review)
        => new ReviewGetDTO()
        {
            Id = review.Id,
            Username = review.Username,
            Email = review.Email,
            Rating = review.Rating,
            Title = review.Title,
            Body = review.Body
        };
}