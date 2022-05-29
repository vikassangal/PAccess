using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.Utilities;

namespace PatientAccess.UI.Reports.FaceSheet
{
    public class DataBuilder : IDataBuilder
    {
        #region Methods

        public void SetFacility(IDictionary data)
        {
            Facility aFacility = iAccount.Facility;
            if (aFacility != null)
            {
                data.Add("lblFacilityName", aFacility.CodeAndDescription);

                ContactPoint address = aFacility.GetContactPoint(TypeOfContactPoint.NewPhysicalContactPointType());
                if (address != null)
                {
                    data.Add("lblFacilityAddress", address.Address.OneLineAddressLabel());
                }
            }
            //This needs to be checked, it probably needs to come form the server;
            data.Add("lblCurrentDateTime", DateTime.Now);
        }

        public void SetDemoGraphics( Hashtable data )
        {
            if ( iAccount.IsShortRegisteredNonDayCareAccount() )
            {
                SetShortRegistrationDemographicsFrom( data );
            }
            else
            {
                SetDemographicsFrom( data );
            }

            SetNppValuesFrom( data );

            SetAddressInformationFrom( data );

        }

        public void SetDiagnosis(IDictionary data)
        {
            if (IsAccountProxy())
            {
                var account = (AccountProxy) iAccount;
                SetDiagnosisFrom(data, account);
            }

            else
            {
                var account = (Account) iAccount;
                if (iAccount.IsShortRegisteredNonDayCareAccount())
                {
                    SetShortRegistrationDiagnosisFrom(data, account);
                }
                else
                {
                    SetDiagnosisFrom(data, account);
                }
            }
        }

        public void SetRegulatory(IDictionary data)
        {
            if (!IsAccountProxy())
            {
                var account = (Account) iAccount;
                var rightToRestrictFeatureManager = new RightToRestrictFeatureManager();
                var rightToRestrict = string.Empty;
                
                if (account.RightToRestrict != null && 
                    rightToRestrictFeatureManager.IsRightToRestrictEnabledForAccountCreatedDate(account.AccountCreatedDate))
                {
                    rightToRestrict = account.RightToRestrict.IsYes
                        ? account.RightToRestrict.Description
                        : YesNoFlag.DESCRIPTION_NO;
                }
				
                data.Add("lblRightToRestrict", rightToRestrict);
            }
        }

        public void SetClinical(IDictionary data)
        {
            SetPhysicianDataFrom(data, iAccount);

            if (!IsAccountProxy())
            {
                var account = (Account) iAccount;

                if (!iAccount.IsShortRegistered)
                {
                    data.Add("lblSmoker", account.Smoker);
                    data.Add("lblResistantOrganism", account.ResistantOrganism);
                    data.Add("lblComments", account.ClinicalComments);

                }
                else
                {
                    ParseCommentsLine(data, account.ClinicalComments);
                }
            }
            SetPriorVisitInformation(data, iAccount);
        }

        private void SetPriorVisitInformation(IDictionary data, IAccount account)
        {
            var priorVisitResponse = new PriorVisitInformationResponse
                {
                    PriorHospitalCode = String.Empty,
                    PriorAccountNumber = String.Empty,
                    PriorAdmitDate = String.Empty,
                    PriorDischargeDate = String.Empty
                };

            if (IsAccountProxy())
            {
                SetPriorVisitData(data, priorVisitResponse);
                return;
            }
            var anAccount = (Account) iAccount;
            if (anAccount == null || anAccount.Insurance == null)
            {
                SetPriorVisitData(data, priorVisitResponse);
                return;
            }
            var primaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
            var secondaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID);

            if ( ( primaryCoverage == null || primaryCoverage.GetType() != typeof(GovernmentMedicareCoverage) ) &&
                ( secondaryCoverage == null || secondaryCoverage.GetType() != typeof (GovernmentMedicareCoverage)) )
            {
                SetPriorVisitData(data, priorVisitResponse);
                return;
            }

            var primaryMedicareHICNumber = GetMedicareHICNumber(primaryCoverage);
            var secondaryMedicareHICNumber = GetMedicareHICNumber(secondaryCoverage);
            var primaryMBINumber = GetMedicareMBINumber(primaryCoverage);
            var secondaryMBINumber = GetMedicareMBINumber(secondaryCoverage);
            if (String.IsNullOrEmpty(primaryMedicareHICNumber) && String.IsNullOrEmpty(secondaryMedicareHICNumber))
            {
                SetPriorVisitData(data, priorVisitResponse);
                return;
            }
       
            if (!string.IsNullOrEmpty( primaryMedicareHICNumber ))
            {

                priorVisitResponse = GetPriorVisitResponse(account, primaryMedicareHICNumber, primaryMBINumber);
                if ((priorVisitResponse == null || String.IsNullOrEmpty(priorVisitResponse.PriorHospitalCode.Trim())) &&
                    !String.IsNullOrEmpty(secondaryMedicareHICNumber))
                {
                    priorVisitResponse = GetPriorVisitResponse(account, secondaryMedicareHICNumber, secondaryMBINumber);
                }
            }
            else
            {
                priorVisitResponse = GetPriorVisitResponse(account, secondaryMedicareHICNumber, secondaryMBINumber);
            }
            SetPriorVisitData(data, priorVisitResponse);
        }


