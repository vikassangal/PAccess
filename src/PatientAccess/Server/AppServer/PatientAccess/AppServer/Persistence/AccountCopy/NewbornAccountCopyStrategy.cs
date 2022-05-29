using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace PatientAccess.Persistence.AccountCopy
{
    /// <summary>
    /// Given the mother's account, create a newborn account
    /// </summary>
    internal class NewbornAccountCopyStrategy : BinaryCloneAccountCopyStrategy
    {
        #region Constants 

        private const string SSN_STATUS = "None";
        private const string MARITAL_STATUS_SINGLE = "S";
        private const string FLORIDA_STATE_CODE = "FL";
        private const string NEWBORN = "NewBorn";
        private const string WALKIN_SCHEDULE_CODE = "W";

        #endregion Constants 

        #region Protected Methods 

        protected override void EditGeneralInformationUsing(Account newAccount, Account oldAccount)
        {
            base.EditGeneralInformationUsing(newAccount, oldAccount);
            newAccount.IsNewBorn = true;
        }

        /// <summary>
        /// Edits the billing.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditBillingUsing(Account newAccount, Account oldAccount)
        {
            base.EditBillingUsing(newAccount, oldAccount);

            newAccount.OccurrenceCodes.Clear();
            newAccount.OccurrenceCodes.Add(new OccurrenceCode());
            //SR 1557 don't add illness occurrence code for Pre-Admit Newborn
            if (newAccount.Activity != null && !newAccount.Activity.IsPreAdmitNewbornActivity())
                AddIllnessOccurrenceCodeUsing(newAccount, oldAccount);
        }

        /// <summary>
        /// Edits the clinical.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditClinicalUsing(Account newAccount, Account oldAccount)
        {
            base.EditClinicalUsing(newAccount, oldAccount);

            newAccount.Smoker = YesNoFlag.No;
            newAccount.Pregnant = YesNoFlag.No;
            newAccount.Bloodless = newAccount.Activity != null && newAccount.Activity.IsNewBornRelatedActivity()
                                       ? oldAccount.Bloodless
                                       : YesNoFlag.Blank;
        }

        /// <summary>
        /// Edits the contact.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditContactsUsing(Account newAccount, Account oldAccount)
        {
            base.EditContactsUsing(newAccount, oldAccount);

            ClearSecondEmergencyContact(newAccount);
            SetMotherAsFirstEmergencyContact(newAccount, oldAccount);
        }

        /// <summary>
        /// Edits the demographics for.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditDemographicsUsing(Account newAccount, Account oldAccount)
        {
            base.EditDemographicsUsing(newAccount, oldAccount);
            //SR 1557, only set Admit Date when it is newborn activity, don't set it for Pre-Admit Newborn activity
            if (newAccount.Activity != null && newAccount.Activity.IsAdmitNewbornActivity())
                DeriveAdmittingTimeFor(newAccount);

            var newbornPatient =
                new Patient(PersistentModel.NEW_OID,
                            PersistentModel.NEW_VERSION,
                            String.Empty,
                            oldAccount.Patient.LastName,
                            String.Empty,
                            newAccount.AdmitDate,
                            new Gender(),
                            oldAccount.Patient.Religion,
                            oldAccount.Patient.PlaceOfWorship);

            newbornPatient.MothersAccount = oldAccount;
            newbornPatient.Facility = oldAccount.Facility;
            newbornPatient.Race = oldAccount.Patient.Race;
            newbornPatient.Nationality = oldAccount.Patient.Nationality;
            newbornPatient.Race2 = oldAccount.Patient.Race2;
            newbornPatient.Nationality2 = oldAccount.Patient.Nationality2;

            newbornPatient.Ethnicity = oldAccount.Patient.Ethnicity;
            newbornPatient.Ethnicity2 = oldAccount.Patient.Ethnicity2;
            newbornPatient.Descent = oldAccount.Patient.Descent;
            newbornPatient.Descent2 = oldAccount.Patient.Descent2;
            newbornPatient.Language = oldAccount.Patient.Language;
            newbornPatient.OtherLanguage = oldAccount.Patient.OtherLanguage;

            CopyContactsFromMotherUsing(oldAccount, newbornPatient);
            newbornPatient.PatientContextHeaderData = oldAccount.Patient.PatientContextHeaderData;
           
            newAccount.Patient = newbornPatient;

            if (newAccount.Activity != null && newAccount.Activity.IsPreAdmitNewbornActivity())
            {
                newAccount.AdmitDate = oldAccount.AdmitDate;
                newAccount.PreopDate = oldAccount.PreopDate;
            }
            
            DeriveSocialSecurityNumberStatusUsing(newAccount, oldAccount);
            SetMaritalStatusToSingleUsing(newAccount);
        }

        /// <summary>
        /// Edits the diagnosis.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditDiagnosisUsing(Account newAccount, Account oldAccount)
        {
            base.EditDiagnosisUsing(newAccount, oldAccount);

            newAccount.KindOfVisit = VisitType.Inpatient;
            newAccount.Diagnosis.Procedure = "Birth";

            DeriveDiagnosisConditionUsing(newAccount);
            SetAdmitSourceForNewbornUsing(newAccount, oldAccount);

            if (newAccount.Activity != null && newAccount.Activity.IsNewBornRelatedActivity())
            {
                SetScheduleCodeToWalkInUsing(newAccount, oldAccount);
            }
        }

        /// <summary>
        /// Edits the employment.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditEmploymentUsing(Account newAccount, Account oldAccount)
        {
            base.EditEmploymentUsing(newAccount, oldAccount);

            newAccount.Patient.Employment =
                new Employment
                    {
                        Status = EmploymentStatus.NewNotEmployed()
                    };
            newAccount.Patient.Employment.Employer = new Employer {Name = EmploymentStatus.NewNotEmployed().Description};
            DerivePlaceOfBirthUsing(oldAccount, newAccount.Patient);
        }

        /// <summary>
        /// Edits the guarantor.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditGuarantorUsing(Account newAccount, Account oldAccount)
        {
            base.EditGuarantorUsing(newAccount, oldAccount);

            SetMotherAsGuarantorUsing(newAccount, oldAccount);
            newAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).PhoneNumber =
                oldAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).PhoneNumber;
            newAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber =
                oldAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber;
        }

        /// <summary>
        /// Edits the insurance.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditInsuranceUsing(Account newAccount, Account oldAccount)
        {
            base.EditInsuranceUsing(newAccount, oldAccount);

            if (newAccount.Insurance != null)
            {
                RemoveInsuredRelationshipFor(newAccount, newAccount.Insurance.PrimaryCoverage);
                RemoveInsuredRelationshipFor(newAccount, newAccount.Insurance.SecondaryCoverage);
            }
        }
        protected override void EditRegulatoryUsing(Account newAccount, Account oldAccount)
        {
            newAccount.ConfidentialityCode = new ConfidentialCode();
            newAccount.COSSigned = new ConditionOfService();
            newAccount.FacilityDeterminedFlag = new FacilityDeterminedFlag();
            newAccount.RightToRestrict = new YesNoFlag();
            newAccount.Patient.HospitalCommunicationOptIn = oldAccount.Patient.HospitalCommunicationOptIn;
            newAccount.COBReceived = new YesNoFlag();
            newAccount.IMFMReceived = new YesNoFlag();
            YesNoFlag DefaultNotifyPCPValue = FeatureManagerPCP.DefaultNotifyPCPForFacility(oldAccount.Facility);
            YesNoFlag DefaultShareDataWithHIE = FeatureManagerHIE.DefaultShareHieDataForFacility(oldAccount.Facility);
            if (newAccount.Activity.IsNewBornRelatedActivity())
            {                
                newAccount.ShareDataWithPCPFlag = new YesNoFlag(YesNoFlag.CODE_NO);
                newAccount.ShareDataWithPublicHieFlag = new YesNoFlag(YesNoFlag.CODE_NO);
            }
            else
            {
                newAccount.ShareDataWithPCPFlag = DefaultNotifyPCPValue == YesNoFlag.Blank ? YesNoFlag.No : DefaultNotifyPCPValue;
                newAccount.ShareDataWithPublicHieFlag = DefaultShareDataWithHIE == YesNoFlag.Blank ? YesNoFlag.No : DefaultShareDataWithHIE;                
            }
            return;
        }
        #endregion Protected Methods 

        #region Private Methods 

        /// <summary>
        /// Adds the illness occurrence code using.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        private static void AddIllnessOccurrenceCodeUsing(Account newAccount, IAccount oldAccount)
        {
            var occurrenceCodeBroker =
                BrokerFactory.BrokerOfType<IOccuranceCodeBroker>();
            OccurrenceCode occurrenceCode =
                occurrenceCodeBroker.OccurrenceCodeWith(oldAccount.Facility.Oid,
                                                        OccurrenceCode.OCCURRENCECODE_ILLNESS);
            occurrenceCode.OccurrenceDate = newAccount.AdmitDate;
            newAccount.AddOccurrenceCode(occurrenceCode);
        }

        /// <summary>
        /// Clears the relationship to first emergency contact.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        private static void ClearSecondEmergencyContact(Account newAccount)
        {
            newAccount.EmergencyContact2 = new EmergencyContact();
        }

        /// <summary>
        /// Copies the contacts from mother using.
        /// </summary>
        /// <param name="oldAccount">The old account.</param>
        /// <param name="newbornPatient">The newborn patient.</param>
        private static void CopyContactsFromMotherUsing(IAccount oldAccount, Party newbornPatient)
        {
            foreach (ContactPoint contact in oldAccount.Patient.ContactPoints)
            {
                newbornPatient.AddContactPoint(contact);
            }
        }

        /// <summary>
        /// Derives the admitting time.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        private static void DeriveAdmittingTimeFor(IAccount newAccount)
        {
            var timeBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
            Facility facility = newAccount.Facility;
            DateTime facilityDateTime = timeBroker.TimeAt(facility.GMTOffset, facility.DSTOffset);
            newAccount.AdmitDate = facilityDateTime.Date;
        }

        /// <summary>
        /// Derives the diagnosis condition using.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        private static void DeriveDiagnosisConditionUsing(Account newAccount)
        {
            var illness =
                new Illness {Onset = newAccount.AdmitDate};

            newAccount.Diagnosis.Condition = illness;
        }

        /// <summary>
        /// Derives the place of birth using.
        /// </summary>
        /// <param name="oldAccount">The old account.</param>
        /// <param name="newbornPatient">The newborn patient.</param>
        private static void DerivePlaceOfBirthUsing(IAccount oldAccount, Patient newbornPatient)
        {
            newbornPatient.PlaceOfBirth = oldAccount.Facility.FacilityState.Description;
        }

        /// <summary>
        /// Derives the social security number status using.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        private static void DeriveSocialSecurityNumberStatusUsing(IAccount newAccount, IAccount oldAccount)
        {
            var ssnPbarBroker = BrokerFactory.BrokerOfType<ISSNBroker>();
            SocialSecurityNumberStatus socialSecurityNumberStatus = ssnPbarBroker.SSNStatusWith(
                oldAccount.Facility.Oid, SSN_STATUS);

            if ( oldAccount.Facility.FacilityState.Code == FLORIDA_STATE_CODE ||
                oldAccount.Facility.FacilityState.IsSouthCarolina || 
                oldAccount.Facility.IsBaylorFacility() )
            {
                socialSecurityNumberStatus = ssnPbarBroker.SSNStatusWith(oldAccount.Facility.Oid, NEWBORN);
            }

            //var socialSecurityNumberStatus =
            //    oldAccount.Facility.FacilityState.Code == FLORIDA_STATE_CODE ? 
            //    ssnPbarBroker.SSNStatusWith(oldAccount.Facility.Oid, NEWBORN) : 
            //    ssnPbarBroker.SSNStatusWith(oldAccount.Facility.Oid, SSN_STATUS);

            ISsnFactory ssnFactory = new SsnFactoryCreator(oldAccount).GetSsnFactory();
            newAccount.Patient.SocialSecurityNumber = ssnFactory.GetValidSocialSecurityNumberFor(
                oldAccount.Facility.FacilityState, newAccount.AdmitDate, socialSecurityNumberStatus);
        }

        /// <summary>
        /// Removes the insured relationship for.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="coverage">The coverage.</param>
        private static void RemoveInsuredRelationshipFor(IAccount newAccount, Coverage coverage)
        {
            if (coverage == null || coverage.Insured == null) return;

            foreach (Relationship relationship in coverage.Insured.Relationships)
            {
                coverage.Insured.RemoveRelationship(relationship);
                newAccount.Patient.RemoveRelationship(relationship);
            }
        }

        /// <summary>
        /// Sets the admit source for newborn.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        private static void SetAdmitSourceForNewbornUsing(Account newAccount, IAccount oldAccount)
        {
            var admitSourceBroker = BrokerFactory.BrokerOfType<IAdmitSourceBroker>();
            newAccount.AdmitSource =
                admitSourceBroker.AdmitSourceForNewBorn(oldAccount.Facility.Oid);
        }

        /// <summary>
        /// Sets the marital status to single.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        private static void SetMaritalStatusToSingleUsing(IAccount newAccount)
        {
            var demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();

            newAccount.Patient.MaritalStatus =
                demographicsBroker.MaritalStatusWith(newAccount.Facility.Oid, MARITAL_STATUS_SINGLE);
        }

        /// <summary>
        /// Sets the mother as guarantor.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        private static void SetMotherAsGuarantorUsing(Account newAccount, IAccount oldAccount)
        {
            Guarantor guarantor = oldAccount.Patient.CopyAsGuarantor();

            var relationShipTypeBroker =
                BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
            RelationshipType relationshipType =
                relationShipTypeBroker.RelationshipTypeWith(
                    oldAccount.Facility.Oid,
                    RelationshipType.RELATIONSHIPTYPE_NATURAL_CHILD);
            newAccount.GuarantorIs(guarantor, relationshipType);
        }

        /// <summary>
        /// Sets the mother as second emergency contact.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        private static void SetMotherAsFirstEmergencyContact(Account newAccount, IAccount oldAccount)
        {
            var emergencyContact =
                new EmergencyContact
                    {
                        Name = oldAccount.Patient.Name.AsFormattedName(),
                        Sex = oldAccount.Patient.Sex,
                        Employment = oldAccount.Patient.Employment,
                        SocialSecurityNumber = oldAccount.Patient.SocialSecurityNumber
                    };

            foreach (ContactPoint contactPoint in oldAccount.Patient.ContactPoints)  
            {
                if (!contactPoint.TypeOfContactPoint.Equals(TypeOfContactPoint.NewMailingContactPointType())) continue;
                var oldAddress = contactPoint.Address;
                var newAddress = new Address()
                {
                    Address1 = ExtractAddress(oldAddress.Address1),
                    Address2 = String.Empty,
                    City = oldAddress.City,
                    State = oldAddress.State,
                    County = oldAddress.County,
                    Country = oldAddress.Country,
                    ZipCode = oldAddress.ZipCode
                };
                var newPhysical =
                    new ContactPoint(TypeOfContactPoint.NewPhysicalContactPointType())
                    {
                        Address = newAddress ,
                        PhoneNumber = contactPoint.PhoneNumber,
                        CellPhoneNumber = contactPoint.CellPhoneNumber,
                        EmailAddress = contactPoint.EmailAddress
                    };

                emergencyContact.AddContactPoint(newPhysical);
            }


            newAccount.EmergencyContact1 = emergencyContact;
            var relationShipTypeBroker =
                BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
            RelationshipType relationshipType =
                relationShipTypeBroker.RelationshipTypeWith(oldAccount.Facility.Oid,
                                                            RelationshipType.RELATIONSHIPTYPE_MOTHER);
            var aRelationship =
                new Relationship(relationshipType,
                                 newAccount.Patient.GetType(),
                                 newAccount.EmergencyContact1.GetType());
            newAccount.EmergencyContact1.RelationshipType = relationshipType;

            newAccount.EmergencyContact1.AddRelationship(aRelationship);
        }

        /// <summary>
        /// Sets the schedule code to walk in using.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        private static void SetScheduleCodeToWalkInUsing(Account newAccount, IAccount oldAccount)
        {
            var scheduleCodeBroker =
                BrokerFactory.BrokerOfType<IScheduleCodeBroker>();
            newAccount.ScheduleCode =
                scheduleCodeBroker.ScheduleCodeWith(oldAccount.Facility.Oid,
                                                    WALKIN_SCHEDULE_CODE);
        }

        private static string ExtractAddress(string Address1)
        {
            if (Address1.Length > 25)
            {
                Address1 = Address1.Substring(0, 25);
            }
            return Address1;
        }
        #endregion Private Methods 

        ShareHIEDataFeatureManager FeatureManagerHIE = new ShareHIEDataFeatureManager();
        NotifyPCPDataFeatureManager FeatureManagerPCP = new NotifyPCPDataFeatureManager();
    }
}