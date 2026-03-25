namespace ERDM.Shared.Kernel.Constants
{
    //before using this key name append with its dbname
    //ex: "__Astra__ClientId"=> "CreditDb__Astra__ClientId" like this
    public static class AstraKeyNames
    {
        
        public const string ClientId = "__Astra__ClientId";
        public const string ClientSecret = "__Astra__ClientSecret";
        public const string ClientToken = "__Astra__ClientToken";
        public const string SecureBundleBase64 = "__Astra__SecureBundleBase64";
        public const string Keyspace = "__Astra__Keyspace";
        public const string Prefix = "__Astra__";
        public const string Datacenter = "__Astra__Datacenter";

        // Consistency Levels
        public const string ConsistencyLocalQuorum = "LocalQuorum";
        public const string ConsistencyQuorum = "Quorum";
        public const string ConsistencyLocalOne = "LocalOne";
        public const string ConsistencyOne = "One";

        // Default Values
        public const string DefaultKeyspace = "credit_management";
        public const string DefaultDatacenter = "datacenter1";
        public const int DefaultPort = 9042;
        public const int DefaultConnectTimeout = 30000;
        public const int DefaultReadTimeout = 120000;

        // Table Names
        public const string UsersTable = "users";
        public const string CreditApplicationsTable = "credit_applications";
        public const string CreditScoresTable = "credit_scores";
        public const string RiskAssessmentsTable = "risk_assessments";
        public const string PaymentsTable = "payments";

        // Column Names
        public const string IdColumn = "id";
        public const string IsActiveColumn = "isactive";
        public const string CreatedOnColumn = "createdon";
        public const string CreatedByColumn = "createdby";
        public const string ModifiedOnColumn = "modifiedon";
        public const string ModifiedByColumn = "modifiedby";
    }
}
