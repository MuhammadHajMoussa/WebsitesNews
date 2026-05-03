using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsSite.Models;

public class News
{
    public int Id { get; set; }

    [Required(ErrorMessage = "العنوان مطلوب")]
    [MaxLength(200, ErrorMessage = "العنوان لا يجب أن يزيد عن 200 حرف")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "المحتوى مطلوب")]
    [MaxLength(5000, ErrorMessage = "المحتوى لا يجب أن يزيد عن 5000 حرف")]
    public string? Content { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime PublishDate { get; set; } = DateTime.Now;

    public int Views { get; set; } = 0;

    public int Likes { get; set; } = 0;

    [Required]
    public string? AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    [Required]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();

    public List<Video> Videos { get; set; } = new List<Video>();
    public List<Image> Images { get; set; } = new List<Image>();
    public List<Document> Documents { get; set; } = new List<Document>();
}

public class Video
{
    public int Id { get; set; }

    [Required]
    public int NewsId { get; set; }
    public News? News { get; set; }

    [Required(ErrorMessage = "عنوان الفيديو مطلوب")]
    [MaxLength(255, ErrorMessage = "عنوان الفيديو لا يجب أن يزيد عن 255 حرف")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "اسم الملف مطلوب")]
    [MaxLength(255, ErrorMessage = "اسم الملف لا يجب أن يزيد عن 255 حرف")]
    public string? FileName { get; set; }

    [Required(ErrorMessage = "محتوى الفيديو مطلوب")]
    public byte[] FileContent { get; set; }

    [Required(ErrorMessage = "نوع الفيديو مطلوب")]
    [MaxLength(100, ErrorMessage = "نوع الفيديو لا يجب أن يزيد عن 100 حرف")]
    public string? FileType { get; set; } // مثل video/mp4، video/webm

    public long FileSize { get; set; } // حجم الملف بالبايت

    [DataType(DataType.DateTime)]
    public DateTime UploadDate { get; set; } = DateTime.Now;
}

public class Image
{
    public int Id { get; set; }

    [Required]
    public int NewsId { get; set; }
    public News? News { get; set; }

    [Required(ErrorMessage = "عنوان الصورة مطلوب")]
    [MaxLength(255, ErrorMessage = "عنوان الصورة لا يجب أن يزيد عن 255 حرف")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "اسم الملف مطلوب")]
    [MaxLength(255, ErrorMessage = "اسم الملف لا يجب أن يزيد عن 255 حرف")]
    public string? FileName { get; set; }

    [Required(ErrorMessage = "محتوى الصورة مطلوب")]
    public byte[] FileContent { get; set; }

    [Required(ErrorMessage = "نوع الصورة مطلوب")]
    [MaxLength(100, ErrorMessage = "نوع الصورة لا يجب أن يزيد عن 100 حرف")]
    public string? FileType { get; set; } // مثل image/jpeg، image/png

    public long FileSize { get; set; } // حجم الملف بالبايت

    [DataType(DataType.DateTime)]
    public DateTime UploadDate { get; set; } = DateTime.Now;
}

public class Document
{
    public int Id { get; set; }

    [Required]
    public int NewsId { get; set; }
    public News? News { get; set; }

    [Required(ErrorMessage = "عنوان المستند مطلوب")]
    [MaxLength(255, ErrorMessage = "عنوان المستند لا يجب أن يزيد عن 255 حرف")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "اسم الملف مطلوب")]
    [MaxLength(255, ErrorMessage = "اسم الملف لا يجب أن يزيد عن 255 حرف")]
    public string? FileName { get; set; }

    [Required(ErrorMessage = "محتوى المستند مطلوب")]
    public byte[] FileContent { get; set; }

    [Required(ErrorMessage = "نوع المستند مطلوب")]
    [MaxLength(100, ErrorMessage = "نوع المستند لا يجب أن يزيد عن 100 حرف")]
    public string? FileType { get; set; } // مثل application/pdf، text/plain

    public long FileSize { get; set; } // حجم الملف بالبايت

    [DataType(DataType.DateTime)]
    public DateTime UploadDate { get; set; } = DateTime.Now;
}
//git remote add origin https://github.com
//git branch -M main
//git push - u origin main