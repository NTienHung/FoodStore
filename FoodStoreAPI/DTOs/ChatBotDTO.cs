using FoodStoreAPI.Models;

namespace FoodStoreAPI.DTOs
{
    public class ChatBotDTO
    {
        public class ChatBotRequest
        {
            public string request { get; set; }
        }

        public class AIQuery
        {
            public string summary { get; set; }
            public string query { get; set; }
        }

        public class AIConnection
        {
            public string ConnectionString { get; set; }
            public string Name { get; set; }
            public List<TableSchema> SchemaStructured { get; set; }
            public List<string> SchemaRaw { get; set; }
        }

        public class ChatRespone
        {
            public string sql { get; set; }
            public string summary { get; set; }
            public List<Dictionary<string, object>> result { get; set; }
        }
    }
}
