namespace SmkUploader.Service.Uploader
{
    internal static class UploaderUtil
    {
        internal static string GetUploaderDirectory<T>(bool isDev) where T : IUploader
        {
            string name = typeof(T).Name.ToLower();

            var rootDir = isDev
                ? Path.Combine(Directory.GetCurrentDirectory(), "Uploader")
                : Path.Combine(AppContext.BaseDirectory, "Uploader");
            if (!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }
            string uploaderDir = Path.Combine(rootDir, name);
            if (!Directory.Exists(uploaderDir))
            {
                Directory.CreateDirectory(uploaderDir);
            }
            return uploaderDir;
        }
    }
}