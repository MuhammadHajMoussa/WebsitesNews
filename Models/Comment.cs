﻿using System.ComponentModel.DataAnnotations;
namespace NewsSite.Models;
public class Comment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "التعليق مطلوب")]
    public string Content { get; set; } = string.Empty;

    public DateTime PostedDate { get; set; } = DateTime.Now;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public int NewsId { get; set; }
    public News News { get; set; } = default!;
}