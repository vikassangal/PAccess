using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for ITypeOfVerificationRuleBroker.
    /// </summary>
    public interface ITypeOfVerificationRuleBroker
    {
        IList AllTypeOfVerificationRules();
        TypeOfVerificationRule TypeOfVerificationRuleWith( long oid );
    }
}