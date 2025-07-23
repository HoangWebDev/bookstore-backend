using BookStore.Request;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookStore.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleCredential _credential;
        private readonly string _modelName = "models/gemini-pro";

        public GeminiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();

            var serviceAccountPath = configuration["Gemini:ServiceAccountPath"];
            if (string.IsNullOrEmpty(serviceAccountPath))
                throw new ArgumentNullException("Path to Gemini service account JSON not found in configuration.");


            // Load service account credentials
            _credential = GoogleCredential.FromFile(serviceAccountPath)
                                            .CreateScoped("https://www.googleapis.com/auth/generative-language");
        }

        public async Task<string> GenerateContentAsync(PromptRequest request)
        {
            // Lấy Access Token
            var token = await _credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            string prompt = $@"
                    Bạn là một chuyên gia viết nội dung. Viết bằng tiếng Việt với yêu cầu:

                    🔹 Tiêu đề: {request.Title}
                    🔹 Từ khóa: {request.Keywords}
                    🔹 Độ dài mong muốn: {request.DesiredLength}
                    🔹 Có hình ảnh minh họa không?: {(request.HasImage ? "Có" : "Không")}

                    Yêu cầu:
                    - Ngắn gọn, rõ ràng, hấp dẫn
                    - Đúng ngữ pháp tiếng Việt";

            var url = "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent";

            var requestObj = new
            {
                contents = new[]
                {
                    new {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(requestObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Thêm token vào header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(url, content);
            var resultJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Gemini API Error: {response.StatusCode} - {resultJson}");
                throw new Exception($"Gemini error: {resultJson}");
            }

            dynamic result = JsonConvert.DeserializeObject(resultJson);
            var text = result?.candidates?[0]?.content?.parts?[0]?.text?.ToString();

            return string.IsNullOrWhiteSpace(text) ? "⚠️ Không có phản hồi từ Gemini." : text.Trim();
        }
    }
}
