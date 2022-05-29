using System;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for RuleComponent.
    /// </summary>
    [Serializable]
    public class RuleComponent
    {
        public RuleComponent()
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

        public Type CompositeRuleType
        {
            get
            {
                return i_CompositeRuleType;
            }
            set
            {
                i_CompositeRuleType = value;
            }
        }

        private object i_RuleContext;
        private object i_RuleControl;
        private Type i_CompositeRuleType;
    }
}
