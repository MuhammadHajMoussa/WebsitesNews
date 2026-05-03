using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        // التحقق من أن جميع المعاملات غير null
        if (context == null) throw new ArgumentNullException(nameof(context), "ApplicationDbContext is null");
        if (roleManager == null) throw new ArgumentNullException(nameof(roleManager), "RoleManager is null");
        if (userManager == null) throw new ArgumentNullException(nameof(userManager), "UserManager is null");

        // التأكد من إنشاء قاعدة البيانات إذا لم تكن موجودة
        context.Database.EnsureCreated();

        // إضافة الأدوار
        string[] roles = { "Subscriber", "Author", "Admin", "Owner" };
        foreach (var role in roles)
        {
            if (string.IsNullOrEmpty(role)) continue; // تجنب القيم الفارغة
            try
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إنشاء الدور {role}: {ex.Message}");
                throw;
            }
        }

        // إضافة مستخدم افتراضي (اختياري)
        var adminEmail = "admin@example.com";
        try
        {
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    Console.WriteLine("فشل إنشاء المستخدم الافتراضي: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"خطأ في إنشاء المستخدم الافتراضي: {ex.Message}");
            throw;
        }
    }
}