using PatientAccess.Persistence.Specialized;

namespace PatientAccess.Persistence.Factories
{
    /// <summary>
    /// Builds out instances of SqlBuilderStrategy subclasses
    /// </summary>
    public static class SqlBuilderStrategyFactory
    {
        public static PatientInsertStrategy CreatePatientInsertStrategy()
        {
            // TODO: Move this out into in IoC mechanism or make configurable
            return new ClinicalTrialsPatientInsertStrategy();
        }
    }
}
