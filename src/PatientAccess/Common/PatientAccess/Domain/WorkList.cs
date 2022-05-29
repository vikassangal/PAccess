using System;
using System.Collections;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Worklist : ReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void AddWorkListSetting( WorklistSettings aWorklistSetting)
        {
            this.i_WorklistSettings.Add( aWorklistSetting );
        }
        public static Worklist PreRegWorklist()
        {
            return new Worklist(1,"PreRegistration" );
        }
        public static Worklist PostRegWorklist()
        {
            return new Worklist(2,"PostRegistration" );
        }

        public void AddRule( Object  aRule)
        {
            this.i_Rules.Add( aRule );
        }
        public void AddAction( Object aAction)
        {
            this.i_RemainingActions.Add( aAction );
        }
        #endregion

        #region Properties
        
        public ICollection WorklistSettings 
        {
            get
            {
                return (ICollection)this.ArrayWorklistSettings.Clone();
            }
        }
           
        public ICollection Rules 
        {
            get
            {
                return (ICollection)this.ArrayRules.Clone();
            }
        }
        public ICollection Actions 
        {
            get
            {
                return (ICollection)this.ArrayRemainingActions.Clone();
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        private ArrayList ArrayWorklistSettings
        {
            get
            {
                return this.i_WorklistSettings;
            }
            set
            {
                this.i_WorklistSettings = value;
            }
        }

        private ArrayList ArrayRules
        {
            get
            {
                return this.i_Rules;
            }
            set
            {
                this.i_Rules = value;
            }
        }

        private ArrayList ArrayRemainingActions
        {
            get
            {
                return this.i_RemainingActions;
            }
            set
            {
                this.i_RemainingActions  = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public Worklist( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        private Worklist( long oid, string description )
            : base( oid, description )
        {
        }

        public Worklist()
        {
        }
        #endregion

        #region Data Elements
            private ArrayList i_RemainingActions  = new ArrayList() ;
            private ArrayList i_Rules = new ArrayList();
        private ArrayList i_WorklistSettings = new ArrayList();

        public const long
            PREREGWORKLISTID                = 1L,
            POSTREGWORKLISTID               = 2L,
            INSURANCEVERIFICATIONWORKLISTID = 3L,
            PATIENTLIABILITYWORKLISTID      = 4L,
            EMERGENCYDEPARMENTWORKLISTID    = 5L,
            NOSHOWWORKLISTID                = 6L,
            ONLINEPREREGWORKLISTID          = 7L;

        #endregion
    }
}

