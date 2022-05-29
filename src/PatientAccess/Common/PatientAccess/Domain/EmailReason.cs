using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class EmailReason : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods

        public void SetProvided()
        {
            base.Code = PROVIDED;
            base.Description = PROVIDED_DESCRIPTION;
        }

        public void SetRequestRemove()
        {
            base.Code = REMOVE;
            base.Description = REMOVE_DESCRIPTION;
        }
        public void SetDeclined()
        {
            base.Code = DECLINED;
            base.Description = DECLINED_DESCRIPTION;
        }

        #endregion

        #region Construction and Finalization

        public EmailReason(long oid, DateTime version, string description, string code)
            : base(oid, version, description, code)
        {
        }
        public EmailReason()
        {
           
        }
        #endregion

        #region Constants

        public static readonly string BLANK = string.Empty;

        public const string PROVIDED = "P",
            REMOVE = "R",
            DECLINED = "D",
            PROVIDED_DESCRIPTION = "Email provided",
            REMOVE_DESCRIPTION =   "Request to remove email",
            DECLINED_DESCRIPTION = "Patient declined email";


        private const string EQUALS = "= ";

        #endregion

        public bool IsProvided
        {
            get { return base.Code == PROVIDED; } 

        }
        public bool IsRemoved
        {
            get { return base.Code == REMOVE; }

        }
        public bool IsDeclined
        {
            get { return base.Code == DECLINED; }

        }
    }
}
