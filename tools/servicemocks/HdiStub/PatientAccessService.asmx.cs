using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using HdiStub.HdiService;
using System.Web.Configuration;


namespace Hsd.PerotSystems.PatientAccess.Services.Hdi
{

    /// <summary>
    /// Intercept the HDI service calls
    /// </summary>
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
    public class PatientAccessService : IPatientAccessSoapBinding
    {

        #region Fields

        /// <summary>
        /// Holds the mock connection specs
        /// </summary>
        private static Dictionary<string, ConnectionSpecification> c_ConnSpecDictionary = 
            new Dictionary<string, ConnectionSpecification>();
        
        /// <summary>
        /// Real Hdi Service to which we send pass-through calls
        /// </summary>
        private HdiStub.HdiService.PatientAccessService i_RealService = 
                new HdiStub.HdiService.PatientAccessService();

        /// <summary>
        /// Should we intercept the connectionspec call or pass it to the real service?
        /// </summary>
        bool i_InterceptConnectionSpecCall = 
            Boolean.Parse( WebConfigurationManager.AppSettings["InterceptConnectionSpec"] );

        /// <summary>
        /// Should we intercept the pbaravailability call or pass it to the real service?
        /// </summary>
        bool i_InterceptPbarAvailabilityCall = 
            Boolean.Parse( WebConfigurationManager.AppSettings["InterceptPbarAvailabilityCall"] );

        #endregion

        #region Construction

        static PatientAccessService()
        {

            LoadExternalMockData();

        }//method

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Loads the external mock data.
        /// </summary>
        private static void LoadExternalMockData()
        {

            string appDataPath =
                HttpContext.Current.Request.PhysicalApplicationPath + "\\App_Data";

            XmlSerializer serializer =
                new XmlSerializer( typeof( ConnectionSpecification[] ),
                                   new XmlRootAttribute( "ArrayOfConnectionSpecification" ) );

            XmlTextReader fileReader =
                new XmlTextReader( File.OpenRead( appDataPath + "\\ConnectionSpecs.xml" ) );

            ConnectionSpecification[] connSpecArray =
                serializer.Deserialize( fileReader ) as ConnectionSpecification[];

            fileReader.Close();

            foreach( ConnectionSpecification spec in connSpecArray )
            {

                c_ConnSpecDictionary.Add( spec.hospitalCode, spec );

            }//foreach

        }//method

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="arg_0_0"></param>
        /// <returns></returns>
        /// <remarks/>
        public ConnectionSpecification getConnectionSpecFor( ConnectionSpecRequest arg_0_0 )
        {

            ConnectionSpecification returnResult = null;

            if( this.i_InterceptConnectionSpecCall )
            {

                returnResult = c_ConnSpecDictionary[arg_0_0.hospitalCode];

            }//if
            else
            {
                
                HdiStub.HdiService.ConnectionSpecRequest proxiedRequest =
                    new HdiStub.HdiService.ConnectionSpecRequest();
                HdiStub.HdiService.ConnectionSpecification proxiedResponse = 
                    null;
                
                proxiedRequest.clientGUID = arg_0_0.clientGUID;
                proxiedRequest.hospitalCode = arg_0_0.hospitalCode;
                proxiedRequest.userID = arg_0_0.userID;

                proxiedResponse = 
                    this.i_RealService.getConnectionSpecFor( proxiedRequest );

                returnResult = new ConnectionSpecification();

                returnResult.description = proxiedResponse.description;
                returnResult.hospitalCode = proxiedResponse.hospitalCode;
                returnResult.hubName = proxiedResponse.hubName;
                returnResult.ipAddress = proxiedResponse.ipAddress;                

            }//else

            return returnResult;

        }//method


        /// <summary>
        /// </summary>
        /// <param name="arg_0_1"></param>
        /// <returns></returns>
        /// <remarks/>
        public bool getPBARAvailabilityFor( PBARAvailabilityRequest arg_0_1 )
        {

            bool returnResult = false;

            if( this.i_InterceptPbarAvailabilityCall )
            {

                returnResult = Boolean.Parse( 
                    WebConfigurationManager.AppSettings["IsPbarAvailableValue"] );

            }//if
            else
            {

                HdiStub.HdiService.PBARAvailabilityRequest proxiedRequest =
                    new HdiStub.HdiService.PBARAvailabilityRequest();

                proxiedRequest.clientGUID = arg_0_1.clientGUID;
                proxiedRequest.hospitalCode = arg_0_1.hospitalCode;
                proxiedRequest.onlineUpCheck = arg_0_1.onlineUpCheck;
                proxiedRequest.userID = arg_0_1.userID;

                returnResult =  
                    this.i_RealService.getPBARAvailabilityFor( proxiedRequest );

            }//else

            return returnResult;

        }//method


