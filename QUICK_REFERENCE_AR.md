# 🚀 مرجع سريع - خريطة الملفات والعمليات

## 📁 هيكل المشروع

```
NewsSite/
│
├── 📁 Areas/
│   └── 📁 Identity/
│       ├── 📁 Data/
│       ├── 📁 Pages/
│       │   ├── 📁 Account/
│       │   └── _ViewStart.cshtml ← تحديد Layout للصفحات
│       └── ...
│
├── 📁 Models/
│   ├── ApplicationUser.cs ← ✅ تعديل البيانات هنا
│   ├── News.cs
│   ├── Category.cs
│   ├── Comment.cs
│   └── ...
│
├── 📁 Data/
│   └── ApplicationDbContext.cs ← ✅ تعديل العلاقات هنا
│
├── 📁 Controllers/
│   └── ProfileController.cs ← ✅ معالجة البيانات (GET/POST)
│
├── 📁 Models/
│   └── 📁 ViewModels/
│       └── ProfileViewModel.cs ← ✅ نموذج بيانات الواجهة
│
├── 📁 Services/
│   └── UserService.cs ← ✅ خدمات المستخدم والأدوار
│
├── 📁 Views/
│   ├── 📁 Profile/
│   │   └── Index.cshtml ← ✅ واجهة المستخدم
│   ├── 📁 Shared/
│   │   └── _Layout.cshtml ← ✅ الهيكل الأساسي للموقع
│   └── ...
│
├── 📁 wwwroot/
│   ├── 📁 css/
│   ├── 📁 js/
│   ├── 📁 images/
│   └── ...
│
├── Program.cs ← ✅ إعدادات الخادم والملفات
├── appsettings.json ← إعدادات الاتصال بقاعدة البيانات
└── 📁 Migrations/ ← ✅ سجل تغييرات قاعدة البيانات
```

---

## 🔄 دورة حياة العملية (Request/Response)

### 1️⃣ طلب الصفحة الأولى (GET)
```
المتصفح → GET /Profile
   ↓
Routing → Pages/Profile/Index.cshtml
   ↓
PageModel → OnGetAsync()
   ↓
جلب البيانات من قاعدة البيانات:
   SELECT * FROM AspNetUsers WHERE Id = @UserId
   ↓
ملء النموذج بالبيانات
   ↓
عرض الصفحة HTML
   ↓
المتصفح يعرض الصفحة
```

### 2️⃣ إرسال تعديلات (POST)
```
المتصفح يرسل ← POST /Profile مع البيانات المعدلة
   ↓
Binding → ملء خصائص النموذج من البيانات المرسلة
   ↓
Validation → التحقق من صحة البيانات
   ↓
PageModel → OnPostAsync()
   ↓
تحديث قاعدة البيانات:
   UPDATE AspNetUsers SET ... WHERE Id = @UserId
   ↓
TempData → رسالة النجاح
   ↓
Redirect → إعادة التوجيه إلى نفس الصفحة
   ↓
OnGetAsync() يُستدعى مجدداً
   ↓
المتصفح يعرض الصفحة المحدثة
```

---

## 🎯 جدول التعديلات حسب المكان

| ما تريد فعله | الملف | الدالة/المكان | النوع |
|-----------|------|-------------|------|
| إضافة حقل بيانات | Models/ApplicationUser.cs | - | خاصية (Property) |
| إضافة تحقق من الصحة | Models/ApplicationUser.cs | [Required], [StringLength], الخ | Attribute |
| إضافة علاقة جديدة | Data/ApplicationDbContext.cs | OnModelCreating | Entity Relationship |
| إنشاء جدول جديد في قاعدة البيانات | - | Add-Migration, Update-Database | PowerShell |
| عرض الحقل على الصفحة | Pages/Profile/Index.cshtml | - | HTML + Razor |
| جلب البيانات من قاعدة البيانات | Pages/Profile/Index.cshtml.cs | OnGetAsync() | C# |
| حفظ البيانات في قاعدة البيانات | Pages/Profile/Index.cshtml.cs | OnPostAsync() | C# |
| إدارة الأدوار | Services/UserService.cs | PromoteUserAsync | C# |
| تغيير كلمة المرور | Pages/Profile/Index.cshtml.cs | OnPostAsync() | C# |
| رفع ملف | Pages/Profile/Index.cshtml | Input type=file | HTML |
| معالجة رفع ملف | Controllers/ProfileController.cs | Index() (POST) | C# (MemoryStream) |
| تصميم الواجهة | Views/Shared/_Layout.cshtml | - | HTML + Bootstrap |
| شهادات الأمان | Program.cs | middleware headers | C# |
| حدود حجم الملفات | Program.cs | ConfigureKestrel, FormOptions | C# |

---

## 💾 استعلامات SQL المهمة

### 📌 جلب المستخدم الحالي
```sql
SELECT TOP(1) * FROM [AspNetUsers]
WHERE [NormalizedUserName] = UPPER(@UserName)
```

### 📌 جلب أدوار المستخدم
```sql
SELECT ar.[Name]
FROM [AspNetUserRoles] ur
INNER JOIN [AspNetRoles] ar ON ur.[RoleId] = ar.[Id]
WHERE ur.[UserId] = @UserId
```

### 📌 تحديث بيانات المستخدم
```sql
UPDATE [AspNetUsers]
SET [FullName] = @FullName,
    [PhoneNumber] = @PhoneNumber,
    [Email] = @Email,
    [ConcurrencyStamp] = @ConcurrencyStamp
WHERE [Id] = @UserId
```

