
using Microsoft.Extensions.Configuration;

namespace SmkUploader.Service.Uploader;

internal interface IUploader : IDisposable
{

    /// <summary>
    /// アップローダーがサポートする画像ファイルの拡張子を取得します。
    /// 下記のVRChat(Unityアプリ)がサポートしている形式以外は定義しないでください:
    /// ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga"
    /// </summary>
    /// <value></value>
    string[] SupportedExtensions { get; }

    /// <summary>
    /// 画像ファイルをアップロードします。
    /// </summary>
    /// <param name="imageStream">アップロードする画像データのストリーム</param>
    /// <param name="fileName">アップロードする画像ファイル名（拡張子付き）</param>
    /// <param name="config">アプリケーションの設定</param>
    /// <param name="isDev">開発環境かどうか</param>
    /// <returns>アップロード結果のURL</returns>
    Task<string> UploadAsync(Stream imageStream, string fileName, IConfiguration config, bool isDev);
}

internal class UploaderException : Exception
{
    public UploaderException(string message) : base(message) { }

    public UploaderException(string message, Exception innerException) : base(message, innerException) { }
}
