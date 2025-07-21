namespace FoodStoreClient.Pages.Guest.Verify_Email
{
    public class GenerateToken
    {
        public string Generate_Token()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
