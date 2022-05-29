using System;
using System.Collections;
using System.Reflection;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.Auditing.FusNotes
{
    //TODO: Create XML summary comment for FUSNote
    [Serializable]
    public class FusNote : PersistentModel
    {

        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return this.Context.ToString();

        }
        public  string AsRecord()
        {   
            return string.Empty ;

        }
        public  IList CreateFUSNoteMessages( Account anAccount )
        {   
            IList messages = null;
            Assembly a = Assembly.Load( DOMAINLIBRNAME );
            string strategyName = FusNoteFomattersNamespace + "." + this.FusActivity.StrategyName;
            Type t = a.GetType( strategyName );
            if( t != null )
            {
                FusFormatterStrategy strategy = Activator.CreateInstance( t, null ) as FusFormatterStrategy;
                strategy.Context = this;
                messages = strategy.Format();
            }

            return messages;
        }
        #endregion

        #region Properties

        public bool IsExtensionNote
        {
            get
            {
                return this.FusActivity.Code == EXTENSION_ACTIVITY_CODE;
            }
        }

        public FusActivity FusActivity
        {
            get
            {
                return i_FUSActivity;
            }
            set
            {
                i_FUSActivity = value;
            }
        }
        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }
        public string AccountNumber
        {
            get
            {
                return i_AccountNumber;
            }
            set
            {
                i_AccountNumber = value;
            }
        }
        public DateTime CreatedOn
        {
            get
            {
                return i_CreatedOn;
            }
            set
            {
                i_CreatedOn = value;
            }
        }
        public object Context
        {
            get
            {
                return i_Context;
            }
            set
            {
                i_Context = value;
            }
        }
        public object Context2
        {
            get
            {
                return i_Context2;
            }
            set
            {
                i_Context2 = value;
            }
        }
        public DateTime WorklistDate
        {
            get
            {
                return i_WorklistDate;
            }
            set
            {
                i_WorklistDate = value;
            }
        }
        public string UserID
        {
            get
            {
                return i_UserID;
            }
            set
            {
                i_UserID = value;
            }
        }
        public string Remarks
        {
            get
            {
                return i_Remarks;
            }
            set
            {
                i_Remarks = value;
            }
        }

        public string Text
        {
            get
            {
                return i_Text;
            }
            set
            {
                i_Text = value;
            }
        }

        public bool Persisted
        {
            get
            {
                return i_Persisted;
            }
            set
            {
                i_Persisted = value;
            }
        }

        public bool ManuallyEntered
        {
            get
            {
                return i_ManuallyEntered;
            }
            set
            {
                i_ManuallyEntered = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FusNote()
        {
        }
        public FusNote( FusActivity fusActivity, DateTime entryDateTime, string userID, string remarks, string text,
            DateTime worklistDate )
        {
            this.FusActivity    = fusActivity;
            this.CreatedOn      = entryDateTime;
            this.UserID         = userID;
            this.Remarks        = remarks;
            this.Text           = text;
            this.WorklistDate   = worklistDate;
        }
        public FusNote( object context1, object context2, FusActivity fusActivity )
        {
            Context     = context1;
            Context2    = context2;
            FusActivity = fusActivity;
        }
        #endregion

        #region Data Elements
        private Account     i_Account =  new Account();
        private string      i_AccountNumber = string.Empty ;
        private DateTime    i_CreatedOn = DateTime.Now ;
        private object      i_Context = new object();
        private object      i_Context2 = new object();
        private FusActivity i_FUSActivity;
        private bool        i_Persisted = false;
        private bool        i_ManuallyEntered = false;
        private string      i_Remarks = string.Empty;
        private string      i_Text = string.Empty;
        private string      i_UserID = string.Empty;
        private DateTime    i_WorklistDate;
       
        #endregion

        #region Constants

        private const string DOMAINLIBRNAME = "PatientAccess.Common";
        private const string EXTENSION_ACTIVITY_CODE = "CREMC";
        private const string FusNoteFomattersNamespace = "PatientAccess.Domain.Auditing.FusNotes.Formatters";

        #endregion
    }
}
