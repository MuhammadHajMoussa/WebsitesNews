# توثيق نظام الصور الشخصية (Avatar System)

## نظرة عامة
نظام شامل لإدارة صور الملفات الشخصية للمستخدمين يجمع بين حفظ الملفات في نظام الملفات وتخزين المسارات في قاعدة البيانات.

---

## البنية الأساسية

### 1. **نموذج البيانات (ApplicationUser)**
**الملف:** `Models/ApplicationUser.cs`

```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    
    /// <summary>
    /// مسار الصورة الشخصية النسبي (مثل: /uploads/avatars/userid.jpg)
    /// </summary>
    public string AvatarPath { get; set; }  // ← عمود جديد في قاعدة البيانات
}
```

**العمود في قاعدة البيانات:**
- **الجدول:** AspNetUsers
- **العمود:** AvatarPath
- **النوع:** nvarchar(max) قابل للتعديل

---

## تدفق البيانات (Data Flow)

### أ) عند تحميل الملف الشخصي (GET)

```
1. المستخدم يفتح صفحة /Profile
   ↓
2. OnGetAsync() في Index.cshtml.cs
   ↓
3. جلب بيانات المستخدم من قاعدة البيانات
   └─ await _userManager.GetUserAsync(User)
   ↓
4. قراءة حقل AvatarPath من البيانات
   └─ user.AvatarPath (من الجدول AspNetUsers)
   ↓
5. عرض الصورة في الصفحة
   └─ <img src="@Model.AvatarUrl" />
```

**الملفات المعنية:**
- `Pages/Profile/Index.cshtml.cs` - OnGetAsync()
- `Pages/Profile/Index.cshtml` - العرض
- `AspNetUsers` - جدول قاعدة البيانات

---

### ب) عند تعديل/رفع صورة جديدة (POST)

```
1. المستخدم يختار صورة ويضغط "حفظ"
   ↓
2. OnPostAsync() يبدأ المعالجة
   ↓
3. التحقق من حجم الملف (الحد الأقصى 5 MB)
   ├─ إذا أكبر من 5 MB → رسالة خطأ
   └─ إذا OK → تابع
   ↓
4. التحقق من صيغة الملف (.jpg, .png, .gif)
   ├─ إذا غير مسموح → رسالة خطأ
   └─ إذا OK → تابع
   ↓
5. حفظ الملف في نظام الملفات
   └─ المسار: wwwroot/uploads/avatars/{userId}.{ext}
   └─ مثال: wwwroot/uploads/avatars/abc123xyz.jpg
   ↓
6. حفظ المسار النسبي في قاعدة البيانات
   └─ user.AvatarPath = "/uploads/avatars/abc123xyz.jpg"
   └─ UPDATE AspNetUsers SET AvatarPath = ... WHERE Id = ...
   ↓
7. تحديث بيانات المستخدم في قاعدة البيانات
   └─ await _userManager.UpdateAsync(user)
   ↓
8. إعادة التوجيه مع رسالة النجاح
```

**الملفات المعنية:**
- `Pages/Profile/Index.cshtml.cs` - OnPostAsync()
- `AspNetUsers` - جدول قاعدة البيانات
- `wwwroot/uploads/avatars/` - نظام الملفات

---

## التخزين

### أ) نظام الملفات
**المسار الفعلي:** `wwwroot/uploads/avatars/`

**أمثلة:**
```
wwwroot/uploads/avatars/550e8400-e29b-41d4-a716-446655440000.jpg
wwwroot/uploads/avatars/660f9511-f30c-42e5-b817-557766551111.png
wwwroot/uploads/avatars/770g0622-g41d-53f6-c928-668877662222.gif
```

### ب) قاعدة البيانات
**الجدول:** AspNetUsers
**الأعمدة المهمة:**

| العمود | النوع | الوصف |
|--------|-------|-------|
| Id | nvarchar(450) | معرف المستخدم الفريد |
| UserName | nvarchar(256) | اسم المستخدم |
| Email | nvarchar(256) | البريد الإلكتروني |
| FullName | nvarchar(max) | الاسم الكامل |
| **AvatarPath** | nvarchar(max) | **مسار الصورة النسبي** ← جديد |

**مثال على بيانات حقيقية:**
```sql
SELECT Id, UserName, FullName, AvatarPath 
FROM AspNetUsers
WHERE Id = '550e8400-e29b-41d4-a716-446655440000';

-- النتيجة:
-- Id: 550e8400-e29b-41d4-a716-446655440000
-- UserName: ahmad
-- FullName: أحمد محمد
-- AvatarPath: /uploads/avatars/550e8400-e29b-41d4-a716-446655440000.jpg
```

---

## المعالجة في الكود

