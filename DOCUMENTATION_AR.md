# 📋 توثيق تطابق البيانات والجداول - نظام الأخبار

## 1️⃣ جداول قاعدة البيانات الرئيسية

### جدول `AspNetUsers` (الهوية - Identity)
**الغرض**: تخزين بيانات المستخدمين

```sql
جدول: AspNetUsers
الأعمدة الأساسية:
- Id (مفتاح أساسي)
- UserName (اسم المستخدم)
- Email (البريد الإلكتروني)
- FullName (الاسم الكامل) - إضافة مخصصة
- PhoneNumber (رقم الهاتف)
- EmailConfirmed (تأكيد البريد)
- PhoneNumberConfirmed (تأكيد الهاتف)
- TwoFactorEnabled (التحقق الثنائي)
- LockoutEnd (نهاية الحظر)
- LockoutEnabled (تفعيل الحظر)
- AccessFailedCount (عدد محاولات الفشل)
- PasswordHash (كلمة المرور المشفرة)
- SecurityStamp (ختم الأمان)
- ConcurrencyStamp (ختم التزامن)
- NormalizedUserName (اسم المستخدم الموحد)
- NormalizedEmail (البريد الموحد)
```

**نموذج C#**:
```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
}
```

**الملف المرتبط**: `Models/ApplicationUser.cs`

---

### جدول `AspNetRoles` (الأدوار)
**الغرض**: تعريف الأدوار المختلفة

```sql
جدول: AspNetRoles
الأعمدة:
- Id (مفتاح أساسي)
- Name (اسم الدور): Owner, Admin, Author, Subscriber
- NormalizedName (اسم الدور الموحد)
- ConcurrencyStamp
```

**الأدوار المحددة مسبقاً**:
- `Owner` - مالك الموقع (صلاحيات كاملة)
- `Admin` - مسؤول (إدارة الأخبار والمستخدمين)
- `Author` - كاتب (كتابة وتعديل أخباره الخاصة)
- `Subscriber` - مشترك (قراءة فقط)

---

### جدول `AspNetUserRoles` (ربط المستخدم بالدور)
**الغرض**: تعريف أدوار كل مستخدم

```sql
جدول: AspNetUserRoles
الأعمدة:
- UserId (مفتاح أجنبي → AspNetUsers)
- RoleId (مفتاح أجنبي → AspNetRoles)
```

---

### جدول `News` (الأخبار)
**الغرض**: تخزين الأخبار والمقالات

```sql
جدول: News
الأعمدة:
- Id (مفتاح أساسي)
- Title (عنوان الخبر)
- Content (محتوى الخبر)
- AuthorId (مفتاح أجنبي → AspNetUsers)
- CategoryId (مفتاح أجنبي → Categories)
- PublishDate (تاريخ النشر)
- Views (عدد المشاهدات)
- Likes (عدد الإعجابات)
```

**نموذج C#**:
```csharp
public class News
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorId { get; set; }
    public ApplicationUser Author { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public DateTime PublishDate { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public ICollection<Video> Videos { get; set; }
    public ICollection<Image> Images { get; set; }
    public ICollection<Document> Documents { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
```

---

### جدول `Categories` (الفئات)
**الغرض**: تصنيف الأخبار

```sql
جدول: Categories
الأعمدة:
- Id
- Name (اسم الفئة): رياضة, تكنولوجيا, سياسة, ترفيه
- Slug (رابط صديق): riyada, technology, politics, entertainment
```

**البيانات المحددة مسبقاً**:
```csharp
new Category { Id = 1, Name = "رياضة", Slug = "riyada" }
new Category { Id = 2, Name = "تكنولوجيا", Slug = "technology" }
new Category { Id = 3, Name = "سياسة", Slug = "politics" }
new Category { Id = 4, Name = "ترفيه", Slug = "entertainment" }
```

---

### جدول `Videos` (الفيديوهات)
**الغرض**: تخزين الفيديوهات المرتبطة بالأخبار

```sql
جدول: Videos
الأعمدة:
- Id
- NewsId (مفتاح أجنبي → News)
- FileName (اسم الملف)
- FileContent (محتوى الملف الثنائي)
- FileSize (حجم الملف)
- FileType (نوع الملف: video/mp4)
- Title (عنوان الفيديو)
- UploadDate (تاريخ الرفع)
```

---

### جدول `Images` (الصور)
**الغرض**: تخزين الصور المرتبطة بالأخبار

```sql
جدول: Images
الأعمدة:
- Id
- NewsId (مفتاح أجنبي → News)
- FileName (اسم الملف)
- FileContent (محتوى الملف الثنائي)
- FileSize (حجم الملف)
- FileType (نوع الملف: image/jpeg, image/png)
- Title (عنوان الصورة)
- UploadDate (تاريخ الرفع)
```

---

### جدول `Documents` (المستندات)
**الغرض**: تخزين المستندات المرتبطة بالأخبار

