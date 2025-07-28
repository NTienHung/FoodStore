using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FoodStoreClient.Pages.Admin.Restoran_Management_ChatBot
{
    public class ChatBotModel : PageModel
    {
        public class ChatMessage
        {
            public string Role { get; set; }
            public string Message { get; set; }
        }

        [BindProperty]
        public string UserPrompt { get; set; }
        public List<ChatMessage> ChatHistory { get; set; } = new List<ChatMessage>();
        public string ErrorMessage { get; set; }

        public List<Dictionary<string, object>> TableResult { get; set; } = new();
        public List<string> TableHeaders { get; set; } = new();

        private const string ChatHistoryKey = "ChatHistory";

        public void OnGet()
        {
            LoadHistory();
        }

        public async Task<IActionResult> OnPostSendAsync()
        {
            LoadHistory();

            if (string.IsNullOrWhiteSpace(UserPrompt))
            {
                ErrorMessage = "Please enter your question.";
                return Page();
            }

            ChatHistory.Add(new ChatMessage { Role = "User", Message = UserPrompt });

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var apiUrl = "http://localhost:7031/api/admin/ChatBot/ask";
                    var reqBody = JsonSerializer.Serialize(new { request = UserPrompt });
                    var content = new StringContent(reqBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);
                    response.EnsureSuccessStatusCode();

                    var respString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(respString);

                    var aiMsg = doc.RootElement.TryGetProperty("summary", out var summaryProp)
                        ? summaryProp.GetString()
                        : respString;
                    ChatHistory.Add(new ChatMessage { Role = "AI", Message = aiMsg });

                    if (doc.RootElement.TryGetProperty("sql", out var sqlProp))
                    {
                        var sql = sqlProp.GetString();
                        if (!string.IsNullOrWhiteSpace(sql))
                        {
                            Console.WriteLine($"[SQL Generated]: {sql}");
                        }
                    }

                    if (doc.RootElement.TryGetProperty("result", out var resultProp))
                    {
                        if (resultProp.ValueKind == JsonValueKind.Object)
                        {
                            var obj = resultProp;

                            if (obj.TryGetProperty("$values", out var innerArray) && innerArray.ValueKind == JsonValueKind.Array)
                            {
                                TableResult = new List<Dictionary<string, object>>();

                                foreach (var item in innerArray.EnumerateArray())
                                {
                                    var row = new Dictionary<string, object>();
                                    foreach (var prop in item.EnumerateObject())
                                    {
                                        row[prop.Name] = prop.Value.ToString();
                                    }
                                    TableResult.Add(row);
                                }

                                TableHeaders = TableResult.FirstOrDefault()?.Keys.ToList() ?? new List<string>();
                            }
                            else
                            {
                                var dict = new Dictionary<string, object>();
                                foreach (var prop in obj.EnumerateObject())
                                {
                                    dict[prop.Name] = prop.Value.ToString();
                                }
                                TableResult = new List<Dictionary<string, object>> { dict };
                                TableHeaders = dict.Keys.ToList();
                            }
                        }
                        else
                        {
                            TableResult = new List<Dictionary<string, object>>();
                            TableHeaders = new List<string>();
                        }
                    }
                    else
                    {
                        TableResult = new List<Dictionary<string, object>>();
                        TableHeaders = new List<string>();
                    }

                    ErrorMessage = null;
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
                ChatHistory.Add(new ChatMessage { Role = "AI", Message = ErrorMessage });
                TableResult = new List<Dictionary<string, object>>();
                TableHeaders = new List<string>();
            }

            SaveHistory();
            UserPrompt = string.Empty;
            return Page();
        }

        private void LoadHistory()
        {
            if (TempData.TryGetValue(ChatHistoryKey, out var obj) && obj is string json && !string.IsNullOrEmpty(json))
            {
                ChatHistory = JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>();
            }
        }

        private void SaveHistory()
        {
            TempData[ChatHistoryKey] = JsonSerializer.Serialize(ChatHistory);
        }
    }
}
