using ImageUploadEF.Data;

namespace ImageUploadEF.Web.Models
{
    public class ViewImageVM
    {
        public Image Image { get; set; }
        public bool Liked { get; set; }
    }
}
