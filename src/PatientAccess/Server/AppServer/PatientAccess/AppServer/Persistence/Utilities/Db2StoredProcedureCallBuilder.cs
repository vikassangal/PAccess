using System.Collections.Generic;
using System.Linq;
using PatientAccess.Utilities;

namespace PatientAccess.Persistence.Utilities
{
    public class Db2StoredProcedureCallBuilder
    {
        private readonly IEnumerable<string> storedProcedureParams;
        private readonly string storedProcedureName;

        public Db2StoredProcedureCallBuilder(IEnumerable<string> storedProcParams, string storedProcName)
        {
            Guard.ThrowIfArgumentIsNull(storedProcParams, "StoredProcParams");
            Guard.ThrowIfArgumentIsNullOrEmpty(storedProcName, "StoredProcName");
            storedProcedureParams = storedProcParams;
            storedProcedureName = storedProcName;
        }

        public string Build()
        {
            var CommaSeparatedListOfArguments = string.Join(",", storedProcedureParams.ToArray());
            return string.Format("{0} {1}({2})", "CALL", storedProcedureName, CommaSeparatedListOfArguments);
        }
    }
}