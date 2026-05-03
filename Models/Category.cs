using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsSite.Models;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Slug is a URL-friendly identifier (lowercase, hyphens)
        [Required]
        [MaxLength(120)]
        public string Slug { get; set; }

        public List<News> News { get; set; } = new List<News>();
    }
}
