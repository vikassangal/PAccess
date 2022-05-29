using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.HelperClasses;
using log4net;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.UI
{
    /// <summary>
    /// Summary description for AccountActivityService.
    /// </summary>
    public class AccountActivityService
    {
        #region Event Handlers

        #endregion

        #region Methods

        /// <summary>
        /// Shows a dialog for user activity cancellation and 
        /// raises Cancel Activity event incase of activity cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool ConfirmCancelActivity( object sender, EventArgs e )
        {
            DialogResult result = MessageBox.Show(
                UIErrorMessages.INCOMPLETE_ACTIVITY_MSG,
                UIErrorMessages.ACTIVITY_DIALOG_TITLE, MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation );

            if ( result == DialogResult.No )
            {
                return false;
            }

            ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent( sender, e );
            ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen( sender, e );
            
            return true;
        }

        /// <summary>
        /// Releases bed lock for the given Location object
        /// </summary>
        /// <param name="location"></param>
        public static void ReleaseBedLock( Location location )
        {
            var locationPBARBroker = new LocationBrokerProxy();
            locationPBARBroker.ReleaseReservedBed( location, User.GetCurrent().Facility );
        }

        /// <summary>
        /// Releases Account lock for the given Account object
        /// </summary>
        /// <param name="account"></param>
        public static void ReleaseAccountlock( IAccount account )
        {
            if ( account != null )
            {
                var broker = BrokerFactory.BrokerOfType<IAccountBroker>();
                broker.ReleaseLockOn( account.AccountNumber, User.GetCurrent().PBAREmployeeID,
                                     User.GetCurrent().WorkstationID, account.Facility.Oid );
            }
        }

        /// <summary>
        /// Place lock on the selected account and raise account
        /// lock event if lock is acquired successfully
        /// </summary>
        /// <param name="selectedAccount"></param>
        /// <param name="errorMessage"></param>
        public static bool PlaceLockOn( IAccount selectedAccount, string errorMessage )
        {
            if ( selectedAccount != null )
            {
                var broker = BrokerFactory.BrokerOfType<IAccountBroker>();
                User appUser = User.GetCurrent();

                selectedAccount.AccountLock = broker.PlaceLockOn( selectedAccount.AccountNumber,
                                                                 selectedAccount.Patient.MedicalRecordNumber,
                                                                 appUser.PBAREmployeeID,
                                                                 appUser.WorkstationID, selectedAccount.Facility.Oid );

                selectedAccount.Activity.Timeout = broker.AccountLockTimeoutFor( selectedAccount.Activity );

                AccountLock al = selectedAccount.AccountLock;

                Logger.Debug( "IsLocked: " + al.IsLocked +
                            "  AcquiredLocked: " + al.AcquiredLock +
                            "  LockedBy: " + al.LockedBy +
                            "  WorkstationID: " + al.LockedAt );

                // If we did not get a lock on the account from PBAR for whatever reason, 
                // display message to the user and do not proceed with the activity.
                if ( !al.IsLocked )
                {
                    DisplayAccountNotLockedMsg();
                    return false;
                }

                // If the account is locked, but the current user is not the one that 
                // acquired the lock or an update is still pending in Oracle for the account, 
                // display message to the user and do not proceed with the activity.
                if ( ( al.IsLocked && !al.AcquiredLock ) )
                {
                    if ( errorMessage == String.Empty )
                    {
                        DisplayAccountLockedMsg();
                    }
                    else
                    {
                        DisplayAccountLockedMsgWith( errorMessage );
                    }
                    return false;
                }

                if ( al.IsLocked && al.AcquiredLock )
                {
                    Logger.Debug( "Raising Account lock event" );
                    ActivityEventAggregator.GetInstance().RaiseAccountLockEvent( null, new LooseArgs( selectedAccount ) );
                }
                else
                {
                    Logger.Debug( "Did not raise account lock event" );
                    return false;
                }

                return true;
            }

            return false;
        }

        public static void DisplayAccountLockedMsg()
        {
            MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_LOCKED, "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
        }

        public static void DisplayAccountCanceledMsg()
        {
            MessageBox.Show(UIErrorMessages.PATIENT_ACCTS_CANCELED, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1);
        }

        private static void DisplayAccountLockedMsgWith( string errorMessage )
        {
            MessageBox.Show( errorMessage, "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
        }

        private static void DisplayAccountNotLockedMsg()
        {
            MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_NOT_LOCKED, "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
        }

        public static Account SelectedAccountFor( IAccount anAccount )
        {
            Account newAccount = null;

            try
            {
                if ( anAccount != null && anAccount.AccountNumber > 0 )
                {
                    var patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                    Patient aPatient = patientBroker.SparsePatientWith( anAccount.Patient.MedicalRecordNumber,
                                                                       User.GetCurrent().Facility.Code );
                    if ( aPatient != null )
                    {
                        foreach ( Name name in aPatient.Aliases )
                        {
                            aPatient.RemoveAlias( name );
                        }

                        foreach ( Name name in anAccount.Patient.Aliases )
                        {
                            aPatient.AddAlias( name );
                        }

                       
                        if ( anAccount.Activity != null &&
                            ( ( anAccount.Activity.AssociatedActivityType == null ) ||
                            ( anAccount.Activity.AssociatedActivityType != null
                            && !anAccount.Activity.AssociatedActivityType.ToString().Contains( "Offline" ) ) ) )
                        {
                            var broker = BrokerFactory.BrokerOfType<IAccountBroker>();
                            newAccount = broker.AccountFor( aPatient, anAccount.AccountNumber, anAccount.Activity );
                            if (newAccount != null)
                            {
                                if (newAccount.Activity is PostMSERegistrationActivity || 
                                    newAccount.Activity is UCCPostMseRegistrationActivity ||
                                    newAccount.Activity is MaintenanceActivity)
                                {
                                    SetPreviousInsuredEmployer(newAccount);
                                    SetReasonForAccommodation(newAccount);
                                }

                                if (anAccount.AccountLock != null)
                                {
                                    newAccount.AccountLock = anAccount.AccountLock;
                                    newAccount.ActionsLoader = new ActionLoader(newAccount);
                                }
                            }
                        }
                    }
                }
            }
            catch ( RemotingTimeoutException )
            {
                throw new LoadAccountTimeoutException();
            }

            if ( newAccount == null )
            {
                // TLG 08/28/2007 - changed from anAccount as Account to anAccount.AsAccount() [ OTD 34710 ].

                newAccount = anAccount.AsAccount();
            }
            else if ( anAccount.Activity != null
                     && anAccount.Activity.AssociatedActivityType != null
                     && anAccount.Activity.AssociatedActivityType == typeof(ActivatePreRegistrationActivity))
            {
                var localAccount = anAccount.AsAccount();

                // copy streamline activation screen information to newAccount           

                newAccount.AdmitDate = localAccount.AdmitDate;
                newAccount.AdmitSource = localAccount.AdmitSource;
                if ( localAccount.KindOfVisit.IsPreRegistrationPatient )
                {
                    if (newAccount.IsNewBorn)
                    {
                        newAccount.KindOfVisit = VisitType.Inpatient;
                    }
                    else
                    {
                        newAccount.ClearVisitType();
                    }
                   
                }
                
                else
                {
                    newAccount.KindOfVisit = localAccount.KindOfVisit;
                }

                newAccount.HospitalService = localAccount.HospitalService;

                newAccount.Location = localAccount.Location;

                

                newAccount.Patient.NoticeOfPrivacyPracticeDocument =
                    localAccount.Patient.NoticeOfPrivacyPracticeDocument;

                newAccount.ConfidentialityCode = localAccount.ConfidentialityCode;
                newAccount.OptOutName = localAccount.OptOutName;
                newAccount.OptOutLocation = localAccount.OptOutLocation;
                newAccount.OptOutHealthInformation = localAccount.OptOutHealthInformation;
                newAccount.OptOutReligion = localAccount.OptOutReligion;
                newAccount.COSSigned = localAccount.COSSigned;
                newAccount.FacilityDeterminedFlag = localAccount.FacilityDeterminedFlag;

                newAccount.ClinicalComments = localAccount.ClinicalComments;
                newAccount.EmbosserCard = localAccount.EmbosserCard;

                newAccount.ValuablesAreTaken = localAccount.ValuablesAreTaken;
                newAccount.RemoveDefaultCoverage();
            }

            newAccount.Activity = anAccount.Activity;
            // set the account in the occurrence code manager
            OccurrenceCodeManager ocm = OccurrenceCodeManager.Instance;
            ocm.OccurrenceCodesLoader = new OccurrenceCodesLoader();
            ocm.Account = newAccount;
            newAccount.Patient.SetPatientContextHeaderData();
            if (anAccount.Patient.InterFacilityTransferAccount.FromMedicalRecordNumber != 0)
            {
                newAccount.Patient.InterFacilityTransferAccount = anAccount.Patient.InterFacilityTransferAccount;
            }

            return newAccount;
        }

        private static void SetPreviousInsuredEmployer( Account newAccount )
        {
            Coverage primaryCoverage = newAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
            if ( primaryCoverage != null && primaryCoverage.Insured != null
                && primaryCoverage.Insured.Employment != null
                && primaryCoverage.Insured.Employment.Status != null
                && primaryCoverage.Insured.Employment.Employer != null )
            {
                primaryCoverage.Insured.PreviousEmploymentStatusCode
                    = primaryCoverage.Insured.Employment.Status.Code;
                primaryCoverage.Insured.PreviousEmployerName
                    = primaryCoverage.Insured.Employment.Employer.Name;
            }

            Coverage secondaryCoverage = newAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
            if ( secondaryCoverage != null && secondaryCoverage.Insured != null
                && secondaryCoverage.Insured.Employment != null
                && secondaryCoverage.Insured.Employment.Status != null
                && secondaryCoverage.Insured.Employment.Employer != null )
            {
                secondaryCoverage.Insured.PreviousEmploymentStatusCode
                    = secondaryCoverage.Insured.Employment.Status.Code;
                secondaryCoverage.Insured.PreviousEmployerName
                    = secondaryCoverage.Insured.Employment.Employer.Name;
            }
        }

        private static void SetReasonForAccommodation( Account newAccount )
        {
            if ( newAccount.Location != null && newAccount.Location.Bed != null
                && newAccount.Location.Bed.Accomodation != null )
            {
                foreach ( ConditionCode conditionCode in newAccount.ConditionCodes )
                {
                    if ( conditionCode.Code == ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED )
                    {
                        newAccount.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                    }
                    if ( conditionCode.Code == ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE )
                    {
                        newAccount.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                    }
                    if ( newAccount.Diagnosis.isPrivateAccommodationRequested )
                    {
                        newAccount.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                    }
                }
            }
        }

        public static Account CheckPostMSEAccount( Account newAccount )
        {
            if ( newAccount.FinancialClass != null )
            {
                if ( newAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE )
                {
                    newAccount.FinancialClass = new FinancialClass();
                    newAccount.Insurance.RemovePrimaryCoverage();
                    newAccount.Insurance.RemoveSecondaryCoverage();
                }
            }
            if ( !( newAccount.Activity is MaintenanceActivity )
                && newAccount.Insurance != null )
            {
                Coverage coverage1 = newAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
                if ( coverage1 != null )
                {
                    coverage1.RemoveCoverageVerificationData();
                    coverage1.RemoveAuthorization();
                }

                Coverage coverage2 = newAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
                if ( coverage2 != null )
                {
                    coverage2.RemoveCoverageVerificationData();
                    coverage1.RemoveAuthorization();
                }
            }

            // set the account in the occurrence code manager
            OccurrenceCodeManager ocm = OccurrenceCodeManager.Instance;
            ocm.OccurrenceCodesLoader = new OccurrenceCodesLoader();
            ocm.Account = newAccount;

            return newAccount;
        }

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        #endregion

        #region Data Elements

        private static readonly ILog Logger = LogManager.GetLogger( typeof( AccountActivityService ) );

        #endregion
    }
}