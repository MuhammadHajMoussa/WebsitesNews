# ✅ قائمة التفقد الشاملة لإضافة/تعديل حقول البيانات

## 📋 مثال: إضافة حقل جديد "Biography" (النبذة الشخصية)

### الخطوة 1️⃣: تعديل نموذج البيانات

**الملف**: `Models/ApplicationUser.cs`

```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    
    // ✅ إضافة حقل جديد
    public string Biography { get; set; }  // النبذة الشخصية
}
```

**الحالة**: ✅ تم إضافة الخاصية في النموذج

---

### الخطوة 2️⃣: إنشاء Migration (هجرة قاعدة البيانات)

**الأمر** (في Package Manager Console):
```powershell
Add-Migration AddBiographyToApplicationUser
```

**النتيجة**: يتم إنشاء ملف migration جديد بنفس الاسم

**مثال الملف الناتج** `Migrations/20240101120000_AddBiographyToApplicationUser.cs`:
```csharp
public partial class AddBiographyToApplicationUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Biography",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Biography",
            table: "AspNetUsers");
    }
}
```

**الحالة**: ✅ تم إنشاء Migration

---

### الخطوة 3️⃣: تطبيق Migration على قاعدة البيانات

**الأمر**:
```powershell
Update-Database
```

**النتيجة**: 
```sql
ALTER TABLE [AspNetUsers]
ADD [Biography] nvarchar(max) NULL
```

**الحالة**: ✅ تم تحديث جدول AspNetUsers في قاعدة البيانات

---

### الخطوة 4️⃣: إضافة حقل إلى الـ ViewModel

**الملف**: `Models/ViewModels/ProfileViewModel.cs`

```csharp
public class ProfileViewModel
{
    // ... الحقول الموجودة ...
    
    // ✅ إضافة حقل جديد
    [StringLength(500, ErrorMessage = "النبذة لا تتجاوز 500 حرف")]
    public string Biography { get; set; }  // النبذة الشخصية
}
```

**الحالة**: ✅ تم إضافة [BindProperty] للحقل

---

### الخطوة 5️⃣: جلب البيانات من قاعدة البيانات (OnGetAsync)

**الملف**: `Pages/Profile/Index.cshtml.cs`

```csharp
public async Task<IActionResult> OnGetAsync()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return Challenge();

    Id = user.Id;
    FullName = user.FullName;
    Email = user.Email;
    
    // ✅ جلب البيانات الجديدة من قاعدة البيانات
    Biography = user.Biography;
    
    // ... باقي الكود ...
    
    return Page();
}
```

**الحالة**: ✅ تم جلب البيانات من جدول AspNetUsers

---

### الخطوة 6️⃣: عرض البيانات على الصفحة

**الملف**: `Pages/Profile/Index.cshtml`

```html
<div class="card mb-3">
    <div class="card-body">
        <h6 class="card-title">البيانات الشخصية</h6>
        
        <!-- الحقول الموجودة -->
        <div class="row">
            <div class="col-md-6 mb-3">
                <label asp-for="FullName" class="form-label">الاسم الكامل</label>
                <input asp-for="FullName" class="form-control" />
            </div>
            
            <!-- ✅ إضافة حقل جديد -->
            <div class="col-md-6 mb-3">
                <label asp-for="Biography" class="form-label">النبذة الشخصية</label>
                <textarea asp-for="Biography" class="form-control" rows="3"></textarea>
                <small class="text-muted">أخبر الآخرين عن نفسك (حد أقصى 500 حرف)</small>
                <span asp-validation-for="Biography" class="text-danger"></span>
            </div>
        </div>
    </div>
</div>
```

**الحالة**: ✅ تم عرض الحقل على واجهة المستخدم

---

### الخطوة 7️⃣: تحديث البيانات في قاعدة البيانات (OnPostAsync)

**الملف**: `Pages/Profile/Index.cshtml.cs`

```csharp
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    var user = await _userManager.GetUserAsync(User);
    if (user == null) return Challenge();

    // تحديث البيانات الموجودة
    user.FullName = FullName;
    user.Email = Email;
    
    // ✅ تحديث البيانات الجديدة
    user.Biography = Biography;
    
    // حفظ جميع التحديثات في قاعدة البيانات
    var updateResult = await _userManager.UpdateAsync(user);
    
    if (!updateResult.Succeeded)
    {
        foreach (var e in updateResult.Errors)
            ModelState.AddModelError(string.Empty, e.Description);
        return Page();
    }

    StatusMessage = "تم تحديث الملف الشخصي بنجاح.";
    return RedirectToPage();
}
```

**استعلام SQL الناتج**:
```sql
UPDATE [AspNetUsers]
SET 
    [FullName] = @FullName,
    [Biography] = @Biography,
    [Email] = @Email,
    [ConcurrencyStamp] = @NewConcurrencyStamp
WHERE [Id] = @UserId
```

**الحالة**: ✅ تم تحديث البيانات في قاعدة البيانات

---

### ✅ النتيجة النهائية

عندما يفتح المستخدم الملف الشخصي:

