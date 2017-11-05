namespace SimpleBlogEngine.Extensions
{
    public static class StringExtensions
    {
		public static string MakeURLFriendly(this string value)
		{
			return value.Replace(" ", "-").Replace(".", "-").ToLower();
		}
    }
}