        /// <summary>
        /// </summary>
        /// <param name="arg_0_2"></param>
        /// <returns></returns>
        /// <remarks/>
        public bool postRemarksFUSNoteFor( RemarksFUSNoteRequest arg_0_2 )
        {

            HdiStub.HdiService.RemarksFUSNoteRequest proxiedRequest =
                new HdiStub.HdiService.RemarksFUSNoteRequest();

            proxiedRequest.accountNumber = arg_0_2.accountNumber;
            proxiedRequest.activityCode = arg_0_2.activityCode;
            proxiedRequest.clientGUID = arg_0_2.clientGUID;
            proxiedRequest.hospitalCode = arg_0_2.hospitalCode;
            proxiedRequest.remarks = arg_0_2.remarks;
            proxiedRequest.serviceDateTime = arg_0_2.serviceDateTime;
            proxiedRequest.userID = arg_0_2.userID;

            return this.i_RealService.postRemarksFUSNoteFor( proxiedRequest );
            
        }//method


        /// <summary>
        /// </summary>
        /// <param name="arg_0_3"></param>
        /// <returns></returns>
        /// <remarks/>
        public bool postExtendedFUSNoteFor( ExtendedFUSNoteRequest arg_0_3 )
        {

            HdiStub.HdiService.ExtendedFUSNoteRequest proxiedRequest =
                new HdiStub.HdiService.ExtendedFUSNoteRequest();

            proxiedRequest.accountNumber = arg_0_3.accountNumber;
            proxiedRequest.activityCode = arg_0_3.activityCode;
            proxiedRequest.amount1 = arg_0_3.amount1;
            proxiedRequest.amount2 = arg_0_3.amount2;
            proxiedRequest.clientGUID = arg_0_3.clientGUID;
            proxiedRequest.date1 = arg_0_3.date1;
            proxiedRequest.date2 = arg_0_3.date2;
            proxiedRequest.extensionMonth = arg_0_3.extensionMonth;
            proxiedRequest.hospitalCode = arg_0_3.hospitalCode;
            proxiedRequest.serviceDateTime = arg_0_3.serviceDateTime;
            proxiedRequest.userID = arg_0_3.userID;
            proxiedRequest.workListDate = arg_0_3.workListDate;

            return this.i_RealService.postExtendedFUSNoteFor( proxiedRequest );
            
        }//method


        /// <summary>
        /// </summary>
        /// <param name="arg_0_4"></param>
        /// <returns></returns>
        /// <remarks/>
        public FUSNote1[] getFUSNotesFor( ReadFUSNotesRequest arg_0_4 )
        {

            HdiStub.HdiService.ReadFUSNotesRequest proxiedRequest =
                new HdiStub.HdiService.ReadFUSNotesRequest();
            HdiStub.HdiService.FUSNote1[] proxiedResponse = 
                null;
            List<FUSNote1> returnResult =
                new List<FUSNote1>();

            proxiedRequest.accountNumber = arg_0_4.accountNumber;
            proxiedRequest.clientGUID = arg_0_4.clientGUID;
            proxiedRequest.hospitalCode = arg_0_4.hospitalCode;
            proxiedRequest.medicalRecordNumber = arg_0_4.medicalRecordNumber;
            proxiedRequest.userID = arg_0_4.userID;

            proxiedResponse = this.i_RealService.getFUSNotesFor( proxiedRequest );

            foreach( HdiStub.HdiService.FUSNote1 note in proxiedResponse )
            {

                FUSNote1 newNote = new FUSNote1();

                newNote.activityCode = note.activityCode;
                newNote.amount1 = note.amount1;
                newNote.amount2 = note.amount2;
                newNote.date1 = note.date1;
                newNote.date2 = note.date2;
                newNote.description = note.description;
                newNote.entryDateTime = note.entryDateTime;
                newNote.extensionMonth = note.extensionMonth;
                newNote.keyedBy = note.keyedBy;
                newNote.medicalRecordNumbers = note.medicalRecordNumbers;
                newNote.noteType = note.noteType;
                newNote.remarks = note.remarks;
                newNote.workListDate = note.workListDate;

                returnResult.Add( newNote );

            }//foreach

            return returnResult.ToArray();

        }//method

        #endregion

    }//class

}//namespace
