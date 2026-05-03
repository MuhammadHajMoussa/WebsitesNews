# 🐛 دليل حل المشاكل الشاملة

## 🚨 المشاكل المتعلقة بالبيانات

### ❌ مشكلة 1: "The type or namespace name 'X' could not be found"

**السبب**: الفئة أو المساحة الاسمية غير معرفة

**الحل**:
```csharp
// ❌ خطأ
public string Biography { get; set; }

// ✅ صحيح - أضف using في الأعلى
using NewsSite.Models;
using Microsoft.AspNetCore.Identity;
```

**الملف المتعلق**: جميع ملفات C#

---

### ❌ مشكلة 2: البيانات لا تُحفظ

**السبب**: نسيان استدعاء `UpdateAsync()` أو `SetEmailAsync()` وغيرها

**الحل**:
```csharp
// ❌ خطأ - البيانات لن تُحفظ
user.FullName = FullName;
// لم نستدعي UpdateAsync!

// ✅ صحيح
user.FullName = FullName;
await _userManager.UpdateAsync(user);
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

### ❌ مشكلة 3: البيانات القديمة تظهر بدل الجديدة

**السبب**: لم نعدّل `OnGetAsync()` لجلب البيانات الجديدة

**الحل**:
```csharp
// ❌ خطأ
public async Task<IActionResult> OnGetAsync()
{
    // لم نجلب Biography!
    return Page();
}

// ✅ صحيح
public async Task<IActionResult> OnGetAsync()
{
    var user = await _userManager.GetUserAsync(User);
    Biography = user.Biography; // ← إضافة هذا
    return Page();
}
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

### ❌ مشكلة 4: "The 'X' table does not exist in the database"

**السبب**: نسيان تطبيق Migration على قاعدة البيانات

**الحل**:
```powershell
# ❌ خطأ - أنشأنا Migration لكن لم نطبقه
Add-Migration AddBiography

# ✅ صحيح - طبق Migration
Add-Migration AddBiography
Update-Database
```

**الملف المتعلق**: Package Manager Console

---

### ❌ مشكلة 5: Model Binding لم يعمل

**السبب**: نسيان `[BindProperty]` على الخاصية

**الحل**:
```csharp
// ❌ خطأ - لن يتم ملء هذا الحقل من النموذج
public string Biography { get; set; }

// ✅ صحيح
[BindProperty]
public string Biography { get; set; }
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

## 🚨 المشاكل المتعلقة بالتحقق

### ❌ مشكلة 6: رسالة خطأ التحقق لا تظهر

**السبب**: نسيان `asp-validation-for` على الصفحة

**الحل**:
```html
<!-- ❌ خطأ -->
<input asp-for="FullName" />
<!-- رسالة الخطأ لن تظهر -->

<!-- ✅ صحيح -->
<input asp-for="FullName" />
<span asp-validation-for="FullName" class="text-danger"></span>
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml`

---

### ❌ مشكلة 7: البيانات غير الصحيحة تمر

**السبب**: عدم التحقق من ModelState

**الحل**:
```csharp
// ❌ خطأ - ستقبل بيانات خاطئة
public async Task<IActionResult> OnPostAsync()
{
    var user = await _userManager.GetUserAsync(User);
    user.FullName = FullName;
    await _userManager.UpdateAsync(user);
}

// ✅ صحيح
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
        return Page(); // إعادة الصفحة مع الأخطاء
    
    var user = await _userManager.GetUserAsync(User);
    user.FullName = FullName;
    await _userManager.UpdateAsync(user);
}
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

## 🚨 مشاكل رفع الملفات

### ❌ مشكلة 8: HTTP 400 عند رفع صورة

**السبب**: حجم الملف يتجاوز الحد الأقصى المسموح به

**الحل**: تحقق من `Program.cs`

```csharp
// تأكد من وجود هذا في Program.cs
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
});
```

**الملف المتعلق**: `Program.cs`

---

