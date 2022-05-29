using System;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for RuleComponents.
	/// </summary>
	[Serializable]
    public class RuleComponents
	{
		public RuleComponents()
		{			
		}

        public object RuleContext
        {
            get
            {
                return i_RuleContext;
            }
            set
            {
                i_RuleContext = value;
            }
        }

        public object RuleControl
        {
            get
            {
                return i_RuleControl;
            }
            set
            {
                i_RuleControl = value;
            }
        }

        private object i_RuleContext;
        private object i_RuleControl;
	}
}
