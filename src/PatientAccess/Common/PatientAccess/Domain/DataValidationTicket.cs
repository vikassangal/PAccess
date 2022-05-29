using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
	[Serializable]
	public class DataValidationTicket : PersistentModel
	{
		#region Event Handlers
		#endregion

		#region Methods

		#endregion

		#region Properties

        public string ResponseText
        {
            get
            {
                return i_ResponseText;
            }
            set
            {
                i_ResponseText = value;
            }
        }
		/// <summary>
		/// TicketId - the GUID returned from the DataValidation web service call
		/// </summary>
		public string TicketId
		{
			get
			{
				return i_TicketId;
			}
			set
			{
				i_TicketId = value;
			}
		}

		/// <summary>
		/// AccountNumber - patient's account number
		/// </summary>
		public long AccountNumber
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
       
		/// <summary>
		/// MedicalRecordNumber - patient's MRN
		/// </summary>
		public long MedicalRecordNumber 
		{
			get
			{
				return i_MedicalRecordNumber;
			}
			set
			{
				i_MedicalRecordNumber = value;
			}
		}

		/// <summary>
		/// Facility - current facility
		/// </summary>
		public Facility Facility
		{
			get
			{
				return i_Facility;
			}
			set
			{
				i_Facility = value;
			}
		}

		/// <summary>
		/// The date and time at which the Data Validation was intiated
		/// </summary>
		public DateTime   InitiatedOn
		{
			get
			{
				return i_InitiatedOn;
			}
			set
			{
				i_InitiatedOn = value;
			}
		}

		/// <summary>
		/// ResultsAvailable - indicator that DataValidation has a response from an iniated data validation
		/// request 
		/// </summary>
		public bool ResultsAvailable
		{
			get
			{
				return i_ResultsAvailable;
			}
			set
			{
				i_ResultsAvailable = value;
			}
		}
   
        /// <summary>
        /// ResultsReviewed - indicator that DataValidation response has been reviewed
        /// request 
        /// </summary>
        public bool ResultsReviewed
        {
            get
            {
                return i_ResultsReviewed;
            }
            set
            {
                i_ResultsReviewed = value;
            }
        }

        /// <summary>
        /// FUSNoteSent - indicator that we have call the DV method to send a FUS note 
        /// </summary>
        public bool FUSNoteSent
        {
            get
            {
                return i_FUSNoteSent;
            }
            set
            {
                i_FUSNoteSent = value;
            }
        }

		public DataValidationTicketType TicketType
		{
			get
			{
				return i_TicketType;
			}
			set
			{
				i_TicketType = value;
			}
		}

        public DataValidationBenefitsResponse BenefitsResponse
        {
            get
            {
                return i_DataValidationBenefitsResponse;
            }
            set
            {
                i_DataValidationBenefitsResponse = value;
            }
        }
	
		#endregion

		#region Private Methods
        
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization

		/// <summary>
		/// DataValidationTicket - null constructor
		/// </summary>
		public DataValidationTicket()
		{
		}

		/// <summary>
		/// DataValidationTicket - construct an instance with account info
		/// </summary>
		/// <param name="anAccount"></param>
		public DataValidationTicket(Account anAccount)
		{
			this.AccountNumber          = anAccount.AccountNumber;
			this.MedicalRecordNumber    = anAccount.Patient.MedicalRecordNumber;
			this.Facility               = anAccount.Facility;
		}

        /// <summary>
        /// DataValidationTicket - construct an instance with account info
        /// </summary>
        /// <param name="anAccount"></param>
        public DataValidationTicket( long  accountNumber, long medicalRecordNumber, Facility facility  )
        {
            this.AccountNumber          = accountNumber;
            this.MedicalRecordNumber    = medicalRecordNumber;
            this.Facility               = facility;
        }

		/// <summary>
		/// DataValidationTicket - construnct an instance with account info
		/// and Ticket info
		/// </summary>
		/// <param name="anAccount"></param>
		/// <param name="aTicketId"></param>
		public DataValidationTicket(Account anAccount, string aTicketId)
		{
			if( anAccount.AccountNumber > 0 )
			{
				this.AccountNumber          = anAccount.AccountNumber;
			}
            
			this.MedicalRecordNumber    = anAccount.Patient.MedicalRecordNumber;
			this.Facility               = anAccount.Facility;

			this.TicketId               = aTicketId;

			if( this.InitiatedOn == DateTime.MinValue )
			{
				this.InitiatedOn    = DateTime.Now;
			}
		}

		/// <summary>
		/// DataValidationTicket - construct an instance with ticket info
		/// </summary>
		/// <param name="aTicketId"></param>
		public DataValidationTicket(string aTicketId)
		{
			this.TicketId       = aTicketId;
			this.InitiatedOn    = DateTime.Now;
		}

		#endregion

		#region Data Elements

		private string                          i_TicketId              = string.Empty;
		private long                            i_AccountNumber         = 0L;
		private long                            i_MedicalRecordNumber   = 0L;
		private Facility                        i_Facility              = null;
		private DateTime                        i_InitiatedOn;
		private bool                            i_ResultsAvailable      = false;
        private bool                            i_ResultsReviewed       = false;
        private bool                            i_FUSNoteSent           = false;
		private DataValidationTicketType        i_TicketType            = null;
        private DataValidationBenefitsResponse  i_DataValidationBenefitsResponse = new DataValidationBenefitsResponse( );
        private string                          i_ResponseText = string.Empty;
		
        #endregion

		#region Constants


		#endregion
	}
}

