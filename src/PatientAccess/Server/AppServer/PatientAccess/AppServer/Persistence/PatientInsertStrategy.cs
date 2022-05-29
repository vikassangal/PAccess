using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties; 
using log4net;
using PatientAccess.Rules;


namespace PatientAccess.Persistence
{
    [Serializable]
    public class PatientInsertStrategy : SqlBuilderStrategy
    {
        #region Construction and Finalization

        public PatientInsertStrategy()
        {
            InitializeColumnValues();
        }

        #endregion

        #region Methods

        public override void UpdateColumnValuesUsing( Account account )
        {
            i_Account = account;

            if ( account != null )
            {
                if ( account.IsNew )
                {
                    AddChangeFlag = ADD_FLAG;
                }
                else
                {
                    AddChangeFlag = CHANGE_FLAG;
                }
                if ( account.Activity.GetType() == typeof( PreDischargeActivity ) )
                {
                    AddChangeFlag = CHANGE_FLAG;
                }
                // 21-JUN-2006
                // KJS
                // According to pbar this value is supposed to be set based on the
                // value of the field HPDATA1.HPADQCP3.QCPAUX where the hsp# is the
                // current hospital. FOR THE MOMENT we can hard code this since its
                // value is always the same.
                DaysSinceAdmission = 365;
                AccountNumber = (int)account.AccountNumber;
                if( account.Activity.IsTransferOutToInActivity() )
                {
                    AdmissionDate = account.TransferDate;
                    AdmittingTime = account.TransferDate;
                }
                else
                {
                    AdmissionDate = account.AdmitDate;
                    AdmittingTime = account.AdmitDate;
                }
             
                PatientType = account.KindOfVisit.Code;

                var tBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
                DateTime facilityDateTime = tBroker.TimeAt( account.Facility.GMTOffset, account.Facility.DSTOffset );

                TransactionDate = facilityDateTime;
                TimeRecordCreation = facilityDateTime;
                GuarantorNumber = (int)account.AccountNumber;
                if ( !account.IsNew )
                {
                    PatientAccountsKey = (int)account.AccountNumber;
                }

                if ( account.ClergyVisit != null )
                {
                    if ( account.ClergyVisit.Code.Trim() == YesNoFlag.CODE_YES )
                    {
                        ClergyVisit = YesNoFlag.CODE_NO;
                    }
                    else if ( account.ClergyVisit.Code.Trim() == YesNoFlag.CODE_NO )
                    {
                        ClergyVisit = YesNoFlag.CODE_YES;
                    }
                    else
                    {
                        ClergyVisit = account.ClergyVisit.Code.Trim();
                    }
                }
                DateOfAdmission = account.AdmitDate;
                TimeOfAdmission = account.AdmitDate;

                if (!account.DischargeDate.Date.Equals(DateTime.MinValue) &&
                    !account.Activity.IsTransferOutToInActivity())
                {
                    DischargeDate = account.DischargeDate;
                    TimeOfDischarge = account.DischargeDate;
                }

                if ( account.ScheduleCode != null )
                {
                    ScheduleCode = account.ScheduleCode.Code;
                }
                if ( account.PreopDate != DateTime.MinValue )
                {
                    PreopDate =
                        ConvertDateToStringInShortyyyyMMddFormat( account.PreopDate );
                }
                if ( account.FacilityDeterminedFlag != null )
                {
                    FacilityDeterminedFlag = account.FacilityDeterminedFlag.Code;
                }

                if ( account.TenetCare != null )
                {
                    TenetCare = account.TenetCare.Code.Trim();
                }

                if ( account.IsShortRegistered )
                {
                    IsShortRegistered = YesNoFlag.CODE_YES;
                }

                if ( account.IsQuickRegistered && account.Activity.AssociatedActivityType != typeof( ActivatePreRegistrationActivity ) )
                {
                    IsShortRegistered = QACFLAG;
                }
                else if (account.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) &&
                    ( !account.IsShortRegistered ) )
                {
                    IsShortRegistered = BLANK_FLAG;
                }

                if (account.IsPAIWalkinRegistered && account.Activity.AssociatedActivityType != typeof(ActivatePreRegistrationActivity))
                {
                    IsShortRegistered = PAIWalkinFlag;
                }

                IsolationCode = account.IsolationCode ?? String.Empty;
                
                if ( account.IsNewBorn )
                {
                    //SR 1557, per PBAR's request, do not set this flag for a straight newborn creation
                    //Side effect: straight created newborn has this flag off, but activate a pre-admit newborn to a newborn, the account has this flag on.
                    if ( account.Activity != null && account.Activity.IsAdmitNewbornActivity()
                            && ( account.Activity.AssociatedActivityType == null || account.Activity.AssociatedActivityType != typeof( ActivatePreRegistrationActivity ) ) )
                    {
                        IsNewbornRegistered = YesNoFlag.CODE_NO;
                    }
                    else
                    {
                        IsNewbornRegistered = YesNoFlag.CODE_YES;
                    }
                }
                else
                {
                    IsNewbornRegistered = YesNoFlag.CODE_NO;
                }
                DateOfLastMaintenance = DateTime.Today;

                if ( account.Facility != null )
                {
                    HospitalNumber = (int)account.Facility.Oid;
                }
                string embosserCard = String.Empty;
                if ( account.EmbosserCard != null )
                {
                    embosserCard = account.EmbosserCard;
                }
                if ( account.ClinicalComments != null )
                {
                    if ( account.ClinicalComments.Length <= TOTAL_LENGTH_OF_PBAR_COMMENTS )
                    {
                        ClinicalCommentsOne = account.ClinicalComments;
                        var leftPad = new string( ' ', 55 );
                        ClinicalCommentsTwo = leftPad + embosserCard;
                    }
                    else
                    {
                        ClinicalCommentsOne = account.ClinicalComments.Substring( 0, TOTAL_LENGTH_OF_PBAR_COMMENTS );
                        ClinicalCommentsTwo = account.ClinicalComments.Substring( TOTAL_LENGTH_OF_PBAR_COMMENTS ).PadRight
                                                  ( LENGTH_OF_COMMENTS_TWO, ' ' )
                                              + embosserCard;
                    }
                }
                if ( account.IsPatientInClinicalResearchStudy != null )
                {
                    PatientInClinicalStudy = account.IsPatientInClinicalResearchStudy.Code;
                }

                ReadClinicalResearchStudies( account );

                if ( account.Reregister != null )
                {
                    ReRegisterFlag = account.Reregister.Code;
                }

                if ( account.Patient != null )
                {
                    //Set Transaction APACFL Add/Change flag
                    MedicalRecordNumber = (int)account.Patient.MedicalRecordNumber;
                    if ( !account.Patient.IsNew )
                    {
                        PatientDemographicsKey = (int)account.Patient.MedicalRecordNumber;
                    }

                    LastName = account.Patient.LastName;
                    // save the name back to pbar with the middle initial appended to the first name
                    if ( account.Patient.MiddleInitial != null &&
                        account.Patient.MiddleInitial.Trim().Length > 0 )
                    {
                        // the column to hold the 1st name is only 15 characters long
                        // so if there is a middle initial and the first name is longer that 13
                        // truncate the first name to 13 characters so the first name and the MI 
                        // will fit in the transaction table's column.
                        string trimmedFirstName = account.Patient.FirstName;
                        if ( account.Patient.FirstName.Length > 13 &&
                            account.Patient.MiddleInitial.Trim().Length > 0 )
                        {
                            trimmedFirstName = account.Patient.FirstName.Substring( 0, 13 );
                        }

                        FirstName = trimmedFirstName + " " + account.Patient.MiddleInitial;
                    }
                    else
                    {
                        FirstName = account.Patient.FirstName;
                    }
                    PlaceOfBirth = account.Patient.PlaceOfBirth;
                    DateOfBirth = account.Patient.DateOfBirth;
                    DateOfBirthCen = ConvertToInt( account.Patient.DateOfBirth.
                                                      ToString( "Mddyyyy", DateTimeFormatInfo.InvariantInfo ) );
                    TimeOfBirth = account.Patient.DateOfBirth;
                    IsBirthTimeEntered = account.BirthTimeIsEntered ? YesNoFlag.CODE_YES : YesNoFlag.CODE_NO;
                    MiddleInitial = account.Patient.MiddleInitial;
                    MaidenName = account.Patient.MaidenName;
                    NameSuffix = account.Patient.Suffix;

                    if ( !account.Patient.DateOfBirth.Equals( DateTime.MinValue ) )
                    {

                        string strAge = account.Patient.AgeAt(account.AdmitDate);
                        //Set HPADAPMP.APAGE='000D' when age returned empty.
                        if ( string.IsNullOrEmpty( strAge ) )
                            strAge = "000D";
                        AgeAtAdmission = strAge.PadLeft( 4, '0' ).ToUpper();
                    }
                    else
                    {
                        AgeAtAdmission = "000D";
                    }

                    if ( account.Patient.PlaceOfWorship != null )
                    {
                        ParishCode = account.Patient.PlaceOfWorship.Code;
                    }

                    if ( account.Patient.Aliases.Count > 0 )
                    {
                        AKAFirstName = ( (Name)account.Patient.Aliases[0] ).FirstName;
                        AKALastName = ( (Name)account.Patient.Aliases[0] ).LastName;
                    }

                    if ( account.Patient.Sex != null )
                    {
                        Sex = account.Patient.Sex.Code;
                    }

                    if (account.Activity.GetType() == typeof(AdmitNewbornActivity) ||
                        account.Activity.GetType() == typeof(PreAdmitNewbornActivity))
                    {
                        account.Patient.BirthSex = account.Patient.Sex;
                    }

                    if (account.Patient.BirthSex != null)
                    {
                        BirthSex = account.Patient.BirthSex.Code;
                    }
                    //SATX
                    TransferFromHospitalNumber = account.Patient.InterFacilityTransferAccount.FromFacilityOid;
                    TransferFromAccountNumber = account.Patient.InterFacilityTransferAccount.FromAccountNumber;
                   

                    if ( account.Patient.NationalID != null )
                    {
                        NationalID = account.Patient.NationalID;
                    }
                    if (account.Patient.Race == null
                        || String.IsNullOrEmpty(account.Patient.Race.Code)
                        || account.Patient.Race.Code == Race.ZERO_CODE)
                    {
                        account.Patient.Race = new Race();
                    }

                    Races = account.Patient.Race.Code;

                    if (account.Patient.Race != null &&
                        account.Patient.Nationality != null)
                    {
                        Nationality = account.Patient.Nationality.Code;
                    }
                    if (account.Patient.Race2 == null
                        || String.IsNullOrEmpty(account.Patient.Race2.Code)
                        || account.Patient.Race2.Code == Race.ZERO_CODE)
                    {
                        account.Patient.Race2 = new Race();
                    }

                    Race2 = account.Patient.Race2.Code;

                    if (account.Patient.Race2 != null &&
                        account.Patient.Nationality2 != null)
                    {
                        Nationality2 = account.Patient.Nationality2.Code;
                    }

                    if (account.Patient.Ethnicity == null
                        || String.IsNullOrEmpty(account.Patient.Ethnicity.Code)
                        || account.Patient.Ethnicity.Code == Ethnicity.ZERO_CODE)
                    {
                        account.Patient.Ethnicity = new Ethnicity();
                    }

                    Ethnicities = account.Patient.Ethnicity.Code;

                    if (account.Patient.Ethnicity != null &&
                        account.Patient.Descent != null)
                    {
                        Descent = account.Patient.Descent.Code;
                    }

                    if (account.Patient.Ethnicity2 == null
                        || String.IsNullOrEmpty(account.Patient.Ethnicity2.Code)
                        || account.Patient.Ethnicity2.Code == Ethnicity.ZERO_CODE)
                    {
                        account.Patient.Ethnicity2 = new Ethnicity();
                    }

                    Ethnicity2 = account.Patient.Ethnicity2.Code;

                    if (account.Patient.Ethnicity2 != null &&
                        account.Patient.Descent2 != null)
                    {
                        Descent2 = account.Patient.Descent2.Code;
                    }
                    
                    Language language = account.Patient.Language;
                    if ( language != null )
                    {
                        PatientLanguage = language.Code;
                        if (language.IsOtherLanguage() && account.Patient.OtherLanguage != null)
                        {
                            PatientOtherLanguage = account.Patient.OtherLanguage.Trim();
                        }
                    }

                    if ( account.Patient.MaritalStatus != null )
                    {
                        MaritalStatusData = account.Patient.MaritalStatus.Code;
                    }

                    if ( account.Patient.SocialSecurityNumber != null )
                    {
                        SocialSecurityNumbers =
                            account.Patient.SocialSecurityNumber.AreaNumber +
                            account.Patient.SocialSecurityNumber.GroupNumber +
                            account.Patient.SocialSecurityNumber.Series;
                    }

                    if ( account.Patient.Religion != null )
                    {
                        ReligionCode = account.Patient.Religion.Code;
                    }

                    if ( account.Patient.Employment != null )
                    {
                        Occupation = account.Patient.Employment.Occupation;
                        EmployeeID = account.Patient.Employment.EmployeeID;
                        if ( account.Patient.Employment.Status != null )
                        {
                            EmploymentStatusCode = account.Patient.Employment.Status.Code;
                        }
                        if ( account.Patient.Employment.Employer.PartyContactPoint != null )
                        {
                            ReadPatientEmploymentAddress();
                        }
                        if ( account.Patient.Employment.Employer != null )
                        {
                            EmployersName = account.Patient.Employment.Employer.Name;
                        }
                    }

                    DriversLicense driversLicense = account.Patient.DriversLicense;
                    if ( driversLicense != null )
                    {
                        if ( driversLicense.State != null &&
                            driversLicense.State.Code != String.Empty )
                        {
                            if ( driversLicense.Number.Length > 15 )
                            {
                                string license = driversLicense.Number.Substring( 0, 15 );
                                DriversLicenseData = license + driversLicense.State.Code;
                            }
                            else
                            {
                                DriversLicenseData =
                                    driversLicense.Number.PadRight( 15, ' ' ) +
                                    driversLicense.State.Code;
                            }
                        }
                        else
                        {
                            DriversLicenseData = driversLicense.Number;
                        }
                    }

                    Passport passport = account.Patient.Passport;
                    if ( passport != null )
                    {
                        PassportNumber = passport.Number;

                        if ( passport.Country != null &&
                            passport.Country.Code != String.Empty )
                        {
                            PassportCountry = passport.Country.Code;
                        }
                    }

                    if ( account.Patient.ContactPointWith(
                            TypeOfContactPoint.NewMailingContactPointType() ).
                            Address != null )
                    {
                        ReadPatientLocalAddress();
                    }

                    if ( account.Patient.ContactPointWith(
                            TypeOfContactPoint.NewMailingContactPointType() ).
                            PhoneNumber != null )
                    {
                        ReadPatientPhoneNumber();
                    }

                    if ( account.Patient.ContactPointWith(
                            TypeOfContactPoint.NewPhysicalContactPointType() ).
                            Address != null )
                    {
                        ReadPermanentPatientAddress();
                    }

                    if ( account.Patient.ContactPointWith(
                            TypeOfContactPoint.NewPhysicalContactPointType() ).
                            PhoneNumber != null )
                    {
                        ReadPermanentPatientPhoneNumber();
                    }

                    ContactPoint emailContactPoint = account.Patient.ContactPointWith(
                        TypeOfContactPoint.NewMailingContactPointType() );

                    if ( emailContactPoint.EmailAddress != null && emailContactPoint.EmailAddress.Uri != null )
                    {
                        EmailAddress = emailContactPoint.EmailAddress.Uri;
                    }

                    //read email reason
                    if (account.Patient.EmailReason != null &&
                        !String.IsNullOrEmpty(account.Patient.EmailReason.Code))
                    {
                        APMDFLValue = account.Patient.EmailReason.Code;
                    }

                    if ( account.Patient.ContactPointWith(
                            TypeOfContactPoint.NewMobileContactPointType() ).
                            PhoneNumber != null )
                    {
                        CellPhoneNumber = account.Patient.ContactPointWith(
                            TypeOfContactPoint.NewMobileContactPointType() ).
                            PhoneNumber.ToString();
                    }


                    if ( account.Patient.ContactPointWith(
                            TypeOfContactPoint.NewEmployerContactPointType() ).
                            PhoneNumber != null )
                    {
                        ReadPatientEmployerPhoneNumber();
                    }
                }

                // the DoNotResuscitate field (also known as the Patient county of residence)
                // can have 3 values 1 = true. 2 = false, 0 = not set.
                if ( account.Patient.DoNotResuscitate.Code.Equals( YesNoFlag.CODE_YES ) )
                {
                    DoNotResuscitate = 1;
                }
                else if ( account.Patient.DoNotResuscitate.Code.Equals( YesNoFlag.CODE_NO ) )
                {
                    DoNotResuscitate = 2;
                }
                else if ( account.Patient.DoNotResuscitate.Code.Equals( YesNoFlag.CODE_BLANK ) )
                {
                    DoNotResuscitate = 0;
                }

                if ( account.HospitalService != null )
                {
                    MedicalServiceCode = account.HospitalService.Code;
                }

                if (account.PatientPortalOptIn == null || account.PatientPortalOptIn.Code == String.Empty)
                {
                    account.PatientPortalOptIn.SetBlank();
                }
				
                if (account.RightToRestrict == null || account.RightToRestrict.Code == String.Empty)
                {
                    account.RightToRestrict.SetBlank();
                }

                if (account.ShareDataWithPublicHieFlag == null || account.ShareDataWithPublicHieFlag.Code.Trim() == String.Empty)
                {
                    account.ShareDataWithPublicHieFlag.SetNo();
                }
                else
                {
                    if(account.Activity.IsQuickAccountCreationActivity() 
                        || account.Activity.IsQuickAccountMaintenanceActivity()
                        || account.Activity.IsPAIWalkinOutpatientCreationActivity()
                        || !FeatureManagerHIE.IsShareHieDataEnabledforaccount(account))
                    {
                        YesNoFlag defualtHIE = FeatureManagerHIE.DefaultShareHieDataForFacility(account.Facility);
                        var ShareHIEData = defualtHIE == YesNoFlag.Blank ? YesNoFlag.No : defualtHIE;
                        account.ShareDataWithPublicHieFlag = ShareHIEData;
                    }
                }

                if (account.ShareDataWithPCPFlag == null || account.ShareDataWithPCPFlag.Code.Trim() == String.Empty)
                {
                    account.ShareDataWithPCPFlag.SetNo();
                }
                PatientPortalOptIn = account.PatientPortalOptIn.Code + account.RightToRestrict.Code+ account.ShareDataWithPublicHieFlag.Code + account.ShareDataWithPCPFlag.Code ;

                //read email reason
                
                var emailReasonCode = String.Empty;

                if (account.Patient.EmailReason != null &&
                    !String.IsNullOrEmpty(account.Patient.EmailReason.Code))
                {
                    emailReasonCode = account.Patient.EmailReason.Code.Substring(0, 1);
                }
                else
                {
                    emailReasonCode = YesNoFlag.CODE_BLANK;
                }
                if (account.Patient.HospitalCommunicationOptIn == null || account.Patient.HospitalCommunicationOptIn.Code.Trim() == String.Empty)
                {
                    account.Patient.HospitalCommunicationOptIn.SetBlank();
                }
             
                if (account.COBReceived == null || account.COBReceived.Code == String.Empty)
                {
                    account.COBReceived.SetBlank();
                }
                if (account.IMFMReceived == null || account.IMFMReceived.Code == String.Empty)
                {
                    account.IMFMReceived.SetBlank();
                }

                APMDFLValue = account.Patient.HospitalCommunicationOptIn.Code + emailReasonCode + APMDFLFILLER + APMDFLFILLER + account.COBReceived.Code + account.IMFMReceived.Code;
                
                if ( account.EmergencyContact1 != null )
                {
                    if ( account.EmergencyContact1.Name.Trim().Length > 25 )
                    {
                        EmergencyContactName = account.EmergencyContact1.Name.Trim().Substring( 0, 24 );
                    }
                    else
                    {
                        EmergencyContactName = account.EmergencyContact1.Name.Trim();
                    }

                    if ( account.EmergencyContact1.ContactPointWith(
                            TypeOfContactPoint.NewPhysicalContactPointType() ).
                            Address != null )
                    {
                        ReadEmergencyContact1Address();
                    }

                    if ( account.EmergencyContact1.ContactPointWith(
                            TypeOfContactPoint.NewPhysicalContactPointType() ).
                            PhoneNumber != null )
                    {
                        ReadEmergencyContact1PhoneNumber();
                    }

                    if ( account.EmergencyContact1.RelationshipType != null )
                    {
                        EmcyContactRelToPatCode =
                            account.EmergencyContact1.RelationshipType.Code;
                    }
                }

                if ( account.EmergencyContact2 != null )
                {
                    if ( account.EmergencyContact2.Name.Trim().Length > 25 )
                    {
                        RelativesName = account.EmergencyContact2.Name.Trim().Substring( 0, 24 );
                    }
                    else
                    {
                        RelativesName = account.EmergencyContact2.Name.Trim();
                    }

                    if ( account.EmergencyContact2.ContactPointWith(
                            TypeOfContactPoint.NewPhysicalContactPointType() ).
                            Address != null )
                    {
                        ReadEmergencyContact2Address();
                    }

                    if ( account.EmergencyContact2.ContactPointWith(
                            TypeOfContactPoint.NewPhysicalContactPointType() ).
                            PhoneNumber != null )
                    {
                        ReadEmergencyContact2PhoneNumber();
                    }

                    if ( account.EmergencyContact2.RelationshipType != null )
                    {
                        NearestRelRelToPatCode =
                            account.EmergencyContact2.RelationshipType.Code;
                    }
                }


                if ( account.Diagnosis != null )
                {
                    DiagnosisData = account.Diagnosis.ChiefComplaint;
                    if ( account.KindOfVisit.Code != VisitType.EMERGENCY_PATIENT )
                    {
                        ProcedureData = account.Diagnosis.Procedure;
                    }
                    else
                    {
                        ProcedureData = string.Empty;
                    }
                    if ( account.Diagnosis.Condition != null )
                    {
                        if ( account.Diagnosis.Condition is Accident )
                        {
                            var accident = ( (Accident)account.Diagnosis.Condition );
                            AccidentTypeCode = ConvertToInt( accident.Kind.Code );
                            AccidentDate = accident.OccurredOn;
                            AccidentTime = accident.OccurredAtHour;
                            AccidentCode = accident.Kind.Code;

                            if ( accident.Country != null )
                            {
                                CountryCodeAfter = accident.Country.Code;
                            }
                            if ( accident.State != null )
                            {
                                AutoAccidentState = accident.State.Code;
                            }
                        }
                        if ( account.Diagnosis.Condition is Crime )
                        {
                            var crime = ( (Crime)account.Diagnosis.Condition );

                            AccidentDate = crime.OccurredOn;
                            AccidentTime = crime.OccurredAtHour;


                            if ( crime.Country != null )
                            {
                                CountryCodeAfter = crime.Country.Code;
                            }
                            if ( crime.State != null )
                            {
                                AutoAccidentState = crime.State.Code;
                            }
                        }
                        if ( account.Diagnosis.Condition is Pregnancy )
                        {
                            var pregnancy = ( (Pregnancy)account.Diagnosis.Condition );
                            DateOfLastMenstruation = pregnancy.LastPeriod;
                        }
                    }

                    ReadOccurrenceCode( account.OccurrenceCodes );
                }

                ReadConditionCode( account.ConditionCodes );
                ReadPBARValueCodesAndValueAmounts();
                ReadClinicCodes( account );
                ReadSiteCode( account );
                ReadCodedDiagnosis( account );

                if ( account.MedicalGroupIPA != null )
                {
                    IPA = account.MedicalGroupIPA.Code;
                    var ipaClinics = (ArrayList)account.MedicalGroupIPA.Clinics;
                    if ( ipaClinics != null && ipaClinics.Count > 0 )
                    {
                        IPAClinic = ( (Clinic)( ipaClinics[0] ) ).Code;
                    }
                }

                if ( account.Insurance != null )
                {
                    if ( account.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ) != null &&
                        account.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ).Insured != null )
                    {
                        ReadPrimaryInsuredEmploymentInfo();
                    }