        private PriorVisitInformationResponse GetPriorVisitResponse(IAccount account, string HICNumber , string MBINumber)
        {
            var priorVisitRequest =
                new PriorVisitInformationRequest
                    {
                        Facility = account.Facility,
                        AccountNumber =  account.AccountNumber.ToString(),
                        First1OfFirstName = GetFirstName(account),
                        First4OfLastName = GetLastName(account),
                        DateOfBirth = account.Patient.DateOfBirth.ToString("yyyyMMdd"),
                        Gender = account.Patient.Sex.Code,
                        MedicareHic = HICNumber,
                        MBINumber =  MBINumber,
                        Certificate = string.Empty
                    };

            IPriorVisitBroker priorVisitBroker;
            priorVisitBroker = BrokerFactory.BrokerOfType<IPriorVisitBroker>();
            if (priorVisitBroker == null) throw new ArgumentNullException("priorVisitBroker");

            return priorVisitBroker.GetPriorVisitResponse(priorVisitRequest);
        }

        private void SetPriorVisitData(IDictionary data, PriorVisitInformationResponse priorVisitResults)
        {
            var priorAdmitDate = String.Empty;
            var priorDischargeDate = String.Empty;
            if ( !String.IsNullOrEmpty(priorVisitResults.PriorAdmitDate.Trim()) )
            {
                priorAdmitDate = ValidDateString(priorVisitResults.PriorAdmitDate);
            }
            if ( !String.IsNullOrEmpty(priorVisitResults.PriorDischargeDate.Trim()))
            {
                priorDischargeDate = ValidDateString(priorVisitResults.PriorDischargeDate);
            }

            data.Add("lblPriorVisit", priorAdmitDate);
            data.Add("lblPriorHospital", priorVisitResults.PriorHospitalCode);
            data.Add("lblPriorVisitFromTo", priorAdmitDate + " " + priorDischargeDate);
        }

        private string ValidDateString(string date)
        {

            var year = Convert.ToInt32(date.Substring(0, 4));
            var month = Convert.ToInt32(date.Substring(4, 2));
            var day = Convert.ToInt32(date.Substring(6, 2));
            if (year != 0 && month > 0 && month <=12  && day > 0 && day <= 31)
            {
                return new DateTime(year, month, day).ToShortDateString();
            }

            return String.Empty;
        }

        private static string GetLastName(IAccount account)
        {
            var lengthOfLastName = account.Patient.Name.LastName.Trim().Length;
            if (lengthOfLastName > 3)
            {
                return account.Patient.Name.LastName.Substring(0, 4);
            }
            else if (lengthOfLastName > 0)
            {
                return account.Patient.Name.LastName.Substring(0, lengthOfLastName);
            }
            return string.Empty;
        }

        private static string GetFirstName(IAccount account)
        {
            if (account.Patient.Name.FirstName.Trim().Length > 0 )
            {
                return account.Patient.Name.FirstName.Substring(0, 1);
            }
            return string.Empty;
        }

      
        private string GetMedicareHICNumber(Coverage coverage)
        {
            if (coverage == null ||  coverage.GetType() != typeof (GovernmentMedicareCoverage))
            {
                return string.Empty;
            }
            var aCoverage = coverage as GovernmentMedicareCoverage;
            if (aCoverage != null)
            {
                return String.Empty;
            }
            return string.Empty;
        }
        private string GetMedicareMBINumber(Coverage coverage)
        {
            if (coverage == null || coverage.GetType() != typeof(GovernmentMedicareCoverage))
            {
                return string.Empty;
            }
            var aCoverage = coverage as GovernmentMedicareCoverage;
            if (aCoverage != null)
            {
                return aCoverage.MBINumber;
            }
            return string.Empty;
        }


        private static void ParseCommentsLine(IDictionary data, string comments)
        {
            if (comments != null)
            {
                if (comments.Length <= TOTAL_LENGTH_OF_PBAR_COMMENTS)
                {
                    data.Add("lblComments", comments);
                    data.Add("lblComments2", "");
                }
                else
                {
                    data.Add("lblComments", comments.Substring(0, TOTAL_LENGTH_OF_PBAR_COMMENTS));
                    data.Add("lblComments2", comments.Substring(TOTAL_LENGTH_OF_PBAR_COMMENTS).PadRight
                                                 (LENGTH_OF_COMMENTS_TWO, ' '));
                }
            }
        }
        public void SetInsurance(Hashtable data)
        {
            if (IsAccountProxy())
            {
                if (((AccountProxy) iAccount).FinancialClass != null)
                {
                    data.Add("lblFinacialClass", ((AccountProxy) iAccount).FinancialClass.ToCodedString());
                }
                return;
            }

            var account = (Account) iAccount;
            if (account.MedicalGroupIPA != null)
            {
                data.Add("lblIPA", account.MedicalGroupIPA.Name);
            }

            if (account.FinancialClass != null)
            {
                data.Add("lblFinacialClass", account.FinancialClass.ToCodedString());
            }

            Insurance insurance = account.Insurance;
            if (insurance != null)
            {
                Insured primaryInsured = account.PrimaryInsured;

                bool isPreMSE = false;
                if (account.Activity != null)
                {
                    if (account.Activity is PreMSERegisterActivity || account.Activity is UCCPreMSERegistrationActivity)
                    {
                        isPreMSE = true;
                    }
                }
                SetPrimaryInsuranceDetailsFrom(data, insurance, primaryInsured, isPreMSE);

                Insured secondaryInsured = account.SecondaryInsured;
                SetSecondaryInsuranceDetailsFrom(data, insurance, secondaryInsured);
            }
        }