### في OnPostAsync() - الحفظ
```csharp
// 1. حفظ الملف في نظام الملفات
using (var stream = new FileStream(filePath, FileMode.Create))
{
    await AvatarImage.CopyToAsync(stream);
}

// 2. تحديث مسار الصورة في البيانات
var avatarPath = "/uploads/avatars/" + fileName;
user.AvatarPath = avatarPath;  // ← تعيين المسار

// 3. حفظ المستخدم مع البيانات المحدثة
var updateResult = await _userManager.UpdateAsync(user);
```

### في OnGetAsync() - الاسترجاع
```csharp
// جلب المسار من قاعدة البيانات
AvatarUrl = !string.IsNullOrEmpty(user.AvatarPath) 
    ? user.AvatarPath 
    : "/images/default-avatar.png";
```

---

## الحدود والمتطلبات

### أ) حدود الملفات
- **الحد الأقصى للملف:** 5 MB
- **الصيغ المسموحة:** .jpg, .jpeg, .png, .gif
- **حد الطلب الأقصى:** 50 MB (Kestrel)

### ب) متطلبات الخادم
```csharp
// في Program.cs
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
});
```

---

## معالجة الأخطاء

### السيناريوهات المحتملة

| السيناريو | الخطأ | الحل |
|----------|------|------|
| الملف أكبر من 5 MB | "حجم الصورة كبير جداً. الحد الأقصى 5 MB." | تقليل حجم الصورة |
| صيغة غير مسموحة | "الملف غير مسموح. استخدم jpg, png, gif." | تحويل الصورة للصيغة الصحيحة |
| مشكلة في الرفع | "خطأ في رفع الصورة: [رسالة الخطأ]" | التحقق من صلاحيات المجلد |
| HTTP 400 | "Bad Request" | تأكد من إعدادات Kestrel |

---

## الملفات المتعلقة

### 1. **Models/ApplicationUser.cs**
- تعريف خاصية AvatarPath

### 2. **Pages/Profile/Index.cshtml.cs**
- `OnGetAsync()` - جلب البيانات
- `OnPostAsync()` - حفظ الصورة والبيانات

### 3. **Pages/Profile/Index.cshtml**
- عرض الصورة
- نموذج الرفع

### 4. **Views/Shared/_LoginPartial.cshtml**
- عرض صورة صغيرة في الـ Navbar

### 5. **Program.cs**
- إعدادات Kestrel وFormOptions

### 6. **Data/ApplicationDbContext.cs**
- تعريف Entity Framework

### 7. **Migrations/AddAvatarPathToApplicationUser**
- إضافة العمود الجديد (Seed Data)

---

## مثال عملي كامل

### السيناريو: مستخدم جديد يرفع صورة

```
1️⃣ المستخدم (ahmad) يدخل صفحة الملف الشخصي
   ✓ الملف الشخصي محمل بالبيانات الموجودة من قاعدة البيانات
   
2️⃣ يختار صورة (profile.jpg - 2 MB)
   
3️⃣ يضغط "حفظ التغييرات"
   
4️⃣ المعالجة:
   ✓ التحقق: 2 MB < 5 MB ✓
   ✓ التحقق: .jpg مسموح ✓
   ✓ حفظ: wwwroot/uploads/avatars/550e8400-e29b-41d4-a716-446655440000.jpg
   ✓ قاعدة البيانات: AvatarPath = "/uploads/avatars/550e8400-e29b-41d4-a716-446655440000.jpg"
   ✓ تحديث: UPDATE AspNetUsers ...
   
5️⃣ النتيجة:
   ✓ الصفحة تعيد التوجيه
   ✓ رسالة: "تم تحديث الملف الشخصي بنجاح"
   ✓ الصورة تظهر في كل مكان:
      - صفحة الملف الشخصي
      - شريط التنقل
      - أي مكان يعرض بيانات المستخدم
```

---

## الصيانة والتحديث

### إذا أردت تعديل حد الحجم الأقصى

```csharp
// في Pages/Profile/Index.cshtml.cs
const long maxFileSize = 10485760; // 10 MB بدلاً من 5 MB
```

### إذا أردت إضافة صيغ جديدة

```csharp
var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
```

### إذا أردت تغيير مسار التخزين

```csharp
private string GetAvatarDirectory() 
    => Path.Combine(_env.WebRootPath ?? string.Empty, "profile-images");
// بدلاً من "uploads/avatars"
```

---

## الخلاصة

النظام يعمل بطريقة متكاملة:
- ✅ **التخزين:** الملفات في نظام الملفات
- ✅ **الفهرسة:** المسارات في قاعدة البيانات
- ✅ **الاسترجاع:** من قاعدة البيانات مباشرة
- ✅ **الأمان:** معالجة الأخطاء والتحقق من الصيغ
- ✅ **الأداء:** لا يوجد تأخير في البحث عن الملفات

