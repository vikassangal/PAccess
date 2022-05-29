using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class TypeOfAccident : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new TypeOfAccident of type AUTO
        /// </summary>
        /// <returns></returns>
        public static TypeOfAccident NewAuto()
        {
            return new TypeOfAccident( AUTO, "Auto" );
        }
        /// <summary>
        ///  Creates a new TypeOfAccident of type AUTO_NO_FAULT_INSURANCE
        /// </summary>
        /// <returns></returns>
        public static TypeOfAccident NewAutoNoFaultInsurance()
        {
            return new TypeOfAccident( AUTO_NO_FAULT_INSURANCE, "AutoNoFaultInsurance" );
        }
        /// <summary>
        ///  Creates a new TypeOfAccident of type EMPLOYMENT_RELATED
        /// </summary>
        /// <returns></returns>
        public static TypeOfAccident NewEmploymentRelated()
        {
            return new TypeOfAccident( EMPLOYMENT_RELATED, "Employment Related" );
        }
        /// <summary>
        /// Creates a new TypeOfAccident of type TORT_LIABILITY
        /// </summary>
        /// <returns></returns>
        public static TypeOfAccident NewTortLiability()
        {
            return new TypeOfAccident( TORT_LIABILITY, "Tort Liability" );
        }
        /// <summary>
        /// Creates a new TypeOfAccident of type OTHER
        /// </summary>
        /// <returns></returns>
        public static TypeOfAccident NewOther()
        {
            return new TypeOfAccident( OTHER, "Other" );
        }

        /// <summary>
        /// Creates a new TypeOfAccident of type NON_AUTO
        /// </summary>
        /// <returns></returns>
        public static TypeOfAccident NewNonAuto()
        {
            return new TypeOfAccident( NON_AUTO, "Non-automobile" );
        }
        
        #endregion

        #region Properties
        /// <summary>
        /// OccurrenceCode for TypeOfAccident 
        /// </summary>
        public OccurrenceCode OccurrenceCode
        {
            get
            {
                return this.i_OccurrenceCode;
            }
            set
            {
                this.i_OccurrenceCode = value;
            }
        }
        #endregion

        #region Private Methods
       
        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TypeOfAccident(long oid,string description)
            : base( oid, description )
        {
        }
        public TypeOfAccident(long oid,string description , OccurrenceCode code)
            : base( oid, description )
        {
            this.OccurrenceCode = code;
        }
        
        public TypeOfAccident()
        {
            Code        = String.Empty;
            Description = String.Empty;
        }

        public TypeOfAccident( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public TypeOfAccident( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        private OccurrenceCode i_OccurrenceCode = new OccurrenceCode();
        /// <summary>
        /// Values for AccidentTypes
        /// </summary>
        public const long 
            AUTO                    = 1,
            AUTO_NO_FAULT_INSURANCE = 2,            
            TORT_LIABILITY          = 3,
            EMPLOYMENT_RELATED      = 4,
            OTHER                   = 5,
            NON_AUTO                = 6;

        #endregion
    }
}
