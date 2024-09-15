using Microsoft.AspNetCore.Hosting;

namespace Inventory.Extensions
{
    public static class InventoryUtility
    {
        public static async Task<string> UploadImage(IFormFile imgFile,string uploadFolderPath,string folderName)
        {
            string uniqueFileName = string.Empty;
            if (imgFile != null)
            {
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var imageExtension = Path.GetExtension(imgFile.FileName);
                var imageName = Path.GetFileNameWithoutExtension(imgFile.FileName).Length > 20 ?
                    Path.GetFileNameWithoutExtension(imgFile.FileName).Substring(0, 20) + imageExtension :
                     Path.GetFileNameWithoutExtension(imgFile.FileName) + imageExtension;
                if (!allowedImageExtensions.Contains(imageExtension.ToLower()))
                {
                    throw new ArgumentException("Invalid image type.");
                }               
                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }
                // Generate unique file name using current date, time, and a random string
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 3); // 8 character suffix
                uniqueFileName = $"{timestamp}_{randomSuffix}_{imageName}";

                //uniqueFileName = Guid.NewGuid().ToString() + "_" + imgFile.FileName;
                var filePath = Path.Combine(uploadFolderPath, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imgFile.CopyTo(fileStream);
                }
                uniqueFileName = $"/{folderName}/{uniqueFileName}";
            }
            return uniqueFileName;
        }
    }
}
