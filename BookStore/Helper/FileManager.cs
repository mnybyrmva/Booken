namespace BookStore.Helper
{
    public static class FileManager
    {
        public static string SaveFile(this IFormFile file, string rootpath, string FolderName)
        {
            string filename = file.FileName;
            filename = filename.Length > 64 ? filename.Substring(filename.Length - 64, 64) : filename;
            filename = Guid.NewGuid().ToString() + filename;
            string path = Path.Combine(rootpath, FolderName, filename);
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return filename;

        }
    }
}
