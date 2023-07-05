
namespace Novels.Handlers
{
    public class UploadFile
    {
        
        public static string Upload(IFormFile file, IWebHostEnvironment host)
        {

            string folder = Path.Combine(host.ContentRootPath, "images");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(folder, uniqueFileName);
            using(var filestream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(filestream);
            }
            
            return uniqueFileName;
        }
    }
}
