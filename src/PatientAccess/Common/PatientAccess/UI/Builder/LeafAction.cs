using System;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;

namespace Extensions.UI.Builder
{
    [Serializable]
    public abstract class LeafAction : PersistentModel, IAction, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public abstract void Execute();

        public virtual void Configure( IView aView )
        {
            return;
        }
        #endregion

        #region Properties

        public long CompositeActionID
        {
            get
            {
                return i_CompositeActionID;
            }
            set
            {
                i_CompositeActionID = value;
            }
        }

        public object Context
        {
            get
            {
                return this.i_PrimContext;
            }
            set
            {
                this.i_PrimContext = value;
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

        public virtual int Priority
        {
            get
            {
                return PRIORITY_NORMAL;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LeafAction()
        {
        }

        public LeafAction( object context )
        {
            this.i_PrimContext = context;
        }
        #endregion

        #region Data Elements

        private object i_PrimContext;
        private string i_Description;
        private int    i_Severity;
        private bool   i_IsComposite;
        private int    i_RuleContextID;
        private long   i_CompositeActionID;

        #endregion

        #region Constants

        private const int PRIORITY_NORMAL = 50;

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            int firstCompare = 0;
            int secondCompare = 0;

            if(obj is LeafAction) 
            {
                LeafAction act = (LeafAction) obj;

                if( this.Priority == act.Priority )
                {
                    firstCompare = 0;
                }
                else if( this.Priority > act.Priority )
                {
                    firstCompare = 1;
                }
                else if( this.Priority < act.Priority )
                {
                    firstCompare = -1;
                }

                if (firstCompare != 0)
                {
                    return firstCompare;
                }

                secondCompare = this.Description.CompareTo(act.Description);

                return secondCompare;
            }
    
            throw new ArgumentException("object is not a LeafAction");                
        }

        #endregion
    }
}
