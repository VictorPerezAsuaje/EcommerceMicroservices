﻿using System.ComponentModel.DataAnnotations;

namespace WebClient.Services.Catalog.Products;

public class ProductPutDTO
{
    [Required(ErrorMessage = "The username is required.")]
    [MaxLength(500)]
    public string Name { get; set; }

    [Required(ErrorMessage = "The username is required.")]
    [Range(0.0, double.MaxValue, ErrorMessage = "The price must be greater than {1}.")]
    public double Price { get; set; }
    public string? Category { get; set; } = null;
    public List<string>? Tags { get; set; } = null;

    [MaxLength(2000, ErrorMessage = "The description can only have a maximum of 2000 characters.")]
    public string? Description { get; set; } = null;

    [MaxLength(2000, ErrorMessage = "The description can only have a maximum of 2000 characters.")]
    public string? Details { get; set; } = null;
}