        public void SetGuarantor(IDictionary data)
        {
            var account = (Account) iAccount;
            if (account != null)
            {
                if (account.Guarantor != null)
                {
                    data.Add("lblGuarantor", account.Guarantor.Name.AsFormattedName());

                    if (account.Guarantor.SocialSecurityNumber != null)
                    {
                        data.Add("lblGuarantorSSN", SsnFormatter(account.Guarantor.SocialSecurityNumber));
                    }

                    if (!account.IsShortRegisteredNonDayCareAccount())
                    {
                        if (account.Guarantor.Employment != null)
                        {
                            data.Add("lblGuarantorOccupation", account.Guarantor.Employment.Occupation);
                        }
                    }

                    ContactPoint physical = account.Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
                    data.Add("lblGuarantorAddress", physical.Address.Address1);
                    data.Add("lblGuarantorCityStreetZip", string.Format("{0}, {1} {2}",
                                                                        physical.Address.City,
                                                                        physical.Address.State != null ? physical.Address.State.Code : string.Empty,
                                                                        physical.Address.ZipCode.ZipCodePrimary));
                    data.Add("lblGuarantorPhone", physical.PhoneNumber.AsFormattedString());
                }
                RelationshipType relationShipType = account.Patient.RelationshipWith(account.Guarantor);

                if (relationShipType != null)
                {
                    data.Add("lblPatientIsGuaranor", relationShipType.Description);
                }

                ContactPoint mobile = account.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
                data.Add("lblCell", mobile.PhoneNumber.AsFormattedString() );
                data.Add("lblCellConsent", mobile.CellPhoneConsent.Description);


            }
        }

        public void SetBilling(IDictionary data)
        {
            var account = (Account) iAccount;
            if (account.OccurrenceSpans.Count > 0)
            {
                var span = (OccurrenceSpan) account.OccurrenceSpans[0];
                if (span != null)
                {
                    data.Add("lblSpanCode", span.SpanCode.ToCodedString());
                    data.Add("lblSpanFromDate", span.FromDate.ToShortDateString());
                    data.Add("lblSpanToDate", span.ToDate.ToShortDateString());
                }
                //Not sure about this one.
                if (account.FacilityDeterminedFlag != null)
                {
                    data.Add("lblFacilityDetermindFlag", account.FacilityDeterminedFlag.ToCodedString());
                }
            }
        }

        public void SetContacts(IDictionary data)
        {
            var account = (Account) iAccount;

            if (account.EmergencyContact1 != null)
            {
                data.Add("lblContact1", account.EmergencyContact1.Name);
                if (account.EmergencyContact1.RelationshipType != null)
                {
                    data.Add("lblRelation1", account.EmergencyContact1.RelationshipType.DisplayString);

                    ContactPoint physical = account.EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
                    data.Add("lblContact1Address", physical.Address.Address1);
                    data.Add("lblContact1CityStreetZip", string.Format("{0}, {1} {2}",
                                                                       physical.Address.City,
                                                                       physical.Address.State != null ? physical.Address.State.Code : string.Empty,
                                                                       physical.Address.ZipCode.ZipCodePrimary));
                    data.Add("lblContact1Phone", physical.PhoneNumber.AsFormattedString());
                    data.Add("lblContact1Cell", physical.CellPhoneNumber.AsFormattedString());

                    ContactPoint work = account.EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewEmployerContactPointType());
                    data.Add("lblContact1WorkPhone", work.PhoneNumber.AsFormattedString());
                }
            }

