﻿using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsSite.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _context;

        public NewsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<News>> GetAllNewsAsync(string? category = null) // ← السماح بـ null
        {
            var query = _context.News
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Include(n => n.Videos)
                .Include(n => n.Images)
                .Include(n => n.Documents)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                var catNorm = category.Trim().ToLower();
                query = query.Where(n => n.Category != null
                                         && n.Category.Name != null
                                         && n.Category.Name.ToLower() == catNorm);
            }

            return await query.ToListAsync();
        }

        public async Task<News> GetNewsByIdAsync(int id)
        {
            return (await _context.News
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Include(n => n.Comments)
                .Include(n => n.Videos)
                .Include(n => n.Images)
                .Include(n => n.Documents)
                .FirstOrDefaultAsync(n => n.Id == id))!;
        }

        public async Task<int> AddNewsAsync(NewsViewModel model, string userId)
        {
            var news = new News
            {
                Title = model.Title ?? "", // ← قيمة افتراضية بدل null
                Content = model.Content ?? "",
                CategoryId = model.CategoryId,
                AuthorId = userId,
                PublishDate = DateTime.Now
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return news.Id;
        }

        public async Task UpdateNewsAsync(NewsViewModel model, string userId)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == model.Id);
            if (news == null) throw new KeyNotFoundException("الخبر غير موجود");
            if (news.AuthorId != userId) throw new UnauthorizedAccessException("غير مصرح لك بتعديل هذا الخبر");

            news.Title = model.Title ?? "";
            news.Content = model.Content ?? "";
            news.CategoryId = model.CategoryId;

            _context.News.Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNewsAsync(int id, string userId)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null) throw new KeyNotFoundException("الخبر غير موجود");
            if (news.AuthorId != userId) throw new UnauthorizedAccessException("غير مصرح لك بحذف هذا الخبر");

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
        }

        public async Task LikeNewsAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                news.Likes++;
                _context.News.Update(news);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCommentAsync(int newsId, string content, string userId)
        {
            var comment = new Comment
            {
                Content = content ?? "", // ← قيمة افتراضية بدل null
                UserId = userId,
                NewsId = newsId
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}
