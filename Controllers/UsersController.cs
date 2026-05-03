using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsSite.Models.ViewModels;
using NewsSite.Services;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin,Owner")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync() ?? new List<ApplicationUser>();
        var model = users.Select(u => new UserManagementViewModel
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            Roles = _userService.GetUserRolesAsync(u.Id).Result // تأكد من وجود هذه الدالة في IUserService
        }).ToList();
        return View(model);
    }

    [HttpGet]
    public IActionResult Promote(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("معرف المستخدم غير صالح.");
        }

        var model = new PromoteViewModel
        {
            UserId = userId,
            Roles = new[] { "Subscriber", "Author", "Admin", "Owner" }
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Promote(string userId, string newRole)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
        {
            ModelState.AddModelError("", "معرف المستخدم أو الدور غير صالح.");
            ViewBag.Roles = new[] { "Subscriber", "Author", "Admin", "Owner" };
            ViewBag.UserId = userId;
            return View();
        }

        try
        {
            // Get current user roles and the target user's roles
            var currentIsOwner = User.IsInRole("Owner");
            var currentIsAdmin = User.IsInRole("Admin");

            var targetRoles = await _userService.GetUserRolesAsync(userId);
            var targetIsSubscriber = targetRoles.Contains("Subscriber");
            var targetIsAuthor = targetRoles.Contains("Author");
            var targetIsAdmin = targetRoles.Contains("Admin");
            var targetIsOwner = targetRoles.Contains("Owner");

            // Rules:
            // - Subscriber -> Author: allowed for Admin or Owner
            // - Author -> Admin: allowed only for Owner
            // - Any -> Owner: allowed only for Owner
            if (newRole == "Author")
            {
                if (targetIsAuthor)
                {
                    // already an author; nothing to do
                }
                else if (targetIsSubscriber)
                {
                    if (!(currentIsAdmin || currentIsOwner))
                    {
                        throw new UnauthorizedAccessException("لا يملك حسابك صلاحية ترقية مشترك إلى مؤلف.");
                    }
                }
            }
            else if (newRole == "Admin")
            {
                // Only Owner can promote to Admin
                if (!currentIsOwner)
                {
                    throw new UnauthorizedAccessException("لا يملك حسابك صلاحية ترقية إلى مشرف (Admin).");
                }
            }
            else if (newRole == "Owner")
            {
                // Only Owner can promote to Owner
                if (!currentIsOwner)
                {
                    throw new UnauthorizedAccessException("لا يملك حسابك صلاحية ترقية إلى مالك النظام (Owner).");
                }
            }

            await _userService.PromoteUserAsync(userId, newRole);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"فشل في ترقية المستخدم: {ex.Message}");
            ViewBag.Roles = new[] { "Subscriber", "Author", "Admin", "Owner" };
            ViewBag.UserId = userId;
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("معرف المستخدم غير صالح.");
        }

        try
        {
            await _userService.DeleteUserAsync(userId);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"فشل في حذف المستخدم: {ex.Message}");
            var users = await _userService.GetAllUsersAsync() ?? new List<ApplicationUser>();
            var model = users.Select(u => new UserManagementViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Roles = _userService.GetUserRolesAsync(u.Id).Result // تأكد من وجود هذه الدالة
            }).ToList();
            return View("Index", model);
        }
    }
    
}