```sql
جدول: Documents
الأعمدة:
- Id
- NewsId (مفتاح أجنبي → News)
- FileName (اسم الملف)
- FileContent (محتوى الملف الثنائي)
- FileSize (حجم الملف)
- FileType (نوع الملف: application/pdf)
- Title (عنوان المستند)
- UploadDate (تاريخ الرفع)
```

---

### جدول `Comments` (التعليقات)
**الغرض**: تخزين تعليقات المستخدمين على الأخبار

```sql
جدول: Comments
الأعمدة:
- Id
- NewsId (مفتاح أجنبي → News)
- UserId (مفتاح أجنبي → AspNetUsers)
- Content (محتوى التعليق)
- CreatedAt (تاريخ الإنشاء)
```

---

## 2️⃣ مسار البيانات: من قاعدة البيانات إلى الواجهة

### 📌 مثال: تحديث بيانات الملف الشخصي

#### **المرحلة 1️⃣: جلب البيانات من قاعدة البيانات**
```csharp
// الملف: Pages/Profile/Index.cshtml.cs
public async Task<IActionResult> OnGetAsync()
{
    var user = await _userManager.GetUserAsync(User);
    // البحث في جدول AspNetUsers عن المستخدم الحالي
    
    if (user == null) return Challenge();

    // نقل البيانات من قاعدة البيانات إلى خصائص النموذج
    Id = user.Id;                          // من: AspNetUsers.Id
    FullName = user.FullName;              // من: AspNetUsers.FullName
    Email = user.Email;                    // من: AspNetUsers.Email
    UserName = user.UserName;              // من: AspNetUsers.UserName
    EmailConfirmed = user.EmailConfirmed;  // من: AspNetUsers.EmailConfirmed
    PhoneNumber = user.PhoneNumber;        // من: AspNetUsers.PhoneNumber
    
    // جلب الأدوار من جدول AspNetUserRoles
    Roles = (await _userManager.GetRolesAsync(user)).ToArray();
    
    // جلب صورة الملف الشخصي من المجلد
    AvatarUrl = FindAvatarUrlForUser(user.Id) ?? "/images/default-avatar.png";

    return Page();
}
```

#### **المرحلة 2️⃣: عرض البيانات على الصفحة**
```html
<!-- الملف: Pages/Profile/Index.cshtml -->
<div class="card-body">
    <!-- عرض الصورة الشخصية من الملف -->
    <img src="@Model.AvatarUrl" class="rounded-circle" />
    
    <!-- عرض البيانات من خصائص النموذج -->
    <h5>@Model.FullName</h5>
    <p>@Model.Email</p>
    
    <!-- عرض الأدوار من جدول AspNetUserRoles -->
    @foreach (var r in Model.Roles)
    {
        <span class="badge">@r</span>
    }
</div>

<!-- نموذج التعديل -->
<form method="post" enctype="multipart/form-data">
    <input asp-for="FullName" />
    <input asp-for="Email" />
    <input asp-for="PhoneNumber" />
    <input asp-for="AvatarImage" type="file" />
    <button type="submit">حفظ التغييرات</button>
</form>
```

#### **المرحلة 3️⃣: تحديث البيانات في قاعدة البيانات**
```csharp
// الملف: Pages/Profile/Index.cshtml.cs
public async Task<IActionResult> OnPostAsync()
{
    var user = await _userManager.GetUserAsync(User);
    
    // تحديث البيانات النصية
    user.FullName = FullName;                          // ← تحديث: AspNetUsers.FullName
    user.PhoneNumber = PhoneNumber;                    // ← تحديث: AspNetUsers.PhoneNumber
    user.EmailConfirmed = EmailConfirmed;              // ← تحديث: AspNetUsers.EmailConfirmed
    
    // حفظ في قاعدة البيانات
    var updateResult = await _userManager.UpdateAsync(user);
    
    // تحديث البريد الإلكتروني (يتطلب معالجة خاصة)
    if (user.Email != Email)
    {
        var setEmailResult = await _userManager.SetEmailAsync(user, Email);
        // ← تحديث: AspNetUsers.Email
    }
    
    // تحديث كلمة المرور (جدول منفصل)
    if (!string.IsNullOrEmpty(NewPassword))
    {
        var changePassResult = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
        // ← تحديث: AspNetUsers.PasswordHash
    }
    
    // حفظ الصورة الشخصية في المجلد
    if (AvatarImage != null && AvatarImage.Length > 0)
    {
        var filePath = Path.Combine(GetAvatarDirectory(), user.Id + ext);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await AvatarImage.CopyToAsync(stream);
        }
        // ملاحظة: الصور تُخزن في الملف النظام وليس في قاعدة البيانات
    }
    
    StatusMessage = "تم تحديث الملف الشخصي بنجاح.";
    return RedirectToPage();
}
```

---

## 3️⃣ العلاقات بين الجداول

