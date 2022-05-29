using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace Extensions.UI.Builder
{
    [Serializable]
    public abstract class LeafRule : PersistentModel, IRule
    {
        #region Event Handlers
        #endregion

        #region Methods

        public abstract bool CanBeAppliedTo( object context );

        public abstract bool RegisterHandler( EventHandler eventHandler );

        public abstract bool UnregisterHandler( EventHandler eventHandler );

        public abstract void UnregisterHandlers( );

        #endregion

        #region Properties
        public int Severity
        {
            get
            {
                return i_Severity;
            }
            set
            {
                i_Severity = value;
            }
        }

        public long CompositeRuleID
        {
            get
            {
                return i_CompositeRuleID;
            }
            set
            {
                i_CompositeRuleID = value;
            }
        }

        public bool IsComposite
        {
            get
            {
                return i_IsComposite;
            }
            set
            {
                i_IsComposite = value;
            }
        }

        public ArrayList ContextItems
        {
            get
            {
                return i_ContextItems;
            }
            set
            {
                i_ContextItems = value;
            }
        }

        public int RuleContextID
        {
            get
            {
                return i_RuleContextID;
            }
            set
            {
                i_RuleContextID = value;
            }
        }

        public object AssociatedControl
        {
            get
            {
                return i_AssociatedControl;
            }
            set
            {
                i_AssociatedControl = value;
            }
        }

        public string Description
        {
            get
            {
                return i_Description;
            }
            set
            {
                i_Description = value;
            }
        }

        public bool FireEvents
        {
            get
            {
                return i_FireEvents;
            }
            set
            {
                i_FireEvents = value;
            }
        }

        public bool GenerateActions
        {
            get
            {
                return i_GenerateActions;
            }
            set
            {
                i_GenerateActions = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LeafRule()
        {
        }
        #endregion

        #region Data Elements

        private int       i_Severity;
        private bool      i_IsComposite;
        private object    i_AssociatedControl;
        private ArrayList i_ContextItems = new ArrayList();
        private int       i_RuleContextID;
        private string    i_Description;
        private long      i_CompositeRuleID;
        private bool      i_FireEvents;
        private bool      i_GenerateActions;

        #endregion

        #region Constants
        #endregion

        public virtual void ApplyTo(object context)
        {
            
        }

        public virtual bool ShouldStopProcessing()
        {
            return false;
        }
    }
}

