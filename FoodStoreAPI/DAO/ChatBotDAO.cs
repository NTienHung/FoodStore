using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FoodStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Azure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static FoodStoreAPI.DTOs.ChatBotDTO;

namespace FoodStoreAPI.DAO
{
    public class ChatBotDAO
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly FoodStoreContext _dbContext;

        public ChatBotDAO(IConfiguration configuration, FoodStoreContext dbContext)
        {
            _apiKey = configuration["OpenAI:ApiKey"];
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _dbContext = dbContext;
        }

        public async Task<AIQuery> ConvertQuestionToSql(string request)
        {
            var schemaBuilder = new StringBuilder();
            schemaBuilder.AppendLine("Database schema:");
            schemaBuilder.AppendLine("Account(AccId, Email, Phone, Name, Username, Password, Wallet, CreateAt, UpdateAt, RoleId, Address)");
            schemaBuilder.AppendLine("AccountShipped(AccId, TransId, AddressShop, AddressCustomer, CreateAt, UpdateAt, Status)");
            schemaBuilder.AppendLine("Category(CateId, CateName)");
            schemaBuilder.AppendLine("Messagess(MessId, FromUserId, ToUserId, MessageText, SentTime)");
            schemaBuilder.AppendLine("Order(OrderId, CustomerId, Gtotal, CreateAt, UpdateAt, Status, OrderDate)");
            schemaBuilder.AppendLine("OrderItem(OrderItemId, OrderId, ProductId, Quantity, CreateAt, UpdateAt)");
            schemaBuilder.AppendLine("Product(ProId, Name, Price, OriginalPrice, Unit, Images, create_at, update_at, Quantity, ProductStatus, AccId, CateId, DiscountStartTime, DiscountEndTime, DiscountPercentage)");
            schemaBuilder.AppendLine("ProductReview(ReviewId, Images, CreateAt, UpdateAt, Star, ProId, AccId)");
            schemaBuilder.AppendLine("Revenue(RevenueId, RevenuePrice, InterestRate, FloorFee, CreateAt, AccId)");
            schemaBuilder.AppendLine("Role(RoleId, RoleName)");
            schemaBuilder.AppendLine("Token(TokenId, CreateAt, TokenString, ExpirationDate, Status, AccId)");
            schemaBuilder.AppendLine("Transaction(TransId, AccId, ProId, TransDate, Amount, Status)");

            var builder = new StringBuilder();
            builder.AppendLine("You are a helpful, cheerful database assistant. Do not respond with any information unrelated to databases or queries. Use the following database schema when creating your answers:");
            builder.AppendLine(schemaBuilder.ToString());
            builder.AppendLine("Include column name headers in the query results.");
            builder.AppendLine("Always provide your answer in the JSON format below:");
            builder.AppendLine(@"{ ""summary"": ""your-summary"", ""query"":  ""your-query"" }");
            builder.AppendLine("Output ONLY JSON formatted on a single line. Do not use new line characters.");
            builder.AppendLine(@"In the preceding JSON response, substitute ""your-query"" with SQL Server (Microsoft SQL) Query to retrieve the requested data.");
            builder.AppendLine(@"In the preceding JSON response, substitute ""your-summary"" with an explanation of each step you took to create this query in a detailed paragraph.");
            builder.AppendLine("Do not use MySQL syntax.");
            builder.AppendLine("Always limit the SQL Query to 100 rows.");
            builder.AppendLine("Always include all of the table columns and details.");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = builder.ToString() },
                    new { role = "user", content = request }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var aiResponse = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            aiResponse = aiResponse.Replace("```json", "").Replace("```", "").Replace("\\n", "").Trim();

            return JsonSerializer.Deserialize<AIQuery>(aiResponse); ;
        }

        public async Task<List<Dictionary<string, object>>> ExecuteSqlQueryAsync(string sql)
        {
            var result = new List<Dictionary<string, object>>();
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                await _dbContext.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader.GetValue(i);
                            row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                        }
                        result.Add(row);
                    }
                }
            }
            return result;
        }

        public async Task<ChatRespone> CallChatGPT(string request)
        {
            try
            {
                var aiQuery = await ConvertQuestionToSql(request);
                string sql = aiQuery?.query;
                string summary = aiQuery?.summary;

                if (string.IsNullOrWhiteSpace(sql) || !sql.Trim().ToLower().StartsWith("select"))
                    throw new Exception("Only SELECT queries are allowed, or AI did not generate a valid SQL.");

                var result = await ExecuteSqlQueryAsync(sql);
                return new ChatRespone { sql = sql, summary = summary, result = result };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatBotDAO][CallChatGPT] Error: {ex.Message}\n{ex.StackTrace}");
                throw new Exception($"ChatBotDAO Error: {ex.Message}");
            }
        }
    }
} 