using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageUploadEF.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;
        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetAllImages()
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.ToList();
        }

        public void AddImage(Image image)
        {
            using var context = new ImageDataContext(_connectionString);
            context.Add(image);
            context.SaveChanges();
        }

        public Image GetImageById(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }

        public void IncrementLikeForImage(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            Image image = context.Images.FirstOrDefault(i => i.Id == id);
            image.Likes++;
            context.Entry(image).State = EntityState.Modified;
            context.SaveChanges();
        }

        public int GetLikesForImage(int id)
        {
            Image image = GetImageById(id);
            return image.Likes;
        }
    }
}
