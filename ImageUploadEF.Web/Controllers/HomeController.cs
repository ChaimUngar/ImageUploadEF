using ImageUploadEF.Data;
using ImageUploadEF.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace ImageUploadEF.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _environment = environment;
        }

        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);
            List<Image> images = repo.GetAllImages();
            return View(images.OrderByDescending(i => i.UploadDate).ToList());
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile image, string title)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullImagePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            using FileStream fs = new (fullImagePath, FileMode.Create);
            image.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            repo.AddImage(new Image
            {
                Title = title,
                ImagePath = fileName,
                UploadDate = DateTime.Now
            });

            return Redirect("/");
        }

        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            Image image = repo.GetImageById(id);

            List<int> ids = HttpContext.Session.Get<List<int>>("LikedIds");

            if (ids == null)
            {
                ids = new();
            }

            return View(new ViewImageVM
            {
                Image = image,
                Liked = ids.Contains(id)
            });
        }

        public IActionResult GetSession()
        {
            List<int> ids = HttpContext.Session.Get<List<int>>("LikedIds");

            if (ids == null)
            {
                ids = new();
            }

            return Json(ids);
        }

        [HttpPost]
        public void SetSession(int id)
        {
            List<int> ids = HttpContext.Session.Get<List<int>>("LikedIds");

            if (ids == null)
            {
                ids = new();
            }

            ids.Add(id);
            HttpContext.Session.Set<List<int>>("LikedIds", ids);
        }

        [HttpPost]
        public void IncrementLikesForImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            repo.IncrementLikeForImage(id);
        }

        public IActionResult GetLikesById(int id)
        {
            var repo = new ImageRepository(_connectionString);
            int likes = repo.GetLikesForImage(id);
            return Json(likes);
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}