```
┌─────────────────────────┐
│   AspNetUsers           │
│─────────────────────────│
│ Id                      │◄─────────┐
│ UserName                │          │
│ Email                   │          │
│ FullName                │          │
│ PasswordHash            │          │
│ PhoneNumber             │          │
│ ... (الأعمدة الأخرى)     │          │
└─────────────────────────┘          │
         ▲          ▲                │
         │          │                │
    ┌────┴──────┬───┴────────┐      │
    │            │            │      │
┌───┴────┐  ┌───┴────┐  ┌───┴────┐  │
│ News   │  │Comments│  │Avatars │  │
│────────│  │────────│  │────────│  │
│AuthorId├──┤UserId  │  │UserId ◄───┘
│Title   │  │Content │  │Path    │
│Content │  │NewsId  │  │        │
└────────┘  └────────┘  └────────┘
     │
     │ CategoryId
     ▼
┌─────────────┐
│ Categories  │
│─────────────│
│ Id          │
│ Name        │
│ Slug        │
└─────────────┘

┌────────────────┐
│ AspNetUserRoles│
│────────────────│
│ UserId ────────┼──► AspNetUsers
│ RoleId ────────┼──► AspNetRoles
└────────────────┘

┌──────────────┐
│ AspNetRoles  │
│──────────────│
│ Id           │
│ Name         │
└──────────────┘
```

---

## 4️⃣ نقاط التحديث الرئيسية

| البيان | الجدول | الطريقة | الملف المسؤول |
|------|--------|--------|-------------|
| الاسم الكامل | AspNetUsers.FullName | UpdateAsync | Pages/Profile/Index.cshtml.cs |
| البريد الإلكتروني | AspNetUsers.Email | SetEmailAsync | Pages/Profile/Index.cshtml.cs |
| رقم الهاتف | AspNetUsers.PhoneNumber | UpdateAsync | Pages/Profile/Index.cshtml.cs |
| كلمة المرور | AspNetUsers.PasswordHash | ChangePasswordAsync | Pages/Profile/Index.cshtml.cs |
| الصورة الشخصية | ملف النظام (wwwroot/uploads/avatars) | FileStream | Pages/Profile/Index.cshtml.cs |
| الأدوار | AspNetUserRoles | AddToRoleAsync / RemoveFromRoleAsync | Services/UserService.cs |
| تأكيد البريد | AspNetUsers.EmailConfirmed | UpdateAsync | Pages/Profile/Index.cshtml.cs |
| التحقق الثنائي | AspNetUsers.TwoFactorEnabled | UpdateAsync | Pages/Profile/Index.cshtml.cs |

---

## 5️⃣ قائمة التحقق عند التعديل

عند إضافة أي حقل جديد للملف الشخصي:

✅ **1. أضفه إلى نموذج البيانات** `ApplicationUser`
```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string YourNewField { get; set; }  // ← إضافة جديدة
}
```

✅ **2. أنشئ migration**
```powershell
Add-Migration AddYourNewField
Update-Database
```

✅ **3. أضفه كـ [BindProperty] في صفحة النموذج**
```csharp
[BindProperty]
public string YourNewField { get; set; }
```

✅ **4. احصل عليه من قاعدة البيانات في OnGetAsync**
```csharp
YourNewField = user.YourNewField;
```

✅ **5. اعرضه على الصفحة**
```html
<input asp-for="YourNewField" />
```

✅ **6. حدثه في OnPostAsync**
```csharp
user.YourNewField = YourNewField;
var result = await _userManager.UpdateAsync(user);
```

---

## 6️⃣ الملفات الرئيسية للمرجعية

| الملف | الوصف |
|------|-------|
| `Data/ApplicationDbContext.cs` | تعريف الجداول والعلاقات |
| `Models/ApplicationUser.cs` | نموذج بيانات المستخدم |
| `Pages/Profile/Index.cshtml.cs` | منطق تحديث الملف الشخصي |
| `Pages/Profile/Index.cshtml` | واجهة الملف الشخصي |
| `Services/UserService.cs` | خدمات المستخدم (الأدوار، إلخ) |
| `Areas/Identity/Pages/_ViewStart.cshtml` | إعدادات Layout للصفحات |
| `Program.cs` | إعدادات حجم الملفات والخادم |

---

## ℹ️ ملاحظات مهمة

⚠️ **الصور الشخصية** تُخزن في النظام، وليس في قاعدة البيانات
- المسار: `wwwroot/uploads/avatars/{UserId}.{extension}`
- الامتدادات المسموحة: jpg, jpeg, png, gif
- الحد الأقصى للحجم: 5 MB

⚠️ **الأدوار** تُدار عبر `UserManager`
- استخدم `AddToRoleAsync` لإضافة دور
- استخدم `RemoveFromRoleAsync` لإزالة دور
- استخدم `GetRolesAsync` للحصول على الأدوار

⚠️ **كلمة المرور** مشفرة بـ PBKDF2
- لا تُخزن بصيغة نصية أبداً
- استخدم دائماً `ChangePasswordAsync`

---
