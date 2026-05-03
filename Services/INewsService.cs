

using NewsSite.Models;
using NewsSite.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsSite.Services;

public interface INewsService
{
    Task<List<News>> GetAllNewsAsync(string category = null);
    Task<News> GetNewsByIdAsync(int id);
    Task<int> AddNewsAsync(NewsViewModel model, string userId);
    Task UpdateNewsAsync(NewsViewModel model, string userId);
    Task DeleteNewsAsync(int id, string userId);
    Task LikeNewsAsync(int id);
    Task AddCommentAsync(int newsId, string content, string userId);
}