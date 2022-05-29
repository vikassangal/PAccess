using System;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IMonthlyDueDateFeatureManager
    {
        bool IsMonthlyDueDateEnabled(Facility facility);
    }

    [Serializable]
    public class MonthlyDueDateFeatureManager : IMonthlyDueDateFeatureManager
    {
        public bool IsMonthlyDueDateEnabled(Facility facility)
        {
            return facility != null && facility.IsMonthlyDueDateEnabled;
        }
    }
}