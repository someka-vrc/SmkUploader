using Microsoft.Extensions.Configuration;

namespace SmkUploader.Service.Uploader;

internal static class UploaderFactory
{
    public static IUploader Create(IConfiguration config, bool isDev)
    {
        var service = config.GetValue<string>("SMKU_SERVICE")?.ToUpperInvariant();
        switch (service)
        {
            case "GYAZO":
                return new Gyazo();
            case "IMGBB":
                return new Imgbb();
            case "FREEIMAGEHOST":
                return new FreeimageHost();
            // 他サービス追加時はここにcaseを追加
            default:
                throw new ArgumentException($"SMKU_SERVICE: {service} はサポートされていません。");
        }
    }
}
