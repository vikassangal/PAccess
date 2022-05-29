namespace PatientAccess
{
    public static class ServerConfigurationConstants
    {
        public const string EncryptionSectionName = "encryptionKeys";
        public const string EncryptionKeyName = "OnlinePreRegSubmissionsKeyAsBase64String";
        public const string SqlServerConnectionStringName = "ConnectionString";
        public const string NumberOfDaysAfterAdmitDateToKeepOnlineReregistrationSubmissionsKey = "NumberOfDaysAfterAdmitDateToKeepOnlineReregistrationSubmissions";
    }
}
