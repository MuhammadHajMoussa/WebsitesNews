using Microsoft.AspNetCore.Mvc;
using NewsSite.Services;
using NewsSite.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

public class HomeController : Controller
{
    private readonly INewsService _newsService;

    public HomeController(INewsService newsService)
    {
        _newsService = newsService;
    }

    public async Task<IActionResult> Index(string category = null)
    {
        // Get latest news to show on home page, optionally filtered by category
        var news = await _newsService.GetAllNewsAsync(category);
        ViewBag.SelectedCategory = category;
        return View(news);
    }

    public IActionResult Error()
    {
        return View();
    }
}
