using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace PatientAccess.Persistence.AccountCopy
{
    /// <summary>
    /// AccountCopyStrategy is used to create a new account based on a previous account
    /// and update the new account based on certain business rules. See Excel document
    /// "PAS - Fields Copied Forward" for detailed requirements.
    /// </summary>
    [Serializable]
    public abstract class AccountCopyStrategy : MarshalByRefObject, IAccountCopyStrategy
    {
        #region Public Methods

        /// <summary>
        /// Create a copy from the given account
        /// </summary>
        /// <param name="fromAccount">From account.</param>
        /// <returns></returns>
        public virtual Account CopyAccount( IAccount fromAccount )
        {
            var oldAccount = fromAccount.AsAccount();
         
            PerformBeforeCopyActions(oldAccount);
            Account newAccount;
            EMPIFeatureManager = new EMPIFeatureManager(oldAccount.Facility);
            if (EMPIFeatureManager.IsEMPIFeatureEnabled(oldAccount.Activity))
            {
                oldAccount.ResetPatientMRN();
                oldAccount = AssignEMPIAccount(oldAccount);
                newAccount = CreateInstanceFrom(oldAccount);
            }
            else
            {
                newAccount = CreateInstanceFrom(fromAccount);
            }
            
            EditGeneralInformationUsing( newAccount, oldAccount );
            EditDemographicsUsing( newAccount, oldAccount );
            EditEmploymentUsing( newAccount, oldAccount );
            EditDiagnosisUsing( newAccount, oldAccount );
            EditClinicalUsing( newAccount, oldAccount );
            EditInsuranceUsing( newAccount, oldAccount );
            EditGuarantorUsing( newAccount, oldAccount );
            EditBillingUsing( newAccount, oldAccount );
            EditLiabilityUsing( newAccount, oldAccount );
            EditPaymentUsing( newAccount, oldAccount );
            EditContactsUsing( newAccount, oldAccount );
            EditRegulatoryUsing( newAccount, oldAccount );

            newAccount.ResetChangeTracking();

            PerformAfterCopyActions( newAccount );

            return newAccount;

        }

        private Account AssignEMPIAccount(Account oldAccount)
        {
            oldAccount.OverLayEMPIData();
            oldAccount.Patient.SetPatientContextHeaderData();
            oldAccount.Activity.EmpiPatient = null;
            return oldAccount;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Creates the instance from.
        /// </summary>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected abstract Account CreateInstanceFrom( IAccount oldAccount );
        protected abstract Account CreateInstanceFrom( Account oldAccount );

        /// <summary>
        /// Edits the billing.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditBillingUsing( Account newAccount, Account oldAccount )
        {

            ResetOccurrenceCodes( newAccount );
            newAccount.ConditionCodes.Clear();

            newAccount.OccurrenceSpans.Clear();

            ResetValueCodes( newAccount );

            return;

        }

        /// <summary>
        /// Resets the value codes.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        private static void ResetValueCodes( Account newAccount )
        {

            // Yeah, we could do something fancy, but probably better
            // just to brute-force this one
            newAccount.ValueAmount1 = 0;
            newAccount.ValueAmount2 = 0;
            newAccount.ValueAmount3 = 0;
            newAccount.ValueAmount4 = 0;
            newAccount.ValueAmount5 = 0;
            newAccount.ValueAmount6 = 0;
            newAccount.ValueAmount7 = 0;
            newAccount.ValueAmount8 = 0;

            newAccount.ValueCode1 = String.Empty;
            newAccount.ValueCode2 = String.Empty;
            newAccount.ValueCode3 = String.Empty;
            newAccount.ValueCode4 = String.Empty;
            newAccount.ValueCode5 = String.Empty;
            newAccount.ValueCode6 = String.Empty;
            newAccount.ValueCode7 = String.Empty;
            newAccount.ValueCode8 = String.Empty;

        }

        /// <summary>
        /// Edits the clinical.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditClinicalUsing( Account newAccount, Account oldAccount )
        {
            newAccount.EmbosserCard = string.Empty;

            if (newAccount.ShouldPCPCarryForward())
            {
                AccountBroker.BuildPrimaryCarePhysicanRelationship(newAccount);
                newAccount.RemoveAllPhysicianRelationshipsExcept(new PhysicianRelationship((PhysicianRole.PrimaryCare().Role()), new Physician()));
            }

            else
            {
                newAccount.RemovePhysicianRelationships();
            }

            newAccount.ClinicalComments = string.Empty;
            newAccount.Pregnant = new YesNoFlag();

            newAccount.PreopDate = DateTime.MinValue;
            newAccount.AccountCreatedDate = DateTime.Now;

            newAccount.IsPatientInClinicalResearchStudy = new YesNoFlag( YesNoFlag.CODE_BLANK );

            if ( newAccount.ClinicalResearchStudies != null && newAccount.ClinicalResearchStudies.Any() )
            {
                newAccount.ClearResearchStudies();
            }

            newAccount.RightCareRightPlace.RCRP = new YesNoFlag( YesNoFlag.CODE_BLANK );
            newAccount.RightCareRightPlace.LeftOrStayed = new LeftOrStayed();
            newAccount.LeftWithOutBeingSeen = new YesNoFlag( YesNoFlag.CODE_BLANK );
            newAccount.LeftWithoutFinancialClearance = new YesNoFlag( YesNoFlag.CODE_BLANK );
            return;
        }

        /// <summary>
        /// Edits the contact.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditContactsUsing( Account newAccount, Account oldAccount )
        {
            return;
        }

        /// <summary>
        /// Edits the demographics for.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditDemographicsUsing( Account newAccount, Account oldAccount )
        {

            newAccount.AdmitDate = DateTime.MinValue;
            newAccount.ArrivalTime = DateTime.MinValue;
            newAccount.DischargeDate = DateTime.MinValue;
            newAccount.AdmitSource = new AdmitSource();
            newAccount.DischargeDisposition = new DischargeDisposition();
            newAccount.DischargeStatus = new DischargeStatus();
            newAccount.PendingDischarge = String.Empty;
            newAccount.PreDischargeLocation = new Location();
            newAccount.TransferDate = DateTime.MinValue;
            newAccount.TransferredFromHospitalService = new HospitalService();
            newAccount.BirthTimeIsEntered = false;
            return;

        }

        /// <summary>
        /// Edits the diagnosis.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditDiagnosisUsing( Account newAccount, Account oldAccount )
        {

            ResetHospitalClinics( newAccount );
            newAccount.Diagnosis = new Diagnosis();
            newAccount.Location = new Location();
            newAccount.HospitalService = new HospitalService();
            newAccount.CodedDiagnoses = new CodedDiagnoses();
            newAccount.TenetCare = new YesNoFlag();
            newAccount.AdmitSource = new AdmitSource();
            newAccount.ScheduleCode = new ScheduleCode(); //Appointment
            newAccount.ArrivalTime = DateTime.MinValue;
            newAccount.ReferralType = new ReferralType();
            newAccount.ReferralFacility = new ReferralFacility();
            newAccount.ModeOfArrival = new ModeOfArrival();
            newAccount.ReAdmitCode = new ReAdmitCode();
            newAccount.KindOfVisit = new VisitType();
            newAccount.Reregister = new YesNoFlag();
            newAccount.AlternateCareFacility = String.Empty;
            newAccount.CptCodes = new Dictionary<int, string>();

            return;

        }

        /// <summary>
        /// Edits the employment.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditEmploymentUsing( Account newAccount, Account oldAccount )
        {

            newAccount.ClergyVisit = YesNoFlag.Blank;
            newAccount.ValuablesAreTaken = YesNoFlag.Blank;

            return;

        }

        /// <summary>
        /// Edits the general information using.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditGeneralInformationUsing( Account newAccount, Account oldAccount )
        {

            newAccount.Oid = PersistentModel.NEW_OID;
            newAccount.AccountNumber = 0;
            newAccount.ClearFusNotes();
            newAccount.ClearPersistedFusNotes();
            newAccount.PreMSECopiedAccountNumber = 0;
            newAccount.LastMaintenanceDate = 0L;
            newAccount.LastMaintenanceLogNumber = 0L;
            newAccount.AbstractExists = false;
            newAccount.AccountCreatedDate = DateTime.Now;
            newAccount.IsNew = true;
            newAccount.ResourceListProvided = new YesNoFlag();
            newAccount.UpdateLogNumber = 0L;
            newAccount.IsNewBorn = false;

            return;
        }

        /// <summary>
        /// Edits the guarantor.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditGuarantorUsing( Account newAccount, Account oldAccount )
        {
            if ( newAccount.Guarantor != null )
            {
                newAccount.Guarantor.DataValidationTicket = null;
                newAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).CellPhoneConsent = new CellPhoneConsent();
                newAccount.OldGuarantorCellPhoneConsent = new CellPhoneConsent();
                newAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).PhoneNumber =
                    oldAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).PhoneNumber;
                newAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber =
                    oldAccount.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType()).PhoneNumber;
            }
        }

        /// <summary>
        /// Edits the insurance.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditInsuranceUsing( Account newAccount, Account oldAccount )
        {
            newAccount.MedicalGroupIPA = oldAccount.Patient.MedicalGroupIPA;

            if ( newAccount.Insurance != null )
            {
                if (oldAccount.FinancialClass != null
                    && oldAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE)
                {
                    newAccount.FinancialClass = new FinancialClass();

                    if (oldAccount.KindOfVisit != null
                        && (oldAccount.IsEDorUrgentCarePremseAccount))
                    {
                        if (oldAccount.Insurance != null &&
                            oldAccount.Insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder()) != null)
                        {
                            var insurancePlan =
                                oldAccount.Insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder())
                                    .InsurancePlan;
                            if (insurancePlan != null && insurancePlan.PlanID == PRE_MSE_INSURANCE_PLAN_ID)
                            {
                                newAccount.Insurance.RemovePrimaryCoverage();
                                newAccount.Insurance.RemoveSecondaryCoverage();
                            }
                        }
                        return;
                    }
                }
                newAccount.RemoveDefaultCoverage();
                if ( newAccount.Insurance.PrimaryCoverage != null )
                {
                    newAccount.Insurance.ForceChangedStatusFor( "PrimaryCoverage" );
                    newAccount.Insurance.PrimaryCoverage.RemoveCoverageVerificationData();
                    newAccount.Insurance.PrimaryCoverage.RemoveCoveragePayorData();
                }

                if ( newAccount.Insurance.SecondaryCoverage != null )
                {
                    newAccount.Insurance.ForceChangedStatusFor( "SecondaryCoverage" );
                    newAccount.Insurance.SecondaryCoverage.RemoveCoverageVerificationData();
                    newAccount.Insurance.SecondaryCoverage.RemoveCoveragePayorData();
                }
            }
          
            if ( newAccount.Activity != null )
            {
                if ( ( newAccount.Activity.GetType() == typeof( ActivatePreRegistrationActivity ) ) ||
                     ( newAccount.Activity.AssociatedActivityType != null &&
                       newAccount.Activity.AssociatedActivityType.GetType() == typeof( ActivatePreRegistrationActivity ) ) )
                {
                    newAccount.MedicareSecondaryPayor = oldAccount.MedicareSecondaryPayor;
                }
                else if ( newAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) || newAccount.Activity.GetType() == typeof( PreAdmitNewbornActivity ) )
                {
                    newAccount.MedicareSecondaryPayor = new MedicareSecondaryPayor();
                }
                else
                {
                    newAccount.MedicareSecondaryPayor =
                        MedicareSecondaryPayor.GetPartiallyCopiedForwardMSPFrom( oldAccount.MedicareSecondaryPayor );
                }
            }

            return;
        }

        /// <summary>
        /// Edits the liability.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditLiabilityUsing( Account newAccount, Account oldAccount )
        {

            if ( newAccount.Insurance.PrimaryCoverage != null )
            {
                newAccount.Insurance.PrimaryCoverage.RemoveCoverageLiabilityData();
            }

            if ( newAccount.Insurance.SecondaryCoverage != null )
            {
                newAccount.Insurance.SecondaryCoverage.RemoveCoverageLiabilityData();
            }

            newAccount.BalanceDue = 0M;
            newAccount.BillHasDropped = false;
            newAccount.LastChargeDate = DateTime.MinValue;
            newAccount.MonthlyPayment = 0M;
            newAccount.NumberOfMonthlyPayments = 0;
            newAccount.Payment = new Payment();
            newAccount.PreviousTotalCurrentAmtDue = 0M;
            newAccount.RequestedPayment = 0M;
            newAccount.TotalCurrentAmtDue = 0M;
            newAccount.TotalPaid = 0M;
            newAccount.TotalCharges = 0M;
            newAccount.OriginalMonthlyPayment = 0M;

            return;

        }

        /// <summary>
        /// Edits the payment.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditPaymentUsing( Account newAccount, Account oldAccount )
        {
            return;
        }

        /// <summary>
        /// Edits the regulatory.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected virtual void EditRegulatoryUsing( Account newAccount, Account oldAccount )
        {

            newAccount.ConfidentialityCode = new ConfidentialCode();
            newAccount.COSSigned = new ConditionOfService();
            newAccount.FacilityDeterminedFlag = new FacilityDeterminedFlag();
            newAccount.RightToRestrict = new YesNoFlag();
            newAccount.ShareDataWithPublicHieFlag = new YesNoFlag(YesNoFlag.CODE_YES);
            newAccount.ShareDataWithPCPFlag = new YesNoFlag(YesNoFlag.CODE_NO);
            if (newAccount.Activity.IsQuickAccountCreationActivity()
                        || newAccount.Activity.IsQuickAccountMaintenanceActivity()
                        || newAccount.Activity.IsPAIWalkinOutpatientCreationActivity()
                        || !NotifyPCPDataFeatureManager.IsNotifyPCPEnabledforaccount(newAccount))
            {
                newAccount.ShareDataWithPCPFlag = YesNoFlag.No;
            }
            // resetting values of COBReceived and IMFM Received
            newAccount.COBReceived = new YesNoFlag();
            newAccount.IMFMReceived = new YesNoFlag();
            return;
        }

        /// <summary>
        /// Performs the after copy actions.
        /// </summary>
        /// <param name="anAccount">An account.</param>
        protected virtual void PerformAfterCopyActions( Account anAccount )
        {

            ForceChangedStatusForNppValues( anAccount );
            ForceChangedStatusForOptOutValues( anAccount );

        }

        /// <summary>
        /// Performs the before copy actions.
        /// </summary>
        /// <param name="anAccount">An account.</param>
        protected virtual void PerformBeforeCopyActions( Account anAccount )
        {
            return;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Forces the changed status for NPP values.
        /// </summary>
        /// <param name="anAccount">An account.</param>
        private static void ForceChangedStatusForNppValues( IAccount anAccount )
        {

            if ( anAccount.Patient.NoticeOfPrivacyPracticeDocument != null )
            {
                anAccount.Patient.NoticeOfPrivacyPracticeDocument.ForceChangedStatusFor( "SignedOnDate" );
                anAccount.Patient.NoticeOfPrivacyPracticeDocument.ForceChangedStatusFor( "SignatureStatus" );
            }

        }

        /// <summary>
        /// Forces the changed status for opt out values.
        /// </summary>
        /// <param name="anAccount">An account.</param>
        private static void ForceChangedStatusForOptOutValues( Account anAccount )
        {

            anAccount.ForceChangedStatusFor( "OptOutHealthInformation" );
            anAccount.ForceChangedStatusFor( "OptOutLocation" );
            anAccount.ForceChangedStatusFor( "OptOutName" );
            anAccount.ForceChangedStatusFor( "OptOutReligion" );

        }


        /// <summary>
        /// Edits the hospital clinics.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        private static void ResetHospitalClinics( Account newAccount )
        {
            for ( int clinicIndex = 0; clinicIndex < newAccount.Clinics.Count; clinicIndex++ )
            {
                newAccount.Clinics[clinicIndex] = new HospitalClinic();
            }
        }

        /// <summary>
        /// Edits the occurrence codes.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        private static void ResetOccurrenceCodes( Account newAccount )
        {

            for ( var occurrenceCodeIndex = newAccount.OccurrenceCodes.Count - 1; occurrenceCodeIndex > -1; occurrenceCodeIndex-- )
            {
                var occurrenceCode =
                    newAccount.OccurrenceCodes[occurrenceCodeIndex] as OccurrenceCode;

                if ( ( occurrenceCode == null ) ||
                    ( ( occurrenceCode.Code != OccurrenceCode.OCCURRENCECODE_RETIREDATE ) &&
                      ( occurrenceCode.Code != OccurrenceCode.OCCURRENCECODE_SPOUSERETIRED ) ) )
                {
                    newAccount.RemoveOccurrenceCode( occurrenceCode );
                }
            }

        }

        #endregion Private Methods
        
        #region Constants

        private const string PRE_MSE_INSURANCE_PLAN_ID = "EDL81";

        #endregion
        private static IAccountBroker AccountBroker
        {
            get
            {
                accountBroker = accountBroker ?? BrokerFactory.BrokerOfType<IAccountBroker>();

                return accountBroker;
            }
        }

        private static IAccountBroker accountBroker;

        private IEMPIFeatureManager EMPIFeatureManager { get; set; }

        private NotifyPCPDataFeatureManager NotifyPCPDataFeatureManager = new NotifyPCPDataFeatureManager();
    }
}
