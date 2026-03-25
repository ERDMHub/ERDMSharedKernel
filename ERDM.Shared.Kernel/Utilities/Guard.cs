namespace ERDM.Shared.Kernel.Utilities
{
    public static class Guard
    {
        public static void AgainstNull(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        public static void AgainstNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{parameterName} cannot be null or empty", parameterName);
        }

        public static void AgainstNullOrEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            if (value == null || !value.Any())
                throw new ArgumentException($"{parameterName} cannot be null or empty", parameterName);
        }

        public static void AgainstNegative(long value, string parameterName)
        {
            if (value < 0)
                throw new ArgumentException($"{parameterName} cannot be negative", parameterName);
        }

        public static void AgainstNegative(decimal value, string parameterName)
        {
            if (value < 0)
                throw new ArgumentException($"{parameterName} cannot be negative", parameterName);
        }

        public static void AgainstOutOfRange(decimal value, decimal min, decimal max, string parameterName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, $"Value must be between {min} and {max}");
        }

        public static void AgainstInvalidDate(DateTime date, DateTime min, DateTime max, string parameterName)
        {
            if (date < min || date > max)
                throw new ArgumentOutOfRangeException(parameterName, $"Date must be between {min:d} and {max:d}");
        }

        public static void AgainstInvalidEmail(string email, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", parameterName);

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                    throw new ArgumentException("Invalid email format", parameterName);
            }
            catch
            {
                throw new ArgumentException("Invalid email format", parameterName);
            }
        }
    }
}