### ❌ مشكلة 9: الصورة لا تُحفظ

**السبب**: المجلد غير موجود أو صلاحيات غير كافية

**الحل**:
```csharp
// ✅ صحيح - تحقق من وجود المجلد وأنشئه
var dir = GetAvatarDirectory();
Directory.CreateDirectory(dir); // ← أنشئ المجلد إن لم يكن موجوداً

var filePath = Path.Combine(dir, fileName);
using (var stream = new FileStream(filePath, FileMode.Create))
{
    await AvatarImage.CopyToAsync(stream);
}
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

### ❌ مشكلة 10: نوع الملف غير صحيح

**السبب**: محاولة رفع ملف غير مسموح به

**الحل**:
```csharp
// ✅ صحيح - تحقق من نوع الملف
const long maxFileSize = 5242880; // 5 MB
if (AvatarImage.Length > maxFileSize)
{
    ModelState.AddModelError(string.Empty, "حجم الصورة كبير جداً");
    return Page();
}

var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
var ext = Path.GetExtension(AvatarImage.FileName).ToLowerInvariant();
if (!allowed.Contains(ext))
{
    ModelState.AddModelError(string.Empty, "نوع الملف غير مسموح");
    return Page();
}
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

## 🚨 مشاكل الأدوار والصلاحيات

### ❌ مشكلة 11: الدور لا يتم إضافة

**السبب**: الدور غير موجود في الجدول

**الحل**:
```csharp
// ✅ صحيح - تحقق من وجود الدور أولاً
if (!await _roleManager.RoleExistsAsync("Admin"))
{
    await _roleManager.CreateAsync(new IdentityRole("Admin"));
}

// ثم أضفه للمستخدم
await _userManager.AddToRoleAsync(user, "Admin");
```

**الملف المتعلق**: `Services/UserService.cs`

---

### ❌ مشكلة 12: المستخدم لا يملك الدور بعد الإضافة

**السبب**: لم تُحفظ التغييرات

**الحل**:
```csharp
// ✅ صحيح - تحقق من نتيجة العملية
var result = await _userManager.AddToRoleAsync(user, "Admin");
if (!result.Succeeded)
{
    foreach (var error in result.Errors)
        Console.WriteLine(error.Description);
}
```

**الملف المتعلق**: `Services/UserService.cs`

---

## 🚨 مشاكل العلاقات والفهارس

### ❌ مشكلة 13: "Concurrency check failed"

**السبب**: تعارض في تحديث نفس السجل

**الحل**:
```csharp
// ✅ صحيح - أعد تحميل البيانات
var user = await _userManager.FindByIdAsync(userId);
user.FullName = "محمد";
await _userManager.UpdateAsync(user);

// أو استخدم AsNoTracking() للقراءة فقط
var readOnlyUser = dbContext.Users.AsNoTracking()
    .FirstOrDefault(u => u.Id == userId);
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

### ❌ مشكلة 14: "Foreign key constraint failed"

**السبب**: حاولت حذف سجل له علاقات أجنبية

**الحل**: تحقق من `OnModelCreating()` في `ApplicationDbContext.cs`

```csharp
// ✅ صحيح - قم بحذف البيانات المرتبطة أولاً
modelBuilder.Entity<Comment>()
    .HasOne(c => c.News)
    .WithMany(n => n.Comments)
    .HasForeignKey(c => c.NewsId)
    .OnDelete(DeleteBehavior.Cascade); // أو Restrict
