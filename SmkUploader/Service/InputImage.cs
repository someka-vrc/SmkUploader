

using Serilog;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;

namespace SmkUploader.Service
{
    internal class InputImage
    {
        /// <summary>
        /// 引数またはクリップボードから有効な画像を取得します。<br />
        /// 1. 引数が空でない場合
        /// 1-1. 引数がファイルパスの場合：存在チェックと拡張子チェックを行い返却します。<br />
        /// 1-2. 引数がURLの場合：URLから画像をダウンロードし画像を返却します。ただしファイルが画像でなかった場合やサイズが20MBを超える場合は失敗します。<br />
        /// 2. 引数が空の場合
        /// 2-1. クリップボードがテキストの場合：クリップボードの内容をもとに 1. と同様に処理します。<br />
        /// 2-2. クリップボードが画像の場合：クリップボードの内容を返却します。ただしファイルが画像でなかった場合は失敗します。<br />
        /// </summary>
        /// <param name="pathOrUrl">空でない場合は解析します。空の場合はクリップボードを解析します。</param>
        /// <returns></returns>
        internal Image Get(string? pathOrUrl, out string ext)
        {
            string? input = pathOrUrl;
            if (string.IsNullOrWhiteSpace(input))
            {
                // STAスレッドでクリップボード操作
                string? clipboardText = null;
                System.Drawing.Image? clipboardImage = null;
                var thread = new Thread(() =>
                {
                    if (Clipboard.ContainsText())
                    {
                        Log.Debug("クリップボードタイプ: Text");
                        clipboardText = Clipboard.GetText();
                    }
                    else if (Clipboard.ContainsImage())
                    {
                        Log.Debug("クリップボードタイプ: Image");
                        clipboardImage = Clipboard.GetImage();
                    }
                    else if (Clipboard.ContainsFileDropList())
                    {
                        var files = Clipboard.GetFileDropList();
                        if (files.Count > 0)
                        {
                            Log.Debug("クリップボードタイプ: FileDropList");
                            clipboardText = files[0];
                        }
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                if (!string.IsNullOrEmpty(clipboardText))
                {
                    input = clipboardText;
                }
                else if (clipboardImage != null)
                {
                    using var ms = new MemoryStream();
                    clipboardImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    ext = ".png";
                    return Image.Load(ms);
                }
                else
                {
                    throw new ArgumentException("クリップボードに画像またはテキストがありません。");
                }
            }

            // inputが""で囲まれている場合削除
            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                input = input[1..^1];
            }

            // 入力がファイルパスかURLか判定
            if (Uri.TryCreate(input, UriKind.Absolute, out var uri) && uri != null && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                Log.Debug("入力テキスト: URL {Url}", input);
                using var client = new HttpClient();
                var response = client.Send(new HttpRequestMessage(HttpMethod.Get, input), HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                    throw new ImageParseException($"画像のダウンロードに失敗しました: {response.StatusCode}");
                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (string.IsNullOrEmpty(contentType) || !contentType.StartsWith("image/"))
                    throw new ArgumentException($"Content-Typeが画像ではありません: {contentType}");
                var contentLength = response.Content.Headers.ContentLength;
                if (contentLength.HasValue && contentLength.Value > 20 * 1024 * 1024)
                    throw new ArgumentException("画像サイズが20MBを超えています。");
                // ストリームで20MB超過時点で中断
                using var stream = response.Content.ReadAsStream();
                using var ms = new MemoryStream();
                var buffer = new byte[81920];
                int read;
                long total = 0;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    total += read;
                    if (total > 20 * 1024 * 1024)
                        throw new ArgumentException("画像サイズが20MBを超えています。");
                    ms.Write(buffer, 0, read);
                }
                // ImageSharpで画像判定
                try
                {
                    ms.Position = 0;
                    var image = Image.Load(ms);
                    ext = Path.GetExtension(uri.AbsolutePath);
                    if (string.IsNullOrEmpty(ext) || ext.Length > 5)
                    {
                        // Content-Typeから拡張子を推定
                        ext = contentType switch
                        {
                            "image/jpeg" => ".jpg",
                            "image/png" => ".png",
                            "image/gif" => ".gif",
                            "image/bmp" => ".bmp",
                            "image/tiff" => ".tiff",
                            "image/webp" => ".webp",
                            _ => ".img"
                        };
                    }
                    ext = ext.ToLower();
                    return image;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("ダウンロードしたファイルが画像ではありません。", ex);
                }
            }
            else
            {
                Log.Debug("入力テキスト: ファイルパス {Path}", input);
                // ファイルパスの場合
                if (!File.Exists(input))
                    throw new ArgumentException("ファイルが存在しません。");
                var fi = new FileInfo(input);
                if (fi.Length > 20 * 1024 * 1024)
                    throw new ArgumentException("画像サイズが20MBを超えています。");
                try
                {
                    ext = Path.GetExtension(input).ToLower();
                    return Image.Load(input);
                }
                catch (Exception ex)
                {
                    throw new ImageParseException("ファイルが画像として認識できません。", ex);
                }
            }
        }
    }

    internal class ImageParseException : Exception
    {
        internal ImageParseException(string message) : base(message) { }
        internal ImageParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