                    if ( account.Insurance.CoverageFor(
                            CoverageOrder.SECONDARY_OID ) != null &&
                        account.Insurance.CoverageFor(
                            CoverageOrder.SECONDARY_OID ).Insured != null )
                    {
                        ReadSecondaryInsuredEmploymentInfo();
                    }
                    if ( account.FinancialClass != null )
                    {
                        FinancialClassData = account.FinancialClass.Code;
                    }
                }
                if ( account.Patient.RelationshipWith( account.Guarantor ) != null )
                {
                    PatientGuarantorRelationship = account.Patient.RelationshipWith( account.Guarantor ).Code;
                }

                if (!string.IsNullOrEmpty(account.AdmittingCategory))
                    AdmittingCategory = account.AdmittingCategory;

                if ( account.KindOfVisit != null )
                {
                    if ( account.KindOfVisit.Code.Equals( VisitType.PREREG_PATIENT ) )
                    {
                        TypeOfRegistration = REGISTRATIONTYPE3;
                    }
                      else if ( account.KindOfVisit.Code.Equals( VisitType.INPATIENT ) && 
                        ! ( account.Activity.IsMaintenanceActivity() || 
                            account.Activity.IsTransferOutToInActivity() )
                        )
                    {
                        TypeOfRegistration = REGISTRATIONTYPE1;
                        if ( account.AdmitSource != null )
                        {
                            if ( account.AdmitSource.Code == NEWBORN_CODE )
                            {
                                AdmittingCategory = ADMITTING_CATEGORY_NEWBORN;
                            }
                            else if (string.IsNullOrEmpty(account.AdmittingCategory))
                            {
                                AdmittingCategory = ADMITTING_CATEGORY_INPATIENT;
                            }
                        }
                        else if (string.IsNullOrEmpty(account.AdmittingCategory))
                        {
                            AdmittingCategory = ADMITTING_CATEGORY_INPATIENT;
                        }
                    }
                    else if ( account.KindOfVisit.Code.Equals( VisitType.RECURRING_PATIENT ) )
                    {
                        TypeOfRegistration = REGISTRATIONTYPE2;
                    }
                    else if ( account.KindOfVisit.Code.Equals( VisitType.OUTPATIENT ) ||
                             account.KindOfVisit.Code.Equals( VisitType.EMERGENCY_PATIENT ) ||
                             account.KindOfVisit.Code.Equals( VisitType.NON_PATIENT ) )
                    {
                        TypeOfRegistration = REGISTRATIONTYPE1;
                    }
                }

                if ( account.AdmitSource != null )
                {
                    AdmitSourceCode = account.AdmitSource.Code;
                }
                if ( account.AlternateCareFacility != null )
                {
                    AlternateCareFacility = account.AlternateCareFacility;
                }

                if ( account.Location != null )
                {
                    if ( account.Location.NursingStation != null )
                    {
                        if ( account.Location.NursingStation.Code.Length < 2 )
                        {
                            var locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
                            account.Location.NursingStation.Code =
                                locationBroker.GetEntireNursingStationCode( account.Facility,
                                                                           account.Location.NursingStation.Code );
                        }
                        NursingStation = account.Location.NursingStation.Code;
                    }
                    if ( account.Location.Room != null )
                    {
                        RoomNumber = Convert.ToInt32( account.Location.Room.Code );
                    }
                    if ( account.Location.Bed != null )
                    {
                        BedNumber = account.Location.Bed.Code;

                        if ( account.Location.Bed.Accomodation != null )
                        {
                            AccomodationCode = account.Location.Bed.Accomodation.Code;
                        }
                    }
                }

                ReadRCRPValues( account );

                var physicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
                nonStaffPhysicianNumber = physicianBroker.GetNonStaffPhysicianNumber();
                ReadAdmittingPhysicianInfo();
                ReadAttendingPhysicianInfo();
                ReadOperatingPhysicianInfo();
                ReadPrimaryCarePhysicianInfo();
                ReadReferringPhysicianInfo();
                ReadOccurrenceSpanCodeInfo();
                ReadMothersInfo();
                ReadFathersInfo();
                ReadConfidentialityAndOptOutInfo();

                if ( account.Activity != null )
                {
                    User appUser = account.Activity.AppUser;
                    if ( appUser != null )
                    {
                        if ( appUser.WorkstationID != null )
                        {
                            WorkStationId = appUser.WorkstationID;
                        }
                    }
                }
                if ( account.Bloodless != null )
                {
                    BloodlessCode = account.Bloodless.Code;
                }

                NoticeOfPrivacyPracticeDocument nppDocument = account.Patient.NoticeOfPrivacyPracticeDocument;
                if ( nppDocument != null && nppDocument.NPPVersion != null )
                {
                    CurrentNPPVersionNumber = nppDocument.NPPVersion.Code;
                    CurrentNPPVersionDate = ConvertDateToStringInShortyyyyMMddFormat( nppDocument.NPPVersion.NPPDate );
                    NPPSignatureStatus = nppDocument.SignatureStatus.Code;
                    NPPDateSigned = ConvertDateToStringInShortyyyyMMddFormat( nppDocument.SignedOnDate );
                }

                if ( account.Smoker != null )
                {
                    SmokingFlag = account.Smoker.Code;
                }
                if ( account.ValuablesAreTaken != null )
                {
                    ValuablesCollectedFlag = account.ValuablesAreTaken.Code;
                }
                if (account.ReferralSource != null)
                {
                    ReferralSource = account.ReferralSource.Code;
                }
                if ( account.AbstractExists )
                {
                    AbstractExists = SHORT_FORM_YES;
                }
                else
                {
                    AbstractExists = SHORT_FORM_NO;
                }
                if ( account.ArrivalTime != DateTime.MinValue )
                {
                    ArrivalTime = account.ArrivalTime;
                }

                if ( account.ModeOfArrival != null )
                {
                    ModeOfArrival = account.ModeOfArrival.Code;
                }
                if ( account.ReAdmitCode != null )
                {
                    ReAdmitCode = account.ReAdmitCode.Code;
                }
                if ( account.ReferralFacility != null )
                {
                    ReferralFacility = account.ReferralFacility.Code;
                }
                if ( account.ReferralType != null )
                {
                    ReferralType = account.ReferralType.Code;
                }
                if ( ( account.Diagnosis.GetType() == typeof( Pregnancy ) ) ||
                 ( account.Pregnant != null && account.Pregnant.Code == YesNoFlag.CODE_YES ) )
                {
                    PregnancyIndicator = YesNoFlag.CODE_YES;
                }
                else
                {
                    PregnancyIndicator = account.Pregnant.Code;
                }

                string cptCodes = string.Empty;

