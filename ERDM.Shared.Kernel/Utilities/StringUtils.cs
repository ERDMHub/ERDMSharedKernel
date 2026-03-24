
namespace ERDM.Shared.Kernel.Utilities
{
    public static class StringUtils
    {
        public static bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string ToTitleCase(string input)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(input.ToLower());
        }
    }
}
