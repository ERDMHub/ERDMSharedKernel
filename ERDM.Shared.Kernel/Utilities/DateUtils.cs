namespace ERDM.Shared.Kernel.Utilities
{
    public static class DateUtils
    {
        public static DateTime ToUtc(DateTime date)
        {
            return date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
        }

        public static int GetAge(DateTime dob)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
