using Microsoft.AspNetCore.Identity;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;


    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task PromoteUserAsync(string userId, string newRole);
        Task DeleteUserAsync(string userId);
        Task<string[]> GetUserRolesAsync(string userId); // إضافة جديدة
    }
