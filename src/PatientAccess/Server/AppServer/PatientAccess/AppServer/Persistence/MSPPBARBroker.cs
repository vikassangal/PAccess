using System;
using System.Configuration;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for MSPPBARBroker.
	/// This broker loads preexisting answers for a MSP Questionare. 
	/// 
	/// NOTE: THIS BROKER IS NOT EXPOSED to the remoting layer. It is never called except
	///         when loading an account.
	/// </summary>
    [Serializable]
    public class MSPPBARBroker : AbstractPBARBroker, IMSPBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        public MedicareSecondaryPayor MSPFor(IAccount anAccount)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            MedicareSecondaryPayor msp = null;

            try
            { 
                cmd = this.CommandFor("CALL " + SPMSPFORACCOUNT + 
                    "(" + PARAM_HSP + 
                    "," + PARAM_MRN + 
                    "," + PARAM_ACCTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSP].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCTNUMBER].Value = anAccount.AccountNumber;

                reader = this.ExecuteReader(cmd);

                msp = this.MSPFrom(anAccount, reader);
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }

            return (msp);
        }

        private MedicareSecondaryPayor MSPFrom(IAccount anAccount, SafeReader reader)
        {
            MedicareSecondaryPayor msp = null;
            while(reader.Read())
            {
                int mspVersion = reader.GetInt32( COL_MSPVERSION );
                
                if( mspVersion > 0 )
                {
                    msp = new MedicareSecondaryPayor();
                    msp.MSPVersion = mspVersion;
                }
                else
                {
                    return msp;
                }                

                // read special programs data
                msp.SpecialProgram.BlackLungBenefits = 
                    new YesNoFlag(reader.GetString(COL_BLACKLUNG));
                msp.SpecialProgram.BLBenefitsStartDate = reader.GetDateTime(COL_BLACKLUNGDATE);
                msp.SpecialProgram.GovernmentProgram =
                    new YesNoFlag(reader.GetString(COL_GOVTPGM));
                msp.SpecialProgram.DVAAuthorized =
                    new YesNoFlag(reader.GetString(COL_DVA));
                msp.SpecialProgram.WorkRelated =
                    new YesNoFlag(reader.GetString(COL_WORKRELATED));
                msp.SpecialProgram.VisitForBlackLung =
                    new YesNoFlag(reader.GetString(COL_TODAYSVISITBLACKLUNG));

                // Read liability data
                msp.LiabilityInsurer.NonWorkRelated =
                    new YesNoFlag(reader.GetString(COL_NONWORKRELATED));
                msp.LiabilityInsurer.AccidentDate = reader.GetDateTime(COL_ACCIDENTINJURYDATE);
                msp.LiabilityInsurer.AnotherPartyResponsibility = 
                    new YesNoFlag(reader.GetString(COL_OTHERPARTY));

                string accidentType = reader.GetString(COL_AUTOACCIDENT);
                switch( accidentType )
                {
                    case "Y":
                        msp.LiabilityInsurer.AccidentType = TypeOfAccident.NewAuto();
                        break;
                    case "N":
                        msp.LiabilityInsurer.AccidentType = TypeOfAccident.NewNonAuto();
                        break;
                    default:
                        msp.LiabilityInsurer.AccidentType = TypeOfAccident.NewOther();
                        break;
                }

                if( anAccount.Patient.Employment != null )
                {
                    IEmploymentStatusBroker employmentStatusBroker = BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
                    if( anAccount.Patient.Employment.Status == null )
                    {
                        anAccount.Patient.Employment.Status = employmentStatusBroker.EmploymentStatusWith( anAccount.Facility.Oid, " " );
                    }
                }

                string hasESRD = reader.GetString(COL_ESRD);
                if(hasESRD.Equals("Y"))
                {
                    msp.MedicareEntitlement = this.ReadESRDData(anAccount, reader);
                }

                string ageEntitlement = reader.GetString(COL_AGE);
                if( ageEntitlement.Equals("Y") )
                {
                    msp.MedicareEntitlement = this.ReadAgeEntitlement(anAccount, reader);
                }

                string DisabilityEntitlement = reader.GetString(COL_DISABILITY);
                if( DisabilityEntitlement.Equals("Y") )
                {
                    msp.MedicareEntitlement = this.ReadDisabilityEntitlement(anAccount, reader);
                }
                msp.HasBeenCompleted = true;

                string noFaultAvailable         = reader.GetString(COL_NOFAULTAVAILABLE);
                if( noFaultAvailable != string.Empty )
                {
                    msp.LiabilityInsurer.NoFaultInsuranceAvailable.Code = noFaultAvailable;
                }
                else
                {
                    msp.LiabilityInsurer.NoFaultInsuranceAvailable.Code = YesNoFlag.CODE_BLANK;
                }

                string liabilityAvailable         = reader.GetString(COL_LIABILITYAVAILABLE);
                if( liabilityAvailable != string.Empty )
                {
                    msp.LiabilityInsurer.LiabilityInsuranceAvailable.Code = liabilityAvailable;
                }
                else
                {
                    msp.LiabilityInsurer.LiabilityInsuranceAvailable.Code = YesNoFlag.CODE_BLANK;
                }            
            }
            return msp;
        }

        public DateTime GetMSP2StartDate()
        {
            string strMSP2Start = ConfigurationManager.AppSettings["MSP2_START_DATE"];
            DateTime msp2StartDate = DateTime.Parse( strMSP2Start );

            return msp2StartDate;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private ESRDEntitlement ReadESRDData(IAccount anAccount, SafeReader reader)
        {
            int mspVersion                  = reader.GetInt32(COL_MSPVERSION);
            
            //MedicareSecondaryPayor msp = anAccount.
            ESRDEntitlement esrd = new ESRDEntitlement();
            esrd.TransplantDate = reader.GetDateTime(COL_TRANSPLANTDATE);
            esrd.DialysisDate = reader.GetDateTime(COL_DIALYSISDATE);
            esrd.DialysisCenterName = reader.GetString(COL_DIALYSISCENTERNAME);
            esrd.DialysisTrainingStartDate = reader.GetDateTime(COL_SELFTRAININGDATE);
            esrd.WithinCoordinationPeriod = 
                new YesNoFlag(reader.GetString(COL_ESRD30MOCOORD));
            esrd.GroupHealthPlanCoverage =
                new YesNoFlag(reader.GetString(COL_ESRDGHP));
            esrd.KidneyTransplant = 
                new YesNoFlag(reader.GetString(COL_ESRDKIDNEYTRANSPLANT));
            esrd.DialysisTreatment = 
                new YesNoFlag(reader.GetString(COL_ESRDDIALYSISTRTMNT));
            
            // Q6 
            esrd.BasedOnESRD =
                new YesNoFlag(reader.GetString(COL_ESRDINITIALENTITLE));

            if( mspVersion == 1 )
            {
                // Q5 answer
                esrd.ESRDandAgeOrDisability = 
                    new YesNoFlag(reader.GetString(COL_ESRDMULTIENTITLE));

                // Q7 answer
                esrd.BasedOnAgeOrDisability = 
                    new YesNoFlag(reader.GetString(COL_ESRDNOTAGEDIS));
            }
            else
            {
                // Q5 answer
                esrd.BasedOnAgeOrDisability = 
                    new YesNoFlag(reader.GetString(COL_ESRDMULTIENTITLE));

                // Q7 answer
                esrd.ProvisionAppliesFlag =
                    new YesNoFlag(reader.GetString(COL_ESRDNOTAGEDIS));
            }

            return esrd;

        }

        private AgeEntitlement ReadAgeEntitlement(IAccount anAccount, SafeReader reader)
        {
            AgeEntitlement ageEnt = new AgeEntitlement();
            ageEnt.GroupHealthPlanCoverage =
                new YesNoFlag(reader.GetString(COL_AGEGHP));
            ageEnt.GHPLimitExceeded = 
                new YesNoFlag(reader.GetString(COL_AGEGHP20));
            ageEnt.GHPEmploysX = ageEnt.GHPLimitExceeded;
            ageEnt.SpouseEmployment = this.ReadSpouseEmployment(reader,anAccount.Facility.Oid);

            if( anAccount.Patient.Employment != null )
            {
                ageEnt.PatientEmployment = ( Employment )anAccount.Patient.Employment.DeepCopy();
                ageEnt.PatientEmployment.Status.Code = this.ReadMSPPatientEmploymentStatus( reader );
                ageEnt.PatientEmployment.RetiredDate = reader.GetDateTime( COL_RETIREDATE );

                anAccount.Patient.Employment.RetiredDate = reader.GetDateTime( COL_RETIREDATE );
            }
            else
            {
                ageEnt.PatientEmployment = null;
            }

            string ageGHPType         = reader.GetString(COL_AGEGHPTYPE);
            if( ageGHPType != string.Empty )
            {
                if( ageGHPType == GroupHealthPlanType.SelfCode )
                {
                    ageEnt.GroupHealthPlanType  = GroupHealthPlanType.NewSelfType();
                }
                else if( ageGHPType == GroupHealthPlanType.SpouseCode )
                {
                    ageEnt.GroupHealthPlanType  = GroupHealthPlanType.NewSpouseType();
                }
                else if( ageGHPType == GroupHealthPlanType.BothCode )
                {
                    ageEnt.GroupHealthPlanType  = GroupHealthPlanType.NewBothType();
                }
                else 
                {
                    ageEnt.GroupHealthPlanType  = new GroupHealthPlanType();
                }                
            }
            else
            {
                ageEnt.GroupHealthPlanType  = new GroupHealthPlanType();
            } 

            string ageSpouseOverX         = reader.GetString(COL_AGESPOUSEOVERX);
            if( ageSpouseOverX != string.Empty )
            {
                ageEnt.GHPSpouseEmploysX.Code = ageSpouseOverX;
            }
            else
            {
                ageEnt.GHPSpouseEmploysX.Code = YesNoFlag.CODE_BLANK;
            }             

            return ageEnt;
        }

        private DisabilityEntitlement ReadDisabilityEntitlement(IAccount anAccount, SafeReader reader)
        {
            DisabilityEntitlement disEnt = new DisabilityEntitlement();
            disEnt.GHPLimitExceeded =
                new YesNoFlag(reader.GetString(COL_DISGHP100));
            disEnt.GHPEmploysMoreThanXFlag = disEnt.GHPLimitExceeded;
            disEnt.GroupHealthPlanCoverage = 
                new YesNoFlag(reader.GetString(COL_DISGHP));
            disEnt.SpouseEmployment = this.ReadSpouseEmployment(reader, anAccount.Facility.Oid);
            disEnt.FamilyMemberGHPFlag = new YesNoFlag(reader.GetString(COL_OTHEREMPLOYED));
            disEnt.FamilyMemberEmployment = this.ReadFamilyMemberEmployment(reader, anAccount.Facility.Oid);
         
            if( anAccount.Patient.Employment != null )
            {
                disEnt.PatientEmployment = ( Employment )anAccount.Patient.Employment.DeepCopy();
                disEnt.PatientEmployment.Status.Code = this.ReadMSPPatientEmploymentStatus( reader );
                disEnt.PatientEmployment.RetiredDate = reader.GetDateTime( COL_RETIREDATE );

                anAccount.Patient.Employment.RetiredDate = reader.GetDateTime( COL_RETIREDATE );
            }
            else
            {
                disEnt.PatientEmployment = null;
            }

            string disGHPType         = reader.GetString(COL_DISGHPTYPE);
            if( disGHPType != string.Empty )
            {                
                if( disGHPType == GroupHealthPlanType.SelfCode )
                {
                    disEnt.GroupHealthPlanType  = GroupHealthPlanType.NewSelfType();
                }
                else if( disGHPType == GroupHealthPlanType.SpouseCode )
                {
                    disEnt.GroupHealthPlanType  = GroupHealthPlanType.NewSpouseType();
                }
                else if( disGHPType == GroupHealthPlanType.BothCode )
                {
                    disEnt.GroupHealthPlanType  = GroupHealthPlanType.NewBothType();
                }
                else 
                {
                    disEnt.GroupHealthPlanType  = new GroupHealthPlanType();
                }                
            }
            else
            {
                disEnt.GroupHealthPlanType  = new GroupHealthPlanType();
            } 

            string disSpouseOverX         = reader.GetString(COL_DISSPOUSEOVERX);
            if( disSpouseOverX != string.Empty )
            {
                disEnt.SpouseGHPEmploysMoreThanXFlag.Code = disSpouseOverX;
            }
            else
            {
                disEnt.SpouseGHPEmploysMoreThanXFlag.Code = YesNoFlag.CODE_BLANK;
            }

            string disFamiliyMemberOverX         = reader.GetString(COL_DISFAMILYMEMBEROVERX);
            if( disFamiliyMemberOverX != string.Empty )
            {
                disEnt.FamilyMemberGHPEmploysMoreThanXFlag.Code = disFamiliyMemberOverX;
            }
            else
            {
                disEnt.FamilyMemberGHPEmploysMoreThanXFlag.Code = YesNoFlag.CODE_BLANK;
            }

            return disEnt;
        }

        private string ReadMSPPatientEmploymentStatus(SafeReader reader)
        {
            string statusFlag = String.Empty;
            string employmentStatusCode = reader.GetString(COL_EMPLOYED);
            string neverEmployedCode = reader.GetString(COL_PATIENTNEVEREMPLOYED);
            DateTime retiredDate = reader.GetDateTime(COL_RETIREDATE);

            if( employmentStatusCode != null && employmentStatusCode != String.Empty )
            {
                if( employmentStatusCode == YES_FLAG )
                {
                    statusFlag = EmploymentStatus.EMPLOYED_FULL_TIME_CODE;
                }
                else if( employmentStatusCode == NO_FLAG &&
                    retiredDate != DateTime.MinValue &&
                    neverEmployedCode == NO_FLAG )
                {
                    statusFlag = EmploymentStatus.RETIRED_CODE;
                }
                else if( employmentStatusCode == NO_FLAG &&
                    neverEmployedCode == YES_FLAG )
                {
                    statusFlag = EmploymentStatus.NOT_EMPLOYED_CODE;
                }
                else
                {
                    statusFlag = EmploymentStatus.OTHER_CODE;
                }
            }

            return statusFlag;
        }

        private Employment ReadSpouseEmployment(SafeReader reader, long facilityID )
        {
            Employment employment = new Employment();
            
            string spouseEmploymentStatusCode = reader.GetString(COL_SPOUSEEMPLOYED);
            string spouseNeverEmployed = reader.GetString(COL_SPOUSENEVEREMPLOYED);

            employment.RetiredDate = reader.GetDateTime(COL_SPOUSERETIREDATE);
            
            if( spouseEmploymentStatusCode != null && spouseEmploymentStatusCode != String.Empty )
            {
                if( spouseEmploymentStatusCode == YES_FLAG )
                {
                    employment.Status.Code = EmploymentStatus.EMPLOYED_FULL_TIME_CODE;
                }
                else if( spouseEmploymentStatusCode == NO_FLAG &&
                    employment.RetiredDate != DateTime.MinValue &&
                    spouseNeverEmployed == NO_FLAG )
                {
                    employment.Status.Code = EmploymentStatus.RETIRED_CODE;
                }
                else if( spouseEmploymentStatusCode == NO_FLAG &&
                    spouseNeverEmployed == YES_FLAG )
                {
                    employment.Status.Code = EmploymentStatus.NOT_EMPLOYED_CODE;
                }
                else
                {
                    employment.Status.Code = EmploymentStatus.OTHER_CODE;
                }
            }

            string empName =
                StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                reader.GetString( COL_SPOUSEEMPLOYERNAME ) );
            empName = Employer.ModifyPBAREmployerName( empName );

            string street = reader.GetString(COL_SPOUSEEMPLOYERADDRESS).Trim();
            street = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( street );
            
            string city = reader.GetString( COL_SPOUSEEMPLOYERCITY ).Trim();
            city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( city );

            string state = reader.GetString( COL_SPOUSEEMPLOYERSTATE ).Trim();
            string zip = reader.GetString(COL_SPOUSEEMPLOYERZIP).Trim();
            string zip4 = reader.GetString(COL_SPOUSEEMPLOYERZIP4).Trim();
            string zipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( zip + zip4 );

            if ( !string.IsNullOrEmpty( empName ) &&
                 !string.IsNullOrEmpty( street ) )
            {
                employment.Employer = new Employer();
                employment.Employer.Name = empName;
                IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
                State stateObj = addressBroker.StateWith( facilityID,state );
                Address address = new Address( street, null, city, new ZipCode( zipCode ),
                    stateObj,null );
                employment.Employer.PartyContactPoint = new ContactPoint(address,null,null,
                    TypeOfContactPoint.NewEmployerContactPointType());
            }

            return employment;
        }

        private Employment ReadFamilyMemberEmployment(SafeReader reader, long facilityID)
        {
            Employment employment = null;

            string empName =
                StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                reader.GetString( COL_FAMILYMBREMPLOYERNAME ) );
            empName = Employer.ModifyPBAREmployerName( empName );
            
            string street = reader.GetString(COL_FAMILYMBREMPLOYERADDRESS).Trim();
            street = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( street );

            string city = reader.GetString( COL_FAMILYMBREMPLOYERCITY ).Trim();
            city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( city );

            string state = reader.GetString( COL_FAMILYMBREMPLOYERSTATE ).Trim();
            string zip = reader.GetString(COL_FAMILYMBREMPLOYERZIP).Trim();
            string zip4 = reader.GetString(COL_FAMILYMBREMPLOYERZIP4).Trim();
            string zipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( zip + zip4 );

            if ( !string.IsNullOrEmpty( empName ) &&
                 !string.IsNullOrEmpty( street ) )
            {
                employment = new Employment();
                employment.Employer = new Employer();
                employment.Employer.Name = empName;
                IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
                State stateObj = addressBroker.StateWith(facilityID ,state );
                Address address = new Address( street, null, city, new ZipCode( zipCode ),
                    stateObj, null);
                employment.Employer.PartyContactPoint = new ContactPoint(address, null, null,
                    TypeOfContactPoint.NewEmployerContactPointType());
            }

            return employment;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MSPPBARBroker()
            : base()
        {
        }
        public MSPPBARBroker(string cxnString)
            : base(cxnString)
        {
        }
        public MSPPBARBroker(IDbTransaction txn )
            : base(txn)
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( MSPPBARBroker ) );
        #endregion

        #region Constants
        private const string  SPMSPFORACCOUNT   = "SELECTMSPFORACCOUNT";
 
        private const string
            PARAM_HSP                           = "@P_HSP", 
            PARAM_MRN                           = "@P_MRN", 
            PARAM_ACCTNUMBER                    = "@P_ACCOUNTNUMBER";

	    private const string
	        COL_BLACKLUNG = "BLACKLUNG",
	        COL_GOVTPGM = "GOVTPGM",
	        COL_DVA = "DVA",
	        COL_WORKRELATED = "WORKRELATED",
	        COL_NONWORKRELATED = "NONWORKRELATED",
	        COL_OTHERPARTY = "OTHERPARTY",
	        COL_AGE = "AGE",
	        COL_EMPLOYED = "EMPLOYED",
	        COL_SPOUSEEMPLOYED = "SPOUSEEMPLOYED",
	        COL_AGEGHP = "AGEGHP",
	        COL_AGEGHP20 = "AGEGHP20",
	        COL_DISABILITY = "DISABILITY",
	        COL_DISGHP = "DISGHP",
	        COL_DISGHP100 = "DISGHP100",
	        COL_ESRD = "ESRD",
	        COL_ESRDGHP = "ESRDGHP",
	        COL_ESRDKIDNEYTRANSPLANT = "ESRDKIDNEYTRANSPLANT",
	        COL_ESRDDIALYSISTRTMNT = "ESRDDIALYSISTRTMNT",
	        COL_ESRD30MOCOORD = "ESRD30MOCOORD",
	        COL_ESRDMULTIENTITLE = "ESRDMULTIENTITLE",
	        COL_ESRDINITIALENTITLE = "ESRDINITIALENTITLE",
	        COL_ESRDNOTAGEDIS = "ESRDNOTAGEDIS",
	        COL_AUTOACCIDENT = "AUTOACCIDENT",
	        COL_BLACKLUNGDATE = "BLACKLUNGDATE",
	        COL_ACCIDENTINJURYDATE = "ACCIDENTINJURYDATE",
	        COL_TRANSPLANTDATE = "TRANSPLANTDATE",
	        COL_DIALYSISDATE = "DIALYSISDATE",
            COL_DIALYSISCENTERNAME = "DIALYSISCENTERNAME",
	        COL_SELFTRAININGDATE = "SELFTRAININGDATE",
	        COL_RETIREDATE = "RETIREDATE",
	        COL_SPOUSERETIREDATE = "SPOUSERETIREDATE",
	        COL_SPOUSEEMPLOYERNAME = "SPOUSEEMPLOYERNAME",
	        COL_SPOUSEEMPLOYERADDRESS = "SPOUSEEMPLOYERADDRESS",
	        COL_SPOUSEEMPLOYERCITY = "SPOUSEEMPLOYERCITY",
	        COL_SPOUSEEMPLOYERSTATE = "SPOUSEEMPLOYERSTATE",
	        COL_SPOUSEEMPLOYERZIP = "SPOUSEEMPLOYERZIP",
	        COL_SPOUSEEMPLOYERZIP4 = "SPOUSEEMPLOYERZIP4",
	        COL_FAMILYMBREMPLOYERNAME = "FAMILYMBREMPLOYERNAME",
	        COL_FAMILYMBREMPLOYERADDRESS = "FAMILYMBREMPLOYERADDRESS",
	        COL_FAMILYMBREMPLOYERCITY = "FAMILYMBREMPLOYERCITY",
	        COL_FAMILYMBREMPLOYERSTATE = "FAMILYMBREMPLOYERSTATE",
	        COL_FAMILYMBREMPLOYERZIP = "FAMILYMBREMPLOYERZIP",
	        COL_FAMILYMBREMPLOYERZIP4 = "FAMILYMBREMPLOYERZIP4",
	        COL_PATIENTNEVEREMPLOYED = "PATIENTNEVEREMPLOYED",
	        COL_SPOUSENEVEREMPLOYED = "SPOUSENEVEREMPLOYED",
	        COL_TODAYSVISITBLACKLUNG = "TODAYSVISITBLACKLUNG",
	        COL_OTHEREMPLOYED        = "OTHEREMPLOYED";

        // new columns for MSP2

        private const string
            COL_NOFAULTAVAILABLE                = "NOFAULTAVAILABLE",
            COL_LIABILITYAVAILABLE              = "LIABILITYAVAILABLE",
            COL_AGEGHPTYPE                      = "AGEGHPTYPE",
            COL_AGESPOUSEOVERX                  = "AGESPOUSEOVERX",
            COL_DISGHPTYPE                      = "DISGHPTYPE",
            COL_DISSPOUSEOVERX                  = "DISSPOUSEOVERX",
            COL_DISFAMILYMEMBEROVERX            = "DISFAMILYMEMBEROVERX",
            COL_MSPVERSION                      = "MSPVERSION";

        private const string YES_FLAG           = "Y";
        private const string NO_FLAG            = "N";
        #endregion
    }
}
