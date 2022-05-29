using System.Collections;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for RuleBroker.
	/// </summary>
	public interface IRuleBroker
	{
		RuleHashtable	RuleActionMapping();
        RuleHashtable	RuleWorklistMapping();
        RuleHashtable	ActionWorklistMapping( string kindOfVisitCode, string financialClassCode );
        RuleArrayList	LoadRules( string aType );
	    RuleHashtable   AllRulesById();
        RuleHashtable	AllActions();
	    RuleHashtable   AllRules();
		Hashtable		GetRulesForCaching();

	}
}
