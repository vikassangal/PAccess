using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using Extensions.Exceptions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.HDI;
using log4net;

namespace PatientAccess.Services.Hdi
{
    /// <summary>
    /// This class is badly named. It is actually a broker but for historical reasons
    /// it's name sounds like it is a service. 
    /// </summary>
    [Serializable]
    public class HDIService : MarshalByRefObject, IHDIService
    {

		#region Constants 

        private const string CIEGUID = "CIEGUID";
        private const string PATIENT_ACCESS_USER_ID = "PACCESS";
        private const string PATIENTACCESSSERVICEENGINE = "PatientAccessServiceEngine";

		#endregion Constants 

		#region Fields 

        private static readonly ILog c_Log = LogManager.GetLogger( typeof( HDIService ) );
        private string i_HdiGuid;
        private IPatientAccessService i_ServiceEngine;

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="HDIService"/> class.
        /// </summary>
        /// <param name="patientAccessService">The patient access service.</param>
        internal HDIService( IPatientAccessService patientAccessService )
        {
            this.ServiceEngine = patientAccessService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HDIService"/> class.
        /// </summary>
        public HDIService()
        {
            // read the GUID
            this.HdiGuid = ConfigurationManager.AppSettings[CIEGUID];
            this.ServiceEngine = new PatientAccessService( ConfigurationManager.AppSettings[PATIENTACCESSSERVICEENGINE] );
        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets the service engine.
        /// </summary>
        /// <value>The service engine.</value>
        private IPatientAccessService ServiceEngine
        {
            get
            {
                return this.i_ServiceEngine;
            }
            set
            {
                this.i_ServiceEngine = value;
            }
        }

        /// <summary>
        /// Gets or sets the HDI GUID.
        /// </summary>
        /// <value>The HDI GUID.</value>
        private string HdiGuid
        {
            get
            {
                return this.i_HdiGuid;
            }
            set
            {
                this.i_HdiGuid = value;
            }
        }

        /// <summary>
        /// Gets or sets the patient access service.
        /// </summary>
        /// <value>The patient access service.</value>
        internal IPatientAccessService PatientAccessService
        {
            get
            {
                return this.ServiceEngine;
            }
            set
            {
                this.ServiceEngine = value;
            }
        }

		#endregion Properties 

		#region Methods

        public ICollection GetFUSNotesFor( PatientAccess.Domain.Account anAccount )
        {

            if( anAccount == null || anAccount.AccountNumber == 0 )
            {
                return new ArrayList();
            }

            var readRequest = this.GetReadRequestFrom( anAccount );

            return this.GetFusNotesFor( readRequest );
        }

        private ICollection GetFusNotesFor( ReadFUSNotesRequest readRequest ) 
        {
            FUSNote[] notes = this.ServiceEngine.getFUSNotesFor( readRequest );
            ArrayList fusNotes = new ArrayList();
            IFUSNoteBroker fusBroker = BrokerFactory.BrokerOfType<IFUSNoteBroker>();

            if( notes != null && notes.Length > 0 )
            {
                foreach( FUSNote1 note in notes )
                {
                    FusActivity fusActivity = fusBroker.FusActivityWith( note.activityCode );

                    DateTime entryDate = note.entryDateTime == null
                                             ? DateTime.MinValue
                                             : Convert.ToDateTime( note.entryDateTime );
                    DateTime worklistDate = note.workListDate == null
                                                ? DateTime.MinValue
                                                : Convert.ToDateTime( note.workListDate );

                    DateTime date1 = note.date1 == null ? DateTime.MinValue : Convert.ToDateTime( note.date1 );
                    DateTime date2 = note.date2 == null ? DateTime.MinValue : Convert.ToDateTime( note.date2 );

                    if( entryDate != DateTime.MinValue )
                    {
                        entryDate = entryDate.ToLocalTime();
                    }

                    decimal amount1 = 0;
                    decimal amount2 = 0;

                    if( note.amount1 != null )
                    {
                        amount1 = Convert.ToDecimal( note.amount1 );
                    }

                    if( note.amount2 != null )
                    {
                        amount2 = Convert.ToDecimal( note.amount2 );
                    }

                    ExtendedFUSNote fusNote = new ExtendedFUSNote( fusActivity, entryDate, note.keyedBy, note.remarks,
                                                       note.description, worklistDate, amount1, amount2, date1, date2,
                                                       note.extensionMonth );


                    fusNote.Persisted = true;

                    fusNotes.Add( fusNote );
                }
            }

            return fusNotes;
        }

        private ReadFUSNotesRequest GetReadRequestFrom( Domain.Account anAccount ) 
        {
            var readRequest = new ReadFUSNotesRequest();

            readRequest.hospitalCode = anAccount.Facility.Code;
            readRequest.medicalRecordNumber = anAccount.Patient.MedicalRecordNumber.ToString();
            readRequest.accountNumber = anAccount.AccountNumber.ToString();
            readRequest.clientGUID = this.HdiGuid;
            readRequest.userID = PATIENT_ACCESS_USER_ID;
            
            return readRequest;
        }

        /// <summary>
        /// Posts the extended remarks FUS note.
        /// </summary>
        /// <param name="fusNote">The FUS note.</param>
        /// <exception cref="HDIServiceException"><c>HDIServiceException</c>.</exception>
        public void PostExtendedRemarksFusNote( ExtendedFUSNote fusNote )
        {
            PostFUSNoteRequest remarksRequest = null;

            try
            {
                remarksRequest = new PostFUSNoteRequest();

                remarksRequest.hospitalCode = fusNote.Account.Facility.Code;
                remarksRequest.clientGUID = this.HdiGuid;
                remarksRequest.userID = fusNote.UserID;
                remarksRequest.accountNumber = fusNote.AccountNumber;
                remarksRequest.activityCode = fusNote.FusActivity.Code;
                remarksRequest.remarks = fusNote.Remarks;
                remarksRequest.date1 = fusNote.Date1;
                remarksRequest.date2 = fusNote.Date2;
                remarksRequest.amount1 = Convert.ToDecimal( fusNote.Dollar1 );
                remarksRequest.amount2 = Convert.ToDecimal( fusNote.Dollar2 );

                if( !string.IsNullOrEmpty( fusNote.Month ) )
                {
                    remarksRequest.extensionMonth = fusNote.Month.ToUpper();
                }

                remarksRequest.workListDate = fusNote.WorklistDate;
                remarksRequest.serviceDateTime = fusNote.CreatedOn.ToUniversalTime();

                this.ServiceEngine.postFUSNoteFor( remarksRequest );
            }
            catch( Exception ex )
            {
                c_Log.Error( string.Format( "FUS Note failed to post : {0} \n\nRequest: {1}", ex.Message,
                                            this.CreateXmlFromRequest( remarksRequest ) ) );

                throw new HDIServiceException( ex.ToString(), null, Severity.Catastrophic );
            }
            finally
            {
                this.ServiceEngine.Dispose();
            }
        }

        /// <summary>
        /// Creates the XML from request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string CreateXmlFromRequest( object request )
        {
            string returnString;

            if( null != request )
            {
                try
                {
                    var stringWriter = new StringWriter();
                    var serializer = new XmlSerializer( request.GetType() );

                    serializer.Serialize( stringWriter, request );
                    returnString = stringWriter.ToString();

                    stringWriter.Dispose();
                } //try
                catch
                {
                    returnString = string.Empty;
                } //catch;
            } //if
            else
            {
                returnString = string.Empty;
            } //else

            return returnString;
        }

		#endregion Methods 

    }
}