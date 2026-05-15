# 🔄 مسار البيانات الكامل - خريطة تفاعلية

## 🎯 مثال عملي: تحديث الملف الشخصي للمستخدم

```
┌──────────────────────────────────────────────────────────────────────────┐
│                        1️⃣ الطلب الأولي (GET)                              │
│                                                                          │
│  المستخدم يفتح: https://localhost:7151/Profile                           │
│                                                                          │
│  ↓                                                                       │
│  الطلب يصل إلى: ProfileController.cs → Index() (GET)                     │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                   2️⃣ جلب البيانات من قاعدة البيانات                      │
│                                                                          │
│  await _userManager.GetUserAsync(User)                                   │
│         ↓                                                                │
│  البحث في: Table [AspNetUsers]                                           │
│         ↓                                                                │
│  حيث: WHERE NormalizedUserName = @CurrentUserName                        │
│         ↓                                                                │
│  النتيجة:                                                                │
│  ┌─────────────────────────────────┐                                     │
│  │ Id          = "user-123"        │                                     │
│  │ UserName    = "mohammad"        │                                     │
│  │ Email       = "user@test.com"   │                                     │
│  │ FullName    = "محمد علي"       │                                      │
│  │ PhoneNumber = "+966501234567"   │                                      │
│  │ ProfilePicture= <binary data>   │                                      │
│  │ ... (أعمدة أخرى)               │                                      │
│  └─────────────────────────────────┘                                     │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                   3️⃣ جلب الأدوار من جدول الروابط                         │
│                                                                          │
│  await _userManager.GetRolesAsync(user)                                  │
│         ↓                                                                │
│  البحث في:                                                              │
│  SELECT ar.Name                                                          │
│  FROM [AspNetUserRoles] AS ur                                            │
│  INNER JOIN [AspNetRoles] AS ar ON ur.RoleId = ar.Id                    │
│  WHERE ur.UserId = "user-123"                                            │
│         ↓                                                                │
│  النتيجة: ["Admin", "Author"]                                           │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                   4️⃣ جلب الصورة الشخصية من قاعدة البيانات وتحويلها        │
│                                                                          │
│  user.ProfilePicture != null                                             │
│         ↓ نعم                                                            │
│  Convert.ToBase64String(user.ProfilePicture)                             │
│         ↓                                                                │
│  النتيجة: "data:image/jpeg;base64,/9j/4AAQSkZJ..."                       │
│  (أو الصورة الافتراضية إذا كانت NULL)                                     │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                   5️⃣ ملء نموذج الصفحة بالبيانات                         │
│                                                                          │
│  ProfileViewModel model = new ProfileViewModel()                         │
│  {                                                                       │
│      Id = "user-123",                            // من: DB              │
│      UserName = "mohammad",                      // من: DB              │
│      Email = "user@test.com",                    // من: DB              │
│      FullName = "محمد علي",                      // من: DB              │
│      PhoneNumber = "+966501234567",              // من: DB              │
│      EmailConfirmed = true,                      // من: DB              │
│      PhoneNumberConfirmed = false,               // من: DB              │
│      TwoFactorEnabled = false,                   // من: DB              │
│      Roles = ["Admin", "Author"],                // من: DB (جدول الربط)  │
│      AvatarUrl = "data:image/jpeg;..."           // Base64              │
│  }                                                                       │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                   6️⃣ عرض الصفحة بالبيانات (HTML)                         │
│                                                                          │
│  يتم تحويل النموذج إلى HTML:                                            │
│                                                                          │
│  <img src="data:image/jpeg;base64,/9j/4A..." />                          │
│  <input value="mohammad" />                                              │
│  <input value="user@test.com" />                                         │
│  <input value="محمد علي" />                                             │
│  <input value="+966501234567" />                                         │
│  <span class="badge">Admin</span>                                        │
│  <span class="badge">Author</span>                                       │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                        7️⃣ المستخدم يعدل البيانات                         │
│                                                                          │
│  يغير المستخدم:                                                        │
│  - FullName → "محمد علي الأحمدي"                                        │
│  - PhoneNumber → "+966509876543"                                         │
│  - يرفع صورة جديدة                                                      │
│                                                                          │
│  ثم يضغط: "حفظ التغييرات"                                              │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                        8️⃣ إرسال الطلب (POST)                             │
│                                                                          │
│  HTTP POST → /Profile                                                    │
│  Content-Type: multipart/form-data                                       │
│                                                                          │
│  البيانات:                                                              │
│  {                                                                       │
│      UserName: "mohammad",                                               │
│      Email: "user@test.com",                                             │
│      FullName: "محمد علي الأحمدي",  ← تغيير                            │
│      PhoneNumber: "+966509876543",   ← تغيير                            │
│      AvatarImage: <binary file>,     ← صورة جديدة                      │
│      ...                                                                 │
│  }                                                                       │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                      9️⃣ معالجة الطلب في Controller (POST)                 │
│                                                                          │
│  ✓ تحقق من صحة البيانات (ModelState)                                   │
│  ✓ احصل على المستخدم الحالي                                            │
│                                                                          │
│  إذا كان UserName مختلفاً (model.UserName):                               │
│  → await _userManager.SetUserNameAsync(user, model.UserName)             │
│     ← تحديث: [AspNetUsers].UserName                                     │
│                                                                          │
│  إذا كان Email مختلفاً (model.Email):                                     │
│  → await _userManager.SetEmailAsync(user, model.Email)                   │
│     ← تحديث: [AspNetUsers].Email                                        │
│     ← تحديث: [AspNetUsers].NormalizedEmail                              │
│                                                                          │
│  تحديث البيانات الأخرى:                                                 │
│  user.FullName = model.FullName                                          │
│  user.PhoneNumber = model.PhoneNumber                                    │
│  user.EmailConfirmed = model.EmailConfirmed                              │
│  ...                                                                     │
│  → await _userManager.UpdateAsync(user)                                  │
│     ← تحديث: [AspNetUsers] الصف بالكامل                                │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                    🔟 معالجة تغيير كلمة المرور                           │
│                                                                          │
│  إذا أدخل المستخدم كلمة مرور جديدة (model.NewPassword):                    │
│  → await _userManager.ChangePasswordAsync(user, model.CurrentPassword,     │
│                                            model.NewPassword)            │
│     ↓                                                                    │
│  التحقق من صحة كلمة المرور الحالية                                      │
│  ← تحديث: [AspNetUsers].PasswordHash (بعد التشفير)                      │
│  ← تحديث: [AspNetUsers].SecurityStamp                                   │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                    1️⃣1️⃣ معالجة رفع الصورة الشخصية                       │
│                                                                          │
│  إذا تم رفع صورة:                                                      │
│                                                                          │
│  ① تحقق من نوع الملف: jpg, jpeg, png, gif                              │
│                                                                          │
│  ② تحقق من حجم الملف: ≤ 5 MB                                           │
│                                                                          │
│  ③ احذف الصور القديمة:                                                 │
│     (لم يعد مطلوباً - الحفظ الآن في قاعدة البيانات)                     │
│                                                                          │
│  ④ احفظ الصورة الجديدة:                                                 │
│     user.ProfilePicture = memoryStream.ToArray()                        │
│     model.AvatarUrl = "data:image/jpeg;base64,..."                      │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                    1️⃣2️⃣ الحفظ في قاعدة البيانات                        │
│                                                                          │
│  UPDATE [AspNetUsers]                                                    │
│  SET                                                                     │
│      [FullName] = 'محمد علي الأحمدي',                                  │
│      [PhoneNumber] = '+966509876543',                                    │
│      [EmailConfirmed] = 1,                                               │
│      [TwoFactorEnabled] = 0,                                             │
│      [PhoneNumberConfirmed] = 0,                                         │
│      [LockoutEnd] = NULL,                                                │
│      [LockoutEnabled] = 1,                                              │
│      [ProfilePicture] = <binary data>,                                  │
│      [AccessFailedCount] = 0                                             │
│  WHERE [Id] = 'user-123'                                                 │
│                                                                          │
│  ← تم التحديث بنجاح                                                    │
└──────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────┐
│                    1️⃣3️⃣ إعادة التوجيه والتأكيد                          │
│                                                                          │
│  HTTP 302 Redirect → /Profile                                           │
│                                                                          │
│  TempData["StatusMessage"] = "تم تحديث الملف الشخصي بنجاح."              │
│                                                                          │
│  المستخدم يرى:                                                         │
│  ✅ رسالة نجاح خضراء                                                    │
│  ✅ البيانات المحدثة                                                    │
│  ✅ الصورة الجديدة                                                      │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## 🗄️ استعلامات SQL الفعلية التي تحدث

### 1. جلب المستخدم:
```sql
SELECT TOP(1) 
    [a].[Id], [a].[UserName], [a].[Email], [a].[FullName],
    [a].[PhoneNumber], [a].[EmailConfirmed], [a].[PhoneNumberConfirmed],
    [a].[TwoFactorEnabled], [a].[LockoutEnd], [a].[LockoutEnabled],
    [a].[AccessFailedCount], [a].[PasswordHash], [a].[SecurityStamp],
    [a].[ConcurrencyStamp], [a].[NormalizedUserName], [a].[NormalizedEmail]
