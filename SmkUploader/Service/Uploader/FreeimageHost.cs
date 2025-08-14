using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmkUploader.Service.Uploader
{
    internal class FreeimageHost : IUploader
    {
        private readonly HttpClient _httpClient;

        public string[] SupportedExtensions => [".png", ".jpg", ".jpeg", ".bmp", ".gif"];

        public FreeimageHost()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadAsync(Stream imageStream, string fileName, IConfiguration config, bool isDev)
        {
            try
            {
                var apiKey = config.GetValue<string>("SMKU_FREEIMAGEHOST_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                    throw new UploaderException("SMKU_FREEIMAGEHOST_API_KEY is not set.");

                var fileContent = new StreamContent(imageStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                using var content = new MultipartFormDataContent
                {
                    { new StringContent(apiKey), "key" },
                    { new StringContent("upload"), "action" },
                    { fileContent, "source", fileName },
                    { new StringContent("json"), "format" }
                };

                var response = await _httpClient.PostAsync("https://freeimage.host/api/1/upload", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new UploaderException($"アップロードに失敗しました。 {response.StatusCode}: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Serilog.Log.Debug("レスポンス: {responseContent}", responseContent);

                var jsonResponse = JsonSerializer.Deserialize<FreeimageHostResponse>(responseContent);
                if (jsonResponse?.Image?.Url != null)
                {
                    Serilog.Log.Debug("アップロード成功: {Url}", jsonResponse.Image.Url);
                    return jsonResponse.Image.Url;
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

        private class FreeimageHostResponse
        {
            [JsonPropertyName("status_code")]
            public int StatusCode { get; set; }
            [JsonPropertyName("image")]
            public FreeimageHostImage? Image { get; set; }
        }

        private class FreeimageHostImage
        {
            [JsonPropertyName("url")]
            public string? Url { get; set; }
            [JsonPropertyName("display_url")]
            public string? DisplayUrl { get; set; }
            [JsonPropertyName("url_viewer")]
            public string? UrlViewer { get; set; }
        }
    }
}
