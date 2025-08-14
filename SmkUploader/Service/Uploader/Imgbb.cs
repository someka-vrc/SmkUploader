using Microsoft.Extensions.Configuration;

namespace SmkUploader.Service.Uploader
{
    internal class Imgbb : IUploader
    {
        private readonly HttpClient _httpClient;

        public string[] SupportedExtensions => [".png", ".jpg", ".jpeg", ".bmp", ".gif"];

        public Imgbb()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadAsync(Stream imageStream, string fileName, IConfiguration config, bool isDev)
        {
            try
            {
                var apiKey = config.GetValue<string>("SMKU_IMGBB_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                    throw new UploaderException("SMKU_IMGBB_API_KEY is not set.");

                var expirationHours = config.GetValue<int>("SMKU_IMGBB_EXPIRATION_HOURS");
                if (expirationHours <= 0)
                    throw new UploaderException("SMKU_IMGBB_EXPIRATION_HOURS must be a positive integer.");
                    
                int expirationSeconds = expirationHours * 3600;

                var fileContent = new StreamContent(imageStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                using var content = new MultipartFormDataContent
                {
                    { new StringContent(apiKey), "key" },
                    { fileContent, "image", fileName }
                };
                
                if (expirationSeconds > 0)
                {
                    content.Add(new StringContent(expirationSeconds.ToString()), "expiration");
                }

                var response = await _httpClient.PostAsync("https://api.imgbb.com/1/upload", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new UploaderException($"アップロードに失敗しました。 {response.StatusCode}: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Serilog.Log.Debug("レスポンス: {responseContent}", responseContent);

                var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<ImgbbResponse>(responseContent);
                if (jsonResponse?.Data?.Url != null)
                {
                    Serilog.Log.Debug("アップロード成功: {Url}", jsonResponse.Data.Url);
                    return jsonResponse.Data.Url;
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
            catch (System.Text.Json.JsonException ex)
            {
                throw new UploaderException($"レスポンスの解析に失敗しました。 {ex.Message}", ex);
            }
        }

        private class ImgbbResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("data")]
            public ImgbbData? Data { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("success")]
            public bool Success { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("status")]
            public int Status { get; set; }
        }

        private class ImgbbData
        {
            [System.Text.Json.Serialization.JsonPropertyName("id")]
            public string? Id { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("url")]
            public string? Url { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("display_url")]
            public string? DisplayUrl { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("delete_url")]
            public string? DeleteUrl { get; set; }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}