```

**الملف المتعلق**: `Data/ApplicationDbContext.cs`

---

## 🚨 مشاكل الصفحات والتوجيه

### ❌ مشكلة 15: "The layout file '_Layout.cshtml' could not be found"

**السبب**: خطأ في مسار الـ Layout

**الحل**: تحقق من `_ViewStart.cshtml`

```html
<!-- ✅ صحيح -->
@{
    Layout = "_Layout"; // في نفس المجلد
    // أو
    Layout = "/Views/Shared/_Layout.cshtml"; // مسار مطلق
}
```

**الملف المتعلق**: `Pages/_ViewStart.cshtml`

---

### ❌ مشكلة 16: "The type or namespace name 'IndexModel' could not be found"

**السبب**: اسم الفئة في الصفحة لا يطابق الكود

**الحل**:
```html
<!-- ✅ صحيح -->
@model NewsSite.Pages.Profile.IndexModel
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml`

---

## 🚨 مشاكل الأمان

### ❌ مشكلة 17: "The antiforgery token could not be decrypted"

**السبب**: توكن CSRF فقد صلاحيته

**الحل**: تأكد من وجود `@Html.AntiForgeryToken()` في النموذج

```html
<!-- ✅ صحيح -->
<form method="post">
    @Html.AntiForgeryToken()
    <!-- الحقول -->
</form>
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml`

---

### ❌ مشكلة 18: كلمة المرور المشفرة معروضة

**السبب**: عرض PasswordHash على الواجهة

**الحل**: لا تعرض كلمة المرور المشفرة أبداً

```csharp
// ❌ خطأ
<p>@Model.User.PasswordHash</p>

// ✅ صحيح - لا تعرضها
// استخدم فقط: ChangePasswordAsync()
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml`

---

## 🚨 مشاكل الأداء

### ❌ مشكلة 19: الصفحة تستغرق وقتاً طويلاً

**السبب**: استعلام قاعدة بيانات غير محسّن

**الحل**: استخدم `AsNoTracking()` للقراءة فقط

```csharp
// ❌ خطأ - تتبع جميع التغييرات
var users = dbContext.Users.ToList();

// ✅ صحيح - قراءة فقط
var users = dbContext.Users.AsNoTracking().ToList();
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs` أو `Services/`

---

### ❌ مشكلة 20: "The operation completed, but no output was generated"

**السبب**: استعلام لا ينتج أي نتائج

**الحل**: تحقق من وجود البيانات

```csharp
// ✅ صحيح
var user = await _userManager.FindByIdAsync(userId);
if (user == null)
{
    return NotFound(); // أو redirect
}
```

**الملف المتعلق**: `Pages/Profile/Index.cshtml.cs`

---

## 📋 قائمة تفقد قبل الحفظ

قبل حفظ أي تعديل، تحقق من:

- ✅ هل أضفت `[BindProperty]` على الحقل الجديد؟
- ✅ هل جلبت البيانات في `OnGetAsync()`؟
- ✅ هل حدثت البيانات في `OnPostAsync()`؟
- ✅ هل استدعيت `UpdateAsync()` أو الدالة المناسبة؟
- ✅ هل أضفت الحقل على الصفحة HTML؟
- ✅ هل أضفت `asp-validation-for` لرسالة الخطأ؟
- ✅ هل تحققت من `ModelState.IsValid`؟
- ✅ هل أنشأت Migration إذا أضفت جدول أو عمود جديد؟
- ✅ هل طبقت Migration باستخدام `Update-Database`؟

---

## 🆘 كيف تحصل على المساعدة

إذا واجهتك مشكلة:

1. **اقرأ رسالة الخطأ بعناية** - قد تحتوي على الحل
2. **ابحث في هذا الملف** عن الخطأ المشابه
3. **تحقق من الملفات المقترحة** في كل مشكلة
4. **جرب الحل المقترح** خطوة بخطوة
5. **أعد بناء المشروع**: Clean + Rebuild
6. **امسح Bin و Obj**: قد تحتوي على ملفات قديمة

---

## 📞 معلومات الدعم الإضافية

لمزيد من التفاصيل، راجع:
- `DOCUMENTATION_AR.md` - شرح شامل للجداول والعلاقات
- `DATA_FLOW_MAP_AR.md` - خريطة مسار البيانات
- `CHECKLIST_ADD_NEW_FIELD_AR.md` - خطوات إضافة حقل جديد
- `QUICK_REFERENCE_AR.md` - مرجع سريع

