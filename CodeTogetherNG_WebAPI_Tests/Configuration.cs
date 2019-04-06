using NUnit.Framework;

namespace CodeTogetherNG_WebAPI_Tests
{
    internal static class Configuration
    {
        public static string WebApiUrl
        {
            get
            {
                var url= TestContext.Parameters["webAppUrl"];
                if (string.IsNullOrWhiteSpace(url))
                    return "https://localhost:44362/";
                return url;
            }
        }

        public static string ConnectionString
        {
            get
            {
                var connectionString = TestContext.Parameters["connectionString"];
                if (string.IsNullOrWhiteSpace(connectionString))
                    return "Server=DESKTOP-67FEEF1\\SQLEXPRESS;Database=CodeTogetherNG; Trusted_Connection=True";
                return connectionString;
            }
        }
    }
}