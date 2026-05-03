using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsSite.Models;

namespace NewsSite.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; }

        public DbSet<Video> Videos { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تعديل قيد العلاقة بين Comments و News
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.News)
                .WithMany(n => n.Comments)
                .HasForeignKey(c => c.NewsId)
                .OnDelete(DeleteBehavior.Restrict);

            // تعديل قيد العلاقة بين News و Author
            modelBuilder.Entity<News>()
                .HasOne(n => n.Author)
                .WithMany()
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // تعديل قيد العلاقة بين Comments و User
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Data لفئات الأخبار
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "رياضة", Slug = "riyada" },
                new Category { Id = 2, Name = "تكنولوجيا", Slug = "technology" },
                new Category { Id = 3, Name = "سياسة", Slug = "politics" },
                new Category { Id = 4, Name = "ترفيه", Slug = "entertainment" }
            );

            // تحسين الأداء: إنشاء فهارس (Indexes) لعمليات الفرز والفلترة الشائعة
            modelBuilder.Entity<News>()
                .HasIndex(n => n.CategoryId)
                .HasDatabaseName("IX_News_CategoryId");

            // فهرس مركب لتسريع استعلامات الفلترة حسب التصنيف والفرز حسب التاريخ
            modelBuilder.Entity<News>()
                .HasIndex(n => new { n.CategoryId, n.PublishDate })
                .HasDatabaseName("IX_News_Category_PublishDate");

            // فهرس على العنوان لتحسين عمليات البحث البسيطة (ليس بديلاً عن full-text)
            modelBuilder.Entity<News>()
                .HasIndex(n => n.Title)
                .HasDatabaseName("IX_News_Title");

            // فهرس على Slug في الفئات لاستخدامه في الروابط الصديقة
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Category_Slug");
        }
    }
}

