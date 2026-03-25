namespace ERDM.Shared.Kernel.Exceptions
{
    public class ValidationException : DomainException
    {
        public ValidationException(string property, string message)
            : base($"Validation failed for {property}: {message}")
        {
            Property = property;
        }

        public string Property { get; }
    }
}
