public static class MyUtil
{
    public static string UpLoadHinh(IFormFile file, string folder)
    {
        var folderPath = Path.Combine("wwwroot", "Hinh", folder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return fileName;
    }

    public static void DeleteHinh(string fileName, string folder)
    {
        var filePath = Path.Combine("wwwroot", "Hinh", folder, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
