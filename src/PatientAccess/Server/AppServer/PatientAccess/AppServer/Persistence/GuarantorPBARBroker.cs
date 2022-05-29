using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;
using log4net;
using PatientAccess.Persistence.Utilities;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements Demographics related methods.
    /// </summary>
    [Serializable]
    public class GuarantorPBARBroker : AbstractPBARBroker, IGuarantorBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public Guarantor GuarantorFor( Account anAccount )
        {
            iDB2Command cmd                                 = null;
            SafeReader reader                               = null;
            Guarantor aGuarantor                            = new Guarantor();

            IDemographicsBroker demographicsBroker      = BrokerFactory.BrokerOfType<IDemographicsBroker>();
            IAddressBroker addressBroker                = BrokerFactory.BrokerOfType<IAddressBroker>();
            IEmployerBroker employerBroker              = BrokerFactory.BrokerOfType<IEmployerBroker>();
            IDataValidationBroker dataValidationBroker  = BrokerFactory.BrokerOfType<IDataValidationBroker>();
            IRelationshipTypeBroker relationshipTypeBroker = 
                BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();

            long oID                        = 0;
            string sexCode                  = string.Empty;
            string sexDescription           = string.Empty;
            string SSN                      = string.Empty;
            string driversLicenseNo         = string.Empty;
            string driversLicenseState      = string.Empty;
            string emailAddress             = string.Empty;
            string lastName                 = string.Empty;
            string firstName                = string.Empty;
            string middleInitial            = string.Empty;
            string suffix                    = string.Empty;            
            string relationshipCode         = string.Empty;
            string relationshipDescription  = string.Empty;
            string dateOfBirth              = string.Empty;

            aGuarantor.Oid                  = oID;

            try
            {
                cmd = this.CommandFor( "CALL " + SP_GUARANTOR_FOR_ACCOUNT +
                    "(" + PARAM_FACILITY_ID + 
                    "," + PARAM_ACCOUNT_NUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility);

                cmd.Parameters[PARAM_FACILITY_ID].Value      = anAccount.Facility.Oid;                
                cmd.Parameters[PARAM_ACCOUNT_NUMBER].Value  = anAccount.AccountNumber;
                
                reader = this.ExecuteReader( cmd );

                while( reader.Read() )
                {
                    sexCode                 = reader.GetString( COL_SEXCODE );
                    sexDescription          = reader.GetString( COL_SEXDESCRIPTION );
                    SSN                     = reader.GetString( COL_SSN );
                    driversLicenseNo        = reader.GetString( COL_DRIVERSLICENSENO );
                    driversLicenseState     = reader.GetString( COL_DRIVERSLICENSESTATE );

                    emailAddress = StringFilter.RemoveAllNonEmailSpecialCharacters( reader.GetString( COL_EMAILADDRESS ) );

                    lastName                = reader.GetString( COL_LASTNAME );
                    lastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( lastName );

                    firstName = reader.GetString( COL_FIRSTNAME );
                    firstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( firstName );

                    middleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    middleInitial = StringFilter.RemoveFirstCharNonLetter( middleInitial );

                    suffix = reader.GetString(COL_SUFFIX);
                    suffix = StringFilter.RemoveFirstCharNonLetterAndRestNonLetter( suffix );

                    relationshipCode = reader.GetString( COL_RELATIONSHIPCODE );
                    relationshipDescription = reader.GetString( COL_RELATIONSHIPDESCRIPTION );

                    dateOfBirth = reader.GetString(COL_DATEOFBIRTH).TrimEnd();
                    if (dateOfBirth.Length == 8 && dateOfBirth != "00000000")
                    {
                        string mm = dateOfBirth.Substring(4, 2);
                        string dd = dateOfBirth.Substring(6, 2);
                        string yyyy = dateOfBirth.Substring(0, 4);

                        if (DateTimeUtilities.IsValidDateTime(mm, dd, yyyy))
                        {
                            DateTime DOB = new DateTime(Convert.ToInt16(yyyy),
                                Convert.ToInt16(mm),
                                Convert.ToInt16(dd));

                            aGuarantor.DateOfBirth = DOB ;
                        }
                    }

                    aGuarantor.DriversLicense.State     = addressBroker.StateWith(anAccount.Facility.Oid, driversLicenseState );
                    aGuarantor.DriversLicense.Number    = driversLicenseNo;

                    middleInitial = StringFilter.StripMiddleInitialFromFirstName( ref firstName );
                    
                    aGuarantor.FirstName                = firstName;
                    aGuarantor.LastName                 = lastName;
                    aGuarantor.Name.MiddleInitial       = middleInitial;
                    aGuarantor.Name.Suffix = suffix;

                    // TODO  [GOO Project] Convert to RelationshipTypePBARBroker
                    RelationshipType aRelationshipType  = relationshipTypeBroker.RelationshipTypeWith( anAccount.Facility.Oid,relationshipCode );
                    anAccount.GuarantorIs( aGuarantor, aRelationshipType );

                    aGuarantor.Sex                      = demographicsBroker.GenderWith( anAccount.Facility.Oid, sexCode );
                    aGuarantor.SocialSecurityNumber     = new SocialSecurityNumber( SSN );

          
                    // TODO [GOO Project] Convert to employerBroker.EmployerFor( guarantor, anAccount.AccountNumber ) to EmployerPBARBroker call
                    aGuarantor = employerBroker.EmployerFor( aGuarantor, anAccount.AccountNumber, anAccount.Facility.Oid );

                    if( aGuarantor != null )
                    {
                        ArrayList contactPoints = addressBroker.ContactPointsForGuarantor( anAccount.Facility.Code, anAccount.AccountNumber );
                        anAccount.Guarantor.AddContactPoints( contactPoints );
                    }

                    var mobileContactPoint =
                        anAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
                    if ( mobileContactPoint != null)
                    {
                        anAccount.OldGuarantorCellPhoneConsent = mobileContactPoint.CellPhoneConsent;
                    }

                    if( aGuarantor != null )
                    {
                        aGuarantor.DataValidationTicket = dataValidationBroker.GetDataValidationTicketFor( anAccount, DataValidationTicketType.GetNewGuarantorTicketType() );
                    } 
                }
            }               
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception in GuarantorBroker; attempting to get Guarantor for Facility: " + 
                    anAccount.Facility.Oid + " and Account: " + anAccount.AccountNumber, ex, c_log );
            }
            finally
            {
                base.Close( reader );
                base.Close( cmd );
            }

            return aGuarantor;
        }

        public void GuarantorEmployerDataFor(Account anAccount)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = this.CommandFor("CALL " + SP_SELECTACCTGAREMPDATA +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility);

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = this.ExecuteReader(cmd);

                anAccount.Guarantor.Employment = this.EmploymentFrom( anAccount.Facility.Oid, reader );
            }
            catch
            {
                throw;
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// construct employment from reader
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Employment EmploymentFrom(long facilityID, SafeReader reader)
        {

            Employment employment = new Employment();

            if (reader.Read())
            {

                string employerName =
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                    reader.GetString( COL_EMPLOYERSNAME ) );
                employerName = Employer.ModifyPBAREmployerName( employerName );

                string employerAddress = reader.GetString(COL_EMPLOYERADDRESS).Trim();
                employerAddress = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( employerAddress );

                string employerCity = reader.GetString( COL_EMPLOYECITY ).Trim();
                employerCity = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( employerCity );

                string employerState = reader.GetString( COL_GUARANTOREMPLOYERSTATE ).Trim();
                string employerZip = reader.GetString(COL_EMPLOYERZIP).Trim();
                string employerZip4 = reader.GetString(COL_EMPLOYERZIP4).Trim();
                string zipCode = StringFilter.
                    RemoveAllNonLetterNumberSpaceAndHyphen( employerZip + employerZip4 );

                string employerCountry = reader.GetString( COL_EMPLOYERCOUNTRY ).Trim();
                long areaCode = reader.GetInt64(COL_EMPLOYERAREACODE);
                long phone = reader.GetInt64(COL_EMPLOYERPHONENUMBER);
                string employmentStatus = reader.GetString(COL_GUARANTOREMPLOYMENTSTATUS).Trim();
                string industry = reader.GetString(COL_GUARANTORSOCCUPATION).Trim();

                string strAreaCode = areaCode.ToString();
                strAreaCode = strAreaCode.PadLeft(3, '0');

                string strPhone = phone.ToString();
                strPhone = strPhone.PadLeft(7, '0');

                employment.Status = employmentStatusBroker.EmploymentStatusWith(facilityID, employmentStatus);
                State state = addressBroker.StateWith(facilityID,employerState);
                Country country = addressBroker.CountryWith( facilityID , employerCountry );
                Address address = new Address(
                    employerAddress,
                    null,
                    employerCity,
                    new ZipCode( zipCode ),
                    state,
                    country);

                PhoneNumber phoneNumber = new PhoneNumber(strAreaCode, strPhone);

                EmployerContactPoint employerContactPoint = new EmployerContactPoint(
                    address,
                    phoneNumber,
                    null,
                    TypeOfContactPoint.NewBusinessContactPointType());

                Employer employer = new Employer(
                    ReferenceValue.NEW_OID,
                    ReferenceValue.NEW_VERSION,
                    employerName,
                    null,
                    0);
                employer.AddContactPoint(employerContactPoint);
                employer.PartyContactPoint = employerContactPoint;
                employer.Industry = industry;

                employment.Employer = employer;
                employment.Occupation = industry;
            }
            else
            {
                // if there was no employer found when reading from the database
                // then this guarantor must be unemployed. 
                employment.Status =
                    employmentStatusBroker.EmploymentStatusWith(facilityID, EmploymentStatus.NOT_EMPLOYED_CODE);
            }
            return employment;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public GuarantorPBARBroker()
            : base()
        {
        }
        public GuarantorPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public GuarantorPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( GuarantorPBARBroker ) );
        private IEmploymentStatusBroker employmentStatusBroker =
            BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
        private IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
        #endregion

        #region Constants

        private const string
            SP_GUARANTOR_FOR_ACCOUNT        = "SELECTGUARANTORFORACCOUNT",
            SP_SELECTACCTGAREMPDATA         = "SELECTACCTGAREMPDATA";

        private const string
            PARAM_ACCOUNT_NUMBER            = "@P_ACCOUNT_NUMBER",
            PARAM_FACILITY_ID               = "@P_FACILITY_ID",
            PARAM_HSPNUMBER                 = "@P_HSP",
            PARAM_MRN                       = "@P_MRN",
            PARAM_ACCOUNTNUMBER             = "@P_ACCOUNTNUMBER";

        private const string
            COL_SEXCODE = "SEXCODE",
            COL_SEXDESCRIPTION = "SEXDESCRIPTION",
            COL_SSN = "SSN",
            COL_DRIVERSLICENSENO = "DRIVERSLICENSENO",
            COL_DRIVERSLICENSESTATE = "DRIVERSLICENSESTATE",
            COL_EMAILADDRESS = "EMAILADDRESS",
            COL_LASTNAME = "LASTNAME",
            COL_FIRSTNAME = "FIRSTNAME",
            COL_MIDDLEINITIAL = "MIDDLEINITIAL",
            COL_SUFFIX = "SUFFIX",
            COL_RELATIONSHIPCODE = "RELATIONSHIPCODE",
            COL_RELATIONSHIPDESCRIPTION = "RELATIONSHIPDESCRIPTION",
            COL_DATEOFBIRTH = "DateOfBirth";

        private const string
            COL_EMPLOYERSNAME = "EMPLOYERSNAME",
            COL_EMPLOYERADDRESS = "EMPLOYERADDRESS",
            COL_EMPLOYERAREACODE = "EMPLOYERAREACODE",
            COL_EMPLOYERPHONENUMBER = "EMPLOYERPHONENUMBER",
            COL_GUARANTORSOCCUPATION = "GUARANTORSOCCUPATION",
            COL_EMPLOYECITY = "EMPLOYECITY",
            COL_GUARANTOREMPLOYERSTATE = "GUARANTOREMPLOYERSTATE",
            COL_GUARANTOREMPLOYMENTSTATUS = "GUARANTOREMPLOYMENTSTATUS",
            COL_EMPLOYERZIP = "EMPLOYERZIP",
            COL_EMPLOYERZIP4 = "EMPLOYERZIP4",
            COL_EMPLOYERCOUNTRY = "EMPLOYERCOUNTRY";

        #endregion
    }
}