                if (account.CptCodes != null && account.CptCodes.Count > 0)
                {
                    for (var i = 1; i <=10; i++)
                    {
                        if ( !account.CptCodes.ContainsKey(i) ||
                            (account.CptCodes[i] == null || account.CptCodes[i].ToString() == string.Empty))
                            cptCodes = cptCodes + BLANK;
                        else
                            cptCodes = cptCodes + account.CptCodes[i];
                    }
                    CptCodes = cptCodes;
                }
            }
        }

        private void ReadRCRPValues( Account account )
        {
            if ( account.KindOfVisit.Code != VisitType.EMERGENCY_PATIENT )
            {
                return;
            }

            if ( account.RightCareRightPlace != null && account.RightCareRightPlace.RCRP != null )
            {
                RightCareRightPlace = account.RightCareRightPlace.RCRP.Code;
            }
            if ( account.RightCareRightPlace != null && account.RightCareRightPlace.LeftOrStayed != null )
            {
                LeftOrStayed = account.RightCareRightPlace.LeftOrStayed.Code;
            }
            if ( account.LeftWithOutBeingSeen != null )
            {
                LeftWithOutBeingSeen = account.LeftWithOutBeingSeen.Code;
            }
            if ( account.LeftWithoutFinancialClearance != null )
            {
                LeftWithOutFinacialClearance = account.LeftWithoutFinancialClearance.Code;
            }
        }

        public override void InitializeColumnValues()
        {
            patientDetailsOrderedList.Add( APIDWS, string.Empty );
            patientDetailsOrderedList.Add( APIDID, string.Empty );
            patientDetailsOrderedList.Add( APGREC, GUARANTOR_TRANSACTION_NUMBER );
            patientDetailsOrderedList.Add( APRR_, PATIENT_RECORD_NUMBER );
            patientDetailsOrderedList.Add( APPAUX, 0 );
            patientDetailsOrderedList.Add( APSEC2, string.Empty );
            patientDetailsOrderedList.Add( APHSP_, 0 );
            patientDetailsOrderedList.Add( APMRC_, 0 );
            patientDetailsOrderedList.Add( APACCT, 0 );
            patientDetailsOrderedList.Add( APPLNM, string.Empty );
            patientDetailsOrderedList.Add( APPFNM, string.Empty );
            patientDetailsOrderedList.Add( APLAST, string.Empty );
            patientDetailsOrderedList.Add( APSEX, string.Empty );
            patientDetailsOrderedList.Add( APRACE, string.Empty );
            patientDetailsOrderedList.Add( APAGE, string.Empty );
            patientDetailsOrderedList.Add( APDOB, 0 );
            patientDetailsOrderedList.Add( APADMC, string.Empty );
            patientDetailsOrderedList.Add( APADMT, 0 );
            patientDetailsOrderedList.Add( APADTM, 0 );
            patientDetailsOrderedList.Add( APNS, string.Empty );
            patientDetailsOrderedList.Add( APROOM, 0 );
            patientDetailsOrderedList.Add( APBED, string.Empty );
            patientDetailsOrderedList.Add( APETHC, string.Empty );
            patientDetailsOrderedList.Add( APPTYP, string.Empty );
            patientDetailsOrderedList.Add( APMSV, string.Empty );
            patientDetailsOrderedList.Add( APSMOK, string.Empty );
            patientDetailsOrderedList.Add( APDIET, string.Empty );
            patientDetailsOrderedList.Add( APCOND, string.Empty );
            patientDetailsOrderedList.Add( APISO, string.Empty );
            patientDetailsOrderedList.Add( APADR_, 0 );
            patientDetailsOrderedList.Add( APRDR_, 0 );
            patientDetailsOrderedList.Add( APPOB, string.Empty );
            patientDetailsOrderedList.Add( APSSN, string.Empty );
            patientDetailsOrderedList.Add( APTDAT, 0 );
            patientDetailsOrderedList.Add( APLAD, 0 );
            patientDetailsOrderedList.Add( APLVLN, string.Empty );
            patientDetailsOrderedList.Add( APRLGN, string.Empty );
            patientDetailsOrderedList.Add( APALRG, string.Empty );
            patientDetailsOrderedList.Add( APCOLG, string.Empty );
            patientDetailsOrderedList.Add( APPADR, string.Empty );
            patientDetailsOrderedList.Add( APPCIT, string.Empty );
            patientDetailsOrderedList.Add( APPSTE, string.Empty );
            patientDetailsOrderedList.Add( APPZIP, 0 );
            patientDetailsOrderedList.Add( APPZP4, 0 );
            patientDetailsOrderedList.Add( APPCNY, 0 );
            patientDetailsOrderedList.Add( APPACD, 0 );
            patientDetailsOrderedList.Add( APPPH_, 0 );
            patientDetailsOrderedList.Add( APMADR, string.Empty );
            patientDetailsOrderedList.Add( APMCIT, string.Empty );
            patientDetailsOrderedList.Add( APMSTE, string.Empty );
            patientDetailsOrderedList.Add( APMZIP, 0 );
            patientDetailsOrderedList.Add( APMZP4, 0 );
            patientDetailsOrderedList.Add( APMCNY, string.Empty );
            patientDetailsOrderedList.Add( APMACD, 0 );
            patientDetailsOrderedList.Add( APMPH_, 0 );
            patientDetailsOrderedList.Add( APRNM, string.Empty );
            patientDetailsOrderedList.Add( APRADR, string.Empty );
            patientDetailsOrderedList.Add( APRCIT, string.Empty );
            patientDetailsOrderedList.Add( APRSTE, string.Empty );
            patientDetailsOrderedList.Add( APRZIP, 0 );
            patientDetailsOrderedList.Add( APRZP4, 0 );
            patientDetailsOrderedList.Add( APRACD, 0 );
            patientDetailsOrderedList.Add( APRPH_, 0 );
            patientDetailsOrderedList.Add( APCNM, string.Empty );
            patientDetailsOrderedList.Add( APCADR, string.Empty );
            patientDetailsOrderedList.Add( APCCIT, string.Empty );
            patientDetailsOrderedList.Add( APCSTE, string.Empty );
            patientDetailsOrderedList.Add( APCZIP, 0 );
            patientDetailsOrderedList.Add( APCZP4, 0 );
            patientDetailsOrderedList.Add( APCACD, 0 );
            patientDetailsOrderedList.Add( APCPH_, 0 );
            patientDetailsOrderedList.Add( APWNM, string.Empty );
            patientDetailsOrderedList.Add( APWADR, string.Empty );
            patientDetailsOrderedList.Add( APWCIT, string.Empty );
            patientDetailsOrderedList.Add( APWSTE, string.Empty );
            patientDetailsOrderedList.Add( APWZIP, 0 );
            patientDetailsOrderedList.Add( APWZP4, 0 );
            patientDetailsOrderedList.Add( APWACD, 0 );
            patientDetailsOrderedList.Add( APWPH_, 0 );
            patientDetailsOrderedList.Add( APHEMP, string.Empty );
            patientDetailsOrderedList.Add( APMAR, string.Empty );
            patientDetailsOrderedList.Add( APMNM, string.Empty );
            patientDetailsOrderedList.Add( APMOTH, string.Empty );
            patientDetailsOrderedList.Add( APMPT_, 0 );
            patientDetailsOrderedList.Add( APSNM, string.Empty );
            patientDetailsOrderedList.Add( APSADR, string.Empty );
            patientDetailsOrderedList.Add( APSCIT, string.Empty );
            patientDetailsOrderedList.Add( APSSTE, string.Empty );
            patientDetailsOrderedList.Add( APSZIP, 0 );
            patientDetailsOrderedList.Add( APSZP4, 0 );
            patientDetailsOrderedList.Add( APSACD, 0 );
            patientDetailsOrderedList.Add( APSPH_, 0 );
            patientDetailsOrderedList.Add( APCD01, string.Empty );
            patientDetailsOrderedList.Add( APCD02, string.Empty );
            patientDetailsOrderedList.Add( APCD03, string.Empty );
            patientDetailsOrderedList.Add( APCD04, string.Empty );
            patientDetailsOrderedList.Add( APCD05, string.Empty );
            patientDetailsOrderedList.Add( APCD06, string.Empty );
            patientDetailsOrderedList.Add( APCD07, string.Empty );
            patientDetailsOrderedList.Add( APCD08, string.Empty );
            patientDetailsOrderedList.Add( APDIAG, string.Empty );

            patientDetailsOrderedList.Add(APCNTY, string.Empty);
            patientDetailsOrderedList.Add( APCOM1, string.Empty );
            patientDetailsOrderedList.Add( APCOM2, string.Empty );
            patientDetailsOrderedList.Add( APTCRT, string.Empty );
            patientDetailsOrderedList.Add( APDCRT, 0 );
            patientDetailsOrderedList.Add( APSCRT, 0 );
            patientDetailsOrderedList.Add( APNCRT, 0 );
            patientDetailsOrderedList.Add( APICRT, 0 );
            patientDetailsOrderedList.Add( AP_EXT, 0 );
            patientDetailsOrderedList.Add( APDTOT, 0 );
            patientDetailsOrderedList.Add( APCLOC, string.Empty );
            patientDetailsOrderedList.Add( APPMRC, 0 );
            patientDetailsOrderedList.Add( APGAR_, 0 );
            patientDetailsOrderedList.Add( APHOWA, string.Empty );
            patientDetailsOrderedList.Add( APPOLN, string.Empty );
            patientDetailsOrderedList.Add( APCORC, string.Empty );
            patientDetailsOrderedList.Add( APALOC, string.Empty );
            patientDetailsOrderedList.Add( APATIM, 0 );
            patientDetailsOrderedList.Add( APPST, string.Empty );
            patientDetailsOrderedList.Add( APDISC, 0 );
            patientDetailsOrderedList.Add( APDEDT, 0 );
            patientDetailsOrderedList.Add( APLVD, 0 );
            patientDetailsOrderedList.Add( APLAT, 0 );
            patientDetailsOrderedList.Add( APTREG, string.Empty );
            patientDetailsOrderedList.Add( APCL01, string.Empty );
            patientDetailsOrderedList.Add( APCL02, string.Empty );
            patientDetailsOrderedList.Add( APCL03, string.Empty );
            patientDetailsOrderedList.Add( APCL04, string.Empty );
            patientDetailsOrderedList.Add( APCL05, string.Empty );
            patientDetailsOrderedList.Add( APCL06, string.Empty );
            patientDetailsOrderedList.Add( APCL07, string.Empty );
            patientDetailsOrderedList.Add( APCL08, string.Empty );
            patientDetailsOrderedList.Add( APCL09, string.Empty );
            patientDetailsOrderedList.Add( APCL10, string.Empty );
            patientDetailsOrderedList.Add( APCL11, string.Empty );
            patientDetailsOrderedList.Add( APCL12, string.Empty );
            patientDetailsOrderedList.Add( APCL13, string.Empty );
            patientDetailsOrderedList.Add( APCL14, string.Empty );
            patientDetailsOrderedList.Add( APCL15, string.Empty );
            patientDetailsOrderedList.Add( APCL16, string.Empty );
            patientDetailsOrderedList.Add( APCL17, string.Empty );
            patientDetailsOrderedList.Add( APCL18, string.Empty );
            patientDetailsOrderedList.Add( APCL19, string.Empty );
            patientDetailsOrderedList.Add( APCL20, string.Empty );
            patientDetailsOrderedList.Add( APCL21, string.Empty );
            patientDetailsOrderedList.Add( APCL22, string.Empty );
            patientDetailsOrderedList.Add( APCL23, string.Empty );
            patientDetailsOrderedList.Add( APCL24, string.Empty );
            patientDetailsOrderedList.Add( APCL25, string.Empty );
            patientDetailsOrderedList.Add( APLDD, 0 );
            patientDetailsOrderedList.Add( APLDT, 0 );
            patientDetailsOrderedList.Add( APKDEM, 0 );
            patientDetailsOrderedList.Add( APKACT, 0 );
            patientDetailsOrderedList.Add( APPGAR, 0 );
            patientDetailsOrderedList.Add( APAL01, 0 );
            patientDetailsOrderedList.Add( APAL02, 0 );
            patientDetailsOrderedList.Add( APAL03, 0 );
            patientDetailsOrderedList.Add( APAL04, 0 );
            patientDetailsOrderedList.Add( APAL05, 0 );
            patientDetailsOrderedList.Add( APAL06, 0 );
            patientDetailsOrderedList.Add( APAL07, 0 );
            patientDetailsOrderedList.Add( APAL08, 0 );
            patientDetailsOrderedList.Add( APAL09, 0 );
            patientDetailsOrderedList.Add( APAL10, 0 );
            patientDetailsOrderedList.Add( APPSRC, string.Empty );
            patientDetailsOrderedList.Add( APPSTS, string.Empty );
            patientDetailsOrderedList.Add( APCI01, string.Empty );
            patientDetailsOrderedList.Add( APCI02, string.Empty );
            patientDetailsOrderedList.Add( APCI03, string.Empty );
            patientDetailsOrderedList.Add( APCI04, string.Empty );
            patientDetailsOrderedList.Add( APCI05, string.Empty );
            patientDetailsOrderedList.Add( APSPGM, string.Empty );
            patientDetailsOrderedList.Add( APAPVL, string.Empty );
            patientDetailsOrderedList.Add( APAPFR, 0 );
            patientDetailsOrderedList.Add( APAPTO, 0 );
            patientDetailsOrderedList.Add( APGRCD, 0 );
            patientDetailsOrderedList.Add( APTACD, string.Empty );
            patientDetailsOrderedList.Add( APOC01, string.Empty );
            patientDetailsOrderedList.Add( APOC02, string.Empty );
            patientDetailsOrderedList.Add( APOC03, string.Empty );
            patientDetailsOrderedList.Add( APOC04, string.Empty );
            patientDetailsOrderedList.Add( APOC05, string.Empty );
            patientDetailsOrderedList.Add( APOA01, 0 );
            patientDetailsOrderedList.Add( APOA02, 0 );
            patientDetailsOrderedList.Add( APOA03, 0 );
            patientDetailsOrderedList.Add( APOA04, 0 );
            patientDetailsOrderedList.Add( APOA05, 0 );
            patientDetailsOrderedList.Add( APSPNC, string.Empty );
            patientDetailsOrderedList.Add( APSPFR, 0 );
            patientDetailsOrderedList.Add( APSPTO, 0 );
            patientDetailsOrderedList.Add( APRA01, string.Empty );
            patientDetailsOrderedList.Add( APRA02, string.Empty );
            patientDetailsOrderedList.Add( APRA03, string.Empty );
            patientDetailsOrderedList.Add( APRA04, string.Empty );
            patientDetailsOrderedList.Add( APRA05, string.Empty );
            patientDetailsOrderedList.Add( APCC01, string.Empty );
            patientDetailsOrderedList.Add( APCC02, string.Empty );
            patientDetailsOrderedList.Add( APCC03, string.Empty );
            patientDetailsOrderedList.Add( APMRPR, string.Empty );
            patientDetailsOrderedList.Add( APRRCD, string.Empty );
            patientDetailsOrderedList.Add( APUCST, 0 );
            patientDetailsOrderedList.Add( APYUCS, 0 );
            patientDetailsOrderedList.Add( APINSN, string.Empty );
            patientDetailsOrderedList.Add( APRF01, string.Empty );
            patientDetailsOrderedList.Add( APRF02, string.Empty );
            patientDetailsOrderedList.Add( APRF03, string.Empty );
            patientDetailsOrderedList.Add( APRF04, string.Empty );
            patientDetailsOrderedList.Add( APRF05, string.Empty );
            patientDetailsOrderedList.Add( APRF06, string.Empty );
            patientDetailsOrderedList.Add( APBA01, string.Empty );
            patientDetailsOrderedList.Add( APBA02, string.Empty );
            patientDetailsOrderedList.Add( APBA03, string.Empty );
            patientDetailsOrderedList.Add( APBA04, string.Empty );
            patientDetailsOrderedList.Add( APBA05, string.Empty );
            patientDetailsOrderedList.Add( APBA06, string.Empty );
            patientDetailsOrderedList.Add( APLN01, string.Empty );
            patientDetailsOrderedList.Add( APLN02, string.Empty );
            patientDetailsOrderedList.Add( APLN03, string.Empty );
            patientDetailsOrderedList.Add( APLN04, string.Empty );
            patientDetailsOrderedList.Add( APLN05, string.Empty );
            patientDetailsOrderedList.Add( APLN06, string.Empty );
            patientDetailsOrderedList.Add( APFI01, string.Empty );
            patientDetailsOrderedList.Add( APFI02, string.Empty );
            patientDetailsOrderedList.Add( APFI03, string.Empty );
            patientDetailsOrderedList.Add( APFI04, string.Empty );
            patientDetailsOrderedList.Add( APFI05, string.Empty );
            patientDetailsOrderedList.Add( APFI06, string.Empty );
            patientDetailsOrderedList.Add( APSX01, string.Empty );
            patientDetailsOrderedList.Add( APSX02, string.Empty );
            patientDetailsOrderedList.Add( APSX03, string.Empty );
            patientDetailsOrderedList.Add( APSX04, string.Empty );
            patientDetailsOrderedList.Add( APSX05, string.Empty );
            patientDetailsOrderedList.Add( APSX06, string.Empty );
            patientDetailsOrderedList.Add( APRS01, string.Empty );
            patientDetailsOrderedList.Add( APRS02, string.Empty );
            patientDetailsOrderedList.Add( APRS03, string.Empty );
            patientDetailsOrderedList.Add( APRS04, string.Empty );
            patientDetailsOrderedList.Add( APRS05, string.Empty );
            patientDetailsOrderedList.Add( APRS06, string.Empty );
            patientDetailsOrderedList.Add( APJA01, string.Empty );
            patientDetailsOrderedList.Add( APJA02, string.Empty );
            patientDetailsOrderedList.Add( APJA03, string.Empty );
            patientDetailsOrderedList.Add( APJA04, string.Empty );
            patientDetailsOrderedList.Add( APJA05, string.Empty );
            patientDetailsOrderedList.Add( APJA06, string.Empty );
            patientDetailsOrderedList.Add( APCX01, string.Empty );
            patientDetailsOrderedList.Add( APCX02, string.Empty );
            patientDetailsOrderedList.Add( APCX03, string.Empty );
            patientDetailsOrderedList.Add( APCX04, string.Empty );
            patientDetailsOrderedList.Add( APCX05, string.Empty );
            patientDetailsOrderedList.Add( APCX06, string.Empty );
            patientDetailsOrderedList.Add( APJS01, string.Empty );
            patientDetailsOrderedList.Add( APJS02, string.Empty );
            patientDetailsOrderedList.Add( APJS03, string.Empty );
            patientDetailsOrderedList.Add( APJS04, string.Empty );
            patientDetailsOrderedList.Add( APJS05, string.Empty );
            patientDetailsOrderedList.Add( APJS06, string.Empty );
            patientDetailsOrderedList.Add( APJZ01, 0 );
            patientDetailsOrderedList.Add( APJZ02, 0 );
            patientDetailsOrderedList.Add( APJZ03, 0 );
            patientDetailsOrderedList.Add( APJZ04, 0 );
            patientDetailsOrderedList.Add( APJZ05, 0 );
            patientDetailsOrderedList.Add( APJZ06, 0 );
            patientDetailsOrderedList.Add( APJ401, 0 );
            patientDetailsOrderedList.Add( APJ402, 0 );
            patientDetailsOrderedList.Add( APJ403, 0 );
            patientDetailsOrderedList.Add( APJ404, 0 );
            patientDetailsOrderedList.Add( APJ405, 0 );
            patientDetailsOrderedList.Add( APJ406, 0 );
            patientDetailsOrderedList.Add( APJC01, 0 );
            patientDetailsOrderedList.Add( APJC02, 0 );
            patientDetailsOrderedList.Add( APJC03, 0 );
            patientDetailsOrderedList.Add( APJC04, 0 );
            patientDetailsOrderedList.Add( APJC05, 0 );
            patientDetailsOrderedList.Add( APJC06, 0 );
            patientDetailsOrderedList.Add( APJP01, 0 );
            patientDetailsOrderedList.Add( APJP02, 0 );
            patientDetailsOrderedList.Add( APJP03, 0 );
            patientDetailsOrderedList.Add( APJP04, 0 );
            patientDetailsOrderedList.Add( APJP05, 0 );
            patientDetailsOrderedList.Add( APJP06, 0 );
            patientDetailsOrderedList.Add( APG_01, string.Empty );
            patientDetailsOrderedList.Add( APG_02, string.Empty );
            patientDetailsOrderedList.Add( APG_03, string.Empty );
            patientDetailsOrderedList.Add( APG_04, string.Empty );
            patientDetailsOrderedList.Add( APG_05, string.Empty );
            patientDetailsOrderedList.Add( APG_06, string.Empty );
            patientDetailsOrderedList.Add( API_01, string.Empty );
            patientDetailsOrderedList.Add( API_02, string.Empty );
            patientDetailsOrderedList.Add( API_03, string.Empty );
            patientDetailsOrderedList.Add( API_04, string.Empty );
            patientDetailsOrderedList.Add( API_05, string.Empty );
            patientDetailsOrderedList.Add( API_06, string.Empty );
            patientDetailsOrderedList.Add( APAG01, 0 );
            patientDetailsOrderedList.Add( APAG02, 0 );
            patientDetailsOrderedList.Add( APAG03, 0 );
            patientDetailsOrderedList.Add( APAG04, 0 );
            patientDetailsOrderedList.Add( APAG05, 0 );
            patientDetailsOrderedList.Add( APAG06, 0 );
            patientDetailsOrderedList.Add( APCH01, 0 );
            patientDetailsOrderedList.Add( APCH02, 0 );
            patientDetailsOrderedList.Add( APCH03, 0 );
            patientDetailsOrderedList.Add( APCH04, 0 );
            patientDetailsOrderedList.Add( APCH05, 0 );
            patientDetailsOrderedList.Add( APCH06, 0 );
            patientDetailsOrderedList.Add( APCP01, 0 );
            patientDetailsOrderedList.Add( APCP02, 0 );
            patientDetailsOrderedList.Add( APCP03, 0 );
            patientDetailsOrderedList.Add( APCP04, 0 );
            patientDetailsOrderedList.Add( APCP05, 0 );
            patientDetailsOrderedList.Add( APCP06, 0 );
            patientDetailsOrderedList.Add( APCMED, string.Empty );
            patientDetailsOrderedList.Add( APNRCD, string.Empty );
            patientDetailsOrderedList.Add( APCRCD, string.Empty );
            patientDetailsOrderedList.Add( APACOD, string.Empty );
            patientDetailsOrderedList.Add( APACDT, 0 );
            patientDetailsOrderedList.Add( APEEID, string.Empty );
            patientDetailsOrderedList.Add( APESCD, string.Empty );
            patientDetailsOrderedList.Add( APFANM, string.Empty );
            patientDetailsOrderedList.Add( APLVTD, 0 );
            patientDetailsOrderedList.Add( APMONM, string.Empty );
            patientDetailsOrderedList.Add( APPOCC, string.Empty );
            patientDetailsOrderedList.Add( APEID1, "00000000000" );
            patientDetailsOrderedList.Add( APEID2, "00000000000" );
            patientDetailsOrderedList.Add( APEDC1, string.Empty );
            patientDetailsOrderedList.Add( APEDC2, string.Empty );
            patientDetailsOrderedList.Add( APESC1, string.Empty );
            patientDetailsOrderedList.Add( APESC2, string.Empty );
            patientDetailsOrderedList.Add( APENM1, string.Empty );
            patientDetailsOrderedList.Add( APENM2, string.Empty );
            patientDetailsOrderedList.Add( APELO1, string.Empty );
            patientDetailsOrderedList.Add( APELO2, string.Empty );
            patientDetailsOrderedList.Add( APPUBL, string.Empty );
            patientDetailsOrderedList.Add( APTSCR, string.Empty );
            patientDetailsOrderedList.Add( APSDED, 0 );
            patientDetailsOrderedList.Add( APDNOT, 0 );
            patientDetailsOrderedList.Add( APTALT, 0 );
            patientDetailsOrderedList.Add( APALTD, 0 );
            patientDetailsOrderedList.Add( AP_COD, 0 );
            patientDetailsOrderedList.Add( APACTP, 0 );
            patientDetailsOrderedList.Add( APWRKC, string.Empty );
            patientDetailsOrderedList.Add( APNOFT, string.Empty );
            patientDetailsOrderedList.Add( APABOR, 0 );
            patientDetailsOrderedList.Add( APLMEN, 0 );
            patientDetailsOrderedList.Add( APRTCD, 0 );
            patientDetailsOrderedList.Add( APSPC1, string.Empty );
            patientDetailsOrderedList.Add( APSPC2, string.Empty );
            patientDetailsOrderedList.Add( APSPC3, string.Empty );
            patientDetailsOrderedList.Add( APSPC4, string.Empty );
            patientDetailsOrderedList.Add( APSPC5, string.Empty );
            patientDetailsOrderedList.Add( APSPC6, string.Empty );
            patientDetailsOrderedList.Add( APODR_, 0 );
            patientDetailsOrderedList.Add( APOP01, string.Empty );
            patientDetailsOrderedList.Add( APOP02, string.Empty );
            patientDetailsOrderedList.Add( APOP03, string.Empty );
            patientDetailsOrderedList.Add( APOP04, string.Empty );
            patientDetailsOrderedList.Add( APOD01, 0 );
            patientDetailsOrderedList.Add( APOD02, 0 );
            patientDetailsOrderedList.Add( APOD03, 0 );
            patientDetailsOrderedList.Add( APOD04, 0 );
            patientDetailsOrderedList.Add( APLCL, string.Empty );
            patientDetailsOrderedList.Add( APTSRC, string.Empty );
            patientDetailsOrderedList.Add( APLML, 0 );
            patientDetailsOrderedList.Add( APLMD, 0 );
            patientDetailsOrderedList.Add( APLUL_, 0 );
            patientDetailsOrderedList.Add( APLUL2, 0 );
            patientDetailsOrderedList.Add( APACFL, string.Empty );
            patientDetailsOrderedList.Add( APTTME, 0 );
            patientDetailsOrderedList.Add( APINLG, LOG_NUMBER );
            patientDetailsOrderedList.Add( APBYPS, string.Empty );
            patientDetailsOrderedList.Add( APSWPY, 0 );
            patientDetailsOrderedList.Add( APPVRR, string.Empty );
            patientDetailsOrderedList.Add( APBDFG, string.Empty );
            patientDetailsOrderedList.Add( APBDAC, string.Empty );
            patientDetailsOrderedList.Add( APPLOE, string.Empty );
            patientDetailsOrderedList.Add( APMBF, string.Empty );
            patientDetailsOrderedList.Add( APERFG, string.Empty );
            patientDetailsOrderedList.Add( APFC, string.Empty );
            patientDetailsOrderedList.Add( APACC, string.Empty );
            patientDetailsOrderedList.Add( APRSRC, 0 );
            patientDetailsOrderedList.Add( APFDOB, 0 );
            patientDetailsOrderedList.Add( APMDOB, 0 );
            patientDetailsOrderedList.Add( APEA01, string.Empty );
            patientDetailsOrderedList.Add( APEA02, string.Empty );
            patientDetailsOrderedList.Add( APEZ01, 0 );
            patientDetailsOrderedList.Add( APEZ02, 0 );
            patientDetailsOrderedList.Add( APFORM, string.Empty );
            patientDetailsOrderedList.Add( APIN01, string.Empty );
            patientDetailsOrderedList.Add( APIN02, string.Empty );
            patientDetailsOrderedList.Add( APFR01, 0 );
            patientDetailsOrderedList.Add( APFR02, 0 );
            patientDetailsOrderedList.Add( APOSIN, string.Empty );
            patientDetailsOrderedList.Add( APOSFD, 0 );
            patientDetailsOrderedList.Add( APOSTD, 0 );
            patientDetailsOrderedList.Add( APPRGT, string.Empty );
            patientDetailsOrderedList.Add( APMELG, string.Empty );
            patientDetailsOrderedList.Add( APABC1, string.Empty );
            patientDetailsOrderedList.Add( APXMIT, string.Empty );
            patientDetailsOrderedList.Add( APQNUM, 0 );
            patientDetailsOrderedList.Add( APVIS_, 0 );
            patientDetailsOrderedList.Add( APUPRV, string.Empty );
            patientDetailsOrderedList.Add( APUPRW, string.Empty );
            patientDetailsOrderedList.Add( APZDTE, string.Empty );
            patientDetailsOrderedList.Add( APZTME, string.Empty );
            patientDetailsOrderedList.Add( APWRNF, string.Empty );
            patientDetailsOrderedList.Add( APNMTL, string.Empty );
            patientDetailsOrderedList.Add( APDOB8, 0 );
            patientDetailsOrderedList.Add( APACMT, string.Empty );
            patientDetailsOrderedList.Add( APMB_1, 0 );
            patientDetailsOrderedList.Add( APMB_2, 0 );
            patientDetailsOrderedList.Add( APMB_3, 0 );
            patientDetailsOrderedList.Add( APLRDT, 0 );
            patientDetailsOrderedList.Add( APNRDT, 0 );
            patientDetailsOrderedList.Add( APPZPA, string.Empty );
            patientDetailsOrderedList.Add( APPZ4A, "0000" );
            patientDetailsOrderedList.Add( APWZPA, string.Empty );
            patientDetailsOrderedList.Add( APWZ4A, "0000" );
            patientDetailsOrderedList.Add( APWCUN, string.Empty );
            patientDetailsOrderedList.Add( APAKLN, string.Empty );
            patientDetailsOrderedList.Add( APAKFM, string.Empty );
            patientDetailsOrderedList.Add( APMDR_, 0 );
            patientDetailsOrderedList.Add( APFBLF, string.Empty );
            patientDetailsOrderedList.Add( APABST, string.Empty );
            patientDetailsOrderedList.Add( APAMLF, string.Empty );
            patientDetailsOrderedList.Add( APDTCD, string.Empty );
            patientDetailsOrderedList.Add( APVALU, string.Empty );
            patientDetailsOrderedList.Add( APMZPA, string.Empty );
            patientDetailsOrderedList.Add( APMZ4A, "0000" );
            patientDetailsOrderedList.Add( APCZPA, string.Empty );
            patientDetailsOrderedList.Add( APCZ4A, "0000" );
            patientDetailsOrderedList.Add( APRZPA, string.Empty );
            patientDetailsOrderedList.Add( APRZ4A, "0000" );
            patientDetailsOrderedList.Add( APCNCD, string.Empty );
            patientDetailsOrderedList.Add( APMNU_, string.Empty );
            patientDetailsOrderedList.Add( APMNDN, string.Empty );
            patientDetailsOrderedList.Add( APMNFR, string.Empty );
            patientDetailsOrderedList.Add( APRNU_, string.Empty );
            patientDetailsOrderedList.Add( APRNDN, string.Empty );
            patientDetailsOrderedList.Add( APRNFR, string.Empty );
            patientDetailsOrderedList.Add( APLNGC, string.Empty );
            patientDetailsOrderedList.Add( APSPN2, string.Empty );
            patientDetailsOrderedList.Add( APOC06, string.Empty );
            patientDetailsOrderedList.Add( APOC07, string.Empty );
            patientDetailsOrderedList.Add( APOC08, string.Empty );
            patientDetailsOrderedList.Add( APOA06, 0 );
            patientDetailsOrderedList.Add( APOA07, 0 );
            patientDetailsOrderedList.Add( APOA08, 0 );
            patientDetailsOrderedList.Add( APPRED, string.Empty );
            patientDetailsOrderedList.Add( AP_FC, 0 );
            patientDetailsOrderedList.Add( AP_RC, 0 );
            patientDetailsOrderedList.Add( APNOFC, 0 );
            patientDetailsOrderedList.Add( AP_PL, 0 );
            patientDetailsOrderedList.Add( APELTM, 0 );
            patientDetailsOrderedList.Add( APIPA, string.Empty );
            patientDetailsOrderedList.Add( APIPAC, string.Empty );
            patientDetailsOrderedList.Add( APCLVS, string.Empty );
            patientDetailsOrderedList.Add( APPEML, string.Empty );
            patientDetailsOrderedList.Add( APPDL_, string.Empty );
            patientDetailsOrderedList.Add( APMNCD, 0 );
            patientDetailsOrderedList.Add( APMNP_, 0 );
            patientDetailsOrderedList.Add( APRTYP, string.Empty );
            patientDetailsOrderedList.Add( APFRCD, string.Empty );
            patientDetailsOrderedList.Add( APARVC, string.Empty );
            patientDetailsOrderedList.Add( APRAMC, string.Empty );
            patientDetailsOrderedList.Add( APPCCD, string.Empty );
            patientDetailsOrderedList.Add( APBLDL, string.Empty );
            patientDetailsOrderedList.Add( APTECR, string.Empty );
            patientDetailsOrderedList.Add( APPCUN, string.Empty );
            patientDetailsOrderedList.Add( APLBWT, string.Empty );
            patientDetailsOrderedList.Add( APPTID, string.Empty );
            patientDetailsOrderedList.Add( APPRGI, string.Empty );
            patientDetailsOrderedList.Add( APPMI, string.Empty );
            patientDetailsOrderedList.Add( APTDR_, string.Empty );
            patientDetailsOrderedList.Add( APMNLN, string.Empty );
            patientDetailsOrderedList.Add( APMNFN, string.Empty );
            patientDetailsOrderedList.Add( APMNMN, string.Empty );
            patientDetailsOrderedList.Add( APMNPR, string.Empty );
            patientDetailsOrderedList.Add( APMTXC, string.Empty );
            patientDetailsOrderedList.Add( APRNLN, string.Empty );
            patientDetailsOrderedList.Add( APRNFN, string.Empty );
            patientDetailsOrderedList.Add( APRNMN, string.Empty );
            patientDetailsOrderedList.Add( APRNPR, string.Empty );
            patientDetailsOrderedList.Add( APRTXC, string.Empty );
            patientDetailsOrderedList.Add( APRNP_, string.Empty );
            patientDetailsOrderedList.Add( APANLN, string.Empty );
            patientDetailsOrderedList.Add( APANFN, string.Empty );
            patientDetailsOrderedList.Add( APANMN, string.Empty );
            patientDetailsOrderedList.Add( APANU_, string.Empty );
            patientDetailsOrderedList.Add( APANPR, string.Empty );
            patientDetailsOrderedList.Add( APATXC, string.Empty );
            patientDetailsOrderedList.Add( APANP_, string.Empty );
            patientDetailsOrderedList.Add( APONLN, string.Empty );
            patientDetailsOrderedList.Add( APONFN, string.Empty );
            patientDetailsOrderedList.Add( APONMN, string.Empty );
            patientDetailsOrderedList.Add( APONU_, string.Empty );
            patientDetailsOrderedList.Add( APONPR, string.Empty );
            patientDetailsOrderedList.Add( APOTXC, string.Empty );
            patientDetailsOrderedList.Add( APONP_, string.Empty );
            patientDetailsOrderedList.Add( APTNLN, string.Empty );
            patientDetailsOrderedList.Add( APTNFN, string.Empty );
            patientDetailsOrderedList.Add( APTNMN, string.Empty );
            patientDetailsOrderedList.Add( APTNU_, string.Empty );
            patientDetailsOrderedList.Add( APTNPR, string.Empty );
            patientDetailsOrderedList.Add( APTTXC, string.Empty );
            patientDetailsOrderedList.Add( APTNP_, string.Empty );
            patientDetailsOrderedList.Add( APOZWT, string.Empty );
            patientDetailsOrderedList.Add( APPCPH, string.Empty );
            patientDetailsOrderedList.Add( APNTPP, string.Empty );
            patientDetailsOrderedList.Add( APDTRC, string.Empty );
            patientDetailsOrderedList.Add( APAVTM, string.Empty );
            patientDetailsOrderedList.Add( APCI06, string.Empty );
            patientDetailsOrderedList.Add( APCI07, string.Empty );
            patientDetailsOrderedList.Add( APNPPV, string.Empty );
            patientDetailsOrderedList.Add( APNPPF, string.Empty );
            patientDetailsOrderedList.Add( APPARC, string.Empty );
            patientDetailsOrderedList.Add( APACST, string.Empty );
            patientDetailsOrderedList.Add( APACCN, string.Empty );
            patientDetailsOrderedList.Add( APSCHD, string.Empty );
            patientDetailsOrderedList.Add( APMSL_, string.Empty );
            patientDetailsOrderedList.Add( APRSL_, string.Empty );
            patientDetailsOrderedList.Add( APASL_, string.Empty );
            patientDetailsOrderedList.Add( APOSL_, string.Empty );
            patientDetailsOrderedList.Add( APTSL_, string.Empty );
            patientDetailsOrderedList.Add( APPPT_, string.Empty );
            patientDetailsOrderedList.Add( APICUN, string.Empty );
            patientDetailsOrderedList.Add( APVCD1, string.Empty );
            patientDetailsOrderedList.Add( APVCD2, string.Empty );
            patientDetailsOrderedList.Add( APVCD3, string.Empty );
            patientDetailsOrderedList.Add( APVCD4, string.Empty );
            patientDetailsOrderedList.Add( APVCD5, string.Empty );
            patientDetailsOrderedList.Add( APVCD6, string.Empty );
            patientDetailsOrderedList.Add( APVCD7, string.Empty );
            patientDetailsOrderedList.Add( APVCD8, string.Empty );
            patientDetailsOrderedList.Add( APVAM1, 0 );
            patientDetailsOrderedList.Add( APVAM2, 0 );
            patientDetailsOrderedList.Add( APVAM3, 0 );
            patientDetailsOrderedList.Add( APVAM4, 0 );
            patientDetailsOrderedList.Add( APVAM5, 0 );
            patientDetailsOrderedList.Add( APVAM6, 0 );
            patientDetailsOrderedList.Add( APVAM7, 0 );
            patientDetailsOrderedList.Add( APVAM8, 0 );
            patientDetailsOrderedList.Add( APWSIR, WORKSTATION_ID );
            patientDetailsOrderedList.Add( APSECR, string.Empty );
            patientDetailsOrderedList.Add( APORR1, string.Empty );
            patientDetailsOrderedList.Add( APORR2, string.Empty );
            patientDetailsOrderedList.Add( APORR3, string.Empty );
            patientDetailsOrderedList.Add( APNPPD, string.Empty );
            patientDetailsOrderedList.Add( APNPPS, string.Empty );
            patientDetailsOrderedList.Add( APPROCD, string.Empty );
            patientDetailsOrderedList.Add( APPREOPD, string.Empty );
            patientDetailsOrderedList.Add( APCRSF, string.Empty );
            patientDetailsOrderedList.Add( APRSCF01, string.Empty );
            patientDetailsOrderedList.Add( APRSCF02, string.Empty );
            patientDetailsOrderedList.Add( APRSCF03, string.Empty );
            patientDetailsOrderedList.Add( APRSCF04, string.Empty );
            patientDetailsOrderedList.Add( APRSCF05, string.Empty );
            patientDetailsOrderedList.Add( APRSCF06, string.Empty );
            patientDetailsOrderedList.Add( APRSCF07, string.Empty );
            patientDetailsOrderedList.Add( APRSCF08, string.Empty );
            patientDetailsOrderedList.Add( APRSCF09, string.Empty );
            patientDetailsOrderedList.Add( APRSCF10, string.Empty );
            patientDetailsOrderedList.Add( APRSID, string.Empty );
            patientDetailsOrderedList.Add( APRSID02, string.Empty );
            patientDetailsOrderedList.Add( APRSID03, string.Empty );
            patientDetailsOrderedList.Add( APRSID04, string.Empty );
            patientDetailsOrderedList.Add( APRSID05, string.Empty );
            patientDetailsOrderedList.Add( APRSID06, string.Empty );
            patientDetailsOrderedList.Add( APRSID07, string.Empty );
            patientDetailsOrderedList.Add( APRSID08, string.Empty );
            patientDetailsOrderedList.Add( APRSID09, string.Empty );
            patientDetailsOrderedList.Add( APRSID10, string.Empty );
            patientDetailsOrderedList.Add( APNHACF, string.Empty );
            patientDetailsOrderedList.Add( APRCRP, string.Empty );
            patientDetailsOrderedList.Add( APPLOS, string.Empty );
            patientDetailsOrderedList.Add( APLWBS, string.Empty );
            patientDetailsOrderedList.Add( APLWFC, string.Empty );
            patientDetailsOrderedList.Add( APPLS, string.Empty );
            patientDetailsOrderedList.Add( APTOB, string.Empty );
            patientDetailsOrderedList.Add( APRGTP, string.Empty);
            patientDetailsOrderedList.Add( APNBFG, string.Empty );
            patientDetailsOrderedList.Add( APBTFG, string.Empty);
            patientDetailsOrderedList.Add( APFILL, string.Empty );
            patientDetailsOrderedList.Add( APHCPCCOD, string.Empty);
            patientDetailsOrderedList.Add( APMDFL, string.Empty );

            patientDetailsOrderedList.Add(APOC09, string.Empty);
            patientDetailsOrderedList.Add(APOC10, string.Empty);
            patientDetailsOrderedList.Add(APOC11, string.Empty);
            patientDetailsOrderedList.Add(APOC12, string.Empty);
            patientDetailsOrderedList.Add(APOC13, string.Empty);
            patientDetailsOrderedList.Add(APOC14, string.Empty);
            patientDetailsOrderedList.Add(APOC15, string.Empty);
            patientDetailsOrderedList.Add(APOC16, string.Empty);
            patientDetailsOrderedList.Add(APOC17, string.Empty);
            patientDetailsOrderedList.Add(APOC18, string.Empty);
            patientDetailsOrderedList.Add(APOC19, string.Empty);
            patientDetailsOrderedList.Add(APOC20, string.Empty);

            patientDetailsOrderedList.Add(APOA09, 0);
            patientDetailsOrderedList.Add(APOA10, 0);
            patientDetailsOrderedList.Add(APOA11, 0);
            patientDetailsOrderedList.Add(APOA12, 0);
            patientDetailsOrderedList.Add(APOA13, 0);
            patientDetailsOrderedList.Add(APOA14, 0);
            patientDetailsOrderedList.Add(APOA15, 0);
            patientDetailsOrderedList.Add(APOA16, 0);
            patientDetailsOrderedList.Add(APOA17, 0);
            patientDetailsOrderedList.Add(APOA18, 0);
            patientDetailsOrderedList.Add(APOA19, 0);
            patientDetailsOrderedList.Add(APOA20, 0);
            patientDetailsOrderedList.Add(APPGNRN, string.Empty);
            patientDetailsOrderedList.Add(APPADRE1, string.Empty);
            patientDetailsOrderedList.Add(APPADRE2, string.Empty);
            patientDetailsOrderedList.Add(APMADRE1, string.Empty);
            patientDetailsOrderedList.Add(APMADRE2, string.Empty);
            patientDetailsOrderedList.Add(APRACE2, string.Empty);
            patientDetailsOrderedList.Add(APNTNLT1, string.Empty);
            patientDetailsOrderedList.Add(APNTNLT2, string.Empty);
            patientDetailsOrderedList.Add(APETHC2, string.Empty);
            patientDetailsOrderedList.Add(APDESCNT1, string.Empty);
            patientDetailsOrderedList.Add(APDESCNT2, string.Empty);



            patientDetailsOrderedList.Add(APSEXBIRTH, string.Empty);
            
                patientDetailsOrderedList.Add(APFRHSP_, 0); //ITFR
                patientDetailsOrderedList.Add(APFRACCT, 0);
            
        }


        public override ArrayList BuildSqlFrom( Account account, TransactionKeys transactionKeys )
        {
            UpdateColumnValuesUsing( account );

            string insertSql = AddColumnsAndValuesToSqlStatement( patientDetailsOrderedList, "HPADAPMP" );
            SqlStatements.Add( insertSql );

            string deleteSql = BuildDeleteSqlStatementforAliases( account );
            SqlStatements.Add( deleteSql );

            foreach ( Name alias in account.Patient.Aliases )
            {
                string insertSqlForAliasNames =
                    BuildSqlForAliasNames( account, alias );
                SqlStatements.Add( insertSqlForAliasNames );
            }

           
            if (account.AuthorizeAdditionalPortalUsers.IsYes)
            {
                account.AuthorizedAdditionalPortalUsers =
                    AssignSequenceNumbersToAuthorizedPortalUsers(account.AuthorizedAdditionalPortalUsers);

                foreach (var AU in account.AuthorizedAdditionalPortalUsers)
                {
                    string insertSqlForAuthorizedUsers =
                        BuildSqlForAuthorizedUsers(account, AU.Value);
                    SqlStatements.Add(insertSqlForAuthorizedUsers);

                }
            }

            var additionalRacesFeatureManager = new AdditionalRacesFeatureManager();

            if (additionalRacesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account))
            {
                string deleteSqlForAdditionalRaceCodes = BuildDeleteSqlForAdditionalRaceCodes(account);
                SqlStatements.Add(deleteSqlForAdditionalRaceCodes);

                if (account.Patient.Race3 != null && !String.IsNullOrEmpty(account.Patient.Race3.Code) &&
                    (account.Patient.Race3.Code != "00"))
                {

                    var insertSqlForAdditionalRaceCodes =
                        BuildSqlForAdditionalRaceCodes((int) account.Facility.Oid,
                            (int) account.Patient.MedicalRecordNumber, 3, account.Patient.Race3);
                    SqlStatements.Add(insertSqlForAdditionalRaceCodes);
                }

                if (account.Patient.Race4 != null && !String.IsNullOrEmpty(account.Patient.Race4.Code) &&
                    (account.Patient.Race4.Code != "00"))
                {

                    var insertSqlForAdditionalRaceCodes =
                        BuildSqlForAdditionalRaceCodes((int) account.Facility.Oid,
                            (int) account.Patient.MedicalRecordNumber, 4, account.Patient.Race4);
                    SqlStatements.Add(insertSqlForAdditionalRaceCodes);
                }

                if (account.Patient.Race5 != null && !String.IsNullOrEmpty(account.Patient.Race5.Code) &&
                    (account.Patient.Race5.Code != "00"))
                {

                    var insertSqlForAdditionalRaceCodes =
                        BuildSqlForAdditionalRaceCodes((int) account.Facility.Oid,
                            (int) account.Patient.MedicalRecordNumber, 5, account.Patient.Race5);
                    SqlStatements.Add(insertSqlForAdditionalRaceCodes);
                }
            }

            if ( account.KindOfVisit.IsPreRegistrationPatient )
            {
                string pregnancyIndicatorSql = BuildSqlForPregnancyIndicator( account );
                SqlStatements.Add( pregnancyIndicatorSql );
            }
            return SqlStatements;
        }

        private Dictionary<int, AuthorizedAdditionalPortalUser> AssignSequenceNumbersToAuthorizedPortalUsers(Dictionary<int, AuthorizedAdditionalPortalUser> authorizedAdditionalPortalUsers)
        {
            foreach (var AU in authorizedAdditionalPortalUsers)
            {
                if (AU.Value.SequenceNumber <= 0)
                {
                    var nextSequenceNumber =
                        GetMaxSequenceNumberForAuthorizedAdditionalPortalUsers(authorizedAdditionalPortalUsers);
                    authorizedAdditionalPortalUsers[AU.Key].SequenceNumber = nextSequenceNumber;
                }
            }

            return authorizedAdditionalPortalUsers;
        }

        private string BuildSqlForAuthorizedUsers(Account account, AuthorizedAdditionalPortalUser AU)
        {
            ReinitializeColumnsAndValues();
            InitializeAuthorizedUserColumnValues();
            UpdateColumnValuesForAuthorizedUsersUsing(account, AU);
            string insertSqlForAuthorizedUsers = AddColumnsAndValuesToSqlStatement(authorizedUSersOrderedList, "HPADAPAUP");

            return insertSqlForAuthorizedUsers;
        }


        private string BuildDeleteSqlStatementforAliases( Account account )
        {
            InitializeDeleteOrderedList();
            UpdateDeleteOrderedListForAliasNames( account );
            string deleteSql = AddColumnsAndValuesToDeleteSqlStatement( deleteOrderedList, "NMNHAKAP" );

            return deleteSql;
        }
        private string BuildDeleteSqlForAdditionalRaceCodes(Account account)
        {
            InitializeDeleteOrderedListForAdditionalRaceCode();
            UpdateDeleteOrderedListForAdditionalRaceCodes(account);
            string deleteSql = AddColumnsAndValuesToDeleteSqlStatement(deleteOrderedListForAdditionalRaceCodes, "HPRAAUXP ");

            return deleteSql;
        }
        private string BuildSqlForAliasNames( Account account, Name alias )
        {
            ReinitializeColumnsAndValues();
            InitializeAliasNamesColumnValues();
            UpdateColumnValuesForAliasNamesUsing( account, alias );
            string insertSqlForAliasNames = AddColumnsAndValuesToSqlStatement( aliasNamesOrderedList, "NMNHAKAP" );

            return insertSqlForAliasNames;
        }

        private string BuildSqlForPregnancyIndicator( Account account )
        {
            ReinitializeColumnsAndValues();
            InitializePregnancyIndicatorColumnValues();
            UpdateColumnValuesForPregnancyIndicatorUsing( account );
            string insertSqlForPregnancyIndicator = AddColumnsAndValuesToSqlStatement( pregnancyIndicatorOrderedList,
                                                                                      "HPTMPADP" );

            return insertSqlForPregnancyIndicator;
        }

        private string BuildSqlForAdditionalRaceCodes(int facilityID, int MRN, int SequenceNumber, Race race)
        {
            ReinitializeColumnsAndValues();
            InitializeAdditionalRaceCodesColumnValues();
            UpdateColumnValuesForAdditionalRaceCodesUsing(facilityID, MRN,
                SequenceNumber, race);
            string insertSqlForAdditionalRaceCodes =
                AddColumnsAndValuesToSqlStatement(additionalRaceCodesOrderedList, "HPRAAUXP");

            return insertSqlForAdditionalRaceCodes;
        }

        private void InitializeDeleteOrderedList()
        {
            deleteOrderedList.Clear();
            deleteOrderedList.Add( AKHSP_, 0 );
            deleteOrderedList.Add( AKMRC_, 0 );
        }
        private void InitializeDeleteOrderedListForAdditionalRaceCode()
        {
            deleteOrderedListForAdditionalRaceCodes.Clear();
            deleteOrderedListForAdditionalRaceCodes.Add(RAHSP, 0);
            deleteOrderedListForAdditionalRaceCodes.Add(RAMR, 0);
        }
        private void InitializeAliasNamesColumnValues()
        {
            aliasNamesOrderedList.Clear();
            aliasNamesOrderedList.Add( AKHSP_, 0 );
            aliasNamesOrderedList.Add( AKMRC_, 0 );
            aliasNamesOrderedList.Add( AKPLNM, string.Empty );
            aliasNamesOrderedList.Add( AKPFNM, string.Empty );
            aliasNamesOrderedList.Add( AKTITL, string.Empty );
            aliasNamesOrderedList.Add( AKEDAT, 0 );
            aliasNamesOrderedList.Add( AKSECF, string.Empty );
        }
        private void InitializeAuthorizedUserColumnValues()
        {
            authorizedUSersOrderedList.Clear();
            authorizedUSersOrderedList.Add(AUAPIDID, string.Empty);
            authorizedUSersOrderedList.Add(AUAPHSP, 0);
            authorizedUSersOrderedList.Add(AUAPMRC, 0);
            authorizedUSersOrderedList.Add(AUAPACCT, 0);
            authorizedUSersOrderedList.Add(AUAPINL, LOG_NUMBER);
            authorizedUSersOrderedList.Add(AUAPSEQ_, 0);
            authorizedUSersOrderedList.Add(AUAPOPTFLG, string.Empty);
            authorizedUSersOrderedList.Add(AUAPRR_, PATIENT_RECORD_NUMBER);

            authorizedUSersOrderedList.Add(AUAPLNAME, string.Empty);
            authorizedUSersOrderedList.Add(AUAPFNAME, string.Empty);
            authorizedUSersOrderedList.Add(AUAPDOB, 0);
            authorizedUSersOrderedList.Add(AUAPEMAIL, string.Empty);
            authorizedUSersOrderedList.Add(AUAPDELFLG, string.Empty);
            authorizedUSersOrderedList.Add(AUAPTCRD, 0); 
            authorizedUSersOrderedList.Add(AUAPTTME, 0);
            
            
        }
        private void InitializePregnancyIndicatorColumnValues()
        {
            pregnancyIndicatorOrderedList.Clear();
            pregnancyIndicatorOrderedList.Add( TMHSP_, 0 );
            pregnancyIndicatorOrderedList.Add( TMMRC_, 0 );
            pregnancyIndicatorOrderedList.Add( TMACCT, 0 );
            pregnancyIndicatorOrderedList.Add( TMPRGI, string.Empty );
        }

        private void InitializeAdditionalRaceCodesColumnValues()
        {
            additionalRaceCodesOrderedList.Clear();
            additionalRaceCodesOrderedList.Add(RAHSP, 0);
            additionalRaceCodesOrderedList.Add(RAMR, 0);
            additionalRaceCodesOrderedList.Add(RASEQ, 0);
            additionalRaceCodesOrderedList.Add(RARACE, string.Empty);
        }

        private void UpdateColumnValuesForAliasNamesUsing( Account account, Name alias )
        {
            i_Account = account;

            if ( account != null )
            {
                AKAHospitalNumber = (int)account.Facility.Oid;
                if ( account.Patient != null )
                {
                    AKAMedicalRecordNumber = (int)account.Patient.MedicalRecordNumber;
                    AKAPatientLastName = alias.LastName;
                    AKAPatientFirstName = alias.FirstName;
                    AKAPatientTitle = alias.Suffix;
                    if ( alias.IsConfidential )
                        AKASecurityFlag = SHORT_FORM_YES;
                    else
                        AKASecurityFlag = SHORT_FORM_NO;

                    AKAEntryDate = ConvertDateToIntInyyMMddFormat( alias.EntryDate );
                }
            }
        }
        public int GetMaxSequenceNumberForAuthorizedAdditionalPortalUsers(Dictionary<int, AuthorizedAdditionalPortalUser> Users)
        {
            int maxSequenceNumber = 0;
            if (Users != null &&
                Users.Count > 0)
            {
                foreach (var seqNum in Users.Values)
                {
                    if (seqNum.SequenceNumber > maxSequenceNumber)
                    {
                        maxSequenceNumber = seqNum.SequenceNumber;
                    }
                }
                maxSequenceNumber = maxSequenceNumber + 1;
            }
            return maxSequenceNumber;
        }

        private void UpdateColumnValuesForAuthorizedUsersUsing(Account account, AuthorizedAdditionalPortalUser AU)
        {
            i_Account = account;
            var tBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
            DateTime facilityDateTime = tBroker.TimeAt(account.Facility.GMTOffset, account.Facility.DSTOffset);

            if (account != null && AU != null)
            { 
                AUTransactionFileId = AUTHORIZEDUSERS_TRANSACTION_ID;
                AUHospitalNumber = (int) account.Facility.Oid;
                AUMedicalRecordNumber = (int) account.Patient.MedicalRecordNumber;
                AUAccountNumber = (int) account.AccountNumber;
                AUSequenceNumber = AU.SequenceNumber;
                AUPortalUserOptinFlag = "Y";
                AULastName = AU.LastName;
                AUFirstName = AU.FirstName;
                AUDateOfBirth = ConvertToInt(AU.DateOfBirth.ToString("Mddyyyy", DateTimeFormatInfo.InvariantInfo)); 
                AUEmailAddress = AU.EmailAddress.Uri;
                AURemoveUserFlag = AU.RemoveUserFlag.Code;
                AUTimeRecordCreation = facilityDateTime;
                AUTransactionDate = facilityDateTime;
            }

        }
        
        private void UpdateColumnValuesForPregnancyIndicatorUsing( Account account )
        {
            if ( account != null )
            {
                PIFacility = (int)account.Facility.Oid;
                PIMedicalRecordNumber = (int)account.Patient.MedicalRecordNumber;
                PIAccountNumber = (int)account.AccountNumber;
                if ( account.Pregnant != null )
                {
                    PreAdmit_PregnancyIndicator = account.Pregnant.Code.Trim();
                }
            }
        }

        private void UpdateColumnValuesForAdditionalRaceCodesUsing(int facilityID, int MRN, int SequenceNumber,
            Race race)
        {

            AdditionalRaceCodeFacility = facilityID;
            AdditionalRaceCodeMedicalRecordNumber = MRN;
            AdditionalRaceCodeSequenceNumber = SequenceNumber;
            AdditionalRaceCode = race.Code.Trim();
        }

        private void UpdateDeleteOrderedListForAliasNames( Account account )
        {
            i_Account = account;

            if ( account != null )
            {
                DeleteAKAHospitalNumber = (int)account.Facility.Oid;
                if ( account.Patient != null )
                {
                    DeleteAKAMedicalRecordNumber = (int)account.Patient.MedicalRecordNumber;
                }
            }
        }
        private void UpdateDeleteOrderedListForAdditionalRaceCodes(Account account)
        {
            i_Account = account;

            if (account != null)
            {
                DeleteAdditionalRaceCodesHospitalNumber = (int)account.Facility.Oid;
                if (account.Patient != null)
                {
                    DeleteAdditionalRaceCodesMedicalRecordNumber = (int)account.Patient.MedicalRecordNumber;
                }
            }
        }
        #endregion

        #region Private Methods

        private void ReadPatientEmploymentAddress()
        {
            Address address = i_Account.Patient.Employment.Employer.PartyContactPoint.Address;

            if ( address != null )
            {
                EmployerAddress = FormatAddress( address.Address1 + address.Address2 );

                EmployerCity = FormattedCity( address.City );

                if ( address.State != null )
                {
                    PatientEmployerStateCode = address.State.Code;
                }

                if ( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
                {
                    PatientEmployerZip = address.ZipCode.ZipCodePrimary.Trim();
                }

                if ( address.ZipCode != null && address.ZipCode.ZipCodeExtendedZeroPadded != null )
                {
                    PatientEmployerZipExt = address.ZipCode.ZipCodeExtendedZeroPadded;
                }

                if ( address.Country != null )
                {
                    PatientEmployerCountry = address.Country.Code;
                }
            }
        }

        private void ReadPatientLocalAddress()
        {
            Address address = i_Account.Patient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).Address;

            PatientLocalAddressStreet1 = FormatAddressStreet1(address.Address1);

            PatientLocalAddressStreet2 = FormatAddressStreet2(address.Address2);

            PatientLocalAddressCity = FormattedCity( address.City );

            if ( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
            {
                PatientLocalZip = address.ZipCode.ZipCodePrimary;
                ZipCode = address.ZipCode.ZipCodePrimaryAsInt;
            }

            if ( address.ZipCode != null && address.ZipCode.ZipCodeExtended != null )
            {
                PatientLocalZipExt = address.ZipCode.ZipCodeExtendedZeroPadded;
                PatientExtendedZipCode = address.ZipCode.ZipCodeExtendedAsInt;
            }

            if ( address.State != null )
            {
                LocalAddressStateID = address.State.Code;
            }

            if ( address.County != null && !string.IsNullOrEmpty( address.County.Code ) )
            {
                PatientMailingAddressCountyCode = address.County.Code;
            }
            if (address.Country != null)
            {
                PatientCountryCode = address.Country.Code;
            }
        }

        private void ReadPatientPhoneNumber()
        {
            PhoneNumber phoneNumber = i_Account.Patient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).PhoneNumber;

            AreaCode = ConvertToInt( phoneNumber.AreaCode );
            PhoneNumbers = ConvertToInt( phoneNumber.Number );
        }

        private void ReadPermanentPatientAddress()
        {
            Address address = i_Account.Patient.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() ).Address;

            PermanentAddressStreet1 = FormatAddressStreet1( address.Address1 );

            PermanentAddressStreet2 = FormatAddressStreet2( address.Address2 );

            PatientPermAddressCity = FormattedCity( address.City );

            if ( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
            {
                PermanentZipCode = address.ZipCode.ZipCodePrimaryAsInt;
                PatientPermZip = address.ZipCode.ZipCodePrimary;
            }

            if ( address.ZipCode != null && address.ZipCode.ZipCodeExtended != null )
            {
                PermPatientAddressZipExt = address.ZipCode.ZipCodeExtendedAsInt;
                PatientPermZipExt = address.ZipCode.ZipCodeExtendedZeroPadded;
            }

            if ( address.County != null )
            {
                PermPatientAddressCountyCode =
                    address.County.Code;
            }
            if ( address.State != null )
            {
                PatientPermAddressStateCode = address.State.Code;
            }

            if ( address.State != null && address.County != null )
            {
                PatientPermAddressFipsCode = address.FIPSCountyCode;
            }
        }

        private void ReadPermanentPatientPhoneNumber()
        {
            PhoneNumber phoneNumber = i_Account.Patient.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;

            PermanentAreaCode = ConvertToInt( phoneNumber.AreaCode );
            PermanentPhonenumber = ConvertToInt( phoneNumber.Number );
        }

        private void ReadPatientEmployerPhoneNumber()
        {
            if ( i_Account.Patient.Employment != null
                && i_Account.Patient.Employment.Employer != null
                && i_Account.Patient.Employment.Employer.PartyContactPoint != null )
            {
            }
            PhoneNumber phoneNumber = i_Account.Patient.Employment.Employer.PartyContactPoint.PhoneNumber;

            EmployerAreaCode = ConvertToInt( phoneNumber.AreaCode );
            EmployerPhoneNumber = ConvertToInt( phoneNumber.Number );
        }

        private void ReadEmergencyContact2Address()
        {
            Address address = i_Account.EmergencyContact2.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() ).Address;

            RelativesAddress = FormatAddress( address.Address1 + address.Address2 );

            RelativesCity = FormattedCity( address.City );

            if ( address.State != null )
            {
                NearRelativeAddressStateCode = address.State.Code;
            }

            if ( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
            {
                NearRelativeZip = address.ZipCode.ZipCodePrimary;
                RelativesZipCode = address.ZipCode.ZipCodePrimaryAsInt;
            }

            if ( address.ZipCode != null && address.ZipCode.ZipCodeExtended != null )
            {
                NearRelativeZipExt = address.ZipCode.ZipCodeExtendedZeroPadded;
                NearRelativeAddressZipCodeExt = address.ZipCode.ZipCodeExtendedAsInt;
            }
        }

        private void ReadEmergencyContact2PhoneNumber()
        {
            PhoneNumber phoneNumber = i_Account.EmergencyContact2.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;

            RelativesAreaCode = ConvertToInt( phoneNumber.AreaCode );
            RelativesPhoneNumber = ConvertToInt( phoneNumber.Number );
        }

        private void ReadEmergencyContact1Address()
        {
            Address address = i_Account.EmergencyContact1.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() ).Address;

            EmergencyContactAddress = FormatAddress( address.Address1 + address.Address2 );
            EmergencyContactCity = FormattedCity( address.City );

            if ( address.State != null )
            {
                EmcyContactAddrStateCode = address.State.Code;
            }

            if ( address.ZipCode != null && address.ZipCode.ZipCodePrimary != null )
            {
                EmergContactZip = address.ZipCode.ZipCodePrimary;
                EmergencyContactZipCode = address.ZipCode.ZipCodePrimaryAsInt;
            }

            if ( address.ZipCode != null && address.ZipCode.ZipCodeExtended != null )
            {
                EmergContactZipExt = address.ZipCode.ZipCodeExtendedZeroPadded;
                EmcyContactAddrZipExt = address.ZipCode.ZipCodeExtendedAsInt;
            }
        }

        private void ReadEmergencyContact1PhoneNumber()
        {
            PhoneNumber phoneNumber = i_Account.EmergencyContact1.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;

            EmergencyContactAreaCode = ConvertToInt( phoneNumber.AreaCode );
            EmergencyContactPhoneNumb = ConvertToInt( phoneNumber.Number );
        }

        private void ReadPrimaryInsuredEmploymentInfo()
        {
            Employment employment = i_Account.Insurance.CoverageFor
                ( CoverageOrder.PRIMARY_OID ).Insured.Employment;

            if ( employment != null )
            {
                OtherEmployeeID1 = employment.EmployeeID;
                if ( employment.Status != null )
                {
                    OtherEmploymentStatus1 = employment.Status.Code;
                }
                if ( employment.Employer != null )
                {
                    OtherEmployerName1 = employment.Employer.Name;

                    if ( employment.Employer.PartyContactPoint != null )
                    {
                        if ( employment.Employer.PartyContactPoint.Address != null )
                        {
                            ReadPrimaryInsuredEmploymentAddress();
                        }
                    }
                }
            }
        }

        private void ReadPrimaryInsuredEmploymentAddress()
        {
            Address address = i_Account.Insurance.CoverageFor(
                CoverageOrder.PRIMARY_OID ).Insured.Employment.Employer.PartyContactPoint.Address;
            if ( address.State != null )
            {
                OtherEmployerLocation1 = address.City + address.State.Code;
            }
        }

        private void ReadSecondaryInsuredEmploymentInfo()
        {
            Employment employment = i_Account.Insurance.CoverageFor(
                CoverageOrder.SECONDARY_OID ).Insured.Employment;

            if ( employment != null )
            {
                OtherEmployeeID2 = employment.EmployeeID;
                if ( employment.Status != null )
                {
                    OtherEmploymentStatus2 = employment.Status.Code;
                }
                if ( employment.Employer != null )
                {
                    OtherEmpoyerName2 =
                        employment.Employer.Name;

                    if ( employment.Employer.PartyContactPoint != null )
                    {
                        if ( employment.Employer.PartyContactPoint.Address != null )
                        {
                            ReadSecondaryInsuredEmploymentAddress();
                        }
                    }
                }
            }
        }

        private void ReadSecondaryInsuredEmploymentAddress()
        {
            Address address = i_Account.Insurance.CoverageFor(
                CoverageOrder.SECONDARY_OID ).Insured.Employment.Employer.PartyContactPoint.Address;

            if ( address.Address1.Length > 25 )
            {
                OEmployerAddress1 = address.Address1.Substring( 0, 25 );
            }
            else
            {
                OEmployerAddress1 = address.Address1;
            }

            if ( address.Address2.Length > 25 )
            {
                OEmployerAddress2 = address.Address2.Substring( 0, 25 );
            }
            else
            {
                OEmployerAddress2 = address.Address2;
            }

            if ( address.ZipCode != null && address.ZipCode.PostalCode != null )
            {
                OEmployerZip1 = address.ZipCode.PostalCodeAsInt;
                OEmployerZip2 = address.ZipCode.PostalCodeAsInt;
            }

            if ( address.State != null )
            {
                OtherEmployerLocation1 =
                    address.City + address.State.Code;
            }
        }

        private void ReadPBARValueCodesAndValueAmounts()
        {
            ValueCode1 = i_Account.ValueCode1;
            ValueCode2 = i_Account.ValueCode2;
            ValueCode3 = i_Account.ValueCode3;
            ValueCode4 = i_Account.ValueCode4;
            ValueCode5 = i_Account.ValueCode5;
            ValueCode6 = i_Account.ValueCode6;
            ValueCode7 = i_Account.ValueCode7;
            ValueCode8 = i_Account.ValueCode8;

            ValueAmount1 = i_Account.ValueAmount1;
            ValueAmount2 = i_Account.ValueAmount2;
            ValueAmount3 = i_Account.ValueAmount3;
            ValueAmount4 = i_Account.ValueAmount4;
            ValueAmount5 = i_Account.ValueAmount5;
            ValueAmount6 = i_Account.ValueAmount6;
            ValueAmount7 = i_Account.ValueAmount7;
            ValueAmount8 = i_Account.ValueAmount8;
        }

        private void ReadCodedDiagnosis( Account account )
        {
            for ( int x = 0; x < account.CodedDiagnoses.CodedDiagnosises.Count; x++ )
            {
                string colName = "APCD0" + ( x + 1 );
                patientDetailsOrderedList[colName] = account.CodedDiagnoses.CodedDiagnosises[x];
            }
            for ( int x = 0; x < account.CodedDiagnoses.AdmittingCodedDiagnosises.Count; x++ )
            {
                string colName = "APRA0" + ( x + 1 );
                patientDetailsOrderedList[colName] = account.CodedDiagnoses.AdmittingCodedDiagnosises[x];
            }
        }

        private void ReadClinicalResearchStudies( Account account )
        {
            if ( account.ClinicalResearchStudies == null )
            {
                return;
            }

            var researchStudies = account.ClinicalResearchStudies as List<ConsentedResearchStudy>;

            for ( int x = 0; x < researchStudies.Count; x++ )
            {
                if ( researchStudies[x] == null )
                {
                    continue;
                }

                switch ( x )
                {
                    case 0:
                        {
                            const string researchColumnName = "APRSID";
                            const string consentColumnName = "APRSCF01";
                            patientDetailsOrderedList[researchColumnName] = researchStudies[x].ResearchStudy.Code;
                            patientDetailsOrderedList[consentColumnName] = researchStudies[x].ProofOfConsent.Code;
                        }
                        break;
                    case 9:
                        {
                            const string researchColumnName = "APRSID10";
                            const string consentColumnName = "APRSCF10";
                            patientDetailsOrderedList[researchColumnName] = researchStudies[x].ResearchStudy.Code;
                            patientDetailsOrderedList[consentColumnName] = researchStudies[x].ProofOfConsent.Code;
                        }
                        break;
                    default:
                        {
                            string researchColumnName = "APRSID0" + ( x + 1 );
                            string consentColumnName = "APRSCF0" + ( x + 1 );
                            patientDetailsOrderedList[researchColumnName] = researchStudies[x].ResearchStudy.Code;
                            patientDetailsOrderedList[consentColumnName] = researchStudies[x].ProofOfConsent.Code;
                        }
                        break;
                }
            }
        }

        private void ReadConditionCode( IList conditionCodes )
        {
            int index = 1;
            foreach ( ConditionCode conditionCode in conditionCodes )
            {
                string conditionCodeValue = conditionCode.Code;
                switch ( index )
                {
                    case 1:
                        {
                            ConditionCode1 = conditionCodeValue;
                            break;
                        }
                    case 2:
                        {
                            ConditionCode2 = conditionCodeValue;
                            break;
                        }
                    case 3:
                        {
                            ConditionCode3 = conditionCodeValue;
                            break;
                        }
                    case 4:
                        {
                            ConditionCode4 = conditionCodeValue;
                            break;
                        }
                    case 5:
                        {
                            ConditionCode5 = conditionCodeValue;
                            break;
                        }
                    case 6:
                        {
                            ConditionCode6 = conditionCodeValue;
                            break;
                        }
                    case 7:
                        {
                            ConditionCode7 = conditionCodeValue;
                            break;
                        }
                }
                index++;
            }
        }

        /// <summary>
        /// Set the site code if there one on the nursing station. 
        /// Other wise set on if there is one on the 1st clinic.
        /// </summary>
        /// <param name="account"></param>
        private void ReadSiteCode( Account account )
        {
            Location location = account.Location;
            // if there is a nursing station and it has a code...
            if ( location != null &&
                location.NursingStation != null &&
                location.NursingStation.Code.Trim() != string.Empty )
            {
                var locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
                NursingStation ns = locationBroker.NursingStationFor( account.Facility,
                                                                     account.Location.NursingStation.Code );
                // if there is a site code on this nursing station...
                if ( ns != null && ns.SiteCode != null && ns.SiteCode.Trim() != string.Empty )
                {
                    SiteCode = ns.SiteCode;
                }
            }
            // else if there is a clinic
            else if ( account.Clinics != null && account.Clinics.Count > 0 )
            {
                var hc = (HospitalClinic)account.Clinics[0];
                // and if the site code is not blank...
                if (hc != null && !String.IsNullOrEmpty( hc.SiteCode ) )
                {
                    SiteCode = hc.SiteCode;
                }
            }
        }

        private void ReadClinicCodes( Account account /*IList clinicCodes*/)
        {
            int index = 1;
            IList clinicCodes = account.Clinics;

            foreach ( HospitalClinic clinicCode in clinicCodes )
            {
                if ( clinicCode != null )
                {
                    string clinicCodeValue = clinicCode.Code;
                    switch ( index )
                    {
                        case 1:
                            {
                                ClinicCode1 = clinicCodeValue;
                                LastClinicRegistered = clinicCodeValue;
                                break;
                            }
                        case 2:
                            {
                                ClinicCode2 = clinicCodeValue;
                                break;
                            }
                        case 3:
                            {
                                ClinicCode3 = clinicCodeValue;
                                break;
                            }
                        case 4:
                            {
                                ClinicCode4 = clinicCodeValue;
                                break;
                            }
                        case 5:
                            {
                                ClinicCode5 = clinicCodeValue;
                                break;
                            }
                    }
                    index++;
                }
            }
            if (
                // if there is not a clinic which was filled in by the user and
                // this account is either Out patient or Emergency patient
                // fill in a facility specific default for last clinic visited.
                ( clinicCodes == null ||
                 clinicCodes.Count == 0 ||
                 clinicCodes[0] == null ||
                 ( clinicCodes[0] != null &&
                  ( (HospitalClinic)clinicCodes[0] ).Code == string.Empty ) )
                &&
                ( account.KindOfVisit.Code == VisitType.OUTPATIENT ||
                 account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT ||
                 account.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                )
            {
                var hcb = BrokerFactory.BrokerOfType<IHospitalClinicsBroker>();

                HospitalClinic hospitalClinic = hcb.PreTestHospitalClinicFor( account.Facility.Oid );
                if ( hospitalClinic != null )
                {
                    LastClinicRegistered = hospitalClinic.Code;
                }
            }
        }

        private void ReadOccurrenceCode( IList occurrenceCodes )
        {
            int index = 1;
            int occurrenceDate;
            foreach ( OccurrenceCode occurrenceCode in occurrenceCodes )
            {
                string occurrenceCodeValue = occurrenceCode.Code;
                occurrenceDate = ConvertDateToIntInMddyyFormat( occurrenceCode.OccurrenceDate );

                if ( occurrenceCodeValue != string.Empty )
                {
                    switch ( index )
                    {
                        case 1:
                            {
                                // if we put a non-accident/crime occ. in the first bucket, logically move it to the
                                // second bucket.  The first bucket is ALWAYS reserved for accident/crime occ. code

                                if ( !occurrenceCode.IsAccidentCrimeOccurrenceCode() )
                                {
                                    index++;
                                    OccurrenceCodeArray2 = occurrenceCodeValue;
                                    OccurrenceDatesArray2 = occurrenceDate;
                                    break;
                                }
                                else
                                {
                                    OccurrenceCodeArray1 = occurrenceCodeValue;
                                    OccurrenceDatesArray1 = occurrenceDate;
                                    break;
                                }
                            }
                        case 2:
                            {
                                OccurrenceCodeArray2 = occurrenceCodeValue;
                                OccurrenceDatesArray2 = occurrenceDate;
                                break;
                            }
                        case 3:
                            {
                                OccurrenceCodeArray3 = occurrenceCodeValue;
                                OccurrenceDatesArray3 = occurrenceDate;
                                break;
                            }
                        case 4:
                            {
                                OccurrenceCodeArray4 = occurrenceCodeValue;
                                OccurrenceDatesArray4 = occurrenceDate;
                                break;
                            }
                        case 5:
                            {
                                OccurrenceCodeArray5 = occurrenceCodeValue;
                                OccurrenceDatesArray5 = occurrenceDate;
                                break;
                            }
                        case 6:
                            {
                                OccurrenceCodeArray6 = occurrenceCodeValue;
                                OccurrenceDatesArray6 = occurrenceDate;
                                break;
                            }
                        case 7:
                            {
                                OccurrenceCodeArray7 = occurrenceCodeValue;
                                OccurrenceDatesArray7 = occurrenceDate;
                                break;
                            }
                        case 8:
                            {
                                OccurrenceCodeArray8 = occurrenceCodeValue;
                                OccurrenceDatesArray8 = occurrenceDate;
                                break;
                            }
                        case 9:
                            {
                                OccurrenceCodeArray9 = occurrenceCodeValue;
                                OccurrenceDatesArray9 = occurrenceDate;
                                break;
                            }
                        case 10:
                            {
                                OccurrenceCodeArray10 = occurrenceCodeValue;
                                OccurrenceDatesArray10 = occurrenceDate;
                                break;
                            }
                        case 11:
                            {
                                OccurrenceCodeArray11 = occurrenceCodeValue;
                                OccurrenceDatesArray11 = occurrenceDate;
                                break;
                            }
                        case 12:
                            {
                                OccurrenceCodeArray12 = occurrenceCodeValue;
                                OccurrenceDatesArray12 = occurrenceDate;
                                break;
                            }
                        case 13:
                            {
                                OccurrenceCodeArray13 = occurrenceCodeValue;
                                OccurrenceDatesArray13 = occurrenceDate;
                                break;
                            }
                        case 14:
                            {
                                OccurrenceCodeArray14 = occurrenceCodeValue;
                                OccurrenceDatesArray14 = occurrenceDate;
                                break;
                            }
                        case 15:
                            {
                                OccurrenceCodeArray15 = occurrenceCodeValue;
                                OccurrenceDatesArray15 = occurrenceDate;
                                break;
                            }
                        case 16:
                            {
                                OccurrenceCodeArray16 = occurrenceCodeValue;
                                OccurrenceDatesArray16 = occurrenceDate;
                                break;
                            }
                        case 17:
                            {
                                OccurrenceCodeArray17 = occurrenceCodeValue;
                                OccurrenceDatesArray17 = occurrenceDate;
                                break;
                            }
                        case 18:
                            {
                                OccurrenceCodeArray18 = occurrenceCodeValue;
                                OccurrenceDatesArray18 = occurrenceDate;
                                break;
                            }
                        case 19:
                            {
                                OccurrenceCodeArray19 = occurrenceCodeValue;
                                OccurrenceDatesArray19 = occurrenceDate;
                                break;
                            }
                        case 20:
                            {
                                OccurrenceCodeArray20 = occurrenceCodeValue;
                                OccurrenceDatesArray20 = occurrenceDate;
                                break;
                            }
                    }
                }

                index++;
            }
        }

        private void ReadAttendingPhysicianInfo()
        {
            PhysicianRelationship physicianRelationship =
                i_Account.PhysicianRelationshipWithRole( PhysicianRole.Attending().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                AttendingPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber;
                AttendingPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                AttendingPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                AttendingPhysicianLastName =
                    physicianRelationship.Physician.LastName;
                AttendingPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                AttendingPhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;

                AttendingPhysicianPhoneNumber =
                    physicianRelationship.Physician.PhoneNumber.AreaCode +
                    physicianRelationship.Physician.PhoneNumber.Number;
                AttendingPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;

            }
        }

        private void ReadAdmittingPhysicianInfo()
        {

            PhysicianRelationship physicianRelationship =
                i_Account.PhysicianRelationshipWithRole( PhysicianRole.Admitting().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                AdmittingPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber;
                string fullName =
                    physicianRelationship.Physician.LastName + physicianRelationship.Physician.FirstName;
                if ( fullName.Length > 13 )
                {
                    AdmittingPhysicianFullName = fullName.Substring( 0, 13 );
                }
                else
                {
                    AdmittingPhysicianFullName = fullName;
                }

                AdmittingPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                AdmittingPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                AdmittingPhysicianLastName =
                    physicianRelationship.Physician.LastName;

                AdmittingPhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;
                AdmittingPhysicianPhoneNumber =
                    ConvertToInt( physicianRelationship.Physician.PhoneNumber.Number );
                AdmittingPhysicianAreaCode =
                    ConvertToInt( physicianRelationship.Physician.PhoneNumber.AreaCode );
                AdmittingPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                AdmittingPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadReferringPhysicianInfo()
        {
            PhysicianRelationship physicianRelationship =
                i_Account.PhysicianRelationshipWithRole( PhysicianRole.Referring().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                ReferringPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber;

                string fullName =
                    physicianRelationship.Physician.LastName + physicianRelationship.Physician.FirstName;
                if ( fullName.Length > 13 )
                {
                    ReferringPhysicianFullName = fullName.Substring( 0, 13 );
                }
                else
                {
                    ReferringPhysicianFullName = fullName;
                }
                ReferringPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                ReferringPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                ReferringPhysicianLastName =
                    physicianRelationship.Physician.LastName;

                if ( physicianRelationship.Physician.UPIN != null )
                {
                    ReferringPhysicianUPINNumber =
                        physicianRelationship.Physician.UPIN;
                }
                else
                {
                    ReferringPhysicianUPINNumber = string.Empty;
                }

                if ( physicianRelationship.Physician.PhoneNumber != null )
                {
                    ReferringPhysicianPhoneNumber =
                        physicianRelationship.Physician.PhoneNumber.AreaCode +
                        physicianRelationship.Physician.PhoneNumber.Number;
                }

                ReferringPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                ReferringPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadOperatingPhysicianInfo()
        {
            PhysicianRelationship physicianRelationship =
                i_Account.PhysicianRelationshipWithRole( PhysicianRole.Operating().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                OperatingPhysicianNumber =
                    physicianRelationship.Physician.PhysicianNumber;
                OperatingPhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                OperatingPhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                OperatingPhysicianLastName =
                    physicianRelationship.Physician.LastName;
                OperatingPhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                OperatingPhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;
                OperatingPhysicianPhoneNumber =
                    physicianRelationship.Physician.PhoneNumber.AreaCode +
                    physicianRelationship.Physician.PhoneNumber.Number;
                OperatingPhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadPrimaryCarePhysicianInfo()
        {
            PhysicianRelationship physicianRelationship =
                i_Account.PhysicianRelationshipWithRole( PhysicianRole.PrimaryCare().Role() );
            if ( physicianRelationship != null &&
                physicianRelationship.Physician != null )
            {
                this.PrimaryCarePhysicianNumber =
                    physicianRelationship.Physician.FormattedPhysicianNumber;
                this.PrimaryCarePhysicianMiddleInitial =
                    physicianRelationship.Physician.Name.MiddleInitial;
                this.PrimaryCarePhysicianFirstName =
                    physicianRelationship.Physician.FirstName;
                this.PrimaryCarePhysicianLastName =
                    physicianRelationship.Physician.LastName;
                this.PrimaryCarePhysicianNationalProviderId =
                    physicianRelationship.Physician.NPI;
                this.PrimaryCarePhysicianUPINNumber =
                    physicianRelationship.Physician.UPIN;
                this.PrimaryCarePhysicianPhoneNumber =
                    physicianRelationship.Physician.PhoneNumber.AreaCode +
                    physicianRelationship.Physician.PhoneNumber.Number;
                this.PrimaryCarePhysicianStateLicense =
                    physicianRelationship.Physician.StateLicense;
            }
        }

        private void ReadOccurrenceSpanCodeInfo()
        {
            if ( i_Account.OccurrenceSpans != null )
            {
                if ( i_Account.OccurrenceSpans.Count > 0
                    && i_Account.OccurrenceSpans[0] != null )
                {
                    if ( ( (OccurrenceSpan)i_Account.OccurrenceSpans[0] ).SpanCode != null )
                    {
                        OccurranceSpanCodeOne =
                            ( (OccurrenceSpan)i_Account.OccurrenceSpans[0] ).SpanCode.Code;
                    }
                    OccurranceSpanFromDateOne =
                        ( (OccurrenceSpan)i_Account.OccurrenceSpans[0] ).FromDate;
                    OccurranceSpanToDateOne =
                        ( (OccurrenceSpan)i_Account.OccurrenceSpans[0] ).ToDate;
                    if ( ( (OccurrenceSpan)i_Account.OccurrenceSpans[0] ).Facility != null )
                    {
                        PreviousFacility =
                            ( (OccurrenceSpan)i_Account.OccurrenceSpans[0] ).Facility;
                    }
                }
                if ( i_Account.OccurrenceSpans.Count > 1
                    && i_Account.OccurrenceSpans[1] != null )
                {
                    if ( ( (OccurrenceSpan)i_Account.OccurrenceSpans[1] ).SpanCode != null )
                    {
                        OccurranceSpanCodeSecond =
                            ( (OccurrenceSpan)i_Account.OccurrenceSpans[1] ).SpanCode.Code;
                    }
                    OccurranceSpanFromDateSecond =
                        ( (OccurrenceSpan)i_Account.OccurrenceSpans[1] ).FromDate;
                    OccurranceSpanToDateSecond =
                        ( (OccurrenceSpan)i_Account.OccurrenceSpans[1] ).ToDate;
                }
            }
        }

        private void ReadMothersInfo()
        {
            if ( i_Account.Patient.MothersAccount != null )
            {
                MothersAccountNumber = i_Account.Patient.MothersAccount.AccountNumber;
            }
            MothersName = i_Account.Patient.MothersName;
            MothersDateOfBirth = i_Account.Patient.MothersDateOfBirth;
        }

        private void ReadFathersInfo()
        {
            FathersName = i_Account.Patient.FathersName;
            FathersDateOfBirth = i_Account.Patient.FathersDateOfBirth;
        }

        private void ReadConfidentialityAndOptOutInfo()
        {
            if ( i_Account.ConfidentialityCode != null && i_Account.ConfidentialityCode.Code != String.Empty )
            {
                ConfidentialityCode = i_Account.ConfidentialityCode.Code.Trim();
            }
            //has to be refactored
            if ( ( i_Account.OptOutName ) ||
                ( i_Account.OptOutLocation ) ||
                ( i_Account.OptOutHealthInformation ) ||
                ( i_Account.OptOutReligion ) )
            {
                optOutInfo = SHORT_FORM_YES;
            }
            else
            {
                optOutInfo = SHORT_FORM_NO;
            }

            ConcatenateOptOutInfo( !i_Account.OptOutName );
            ConcatenateOptOutInfo( !i_Account.OptOutLocation );
            ConcatenateOptOutInfo( !i_Account.OptOutHealthInformation );
            ConcatenateOptOutInfo( !i_Account.OptOutReligion );
            ConfidentialityAndOptOut = optOutInfo;
        }

        private void ConcatenateOptOutInfo( bool optOut )
        {
            if ( optOut )
            {
                optOutInfo += SHORT_FORM_YES;
            }
            else
            {
                optOutInfo += SHORT_FORM_NO;
            }
        }

        #endregion

        #region Public Properties

        internal string PreDischargeFlag
        {
            set { patientDetailsOrderedList[APPRED] = value; }
        }

        internal string TransactionFileId
        {
            set { patientDetailsOrderedList[APIDID] = value; }
        }

        internal string UserSecurityCode
        {
            set { patientDetailsOrderedList[APSEC2] = value; }
        }

        private int PatientDemographicsKey
        {
            set { patientDetailsOrderedList[APKDEM] = value; }
        }

        #endregion

        #region  Private Properties

        private string TypeOfRegistration
        {
            set { patientDetailsOrderedList[APTREG] = value; }
        }

        private string IsShortRegistered
        {
            set { patientDetailsOrderedList[APRGTP] = value; }
        }

        private string IsNewbornRegistered
        {
            set { patientDetailsOrderedList[APNBFG] = value; }
        }

        private string IsBirthTimeEntered
        {
            set { patientDetailsOrderedList[APBTFG] = value; }
        }

        private int DaysSinceAdmission
        {
            set { patientDetailsOrderedList[APPAUX] = value; }
        }

        private int HospitalNumber
        {
            set { patientDetailsOrderedList[APHSP_] = value; }
        }

        private int MedicalRecordNumber
        {
            set { patientDetailsOrderedList[APMRC_] = value; }
        }

        private int AccountNumber
        {
            set { patientDetailsOrderedList[APACCT] = value; }
        }

        private int GuarantorNumber
        {
            set { patientDetailsOrderedList[APGAR_] = value; }
        }

        private string LastName
        {
            set { patientDetailsOrderedList[APPLNM] = value; }
        }

        private string FirstName
        {
            set { patientDetailsOrderedList[APPFNM] = value; }
        }

        private string Sex
        {
            set { patientDetailsOrderedList[APSEX] = value; }
        }

        private string BirthSex
        {
            set { patientDetailsOrderedList[APSEXBIRTH] = value; }
        }

        private long TransferFromHospitalNumber //SATX
        {
            set { patientDetailsOrderedList[APFRHSP_] = value; }
        }
        private long TransferFromAccountNumber //SATX
        {
            set { patientDetailsOrderedList[APFRACCT] = value; }
        }

        private string Ethnicity2
        {
            set { patientDetailsOrderedList[APETHC2] = value; }
        }
        private string Descent
        {
            set { patientDetailsOrderedList[APDESCNT1] = value; }
        }
        private string Descent2
        {
            set { patientDetailsOrderedList[APDESCNT2] = value; }
        }

        private string Races
        {
            set { patientDetailsOrderedList[APRACE] = value; }
        }
        private string Race2
        {
            set { patientDetailsOrderedList[APRACE2] = value; }
        }
        private string Nationality
        {
            set { patientDetailsOrderedList[APNTNLT1] = value; }
        }
        private string Nationality2
        {
            set { patientDetailsOrderedList[APNTNLT2] = value; }
        }

        private string AgeAtAdmission
        {
            set { patientDetailsOrderedList[APAGE] = value; }
        }

        private DateTime DateOfBirth
        {
            set { patientDetailsOrderedList[APDOB] = ConvertDateToIntInMddyyFormat( value ); }
        }
        private DateTime TimeOfBirth
        {
            set { patientDetailsOrderedList[APTOB] = ConvertTimeToStringInHHmmFormat( value ); }
        }
        private DateTime AdmissionDate
        {
            set { patientDetailsOrderedList[APADMT] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private DateTime AdmittingTime
        {
            set { patientDetailsOrderedList[APADTM] = ConvertTimeToIntInHHmmFormat( value ); }
        }

        private DateTime ArrivalTime
        {
            set { patientDetailsOrderedList[APAVTM] = ConvertTimeToStringInHHmmFormat( value ); }
        }

        private string Ethnicities
        {
            set { patientDetailsOrderedList[APETHC] = value; }
        }

        private string PatientType
        {
            set { patientDetailsOrderedList[APPTYP] = value; }
        }

        private string MedicalServiceCode
        {
            set { patientDetailsOrderedList[APMSV] = value; }
        }
        private string PatientPortalOptIn
        {
            set { patientDetailsOrderedList[APFILL] = value; }
        }

        private string APMDFLValue
        {
            set { patientDetailsOrderedList[APMDFL] = value; }
        }
        private string CptCodes
        {
            set { patientDetailsOrderedList[APHCPCCOD] = value; }
        }
        private int PatientAccountsKey
        {
            set { patientDetailsOrderedList[APKACT] = value; }
        }

        private string PlaceOfBirth
        {
            set { patientDetailsOrderedList[APPOB] = value; }
        }

        private string SocialSecurityNumbers
        {
            set { patientDetailsOrderedList[APSSN] = value; }
        }

        private DateTime TransactionDate
        {
            set { patientDetailsOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private DateTime DateOfAdmission
        {
            set { patientDetailsOrderedList[APLAD] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private string ReligionCode
        {
            set { patientDetailsOrderedList[APRLGN] = value; }
        }
        private string PatientLocalAddress
        {
            set { patientDetailsOrderedList[APPADR] = value; }
        }
        private string PatientLocalAddressStreet1
        {
            set { patientDetailsOrderedList[APPADRE1] = value; }
        }

        private string PatientLocalAddressStreet2
        {
            set { patientDetailsOrderedList[APPADRE2] = value; }
        }

        private string PatientLocalAddressCity
        {
            set { patientDetailsOrderedList[APPCIT] = value; }
        }

        private string LocalAddressStateID
        {
            set { patientDetailsOrderedList[APPSTE] = value; }
        }

        private int ZipCode
        {
            set { patientDetailsOrderedList[APPZIP] = value; }
        }

        private int PatientExtendedZipCode
        {
            set { patientDetailsOrderedList[APPZP4] = value; }
        }

        private int DoNotResuscitate
        {
            set { patientDetailsOrderedList[APPCNY] = value; }
        }

        private string PatientMailingAddressCountyCode
        {
            set { patientDetailsOrderedList[APPCCD] = value; }
        }

        private int AreaCode
        {
            set { patientDetailsOrderedList[APPACD] = value; }
        }

        private int PhoneNumbers
        {
            set { patientDetailsOrderedList[APPPH_] = value; }
        }

        private string CellPhoneNumber
        {
            set { patientDetailsOrderedList[APPCPH] = value; }
        }
        private string PermanentAddress
        {
            set { patientDetailsOrderedList[APMADR] = value; }
        }
        private string PermanentAddressStreet1
        {
            set { patientDetailsOrderedList[APMADRE1] = value; }
        }

        private string PermanentAddressStreet2
        {
            set { patientDetailsOrderedList[APMADRE2] = value; }
        }

        private string PatientPermAddressCity
        {
            set { patientDetailsOrderedList[APMCIT] = value; }
        }

        private string PatientPermAddressStateCode
        {
            set { patientDetailsOrderedList[APMSTE] = value; }
        }

        
        private string PatientPermAddressFipsCode
        {
            set { patientDetailsOrderedList[APCNTY] = value; }
        }
        private int PermanentZipCode
        {
            set { patientDetailsOrderedList[APMZIP] = value; }
        }

        private int PermPatientAddressZipExt
        {
            set { patientDetailsOrderedList[APMZP4] = value; }
        }

        private string PermPatientAddressCountyCode
        {
            set { patientDetailsOrderedList[APMCNY] = value; }
        }

        private int PermanentAreaCode
        {
            set { patientDetailsOrderedList[APMACD] = value; }
        }

        private int PermanentPhonenumber
        {
            set { patientDetailsOrderedList[APMPH_] = value; }
        }

        private string NationalID
        {
            set { patientDetailsOrderedList[APPTID] = value; }
        }

        private string RelativesName
        {
            set { patientDetailsOrderedList[APRNM] = value; }
        }

        private string RelativesAddress
        {
            set { patientDetailsOrderedList[APRADR] = value; }
        }

        private string RelativesCity
        {
            set { patientDetailsOrderedList[APRCIT] = value; }
        }

        private string NearRelativeAddressStateCode
        {
            set { patientDetailsOrderedList[APRSTE] = value; }
        }

        private int RelativesZipCode
        {
            set { patientDetailsOrderedList[APRZIP] = value; }
        }

        private int NearRelativeAddressZipCodeExt
        {
            set { patientDetailsOrderedList[APRZP4] = value; }
        }

        private int RelativesAreaCode
        {
            set { patientDetailsOrderedList[APRACD] = value; }
        }

        private int RelativesPhoneNumber
        {
            set { patientDetailsOrderedList[APRPH_] = value; }
        }

        private string EmergencyContactName
        {
            set { patientDetailsOrderedList[APCNM] = value; }
        }

        private string EmergencyContactAddress
        {
            set { patientDetailsOrderedList[APCADR] = value; }
        }

        private string EmergencyContactCity
        {
            set { patientDetailsOrderedList[APCCIT] = value; }
        }

        private string EmcyContactAddrStateCode
        {
            set { patientDetailsOrderedList[APCSTE] = value; }
        }

        private int EmergencyContactZipCode
        {
            set { patientDetailsOrderedList[APCZIP] = value; }
        }

        private int EmcyContactAddrZipExt
        {
            set { patientDetailsOrderedList[APCZP4] = value; }
        }

        private int EmergencyContactAreaCode
        {
            set { patientDetailsOrderedList[APCACD] = value; }
        }

        private int EmergencyContactPhoneNumb
        {
            set { patientDetailsOrderedList[APCPH_] = value; }
        }

        private string EmployersName
        {
            set { patientDetailsOrderedList[APWNM] = value; }
        }

        private string EmployerAddress
        {
            set { patientDetailsOrderedList[APWADR] = value; }
        }

        private string EmployerCity
        {
            set { patientDetailsOrderedList[APWCIT] = value; }
        }

        private string PatientEmployerStateCode
        {
            set { patientDetailsOrderedList[APWSTE] = value; }
        }

        private int EmployerAreaCode
        {
            set { patientDetailsOrderedList[APWACD] = value; }
        }

        private int EmployerPhoneNumber
        {
            set { patientDetailsOrderedList[APWPH_] = value; }
        }

        private string MaritalStatusData
        {
            set { patientDetailsOrderedList[APMAR] = value; }
        }

        private string DiagnosisData
        {
            set { patientDetailsOrderedList[APDIAG] = value; }
        }

        private string ProcedureData
        {
            set { patientDetailsOrderedList[APPROCD] = value; }
        }

        private string AccidentTime
        {
            set
            {
                long hourAsNumber;

                if ( value.ToUpper().Equals( "UNKNOWN" ) )
                {
                    hourAsNumber = 99*100;
                }
                else
                {
                    try
                    {
                        hourAsNumber = long.Parse( value );
                        if ( hourAsNumber > 0 && hourAsNumber <= 23 )
                        {
                            hourAsNumber = hourAsNumber*100;
                        }
                    }
                    catch
                    {
                        hourAsNumber = 99*100;
                    }
                }
                patientDetailsOrderedList[APATIM] = hourAsNumber;
            }
        }

        private DateTime TimeOfAdmission
        {
            set { patientDetailsOrderedList[APLAT] = ConvertTimeToIntInHHmmFormat( value ); }
        }

        private string TenetCare
        {
            set { patientDetailsOrderedList[APTECR] = value; }
        }

        private string ScheduleCode
        {
            set { patientDetailsOrderedList[APSCHD] = value; }
        }

        private string PreopDate
        {
            set { patientDetailsOrderedList[APPREOPD] = value; }
        }

        private string FacilityDeterminedFlag
        {
            set { patientDetailsOrderedList[APWRNF] = value; }
        }

        private DateTime DischargeDate
        {
            set { patientDetailsOrderedList[APLDD] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private DateTime TimeOfDischarge
        {
            set { patientDetailsOrderedList[APLDT] = ConvertTimeToIntInHHmmFormat( value ); }
        }

        private string NearestRelRelToPatCode
        {
            set { patientDetailsOrderedList[APNRCD] = value; }
        }

        private string EmcyContactRelToPatCode
        {
            set { patientDetailsOrderedList[APCRCD] = value; }
        }

        private string AccidentCode
        {
            set { patientDetailsOrderedList[APACOD] = value; }
        }

        private DateTime AccidentDate
        {
            set { patientDetailsOrderedList[APACDT] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private string EmployeeID
        {
            set { patientDetailsOrderedList[APEEID] = value; }
        }

        private string Occupation
        {
            set { patientDetailsOrderedList[APPOCC] = value; }
        }

        private string EmploymentStatusCode
        {
            set { patientDetailsOrderedList[APESCD] = value; }
        }

        private string PatientLocalZip
        {
            set { patientDetailsOrderedList[APPZPA] = value; }
        }

        private string PatientEmployerZip
        {
            set { patientDetailsOrderedList[APWZPA] = value; }
        }

        private string ClergyVisit
        {
            set { patientDetailsOrderedList[APCLVS] = value; }
        }

        private string EmailAddress
        {
            set { patientDetailsOrderedList[APPEML] = value; }
        }

        private string DriversLicenseData
        {
            set { patientDetailsOrderedList[APPDL_] = value; }
        }

        private string PassportNumber
        {
            set { patientDetailsOrderedList[APPPT_] = value; }
        }

        private string PassportCountry
        {
            set { patientDetailsOrderedList[APICUN] = value; }
        }

        private string MiddleInitial
        {
            set { patientDetailsOrderedList[APPMI] = value; }
        }

        private string OtherEmployeeID1
        {
            set
            {
                if ( value == string.Empty )
                {
                    patientDetailsOrderedList[APEID1] = "00000000000";
                }
                else
                {
                    patientDetailsOrderedList[APEID1] = value;
                }
            }
        }

        private string OtherEmployeeID2
        {
            set
            {
                if ( value == string.Empty )
                {
                    patientDetailsOrderedList[APEID2] = "00000000000";
                }
                else
                {
                    patientDetailsOrderedList[APEID2] = value;
                }
            }
        }

        private string OtherEmploymentStatus1
        {
            set { patientDetailsOrderedList[APESC1] = value; }
        }

        private string OtherEmploymentStatus2
        {
            set { patientDetailsOrderedList[APESC2] = value; }
        }

        private string OtherEmployerName1
        {
            set { patientDetailsOrderedList[APENM1] = value; }
        }

        private string OtherEmpoyerName2
        {
            set { patientDetailsOrderedList[APENM2] = value; }
        }

        private string OtherEmployerLocation1
        {
            set { patientDetailsOrderedList[APELO1] = value; }
        }


        private int AccidentTypeCode
        {
            set { patientDetailsOrderedList[APACTP] = value; }
        }

        private DateTime DateOfLastMaintenance
        {
            set { patientDetailsOrderedList[APLMD] = ConvertDateToIntInMddyyFormat( value ); }
        }


        private string FinancialClassData
        {
            set { patientDetailsOrderedList[APFC] = value; }
        }

        private string PatientGuarantorRelationship
        {
            set { patientDetailsOrderedList[APRRCD] = value; }
        }

        private string OEmployerAddress1
        {
            set { patientDetailsOrderedList[APEA01] = value; }
        }

        private string LastClinicRegistered
        {
            set { patientDetailsOrderedList[APLCL] = value; }
        }

        private string OEmployerAddress2
        {
            set { patientDetailsOrderedList[APEA02] = value; }
        }

        private int OEmployerZip1
        {
            set { patientDetailsOrderedList[APEZ01] = value; }
        }

        private int OEmployerZip2
        {
            set { patientDetailsOrderedList[APEZ02] = value; }
        }

        private string NameSuffix
        {
            set
            {
                patientDetailsOrderedList[APPGNRN] = value;
            }
        }

        private int DateOfBirthCen
        {
            set { patientDetailsOrderedList[APDOB8] = value; }
        }

        private string PatientLocalZipExt
        {
            set { patientDetailsOrderedList[APPZ4A] = value; }
        }

        private string PatientEmployerZipExt
        {
            set { patientDetailsOrderedList[APWZ4A] = value; }
        }

        private string PatientEmployerCountry
        {
            set { patientDetailsOrderedList[APWCUN] = value; }
        }

        private string AKALastName
        {
            set { patientDetailsOrderedList[APAKLN] = value; }
        }

        private string AKAFirstName
        {
            set { patientDetailsOrderedList[APAKFM] = value; }
        }

        private string PatientPermZip
        {
            set { patientDetailsOrderedList[APMZPA] = value; }
        }

        private string PatientPermZipExt
        {
            set { patientDetailsOrderedList[APMZ4A] = value; }
        }

        private string EmergContactZip
        {
            set { patientDetailsOrderedList[APCZPA] = value; }
        }

        private string EmergContactZipExt
        {
            set { patientDetailsOrderedList[APCZ4A] = value; }
        }

        private string NearRelativeZip
        {
            set { patientDetailsOrderedList[APRZPA] = value; }
        }

        private string NearRelativeZipExt
        {
            set { patientDetailsOrderedList[APRZ4A] = value; }
        }

        private string PatientLanguage
        {
            set { patientDetailsOrderedList[APLNGC] = value; }
        }
        private string PatientOtherLanguage
        {
            set
            {
                patientDetailsOrderedList["APPLS"] = value;
            }
        }

        private string PatientCountryCode
        {
            set { patientDetailsOrderedList[APPCUN] = value; }
        }
        private string PregnancyIndicator
        {
            set
            {
                this.patientDetailsOrderedList[APPRGI] = value;
            }
        }
        private string PreAdmit_PregnancyIndicator
        {
            set { pregnancyIndicatorOrderedList[TMPRGI] = value; }
        }

        private int PIFacility
        {
            set { pregnancyIndicatorOrderedList[TMHSP_] = value; }
        }

        private int PIMedicalRecordNumber
        {
            set { pregnancyIndicatorOrderedList[TMMRC_] = value; }
        }

        private int PIAccountNumber
        {
            set { pregnancyIndicatorOrderedList[TMACCT] = value; }
        }
        private int AdditionalRaceCodeFacility
        {
            set { additionalRaceCodesOrderedList[RAHSP] = value; }
        }
        private int AdditionalRaceCodeMedicalRecordNumber
        {
            set { additionalRaceCodesOrderedList[RAMR] = value; }
        }

        private int AdditionalRaceCodeSequenceNumber
        {
            set { additionalRaceCodesOrderedList[RASEQ] = value; }
        }

        private string AdditionalRaceCode 
        {
            set { additionalRaceCodesOrderedList[RARACE] = value; }
        }

        private string ParishCode
        {
            set { patientDetailsOrderedList[APPARC] = value; }
        }

        private string AutoAccidentState
        {
            set { patientDetailsOrderedList[APACST] = value; }
        }

        private string CountryCodeAfter
        {
            set { patientDetailsOrderedList[APACCN] = value; }
        }

        private DateTime DateOfLastMenstruation
        {
            set { patientDetailsOrderedList[APLMEN] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private string IPA
        {
            set { patientDetailsOrderedList[APIPA] = value; }
        }

        private string IPAClinic
        {
            set { patientDetailsOrderedList[APIPAC] = value; }
        }

        private DateTime TimeRecordCreation
        {
            set { patientDetailsOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value ); }
        }


        private string NursingStation
        {
            set { patientDetailsOrderedList[APNS] = value; }
        }

        private string IsolationCode
        {
            set { patientDetailsOrderedList[APISO] = value; }
        }

        private int RoomNumber
        {
            set { patientDetailsOrderedList[APROOM] = value; }
        }

        private string BedNumber
        {
            set { patientDetailsOrderedList[APBED] = value; }
        }

        private string AccomodationCode
        {
            set { patientDetailsOrderedList[APACC] = value; }
        }

        private string AdmittingCategory
        {
            set { patientDetailsOrderedList[APADMC] = value; }
        }

        private string AdmitSourceCode
        {
            set { patientDetailsOrderedList[APPSRC] = value; }
        }

        private string AlternateCareFacility
        {
            set { patientDetailsOrderedList[APNHACF] = value; }
        }

        private string RightCareRightPlace
        {
            set { patientDetailsOrderedList[APRCRP] = value; }
        }

        private string LeftOrStayed
        {
            set { patientDetailsOrderedList[APPLOS] = value; }
        }

        private string LeftWithOutBeingSeen
        {
            set { patientDetailsOrderedList[APLWBS] = value; }
        }

        private string LeftWithOutFinacialClearance
        {
            set { patientDetailsOrderedList[APLWFC] = value; }
        }

        private string BloodlessCode
        {
            set { patientDetailsOrderedList[APBLDL] = value; }
        }

        private string SmokingFlag
        {
            set { patientDetailsOrderedList[APSMOK] = value; }
        }

        private string ValuablesCollectedFlag
        {
            set { patientDetailsOrderedList[APVALU] = value; }
        }

      
        private string ReferralSource
        {
            set { patientDetailsOrderedList[APRSRC] = value; }
        }

        private string ReferralFacility
        {
            set { patientDetailsOrderedList[APFRCD] = value; }
        }

        private string ReferralType
        {
            set { patientDetailsOrderedList[APRTYP] = value; }
        }

        private string ModeOfArrival
        {
            set { patientDetailsOrderedList[APARVC] = value; }
        }

        private string AbstractExists
        {
            set { patientDetailsOrderedList[APABST] = value; }
        }

        private string ReAdmitCode
        {
            set { patientDetailsOrderedList[APRAMC] = value; }
        }

        private string WorkStationId
        {
            set { patientDetailsOrderedList[APIDWS] = value; }
        }

        private string MaidenName
        {
            set { patientDetailsOrderedList[APMNM] = value; }
        }

        private string ClinicalCommentsOne
        {
            set { patientDetailsOrderedList[APCOM1] = value; }
        }

        private string ClinicalCommentsTwo
        {
            set { patientDetailsOrderedList[APCOM2] = value; }
        }

        private string PatientInClinicalStudy
        {
            set { patientDetailsOrderedList[APCRSF] = value; }
        }

        private string ReRegisterFlag
        {
            set { patientDetailsOrderedList[APPUBL] = value; }
        }

        #region Spouce Details

        #endregion

        #region Mothers Account

        private long MothersAccountNumber
        {
            set { patientDetailsOrderedList[APMPT_] = value; }
        }

        private string MothersName
        {
            set { patientDetailsOrderedList[APMONM] = value; }
        }

        private DateTime MothersDateOfBirth
        {
            set { patientDetailsOrderedList[APMDOB] = ConvertDateToIntInMddyyFormat( value ); }
        }

        #endregion

        #region Confidentiality and Opt Out

        private string ConfidentialityAndOptOut
        {
            set { patientDetailsOrderedList[APNPPF] = value; }
        }

        private string ConfidentialityCode
        {
            set { patientDetailsOrderedList[APCNCD] = value; }
        }

        #endregion

        #region Also Known As Details

        private int AKAHospitalNumber
        {
            set { aliasNamesOrderedList[AKHSP_] = value; }
        }

        private int AKAMedicalRecordNumber
        {
            set { aliasNamesOrderedList[AKMRC_] = value; }
        }

        private string AKAPatientLastName
        {
            set { aliasNamesOrderedList[AKPLNM] = value; }
        }

        private string AKAPatientFirstName
        {
            set { aliasNamesOrderedList[AKPFNM] = value; }
        }

        private string AKAPatientTitle
        {
            set { aliasNamesOrderedList[AKTITL] = value; }
        }

        private int AKAEntryDate
        {
            set { aliasNamesOrderedList[AKEDAT] = value; }
        }

        private string AKASecurityFlag
        {
            set { aliasNamesOrderedList[AKSECF] = value; }
        }

        #endregion
        #region Authorized Users Details
        internal string AUTransactionFileId
        {
            set { authorizedUSersOrderedList[AUAPIDID] = value; }
        }
        private int AUHospitalNumber
        {
            set { authorizedUSersOrderedList[AUAPHSP] = value; }
        }

        private int AUMedicalRecordNumber
        {
            set { authorizedUSersOrderedList[AUAPMRC] = value; }
        }
        private int AUAccountNumber
        {
            set { authorizedUSersOrderedList[AUAPACCT] = value; }
        }

        private int AUSequenceNumber
        {
            set { authorizedUSersOrderedList[AUAPSEQ_] = value; }
        }

        private string AUPortalUserOptinFlag
        {
            set { authorizedUSersOrderedList[AUAPOPTFLG] = value; }
        }

        private string AULastName
        {
            set { authorizedUSersOrderedList[AUAPLNAME] = value; }
        }

        private string AUFirstName
        {
            set { authorizedUSersOrderedList[AUAPFNAME] = value; }
        }

        private int AUDateOfBirth
        {
            set { authorizedUSersOrderedList[AUAPDOB] = value; }
        }

        private string AUEmailAddress
        {
            set { authorizedUSersOrderedList[AUAPEMAIL] = value; }
        }

        private string AURemoveUserFlag
        {
            set { authorizedUSersOrderedList[AUAPDELFLG] = value; }
        }
        private DateTime AUTransactionDate
        {
            set { authorizedUSersOrderedList[AUAPTCRD] = ConvertDateToStringInShortyyyyMMddFormat(value); }
        }
        private DateTime AUTimeRecordCreation
        {
            set { authorizedUSersOrderedList[AUAPTTME] = ConvertTimeToIntInHHmmSSFormat(value); }
        }
        #endregion

        #region NPP Details

        private string CurrentNPPVersionNumber
        {
            set { patientDetailsOrderedList[APNPPV] = value; }
        }

        private string CurrentNPPVersionDate
        {
            set { patientDetailsOrderedList[APDTRC] = value; }
        }

        private string NPPSignatureStatus
        {
            set { patientDetailsOrderedList[APNPPS] = value; }
        }

        private string NPPDateSigned
        {
            set { patientDetailsOrderedList[APNPPD] = value; }
        }

        #endregion

        #region Delete Also Known As Details

        private int DeleteAKAHospitalNumber
        {
            set { deleteOrderedList[AKHSP_] = value; }
        }

        private int DeleteAKAMedicalRecordNumber
        {
            set { deleteOrderedList[AKMRC_] = value; }
        }
        private int DeleteAdditionalRaceCodesHospitalNumber
        {
            set { deleteOrderedListForAdditionalRaceCodes[RAHSP] = value; }
        }

        private int DeleteAdditionalRaceCodesMedicalRecordNumber
        {
            set { deleteOrderedListForAdditionalRaceCodes[RAMR] = value; }
        }

        private int DeleteAdditionalRaceHospitalNumber
        {
            set { deleteOrderedList[RAHSP] = value; }
        }

        private int DeleteAdditionalRaceMedicalRecordNumber
        {
            set { deleteOrderedList[RAMR] = value; }
        }
        private string AddChangeFlag
        {
            set { patientDetailsOrderedList[APACFL] = value; }
        }

        #endregion

        #region Occurrance Span Details

        private string OccurranceSpanCodeOne
        {
            set { patientDetailsOrderedList[APSPNC] = value; }
        }

        private DateTime OccurranceSpanFromDateOne
        {
            set { patientDetailsOrderedList[APSPFR] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private DateTime OccurranceSpanToDateOne
        {
            set { patientDetailsOrderedList[APSPTO] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private string OccurranceSpanCodeSecond
        {
            set { patientDetailsOrderedList[APSPN2] = value; }
        }

        private DateTime OccurranceSpanFromDateSecond
        {
            set { patientDetailsOrderedList[APAPFR] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private DateTime OccurranceSpanToDateSecond
        {
            set { patientDetailsOrderedList[APAPTO] = ConvertDateToIntInMddyyFormat( value ); }
        }

        private string PreviousFacility
        {
            set { patientDetailsOrderedList[APINSN] = value; }
        }

        #endregion

        #region Admitting Physician

        private long AdmittingPhysicianNumber
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianNumber - " + value );
                patientDetailsOrderedList[APMDR_] = value;
            }
        }

        private string AdmittingPhysicianFullName
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianFullName - " + value );
                patientDetailsOrderedList[APMNDN] = value;
            }
        }

        private string AdmittingPhysicianLastName
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianLastName - " + value );
                patientDetailsOrderedList[APMNLN] = value;
            }
        }

        private string AdmittingPhysicianFirstName
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianFirstName - " + value );
                patientDetailsOrderedList[APMNFN] = value;
            }
        }

        private string AdmittingPhysicianMiddleInitial
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianMiddleInitial - " + value );
                patientDetailsOrderedList[APMNMN] = value;
            }
        }

        private string AdmittingPhysicianNationalProviderId
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianNationalProviderId - " + value );
                patientDetailsOrderedList[APMNPR] = value;
            }
        }

        private string AdmittingPhysicianUPINNumber
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianUPINNumber - " + value );
                patientDetailsOrderedList[APMNU_] = value;
            }
        }


        // Be careful with these 2 values. 
        // For some reason the admitting Dr's phone number and AC are DECIMAL rather than
        // string like every other dr phone number!!!
        private int AdmittingPhysicianPhoneNumber
        {
            set
            {
                patientDetailsOrderedList[APMNP_] = value;
            }
        }

        private int AdmittingPhysicianAreaCode
        {
            set
            {
                patientDetailsOrderedList[APMNCD] = value;
            }
        }

        private string AdmittingPhysicianStateLicense
        {
            set
            {
                c_log.Debug( "AdmittingPhysicianStateLicense - " + value );
                patientDetailsOrderedList[APMSL_] = value;
            }
        }

        #endregion

        #region Attending Physician

        private long AttendingPhysicianNumber
        {
            set
            {
                c_log.Debug( "AttendingPhysicianNumber - " + value );
                patientDetailsOrderedList[APADR_] = value;
            }
        }

        private string AttendingPhysicianLastName
        {
            set
            {
                c_log.Debug( "AttendingPhysicianLastName - " + value );
                patientDetailsOrderedList[APANLN] = value;
            }
        }

        private string AttendingPhysicianFirstName
        {
            set
            {
                c_log.Debug( "AttendingPhysicianFirstName - " + value );
                patientDetailsOrderedList[APANFN] = value;
            }
        }

        private string AttendingPhysicianMiddleInitial
        {
            set
            {
                c_log.Debug( "AttendingPhysicianMiddleInitial - " + value );
                patientDetailsOrderedList[APANMN] = value;
            }
        }

        private string AttendingPhysicianNationalProviderId
        {
            set
            {
                c_log.Debug( "AttendingPhysicianNationalProviderId - " + value );
                patientDetailsOrderedList[APANPR] = value;
            }
        }

        private string AttendingPhysicianUPINNumber
        {
            set
            {
                c_log.Debug( "AttendingPhysicianUPINNumber - " + value );
                patientDetailsOrderedList[APANU_] = value;
            }
        }

        private string AttendingPhysicianPhoneNumber
        {
            set
            {
                c_log.Debug( "AttendingPhysicianPhoneNumber - " + value );
                patientDetailsOrderedList[APANP_] = value;
            }
        }

        private string AttendingPhysicianStateLicense
        {
            set
            {
                c_log.Debug( "AttendingPhysicianStateLicense - " + value );
                patientDetailsOrderedList[APASL_] = value;
            }
        }

        #endregion

        #region Referring Physician

        private long ReferringPhysicianNumber
        {
            set { patientDetailsOrderedList[APRDR_] = value; }
        }

        private string ReferringPhysicianFullName
        {
            set { patientDetailsOrderedList[APRNDN] = value; }
        }

        private string ReferringPhysicianLastName
        {
            set { patientDetailsOrderedList[APRNLN] = value; }
        }

        private string ReferringPhysicianFirstName
        {
            set { patientDetailsOrderedList[APRNFN] = value; }
        }

        private string ReferringPhysicianMiddleInitial
        {
            set { patientDetailsOrderedList[APRNMN] = value; }
        }

        private string ReferringPhysicianNationalProviderId
        {
            set { patientDetailsOrderedList[APRNPR] = value; }
        }

        private string ReferringPhysicianPhoneNumber
        {
            set { patientDetailsOrderedList[APRNP_] = value; }
        }

        private string ReferringPhysicianUPINNumber
        {
            set { patientDetailsOrderedList[APRNU_] = value; }
        }

        private string ReferringPhysicianStateLicense
        {
            set { patientDetailsOrderedList[APRSL_] = value; }
        }

        #endregion

        #region Operating Physician

        private long OperatingPhysicianNumber
        {
            set { patientDetailsOrderedList[APODR_] = value; }
        }

        private string OperatingPhysicianLastName
        {
            set { patientDetailsOrderedList[APONLN] = value; }
        }

        private string OperatingPhysicianFirstName
        {
            set { patientDetailsOrderedList[APONFN] = value; }
        }

        private string OperatingPhysicianMiddleInitial
        {
            set { patientDetailsOrderedList[APONMN] = value; }
        }

        private string OperatingPhysicianNationalProviderId
        {
            set { patientDetailsOrderedList[APONPR] = value; }
        }

        private string OperatingPhysicianUPINNumber
        {
            set { patientDetailsOrderedList[APONU_] = value; }
        }

        private string OperatingPhysicianPhoneNumber
        {
            set { patientDetailsOrderedList[APONP_] = value; }
        }

        private string OperatingPhysicianStateLicense
        {
            set { patientDetailsOrderedList[APOSL_] = value; }
        }

        #endregion

        #region PrimaryCare Physician
        private string PrimaryCarePhysicianNumber
        {
            set
            {
                this.patientDetailsOrderedList[APTDR_] = value;
            }
        }
        private string PrimaryCarePhysicianLastName
        {
            set
            {
                this.patientDetailsOrderedList[APTNLN] = value;
            }
        }
        private string PrimaryCarePhysicianFirstName
        {
            set
            {
                this.patientDetailsOrderedList[APTNFN] = value;
            }
        }
        private string PrimaryCarePhysicianMiddleInitial
        {
            set
            {
                this.patientDetailsOrderedList[APTNMN] = value;
            }
        }
        private string PrimaryCarePhysicianNationalProviderId
        {
            set
            {
                this.patientDetailsOrderedList[APTNPR] = value;
            }
        }
        private string PrimaryCarePhysicianUPINNumber
        {
            set
            {
                this.patientDetailsOrderedList[APTNU_] = value;
            }
        }
        private string PrimaryCarePhysicianPhoneNumber
        {
            set
            {
                this.patientDetailsOrderedList[APTNP_] = value;
            }
        }
        private string PrimaryCarePhysicianStateLicense
        {
            set
            {
                this.patientDetailsOrderedList[APTSL_] = value;
            }
        }
        #endregion

        #region PBAR Value Codes & Value Amounts

        private string ValueCode1
        {
            set { patientDetailsOrderedList[APVCD1] = value; }
        }

        private string ValueCode2
        {
            set { patientDetailsOrderedList[APVCD2] = value; }
        }

        private string ValueCode3
        {
            set { patientDetailsOrderedList[APVCD3] = value; }
        }

        private string ValueCode4
        {
            set { patientDetailsOrderedList[APVCD4] = value; }
        }

        private string ValueCode5
        {
            set { patientDetailsOrderedList[APVCD5] = value; }
        }

        private string ValueCode6
        {
            set { patientDetailsOrderedList[APVCD6] = value; }
        }

        private string ValueCode7
        {
            set { patientDetailsOrderedList[APVCD7] = value; }
        }

        private string ValueCode8
        {
            set { patientDetailsOrderedList[APVCD8] = value; }
        }

        private decimal ValueAmount1
        {
            set { patientDetailsOrderedList[APVAM1] = value; }
        }

        private decimal ValueAmount2
        {
            set { patientDetailsOrderedList[APVAM2] = value; }
        }

        private decimal ValueAmount3
        {
            set { patientDetailsOrderedList[APVAM3] = value; }
        }

        private decimal ValueAmount4
        {
            set { patientDetailsOrderedList[APVAM4] = value; }
        }

        private decimal ValueAmount5
        {
            set { patientDetailsOrderedList[APVAM5] = value; }
        }

        private decimal ValueAmount6
        {
            set { patientDetailsOrderedList[APVAM6] = value; }
        }

        private decimal ValueAmount7
        {
            set { patientDetailsOrderedList[APVAM7] = value; }
        }

        private decimal ValueAmount8
        {
            set { patientDetailsOrderedList[APVAM8] = value; }
        }

        #endregion

        # region Occurrence Codes and Dates

        private string OccurrenceCodeArray1
        {
            set { patientDetailsOrderedList[APOC01] = value; }
        }

        private string OccurrenceCodeArray2
        {
            set { patientDetailsOrderedList[APOC02] = value; }
        }

        private string OccurrenceCodeArray3
        {
            set { patientDetailsOrderedList[APOC03] = value; }
        }

        private string OccurrenceCodeArray4
        {
            set { patientDetailsOrderedList[APOC04] = value; }
        }

        private string OccurrenceCodeArray5
        {
            set { patientDetailsOrderedList[APOC05] = value; }
        }

        private string OccurrenceCodeArray6
        {
            set { patientDetailsOrderedList[APOC06] = value; }
        }

        private string OccurrenceCodeArray7
        {
            set { patientDetailsOrderedList[APOC07] = value; }
        }

        private string OccurrenceCodeArray8
        {
            set { patientDetailsOrderedList[APOC08] = value; }
        }
        private string OccurrenceCodeArray9
        {
            set { patientDetailsOrderedList[APOC09] = value; }
        }
        private string OccurrenceCodeArray10
        {
            set { patientDetailsOrderedList[APOC10] = value; }
        }
        private string OccurrenceCodeArray11
        {
            set { patientDetailsOrderedList[APOC11] = value; }
        }
        private string OccurrenceCodeArray12
        {
            set { patientDetailsOrderedList[APOC12] = value; }
        }
        private string OccurrenceCodeArray13
        {
            set { patientDetailsOrderedList[APOC13] = value; }
        }
        private string OccurrenceCodeArray14
        {
            set { patientDetailsOrderedList[APOC14] = value; }
        }
        private string OccurrenceCodeArray15
        {
            set { patientDetailsOrderedList[APOC15] = value; }
        }
        private string OccurrenceCodeArray16
        {
            set { patientDetailsOrderedList[APOC16] = value; }
        }
        private string OccurrenceCodeArray17
        {
            set { patientDetailsOrderedList[APOC17] = value; }
        }
        private string OccurrenceCodeArray18
        {
            set { patientDetailsOrderedList[APOC18] = value; }
        }
        private string OccurrenceCodeArray19
        {
            set { patientDetailsOrderedList[APOC19] = value; }
        }
        private string OccurrenceCodeArray20
        {
            set { patientDetailsOrderedList[APOC20] = value; }
        }
        private int OccurrenceDatesArray1
        {
            set { patientDetailsOrderedList[APOA01] = value; }
        }

        private int OccurrenceDatesArray2
        {
            set { patientDetailsOrderedList[APOA02] = value; }
        }

        private int OccurrenceDatesArray3
        {
            set { patientDetailsOrderedList[APOA03] = value; }
        }

        private int OccurrenceDatesArray4
        {
            set { patientDetailsOrderedList[APOA04] = value; }
        }

        private int OccurrenceDatesArray5
        {
            set { patientDetailsOrderedList[APOA05] = value; }
        }

        private int OccurrenceDatesArray6
        {
            set { patientDetailsOrderedList[APOA06] = value; }
        }

        private int OccurrenceDatesArray7
        {
            set { patientDetailsOrderedList[APOA07] = value; }
        }

        private int OccurrenceDatesArray8
        {
            set { patientDetailsOrderedList[APOA08] = value; }
        }
        private int OccurrenceDatesArray9
        {
            set { patientDetailsOrderedList[APOA09] = value; }
        }
        private int OccurrenceDatesArray10
        {
            set { patientDetailsOrderedList[APOA10] = value; }
        }
        private int OccurrenceDatesArray11
        {
            set { patientDetailsOrderedList[APOA11] = value; }
        }
        private int OccurrenceDatesArray12
        {
            set { patientDetailsOrderedList[APOA12] = value; }
        }
        private int OccurrenceDatesArray13
        {
            set { patientDetailsOrderedList[APOA13] = value; }
        }
        private int OccurrenceDatesArray14
        {
            set { patientDetailsOrderedList[APOA14] = value; }
        }
        private int OccurrenceDatesArray15
        {
            set { patientDetailsOrderedList[APOA15] = value; }
        }
        private int OccurrenceDatesArray16
        {
            set { patientDetailsOrderedList[APOA16] = value; }
        }
        private int OccurrenceDatesArray17
        {
            set { patientDetailsOrderedList[APOA17] = value; }
        }
        private int OccurrenceDatesArray18
        {
            set { patientDetailsOrderedList[APOA18] = value; }
        }
        private int OccurrenceDatesArray19
        {
            set { patientDetailsOrderedList[APOA19] = value; }
        }
        private int OccurrenceDatesArray20
        {
            set { patientDetailsOrderedList[APOA20] = value; }
        }
        # endregion

        # region Condition Codes

        private string ConditionCode1
        {
            set { patientDetailsOrderedList[APCI01] = value; }
        }

        private string ConditionCode2
        {
            set { patientDetailsOrderedList[APCI02] = value; }
        }

        private string ConditionCode3
        {
            set { patientDetailsOrderedList[APCI03] = value; }
        }

        private string ConditionCode4
        {
            set { patientDetailsOrderedList[APCI04] = value; }
        }

        private string ConditionCode5
        {
            set { patientDetailsOrderedList[APCI05] = value; }
        }

        private string ConditionCode6
        {
            set { patientDetailsOrderedList[APCI06] = value; }
        }

        private string ConditionCode7
        {
            set { patientDetailsOrderedList[APCI07] = value; }
        }

        private string ClinicCode1
        {
            set { patientDetailsOrderedList[APCL01] = value; }
        }

        private string ClinicCode2
        {
            set { patientDetailsOrderedList[APCL02] = value; }
        }

        private string ClinicCode3
        {
            set { patientDetailsOrderedList[APCL03] = value; }
        }

        private string ClinicCode4
        {
            set { patientDetailsOrderedList[APCL04] = value; }
        }

        private string ClinicCode5
        {
            set { patientDetailsOrderedList[APCL05] = value; }
        }

        private string SiteCode
        {
            set { patientDetailsOrderedList[APCL25] = value; }
        }

        # endregion

        # region Fathers Information

        private string FathersName
        {
            set { patientDetailsOrderedList[APFANM] = value; }
        }

        private DateTime FathersDateOfBirth
        {
            set { patientDetailsOrderedList[APFDOB] = ConvertDateToIntInMddyyFormat( value ); }
        }

        #endregion

        #endregion

        #region Data Elements

        private static readonly ILog c_log = LogManager.GetLogger( typeof( PatientInsertStrategy ) );
        private OrderedList aliasNamesOrderedList = new OrderedList();
        private OrderedList authorizedUSersOrderedList = new OrderedList();
        private OrderedList additionalRaceCodesOrderedList = new OrderedList();
        private OrderedList deleteOrderedList = new OrderedList();
        private OrderedList deleteOrderedListForAdditionalRaceCodes = new OrderedList();
        private Account i_Account;
        private long nonStaffPhysicianNumber;
        private string optOutInfo = "NYYYY";
        protected OrderedList patientDetailsOrderedList = new OrderedList();
        private OrderedList pregnancyIndicatorOrderedList = new OrderedList();
        private ShareHIEDataFeatureManager FeatureManagerHIE = new ShareHIEDataFeatureManager();
        #endregion

        #region Constants

        private const string
            ADMITTING_CATEGORY_INPATIENT = "3",
            ADMITTING_CATEGORY_NEWBORN = "5";
        

        private const string
            AKEDAT = "AKEDAT";

        private const string
            AKHSP_ = "AKHSP#",
            AKMRC_ = "AKMRC#";

        private const string
            AKPFNM = "AKPFNM";

        private const string
            AKPLNM = "AKPLNM";

        private const string
            AKSECF = "AKSECF";

        private const string
            AKTITL = "AKTITL";

        private const string
            AP_COD = "AP#COD";

        private const string
            AP_EXT = "AP#EXT";

        private const string
            AP_FC = "AP#FC";

        private const string
            AP_PL = "AP#PL";

        private const string
            AP_RC = "AP#RC";

        private const string
            APABC1 = "APABC1";

        private const string
            APABOR = "APABOR";

        private const string
            APABST = "APABST";

        private const string
            APACC = "APACC";

        private const string
            APACCN = "APACCN";

        private const string
            APACCT = "APACCT";

        private const string
            APACDT = "APACDT";

        private const string
            APACFL = "APACFL";

        private const string
            APACMT = "APACMT";

        private const string
            APACOD = "APACOD";

        private const string
            APACST = "APACST";

        private const string
            APACTP = "APACTP";

        private const string
            APADMC = "APADMC",
            APADMT = "APADMT";

        private const string
            APADR_ = "APADR#";

        private const string
            APADTM = "APADTM";

        private const string
            APAG01 = "APAG01",
            APAG02 = "APAG02",
            APAG03 = "APAG03",
            APAG04 = "APAG04",
            APAG05 = "APAG05",
            APAG06 = "APAG06";

        private const string
            APAGE = "APAGE";

        private const string
            APAKFM = "APAKFM";

        private const string
            APAKLN = "APAKLN";

        private const string
            APAL01 = "APAL01",
            APAL02 = "APAL02",
            APAL03 = "APAL03",
            APAL04 = "APAL04",
            APAL05 = "APAL05",
            APAL06 = "APAL06",
            APAL07 = "APAL07",
            APAL08 = "APAL08",
            APAL09 = "APAL09",
            APAL10 = "APAL10";

        private const string
            APALOC = "APALOC";

        private const string
            APALRG = "APALRG";

        private const string
            APALTD = "APALTD";

        private const string
            APAMLF = "APAMLF";

        private const string
            APANFN = "APANFN";

        private const string
            APANLN = "APANLN";

        private const string
            APANMN = "APANMN";

        private const string
            APANP_ = "APANP#";

        private const string
            APANPR = "APANPR";

        private const string
            APANU_ = "APANU#";

        private const string
            APAPFR = "APAPFR",
            APAPTO = "APAPTO";

        private const string
            APAPVL = "APAPVL";

        private const string
            APARVC = "APARVC";

        private const string
            APASL_ = "APASL#";

        private const string
            APATIM = "APATIM";

        private const string
            APATXC = "APATXC";

        private const string
            APAVTM = "APAVTM";

        private const string
            APBA01 = "APBA01",
            APBA02 = "APBA02",
            APBA03 = "APBA03",
            APBA04 = "APBA04",
            APBA05 = "APBA05",
            APBA06 = "APBA06";

        private const string
            APBDAC = "APBDAC";

        private const string
            APBDFG = "APBDFG";

        private const string
            APBED = "APBED";

        private const string
            APBLDL = "APBLDL";

        private const string
            APBYPS = "APBYPS";

        private const string
            APCACD = "APCACD";

        private const string
            APCADR = "APCADR";

        private const string
            APCC01 = "APCC01",
            APCC02 = "APCC02",
            APCC03 = "APCC03";

        private const string
            APCCIT = "APCCIT";

        private const string
            APCD01 = "APCD01",
            APCD02 = "APCD02",
            APCD03 = "APCD03",
            APCD04 = "APCD04",
            APCD05 = "APCD05",
            APCD06 = "APCD06",
            APCD07 = "APCD07",
            APCD08 = "APCD08";

        private const string
            APCH01 = "APCH01",
            APCH02 = "APCH02",
            APCH03 = "APCH03",
            APCH04 = "APCH04",
            APCH05 = "APCH05",
            APCH06 = "APCH06";

        private const string
            APCI01 = "APCI01",
            APCI02 = "APCI02",
            APCI03 = "APCI03",
            APCI04 = "APCI04",
            APCI05 = "APCI05";

        private const string
            APCI06 = "APCI06",
            APCI07 = "APCI07";

        private const string
            APCL01 = "APCL01",
            APCL02 = "APCL02",
            APCL03 = "APCL03",
            APCL04 = "APCL04",
            APCL05 = "APCL05",
            APCL06 = "APCL06",
            APCL07 = "APCL07",
            APCL08 = "APCL08",
            APCL09 = "APCL09",
            APCL10 = "APCL10",
            APCL11 = "APCL11",
            APCL12 = "APCL12",
            APCL13 = "APCL13",
            APCL14 = "APCL14",
            APCL15 = "APCL15",
            APCL16 = "APCL16",
            APCL17 = "APCL17",
            APCL18 = "APCL18",
            APCL19 = "APCL19",
            APCL20 = "APCL20",
            APCL21 = "APCL21",
            APCL22 = "APCL22",
            APCL23 = "APCL23",
            APCL24 = "APCL24",
            APCL25 = "APCL25";

        private const string
            APCLOC = "APCLOC";

        private const string
            APCLVS = "APCLVS";

        private const string
            APCMED = "APCMED";

        private const string
            APCNCD = "APCNCD";

        private const string
            APCNM = "APCNM";

        
        private const string APCNTY = "APCNTY";

        private const string
            APCOLG = "APCOLG";

        private const string
            APCOM1 = "APCOM1",
            APCOM2 = "APCOM2";

        private const string
            APCOND = "APCOND";

        private const string
            APCORC = "APCORC";

        private const string
            APCP01 = "APCP01",
            APCP02 = "APCP02",
            APCP03 = "APCP03",
            APCP04 = "APCP04",
            APCP05 = "APCP05",
            APCP06 = "APCP06";

        private const string
            APCPH_ = "APCPH#";

        private const string
            APCRCD = "APCRCD";

        private const string
            APCRSF = "APCRSF";

        private const string
            APCSTE = "APCSTE";

        private const string
            APCX01 = "APCX01",
            APCX02 = "APCX02",
            APCX03 = "APCX03",
            APCX04 = "APCX04",
            APCX05 = "APCX05",
            APCX06 = "APCX06";

        private const string
            APCZ4A = "APCZ4A";

        private const string
            APCZIP = "APCZIP",
            APCZP4 = "APCZP4";

        private const string
            APCZPA = "APCZPA";

        private const string
            APDCRT = "APDCRT";

        private const string
            APDEDT = "APDEDT";

        private const string
            APDIAG = "APDIAG";

        private const string
            APDIET = "APDIET";

        private const string
            APDISC = "APDISC";

        private const string
            APDNOT = "APDNOT";

        private const string
            APDOB = "APDOB",
            APTOB = "APTOB";
        private const string
            APDOB8 = "APDOB8";

        private const string
            APDTCD = "APDTCD";

        private const string
            APDTOT = "APDTOT";

        private const string
            APDTRC = "APDTRC";

        private const string
            APEA01 = "APEA01",
            APEA02 = "APEA02";

        private const string
            APEDC1 = "APEDC1",
            APEDC2 = "APEDC2";

        private const string
            APEEID = "APEEID";

        private const string
            APEID1 = "APEID1",
            APEID2 = "APEID2";

        private const string
            APELO1 = "APELO1",
            APELO2 = "APELO2";

        private const string
            APELTM = "APELTM";

        private const string
            APENM1 = "APENM1",
            APENM2 = "APENM2";

        private const string
            APERFG = "APERFG";

        private const string
            APESC1 = "APESC1",
            APESC2 = "APESC2";

        private const string
            APESCD = "APESCD";

        private const string
            APETHC = "APETHC";

        private const string
            APETHC2 = "APETHC2";

        private const string
            APDESCNT1 = "APDESCNT1",
            APDESCNT2 = "APDESCNT2";
         
        private const string
            APEZ01 = "APEZ01",
            APEZ02 = "APEZ02";

        private const string
            APFANM = "APFANM";

        private const string
            APFBLF = "APFBLF";

        private const string
            APFC = "APFC";

        private const string
            APFDOB = "APFDOB";

        private const string
            APFI01 = "APFI01",
            APFI02 = "APFI02",
            APFI03 = "APFI03",
            APFI04 = "APFI04",
            APFI05 = "APFI05",
            APFI06 = "APFI06";

        private const string
            APFORM = "APFORM";

        private const string
            APFR01 = "APFR01",
            APFR02 = "APFR02";

        private const string
            APFRCD = "APFRCD";

        private const string
            APG_01 = "APG#01",
            APG_02 = "APG#02",
            APG_03 = "APG#03",
            APG_04 = "APG#04",
            APG_05 = "APG#05",
            APG_06 = "APG#06";

        private const string
            APGAR_ = "APGAR#";

        private const string
            APGRCD = "APGRCD";

        private const string
            APGREC = "APGREC";

        private const string
            APHEMP = "APHEMP";

        private const string
            APHOWA = "APHOWA";

        private const string
            APHSP_ = "APHSP#";

        private const string
            API_01 = "API#01",
            API_02 = "API#02",
            API_03 = "API#03",
            API_04 = "API#04",
            API_05 = "API#05",
            API_06 = "API#06";

        private const string
            APICRT = "APICRT";

        private const string
            APICUN = "APICUN";

        private const string
            APIDID = "APIDID";

        private const string
            APIDWS = "APIDWS";

        private const string
            APIN01 = "APIN01",
            APIN02 = "APIN02";

        private const string
            APINLG = "APINLG";

        private const string
            APINSN = "APINSN";

        private const string
            APIPA = "APIPA",
            APIPAC = "APIPAC";

        private const string
            APISO = "APISO";

        private const string
            APJ401 = "APJ401",
            APJ402 = "APJ402",
            APJ403 = "APJ403",
            APJ404 = "APJ404",
            APJ405 = "APJ405",
            APJ406 = "APJ406";

        private const string
            APJA01 = "APJA01",
            APJA02 = "APJA02",
            APJA03 = "APJA03",
            APJA04 = "APJA04",
            APJA05 = "APJA05",
            APJA06 = "APJA06";

        private const string
            APJC01 = "APJC01",
            APJC02 = "APJC02",
            APJC03 = "APJC03",
            APJC04 = "APJC04",
            APJC05 = "APJC05",
            APJC06 = "APJC06",
            APJP01 = "APJP01",
            APJP02 = "APJP02",
            APJP03 = "APJP03",
            APJP04 = "APJP04",
            APJP05 = "APJP05",
            APJP06 = "APJP06";

        private const string
            APJS01 = "APJS01",
            APJS02 = "APJS02",
            APJS03 = "APJS03",
            APJS04 = "APJS04",
            APJS05 = "APJS05",
            APJS06 = "APJS06",
            APJZ01 = "APJZ01",
            APJZ02 = "APJZ02",
            APJZ03 = "APJZ03",
            APJZ04 = "APJZ04",
            APJZ05 = "APJZ05",
            APJZ06 = "APJZ06";

        private const string
            APKACT = "APKACT";

        private const string
            APKDEM = "APKDEM";

        private const string
            APLAD = "APLAD";

        private const string
            APLAST = "APLAST";

        private const string
            APLAT = "APLAT";

        private const string
            APLBWT = "APLBWT";

        private const string
            APLCL = "APLCL";

        private const string
            APLDD = "APLDD",
            APLDT = "APLDT";

        private const string
            APLMD = "APLMD";

        private const string
            APLMEN = "APLMEN";

        private const string
            APLML = "APLML";

        private const string
            APLN01 = "APLN01",
            APLN02 = "APLN02",
            APLN03 = "APLN03",
            APLN04 = "APLN04",
            APLN05 = "APLN05",
            APLN06 = "APLN06";

        private const string
            APLNGC = "APLNGC",
            APPLS = "APPLS";

        private const string
            APLRDT = "APLRDT";

        private const string
            APLUL_ = "APLUL#",
            APLUL2 = "APLUL2";

        private const string
            APLVD = "APLVD";

        private const string
            APLVLN = "APLVLN";

        private const string
            APLVTD = "APLVTD";

        private const string
            APLWBS = "APLWBS",
            APLWFC = "APLWFC";

        private const string
            APMACD = "APMACD";

        private const string
            APMADR = "APMADR";

        private const string
            APMAR = "APMAR";

        private const string
            APMB_1 = "APMB#1",
            APMB_2 = "APMB#2",
            APMB_3 = "APMB#3";

        private const string
            APMBF = "APMBF";

        private const string
            APMCIT = "APMCIT";

        private const string
            APMCNY = "APMCNY";

        private const string
            APMDOB = "APMDOB";

        private const string
            APMDR_ = "APMDR#";

        private const string
            APMELG = "APMELG";

        private const string
            APMNCD = "APMNCD";

        private const string
            APMNDN = "APMNDN";

        private const string
            APMNFN = "APMNFN";

        private const string
            APMNFR = "APMNFR";

        private const string
            APMNLN = "APMNLN";

        private const string
            APMNM = "APMNM";

        private const string
            APMNMN = "APMNMN";

        private const string
            APMNP_ = "APMNP#";

        private const string
            APMNPR = "APMNPR";

        private const string
            APMNU_ = "APMNU#";

        private const string
            APMONM = "APMONM";

        private const string
            APMOTH = "APMOTH";

        private const string
            APMPH_ = "APMPH#";

        private const string
            APMPT_ = "APMPT#";

        private const string
            APMRC_ = "APMRC#";

        private const string
            APMRPR = "APMRPR";

        private const string
            APMSL_ = "APMSL#";

        private const string
            APMSTE = "APMSTE";

        private const string
            APMSV = "APMSV";

        private const string
            APFILL = "APFILL";

        private const string
            APHCPCCOD = "APHCPCCOD";

        private const string
            APMDFL = "APMDFL ";
        private const string
            APMTXC = "APMTXC";

        private const string
            APMZ4A = "APMZ4A";

        private const string
            APMZIP = "APMZIP",
            APMZP4 = "APMZP4";

        private const string
            APMZPA = "APMZPA";

        private const string
            APNCRT = "APNCRT";

        private const string
            APNHACF = "APNHACF";

        private const string
            APNMTL = "APNMTL";

         private const string
            APPGNRN = "APPGNRN ";

        private const string
            APNOFC = "APNOFC";

        private const string
            APNOFT = "APNOFT";

        private const string
            APNPPD = "APNPPD";

        private const string
            APNPPF = "APNPPF";

        private const string
            APNPPS = "APNPPS";

        private const string
            APNPPV = "APNPPV";

        private const string
            APNRCD = "APNRCD";

        private const string
            APNRDT = "APNRDT";

        private const string
            APNS = "APNS";

        private const string
            APNTPP = "APNTPP";

        private const string
            APOA01 = "APOA01",
            APOA02 = "APOA02",
            APOA03 = "APOA03",
            APOA04 = "APOA04",
            APOA05 = "APOA05";

        private const string
            APOA06 = "APOA06",
            APOA07 = "APOA07",
            APOA08 = "APOA08",
            APOA09 = "APOA09",
            APOA10 = "APOA10";

         private const string
            APOA11 = "APOA11",
            APOA12 = "APOA12",
            APOA13 = "APOA13",
            APOA14 = "APOA14",
            APOA15 = "APOA15";

         private const string
            APOA16 = "APOA16",
            APOA17 = "APOA17",
            APOA18 = "APOA18",
            APOA19 = "APOA19",
            APOA20 = "APOA20";

        private const string
            APOC01 = "APOC01",
            APOC02 = "APOC02",
            APOC03 = "APOC03",
            APOC04 = "APOC04",
            APOC05 = "APOC05";

        private const string
            APOC06 = "APOC06",
            APOC07 = "APOC07",
            APOC08 = "APOC08",
            APOC09 = "APOC09",
            APOC10 = "APOC10";

           private const string
            APOC11 = "APOC11",
            APOC12 = "APOC12",
            APOC13 = "APOC13",
            APOC14 = "APOC14",
            APOC15 = "APOC15";

           private const string
            APOC16 = "APOC16",
            APOC17 = "APOC17",
            APOC18 = "APOC18",
            APOC19 = "APOC19",
            APOC20 = "APOC20";


        private const string
            APOD01 = "APOD01",
            APOD02 = "APOD02",
            APOD03 = "APOD03",
            APOD04 = "APOD04";

        private const string
            APODR_ = "APODR#";

        private const string
            APONFN = "APONFN";

        private const string
            APONLN = "APONLN";

        private const string
            APONMN = "APONMN";

        private const string
            APONP_ = "APONP#";

        private const string
            APONPR = "APONPR";

        private const string
            APONU_ = "APONU#";

        private const string
            APOP01 = "APOP01",
            APOP02 = "APOP02",
            APOP03 = "APOP03",
            APOP04 = "APOP04";

        private const string
            APORR1 = "APORR1",
            APORR2 = "APORR2",
            APORR3 = "APORR3";

        private const string
            APOSFD = "APOSFD";

        private const string
            APOSIN = "APOSIN";

        private const string
            APOSL_ = "APOSL#";

        private const string
            APOSTD = "APOSTD";

        private const string
            APOTXC = "APOTXC";

        private const string
            APOZWT = "APOZWT";

        private const string
            APPACD = "APPACD";

        private const string
            APPADR = "APPADR";

        private const string
            APPADRE1 = "APPADRE1";

        private const string
            APPADRE2 = "APPADRE2";

        private const string
            APMADRE1 = "APMADRE1";

        private const string
            APMADRE2 = "APMADRE2";

        private const string
            APPARC = "APPARC";

        private const string
            APPAUX = "APPAUX";

        private const string
            APPCCD = "APPCCD";

        private const string
            APPCIT = "APPCIT";

        private const string
            APPCNY = "APPCNY";

        private const string
            APPCPH = "APPCPH";

        private const string
            APPCUN = "APPCUN";

        private const string
            APPDL_ = "APPDL#";

        private const string
            APPEML = "APPEML";

        private const string
            APPFNM = "APPFNM";

        private const string
            APPGAR = "APPGAR";

        private const string
            APPLNM = "APPLNM";

        private const string
            APPLOE = "APPLOE";

        private const string
            APPLOS = "APPLOS";

        private const string
            APPMI = "APPMI";

        private const string
            APPMRC = "APPMRC";

        private const string
            APPOB = "APPOB";

        private const string
            APPOCC = "APPOCC";

        private const string
            APPOLN = "APPOLN";

        private const string
            APPPH_ = "APPPH#";

        private const string
            APPPT_ = "APPPT#";

        private const string
            APPRED = "APPRED";

        private const string
            APPREOPD = "APPREOPD";

        private const string
            APPRGI = "APPRGI";

        private const string
            APPRGT = "APPRGT";

        private const string
            APPROCD = "APPROCD";

        private const string
            APPSRC = "APPSRC";

        private const string
            APPST = "APPST";

        private const string
            APPSTE = "APPSTE";

        private const string
            APPSTS = "APPSTS";

        private const string
            APPTID = "APPTID";

        private const string
            APPTYP = "APPTYP";

        private const string
            APPUBL = "APPUBL";

        private const string
            APPVRR = "APPVRR";

        private const string
            APPZ4A = "APPZ4A";

        private const string
            APPZIP = "APPZIP",
            APPZP4 = "APPZP4";

        private const string
            APPZPA = "APPZPA";

        private const string
            APQNUM = "APQNUM";

        private const string
            APRA01 = "APRA01",
            APRA02 = "APRA02",
            APRA03 = "APRA03",
            APRA04 = "APRA04",
            APRA05 = "APRA05";

        private const string
            APRACD = "APRACD";

        private const string
            APRACE = "APRACE";

        private const string
            APRACE2 = "APRACE2";

        private const string
            APNTNLT1 = "APNTNLT1";

        private const string
            APNTNLT2 = "APNTNLT2";


        private const string
            APRADR = "APRADR";

        private const string
            APRAMC = "APRAMC";

        private const string
            APRCIT = "APRCIT";

        private const string
            APRCRP = "APRCRP";

        private const string
            APRDR_ = "APRDR#";

        private const string
            APRF01 = "APRF01",
            APRF02 = "APRF02",
            APRF03 = "APRF03",
            APRF04 = "APRF04",
            APRF05 = "APRF05",
            APRF06 = "APRF06";

        private const string
            APRLGN = "APRLGN";

        private const string
            APRNDN = "APRNDN";

        private const string
            APRNFN = "APRNFN";

        private const string
            APRNFR = "APRNFR";

        private const string
            APRNLN = "APRNLN";

        private const string
            APRNM = "APRNM";

        private const string
            APRNMN = "APRNMN";

        private const string
            APRNP_ = "APRNP#";

        private const string
            APRNPR = "APRNPR";

        private const string
            APRNU_ = "APRNU#";

        private const string
            APROOM = "APROOM";

        private const string
            APRPH_ = "APRPH#";

        private const string
            APRR_ = "APRR#";

        private const string
            APRRCD = "APRRCD";

        private const string
            APRS01 = "APRS01",
            APRS02 = "APRS02",
            APRS03 = "APRS03",
            APRS04 = "APRS04",
            APRS05 = "APRS05",
            APRS06 = "APRS06";

        private const string
            APRSCF01 = "APRSCF01",
            APRSCF02 = "APRSCF02",
            APRSCF03 = "APRSCF03",
            APRSCF04 = "APRSCF04",
            APRSCF05 = "APRSCF05",
            APRSCF06 = "APRSCF06",
            APRSCF07 = "APRSCF07",
            APRSCF08 = "APRSCF08",
            APRSCF09 = "APRSCF09",
            APRSCF10 = "APRSCF10";

        private const string
            APRSID = "APRSID";

        private const string
            APRSID02 = "APRSID02",
            APRSID03 = "APRSID03",
            APRSID04 = "APRSID04",
            APRSID05 = "APRSID05",
            APRSID06 = "APRSID06",
            APRSID07 = "APRSID07",
            APRSID08 = "APRSID08",
            APRSID09 = "APRSID09",
            APRSID10 = "APRSID10";

        private const string
            APRSL_ = "APRSL#";

        private const string
            APRSRC = "APRSRC";

        private const string
            APRSTE = "APRSTE";

        private const string
            APRTCD = "APRTCD";

        private const string
            APRTXC = "APRTXC";

        private const string
            APRTYP = "APRTYP";

        private const string
            APRZ4A = "APRZ4A";

        private const string
            APRZIP = "APRZIP",
            APRZP4 = "APRZP4";

        private const string
            APRZPA = "APRZPA";

        private const string
            APSACD = "APSACD";

        private const string
            APSADR = "APSADR";

        private const string
            APSCHD = "APSCHD";

        private const string
            APSCIT = "APSCIT";

        private const string
            APSCRT = "APSCRT";

        private const string
            APSDED = "APSDED";

        private const string
            APSEC2 = "APSEC2";

        private const string
            APSECR = "APSECR";

        private const string
            APSEX = "APSEX";

        private const string
            APSEXBIRTH = "APSEXBIRTH";

        private const string // SATX
            APFRHSP_ = "APFRHSP#",
            APFRACCT = "APFRACCT";

        private const string
            APSMOK = "APSMOK";

        private const string
            APSNM = "APSNM";

        private const string
            APSPC1 = "APSPC1",
            APSPC2 = "APSPC2",
            APSPC3 = "APSPC3",
            APSPC4 = "APSPC4",
            APSPC5 = "APSPC5",
            APSPC6 = "APSPC6";

        private const string
            APSPFR = "APSPFR";

        private const string
            APSPGM = "APSPGM";

        private const string
            APSPH_ = "APSPH#";

        private const string
            APSPN2 = "APSPN2";

        private const string
            APSPNC = "APSPNC";

        private const string
            APSPTO = "APSPTO";

        private const string
            APSSN = "APSSN";

        private const string
            APSSTE = "APSSTE";

        private const string
            APSWPY = "APSWPY";

        private const string
            APSX01 = "APSX01",
            APSX02 = "APSX02",
            APSX03 = "APSX03",
            APSX04 = "APSX04",
            APSX05 = "APSX05",
            APSX06 = "APSX06";

        private const string
            APSZIP = "APSZIP",
            APSZP4 = "APSZP4";

        private const string
            APTACD = "APTACD";

        private const string
            APTALT = "APTALT";

        private const string
            APTCRT = "APTCRT";

        private const string
            APTDAT = "APTDAT";

        private const string
            APTDR_ = "APTDR#";

        private const string
            APTECR = "APTECR";

        private const string
            APTNFN = "APTNFN";

        private const string
            APTNLN = "APTNLN";

        private const string
            APTNMN = "APTNMN";

        private const string
            APTNP_ = "APTNP#";

        private const string
            APTNPR = "APTNPR";

        private const string
            APTNU_ = "APTNU#";

        private const string
            APTREG = "APTREG";

        private const string
            APTSCR = "APTSCR";

        private const string
            APTSL_ = "APTSL#";

        private const string
            APTSRC = "APTSRC";

        private const string
            APTTME = "APTTME";

        private const string
            APTTXC = "APTTXC";

        private const string
            APUCST = "APUCST";

        private const string
            APUPRV = "APUPRV",
            APUPRW = "APUPRW";

        private const string
            APVALU = "APVALU";

        private const string
            APVAM1 = "APVAM1",
            APVAM2 = "APVAM2",
            APVAM3 = "APVAM3",
            APVAM4 = "APVAM4",
            APVAM5 = "APVAM5",
            APVAM6 = "APVAM6",
            APVAM7 = "APVAM7",
            APVAM8 = "APVAM8";

        private const string
            APVCD1 = "APVCD1",
            APVCD2 = "APVCD2",
            APVCD3 = "APVCD3",
            APVCD4 = "APVCD4",
            APVCD5 = "APVCD5",
            APVCD6 = "APVCD6",
            APVCD7 = "APVCD7",
            APVCD8 = "APVCD8";

        private const string
            APVIS_ = "APVIS#";

        private const string
            APWACD = "APWACD";

        private const string
            APWADR = "APWADR",
            APWCIT = "APWCIT";

        private const string
            APWCUN = "APWCUN";

        private const string
            APWNM = "APWNM";

        private const string
            APWPH_ = "APWPH#";

        private const string
            APWRKC = "APWRKC";

        private const string
            APWRNF = "APWRNF";

        private const string
            APWSIR = "APWSIR";

        private const string
            APWSTE = "APWSTE";

        private const string
            APWZ4A = "APWZ4A";

        private const string
            APWZIP = "APWZIP",
            APWZP4 = "APWZP4";

        private const string
            APWZPA = "APWZPA";

        private const string
            APXMIT = "APXMIT";

        private const string
            APYUCS = "APYUCS";

        private const string
            APZDTE = "APZDTE",
            APZTME = "APZTME";

        private const string
            APRGTP = "APRGTP";

        private const string
            APNBFG = "APNBFG";

        private const string
            APBTFG = "APBTFG";

        private const int
            LENGTH_OF_COMMENTS_TWO = 55;

        private const string
            NEWBORN_CODE = "L";

        private const string
            REGISTRATIONTYPE1 = "1",
            REGISTRATIONTYPE2 = "2",
            REGISTRATIONTYPE3 = "3";

        private const string
            SHORT_FORM_NO = "N";

        private const string PAIWalkinFlag = "W";

        private const string
            SHORT_FORM_YES = "Y";

        private const string
            TMACCT = "TMACCT";

        private const string
            TMHSP_ = "TMHSP#",
            TMMRC_ = "TMMRC#";

        private const string
            TMPRGI = "TMPRGI";

        private const int
            TOTAL_LENGTH_OF_PBAR_COMMENTS = 65;
        private const string
            QACFLAG = "Q";

        public const string BLANK = "     ";

        private const string
            APMDFLFILLER = YesNoFlag.CODE_BLANK;

        private const string
            AUAPIDID = "APIDID",
            AUAPHSP = "APHSP#",
            AUAPMRC = "APMRC#",
            AUAPACCT = "APACCT",
            AUAPINL = "APINLG",
            AUAPSEQ_ = "APSEQ#",
            AUAPOPTFLG = "APOPTFLG",
            AUAPLNAME = "APLNAME",
            AUAPFNAME = "APFNAME",
            AUAPDOB = "APDOB",
            AUAPEMAIL = "APEMAIL",
            AUAPDELFLG = "APDELFLG",
            AUAPTCRD = "APTCRD",
            AUAPTTME = "APTTME",
            AUAPRR_  = "APRR#",
            AUTHORIZEDUSERS_TRANSACTION_ID = "AU";

        private const string
            RAHSP = "RAHSP#" ,
            RAMR = "RAMR#",
            RASEQ = "RASEQ#",
            RARACE = "RARACE";

        #endregion
    }
}