FROM [AspNetUsers] AS [a]
WHERE [a].[NormalizedUserName] = @__normalizedUserName_0
```

### 2. جلب الأدوار:
```sql
SELECT [a0].[Name]
FROM [AspNetUserRoles] AS [a]
INNER JOIN [AspNetRoles] AS [a0] ON [a].[RoleId] = [a0].[Id]
WHERE [a].[UserId] = @__userId_0
```

### 3. تحديث بيانات المستخدم:
```sql
UPDATE [AspNetUsers]
SET 
    [FullName] = @FullName,
    [PhoneNumber] = @PhoneNumber,
    [EmailConfirmed] = @EmailConfirmed,
    [TwoFactorEnabled] = @TwoFactorEnabled,
    [ConcurrencyStamp] = @ConcurrencyStamp
WHERE [Id] = @Id
```

---

## 📝 جدول المراسلات

| البيانات على الصفحة | النموذج (C#) | جدول قاعدة البيانات | العمود |
|---|---|---|---|
| اسم المستخدم | UserName | AspNetUsers | UserName |
| البريد | Email | AspNetUsers | Email |
| الاسم الكامل | FullName | AspNetUsers | FullName |
| الهاتف | PhoneNumber | AspNetUsers | PhoneNumber |
| تأكيد البريد | EmailConfirmed | AspNetUsers | EmailConfirmed |
| الأدوار | Roles[] | AspNetUserRoles + AspNetRoles | RoleId / Name |
| الصورة | AvatarUrl | AspNetUsers | ProfilePicture |

---

## ⚙️ العمليات المعقدة

### إضافة دور للمستخدم:
```csharp
await _userManager.AddToRoleAsync(user, "Admin");
// ← INSERT INTO [AspNetUserRoles] (UserId, RoleId) VALUES (@UserId, @RoleId)
```

### إزالة دور:
```csharp
await _userManager.RemoveFromRoleAsync(user, "Author");
// ← DELETE FROM [AspNetUserRoles] WHERE UserId = @UserId AND RoleId = @RoleId
```

### تغيير كلمة المرور:
```csharp
await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
// ← UPDATE [AspNetUsers] SET [PasswordHash] = @NewHash WHERE [Id] = @UserId
```

---

## 🚀 الملفات المسؤولة عن كل خطوة

| الخطوة | الملف | الدالة |
|------|------|-------|
| GET الملف الشخصي | Pages/Profile/Index.cshtml.cs | OnGetAsync |
| POST تحديث البيانات | Pages/Profile/Index.cshtml.cs | OnPostAsync |
| عرض الصفحة | Pages/Profile/Index.cshtml | - |
| تصميم قاعدة البيانات | Data/ApplicationDbContext.cs | OnModelCreating |
| نموذج البيانات | Models/ApplicationUser.cs | - |
| إدارة الأدوار | Services/UserService.cs | PromoteUserAsync |
| الإعدادات | Program.cs | - |
