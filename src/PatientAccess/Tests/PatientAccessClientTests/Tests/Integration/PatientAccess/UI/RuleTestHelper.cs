using System.Linq;
using Extensions.UI.Builder;
using PatientAccess.Actions;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Integration.PatientAccess.UI
{
    public static class RuleTestHelper
    {
        public static bool IsRuleEnforcedForCompositeAction<T>( this Account account, int ruleId ) where T : CompositeAction
        {
            var ruleEngine = RuleEngine.GetInstance();
            ruleEngine.LoadRules( account );

            ruleEngine.EvaluateAllRules( account );
            var compositeActions = ruleEngine.GetCompositeItemsCollection();
            var compositeActionForPatientDemographics = compositeActions.First( x => x.GetType() == typeof( T ) );

            ruleEngine.EvaluateAllRules( account );

            var ruleIsEnforced = compositeActionForPatientDemographics.Constituents.OfType<GenericAction>().Where( x => x.Oid == ruleId ).Count() == 1;

            return ruleIsEnforced;
        }
    }
}
