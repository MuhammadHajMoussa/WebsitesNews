using System.ComponentModel.DataAnnotations;
namespace NewsSite.Models;
public class Comment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "التعليق مطلوب")]
    public string Content { get; set; }

    public DateTime PostedDate { get; set; } = DateTime.Now;

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public int NewsId { get; set; }
    public News News { get; set; }
}