### 📌 إضافة دور للمستخدم
```sql
INSERT INTO [AspNetUserRoles] (UserId, RoleId)
SELECT @UserId, Id FROM [AspNetRoles] WHERE [Name] = @RoleName
```

### 📌 إزالة دور من المستخدم
```sql
DELETE FROM [AspNetUserRoles]
WHERE [UserId] = @UserId AND [RoleId] = (
    SELECT [Id] FROM [AspNetRoles] WHERE [Name] = @RoleName
)
```

---

## 🔧 أوامر PowerShell المهمة

### 📌 إنشاء Migration جديد
```powershell
Add-Migration AddXXXToApplicationUser
```

### 📌 تطبيق الـ Migrations
```powershell
Update-Database
```

### 📌 الرجوع إلى Migration سابق
```powershell
Update-Database -Migration PreviousMigrationName
```

### 📌 حذف آخر Migration (قبل تطبيقه)
```powershell
Remove-Migration
```

---

## 🎨 معايير الكود الموصى بها

### ✅ نموذج جيد:
```csharp
// Models/ApplicationUser.cs
public class ApplicationUser : IdentityUser
{
    [Display(Name = "الاسم الكامل")]
    [StringLength(100, MinimumLength = 3, 
        ErrorMessage = "الاسم يجب أن يكون بين 3 و 100 حرف")]
    public string FullName { get; set; }
}
```

### ✅ صفحة Razor جيدة:
```csharp
// Pages/Profile/Index.cshtml.cs
[BindProperty]
[Required(ErrorMessage = "الحقل مطلوب")]
[StringLength(100)]
[Display(Name = "الاسم الكامل")]
public string FullName { get; set; }

public async Task<IActionResult> OnGetAsync()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return Challenge();
    
    FullName = user.FullName;
    return Page();
}

public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
        return Page();
    
    var user = await _userManager.GetUserAsync(User);
    user.FullName = FullName;
    await _userManager.UpdateAsync(user);
    
    StatusMessage = "تم التحديث بنجاح";
    return RedirectToPage();
}
```

### ✅ واجهة جيدة:
```html
<!-- Pages/Profile/Index.cshtml -->
<div class="card mb-3">
    <div class="card-body">
        <h6 class="card-title">البيانات الشخصية</h6>
        <form method="post">
            <div class="mb-3">
                <label asp-for="FullName" class="form-label"></label>
                <input asp-for="FullName" class="form-control" />
                <span asp-validation-for="FullName" class="text-danger"></span>
            </div>
            <button type="submit" class="btn btn-primary">حفظ</button>
        </form>
    </div>
</div>
```

---

## 🆘 استكشاف الأخطاء

### ❌ خطأ: "The type or namespace X could not be found"
```
👉 الحل: تأكد من إضافة using في رأس الملف
```

### ❌ خطأ: "Object reference not set to an instance"
```
👉 الحل: تحقق من null قبل استخدام الكائن
if (user != null) { ... }
```

### ❌ خطأ: "No tracking query attempted"
```
👉 الحل: استخدم AsNoTracking() للقراءة فقط
users.AsNoTracking().ToList()
```

### ❌ خطأ: HTTP 400 عند الرفع
```
👉 الحل: تحقق من حجم الملف وحدود Kestrel في Program.cs
```

### ❌ خطأ: "Concurrency check failed"
```
👉 الحل: أعد تحميل البيانات من قاعدة البيانات
user = await _userManager.FindByIdAsync(user.Id);
```

---

## 📊 ملخص الجداول الأساسية

| الجدول | الغرض | المفتاح الأساسي | المفاتيح الأجنبية |
|-------|-------|-------------|-----------------|
| AspNetUsers | بيانات المستخدمين | Id | - |
| AspNetRoles | الأدوار | Id | - |
| AspNetUserRoles | ربط المستخدمين بالأدوار | UserId, RoleId | UserId, RoleId |
| News | الأخبار | Id | AuthorId, CategoryId |
| Categories | فئات الأخبار | Id | - |
| Videos | الفيديوهات | Id | NewsId |
| Images | الصور | Id | NewsId |
| Documents | المستندات | Id | NewsId |
| Comments | التعليقات | Id | NewsId, UserId |

---

## 🎓 نصائح مهمة

1. **استخدم async/await** دائماً في العمليات المتعلقة بقاعدة البيانات
2. **تحقق من ModelState** قبل معالجة البيانات
3. **استخدم Transactions** عند تحديثات معقدة متعددة
4. **سجل الأخطاء** باستخدام logging (ILogger)
5. **اختبر التحقق من الصحة** على جانب العميل والخادم
6. **استخدم Entity Framework Interceptors** للتدقيق (Auditing)
7. **لا تعرّض معلومات حساسة** في رسائل الخطأ
8. **استخدم دائماً parameterized queries** لتجنب SQL Injection

---

## 📚 الملفات الموصى بقراءتها

1. `DOCUMENTATION_AR.md` - توثيق شامل لجميع الجداول
2. `DATA_FLOW_MAP_AR.md` - خريطة تفاعلية لمسار البيانات
3. `CHECKLIST_ADD_NEW_FIELD_AR.md` - خطوات إضافة حقل جديد
4. `Pages/Profile/Index.cshtml.cs` - مثال عملي
5. `Models/ApplicationUser.cs` - نموذج البيانات
