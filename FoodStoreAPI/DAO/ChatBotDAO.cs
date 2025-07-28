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
            var orders = FoodStoreAPI.DAO.OrderDAO.GetAllOrderDetail();
            var products = FoodStoreAPI.DAO.ProductDAO.getAllProduct();
            var categories = FoodStoreAPI.DAO.CategoryDAO.getCategory();

            var schemaBuilder = new StringBuilder();
            schemaBuilder.AppendLine("Database schema:");
            schemaBuilder.AppendLine("Tables:");
            schemaBuilder.AppendLine("Account(AccId, Email, Phone, Name, Username, Password, Wallet, CreateAt, UpdateAt, RoleId, Address)");
            schemaBuilder.AppendLine("AccountShipped(AccId, TransId, AddressShop, AddressCustomer, CreateAt, UpdateAt, Status)");
            schemaBuilder.AppendLine("Category(CateId, CateName)");
            schemaBuilder.AppendLine("Messagess(MessId, FromUserId, ToUserId, MessageText, SentTime)");
            schemaBuilder.AppendLine("Orders(OrderId, CustomerID, Gtotal, create_at, update_at, Status, OrderDate)");
            schemaBuilder.AppendLine("OrderItems(OrderItemId, OrderID, ProductID, Quantity, create_at, update_at)");
            schemaBuilder.AppendLine("Product(ProId, Name, Price, OriginalPrice, Unit, Images, create_at, update_at, Quantity, ProductStatus, AccId, CateId, DiscountStartTime, DiscountEndTime, DiscountPercentage)");
            schemaBuilder.AppendLine("ProductReviews(ReviewID, images, create_at, update_at, star, ProID, AccID)");
            schemaBuilder.AppendLine("Revenue(revenueId, revenuePrice, interestRate, FloorFee, create_At, AccID)");
            schemaBuilder.AppendLine("Role(RoleId, RoleName)");
            schemaBuilder.AppendLine("Token(TokenId, create_at, tokenString, ExpirationDate, Status, AccID)");
            schemaBuilder.AppendLine("Transactions(TransId, AccID, ProID, TransDate, Amount, Status)");

            schemaBuilder.AppendLine();
            schemaBuilder.AppendLine("Relationships:");
            schemaBuilder.AppendLine("- Account.RoleId → Role.RoleId");
            schemaBuilder.AppendLine("- AccountShipped.AccId → Account.AccId");
            schemaBuilder.AppendLine("- AccountShipped.TransId → Transactions.TransId");
            schemaBuilder.AppendLine("- Messagess.FromUserId → Account.AccId");
            schemaBuilder.AppendLine("- Messagess.ToUserId → Account.AccId");
            schemaBuilder.AppendLine("- Orders.CustomerId → Account.AccId");
            schemaBuilder.AppendLine("- OrderItems.OrderID → Orders.OrderID");
            schemaBuilder.AppendLine("- OrderItems.ProductID → Product.ProID");
            schemaBuilder.AppendLine("- Product.AccID → Account.AccID");
            schemaBuilder.AppendLine("- Product.CateID → Category.CateID");
            schemaBuilder.AppendLine("- ProductReviews.ProID → Product.ProID");
            schemaBuilder.AppendLine("- ProductReviews.AccID → Account.AccID");
            schemaBuilder.AppendLine("- Revenue.AccID → Account.AccID");
            schemaBuilder.AppendLine("- Token.AccID → Account.AccID");
            schemaBuilder.AppendLine("- Transactions.AccID → Account.AccID");
            schemaBuilder.AppendLine("- Transactions.ProID → Product.ProID");

            var ordersJson = JsonSerializer.Serialize(orders);
            var productsJson = JsonSerializer.Serialize(products);
            var categoriesJson = JsonSerializer.Serialize(categories);

            var builder = new StringBuilder();
            builder.AppendLine("You are a helpful, cheerful database and data analysis assistant.");
            builder.AppendLine("Your role is to help users generate SQL queries *and* analyze data insights from the provided database schema and actual content.");
            builder.AppendLine("You are also a market analyst and business consultant. If the user's request goes beyond simple SQL and requires predictions, trends, or combo analysis, you must analyze it based on table data structure and actual data.");
            builder.AppendLine("Use the following database schema to understand the structure of the system:");
            builder.AppendLine(schemaBuilder.ToString());
            builder.AppendLine("Here is a sample of the actual data in the system (in JSON):");
            builder.AppendLine("Order data: " + ordersJson);
            builder.AppendLine("Product data: " + productsJson);
            builder.AppendLine("Categorie data: " + categoriesJson);
            builder.AppendLine("- If the user's question requires a SQL query, generate it.");
            builder.AppendLine("- If the user's question is analytical (e.g., predicting low stock, recommending combos), provide a logical explanation and an appropriate query if needed.");
            builder.AppendLine("- Always include ALL relevant columns in the query.");
            builder.AppendLine("- You must ONLY use Microsoft SQL Server syntax.");
            builder.AppendLine("- DO NOT use MySQL-specific syntax such as `LIMIT`, backticks (`), `AUTO_INCREMENT`, or `NOW()`. These are not supported in SQL Server.");
            builder.AppendLine("- Always use `TOP n` instead of `LIMIT n`.");
            builder.AppendLine("- Use square brackets [] for table and column names if needed, not backticks (`).");
            builder.AppendLine("- Date functions must follow SQL Server conventions (e.g., `GETDATE()` instead of `NOW()`).");
            builder.AppendLine("- Ensure queries run properly on Microsoft SQL Server (version 2019+).");
            builder.AppendLine("- If you are unsure whether syntax is valid for SQL Server, leave it out.");
            builder.AppendLine("- Include column headers in your result sets.");
            builder.AppendLine("- Always respond with a single-line JSON formatted response like this:");
            builder.AppendLine(@"{ ""summary"": ""your-summary"", ""query"": ""your-query"" }");
            builder.AppendLine("- Do NOT use line breaks inside the JSON output.");
            builder.AppendLine("- Please return the JSON as a single line. Do not include newline characters or backslashes unless they are properly escaped (e.g., use \\\\ if needed).");
            builder.AppendLine("- The SQL query must be written in one line.");
            builder.AppendLine("- Replace `your-summary` with your thought process: explain how you reasoned from schema to query and/or data.");
            builder.AppendLine("- Replace `your-query` with your SQL Server query if applicable. If no SQL is needed, explain that in the summary.");
            builder.AppendLine("- If the user asks for predictions or suggestions, your summary must include detailed reasoning, metrics, or patterns to support your answer.");

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

            return JsonSerializer.Deserialize<AIQuery>(aiResponse); 
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
                            row[reader.GetName(i)] = value == DBNull.Value ? "null" : value;
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
                    throw new Exception("Invalid SQL.");

                var result = await ExecuteSqlQueryAsync(sql);
                return new ChatRespone { sql = sql, summary = summary, result = result };
            }
            catch (Exception ex)
            {
                throw new Exception($"ChatBotDAO Error: {ex.Message}");
            }
        }
    }
} 