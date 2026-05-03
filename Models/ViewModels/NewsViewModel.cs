
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewsSite.Models.ViewModels;

public class NewsViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "العنوان مطلوب")]
    [MaxLength(200, ErrorMessage = "العنوان لا يجب أن يزيد عن 200 حرف")]
    public string Title { get; set; }

    [Required(ErrorMessage = "المحتوى مطلوب")]
    [MaxLength(5000, ErrorMessage = "المحتوى لا يجب أن يزيد عن 5000 حرف")]
    public string Content { get; set; }

    [Required(ErrorMessage = "التصنيف مطلوب")]
    public int CategoryId { get; set; }

    public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    public List<string> FileTitles { get; set; } = new List<string>();
    public List<string> FileTypes { get; set; } = new List<string>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // التحقق من الملفات إذا تم رفعها
        if (Files != null && Files.Any(f => f != null && f.Length > 0))
        {
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i] != null && Files[i].Length > 0)
                {
                    // التحقق من وجود عنوان
                    if (string.IsNullOrWhiteSpace(FileTitles[i]))
                    {
                        yield return new ValidationResult(
                            "عنوان الملف مطلوب عند رفع ملف.",
                            new[] { $"FileTitles[{i}]" });
                    }

                    // التحقق من نوع الملف
                    if (string.IsNullOrWhiteSpace(FileTypes[i]))
                    {
                        yield return new ValidationResult(
                            "يجب اختيار نوع الملف (فيديو، صورة، أو مستند).",
                            new[] { $"FileTypes[{i}]" });
                    }

                    // التحقق من صيغة الملف
                    string contentType = Files[i].ContentType.ToLower();
                    switch (FileTypes[i].ToLower())
                    {
                        case "video":
                            if (!contentType.Equals("video/mp4") &&
                                !contentType.Equals("video/webm") &&
                                !contentType.Equals("video/ogg"))
                            {
                                yield return new ValidationResult(
                                    "تنسيق الفيديو غير مدعوم. التنسيقات المدعومة هي: MP4, WebM, Ogg",
                                    new[] { $"Files[{i}]" });
                            }
                            break;
                        case "image":
                            if (!contentType.StartsWith("image/"))
                            {
                                yield return new ValidationResult(
                                    "تنسيق الصورة غير مدعوم. التنسيقات المدعومة هي: JPEG, PNG, GIF, إلخ.",
                                    new[] { $"Files[{i}]" });
                            }
                            break;
                        case "document":
                            if (!contentType.Equals("application/pdf") &&
                                !contentType.Equals("text/plain"))
                            {
                                yield return new ValidationResult(
                                    "تنسيق المستند غير مدعوم. التنسيقات المدعومة هي: PDF, Text",
                                    new[] { $"Files[{i}]" });
                            }
                            break;
                        default:
                            yield return new ValidationResult(
                                "نوع الملف غير صالح.",
                                new[] { $"FileTypes[{i}]" });
                            break;
                    }
                }
            }
        }
    }
}