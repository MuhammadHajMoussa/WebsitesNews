using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewsSite.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray(),
                AvatarUrl = user.ProfilePicture != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(user.ProfilePicture)}" : "/images/default-avatar.png"
            };

            return View("Profile", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Repopulate roles and avatar for the view in case of error
            model.Roles = (await _userManager.GetRolesAsync(user)).ToArray();
            model.AvatarUrl = user.ProfilePicture != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(user.ProfilePicture)}" : "/images/default-avatar.png";
            model.Id = user.Id;

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            // Update username
            if (user.UserName != model.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var e in setUserNameResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return View("Profile", model);
                }
            }

            // Update email
            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var e in setEmailResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return View("Profile", model);
                }
            }

            // Update basic fields
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            // Handle avatar upload
            if (model.AvatarImage != null && model.AvatarImage.Length > 0)
            {
                try
                {
                    const long maxFileSize = 5242880; // 5 MB
                    if (model.AvatarImage.Length > maxFileSize)
                    {
                        ModelState.AddModelError(string.Empty, "حجم الصورة كبير جداً. الحد الأقصى 5 MB.");
                        return View("Profile", model);
                    }

                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var ext = Path.GetExtension(model.AvatarImage.FileName).ToLowerInvariant();
                    if (!allowed.Contains(ext))
                    {
                        ModelState.AddModelError(string.Empty, "الملف غير مسموح. استخدم jpg, png, gif.");
                        return View("Profile", model);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await model.AvatarImage.CopyToAsync(memoryStream);
                        user.ProfilePicture = memoryStream.ToArray();
                    }

                    model.AvatarUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(user.ProfilePicture)}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"خطأ في رفع الصورة: {ex.Message}");
                    return View("Profile", model);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return View("Profile", model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError(string.Empty, "لتغيير كلمة المرور، أدخل كلمة المرور الحالية.");
                    return View("Profile", model);
                }

                var changePassResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePassResult.Succeeded)
                {
                    foreach (var e in changePassResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return View("Profile", model);
                }
            }

            TempData["StatusMessage"] = "تم تحديث الملف الشخصي بنجاح.";
            return RedirectToAction(nameof(Index));
        }
    }
}