```
FROM: AspNetUsers.Biography
     ↓
في الذاكرة: Biography = "أنا مطور ويب متخصص في ASP.NET Core"
     ↓
عرض على الصفحة: <textarea value="أنا مطور ويب متخصص في ASP.NET Core" />
     ↓
عند التعديل والحفظ:
     ↓
تحديث في: AspNetUsers.Biography
```

---

## 🔍 قائمة التفقد النهائية

استخدم هذه القائمة عند إضافة أي حقل جديد:

### ☑️ 1. نموذج البيانات
- [ ] تم إضافة الخاصية في `Models/ApplicationUser.cs`
- [ ] تم تحديد نوع البيانات الصحيح (string, int, DateTime, etc.)
- [ ] تم إضافة Validation Attributes إن لزم الأمر

### ☑️ 2. قاعدة البيانات
- [ ] تم إنشاء Migration: `Add-Migration AddXXX`
- [ ] تم تطبيق Migration: `Update-Database`
- [ ] تم التحقق من أن العمود تم إضافته للجدول

### ☑️ 3. نموذج الصفحة (PageModel)
- [ ] تم إضافة [BindProperty] للحقل الجديد في `Pages/Profile/Index.cshtml.cs`
- [ ] تم إضافة Validation Attributes إن لزم الأمر
- [ ] تم جلب البيانات في `OnGetAsync()`
- [ ] تم تحديث البيانات في `OnPostAsync()`

### ☑️ 4. واجهة المستخدم
- [ ] تم إضافة Label للحقل في `Pages/Profile/Index.cshtml`
- [ ] تم إضافة Input/TextArea/Select حسب النوع
- [ ] تم إضافة رسالة خطأ: `<span asp-validation-for="Field" />`
- [ ] تم إضافة وصف أو مساعدة إن لزم الأمر

### ☑️ 5. الاختبار
- [ ] تم الدخول إلى صفحة الملف الشخصي (GET)
- [ ] تم التحقق من ظهور البيانات الموجودة
- [ ] تم تعديل البيانات الجديدة
- [ ] تم حفظ التعديلات (POST)
- [ ] تم التحقق من ظهور رسالة النجاح
- [ ] تم إعادة تحميل الصفحة والتحقق من حفظ البيانات

---

## ⚠️ الأخطاء الشائعة

### ❌ خطأ 1: نسيان إضافة [BindProperty]

```csharp
// ❌ خطأ - الحقل لن يتم ملأه من النموذج
public string Biography { get; set; }

// ✅ صحيح
[BindProperty]
public string Biography { get; set; }
```

### ❌ خطأ 2: نسيان تحديث OnPostAsync

```csharp
// ❌ خطأ - البيانات الجديدة لن تُحفظ
user.FullName = FullName;
// لم نحدث Biography!

// ✅ صحيح
user.FullName = FullName;
user.Biography = Biography;
```

### ❌ خطأ 3: نسيان جلب البيانات في OnGetAsync

```csharp
// ❌ خطأ - الحقل سيكون فارغاً عند فتح الصفحة
// لم نجلب Biography!

// ✅ صحيح
Biography = user.Biography;
```

### ❌ خطأ 4: نسيان تطبيق Migration

```powershell
# ❌ خطأ - الجدول لم يتم تحديثه
Add-Migration AddBiography
# لم نقم بـ Update-Database!

# ✅ صحيح
Add-Migration AddBiography
Update-Database
```

### ❌ خطأ 5: استخدام قيمة افتراضية غير صحيحة

```csharp
// ❌ خطأ - قد يسبب خطأ إذا كانت NULL
string bio = Biography; // قد تكون null

// ✅ صحيح
string bio = Biography ?? "بلا وصف";
```

---

## 🎯 مثال سريع: إضافة حقل "DateOfBirth" (تاريخ الميلاد)

### 1. النموذج
```csharp
public DateTime? DateOfBirth { get; set; }
```

### 2. Razor Page
```csharp
[BindProperty]
[Display(Name = "تاريخ الميلاد")]
public DateTime? DateOfBirth { get; set; }

// في OnGetAsync
DateOfBirth = user.DateOfBirth;

// في OnPostAsync
user.DateOfBirth = DateOfBirth;
```

### 3. الواجهة
```html
<label asp-for="DateOfBirth">تاريخ الميلاد</label>
<input asp-for="DateOfBirth" type="date" class="form-control" />
<span asp-validation-for="DateOfBirth" class="text-danger"></span>
```

### 4. Migration
```powershell
Add-Migration AddDateOfBirthToApplicationUser
Update-Database
```

---

## 📞 الدعم

إذا واجهتك مشاكل، تحقق من:

1. **الخطأ**: "The type or namespace name 'Biography' could not be found"
   - **الحل**: تأكد من إضافة الخاصية في `ApplicationUser.cs`

2. **الخطأ**: Model binding لم يعمل
   - **الحل**: تأكد من إضافة `[BindProperty]`

3. **الخطأ**: البيانات القديمة تظهر
   - **الحل**: تأكد من تحديث `OnGetAsync()` لجلب البيانات الجديدة

4. **الخطأ**: HTTP 400 عند الحفظ
   - **الحل**: تحقق من حدود حجم الملفات في `Program.cs`
