using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models.ViewModels
{
    public class ProfileViewModel
    {
        // Readonly identifier
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; }

        // Password change fields
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور غير متطابقة")]
        public string? ConfirmPassword { get; set; }

        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public string[] Roles { get; set; } = new string[0];

        public IFormFile? AvatarImage { get; set; }

        public string? AvatarUrl { get; set; }
    }
}