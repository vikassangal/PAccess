using System;
using System.Collections;
using Extensions.UI.Builder;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for RuleGroup.
	/// </summary>
	[Serializable]
    public class RuleGroup
	{   

        public RuleGroup()
		{
		}

        public void AddRule(LeafRule aRule)
        {
            this.i_Rules.Add(aRule);
        }

        public void AddRules(ArrayList rules)
        {
            this.i_Rules = rules;
        }

//        private int          i_RuleGroupId;
//        private string       i_Name;
        private ArrayList    i_Rules = new ArrayList();
	}
}
