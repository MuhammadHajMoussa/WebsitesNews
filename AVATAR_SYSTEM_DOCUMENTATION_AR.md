# توثيق نظام الصور الشخصية (Avatar System)

## نظرة عامة
تم تحديث نظام الصور الشخصية بالكامل بحيث لا يعتمد على نظام الملفات (`wwwroot`)، بل يتم تحويل الصورة إلى مصفوفة بايتات `byte[]` وتُخزّن مباشرة في قاعدة البيانات. يتم عرضها في الواجهة على شكل سلسلة نصية `Base64`.

---

## البنية الأساسية

### 1. **نموذج البيانات (ApplicationUser)**
**الملف:** `Models/ApplicationUser.cs`

```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    
    /// <summary>
    /// الصورة الشخصية مخزنة كبيانات ثنائية (Byte Array)
    /// </summary>
    public byte[]? ProfilePicture { get; set; }
}
```

**العمود في قاعدة البيانات:**
- **الجدول:** AspNetUsers
- **العمود:** ProfilePicture
- **النوع:** varbinary(max)

---

## تدفق البيانات (Data Flow)

### أ) عند تحميل الملف الشخصي (GET)

```
1. المستخدم يفتح صفحة /Profile
   ↓
2. Index() (GET) في ProfileController
   ↓
3. جلب بيانات المستخدم من قاعدة البيانات
   └─ await _userManager.GetUserAsync(User)
   ↓
4. قراءة حقل AvatarPath من البيانات
   └─ user.ProfilePicture (من الجدول AspNetUsers)
   ↓
5. عرض الصورة في الصفحة
   └─ <img src="@Model.AvatarUrl" />
```

**الملفات المعنية:**
- `Controllers/ProfileController.cs` - Index() (GET)
- `Views/Profile/Index.cshtml` - العرض
- `AspNetUsers` - جدول قاعدة البيانات

---

### ب) عند تعديل/رفع صورة جديدة (POST)

```
1. المستخدم يختار صورة ويضغط "حفظ"
   ↓
2. Index() (POST) يبدأ المعالجة
   ↓
3. التحقق من حجم الملف (الحد الأقصى 5 MB)
   ├─ إذا أكبر من 5 MB → رسالة خطأ
   └─ إذا OK → تابع
   ↓
4. التحقق من صيغة الملف (.jpg, .png, .gif)
   ├─ إذا غير مسموح → رسالة خطأ
   └─ إذا OK → تابع
   ↓
5. تحويل الملف إلى بيانات ثنائية
   └─ باستخدام MemoryStream وتحويله إلى byte[]
   ↓
6. تخزين الصورة في قاعدة البيانات
   └─ user.ProfilePicture = memoryStream.ToArray()
   ↓
7. تحديث بيانات المستخدم في قاعدة البيانات
   └─ await _userManager.UpdateAsync(user)
   ↓
8. إعادة التوجيه مع رسالة النجاح
```

**الملفات المعنية:**
- `Controllers/ProfileController.cs` - Index() (POST)
- `AspNetUsers` - جدول قاعدة البيانات

---

## التخزين

### قاعدة البيانات (حيث يتم كل شيء)
**الجدول:** AspNetUsers
**الأعمدة المهمة:**

| العمود | النوع | الوصف |
|--------|-------|-------|
| Id | nvarchar(450) | معرف المستخدم الفريد |
| UserName | nvarchar(256) | اسم المستخدم |
| Email | nvarchar(256) | البريد الإلكتروني |
| FullName | nvarchar(max) | الاسم الكامل |
| **ProfilePicture** | varbinary(max) | **بيانات الصورة الثنائية** |

**مثال على بيانات حقيقية:**
```sql
SELECT Id, UserName, FullName 
FROM AspNetUsers
WHERE Id = '550e8400-e29b-41d4-a716-446655440000';

-- النتيجة:
-- Id: 550e8400-e29b-41d4-a716-446655440000
-- UserName: ahmad
-- (البيانات الثنائية يتم عرضها في التطبيق عبر Base64)
```

---

## المعالجة في الكود

### في Controller (POST) - الحفظ
```csharp
// 1. حفظ الملف في نظام الملفات
using (var stream = new FileStream(filePath, FileMode.Create))
{
    await model.AvatarImage.CopyToAsync(stream);
}

// 2. تحديث مسار الصورة في البيانات
var avatarPath = "/uploads/avatars/" + fileName;
user.AvatarPath = avatarPath;  // ← تعيين المسار

// 3. حفظ المستخدم مع البيانات المحدثة
var updateResult = await _userManager.UpdateAsync(user);
```

### في Controller (GET) - الاسترجاع
```csharp
// جلب المسار من قاعدة البيانات
model.AvatarUrl = !string.IsNullOrEmpty(user.AvatarPath) 
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

### 2. **Controllers/ProfileController.cs**
- `Index()` GET - جلب البيانات
- `Index()` POST - حفظ الصورة والبيانات

### 3. **Views/Profile/Index.cshtml**
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
// في Controllers/ProfileController.cs
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
