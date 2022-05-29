using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class DataValidationTicketType : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
		public static DataValidationTicketType GetNewPrimaryCoveragTicketType()
		{
			DataValidationTicketType type = new DataValidationTicketType();
			type.Oid = TICKKETTYPE_PRIMARY_COVERAGE;
            type.Description = "Primary Coverage";
			return type;
		}

		public static DataValidationTicketType GetNewSecondaryCoveragTicketType()
		{
			DataValidationTicketType type = new DataValidationTicketType();
			type.Oid = TICKKETTYPE_SECONDARY_COVERAGE;
            type.Description = "Secondary Coverage";
			return type;
		}

		public static DataValidationTicketType GetNewGuarantorTicketType()
		{
			DataValidationTicketType type = new DataValidationTicketType();
			type.Oid = TICKKETTYPE_GUARANTOR;
            type.Description = "Guarantor";
			return type;
		}


        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DataValidationTicketType()
        {
        }
        public DataValidationTicketType( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public DataValidationTicketType( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion

		#region Constants

		public const long 
			TICKKETTYPE_PRIMARY_COVERAGE            = 1L,
			TICKKETTYPE_SECONDARY_COVERAGE			= 2L,
			TICKKETTYPE_GUARANTOR					= 3L;

		#endregion
		
    }
}