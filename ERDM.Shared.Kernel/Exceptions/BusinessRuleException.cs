namespace ERDM.Shared.Kernel.Exceptions
{
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string rule, string message)
            : base($"Business rule '{rule}' violated: {message}")
        {
            Rule = rule;
        }

        public string Rule { get; }
    }
}