            if (account.EmergencyContact2 != null)
            {
                data.Add("lblContact2", account.EmergencyContact2.Name);
                if (account.EmergencyContact2.RelationshipType != null)
                {
                    data.Add("lblRelation2", account.EmergencyContact2.RelationshipType.DisplayString);

                    ContactPoint physical = account.EmergencyContact2.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
                    data.Add("lblContact2Address", physical.Address.Address1);
                    data.Add("lblContact2CityStreetZip", string.Format("{0}, {1} {2}",
                                                                       physical.Address.City,
                                                                       physical.Address.State != null ? physical.Address.State.Code : string.Empty,
                                                                       physical.Address.ZipCode.ZipCodePrimary));
                    data.Add("lblContact2Phone", physical.PhoneNumber.AsFormattedString());
                    data.Add("lblContact2Cell", physical.CellPhoneNumber.AsFormattedString());

                    ContactPoint work = account.EmergencyContact2.ContactPointWith(TypeOfContactPoint.NewEmployerContactPointType());
                    data.Add("lblContact2WorkPhone", work.PhoneNumber);
                }
            }
        }

        public void SetShortRegistrationContacts(IDictionary data)
        {
            var account = (Account) iAccount;

            if (account.EmergencyContact1 != null)
            {
                data.Add("lblContact1", account.EmergencyContact1.Name);
                if (account.EmergencyContact1.RelationshipType != null)
                {
                    data.Add("lblRelation1", account.EmergencyContact1.RelationshipType.DisplayString);

                    ContactPoint physical = account.EmergencyContact1.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
                    data.Add("lblContact1Phone", physical.PhoneNumber.AsFormattedString());
                }
            }
        }

        public bool IsAccountProxy()
        {
            return iAccount is AccountProxy;
        }

        public Func<SocialSecurityNumber, string> SsnFormatter { get; set; }

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        private void SetDemographicsFrom(IDictionary data)
        {
            var aPatient = iAccount.Patient;

            if ( aPatient == null ) return;

            data.Add("lblPatientName", aPatient.Name.AsFormattedName());
            if (iAccount.AdmitDate != DateTime.MinValue)
            {
                data.Add("lblAdmitDate", iAccount.AdmitDate.ToString());
            }

            data.Add("lblAccount", iAccount.AccountNumber.ToString());
            data.Add("lblMRN", aPatient.MedicalRecordNumber.ToString());
            if (aPatient.SocialSecurityNumber != null)
            {
                data.Add("lblPatientSSN", SsnFormatter(aPatient.SocialSecurityNumber));
            }
            data.Add("lblPatientDOB", aPatient.DateOfBirth.ToShortDateString());
            data.Add("lblPatientAge", aPatient.Age());

            if (aPatient.Sex != null)
            {
                data.Add("lblPatientGender", aPatient.Sex.Description);
            }
            if (aPatient.MaritalStatus != null)
            {
                data.Add("lblPatientMaritalStatus", aPatient.MaritalStatus.Description);
            }
            if (!String.IsNullOrEmpty(aPatient.PrintRaceString))
            {
                data.Add("lblPatientRace", aPatient.PrintRaceString);
            }
            if (aPatient.Language != null)
            {
                data.Add("lblPatientLanguage", aPatient.Language.Description);
            }
            if (aPatient.Employment != null)
            {
                data.Add("lblPatientOccupation", aPatient.Employment.Occupation);
            }
            if (aPatient.Religion != null)
            {
                data.Add("lblPatientReligion", aPatient.Religion.Description);
            }
            if (aPatient.PlaceOfWorship != null)
            {
                data.Add("lblPlaceOfWorship", aPatient.PlaceOfWorship.Description);
            }
        }

        private void SetAddressInformationFrom(IDictionary data)
        {
            var aPatient = iAccount.Patient;

            if ( aPatient == null ) return;

            ContactPoint patientMailing = aPatient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            data.Add("lblPatientMailingAddress", patientMailing.Address.Address1);
            data.Add("lblPatientMailingCityStreetZip",
                     string.Format("{0}, {1} {2}",
                                   patientMailing.Address.City,
                                   patientMailing.Address.State != null ? patientMailing.Address.State.Code : string.Empty,
                                   patientMailing.Address.ZipCode.ZipCodePrimary));
            data.Add("lblPatintMailingPhone", patientMailing.PhoneNumber.AsFormattedString());

            ContactPoint patientPhysical = aPatient.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
            data.Add("lblPatientPhysicalAddress", patientPhysical.Address.Address1);
            data.Add("lblPatientPhysicalCityStreetZip", string.Format("{0}, {1} {2}",
                                                                      patientPhysical.Address.City,
                                                                      patientPhysical.Address.State != null ? patientPhysical.Address.State.Code : string.Empty,
                                                                      patientPhysical.Address.ZipCode.ZipCodePrimary));
            data.Add("lblPatintPhysicalPhone", patientPhysical.PhoneNumber.AsFormattedString());

            var cellCP = aPatient.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
            data.Add("lblPatientsCellPhone", cellCP.PhoneNumber.AsFormattedString());

            var emailAddressText = patientMailing.EmailAddress.Uri;
            data.Add("lblPatientsEmail",
                emailAddressText.Length > 32 ? emailAddressText.Substring(0, 32) : emailAddressText);
        }

        private void SetNppValuesFrom(IDictionary data)
        {
            data.Add("lblValuablesCollected", iAccount.ValuablesAreTaken.Description);
            if (iAccount.Patient.NoticeOfPrivacyPracticeDocument != null &&
                iAccount.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion != null)
            {
                data.Add("lblNPP", iAccount.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion.ToCodedString());
            }
        }

        private void SetShortRegistrationDemographicsFrom(IDictionary data)
        {
            var aPatient = iAccount.Patient;

            if (aPatient == null) return;

            data.Add("lblPatientName", aPatient.Name.AsFormattedName());
            if (iAccount.AdmitDate != DateTime.MinValue)
            {
                data.Add("lblAdmitDate", iAccount.AdmitDate.ToString());
            }

            data.Add("lblAccount", iAccount.AccountNumber.ToString());
            data.Add("lblMRN", aPatient.MedicalRecordNumber.ToString());
            if (aPatient.SocialSecurityNumber != null)
            {
                data.Add("lblPatientSSN", SsnFormatter(aPatient.SocialSecurityNumber));
            }
            data.Add("lblPatientDOB", aPatient.DateOfBirth.ToShortDateString());
            data.Add("lblPatientAge", aPatient.Age());

            if (aPatient.Sex != null)
            {
                data.Add("lblPatientGender", aPatient.Sex.Description);
            }
            if (aPatient.MaritalStatus != null)
            {
                data.Add("lblPatientMaritalStatus", aPatient.MaritalStatus.Description);
            }
            if (!String.IsNullOrEmpty(aPatient.PrintRaceString))
            {
                data.Add("lblPatientRace", aPatient.PrintRaceString);
            }
            if (aPatient.Language != null)
            {
                data.Add("lblPatientLanguage", aPatient.Language.Description);
            }
        }

        private static void SetShortRegistrationDiagnosisFrom(IDictionary data, Account account)
        {
            if (account.KindOfVisit != null)
            {
                data.Add("lblVisitType", account.KindOfVisit.PrintString);
            }
            if (account.HospitalService != null)
            {
                data.Add("lblHospitalService", account.HospitalService.DisplayString);
            }
            if (account.AdmitSource != null)
            {
                data.Add("lblAdmitSource", account.AdmitSource.DisplayString);
            }
            if (account.Diagnosis != null)
            {
                data.Add("lblChiefComplaint", account.Diagnosis.ChiefComplaint);
            }

            for (int i = 0; i < account.Clinics.Count; i++)
            {
                object tempClinic = account.Clinics[i];
                if (tempClinic is HospitalClinic)
                {
                    var clinic = (HospitalClinic) account.Clinics[i];
                    if (clinic != null)
                    {
                        data.Add(string.Format("lblClinic{0}", i + 1), clinic.DisplayString);
                    }
                }
            }
        }

        private static void SetDiagnosisFrom(IDictionary data, Account account)
        {
            if (account.KindOfVisit != null)
            {
                data.Add("lblVisitType", account.KindOfVisit.PrintString);
            }
            if (account.HospitalService != null)
            {
                data.Add("lblHospitalService", account.HospitalService.DisplayString);
            }
            if (account.Location != null)
            {
                data.Add("lblLocation", account.Location.DisplayString);
            }
            if (account.ReferralSource != null)
            {
                data.Add("lblReferralSource", account.ReferralSource.DisplayString);
            }
            if (account.Diagnosis != null)
            {
                data.Add("lblChiefComplaint", account.Diagnosis.ChiefComplaint);
            }

            for (int i = 0; i < account.Clinics.Count; i++)
            {
                object tempClinic = account.Clinics[i];
                if (tempClinic is HospitalClinic)
                {
                    var clinic = (HospitalClinic) account.Clinics[i];
                    if (clinic != null)
                    {
                        data.Add(string.Format("lblClinic{0}", i + 1), clinic.DisplayString);
                    }
                }
            }
        }

        private static void SetDiagnosisFrom(IDictionary data, AccountProxy account)
        {
            if (account.KindOfVisit != null)
            {
                data.Add("lblVisitType", account.KindOfVisit.PrintString);
            }
            data.Add("lblHospitalService", account.HospitalService.DisplayString);

            if (account.Diagnosis != null)
            {
                data.Add("lblChiefComplaint", account.Diagnosis.ChiefComplaint);
            }

            for (int i = 0; i < account.HospitalClinics.Count; i++)
            {
                object tempClinic = account.HospitalClinics[i];
                if (tempClinic is HospitalClinic)
                {
                    var clinic = (HospitalClinic) account.HospitalClinics[i];
                    if (clinic != null)
                    {
                        data.Add(string.Format("lblClinic{0}", i + 1), clinic.DisplayString);
                    }
                }
            }
        }

        private static void SetPhysicianDataFrom(IDictionary data, IAccount account)
        {
            var broker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
            if (account.ReferringPhysician != null)
            {
                data.Add("lblReferringPhysisian", account.ReferringPhysician.Name.AsFormattedName());
                if (account.ReferringPhysician.PhoneNumber.Number != string.Empty)
                {
                    data.Add("lblReferringPhysicianPhone", account.ReferringPhysician.PhoneNumber.AsFormattedString());
                }
                else
                {
                    if (Pbar.IsAvailable())
                    {
                        Physician physician = broker.PhysicianDetails(
                            User.GetCurrent().Facility.Oid, account.ReferringPhysician.PhysicianNumber);
                        if (physician != null)
                        {
                            if (physician.PhoneNumber != null)
                            {
                                data.Add("lblReferringPhysicianPhone", physician.PhoneNumber.AsFormattedString());
                            }
                        }
                    }
                }
            }
            if (account.AdmittingPhysician != null)
            {
                data.Add("lblAdmittingPhysician", account.AdmittingPhysician.Name.AsFormattedName());
                if (account.AdmittingPhysician.PhoneNumber.Number != string.Empty)
                {
                    data.Add("lblAdmittingPhysicianPhone", account.AdmittingPhysician.PhoneNumber.AsFormattedString());
                }
                else
                {
                    if (Pbar.IsAvailable())
                    {
                        Physician physician = broker.PhysicianDetails(
                            User.GetCurrent().Facility.Oid, account.AdmittingPhysician.PhysicianNumber);
                        if (physician != null)
                        {
                            if (physician.PhoneNumber != null)
                            {
                                data.Add("lblAdmittingPhysicianPhone", physician.PhoneNumber.AsFormattedString());
                            }
                        }
                    }
                }
            }
            if (account.AttendingPhysician != null)
            {
                data.Add("lblAttendingPhysician", account.AttendingPhysician.Name.AsFormattedName());
                if (account.AttendingPhysician.PhoneNumber.Number != string.Empty)
                {
                    data.Add("lblAttendingPhysicianPhone", account.AttendingPhysician.PhoneNumber.AsFormattedString());
                }
                else
                {
                    if (Pbar.IsAvailable())
                    {
                        Physician physician = broker.PhysicianDetails(
                            User.GetCurrent().Facility.Oid, account.AttendingPhysician.PhysicianNumber);

                        if (physician != null)
                        {
                            if (physician.PhoneNumber != null)
                            {
                                data.Add("lblAttendingPhysicianPhone", physician.PhoneNumber.AsFormattedString());
                            }
                        }
                    }
                }
            }
        }


        private static void SetSecondaryInsuranceDetailsFrom(Hashtable data, Insurance insurance, Insured secondaryInsured)
        {
            Coverage secondary = insurance.CoverageFor(CoverageOrder.SECONDARY_OID);
            if (secondary != null)
            {
                data.Add("lblPayorPriority2", "SECONDARY");
                if (secondary.InsurancePlan != null)
                {
                    data.Add("lblPlanId2", secondary.InsurancePlan.PlanID);
                    data.Add("lblPlanName2", secondary.InsurancePlan.PlanName);
                }

                if (secondary.Insured != null)
                {
                    SetSecondaryInsuredDetailsFrom(data, secondary, secondaryInsured);
                }

                SetInsuraceLabelandValueFrom(data, secondary, false);

                if (secondary.BillingInformation != null)
                {
                    SetSecondaryBillingInformationFrom(data, secondary);
                }
            }
        }


        private static void SetSecondaryInsuredDetailsFrom(IDictionary data, Coverage secondary, Party secondaryInsured)
        {
            data.Add("lblPayorName2", secondary.Insured.Name.AsFormattedName());
            data.Add("lblInsured2", secondary.Insured.Name.AsFormattedName());

            RelationshipType relationship = secondary.Insured.RelationshipWith(secondaryInsured);
            if (relationship != null)
            {
                data.Add("lblInsuredRelationship2", relationship.Description);
            }
            data.Add("lblGender2", secondary.Insured.Sex.Description);

            if (secondary.Insured.Employment != null && secondary.Insured.Employment.Employer != null)
            {
                data.Add("lblInsuredEmployer2", secondary.Insured.Employment.Employer.Name);

                if (secondary.Insured.Employment.Employer.PartyContactPoint != null)
                {
                    data.Add("lblInsuredAddress2", secondary.Insured.Employment.Employer.PartyContactPoint.Address.Address1);
                    data.Add("lblInsuredCityStateZip2", string.Format("{0}, {1} {2}",
                                                                      secondary.Insured.Employment.Employer.PartyContactPoint.Address.City,
                                                                      secondary.Insured.Employment.Employer.PartyContactPoint.Address.State != null ? secondary.Insured.Employment.Employer.PartyContactPoint.Address.State.Code : string.Empty,
                                                                      secondary.Insured.Employment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary));
                    if (secondary.Insured.Employment.Employer.PartyContactPoint.PhoneNumber != null)
                    {
                        data.Add("lblInsuredPhone2", secondary.Insured.Employment.Employer.PartyContactPoint.PhoneNumber.AsFormattedString());
                    }
                }
            }
        }


        private static void SetPrimaryInsuranceDetailsFrom(Hashtable data, Insurance insurance, Party primaryInsured, bool isPreMSE)
        {
            Coverage primary = insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
            if (primary != null)
            {
                data.Add("lblPayorPriority", "PRIMARY");

                if (primary.InsurancePlan != null)
                {
                    data.Add("lblPlanId", primary.InsurancePlan.PlanID);
                    data.Add("lblPlanName", primary.InsurancePlan.PlanName);
                }
                if (primary.Insured != null)
                {
                    if (!isPreMSE)
                    {
                        SetNonPreMsePrimaryInsuredDetailsFrom(data, primary);
                    }

                    RelationshipType relationship = primary.Insured.RelationshipWith(primaryInsured);
                    if (relationship != null)
                    {
                        data.Add("lblInsuredRelationship", relationship.Description);
                    }
                }

                SetInsuraceLabelandValueFrom(data, primary, true);

                if (primary.BillingInformation != null && !isPreMSE)
                {
                    SetNonPreMsePrimaryBillingInformationFrom(data, primary);
                }
            }
        }


        private static void SetSecondaryBillingInformationFrom(IDictionary data, Coverage secondary)
        {
            data.Add("lblBillingCOName2", secondary.BillingInformation.BillingCOName);
            data.Add("lblBillingName2", secondary.BillingInformation.BillingName);

            if (secondary.BillingInformation.Address != null)
            {
                data.Add("lblBillingAddress2", secondary.BillingInformation.Address.Address1);
                data.Add("lblBillingCityStateZip2", string.Format("{0}, {1} {2}",
                                                                  secondary.BillingInformation.Address.City,
                                                                  secondary.BillingInformation.Address.State != null ? secondary.BillingInformation.Address.State.Code : string.Empty,
                                                                  secondary.BillingInformation.Address.ZipCode.ZipCodePrimary));
                data.Add("lblBillingPhone2", secondary.BillingInformation.PhoneNumber.AsFormattedString());
            }
        }


        private static void SetNonPreMsePrimaryBillingInformationFrom(IDictionary data, Coverage primary)
        {
            data.Add("lblBillingCOName", primary.BillingInformation.BillingCOName);
            data.Add("lblBillingName", primary.BillingInformation.BillingName);

            if (primary.BillingInformation.Address != null)
            {
                data.Add("lblBillingAddress", primary.BillingInformation.Address.Address1);
                data.Add("lblBillingCityStateZip", string.Format("{0}, {1} {2}",
                                                                 primary.BillingInformation.Address.City,
                                                                 primary.BillingInformation.Address.State != null ? primary.BillingInformation.Address.State.Code : string.Empty,
                                                                 primary.BillingInformation.Address.ZipCode.ZipCodePrimary));
                data.Add("lblBillingPhone", primary.BillingInformation.PhoneNumber.AsFormattedString());
            }
        }


        private static void SetNonPreMsePrimaryInsuredDetailsFrom(IDictionary data, Coverage primary)
        {
            data.Add("lblPayorName", primary.Insured.Name.AsFormattedName());
            data.Add("lblInsured", primary.Insured.Name.AsFormattedName());
            if (primary.Insured.Sex != null)
            {
                data.Add("lblGender", primary.Insured.Sex.Description);
            }
            if (primary.Insured.Employment != null && primary.Insured.Employment.Employer != null)
            {
                data.Add("lblInsuredEmployer", primary.Insured.Employment.Employer.Name);
                if (primary.Insured.Employment.Employer.PartyContactPoint != null)
                {
                    data.Add("lblInsuredAddress", primary.Insured.Employment.Employer.PartyContactPoint.Address.Address1);
                    data.Add("lblInsuredCityStateZip", string.Format("{0}, {1} {2}",
                                                                     primary.Insured.Employment.Employer.PartyContactPoint.Address.City,
                                                                     primary.Insured.Employment.Employer.PartyContactPoint.Address.State != null ? primary.Insured.Employment.Employer.PartyContactPoint.Address.State.Code : string.Empty,
                                                                     primary.Insured.Employment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary));
                    if (primary.Insured.Employment.Employer.PartyContactPoint.PhoneNumber != null)
                    {
                        data.Add("lblInsuredPhone", primary.Insured.Employment.Employer.PartyContactPoint.PhoneNumber.AsFormattedString());
                    }
                }
            }
        }


        private static void SetInsuraceLabelandValueFrom(IDictionary data, Coverage aCoverage, bool primary)
        {
            if (aCoverage.Insured == null)
            {
                return;
            }

            Type t = aCoverage.InsurancePlan.GetType();
            if (primary)
            {
                SetPrimaryCoverageDetailsFrom(data, t, aCoverage);
                return;
            }
            else
            {
                SetSecondaryCoverageDetailsFrom(data, t, aCoverage);
                return;
            }
        }


        private static void SetSecondaryCoverageDetailsFrom(IDictionary data, Type t, Coverage aCoverage)
        {
            if (t == typeof (CommercialInsurancePlan))
            {
                data.Add("lblInslbl2", "Cert/SSN/ID");
                data.Add("lblInsvalue2", ((CommercialCoverage) aCoverage).CertSSNID);
                data.Add("lblTracking2", ((CommercialCoverage) aCoverage).TrackingNumber);
                data.Add("lblAuthorization2", ((CommercialCoverage) aCoverage).Authorization.AuthorizationNumber);
                data.Add("lblGroupNumber2", ((CommercialCoverage) aCoverage).GroupNumber);
                return;
            }

            if (t == typeof (GovernmentMedicaidInsurancePlan))
            {
                data.Add("lblInslbl2", "Policy/CIN");
                data.Add("lblInsvalue2", ((GovernmentMedicaidCoverage) aCoverage).PolicyCINNumber);
                data.Add("lblTracking2", ((GovernmentMedicaidCoverage)aCoverage).TrackingNumber);
                data.Add("lblAuthorization2", ((GovernmentMedicaidCoverage) aCoverage).Authorization.AuthorizationNumber);

                return;
            }

            if (t == typeof (WorkersCompensationInsurancePlan))
            {
                data.Add("lblInslbl2", "Policy");
                data.Add("lblInsvalue2", ((WorkersCompensationCoverage) aCoverage).PolicyNumber);
                data.Add("lblAuthorization2", ((WorkersCompensationCoverage) aCoverage).Authorization.AuthorizationNumber);
                return;
            }

            if (t == typeof (GovernmentOtherInsurancePlan))
            {
                data.Add("lblInslbl2", "Cert/SSN/ID");
                data.Add("lblInsvalue2", ((GovernmentOtherCoverage) aCoverage).CertSSNID);
                data.Add("lblTracking2", ((GovernmentOtherCoverage) aCoverage).TrackingNumber);
                data.Add("lblAuthorization2", ((GovernmentOtherCoverage) aCoverage).Authorization.AuthorizationNumber);
                data.Add("lblGroupNumber2", ((GovernmentOtherCoverage) aCoverage).GroupNumber);
                return;
            }

            if (t == typeof (GovernmentMedicareInsurancePlan))
            {
                data.Add("lblInslbl2", "MBI");
                data.Add("lblInsvalue2", ((GovernmentMedicareCoverage) aCoverage).FormattedMBINumber);
                if (aCoverage.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    var medicareCoverage = (GovernmentMedicareCoverage)aCoverage;
                    if (medicareCoverage != null &&
                        medicareCoverage.IsMedicareCoverageValidForAuthorization)
                    {
                        data.Add("lblTracking2",
                            ((GovernmentMedicareCoverage) aCoverage).TrackingNumber);
                        data.Add("lblAuthorization2",
                            ((GovernmentMedicareCoverage) aCoverage).Authorization
                            .AuthorizationNumber);
                    }
                }
                return;
            }

            if (t == typeof (OtherInsurancePlan))
            {
                data.Add("lblInslbl2", "Cert/SSN/ID");
                data.Add("lblInsvalue2", ((OtherCoverage) aCoverage).CertSSNID);
                data.Add("lblTracking2", ((OtherCoverage) aCoverage).TrackingNumber);
                data.Add("lblAuthorization2", ((OtherCoverage) aCoverage).Authorization.AuthorizationNumber);
                data.Add("lblGroupNumber2", ((OtherCoverage) aCoverage).GroupNumber);
                return;
            }
        }


        private static void SetPrimaryCoverageDetailsFrom(IDictionary data, Type t, Coverage aCoverage)
        {
            if (t == typeof (CommercialInsurancePlan))
            {
                data.Add("lblInslbl", "Cert/SSN/ID");
                data.Add("lblInsvalue", ((CommercialCoverage) aCoverage).CertSSNID);
                data.Add("lblTracking", ((CommercialCoverage) aCoverage).TrackingNumber);
                data.Add("lblAuthorization", ((CommercialCoverage) aCoverage).Authorization.AuthorizationNumber);
                data.Add("lblGroupNumber", ((CommercialCoverage) aCoverage).GroupNumber);
                return;
            }

            if (t == typeof (GovernmentMedicaidInsurancePlan))
            {
                data.Add("lblInslbl", "Policy/CIN");
                data.Add("lblInsvalue", ((GovernmentMedicaidCoverage) aCoverage).PolicyCINNumber);
                data.Add("lblTracking", ((GovernmentMedicaidCoverage)aCoverage).TrackingNumber);
                data.Add("lblAuthorization", ((GovernmentMedicaidCoverage) aCoverage).Authorization.AuthorizationNumber);

                return;
            }

            if (t == typeof (WorkersCompensationInsurancePlan))
            {
                data.Add("lblInslbl", "Policy");
                data.Add("lblInsvalue", ((WorkersCompensationCoverage) aCoverage).PolicyNumber);
                data.Add("lblAuthorization", ((WorkersCompensationCoverage) aCoverage).Authorization.AuthorizationNumber);
                return;
            }

            if (t == typeof (GovernmentOtherInsurancePlan))
            {
                data.Add("lblInslbl", "Cert/SSN/ID");
                data.Add("lblInsvalue", ((GovernmentOtherCoverage) aCoverage).CertSSNID);
                data.Add("lblTracking", ((GovernmentOtherCoverage) aCoverage).TrackingNumber);
                data.Add("lblAuthorization", ((GovernmentOtherCoverage) aCoverage).Authorization.AuthorizationNumber);
                data.Add("lblGroupNumber", ((GovernmentOtherCoverage) aCoverage).GroupNumber);
                return;
            }

            if (t == typeof(GovernmentMedicareInsurancePlan))
            {
                data.Add("lblInslbl", "MBI");
                data.Add("lblInsvalue", ((GovernmentMedicareCoverage)aCoverage).FormattedMBINumber);
                if (aCoverage.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    var medicareCoverage = (GovernmentMedicareCoverage) aCoverage;
                    if (medicareCoverage != null &&
                        medicareCoverage.IsMedicareCoverageValidForAuthorization)
                    {
                        data.Add("lblTracking", ((GovernmentMedicareCoverage) aCoverage).TrackingNumber);
                        data.Add("lblAuthorization",
                            ((GovernmentMedicareCoverage) aCoverage).Authorization.AuthorizationNumber);

                    }
                }

                return;
            }

            if (t == typeof (OtherInsurancePlan))
            {
                data.Add("lblInslbl", "Cert/SSN/ID");
                data.Add("lblInsvalue", ((OtherCoverage) aCoverage).CertSSNID);
                data.Add("lblTracking", ((OtherCoverage) aCoverage).TrackingNumber);
                data.Add("lblAuthorization", ((OtherCoverage) aCoverage).Authorization.AuthorizationNumber);
                data.Add("lblGroupNumber", ((OtherCoverage) aCoverage).GroupNumber);
                return;
            }
        }
       
        #endregion

        #region Construction and Finalization

        /// 
        /// <param name="anAccount"></param>
        public DataBuilder(IAccount anAccount)
        {
            iAccount = anAccount;
            SsnFormatter = x => x.AsFormattedString();
        }

        #endregion

        #region Data Elements

        private readonly IAccount iAccount;
        private const int
            TOTAL_LENGTH_OF_PBAR_COMMENTS = 65;

        private const int
            LENGTH_OF_COMMENTS_TWO = 55;

        #endregion
    }
}
