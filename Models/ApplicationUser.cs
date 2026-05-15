using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }

    /// <summary>
    /// الصورة الشخصية مخزنة كبيانات ثنائية (Byte Array) في قاعدة البيانات
    /// </summary>
    public byte[]? ProfilePicture { get; set; }
}
