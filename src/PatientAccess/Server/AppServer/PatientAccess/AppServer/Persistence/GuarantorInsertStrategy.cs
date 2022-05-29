using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class GuarantorInsertStrategy : SqlBuilderStrategy
    {
        #region Construction and Finalization
        public GuarantorInsertStrategy()
        {
            InitializeColumnValues();
        }
        #endregion

        #region Methods

        public override void UpdateColumnValuesUsing( Account account )
        {
            i_Account = account;

            if( i_Account != null )
            {
                //Set Transaction APACFL Add/Change flag
                if( account.IsNew )
                {
                    this.AddChangeFlag = ADD_FLAG;
                }
                else
                {
                    this.AddChangeFlag = CHANGE_FLAG;
                }

                if( account.Activity.GetType() == typeof( PreDischargeActivity ) )
                {
                    this.AddChangeFlag = CHANGE_FLAG;
                }


                ITimeBroker tBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
                DateTime facilityDateTime = tBroker.TimeAt( account.Facility.GMTOffset, account.Facility.DSTOffset );

                this.TimeRecordCreation = facilityDateTime; //DateTime.Now;
                this.TransactionDate = facilityDateTime; // DateTime.Today;

                this.PatientAccountNumber = (int)account.AccountNumber;
                this.GuarantorNumber = (int)account.AccountNumber;

                if( account.Activity != null )
                {
                    User appUser = account.Activity.AppUser;
                    if( appUser != null )
                    {
                        if( appUser.WorkstationID != null )
                        {
                            this.WorkStationId = appUser.WorkstationID;
                        }
                    }
                }
                if( account.Facility != null )
                {
                    this.HospitalNumber = (int)account.Facility.Oid;
                }
                if( account.Patient != null )
                {
                    this.MedicalRecordNumber = (int)account.Patient.MedicalRecordNumber;
                    this.OrignalMedicalRecordNumber =
                        account.Patient.MedicalRecordNumber.ToString();
                }
                if( account.Guarantor != null )
                {
                    ReadGuarantorData();
                }
                if( account.Insurance != null )
                {
                    if( account.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ) != null
                        && account.Insurance.CoverageFor(
                        CoverageOrder.PRIMARY_OID ).Insured != null )
                    {
                        this.GroupNumber1 = account.Insurance.CoverageFor(
                            CoverageOrder.PRIMARY_OID ).Insured.GroupNumber;
                        this.SubscriberName1 = account.Insurance.CoverageFor(
                            CoverageOrder.PRIMARY_OID ).Insured.Name.FirstName;
                    }
                    if( account.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID ) != null &&
                        account.Insurance.CoverageFor(
                        CoverageOrder.SECONDARY_OID ).Insured != null )
                    {
                        this.GroupNumber2 = account.Insurance.CoverageFor(
                            CoverageOrder.SECONDARY_OID ).Insured.GroupNumber;
                        this.SubscriberName2 = account.Insurance.CoverageFor(
                            CoverageOrder.SECONDARY_OID ).Insured.Name.FirstName;
                    }
                }
            }
        }

        public override void InitializeColumnValues()
        {
            guarantorDetailsOrderedList.Add( APIDWS, string.Empty );
            guarantorDetailsOrderedList.Add( APIDID, "GM" );
            guarantorDetailsOrderedList.Add( APRR_, GUARANTOR_TRANSACTION_NUMBER );
            guarantorDetailsOrderedList.Add( APSEC2, string.Empty );
            guarantorDetailsOrderedList.Add( APHSP_, 0 );
            guarantorDetailsOrderedList.Add( APPREC, PATIENT_RECORD_NUMBER );
            guarantorDetailsOrderedList.Add( APACCT, 0 );
            guarantorDetailsOrderedList.Add( APMRC_, 0 );
            guarantorDetailsOrderedList.Add( APGAR_, 0 );
            guarantorDetailsOrderedList.Add( APPGAR, 0 );
            guarantorDetailsOrderedList.Add( APGLNM, string.Empty );
            guarantorDetailsOrderedList.Add( APGFNM, string.Empty );
            guarantorDetailsOrderedList.Add( APGAD1, string.Empty );
            guarantorDetailsOrderedList.Add( APGAD2, string.Empty );
            guarantorDetailsOrderedList.Add( APGCIT, string.Empty );
            guarantorDetailsOrderedList.Add( APGZIP, 0 );
            guarantorDetailsOrderedList.Add( APGCNT, string.Empty );
            guarantorDetailsOrderedList.Add( APGACD, 0 );
            guarantorDetailsOrderedList.Add( APGPH_, 0 );
            guarantorDetailsOrderedList.Add( APENM, string.Empty );
            guarantorDetailsOrderedList.Add( APEADR, string.Empty );
            guarantorDetailsOrderedList.Add( APECIT, string.Empty );
            guarantorDetailsOrderedList.Add( APESTE, string.Empty );
            guarantorDetailsOrderedList.Add( APEZIP, 0 );
            guarantorDetailsOrderedList.Add( APEZP4, 0 );
            guarantorDetailsOrderedList.Add( APEACD, 0 );
            guarantorDetailsOrderedList.Add( APEPH_, 0 );
            guarantorDetailsOrderedList.Add( APGOCC, string.Empty );
            guarantorDetailsOrderedList.Add( APFR01, 0 );
            guarantorDetailsOrderedList.Add( APFR02, 0 );
            guarantorDetailsOrderedList.Add( APFR03, 0 );
            guarantorDetailsOrderedList.Add( APGP01, string.Empty );
            guarantorDetailsOrderedList.Add( APGP02, string.Empty );
            guarantorDetailsOrderedList.Add( APGP03, string.Empty );
            guarantorDetailsOrderedList.Add( APPO01, string.Empty );
            guarantorDetailsOrderedList.Add( APPO02, string.Empty );
            guarantorDetailsOrderedList.Add( APPO03, string.Empty );
            guarantorDetailsOrderedList.Add( APSB01, string.Empty );
            guarantorDetailsOrderedList.Add( APSB02, string.Empty );
            guarantorDetailsOrderedList.Add( APSB03, string.Empty );
            guarantorDetailsOrderedList.Add( APRL01, string.Empty );
            guarantorDetailsOrderedList.Add( APRL02, string.Empty );
            guarantorDetailsOrderedList.Add( APRL03, string.Empty );
            guarantorDetailsOrderedList.Add( APNST, string.Empty );
            guarantorDetailsOrderedList.Add( APNOTE, string.Empty );
            guarantorDetailsOrderedList.Add( APGSTE, string.Empty );
            guarantorDetailsOrderedList.Add( APGZP4, 0 );
            guarantorDetailsOrderedList.Add( APGCNY, 0 );
            guarantorDetailsOrderedList.Add( APGSSN, string.Empty );
            guarantorDetailsOrderedList.Add( APGEID, string.Empty );
            guarantorDetailsOrderedList.Add( APGESC, string.Empty );
            guarantorDetailsOrderedList.Add( APFR04, 0 );
            guarantorDetailsOrderedList.Add( APFR05, 0 );
            guarantorDetailsOrderedList.Add( APFR06, 0 );
            guarantorDetailsOrderedList.Add( APGP04, string.Empty );
            guarantorDetailsOrderedList.Add( APGP05, string.Empty );
            guarantorDetailsOrderedList.Add( APGP06, string.Empty );
            guarantorDetailsOrderedList.Add( APPO04, string.Empty );
            guarantorDetailsOrderedList.Add( APPO05, string.Empty );
            guarantorDetailsOrderedList.Add( APPO06, string.Empty );
            guarantorDetailsOrderedList.Add( APSB04, string.Empty );
            guarantorDetailsOrderedList.Add( APSB05, string.Empty );
            guarantorDetailsOrderedList.Add( APSB06, string.Empty );
            guarantorDetailsOrderedList.Add( APRL04, string.Empty );
            guarantorDetailsOrderedList.Add( APRL05, string.Empty );
            guarantorDetailsOrderedList.Add( APRL06, string.Empty );
            guarantorDetailsOrderedList.Add( APLML, 0 );
            guarantorDetailsOrderedList.Add( APLMD, 0 );
            guarantorDetailsOrderedList.Add( APLUL_, 0 );
            guarantorDetailsOrderedList.Add( APACFL, string.Empty );
            guarantorDetailsOrderedList.Add( APTTME, 0 );
            guarantorDetailsOrderedList.Add( APINLG, LOG_NUMBER );
            guarantorDetailsOrderedList.Add( APBYPS, string.Empty );
            guarantorDetailsOrderedList.Add( APSWPY, 0 );
            guarantorDetailsOrderedList.Add( APDRL_, string.Empty );
            guarantorDetailsOrderedList.Add( APGLOE, string.Empty );
            guarantorDetailsOrderedList.Add( APUN, string.Empty );
            guarantorDetailsOrderedList.Add( APGPSM, string.Empty );
            guarantorDetailsOrderedList.Add( APGEML, string.Empty );
            guarantorDetailsOrderedList.Add( APGLR, string.Empty );
            guarantorDetailsOrderedList.Add( APGLRO, string.Empty );
            guarantorDetailsOrderedList.Add( APIN01, string.Empty );
            guarantorDetailsOrderedList.Add( APIN02, string.Empty );
            guarantorDetailsOrderedList.Add( APIN03, string.Empty );
            guarantorDetailsOrderedList.Add( APIN04, string.Empty );
            guarantorDetailsOrderedList.Add( APIN05, string.Empty );
            guarantorDetailsOrderedList.Add( APIN06, string.Empty );
            guarantorDetailsOrderedList.Add( APTDAT, 0 );
            guarantorDetailsOrderedList.Add( APCLRK, string.Empty );
            guarantorDetailsOrderedList.Add( APZDTE, string.Empty );
            guarantorDetailsOrderedList.Add( APZTME, string.Empty );
            guarantorDetailsOrderedList.Add( APGZPA, string.Empty );
            guarantorDetailsOrderedList.Add( APGZ4A, "0000" );
            guarantorDetailsOrderedList.Add( APGCUN, string.Empty );
            guarantorDetailsOrderedList.Add( APEZPA, string.Empty );
            guarantorDetailsOrderedList.Add( APEZ4A, "0000" );
            guarantorDetailsOrderedList.Add( APECUN, string.Empty );
            guarantorDetailsOrderedList.Add( APLAST, string.Empty );
            guarantorDetailsOrderedList.Add( APOMR_, string.Empty );
            guarantorDetailsOrderedList.Add( APAPP_, string.Empty );
            guarantorDetailsOrderedList.Add( APGMI, string.Empty );
            guarantorDetailsOrderedList.Add( APGSEX, string.Empty );
            guarantorDetailsOrderedList.Add( APGCPH, string.Empty );
            guarantorDetailsOrderedList.Add( APWSIR, WORKSTATION_ID );
            guarantorDetailsOrderedList.Add( APSECR, string.Empty );
            guarantorDetailsOrderedList.Add( APORR1, string.Empty );
            guarantorDetailsOrderedList.Add( APORR2, string.Empty );
            guarantorDetailsOrderedList.Add( APORR3, string.Empty );
            guarantorDetailsOrderedList.Add(APGGNRN, string.Empty);
            guarantorDetailsOrderedList.Add(APGAD1E1, string.Empty);
            guarantorDetailsOrderedList.Add(APGAD1E2, string.Empty);
        }



        public override ArrayList BuildSqlFrom( Account account,
            TransactionKeys transactionKeys )
        { 
            UpdateColumnValuesUsing( account );
            AddColumnsAndValuesToSqlStatement( this.guarantorDetailsOrderedList,
                "HPADAPGM" );
            //                "HPDATA2.HPADAPGM" );
            this.SqlStatements.Add( SqlStatement );
            return SqlStatements;
        }


        #endregion

        #region Private Methods
        private void ReadGuarantorData()
        {
            this.GuarantorLastName = i_Account.Guarantor.LastName;
            //this.GuarantorFirstName = i_Account.Guarantor.FirstName;
            if (i_Account.Guarantor.Name != null)
            {
                this.RespPartyMiddleInitial = i_Account.Guarantor.Name.MiddleInitial;
                GuarantorNameSuffix = i_Account.Guarantor.Name.Suffix.Trim();
            }

            // save the name back to pbar with the middle initial appended to the first name
            if (i_Account.Guarantor.Name.MiddleInitial != null &&
                i_Account.Guarantor.Name.MiddleInitial.Trim().Length > 0)
            {
                // the column to hold the 1st name is only 15 characters long
                // so if there is a middle initial and the first name is longer that 13
                // truncate the first name to 13 characters so the first name and the MI 
                // will fit in the transaction table's column.
                string trimmedFirstName = i_Account.Guarantor.FirstName;
                if (i_Account.Guarantor.FirstName.Length > 13 &&
                    i_Account.Guarantor.Name.MiddleInitial.Trim().Length > 0)
                {
                    trimmedFirstName = i_Account.Guarantor.FirstName.Substring(0, 13);
                }

                this.GuarantorFirstName = trimmedFirstName + " " + i_Account.Guarantor.Name.MiddleInitial;
            }
            else
            {
                this.GuarantorFirstName = i_Account.Guarantor.FirstName;
            }

            GuarantorDateOfBirth = i_Account.Guarantor.DateOfBirth;
            
            var guarantorMobileContactPoint = i_Account.Guarantor.ContactPointWith(
                            TypeOfContactPoint.NewMobileContactPointType());
            if (guarantorMobileContactPoint != null &&
                guarantorMobileContactPoint.PhoneNumber != null)
            {
                var cellPhone = guarantorMobileContactPoint.PhoneNumber.AreaCode.PadRight(3, ' ') +
                                guarantorMobileContactPoint.PhoneNumber.Number.PadRight(7, ' ');
                GuarantorCellPhone = cellPhone;
            }
            if (guarantorMobileContactPoint != null &&
                guarantorMobileContactPoint.CellPhoneConsent != null)
            {
                var consent = guarantorMobileContactPoint.CellPhoneConsent.Code;
                GuarantorConsent = consent;
            }

            if( i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).
                Address != null )
            {
                ReadGuarantorAddress();
            }
            if( i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).
                PhoneNumber != null )
            {
                ReadGuarantorPhoneNumber();
            }
            if( i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).
                EmailAddress != null )
            {
                this.EmailAddresses = i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).EmailAddress.Uri;
            }

            DriversLicense driversLicense = i_Account.Guarantor.DriversLicense;
            if( driversLicense != null )
            {
                if( driversLicense.State != null &&
                    driversLicense.State.Code != String.Empty )
                {
                    if( driversLicense.Number.Length > 15 )
                    {
                        string license = driversLicense.Number.Substring( 0, 15 );
                        this.GuarantorDriversLicenseNumber = license + driversLicense.State.Code;
                    }
                    else
                    {
                        this.GuarantorDriversLicenseNumber =
                            driversLicense.Number.PadRight( 15, ' ' ) +
                            driversLicense.State.Code;
                    }
                }
                else
                {
                    this.GuarantorDriversLicenseNumber = driversLicense.Number;
                }
            }
            if( i_Account.Guarantor.Sex != null )
            {
                this.GuarantorGender = i_Account.Guarantor.Sex.Code;

            }
            if( i_Account.Guarantor.SocialSecurityNumber != null )
            {
                if( i_Account.Guarantor.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim().Length > 1 )
                {
                    this.GuarantorSSN = i_Account.Guarantor.SocialSecurityNumber.UnformattedSocialSecurityNumber;

                }
            }
            if( i_Account.PrimaryInsured != null )
            {
                if( i_Account.Guarantor.Oid.Equals( i_Account.PrimaryInsured.Oid ) )
                {
                    this.GuarantorSameAsInsured = "Y";
                }
                else
                {
                    this.GuarantorSameAsInsured = "N";
                }
            }
            if( i_Account.Guarantor.Employment != null )
            {
                this.GuarantorsOccupation = i_Account.Guarantor.Employment.Occupation;
                this.GuarantorEmployeeID = i_Account.Guarantor.Employment.EmployeeID;

                if( i_Account.Guarantor.Employment.Status != null )
                {
                    this.GuarantorEmploymentStatus = i_Account.Guarantor.
                        Employment.Status.Code;
                }
                if( i_Account.Guarantor.Employment.Employer != null )
                {
                    this.EmployersName = i_Account.Guarantor.Employment.Employer.Name;
                }
            }
            if( i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewEmployerContactPointType() ).
                Address != null )
            {
                ReadEmployerAddress();
            }
            if( i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewEmployerContactPointType() ).
                PhoneNumber != null )
            {
                ReadEmployerPhoneNumber();
            }
        }

        private void ReadGuarantorAddress()
        {
            Address address = i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).
                Address;

            this.AddressLineStreet1 = address.Address1;
            this.AddressLineStreet2 = address.Address2;
            this.GuarantorCity = this.FormattedCity( address.City );
            //            this.GuarantorCity = address.City;

            if( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
            {
                this.GuarantorsZip = address.ZipCode.ZipCodePrimary;
                this.GuarantorsZipCode = address.ZipCode.ZipCodePrimaryAsInt;
            }
        
            if( address.ZipCode != null && address.ZipCode.ZipCodeExtended != null )
            {
                this.GuarantorsZip4 = address.ZipCode.ZipCodeExtendedZeroPadded;
                this.GuarantorZipCodeExt = address.ZipCode.ZipCodeExtendedAsInt;
            }

            if( address.County != null )
            {
                this.County = address.County.Code;
                this.GuarantorCountyCode = ConvertToInt( address.County.Code );
            }
            if( address.State != null )
            {
                this.GuarantorStateCode = address.State.Code;
            }
            if( address.Country != null )
            {
                this.GuarantorsCountry = address.Country.Code;
            }
        }

        private void ReadGuarantorPhoneNumber()
        {
            PhoneNumber phoneNumber = i_Account.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).
                PhoneNumber;

            this.AreaCodes = ConvertToInt( phoneNumber.AreaCode );
            this.PhoneNumbers = ConvertToInt( phoneNumber.Number );
        }

        private void ReadEmployerAddress()
        { 
            Address address =
                i_Account.Guarantor.Employment.Employer.PartyContactPoint.Address;

            this.EmployerAddress = this.FormatAddress( address.Address1 + address.Address2 );
            this.EmployerCity = this.FormattedCity( address.City );

            if( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
            {
                this.EmployersZipCode = address.ZipCode.ZipCodePrimaryAsInt;
                if( address.ZipCode.ZipCodePrimary.Trim().Length > 5 )
                {
                    this.EmployersZipCode1 = address.ZipCode.ZipCodePrimary.Trim().Substring( 0, 5 );
                }
                else
                {
                    this.EmployersZipCode1 = address.ZipCode.ZipCodePrimary.Trim();
                }
            }
        
            if( address.ZipCode != null && address.ZipCode.ZipCodeExtended != null )
            {
                this.GuarantorEmployerZipExt = address.ZipCode.ZipCodeExtendedAsInt;
                if( address.ZipCode.ZipCodeExtended.Trim().Length > 4 )
                {
                    this.GuarantorEmployerZipExt1 = address.ZipCode.ZipCodeExtended.Trim().Substring( 0, 4 );
                }
                else
                {
                    this.GuarantorEmployerZipExt1 = address.ZipCode.PadZeroZipCodeExtended( address.ZipCode.ZipCodeExtended.Trim() );
                }
            }

            if( address.State != null )
            {
                this.GuarantorEmployerState = address.State.Code;
            }

            if( address.Country != null )
            {
                this.EmployerCountry = address.Country.Code;
            }
        }

        private void ReadEmployerPhoneNumber()
        {
            PhoneNumber phoneNumber =
                i_Account.Guarantor.Employment.Employer.PartyContactPoint.PhoneNumber;

            this.EmployerAreaCode = ConvertToInt( phoneNumber.AreaCode );
            this.EmployerPhoneNumber = ConvertToInt( phoneNumber.Number );
        }

        #endregion

        #region Public Properties
        public string LastTransactionInGroup
        {
            set
            {
                this.guarantorDetailsOrderedList[APLAST] = value;
            }
        }
        public string UserSecurityCode
        {
            set
            {
                this.guarantorDetailsOrderedList[APSEC2] = value;
            }
        }
        #endregion

        #region  Private Properties

        private int HospitalNumber
        {
            set
            {
                this.guarantorDetailsOrderedList[APHSP_] = value;
            }
        }

        private int PatientAccountNumber
        {
            set
            {
                this.guarantorDetailsOrderedList[APACCT] = value;
            }
        }

        private string GuarantorConsent
        {
            set
            {
                guarantorDetailsOrderedList[APCLRK] = value;
            }
        }

        private int GuarantorNumber
        {
            set
            {
                this.guarantorDetailsOrderedList[APGAR_] = value;
            }
        }

        private int MedicalRecordNumber
        {
            set
            {
                this.guarantorDetailsOrderedList[APMRC_] = value;
            }
        }
        private string GuarantorLastName
        {
            set
            {
                this.guarantorDetailsOrderedList[APGLNM] = value;
            }
        }

        private DateTime GuarantorDateOfBirth
        {
            set
            {
                if (value == DateTime.MinValue)
                {
                    this.guarantorDetailsOrderedList[APUN] = "00000000";
                }
                else
                {
                    this.guarantorDetailsOrderedList[APUN] = ConvertDateToStringInShortyyyyMMddFormat(value);
                }
            }
        }

        private string GuarantorFirstName
        {
            set
            {
                this.guarantorDetailsOrderedList[APGFNM] = value;
            }
        }
        private string GuarantorNameSuffix
        {
            set
            {
                this.guarantorDetailsOrderedList[APGGNRN] = value;
            }
        }
        private string AddressLineStreet1
        {
            set
            {
                this.guarantorDetailsOrderedList[APGAD1E1] = value;
            }
        }

        private string AddressLineStreet2
        {
            set
            {
                this.guarantorDetailsOrderedList[APGAD1E2] = value;
            }
        }

        private string AddressLine1
        {
            set
            {
                this.guarantorDetailsOrderedList[APGAD1] = value;
            }
        }

        private string AddressLine2
        {
            set
            {
                this.guarantorDetailsOrderedList[APGAD2] = value;
            }
        }

        private string GuarantorCity
        {
            set
            {
                this.guarantorDetailsOrderedList[APGCIT] = value;
            }
        }

        private int GuarantorsZipCode
        {
            set
            {
                this.guarantorDetailsOrderedList[APGZIP] = value;
            }
        }

        private string County
        {
            set
            {
                this.guarantorDetailsOrderedList[APGCNT] = value;
            }
        }

        private int AreaCodes
        {
            set
            {
                this.guarantorDetailsOrderedList[APGACD] = value;
            }
        }

        private int PhoneNumbers
        {
            set
            {
                this.guarantorDetailsOrderedList[APGPH_] = value;
            }
        }

        private string EmployersName
        {
            set
            {
                this.guarantorDetailsOrderedList[APENM] = value;
            }
        }

        private string EmployerAddress
        {
            set
            {
                this.guarantorDetailsOrderedList[APEADR] = value;
            }
        }

        private string EmployerCity
        {
            set
            {
                this.guarantorDetailsOrderedList[APECIT] = value;
            }
        }

        private string EmployerCountry
        {
            set
            {
                this.guarantorDetailsOrderedList[APECUN] = value;
            }
        }
        private string GuarantorEmployerState
        {
            set
            {
                this.guarantorDetailsOrderedList[APESTE] = value;
            }
        }

        private int EmployersZipCode
        {
            set
            {
                this.guarantorDetailsOrderedList[APEZIP] = value;
            }
        }

        private int GuarantorEmployerZipExt
        {
            set
            {
                this.guarantorDetailsOrderedList[APEZP4] = value;
            }
        }
        private string EmployersZipCode1
        {
            set
            {
                this.guarantorDetailsOrderedList[APEZPA] = value;
            }
        }

        private string GuarantorEmployerZipExt1
        {
            set
            {
                this.guarantorDetailsOrderedList[APEZ4A] = value;
            }
        }
        private int EmployerAreaCode
        {
            set
            {
                this.guarantorDetailsOrderedList[APEACD] = value;
            }
        }

        private int EmployerPhoneNumber
        {
            set
            {
                this.guarantorDetailsOrderedList[APEPH_] = value;
            }
        }

        private string GuarantorsOccupation
        {
            set
            {
                this.guarantorDetailsOrderedList[APGOCC] = value;
            }
        }

        private string GroupNumber1
        {
            set
            {
                this.guarantorDetailsOrderedList[APGP01] = value;
            }
        }

        private string GroupNumber2
        {
            set
            {
                this.guarantorDetailsOrderedList[APGP02] = value;
            }
        }

        private string SubscriberName1
        {
            set
            {
                this.guarantorDetailsOrderedList[APSB01] = value;
            }
        }

        private string SubscriberName2
        {
            set
            {
                this.guarantorDetailsOrderedList[APSB02] = value;
            }
        }

        private string GuarantorStateCode
        {
            set
            {
                this.guarantorDetailsOrderedList[APGSTE] = value;
            }
        }

        private int GuarantorZipCodeExt
        {
            set
            {
                this.guarantorDetailsOrderedList[APGZP4] = value;
            }
        }

        private int GuarantorCountyCode
        {
            set
            {
                this.guarantorDetailsOrderedList[APGCNY] = value;
            }
        }

        private string GuarantorEmployeeID
        {
            set
            {
                this.guarantorDetailsOrderedList[APGEID] = value;
            }
        }

        private string GuarantorEmploymentStatus
        {
            set
            {
                this.guarantorDetailsOrderedList[APGESC] = value;
            }
        }

        private DateTime TimeRecordCreation
        {
            set
            {
                this.guarantorDetailsOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value );
            }
        }

        private string GuarantorDriversLicenseNumber
        {
            set
            {
                this.guarantorDetailsOrderedList[APDRL_] = value;
            }
        }

        private string GuarantorSameAsInsured
        {
            set
            {
                this.guarantorDetailsOrderedList[APGPSM] = value;
            }
        }

        private DateTime TransactionDate
        {
            set
            {
                this.guarantorDetailsOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string GuarantorsZip
        {
            set
            {
                this.guarantorDetailsOrderedList[APGZPA] = value;
            }
        }

        private string GuarantorsZip4
        {
            set
            {
                this.guarantorDetailsOrderedList[APGZ4A] = value;
            }
        }

        private string GuarantorsCountry
        {
            set
            {
                this.guarantorDetailsOrderedList[APGCUN] = value;
            }
        }

        private string OrignalMedicalRecordNumber
        {
            set
            {
                this.guarantorDetailsOrderedList[APOMR_] = value;
            }
        }

        private string EmailAddresses
        {
            set
            {
                this.guarantorDetailsOrderedList[APGEML] = value;
            }
        }

        private string RespPartyMiddleInitial
        {
            set
            {
                this.guarantorDetailsOrderedList[APGMI] = value;
            }
        }

        private string GuarantorCellPhone
        {
            set
            {
                this.guarantorDetailsOrderedList[APGCPH] = value;
            }
        }
        private string WorkStationId
        {
            set
            {
                this.guarantorDetailsOrderedList[APIDWS] = value;
            }
        }
        private string AddChangeFlag
        {
            set
            {
                this.guarantorDetailsOrderedList[APACFL] = value;
            }
        }
        private string GuarantorSSN
        {
            set
            {
                this.guarantorDetailsOrderedList[APGSSN] = value;
            }
        }
        private string GuarantorGender
        {
            set
            {
                this.guarantorDetailsOrderedList[APGSEX] = value;
            }
        }
        #endregion

        #region Data Elements

        //private Hashtable guarantorDetailsHashtable = new Hashtable();
        private OrderedList guarantorDetailsOrderedList = new OrderedList();
        private Account i_Account;

        #endregion

        #region Constants
        private const string
         APIDWS = "APIDWS",
         APIDID = "APIDID",
         APRR_ = "APRR#",
         APSEC2 = "APSEC2",
         APHSP_ = "APHSP#",
         APPREC = "APPREC",
         APACCT = "APACCT",
         APMRC_ = "APMRC#",
         APGAR_ = "APGAR#",
         APPGAR = "APPGAR",
         APGLNM = "APGLNM",
         APGFNM = "APGFNM",
         APGAD1 = "APGAD1",
         APGAD2 = "APGAD2",
         APGAD1E1 = "APGAD1E1",
         APGAD1E2 = "APGAD1E2",
         APGCIT = "APGCIT",
         APGZIP = "APGZIP",
         APGCNT = "APGCNT",
         APGACD = "APGACD",
         APGPH_ = "APGPH#",
        APENM = "APENM",
         APEADR = "APEADR",
         APECIT = "APECIT",
         APESTE = "APESTE",
         APEZIP = "APEZIP",
         APEZP4 = "APEZP4",
         APEACD = "APEACD",
         APEPH_ = "APEPH#",
         APGOCC = "APGOCC",
         APFR01 = "APFR01",
         APFR02 = "APFR02",
         APFR03 = "APFR03",
         APGP01 = "APGP01",
         APGP02 = "APGP02",
         APGP03 = "APGP03",
         APPO01 = "APPO01",
         APPO02 = "APPO02",
         APPO03 = "APPO03",
         APSB01 = "APSB01",
         APSB02 = "APSB02",
         APSB03 = "APSB03",
         APRL01 = "APRL01",
         APRL02 = "APRL02",
         APRL03 = "APRL03",
         APNST = "APNST",
         APNOTE = "APNOTE",
         APGSTE = "APGSTE",
         APGZP4 = "APGZP4",
         APGCNY = "APGCNY",
         APGSSN = "APGSSN",
         APGEID = "APGEID",
         APGESC = "APGESC",
         APFR04 = "APFR04",
         APFR05 = "APFR05",
         APFR06 = "APFR06",
         APGP04 = "APGP04",
         APGP05 = "APGP05",
         APGP06 = "APGP06",
         APPO04 = "APPO04",
         APPO05 = "APPO05",
         APPO06 = "APPO06",
         APSB04 = "APSB04",
         APSB05 = "APSB05",
         APSB06 = "APSB06",
         APRL04 = "APRL04",
         APRL05 = "APRL05",
         APRL06 = "APRL06",
         APLML = "APLML",
         APLMD = "APLMD",
         APLUL_ = "APLUL#",
         APACFL = "APACFL",
         APTTME = "APTTME",
         APINLG = "APINLG",
         APBYPS = "APBYPS",
         APSWPY = "APSWPY",
         APDRL_ = "APDRL#",
         APGLOE = "APGLOE",
         APUN = "APUN",
         APGPSM = "APGPSM",
         APGEML = "APGEML",
         APGLR = "APGLR",
         APGLRO = "APGLRO",
         APIN01 = "APIN01",
         APIN02 = "APIN02",
         APIN03 = "APIN03",
         APIN04 = "APIN04",
         APIN05 = "APIN05",
         APIN06 = "APIN06",
         APTDAT = "APTDAT",
         APCLRK = "APCLRK",
         APZDTE = "APZDTE",
         APZTME = "APZTME",
         APGZPA = "APGZPA",
         APGZ4A = "APGZ4A",
         APGCUN = "APGCUN",
         APEZPA = "APEZPA",
         APEZ4A = "APEZ4A",
         APECUN = "APECUN",
         APLAST = "APLAST",
         APOMR_ = "APOMR#",
         APAPP_ = "APAPP#",
         APGMI = "APGMI",
         APGSEX = "APGSEX",
         APGCPH = "APGCPH",
         APWSIR = "APWSIR",
         APSECR = "APSECR",
         APORR1 = "APORR1",
         APORR2 = "APORR2",
         APORR3 = "APORR3",
         APGGNRN = "APGGNRN";
        private const string NO_CELLPHONECONSENT = "N";
        #endregion

    }
}
