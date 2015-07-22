namespace ElasticsearchCRUD.Utils
{
    public static class StringExtensions
    {
        public static string ToLower(this string str, bool toLower)
        {
            return toLower ? str.ToLower() : str;
        }
    }
}
