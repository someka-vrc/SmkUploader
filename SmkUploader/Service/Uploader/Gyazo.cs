using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SmkUploader.Service.Uploader
{
    internal class Gyazo : IUploader
    {
        private readonly HttpClient _httpClient;

        public string[] SupportedExtensions => [".png", ".jpg", ".jpeg", ".gif"];

        public Gyazo()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadAsync(Stream imageStream, string fileName, IConfiguration config, bool isDev)
        {
            try
            {
                var token = config.GetValue<string>("SMKU_GYAZO_TOKEN");
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("SMKU_GYAZO_TOKEN is not set.");

                var fileContent = new StreamContent(imageStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                using var content = new MultipartFormDataContent
                {
                    // access_token（必須）
                    { new StringContent(token), "access_token" },
                    // imagedata（必須）- filenameを必ず含める
                    { fileContent, "imagedata", fileName },
                    // オプションパラメータ
                    { new StringContent("anyone"), "access_policy" },
                    { new StringContent("SmkUploader"), "app" }
                };

                var response = await _httpClient.PostAsync("https://upload.gyazo.com/api/upload", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new UploaderException($"アップロードに失敗しました。 {response.StatusCode}: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Log.Debug("レスポンス: {responseContent}", responseContent);

                // レスポンスをパースしてURLを取得
                var jsonResponse = JsonSerializer.Deserialize<GyazoResponse>(responseContent);
                if (jsonResponse?.Url != null)
                {
                    Log.Debug("アップロード成功: {Url}", jsonResponse.Url);
                    return jsonResponse.Url;
                }
                else
                {
                    throw new UploaderException("アップロードには成功しましたが、レスポンスに画像URLが含まれていません。");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new UploaderException($"ネットワークエラーが発生しました。 {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new UploaderException($"レスポンスの解析に失敗しました。 {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    internal class GyazoResponse
    {
        [JsonPropertyName("image_id")]
        public string? ImageId { get; set; }

        [JsonPropertyName("permalink_url")]
        public string? PermalinkUrl { get; set; }

        [JsonPropertyName("thumb_url")]
        public string? ThumbUrl { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}