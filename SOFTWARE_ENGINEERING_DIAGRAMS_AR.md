# 📐 مخططات هندسة البرمجيات ونموذج التطوير

يوثق هذا الملف منهجية التطوير المتبعة في بناء موقع الأخبار (NewsSite)، بالإضافة إلى المخططات الهندسية (UML و Architecture Diagrams) التي تصف تفاعل مكونات النظام.

---

## 1️⃣ نموذج التطوير المتبع (Development Model)

النموذج الهندسي الذي تم اعتماده في تطوير هذا النظام هو **النموذج التكراري التزايدي (Iterative and Incremental Model)** المنسجم مع مبادئ **Agile**.

### 💡 لماذا هذا النموذج؟
1. **التزايد (Incremental):** تم بناء النظام على شكل وحدات (Modules) متتالية. بدأنا بأساسيات تسجيل الدخول، ثم أضفنا نظام الملف الشخصي، ثم نظام الأخبار، ثم المرفقات والتعليقات.
2. **التكرار (Iterative):** قمنا بتحسين نفس الميزة عدة مرات بناءً على المتطلبات. (مثال: نظام الصور الشخصية بدأ بحفظ الملفات في المجلدات `wwwroot`، ثم في تكرار لاحق تم تعديله ليحفظ البيانات الثنائية `byte[]` في قاعدة البيانات مباشرة لتحسين الأمان وقابلية النقل).

---

## 2️⃣ البنية المعمارية للنظام (System Architecture)

يعتمد المشروع على المعمارية القياسية **MVC (Model-View-Controller)** لضمان فصل الاهتمامات (Separation of Concerns).

```mermaid
flowchart TD
    %% تعريف المكونات
    User((المستخدم))
    View["الواجهة (Views/UI)
    HTML/CSS/JS/Razor"]
    Controller["المتحكم (Controllers)
    NewsController / ProfileController"]
    Model["النماذج (Models)
    ApplicationUser / News / ViewModels"]
    DB[(قاعدة البيانات
    SQL Server)]
    Service["طبقة الخدمات (Services)
    UserService / NewsService"]

    %% الروابط
    User <-->|يتفاعل مع| View
    View -->|يرسل طلبات HTTP| Controller
    Controller -->|يمرر البيانات (ViewModel)| View
    Controller -->|يستدعي منطق العمل| Service
    Service -->|يقرأ/يكتب| Model
    Service <-->|Entity Framework Core| DB
    Controller <-->|مباشرة أحياناً| DB
```

---

## 3️⃣ مخطط حالات الاستخدام (Use Case Diagram)

يوضح هذا المخطط أنواع المستخدمين (Actors) في النظام والصلاحيات الممنوحة لكل منهم.

```mermaid
flowchart LR
    %% Actors
    Guest((زائر))
    Subscriber((مشترك))
    Author((كاتب))
    Admin((مدير / مالك))

    %% Use Cases
    UC_Browse(تصفح الأخبار والبحث)
    UC_Login(تسجيل الدخول والاشتراك)
    UC_Interact(التعليق والإعجاب)
    UC_Profile(إدارة الملف الشخصي)
    UC_News(إضافة وتعديل الأخبار والمرفقات)
    UC_Users(إدارة المستخدمين والأدوار والفئات)

    %% Relations
    Guest --> UC_Browse
    Guest --> UC_Login

    Subscriber --> UC_Browse
    Subscriber --> UC_Interact
    Subscriber --> UC_Profile

    Author --> UC_Browse
    Author --> UC_Interact
    Author --> UC_Profile
    Author --> UC_News

    Admin --> UC_Browse
    Admin --> UC_Interact
    Admin --> UC_Profile
    Admin --> UC_News
    Admin --> UC_Users

    %% Styling
    style Guest fill:#f9f9f9,stroke:#333,stroke-width:2px
    style Subscriber fill:#d4edda,stroke:#28a745,stroke-width:2px
    style Author fill:#cce5ff,stroke:#004085,stroke-width:2px
    style Admin fill:#f8d7da,stroke:#dc3545,stroke-width:2px
```

---

## 4️⃣ مخطط التتابع (Sequence Diagram)

يوضح المخطط التالي تسلسل العمليات عند **إضافة كاتب لخبر جديد يحتوي على مرفقات (صور/فيديوهات)**.

```mermaid
sequenceDiagram
    autonumber
    actor Author as الكاتب (Author)
    participant UI as الواجهة (Create.cshtml)
    participant Ctrl as المتحكم (NewsController)
    participant Srv as الخدمة (NewsService)
    participant DB as قاعدة البيانات (SQL Server)

    Author->>UI: إدخال عنوان ومحتوى الخبر
    Author->>UI: رفع ملفات (صور/فيديو) واختيار الفئة
    UI->>Ctrl: إرسال الطلب (POST /News/Create)
    
    Ctrl->>Ctrl: التحقق من صحة البيانات (ModelState.IsValid)
    Ctrl->>Ctrl: التحقق من عدد الملفات (< 5) وإجمالي الحجم
    
    alt البيانات غير صالحة
        Ctrl-->>UI: إعادة الصفحة مع رسائل الخطأ
    else البيانات صالحة
        Ctrl->>Srv: AddNewsAsync(model, userId)
        Srv->>DB: حفظ تفاصيل الخبر النصية (News Table)
        DB-->>Srv: إرجاع معرف الخبر الجديد (NewsId)
        Srv-->>Ctrl: إرجاع NewsId
        
        loop لكل ملف مرفق
            Ctrl->>Ctrl: التحقق من نوع الملف (MIME Type)
            Ctrl->>Ctrl: تحويل الملف عبر MemoryStream إلى Byte Array
            Ctrl->>DB: حفظ كبيانات ثنائية في (Images/Videos/Documents)
        end
        
        DB-->>Ctrl: تأكيد الحفظ (SaveChangesAsync)
        Ctrl->>UI: إعادة التوجيه (Redirect To Index)
        UI-->>Author: عرض الخبر الجديد في الصفحة الرئيسية
    end
```

---

## 5️⃣ تقنيات وأدوات الهندسة المستخدمة

- **النمط المعماري:** MVC (Model-View-Controller).
- **إطار العمل:** ASP.NET Core 8.0 / 7.0.
- **الوصول للبيانات (ORM):** Entity Framework Core (Code-First Approach).
- **إدارة الهوية:** ASP.NET Core Identity (للمصادقة والتفويض).
- **تأمين البيانات:**
  - تشفير كلمات المرور (Password Hashing).
  - الحماية من هجمات CSRF باستخدام `[ValidateAntiForgeryToken]`.
  - التعقيم (Sanitization) لأسماء الملفات المرفوعة.
- **التخزين (Storage):** 
  - تخزين النصوص والبيانات الأساسية في الجداول.
  - تخزين الملفات والصور كبيانات ثنائية `varbinary(max)` (Byte Arrays) لضمان المركزية وتقليل مشاكل صلاحيات خوادم الملفات.

---
*تم توليد هذه المخططات باستخدام لغة Mermaid المتوافقة بالكامل مع مستودعات GitHub وأنظمة التوثيق الحديثة.*