using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services.Protocols;
using Extensions;
using Extensions.DB2Persistence;
using Extensions.Exceptions;
using Extensions.PersistenceCommon;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence.Utilities;
using PatientAccess.RemotingServices;
using PatientAccess.Utilities;
using log4net;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for AccountPBARBroker.
    /// </summary>
    [Serializable]
    public class AccountPBARBroker : AbstractPBARBroker, IAccountBroker
    {
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// IsTxnAllowed - test the number of patient-type-changing transactions for this patient
        /// </summary>
        /// <param name="anAccountFacilityCode"> A string, Account Facility Code</param>
        /// <param name="accountNumber">Account Number</param>
        /// <param name="activity"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Unhandled Exception.</exception>
        public bool IsTxnAllowed( string anAccountFacilityCode, long accountNumber, Activity activity )
        {
            bool isAllowed = true;

            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                Facility facility = facilityBroker.FacilityWith( anAccountFacilityCode );

                cmd = CommandFor( "CALL " + SP_TXN_COUNT_FOR +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_ACCT +
                    "," + PARAM_TXN + ")",
                    CommandType.Text,
                    facility
                    );
                cmd.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;
                cmd.Parameters[PARAM_ACCT].Value = accountNumber;

                if ( activity.GetType().Equals( typeof( CancelInpatientDischargeActivity ) ) ||
                    activity.GetType().Equals( typeof( CancelOutpatientDischargeActivity ) ) )
                {
                    cmd.Parameters[PARAM_TXN].Value =
                        CancelDischargeTransactionCoordinator.CANCEL_DISCHARGE_TRANSACTION_ID;
                }
                else if ( activity.GetType().Equals( typeof( TransferInToOutActivity ) ) )
                {
                    cmd.Parameters[PARAM_TXN].Value =
                        TransferInPatientToOutPatientTransactionCoordinator.TRANSFER_INPATIENT_OUTPATIENT_TRANSACTION_ID;
                }
                else if ( activity.GetType().Equals( typeof( TransferOutToInActivity ) ) )
                {
                    cmd.Parameters[PARAM_TXN].Value =
                        TransferOutPatientToInPatientTransactionCoordinator.TRANSFER_OUTPATIENT_INPATIENT_TRANSACTION_ID;
                }

                reader = ExecuteReader( cmd );

                if ( reader.Read() )
                {
                    long reccnt = reader.GetInt64( COL_RECCNT );

                    if ( reccnt > 0 )
                    {
                        isAllowed = false;
                    }
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception.", ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return isAllowed;
        }

        /// <summary>
        /// This method is currently used for View Accounts menu, to display details for a non-purged account (HistoricalAccountViews\NonPurgedAccountView). 
        /// It returns Account object with almost all the fields populated. 
        /// This method signature is similar to AccountFor method, 
        /// but here more of the Account object's fields would get populated with data.
        /// </summary>
        /// <param name="anAccountProxy">An account proxy.</param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public Account AccountDetailsFor( AccountProxy anAccountProxy )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            // this is necessary to fully populate the Patient

            anAccountProxy = AccountProxyFor( anAccountProxy.Facility.Code,
                anAccountProxy.Patient,
                anAccountProxy.Patient.MedicalRecordNumber,
                anAccountProxy.AccountNumber );

            try
            {
                cmd = CommandFor(
                    String.Format( "CALL {0}({1},{2},{3})",
                    SP_ACCOUNT_FOR,
                    PARAM_HSPNUMBER,
                    PARAM_MRC,
                    PARAM_ACCOUNTNUMBER ),
                    CommandType.Text,
                    anAccountProxy.Patient.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value =
                    anAccountProxy.Patient.Facility.Oid;
                cmd.Parameters[PARAM_MRC].Value =
                    anAccountProxy.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value =
                    anAccountProxy.AccountNumber;

                reader = ExecuteReader( cmd );
                if ( reader.Read() )
                {
                    BuildPhysicianRelationships( reader,
                        physicianBroker,
                        anAccountProxy.Patient.Facility,
                        anAccountProxy );

                    BuildAdmitSources( reader, anAccountProxy );
                    BuildAdmittingCategory(reader, anAccountProxy);
                    SetPrimaryPlanID( reader, anAccountProxy );
                    GetAccountCreatedDate( anAccountProxy );
                    SetShortRegisteredAccountFlag( reader, anAccountProxy );
                    SetNewbornRegisteredAccountFlag( reader, anAccountProxy );
                    SetBirthTimeIsEnteredFlag(reader, anAccountProxy);
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }

            anAccountProxy.Patient = employerBroker.EmployerFor( anAccountProxy.Patient, anAccountProxy.Facility.Oid );

            ArrayList contactPoints =
                addressBroker.ContactPointsForPatient( anAccountProxy.Patient.Facility.Code, anAccountProxy.Patient.MedicalRecordNumber );
            foreach ( ContactPoint cp in contactPoints )
            {
                if ( cp != null )
                {
                    anAccountProxy.Patient.AddContactPoint( cp );
                }
            }
            
            // store this value since AsAccount() does not copy it forward to the account object from the proxy to
            // avoid a standard reg account from opening in 8-tab view when it is copied from a short-reg account.
            bool isShortReg = anAccountProxy.IsShortRegistered;

            Account account = anAccountProxy.AsAccount();
            account.IsShortRegistered = isShortReg;
            account.RemoveDefaultCoverage();
            return account;
        }

        public Account AccountFor( Patient aPatient, long accountNumber )
        {
            return AccountFor( aPatient, accountNumber, null );
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public Account AccountFor( Patient aPatient, long accountNumber, Activity anActivity )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            Account account = null;

            var userId =  new CallContextAccessor().GetContext().UserId;

            var auditMessage = string.Format( "User [{0}] from facility [{1}] requested details of account number [{2}] for patient MRN [{3}]",
                                         userId, aPatient.Facility.Code, accountNumber, aPatient.MedicalRecordNumber );
            Log.Info( auditMessage );
            
            try
            {
                cmd = CommandFor(
                    String.Format( "CALL {0}({1},{2},{3})",
                    SP_ACCOUNT_FOR,
                    PARAM_HSPNUMBER,
                    PARAM_MRC,
                    PARAM_ACCOUNTNUMBER ),
                    CommandType.Text,
                    aPatient.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = aPatient.Facility.Oid;
                cmd.Parameters[PARAM_MRC].Value = aPatient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;

                reader = ExecuteReader( cmd );
                if ( reader.Read() )
                {
                    AccountProxy anAccountProxy = AccountProxyFrom( reader, aPatient,
                        aPatient.Facility );

                    BuildPhysicianRelationships( reader,
                        physicianBroker,
                        aPatient.Facility,
                        anAccountProxy );

                    BuildAdmitSources( reader, anAccountProxy );
                    BuildAdmittingCategory(reader, anAccountProxy);
                    SetPrimaryPlanID( reader, anAccountProxy );
                    GetAccountCreatedDate( anAccountProxy );
                    SetShortRegisteredAccountFlag( reader, anAccountProxy );
                    SetNewbornRegisteredAccountFlag( reader, anAccountProxy );
                    SetBirthTimeIsEnteredFlag(reader, anAccountProxy);
                    SetIsolationCode(reader, anAccountProxy);
                    if ( anActivity != null )
                    {
                        anAccountProxy.Activity = anActivity;
                        if ( anActivity.GetType().Equals( typeof( ShortRegistrationActivity ) ) ||
                             anActivity.GetType().Equals( typeof( ShortPreRegistrationActivity ) ) ||
                             anActivity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) )
                        {
                            anAccountProxy.IsShortRegistered = true;
                        }

                        if ( anActivity.GetType().Equals( typeof( QuickAccountCreationActivity ) ) ||

                             anActivity.GetType().Equals( typeof( QuickAccountMaintenanceActivity ) ) )
                        {
                            anAccountProxy.IsQuickRegistered = true;
                        }

                    }

                    account = anAccountProxy.AsAccount();
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return account;
        }

        /// <exception cref="AccountNotFoundException">Account Not Found.</exception>
        /// <exception cref="EnterpriseException">Wrong number of patients found</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public virtual Account AccountFor( AccountProxy proxy )
        {
            return AccountFor( proxy, proxy.Activity );
        }

        

        /// <exception cref="AccountNotFoundException">Account Not Found.</exception>
        /// <exception cref="EnterpriseException">Wrong number of patients found</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public virtual Account AccountFor( AccountProxy proxy, Activity activity )
        {
            long startTime = DateTime.Now.Ticks;

            Account anAccount = new Account
            {
                Activity = activity,
                AccountNumber = proxy.AccountNumber,
                AdmitDate = proxy.AdmitDate,
                DischargeDate = proxy.DischargeDate,
                Facility = proxy.Facility,
                FinancialClass = proxy.FinancialClass,
                HospitalService = proxy.HospitalService,
                KindOfVisit = proxy.KindOfVisit,
                DerivedVisitType = proxy.DerivedVisitType,
                DischargeDisposition = proxy.DischargeDisposition,
                ValuablesAreTaken = proxy.ValuablesAreTaken,
                AbstractExists = proxy.AbstractExists,
                PendingDischarge = proxy.PendingDischarge,
                Location = proxy.Location,
                AdmitSource = proxy.AdmitSource,
                IsCanceled = proxy.IsCanceled,
                AllPhysicianRelationships = proxy.AllPhysicianRelationships,
                LastMaintenanceDate = proxy.LastMaintenanceDate,
                LastMaintenanceLogNumber = proxy.LastMaintenanceLogNumber,
                UpdateLogNumber = proxy.UpdateLogNumber,
                IsNew = proxy.IsNew,
                BalanceDue = proxy.BalanceDue,
                AccountLock = proxy.AccountLock,
                AccountCreatedDate = proxy.AccountCreatedDate,
                IsNewBorn = proxy.IsNewBorn,
                BirthTimeIsEntered = proxy.BirthTimeIsEntered,
                AdmittingCategory = proxy.AdmittingCategory,
                IsolationCode = proxy.IsolationCode,
                IsPAIWalkinRegistered = proxy.IsPAIWalkinRegistered
            };
            proxy.SetUCCVisitType();
            PatientSearchCriteria patientSearchCriteria =
                new PatientSearchCriteria( proxy.Facility.Code,
                                           null,
                                           null,
                                           new SocialSecurityNumber(),
                                           null,
                                           PatientSearchCriteria.NO_MONTH,
                                           PatientSearchCriteria.NO_YEAR,
                                           null,
                                           null );

            if ( proxy.Patient.MedicalRecordNumber != 0 )
            {

                patientSearchCriteria.MedicalRecordNumber =
                    proxy.Patient.MedicalRecordNumber.ToString();

            }//if
            else
            {
                patientSearchCriteria.AccountNumber =
                    proxy.AccountNumber.ToString();

            }//else

            PatientSearchResponse patientSearchResponse =
                patientBroker.GetPatientSearchResponseFor( patientSearchCriteria );

            List<PatientSearchResult> realNameSearchResults = patientSearchResponse.GetResultsOfType( TypeOfName.Normal );
            switch ( realNameSearchResults.Count )
            {

                case 0:

                    throw new AccountNotFoundException(
                        "Account Not Found.",
                        null,
                        Severity.High );

                case 1:

                    anAccount.Patient =
                        patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0] );
                    break;

                default:

                    throw new EnterpriseException(
                        "Wrong number of patients found",
                        Severity.High );

            }//switch


            anAccount.Patient.PreviousName = new Name( anAccount.Patient.Name.FirstName, anAccount.Patient.Name.LastName, string.Empty );
            anAccount.Facility = proxy.Facility;
            
            Patient patient = anAccount.Patient;
            var mostRecentAccountDetails = patientBroker.GetMostRecentAccountDetailsFor(patient.MedicalRecordNumber, anAccount.Facility);
            
            anAccount.Patient.MostRecentAccountNumber = mostRecentAccountDetails.AccountNumber;
            anAccount.Patient.MostRecentAccountCreationDate = mostRecentAccountDetails.AccountCreationDate;
            anAccount.SetUCCVisitType();
            try
            {
                long emergContactPresent = 0;
                long nearestRelativePresent = 0;
                long mspPresent = 0;
                long finDataPresent = 0;
                long bvDataPresent = 0;

                CheckIfAccountComponentsArePresent(
                    anAccount,
                    ref emergContactPresent,
                    ref nearestRelativePresent,
                    ref mspPresent,
                    ref finDataPresent,
                    ref bvDataPresent );

                ReadGeneralAccountData( anAccount );

                if ( anAccount.Activity != null )
                {
                    Log.Debug( String.Format( "Activity,IsLocked,Readonly - {0},{1},{2}",
                        anAccount.Activity.Description,
                        anAccount.IsLocked,
                        anAccount.Activity.ReadOnlyAccount() ) );
                }
                else
                {
                    Log.Debug( "Account requested without an activity" );
                }

                guarantorBroker.GuarantorFor( anAccount );

                ReadAccountInsuredData( anAccount );

                AddDemographicsTo( anAccount );

                if ( emergContactPresent != NO_DATA_PRESENT )
                {
                    ReadAccountEmergContactData( anAccount );
                }

                if ( nearestRelativePresent != NO_DATA_PRESENT )
                {
                    ReadAccountNearestRelContactData( anAccount );
                }

                anAccount.ActionsLoader = new ActionLoader( anAccount );

                if ( finDataPresent != NO_DATA_PRESENT )
                {
                    ReadFinancialData( anAccount );
                }

                anAccount.Patient.SelectedAccount = anAccount;

                if ( mspPresent != NO_DATA_PRESENT )
                {
                    IMSPBroker mspBroker = BrokerFactory.BrokerOfType<IMSPBroker>();
                    MedicareSecondaryPayor msp = mspBroker.MSPFor( anAccount );
                    if ( msp != null )
                    {
                        anAccount.MedicareSecondaryPayor = msp;
                    }
                }

                ICodedDiagnosisBroker cdBroker = BrokerFactory.BrokerOfType<ICodedDiagnosisBroker>();
                anAccount.CodedDiagnoses = cdBroker.CodedDiagnosisFor(
                    anAccount.Facility.Oid,
                    anAccount.AccountNumber,
                    anAccount.Patient.MedicalRecordNumber,
                    ( anAccount.Activity != null &&
                    anAccount.Activity.GetType() == typeof( MaintenanceActivity ) &&
                    anAccount.FinancialClass != null &&
                    anAccount.FinancialClass.Code != "37" &&
                    anAccount.KindOfVisit.IsEmergencyPatient ) ? true : false,
                    anAccount.Facility );

                if ( anAccount.Activity != null &&
                     (anAccount.Activity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) ||
                     anAccount.Activity.GetType().Equals(typeof(UCCPostMseRegistrationActivity))))
                {
                    //next cursor - get CopiedFrom Account Number, its financialCode and MSP data if any.
                    //This data is from the account copied from during PRE-MSE
                    ReadPostMSEData( anAccount );
                    ReadPostMseMSPData( anAccount );
                }

                // SR 39492 - cancel OP discharge - KJS - 04/20/2008
                // if the activity is Cancel OP discharge then read the pre discharge location
                if ( anAccount.Activity != null &&
                    anAccount.Activity is CancelOutpatientDischargeActivity )
                {
                    ReadPreDischargeLocation( anAccount );
                }

                // read benefits data
                if ( bvDataPresent != NO_DATA_PRESENT )
                {
                    AddBenefitsDataTo( anAccount );
                }

                // SR 39384 - kjs - 7/20/07
                // make a copy of the original Coverages
                if (anAccount.Activity != null && (!anAccount.Activity.GetType().Equals(typeof(PostMSERegistrationActivity)) || 
                    !anAccount.Activity.GetType().Equals(typeof(UCCPostMseRegistrationActivity))))
                {
                    if ( anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ) != null )
                    {
                        anAccount.Insurance.AddOrigCoverage(
                            anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ).DeepCopy() );
                    }
                    if ( anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID ) != null )
                    {
                        anAccount.Insurance.AddOrigCoverage(
                            anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID ).DeepCopy() );
                    }
                }

                if ( anAccount.Patient.MedicalGroupIPA != null && anAccount.Activity != null &&
                    (
                      anAccount.IsNew || anAccount.IsLatestPostMSEAccount() ||
                      anAccount.Activity.IsActivatePreRegisterActivity()
                    ) 
                   )
                {
                    anAccount.MedicalGroupIPA = patientBroker.GetIpaForPatient( anAccount.Patient );
                }

                if (DOFRFeatureManager.IsDOFREnabled(anAccount) && anAccount.Insurance.PrimaryCoverage != null)
                {
                    if (DOFRFeatureManager.IsPlanCommercial(anAccount))
                    {
                        IDOFRInitiateBroker dOFRInitiateBroker = BrokerFactory.BrokerOfType<IDOFRInitiateBroker>();
                        var ispartofipa = dOFRInitiateBroker.GetInsurancePlanPartOfIPA(anAccount);
                        if (!string.IsNullOrEmpty(ispartofipa))
                            ((CommercialCoverage)anAccount.Insurance.PrimaryCoverage).IsInsurancePlanPartOfIPA = bool.Parse(dOFRInitiateBroker.GetInsurancePlanPartOfIPA(anAccount));
                    }
                    if (anAccount.Activity !=null && anAccount.Activity.IsMaintenanceActivity())
                    {
                        bool initiatedDOFR = true;
                        IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                        var hasPatientTypeChanged = accountBroker.HasPatientTypeChangedDuringTransfer(anAccount);
                        if (hasPatientTypeChanged)
                            initiatedDOFR = false;

                        anAccount.IsDOFRInitiated = initiatedDOFR;
                    }
                }
            }
            catch ( AccountNotFoundException ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }

            if ( anAccount.Activity != null )
            {
                User appUser = anAccount.Activity.AppUser;
                anAccount.AccountLock = AccountLockStatusFor( anAccount.AccountNumber, appUser.Facility.Code, appUser.PBAREmployeeID, appUser.WorkstationID );
            }

            Log.InfoFormat("Loading Account {0} at Hospital {1} took {2} Milliseconds with Patient Type={3}, HSV={4}, FC={5}, TransactionType={6},Transaction Time={7}, IsLocked={8}, AdmitDate={9}, DischargeDate={10}, LastMaintenanceDate={11}, LastMaintenanceLognumber={12}, UpdateLogNumber={13}, AccountCreatedDate={14}, PrimaryInsurancePlan={15}, NursingStation={16}",
                proxy.AccountNumber,
                proxy.Facility.Code,
                new TimeSpan(DateTime.Now.Ticks - startTime).Milliseconds, proxy.KindOfVisit, proxy.HospitalService, proxy.FinancialClass, proxy.TransactionType, proxy.TransactionTime, proxy.IsLocked, proxy.AdmitDate, proxy.DischargeDate, proxy.LastMaintenanceDate, proxy.LastMaintenanceLogNumber, proxy.UpdateLogNumber, proxy.AccountCreatedDate, proxy.PrimaryInsurancePlan, proxy.NursingStation);

            return anAccount;
        }

        /// <summary>
        /// This method is used to make the determination that an account was, at one 
        /// time, a Patient Type 3. This can be made by examining the occurrences of 
        /// the NWACT activity code for that account. The AC0005P.ACACTD field will 
        /// contain the Patient Type in position 18 of the (35-char) field.  If you 
        /// find a ‘3’ in this position for any NWACT for the account undergoing the 
        /// Out/In Xfer, then this account qualifies for the P7 Condition Code.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool WasAccountEverAnERType( Account account )
        {
            var activitiesOnAccount = GetActivitiesOn( account );

            return WasEmergencyAccount( activitiesOnAccount );
        }

        private IEnumerable<string> GetActivitiesOn( Account account )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            var accountHistory = new List<string>();

            try
            {
                cmd = CommandFor( "CALL " + SP_GETACCOUNTACTIVITYHISTORY +
                                  "(" + PARAM_HSPCODE +
                                  "," + PARAM_ACCOUNTNUMBER + ")",
                                  CommandType.Text,
                                  account.Facility );

                cmd.Parameters[PARAM_HSPCODE].Value = account.Facility.Code;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = account.AccountNumber;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    if ( !reader.IsDBNull( COL_ACTIVITY_DESCRIPTION ) )
                    {
                        string accountActivity = reader.GetString( COL_ACTIVITY_DESCRIPTION );
                        accountHistory.Add( accountActivity );
                    }
                }
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return accountHistory;
        }

        internal bool WasEmergencyAccount( IEnumerable<string> activitiesOnAccount )
        {
            bool isEmergencyPatient = false;
            foreach ( var activity in activitiesOnAccount )
            {
                if ( activity.Length < 18 ) continue;

                string patientType = activity.Substring( 17, 1 );
                if ( patientType == VisitType.EMERGENCY_PATIENT )
                {
                    isEmergencyPatient = true;
                    break;
                }
            }
            return isEmergencyPatient;
        }

        /// <summary>
        /// Releases the account lock and sets the account IsLocked property to false
        /// returns true if the account lock is successful
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="userid">PBAR userid property is required</param>
        /// <param name="workstationID">User WorkstationId property is required</param>
        /// <param name="facilityOid">Oid of facility</param>
        public AccountLock ReleaseLockOn( long accountNumber,
            string userid, string workstationID, long facilityOid )
        {
            Facility facility = facilityBroker.FacilityWith( facilityOid );

            AccountLock accountLock = AccountLockStatusFor( accountNumber, facility.Code, userid, workstationID );

            if ( accountLock.IsLocked && accountLock.LockedBy.ToUpper() == userid.ToUpper()
                && accountLock.LockedAt.Trim().ToUpper() == workstationID.Trim().ToUpper() )
            {
                IBMUtilities util = new IBMUtilities( facility );
                util.UnlockAccount( accountNumber, userid, workstationID, facility );
                return AccountLockStatusFor( accountNumber, facility.Code, userid, workstationID );
            }
            return accountLock;
        }

        public AccountLock FinishLockingOn( long accountNumber,
            string userid, string workstationID, long facilityOid )
        {
            Facility facility = facilityBroker.FacilityWith( facilityOid );

            AccountLock accountLock = AccountLockStatusFor( accountNumber, facility.Code, userid, workstationID );

            if ( accountLock.IsLocked && accountLock.LockedBy.ToUpper() == userid.ToUpper() )
            {
                IBMUtilities util = new IBMUtilities( facility );
                util.FinishWithAccountLock( accountNumber, userid, workstationID, facility );
                return AccountLockStatusFor( accountNumber, facility.Code, userid, workstationID );
            }
            return accountLock;

        }

        /// <summary>
        /// returns AccountLock object by setting the AcquiredLock property.
        /// Set account's activity by calling AccountLockTimeoutFor() for 
        /// an explicit account lock call from UI.
        /// </summary>
        /// <param name="accountNumber">Account Number</param>
        /// <param name="mrn">Medical Record Number</param>
        /// <param name="userid">PBARuserid property is required</param>
        /// <param name="workstationID">User WorkstationId property is required</param>
        /// <param name="facilityOid">Oid of facility</param>
        /// <returns>>returns false if the account is already locked</returns>
        public AccountLock PlaceLockOn( long accountNumber, long mrn, string userid,
            string workstationID, long facilityOid )
        {
            Log.Debug( "Placing lock on HSP: " + facilityOid +
                " MRN: " + mrn +
                " account: " + accountNumber );

            Facility facility = facilityBroker.FacilityWith( facilityOid );

            AccountLock accountLock = AccountLockStatusFor( accountNumber, facility.Code, userid, workstationID );

            if ( !accountLock.IsLocked )
            {
                Log.DebugFormat( "Account {0} was not locked and no update was pending.  Waiting to verify lock starting.", accountNumber );

                IBMUtilities util = new IBMUtilities( facility );
                util.LockAccount( accountNumber, userid, workstationID, facility );
                accountLock = AccountLockStatusFor( accountNumber, facility.Code, userid, workstationID );

                // We would like to be able to insure that the user requesting this
                // lock is the same as the person who actually got the lock. However
                // we can not even be sure that the lock was acquired. The Lock Utility
                // only places a request that a lock be set on a queue. The subsequent
                // call to AccountLockStatusFor may come back with no lock because the
                // program on the AS/400 has not had time to process the message. 
                // So until they get this resolved we must manually set this lock. 

                int loopCount = 1;

                Log.DebugFormat( "Beginning attempt {0} at confirming account {1} recieved a lock.", loopCount, accountNumber );

                int lockWaitAttempts = Convert.ToInt32( ConfigurationManager.AppSettings["LockWaitAttempts"] );
                int lockWaitInterval = Convert.ToInt32( ConfigurationManager.AppSettings["LockWaitInterval"] );

                while ( accountLock.LockedBy.Trim().ToUpper() != userid.Trim().ToUpper()
                        && loopCount <= lockWaitAttempts )
                {
                    Thread.Sleep( lockWaitInterval );

                    accountLock = AccountLockStatusFor( accountNumber, facility.Code, userid, workstationID );

                    if ( accountLock.IsLocked &&
                        ( accountLock.LockedBy.Trim().ToUpper() != string.Empty
                        && accountLock.LockedBy.Trim().ToUpper() != userid.Trim().ToUpper() ) &&
                        ( accountLock.LockedAt.Trim().ToUpper() != string.Empty
                        && accountLock.LockedAt.Trim().ToUpper() != workstationID.Trim().ToUpper() ) )
                    {
                        Log.DebugFormat( "Attempt {0} at confirming account {1} received a lock failure.  Was locked by PBAR UserID: {2}, Workstation: {3}"
                           , loopCount, accountNumber, userid.Trim().ToUpper(), workstationID.Trim().ToUpper() );

                        break;
                    }

                    Log.DebugFormat( "Attempt {0} at confirming account {1} received a lock failure. (Was not locked or lock incomplete.)", loopCount, accountNumber );

                    loopCount++;
                }

                if ( accountLock.LockedBy.Trim().ToUpper() == userid.Trim().ToUpper()
                    && ( accountLock.LockedAt.ToUpper() == workstationID.Trim().ToUpper() ) )
                {
                    Log.DebugFormat( "Acquired lock for PBAR UserID: {0}, Workstation: {1} on attempt number {2}", userid.Trim().ToUpper(), workstationID.Trim().ToUpper(), loopCount );
                    accountLock.AcquiredLock = true;
                }

                // If we did not get a lock on the account from PBAR even after all the retries due to the PBAR  
                // Account Lock Processor being slow or for whatever reason, issue an unlock message to PBAR,
                // to get the account unlocked immediately after the lock request has been processed whenever, 
                // so as not to keep it locked for a long time if the user has switched to another activity.
                if ( !accountLock.IsLocked )
                {
                    util.UnlockAccount( accountNumber, userid, workstationID, facility );
                    Log.WarnFormat( "Forced to cancel lock attempt for Account {0} at Facility {1}",
                                      new object[] { accountNumber, facility.Code } );
                }
            }

            return accountLock;
        }

        /// <summary>
        /// Locks already locked accounts for synchronizing 2 account lock timeouts
        /// </summary>
        /// <param name="accountNumber1">a string, Account Number</param>
        /// <param name="accountNumber2">a string, Account Number</param>
        /// <param name="userid">PBAR userid property is required</param>
        /// <param name="workstationID">User WorkstationId property is required</param>
        /// <param name="facilityOid">Oid of facility</param>
        public void LockAccounts( long accountNumber1, long accountNumber2, string userid,
            string workstationID, long facilityOid )
        {
            Facility facility = facilityBroker.FacilityWith( facilityOid );

            IBMUtilities util = new IBMUtilities( facility );
            util.LockAccount( accountNumber1, userid, workstationID, facility );
            util.LockAccount( accountNumber2, userid, workstationID, facility );
        }

        /// <summary>
        /// This method saves the Account Object into PBAR
        /// </summary>
        /// <param name="anAccount"></param>
        /// <param name="currentActivity"></param>
        /// <exception cref="BrokerException"><c>BrokerException</c>.</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public AccountSaveResults Save( Account anAccount, Activity currentActivity )
        {
            AccountSaveResults results = new AccountSaveResults();
            IDbTransaction txn = null;
            IDbConnection cxn = null;

            try
            {
                if ( anAccount.Patient.MedicalRecordNumber == 0 )
                {
                    AssignMedicalRecordNumberTo( anAccount );
                    results.MedicalRecordNumber = anAccount.Patient.MedicalRecordNumber;
                }

                if ( anAccount.AccountNumber == 0 )
                {
                    AssignAccountNumberTo( anAccount );
                    results.AccountNumber = anAccount.AccountNumber;
                }

                results.interFacilityTransferAccount = anAccount.Patient.InterFacilityTransferAccount;
                anAccount.Patient.SetPatientContextHeaderData();
                if ( anAccount.KindOfVisit != null && anAccount.KindOfVisit.Code == VisitType.INPATIENT )
                {
                    anAccount.Clinics.Clear();
                    results.ClearClinics = true;
                }

                SaveInsuranceData( anAccount, currentActivity, results );

                TransactionCoordinator txnCoordinator =
                    TransactionCoordinator.TransactionCoordinatorFor( currentActivity );

                // OTD - 36784 - KJS - 3/6/08 insure required fields are present
                // otherwise abort with critical error
                if ( !AllRequiredFieldsPresent( anAccount ) )
                {
                    throw CreateRequiredFieldsException( anAccount );
                }

                if ( txnCoordinator != null )
                {
                    txnCoordinator.Account = anAccount;

                    ITransactionBroker transactionBroker =
                        new TransactionBroker( ConnectionString );

                    txn = transactionBroker.NewTransaction( anAccount.Facility );
                    cxn = txn.Connection;


                    Log.Info( "Saving transaction for: " +
                        anAccount.Facility.Oid + " " +
                        anAccount.Patient.MedicalRecordNumber + " " +
                        anAccount.AccountNumber + " " );

                    transactionBroker.ExecuteTransaction( txnCoordinator );


                    WorklistPBARBroker worklistBroker = new WorklistPBARBroker( txn );
                    worklistBroker.SaveRemainingActions( anAccount );

                    txnCoordinator.WriteFUSNotesForAccount();

                    anAccount.ClearFusNotes();

                }
                if ( txn != null ) txn.Commit();
            }
            catch ( Exception ex )
            {
                if ( txn != null )
                {
                    txn.Rollback();
                }
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
            finally
            {
                if ( cxn != null )
                {
                    cxn.Close();
                }
                if ( txn != null )
                {
                    txn.Dispose();
                }

            }
            return results;
        }
		
        public AccountSaveResults SaveMultipleTransactions(Account anAccount, Activity currentActivity)
        {
            AccountSaveResults results = new AccountSaveResults();
            IDbTransaction txn = null;
            IDbConnection cxn = null;

            try
            {
                if (anAccount.Patient.MedicalRecordNumber == 0)
                {
                    AssignMedicalRecordNumberTo(anAccount);
                    results.MedicalRecordNumber = anAccount.Patient.MedicalRecordNumber;
                }

                if (anAccount.AccountNumber == 0)
                {
                    AssignAccountNumberTo(anAccount);
                    results.AccountNumber = anAccount.AccountNumber;
                }

                if (anAccount.KindOfVisit != null && anAccount.KindOfVisit.Code == VisitType.INPATIENT)
                {
                    anAccount.Clinics.Clear();
                    results.ClearClinics = true;
                }

                SaveInsuranceData(anAccount, currentActivity, results);

                // OTD - 36784 - KJS - 3/6/08 insure required fields are present
                // otherwise abort with critical error
                if (!AllRequiredFieldsPresent(anAccount))
                {
                    throw CreateRequiredFieldsException(anAccount);
                }
				
                IList<TransactionCoordinator> txnCoordinators =
                    TransactionCoordinator.TransactionCoordinatorsFor(currentActivity);

                if (txnCoordinators != null)
                {
                    foreach (var txnCoordinator in txnCoordinators)
                    {


                        txnCoordinator.Account = anAccount;

                        ITransactionBroker transactionBroker =
                            new TransactionBroker(ConnectionString);

                        txn = transactionBroker.NewTransaction(anAccount.Facility);
                        cxn = txn.Connection;


                        Log.Info("Saving transaction for: " +
                                 anAccount.Facility.Oid + " " +
                                 anAccount.Patient.MedicalRecordNumber + " " +
                                 anAccount.AccountNumber + " ");

                        transactionBroker.ExecuteTransaction(txnCoordinator);
                    }

                    WorklistPBARBroker worklistBroker = new WorklistPBARBroker(txn);
                        worklistBroker.SaveRemainingActions(anAccount);

                        txnCoordinators[0].WriteFUSNotesForAccount();

                        anAccount.ClearFusNotes();
                    
                    if (txn != null) txn.Commit();
                }              
            }
			
            catch (Exception ex)
            {
                if (txn != null)
                {
                    txn.Rollback();
                }
				
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, Log);
            }
			
            finally
            {
                if (cxn != null)
                {
                    cxn.Close();
                }
				
                if (txn != null)
                {
                    txn.Dispose();
                }
            }
			
            return results;
        }

        /// <summary>
        /// This method builds the coverage for PreMSEActivity for an account
        /// </summary>
        /// <param name="anAccount"></param>
        /// <exception cref="EnterpriseException">No Insurance Plan found in the database for PRE-MSE Default Plan IDs.</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public Account BuildCoverageForPreMSEActivity( Account anAccount )
        {
            try
            {
                InsurancePlan plan = insuranceBroker.PlanWith( PRE_MSE_INSURANCE_PLAN_ID, anAccount.Facility.Oid, anAccount.AdmitDate );
                if ( plan == null )
                {
                    plan = insuranceBroker.PlanWith( PRE_MSE_INSURANCE_BACKUP_PLAN_ID, anAccount.Facility.Oid, anAccount.AdmitDate );
                    if ( plan == null )
                    {
                        throw new EnterpriseException( "No Insurance Plan found in the database for PRE-MSE Default Plan IDs.", Severity.High );
                    }
                }

                SetFinancialClassForPreMseActivity(anAccount);

                // only create a new Insured if the insured was not copied forward

                Coverage coverage = anAccount.Insurance.PrimaryCoverage;

                if ( coverage == null || coverage.Insured == new Insured() )
                {
                    Insured insured = anAccount.Patient.CopyAsInsured();

                    RelationshipType relType = relTypeBroker.RelationshipTypeWith( anAccount.Facility.Oid, PRE_MSE_RELATIONSHIP_CODE );
                    Relationship relationship = new Relationship( relType, anAccount.Patient.GetType(), insured.GetType() );
                    anAccount.Patient.AddRelationship( relationship );

                    coverage = Coverage.CoverageFor( plan, insured );
                    coverage.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, COVERAGE_ORDER_DESCRIPTION );

                    anAccount.Insurance.AddCoverage( coverage );
                }
                else
                {
                    coverage.InsurancePlan = plan;
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex );
            }

            return anAccount;
        }

        private void SetFinancialClassForPreMseActivity(Account anAccount)
        {
            anAccount.FinancialClass = fcBroker.FinancialClassWith(anAccount.Facility.Oid, PRE_MSE_FINANCIAL_CLASS_CODE);
        }

        /// <summary>
        /// This method is used for Swap Bed Activity which
        /// requires two Account Object
        /// </summary>
        /// <param name="anAccountOne"></param> 
        /// <param name="anAccountTwo"></param> 
        /// <param name="currentActivity"></param>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void Save( Account anAccountOne, Account anAccountTwo,
            Activity currentActivity )
        {
            try
            {
                TransactionCoordinator txnCoordinator =
                    TransactionCoordinator.TransactionCoordinatorFor( currentActivity );

                if ( txnCoordinator != null )
                {
                    txnCoordinator.Account = anAccountOne;
                    txnCoordinator.AccountTwo = anAccountTwo;
                    ITransactionBroker txnbroker = new TransactionBroker();
                    txnbroker.ExecuteTransaction( txnCoordinator );
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
        }

        /// <summary>
        /// Get an account proxy for one and only one account
        /// </summary>
        /// <param name="facilityCode">a string, facility code</param>
        /// <param name="medicalRecordNumber"></param>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public AccountProxy AccountProxyFor( string facilityCode, long medicalRecordNumber, long accountNumber )
        {
            Patient aPatient = new Patient();

            AccountProxy aProxy = AccountProxyFor( facilityCode, aPatient, medicalRecordNumber, accountNumber );
            if (aProxy != null)
            {
                aProxy.Patient.MedicalRecordNumber = medicalRecordNumber;
                aProxy.ActionsLoader = new ActionLoader(aProxy);
            }
            return aProxy;
        }
        /// <summary>
        /// Get an account proxy for one and only one account
        /// </summary>
        /// <param name="facilityCode">The facility code.</param>
        /// <param name="aPatient">A patient.</param>
        /// <param name="medicalRecordNumber">The medical record number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public AccountProxy AccountProxyFor(
            string facilityCode, Patient aPatient,
            long medicalRecordNumber, long accountNumber )
        {
            Facility facility = facilityBroker.FacilityWith( facilityCode );
            AccountProxy anAccountProxy = null;
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = CommandFor(
                    "CALL " + SP_SELECT_ACCOUNT_PROXY_FOR +
                    "(" + PARAM_HSPCODE +
                    "," + PARAM_MRC +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    facility );
                cmd.Parameters[PARAM_HSPCODE].Value = facility.Oid;
                cmd.Parameters[PARAM_MRC].Value = medicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;

                reader = ExecuteReader( cmd );

                if ( reader.Read() )
                {
                    anAccountProxy = AccountProxyFrom( reader, aPatient, facility );
                }
                else
                {
                    return anAccountProxy;
                }

                BuildPhysicianRelationships( reader,
                    physicianBroker,
                    facility,
                    anAccountProxy );

                BuildAdmitSources( reader, anAccountProxy );
                SetPrimaryPlanID( reader, anAccountProxy );
                GetAccountCreatedDate( anAccountProxy );
                return anAccountProxy;
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// Get all types of accounts for a patient based on MRN
        /// </summary>
        /// <param name="aPatient"></param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ArrayList AccountsFor( Patient aPatient )
        {
            ArrayList accounts = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = CreateCmdForAccountsQuery( aPatient );
                cmd.Parameters[PARAM_PATIENTTYPE_PREREG].Value = "Y";
                cmd.Parameters[PARAM_PATIENTTYPE_INPAT].Value = "Y";
                cmd.Parameters[PARAM_PATIENTTYPE_OUTPATIENT].Value = "Y";
                cmd.Parameters[PARAM_PATIENTTYPE_POSTMSE].Value = "Y";
                cmd.Parameters[PARAM_PATIENTTYPE_RECUR].Value = "Y";
                cmd.Parameters[PARAM_PATIENTTYPE_NONPAT].Value = "Y";
                cmd.Parameters[PARAM_PATIENTTYPE_PREMSE].Value = "Y";

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    AccountProxy anAccountProxy = AccountProxyFrom( reader, aPatient, aPatient.Facility );

                    BuildAdmitSources( reader, anAccountProxy );
                    SetPrimaryPlanID( reader, anAccountProxy );
                    GetAccountCreatedDate( anAccountProxy );
                    accounts.Add( anAccountProxy );
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            Log.Debug( String.Format( "MRN, Number of ACCTS :{0}, {1}",
                aPatient.MedicalRecordNumber, accounts.Count ) );

            return SortAccountsByDate( accounts );
        }

        /// <summary>
        /// Get only pre registered accounts for a patient
        /// </summary>
        /// <param name="aPatient"></param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ArrayList PreRegAccountsFor( Patient aPatient )
        {
            ArrayList accounts;
            iDB2Command cmd = null;

            try
            {
                cmd = CreateCmdForAccountsQuery( aPatient );
                cmd.Parameters[PARAM_PATIENTTYPE_PREREG].Value = "Y";

                accounts = ReadAccounts( cmd, aPatient );
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
            finally
            {
                Close( cmd );
            }

            return SortAccountsByDate( accounts );
        }

        /// <summary>
        /// This method returns all MPI accounts for a patient
        /// </summary>
        /// <param name="aPatient"></param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ArrayList MPIAccountsFor( Patient aPatient )
        {
            ArrayList accounts = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = CommandFor( "CALL " + SP_MPI_ACCOUNTS +
                    "(" + PARAM_MRN +
                    "," + PARAM_FACILITYID + ")",
                    CommandType.Text,
                    aPatient.Facility );

                cmd.Parameters[PARAM_MRN].Value = aPatient.MedicalRecordNumber;
                cmd.Parameters[PARAM_FACILITYID].Value = aPatient.Facility.Oid;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {

                    long accountNumber = reader.GetInt64( COL_ACCOUNTNUMBER );

                    long originalMrn = reader.GetInt64( COL_ORIGINAL_MRN1 );
                    if ( originalMrn == 0 )
                    {
                        originalMrn = reader.GetInt64( COL_ORIGINAL_MRN2 );
                    }
                    aPatient.MedicalRecordNumber = originalMrn;

                    VisitType kindOfVisit = patientBroker.PatientTypeWith( aPatient.Facility.Oid,
                        reader.GetString( COL_PATIENTTYPE ).Trim() );

                    string derivedVisitType = reader.GetString( COL_PATIENTTYPE ).Trim();

                    HospitalService hospitalService = hsvBroker.HospitalServiceWith(
                        aPatient.Facility.Oid,
                        reader.GetString( COL_HOSPITALSERVICECODE ).Trim() );

                    string admitTime = reader.GetString( COL_ADMIT_TIME );

                    DateTime admitDate = DateTimeUtilities.FullDateTime(
                        reader.GetInt64( COL_ADMIT_DATE ),
                        Convert.ToInt32( admitTime.PadLeft( 4, '0' ).Substring( 0, 2 ) ),
                        Convert.ToInt32( admitTime.PadLeft( 4, '0' ).Substring( 2, 2 ) ),
                        0 );

                    DateTime dischargeDate = DateTimeUtilities.DateTimeForYYMMDDFormat(
                        reader.GetInt64( COL_DISCHARGEDATE ) );

                    AccountProxy ap = new AccountProxy(
                        accountNumber,
                        aPatient,
                        admitDate,
                        dischargeDate,
                        kindOfVisit,
                        aPatient.Facility,
                        null,
                        hospitalService,
                        derivedVisitType,
                        true );

                    ap.DischargeDisposition = dischargeBroker.DischargeDispositionWith( aPatient.Facility.Oid,
                        reader.GetString( COL_DISCHARGECODE ).Trim() );

                    ap.MultiSiteFlag = reader.GetString( COL_MULTISITE_FLAG );
                    ap.SiteCode = reader.GetString( COL_SITE_CODE ).Trim();
                    ap.ConfidentialFlag = reader.GetString( COL_CONFIDENTIALFLAG ).Trim();

                    long lppAccountNumber = reader.GetInt64( COL_LPP_ACCOUNT_NUMBER );
                    string financialCode = reader.GetString(COL_FINANCIALCODE).Trim();
                    ap.FinancialClass = new FinancialClass() { Code = financialCode };
                    ap.SetUCCVisitType();

                    ap.IsPurged = new YesNoFlag();

                    if ( lppAccountNumber != 0 )
                    {
                        ap.IsPurged.SetNo();
                    }
                    else
                    {
                        ap.IsPurged.SetYes();
                    }

                    ap.Activity = new ViewAccountActivity();

                    accounts.Add( ap );
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
            finally
            {

                Close( reader );
                Close( cmd );
            }

            return SortAccountsByDate( accounts );
        }

        /// <summary>
        /// This method returns the details for an MPI account
        /// </summary>
        /// <param name="aProxy"></param>
        /// <returns></returns>

        //CodeReview: Refactor this method to smaller methods that accomplish a discrete 
        //unit of work.  
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public AccountProxy MPIAccountDetailsFor( AccountProxy aProxy )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            AccountProxy ap = null;

            try
            {
                DateTime admitDate;
                DateTime dischargeDate;


                cmd = CommandFor( "CALL " + SP_MPI_ACCOUNTDETAILS_FOR +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_MRN + ")",
                    CommandType.Text,
                    aProxy.Facility );

                cmd.Parameters[PARAM_FACILITYID].Value = aProxy.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = aProxy.AccountNumber;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    string firstName = reader.GetString( COL_FIRSTNAME ).Trim();
                    firstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( firstName );

                    string lastName = reader.GetString( COL_LASTNAME ).Trim();
                    lastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( lastName );

                    string middleInitial = reader.GetString( COL_MIDDLEINITIAL ).Trim();
                    middleInitial = StringFilter.RemoveFirstCharNonLetter( middleInitial );

                    string suffix = reader.GetString( COL_SUFFIX ).Trim();
                    suffix = StringFilter.RemoveFirstCharNonLetterAndRestNonLetter( suffix );

                    Name patientName = new Name( firstName, lastName, middleInitial, suffix );

                    DateTime patientDateOfBirth = DateTimeUtilities
                        .DateTimeFromString(
                        reader.GetString( COL_DATEOFBIRTH ).Trim() );
                    Gender gender = demographicsBroker.GenderWith( aProxy.Facility.Oid,
                        reader.GetString( COL_GENDER ).Trim() );

                  
                    Patient aPatient = new Patient(
                        PersistentModel.NEW_OID,
                        PersistentModel.NEW_VERSION,
                        patientName,
                        reader.GetInt32( COL_MEDICALRECORDNUMBER ),
                        patientDateOfBirth,
                        null,
                        gender,
                        aProxy.Facility );

                    string aliasFirstName = reader.GetString( COL_ALIAS_FIRSTNAME ).Trim();
                    aliasFirstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( aliasFirstName );

                    string aliasLastName = reader.GetString( COL_ALIAS_LASTNAME ).Trim();
                    aliasLastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( aliasLastName );

                    Name aliasName = new Name(
                        aliasFirstName,
                        aliasLastName,
                        string.Empty,
                        suffix );
                    aPatient.AddAlias( aliasName );

                    aPatient.MaritalStatus = demographicsBroker.MaritalStatusWith( aProxy.Facility.Oid,
                        reader.GetString( COL_MARITALSTATUSCODE ).Trim() );

                    IOriginBroker originBrokerLocal = BrokerFactory.BrokerOfType<IOriginBroker>();
                    aPatient.Race = originBrokerLocal.RaceWith( aPatient.Facility.Oid,
                        reader.GetString( COL_RACE ).Trim() );
                    aPatient.Race2 = originBrokerLocal.RaceWith(aPatient.Facility.Oid,
                        reader.GetString(COL_RACE2CODE).Trim());

                    aPatient.Nationality = originBrokerLocal.RaceWith(aPatient.Facility.Oid,
                        reader.GetString(COL_NATIONALITY1).Trim());
                    aPatient.Nationality2 = originBrokerLocal.RaceWith(aPatient.Facility.Oid,
                        reader.GetString(COL_NATIONALITY2).Trim());

                    aPatient.Ethnicity = originBrokerLocal.EthnicityWith( aPatient.Facility.Oid,
                        reader.GetString( COL_ETHNICITY ).Trim() );
                    aPatient.Descent = originBrokerLocal.EthnicityWith(aPatient.Facility.Oid,
                        reader.GetString(COL_DESCENT1).Trim());
                    aPatient.Ethnicity2 = originBrokerLocal.EthnicityWith(aPatient.Facility.Oid,
                        reader.GetString(COL_ETHNICITY2CODE).Trim());
                    aPatient.Descent2 = originBrokerLocal.EthnicityWith(aPatient.Facility.Oid,
                        reader.GetString(COL_DESCENT2).Trim());
                    string mailingStreet = StringFilter.RemoveHL7Chars( reader.GetString( COL_LOCALADDRESS ).Trim() );
                    mailingStreet = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( mailingStreet );

                    string mailingCity = StringFilter.RemoveHL7Chars( reader.GetString( COL_LOCALADDRESSCITY ).Trim() );
                    mailingCity = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( mailingCity );

                    string mailingZip = reader.GetString( COL_LOCALZIP ).Trim();
                    string mailingZipExtn = reader.GetString( COL_LOCALZIPEXTN ).Trim();
                    string mailingFullZipCode = mailingZip + StringFilter.RemoveHL7Chars( mailingZipExtn );
                    mailingFullZipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( mailingFullZipCode );

                    Address aMailingAddress =
                        new Address( mailingStreet,
                                     String.Empty,
                                     mailingCity,
                                     new ZipCode( mailingFullZipCode ),
                                     addressBroker.StateWith(aPatient.Facility.Oid, reader.GetString( COL_LOCALSTATEID ).Trim() ),
                                     addressBroker.CountryWith( aPatient.Facility.Oid,
                                                      reader.GetString( COL_COUNTRYCODE ).Trim() ) );

                    PhoneNumber aMailingPhoneNumber = new PhoneNumber
                        ( reader.GetInt32( COL_AREA_CODE ).ToString().PadLeft( 3, '0' ),
                        reader.GetInt32( COL_PHONE_NO ).ToString().PadLeft( 7, '0' ) );

                    PhoneNumber aMailingCellPhoneNumber = new PhoneNumber
                        ( reader.GetString( COL_CELL_PHONE_NO ).PadLeft( 10, '0' ) );

                    ContactPoint mailingContact = new ContactPoint(
                        aMailingAddress,
                        aMailingPhoneNumber,
                        aMailingCellPhoneNumber,
                        null,
                        TypeOfContactPoint.NewMailingContactPointType() );

                    string physicalStreet = StringFilter.RemoveHL7Chars( reader.GetString( COL_PERMANENTADDRESS ).Trim() );
                    physicalStreet = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( physicalStreet );

                    string physicalCity = StringFilter.RemoveHL7Chars( reader.GetString( COL_PERMANENTCITY ).Trim() );
                    physicalCity = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( physicalCity );

                    string physicalZip = reader.GetString( COL_PERMANENTZIP ).Trim();
                    string physicalZipExtn = reader.GetString( COL_PERMANENTZIPEXTN ).Trim();
                    string physicalFullZipCode = physicalZip + StringFilter.RemoveHL7Chars( physicalZipExtn );
                    physicalFullZipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( physicalFullZipCode );

                    Address aPhysicalAddress = new Address(
                        physicalStreet,
                        String.Empty,
                        physicalCity,
                        new ZipCode( physicalZip +
                        reader.GetString( COL_PERMANENTZIPEXTN ).Trim() ),
                        addressBroker.StateWith(aPatient.Facility.Oid,
                        reader.GetString( COL_PERMANENTSTATECODE ).Trim() ),
                        addressBroker.CountryWith( aProxy.Facility.Oid,
                        reader.GetString( COL_COUNTRYCODE ).Trim() ) );

                    PhoneNumber aPhysicalPhoneNumber = new PhoneNumber
                        ( reader.GetInt32( COL_PERMANENTAREACODE ).ToString().PadLeft( 3, '0' ),
                        reader.GetInt32( COL_PERMANENTPHONE ).ToString().PadLeft( 7, '0' ) );

                    ContactPoint physicalContact = new ContactPoint(
                        aPhysicalAddress,
                        aPhysicalPhoneNumber,
                        null,
                        TypeOfContactPoint.NewPhysicalContactPointType() );

                    aPatient.AddContactPoint( mailingContact );
                    aPatient.AddContactPoint( physicalContact );

                    aPatient.SocialSecurityNumber = new SocialSecurityNumber(
                        reader.GetInt64( COL_SSN ).ToString() );

                    string drivingLicenseStr = reader.GetString( COL_DRIVINGLICENSE ).Trim();

                    aPatient.DriversLicense = GetPatientDrivingLicenseInfo(drivingLicenseStr, aPatient.Facility.Oid);
                    string passportNumStr = reader.GetString( COL_PASSPORTNUMBER ).Trim();
                    string passportCountryStr = reader.GetString( COL_PASSPORTCOUNTRY ).Trim();
                    if ( passportNumStr != String.Empty )
                    {
                        Country passportCountry = null;
                        if ( passportCountryStr != string.Empty )
                        {
                            passportCountry = addressBroker.CountryWith( aPatient.Facility.Oid, passportCountryStr );
                        }

                        if ( passportCountry != null )
                        {
                            aPatient.Passport = new Passport( passportNumStr, passportCountry );
                        }
                        else
                        {
                            aPatient.Passport = new Passport( passportNumStr, null );
                        }
                    }

                    aPatient.MaidenName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma(
                        reader.GetString( COL_MAIDENNAME ).Trim() );

                    aPatient.MothersName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma(
                        reader.GetString( COL_MOTHERSNAME ).Trim() );

                    aPatient.FathersName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma(
                        reader.GetString( COL_FATHERSNAME ).Trim() );

                    // The yes no field is initialized to blank on construction
                    // on of these conditions (if met) will change the value to Yes or no
                    if ( reader.GetInt32( COL_DNR ) == 1 )
                    {
                        aPatient.DoNotResuscitate.SetYes();
                    }
                    else if ( reader.GetInt32( COL_DNR ) == 2 )
                    {
                        aPatient.DoNotResuscitate.SetNo();
                    }

                    aPatient.PreviousMRN = reader.GetInt32( COL_PREVIOUSMRN );

                    aPatient.BloodlessPatient = reader.GetString( COL_BLOODLESS ).Trim();

                    AdmitSource aSource = asBroker.AdmitSourceWith(
                        aProxy.Facility.Oid,
                        reader.GetString( COL_ADMIT_SOURCE_CODE ).Trim() );

                    VisitType kindOfVisit = patientBroker.PatientTypeWith( aProxy.Facility.Oid,
                        reader.GetString( COL_PATIENTTYPE ).Trim() );

                    HospitalService hospitalService = hsvBroker.HospitalServiceWith(
                        aProxy.Facility.Oid,
                        reader.GetString( COL_HOSPITALSERVICECODE ).Trim() );

                    try
                    {
                        string dischargeTime = reader.GetString( COL_DISCHARGETIME ).Trim();
                        int dischargeHour = Convert.ToInt32( dischargeTime.PadLeft( 4, '0' ).Substring( 0, 2 ) );
                        int dischargeMin = Convert.ToInt32( dischargeTime.PadLeft( 4, '0' ).Substring( 2, 2 ) );

                        dischargeDate = DateTimeUtilities.DateTimeForYYMMDDFormat(
                            reader.GetInt64( COL_DISCHARGEDATE ) );

                        dischargeDate = new DateTime(
                            dischargeDate.Year,
                            dischargeDate.Month,
                            dischargeDate.Day,
                            dischargeHour,
                            dischargeMin,
                            0 );
                    }
                    catch
                    {
                        dischargeDate = new DateTime();
                    }

                    try
                    {
                        string admitTime = reader.GetString( COL_ADMIT_TIME ).Trim();

                        int hour = Convert.ToInt32( admitTime.PadLeft( 4, '0' ).Substring( 0, 2 ) );
                        int min = Convert.ToInt32( admitTime.PadLeft( 4, '0' ).Substring( 2, 2 ) );

                        admitDate = DateTimeUtilities.DateTimeForYYMMDDFormat(
                            reader.GetInt64( COL_ADMIT_DATE ) );

                        admitDate = new DateTime(
                            admitDate.Year,
                            admitDate.Month,
                            admitDate.Day,
                            hour,
                            min,
                            0 );
                    }
                    catch
                    {
                        admitDate = new DateTime();
                    }

                    ap = new AccountProxy(
                        reader.GetInt64( COL_ACCOUNTNUMBER ),
                        aPatient,
                        admitDate,
                        dischargeDate,
                        kindOfVisit,
                        aPatient.Facility,
                        null,
                        hospitalService,
                        reader.GetString( COL_PATIENTTYPE ).Trim(),
                        true );
                    

                   ap. SetUCCVisitType();
                    if ( ap.Patient.NoticeOfPrivacyPracticeDocument == null )
                    {
                        ap.Patient.NoticeOfPrivacyPracticeDocument = new NoticeOfPrivacyPracticeDocument();
                    }

                    ap.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion =
                        nppBroker.NPPVersionWith( aProxy.Facility.Oid, reader.GetInt64( COL_NPP_VERSION ).ToString() );

                    string code = reader.GetString( COL_NPP_SIGNATURE_STATUS );
                    if ( code != String.Empty )
                    {
                        ap.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus( code );
                    }
                    else
                    {
                        ap.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus();
                    }

                    string nppSignedDate = reader.GetString( COL_NPP_DATE_SIGNED );
                    if ( nppSignedDate.Trim() != String.Empty && nppSignedDate.Trim() != DATETIME_MIN_VALUE )
                    {
                        ap.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate =
                            DateTimeUtilities.DateTimeForYYYYMMDDFormat( nppSignedDate );
                    }
                    else
                    {
                        ap.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate = new DateTime( 0 );
                    }

                    DateTime patientLastUpdatedDate;
                    try
                    {
                        patientLastUpdatedDate =
                            DateTimeUtilities.DateTimeForYYYYMMDDFormat(
                            reader.GetInt64( COL_PATIENT_LASTUPDATED ) );
                    }
                    catch
                    {
                        patientLastUpdatedDate = new DateTime();
                    }
                    aPatient.LastUpdated = patientLastUpdatedDate;

                    ap.DischargeDisposition =
                        dischargeBroker.DischargeDispositionWith( aPatient.Facility.Oid,
                        reader.GetString( COL_DISCHARGECODE ).Trim() );

                    ap.ConfidentialFlag = reader.GetString( COL_CONFIDENTIALFLAG ).Trim();
                    ap.Bloodless = new YesNoFlag();
                    switch ( reader.GetString( COL_BLOODLESS ).Trim().ToUpper() )
                    {
                        case "Y":
                            ap.Bloodless.SetYes();
                            break;
                        case "N":
                            ap.Bloodless.SetNo();
                            break;
                        case " ":
                            ap.Bloodless.SetBlank( string.Empty );
                            break;
                    }

                    Diagnosis diagnosis = new Diagnosis();
                    diagnosis.ChiefComplaint = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                        reader.GetString( COL_CHIEFCOMPLAINT ).Trim() );

                    ap.Diagnosis = diagnosis;

                    ap.AdmitSource = aSource;
                    ap.FinancialClass = fcBroker.FinancialClassWith( aProxy.Facility.Oid,
                        reader.GetString( COL_FINANCIALCODE ).Trim() );

                    ap.AddPhysicianRelationship( physicianBroker.BuildAttendingPhysicianRelationship(
                        aProxy.Facility.Oid,
                        reader.GetInt32( COL_ATTENDING_DOCTOR_ID ) ) );

                    ap.AddPhysicianRelationship( physicianBroker.BuildAdmittingPhysicianRelationship(
                        aProxy.Facility.Oid,
                        reader.GetInt32( COL_ADMITTING_DOCTOR_ID ) ) );

                    ap.AddPhysicianRelationship( physicianBroker.BuildReferringPhysicianRelationship(
                        aProxy.Facility.Oid,
                        reader.GetInt32( COL_REFERRING_DOCTOR_ID ) ) );

                    ap.AddPhysicianRelationship( physicianBroker.BuildOperatingPhysicianRelationship(
                        aProxy.Facility.Oid,
                        reader.GetInt32( COL_OPERATING_DOCTOR_ID ) ) );

                    ap.AddPhysicianRelationship( physicianBroker.BuildPrimaryCarePhysicianRelationship(
                        aProxy.Facility.Oid,
                        reader.GetInt32( COL_PCP_DOCTOR_ID ) ) );

                    ap.MultiSiteFlag = reader.GetString( COL_MULTISITE_FLAG );
                    ap.SiteCode = reader.GetString( COL_SITE_CODE ).Trim();

                    ap.LastUpdated = DateTimeUtilities.DateTimeForMMDDYYFormat(
                        reader.GetInt64( COL_ACCOUNT_LASTUPDATED ) );
                    ap.SeenInED = reader.GetString( COL_SEEN_IN_ED ).Trim();

                    string clinicCode1 = reader.GetString( COL_CLINIC1 ).Trim();
                    string clinicCode2 = reader.GetString( COL_CLINIC2 ).Trim();
                    string clinicCode3 = reader.GetString( COL_CLINIC3 ).Trim();
                    string clinicCode4 = reader.GetString( COL_CLINIC4 ).Trim();
                    string clinicCode5 = reader.GetString( COL_CLINIC5 ).Trim();

                    HospitalClinic hospitalClinic;

                    if ( clinicCode1.Length > 0 )
                    {
                        hospitalClinic = hcBroker.HospitalClinicWith( aProxy.Facility.Oid, clinicCode1 );
                        ap.AddHospitalClinic( hospitalClinic );
                    }

                    if ( clinicCode2.Length > 0 )
                    {
                        hospitalClinic = hcBroker.HospitalClinicWith( aProxy.Facility.Oid, clinicCode2 );
                        ap.AddHospitalClinic( hospitalClinic );
                    }

                    if ( clinicCode3.Length > 0 )
                    {
                        hospitalClinic = hcBroker.HospitalClinicWith( aProxy.Facility.Oid, clinicCode3 );
                        ap.AddHospitalClinic( hospitalClinic );
                    }

                    if ( clinicCode4.Length > 0 )
                    {
                        hospitalClinic = hcBroker.HospitalClinicWith( aProxy.Facility.Oid, clinicCode4 );
                        ap.AddHospitalClinic( hospitalClinic );
                    }

                    if ( clinicCode5.Length > 0 )
                    {
                        hospitalClinic = hcBroker.HospitalClinicWith( aProxy.Facility.Oid, clinicCode5 );
                        ap.AddHospitalClinic( hospitalClinic );
                    }
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }
            finally
            {

                Close( reader );
                Close( cmd );
            }
            return ap;
        }

        public ArrayList GetPriorAccounts( PriorAccountsRequest request )
        {
            // Sanjeev Kumar: Defect ID 35587 (OnTime). Index out of range error.
            // Removed the try/catch construct from this method so that we don't consume
            // any errors here. Instead, we handle it in the AfterWork by allowing it to bubble-up

            request.Facility = facilityBroker.FacilityWith( request.FacilityOid );

            GetPriorAccountsDelegate getPbarAccountsDelegate = delegate { return GetPbarPriorAccounts( request ); };
            GetPriorAccountsDelegate getSosAccountsDelegate = delegate { return GetSOSPriorAccounts( request ); };

            IAsyncResult pbarAccountsResult = getPbarAccountsDelegate.BeginInvoke( null, null );
            IAsyncResult sosAccountsResult = getSosAccountsDelegate.BeginInvoke( null, null );

            pbarAccountsResult.AsyncWaitHandle.WaitOne();
            sosAccountsResult.AsyncWaitHandle.WaitOne();

            ArrayList pbarAccounts = getPbarAccountsDelegate.EndInvoke( pbarAccountsResult );
            ArrayList sosAccounts = getSosAccountsDelegate.EndInvoke( sosAccountsResult );


            ArrayList al = new ArrayList();

            // 3. Combine the accounts into two arrays - one with payment plans and one without

            if ( pbarAccounts != null )
            {
                foreach ( AccountProxy ap in pbarAccounts )
                {
                    if ( ap.HasPaymentPlan )
                    {
                        withPaymentPlan.Add( ap );
                    }
                    else
                    {
                        withoutPaymentPlan.Add( ap );
                    }
                }
            }

            if ( sosAccounts != null )
            {
                foreach ( AccountProxy ap in sosAccounts )
                {
                    if ( ap.HasPaymentPlan )
                    {
                        withPaymentPlan.Add( ap );
                    }
                    else
                    {
                        withoutPaymentPlan.Add( ap );
                    }
                }
            }

            SortAccountsByDate( withPaymentPlan );
            SortAccountsByDate( withoutPaymentPlan );

            al.Add( withPaymentPlan );
            al.Add( withoutPaymentPlan );

            return al;
        }




        /// <summary>
        /// This method returns list of Accounts (Proxy) matching
        /// given religion for a facility
        /// <param name="facilityCode"> A string, facility code</param>
        /// <param name="religionCode"></param>     
        /// <returns>Collection of Accounts</returns>
        /// <exception cref="Exception">AccountsFor the given facility and religion method failed with an unknown error.</exception>
        /// <exception cref="Exception">AccountsFor the given facility and religion method failed with an unknown error.</exception>
        /// <exception cref="Exception">AccountsFor the given facility and religion method failed with an unknown error.</exception>
        /// </summary>
        /// <param name="facilityCode">The facility code.</param>
        /// <param name="religionCode">The religion code.</param>
        public ICollection AccountsFor( string facilityCode, string religionCode )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            DateTime dateOfBirth = DateTime.MinValue;

            accountProxyList = new ArrayList();

            try
            {
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor( "CALL " + SP_RELIGION_ACCOUNTS_MATCHING +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_RELIGION + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_RELIGION].Value = religionCode;

                reader = ExecuteReader( cmd );

                IReligionBroker religionBrokerLocal = BrokerFactory.BrokerOfType<IReligionBroker>();

                while ( reader.Read() )
                {
                    string firstName = reader.GetString( COL_FIRSTNAME );
                    firstName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( firstName );

                    string lastName = reader.GetString( COL_LASTNAME );
                    lastName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( lastName );

                    string middleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    middleInitial = StringFilter.RemoveFirstCharNonLetter( middleInitial );

                    string religionID = reader.GetString( COL_RELIGION_RELIGIONID );
                    string placeOfWorshipCode = reader.GetString( COL_RELIGION_PLACEOFWORSHIPCODE ).Trim();
                    string clergyVisit = reader.GetString( COL_CLERGYVISIT );

                    Gender gender = demographicsBroker.GenderWith( facility.Oid,
                        reader.GetString( COL_GENDER_ID ).Trim() );

                    Religion religion;
                    if ( religionID.Trim().Length > 0 )
                    {
                        religion = religionBrokerLocal.ReligionWith( facility.Oid, religionID );
                    }
                    else
                    {
                        religion = new Religion(
                            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION,
                            "UNSPECIFIED", "UNSPECIFIED" );
                    }

                    PlaceOfWorship placeOfWorship = religionBrokerLocal.PlaceOfWorshipWith(
                        facility.Oid, placeOfWorshipCode );

                    string patientDateOfBirthStr = reader.GetString( COL_DATEOFBIRTH );
                    if ( patientDateOfBirthStr.Trim().Length > 0 )
                    {
                        dateOfBirth = DateTimeUtilities
                            .DateTimeFromString(
                            patientDateOfBirthStr );
                    }

                    Patient patient = new Patient(
                        PersistentModel.NEW_OID,
                        PersistentModel.NEW_VERSION,
                        firstName,
                        lastName,
                        middleInitial,
                        dateOfBirth,
                        gender,
                        religion,
                        placeOfWorship );

                    patient.MedicalRecordNumber = reader.GetInt32(
                        COL_MEDICALRECORDNUMBER );

                    AccountProxy accountProxy =
                        AccountProxyFrom( reader, patient, facility );

                    switch ( clergyVisit.ToUpper() )
                    {
                        case "Y":
                            accountProxy.ClergyVisit.SetNo();
                            break;
                        case "N":
                            accountProxy.ClergyVisit.SetYes();
                            break;
                        default:
                            accountProxy.ClergyVisit.SetBlank( string.Empty );
                            break;
                    }

                    accountProxy.IsolationCode = reader.GetChar( COL_ISOLATIONCODE ).ToString();
                    accountProxy.Confidential = reader.GetString( COL_CONFIDENTIAL );
                    accountProxy.OptOutInformation = reader.GetString(
                        COL_OPTOUT_INFORMATION );

                    accountProxyList.Add( accountProxy );
                }
            }
            catch ( Exception ex )
            {
                const string message = "AccountsFor the given facility and religion method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( message, ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return accountProxyList;
        }

        /// <summary>
        /// Gets a list of accountproxy based on physician filter criteria.
        /// </summary>
        /// <param name="physicianPatientsSearchCriteria">
        /// This object provides the parameters for the search
        /// </param>
        /// <returns>Collection of Accounts</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ICollection AccountsMatching(
            PhysicianPatientsSearchCriteria physicianPatientsSearchCriteria )
        {
            ArrayList accounts = new ArrayList();
            SafeReader reader = null;
            iDB2Command cmd = null;

            try
            {
                cmd = CommandFor(
                    "CALL " + SP_PHYSICIAN_PATIENTS_ACCOUNTS_MATCHING +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_PHYSICIAN_NUMBER +
                    "," + PARAM_ADMITTING +
                    "," + PARAM_ATTENDING +
                    "," + PARAM_REFERRING +
                    "," + PARAM_CONSULTING +
                    "," + PARAM_OPERATING + ")",
                    CommandType.Text,
                    physicianPatientsSearchCriteria.Facility );

                cmd.Parameters[PARAM_FACILITYID].Value = physicianPatientsSearchCriteria.Facility.Oid;

                cmd.Parameters[PARAM_PHYSICIAN_NUMBER].Value = physicianPatientsSearchCriteria.PhysicianNumber;

                cmd.Parameters[PARAM_ADMITTING].Value = physicianPatientsSearchCriteria.Admitting;

                cmd.Parameters[PARAM_ATTENDING].Value = physicianPatientsSearchCriteria.Attending;

                cmd.Parameters[PARAM_REFERRING].Value = physicianPatientsSearchCriteria.Referring;

                cmd.Parameters[PARAM_CONSULTING].Value = physicianPatientsSearchCriteria.Consulting;

                cmd.Parameters[PARAM_OPERATING].Value = physicianPatientsSearchCriteria.Operating;

                string selectedPhysicianNumber = physicianPatientsSearchCriteria.PhysicianNumber.ToString();

                DateTime patientDateOfBirth = DateTime.MinValue;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    string relationship = String.Empty;

                    string patientFirstName = reader.GetString( COL_FIRSTNAME );
                    patientFirstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientFirstName );

                    string patientLastName = reader.GetString( COL_LASTNAME );
                    patientLastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientLastName );

                    string patientMiddleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    patientMiddleInitial = StringFilter.RemoveFirstCharNonLetter( patientMiddleInitial );

                    long patientMRN = reader.GetInt32( COL_MEDICALRECORDNUMBER );

                    IDemographicsBroker demographicsBrokerLocal = BrokerFactory.BrokerOfType<IDemographicsBroker>();
                    Gender gender = demographicsBrokerLocal.GenderWith( physicianPatientsSearchCriteria.Facility.Oid,
                        reader.GetString( COL_GENDER_ID ).Trim() );

                    string patientDateOfBirthStr = reader.GetString( COL_DATEOFBIRTH );
                    if ( patientDateOfBirthStr.Trim().Length > 0 )
                    {
                        patientDateOfBirth = DateTimeUtilities
                            .DateTimeFromString(
                            patientDateOfBirthStr );
                    }

                    Patient patient = new Patient(
                        PersistentModel.NEW_OID,
                        PersistentModel.NEW_VERSION,
                        patientFirstName,
                        patientLastName,
                        patientMiddleInitial,
                        patientMRN,
                        patientDateOfBirth,
                        null,
                        gender,
                        physicianPatientsSearchCriteria.Facility );

                    AccountProxy accountProxy =
                        AccountProxyFrom( reader, patient,
                        physicianPatientsSearchCriteria.Facility );

                    string drID = reader.GetValue( COL_ADMITTING_DOCTOR_ID ).ToString().Trim();
                    if ( drID.Trim().Length > 0 && drID == selectedPhysicianNumber )
                    {
                        relationship = "Adm, ";
                    }
                    drID = reader.GetValue( COL_ATTENDING_DOCTOR_ID ).ToString().Trim();
                    if ( drID.Trim().Length > 0 && drID == selectedPhysicianNumber )
                    {
                        relationship = relationship + "Att, ";
                    }
                    drID = reader.GetValue( COL_REFERRING_DOCTOR_ID ).ToString().Trim();
                    if ( drID.Trim().Length > 0 && drID == selectedPhysicianNumber.PadLeft( 5, '0' ) )
                    {
                        relationship = relationship + "Ref, ";
                    }
                    drID = reader.GetValue( COL_CONSULTINGDR1ID ).ToString().Trim();
                    if ( drID.Trim().Length > 0 && drID == selectedPhysicianNumber )
                    {
                        relationship = relationship + "Con, ";
                    }
                    else
                    {
                        drID = reader.GetValue( COL_CONSULTINGDR2ID ).ToString().Trim();
                        if ( drID != null && drID.Trim().Length > 0 &&
                            drID == selectedPhysicianNumber )
                        {
                            relationship = relationship + "Con, ";
                        }
                        else
                        {
                            drID = reader.GetValue( COL_CONSULTINGDR3ID ).ToString().Trim();
                            if ( drID != null && drID.Trim().Length > 0 &&
                                drID == selectedPhysicianNumber )
                            {
                                relationship = relationship + "Con, ";
                            }
                            else
                            {
                                drID = reader.GetValue( COL_CONSULTINGDR4ID ).ToString().Trim();
                                if ( drID != null && drID.Trim().Length > 0 &&
                                    drID == selectedPhysicianNumber )
                                {
                                    relationship = relationship + "Con, ";
                                }
                                else
                                {
                                    drID = reader.GetValue( COL_CONSULTINGDR5ID ).ToString().Trim();
                                    if ( drID != null && drID.Trim().Length > 0 &&
                                        drID == selectedPhysicianNumber )
                                    {
                                        relationship = relationship + "Con, ";
                                    }
                                }
                            }
                        }
                    }
                    drID = reader.GetString( COL_OPERATING_DOCTOR_ID ).Trim();
                    if ( drID.Trim().Length > 0 && drID == selectedPhysicianNumber )
                    {
                        relationship = relationship + "Opr, ";
                    }
                    relationship = relationship.Remove( relationship.Length - 2, 2 );
                    accountProxy.PhysicianRelationship = relationship;

                    SetIsolationCode(reader,accountProxy);
                    Diagnosis diagnosis = new Diagnosis();
                    diagnosis.ChiefComplaint =
                        StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                        reader.GetString( COL_CHIEFCOMPLAINT ) );

                    accountProxy.Diagnosis = diagnosis;

                    accountProxy.Confidential = reader.GetString( COL_CONFIDENTIAL );
                    accountProxy.OptOutInformation = reader.GetString( COL_OPTOUT_INFORMATION );

                    accounts.Add( accountProxy );
                }

            }
            catch ( Exception ex )
            {
                const string message = "AccountsMatching for the given " +
                                       "PhysicianPatientsSearchCriteria method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( message, ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return accounts;
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ICollection AccountsMatching( PatientCensusSearchCriteria patientSearchCriteria )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            ArrayList accountProxyListLocal = new ArrayList();

            try
            {
                string firstName = patientSearchCriteria.FirstName;
                string lastName = patientSearchCriteria.LastName;
                long facilityId = patientSearchCriteria.Facility.Oid;
                long accountNumber = patientSearchCriteria.AccountNumber;

                cmd = CommandFor( "CALL " + SP_ACCOUNTS_MATCHING +
                    "(" + PARAM_FIRSTNAME +
                    "," + PARAM_LASTNAME +
                    "," + PARAM_ACCOUNTNUMBER +
                    "," + PARAM_FACILITYID + ")",
                    CommandType.Text,
                    patientSearchCriteria.Facility );

                cmd.Parameters[PARAM_LASTNAME].Value = lastName;
                cmd.Parameters[PARAM_FIRSTNAME].Value = firstName;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityId;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    string patientFirstName = reader.GetString( COL_FIRSTNAME );
                    patientFirstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientFirstName );

                    string patientLastName = reader.GetString( COL_LASTNAME );
                    patientLastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientLastName );

                    string patientMiddleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    patientMiddleInitial = StringFilter.RemoveFirstCharNonLetter( patientMiddleInitial );

                    string patientBloodlessCondition = reader.GetString( COL_BLOODLESSPATIENT );
                    long patientMedicalRecordNumber = reader.GetInt32( COL_MEDICALRECORDNUMBER );

                    Patient patient = new Patient(
                        PersistentModel.NEW_OID,
                        PersistentModel.NEW_VERSION,
                        patientFirstName,
                        patientLastName,
                        patientMiddleInitial,
                        patientMedicalRecordNumber,
                        DateTime.MinValue,
                        null,
                        null,
                        patientSearchCriteria.Facility,
                        patientBloodlessCondition
                        );

                    AccountProxy accountProxy =
                        AccountProxyFrom( reader, patient, patientSearchCriteria.Facility );

                    SetIsolationCode(reader,accountProxy);

                    accountProxy.AddPhysicianRelationship(
                        physicianBroker.BuildAttendingPhysicianRelationship( facilityId,
                        reader.GetInt32( COL_ATTENDING_DOCTOR_ID ) ) );

                    accountProxy.AddPhysicianRelationship(
                        physicianBroker.BuildAdmittingPhysicianRelationship( facilityId,
                        reader.GetInt32( COL_ADMITTING_DOCTOR_ID ) ) );

                    for ( int i = 1; i <= TOTAL_NUMBER_OF_CONSULTING_PHYSICIANS; i++ )
                    {
                        int consultingPhysicianNumber = reader.GetInt32( CONSULTING_DR + i + ID );
                        if ( consultingPhysicianNumber > 0 )
                        {
                            accountProxy.AddPhysicianRelationship(
                                physicianBroker.BuildConsultingPhysicianRelationship( facilityId,
                                consultingPhysicianNumber ) );
                        }
                    }

                    accountProxy.Confidential = reader.GetString( COL_CONFIDENTIAL );
                    accountProxy.OptOutInformation =
                        reader.GetString( COL_OPTOUT_INFORMATION );
                    accountProxyListLocal.Add( accountProxy );
                }
            }
            catch ( Exception ex )
            {
                const string message = "AccountsMatching for the given " +
                                       "PatientsCensusSearchCriteria method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( message, ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            ConnectionString = "SDFDSF";
            return accountProxyListLocal;
        }

        /// <summary>
        /// Gets a list of accountproxy based on the filter criteria given as parameters.
        /// </summary>
        /// <param name="isOccupiedBeds">
        /// This value is true if we require only UnOccupied and Pending Admission
        /// and false if we require all.
        /// </param>
        /// <param name="nursingStation">Takes nursing station code.</param>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ICollection AccountsMatching( bool isOccupiedBeds, string nursingStation, string facilityCode )
        {
            ArrayList accounts = new ArrayList();

            SafeReader reader = null;
            iDB2Command cmd = null;

            Facility facility = facilityBroker.FacilityWith( facilityCode );
            long facilityId = facility.Oid;
            try
            {
                cmd = CommandFor( "CALL " + SP_NURSING_STATION_ACCOUNTS_MATCHING +
                    "(" + PARAM_IS_OCCUPIEDBEDS +
                    "," + PARAM_NURSING_STATION +
                    "," + PARAM_FACILITYID + ")",
                    CommandType.Text,
                    facility );

                if ( isOccupiedBeds )
                {
                    cmd.Parameters[PARAM_IS_OCCUPIEDBEDS].Value = SHOW_UNOCCUPIED_BEDS;
                }
                else
                {
                    cmd.Parameters[PARAM_IS_OCCUPIEDBEDS].Value = SHOW_ALL_BEDS;
                }

                if ( nursingStation == ALL_NURSING_STATIONS )
                {
                    cmd.Parameters[PARAM_NURSING_STATION].Value = ALL_NURSING_STATIONS;
                }
                else
                {
                    cmd.Parameters[PARAM_NURSING_STATION].Value = nursingStation.Trim();
                }
                cmd.Parameters[PARAM_FACILITYID].Value = facilityId;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    DateTime patientDateOfBirth = DateTime.MinValue;

                    string patientFirstName = reader.GetString( COL_FIRSTNAME );
                    patientFirstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientFirstName );

                    string patientLastName = reader.GetString( COL_LASTNAME );
                    patientLastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientLastName );

                    string patientMiddleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    patientMiddleInitial = StringFilter.RemoveFirstCharNonLetter( patientMiddleInitial );

                    string patientDateOfBirthStr = reader.GetString( COL_DATEOFBIRTH );
                    int patientMRN = reader.GetInt32( COL_MEDICALRECORDNUMBER );
                    string roomConditionCode = reader.GetString( COL_ROOM_CONDITION );

                    string portalOptin = reader.GetString(COL_PORTALOPTIN_RIGHTTORESTRICT);
                   

                    Gender gender = demographicsBroker.GenderWith( facility.Oid,
                        reader.GetString( COL_GENDER_ID ).Trim() );

                    if ( !string.IsNullOrEmpty( patientDateOfBirthStr ) )
                    {
                        patientDateOfBirth = DateTimeUtilities
                            .DateTimeFromString(
                            patientDateOfBirthStr );
                    }

                    Name patientName = new Name(
                        patientFirstName,
                        patientLastName,
                        patientMiddleInitial,
                        String.Empty );

                    Patient patient = new Patient(
                        PersistentModel.NEW_OID,
                        PersistentModel.NEW_VERSION,
                        patientName,
                        patientMRN,
                        patientDateOfBirth,
                        null,
                        gender,
                        facility );

                    AccountProxy accountProxy = AccountProxyFrom( reader,
                        patient,
                        facility );

                    SetIsolationCode(reader,accountProxy);

                    SetPortalOptIn( accountProxy, portalOptin );

                    if ( roomConditionCode != null &&
                        roomConditionCode.Trim() != String.Empty )
                    {
                        RoomCondition roomCondition = new RoomCondition(
                            PersistentModel.NEW_OID,
                            PersistentModel.NEW_VERSION,
                            String.Empty,
                            roomConditionCode );
                        if ( accountProxy.Location.Room != null )
                        {
                            accountProxy.Location.Room.RoomCondition = roomCondition;
                        }
                    }

                    accountProxy.AddPhysicianRelationship(
                        physicianBroker.BuildAttendingPhysicianRelationship( facility.Oid,
                        reader.GetInt32( COL_ATTENDING_DOCTOR_ID ) ) );

                    accountProxy.Overflow = reader.GetString( COL_OVERFLOW );
                    accountProxy.PendingAdmission = reader.GetString(
                        COL_PENDING_ADMISSION );
                    accountProxy.Confidential = reader.GetString( COL_CONFIDENTIAL );
                    accountProxy.OptOutInformation = reader.GetString(
                        COL_OPTOUT_INFORMATION );

                    accounts.Add( accountProxy );
                }
            }
            catch ( Exception ex )
            {
                const string message = "AccountsMatching for the given " +
                                       "nursing station and facility method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( message, ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return accounts;
        }

        /// <summary>
        /// coverageCategory and nursingStation should be
        /// comma separated string. Example: "'NS','N1','N2'"
        /// </summary>
        /// <param name="coverageCategory">coverageCategory</param>
        /// <param name="nursingStation">nursingStation</param>
        /// <param name="facilityCode">facilityCode</param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ICollection AccountsMatching( string coverageCategory, string nursingStation, string facilityCode )
        {
            ArrayList accounts = new ArrayList();

            Facility facility = facilityBroker.FacilityWith( facilityCode );

            iDB2Command cmd = null;
            SafeReader reader = null;
            long facilityId = facility.Oid;
            try
            {
                cmd = CommandFor( "CALL " + SP_INSURANCE_PLAN_ACCOUNTS_MATCHING +
                    "(" + PARAM_COVERAGECATEGORY +
                    "," + PARAM_NURSING_STATION +
                    "," + PARAM_FACILITYID +
                    "," + PARAM_HSPCODE + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_COVERAGECATEGORY].Value = coverageCategory;
                cmd.Parameters[PARAM_NURSING_STATION].Value = nursingStation;
                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_HSPCODE].Value = facility.Code;

                reader = ExecuteReader( cmd );

                DateTime patientDateOfBirth = DateTime.MinValue;

                while ( reader.Read() )
                {
                    string patientFirstName = reader.GetString( COL_FIRSTNAME );
                    patientFirstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientFirstName );

                    string patientLastName = reader.GetString( COL_LASTNAME );
                    patientLastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientLastName );

                    string patientMiddleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    patientMiddleInitial = StringFilter.RemoveFirstCharNonLetter( patientMiddleInitial );

                    int patientMRN = reader.GetInt32( COL_MEDICALRECORDNUMBER );

                    string patientDateOfBirthStr = reader.GetString( COL_DATEOFBIRTH );

                    Gender gender = demographicsBroker.GenderWith( facilityId,
                        reader.GetString( COL_GENDER_ID ).Trim() );

                    if ( patientDateOfBirthStr.Length > 0 )
                    {
                        patientDateOfBirth = DateTimeUtilities.
                            DateTimeFromString( patientDateOfBirthStr );
                    }

                    Name patientName = new Name(
                        patientFirstName,
                        patientLastName,
                        patientMiddleInitial,
                        String.Empty );

                    Patient patient = new Patient(
                        PersistentModel.NEW_OID,
                        PersistentModel.NEW_VERSION,
                        patientName,
                        patientMRN,
                        patientDateOfBirth,
                        null,
                        gender,
                        facility );

                    AccountProxy accountProxy = AccountProxyFrom( reader,
                        patient, facility );

                    accountProxy.AddPhysicianRelationship(
                        physicianBroker.BuildAttendingPhysicianRelationship( facility.Oid,
                        reader.GetInt32( COL_ATTENDING_DOCTOR_ID ) ) );

                    if ( fcBroker.IsUninsured( accountProxy.Facility.Oid, accountProxy.FinancialClass ) )
                    {
                        accountProxy.AmountDue = reader.GetDecimal( COL_UNINSURED_TOTALAMOUNTDUE );
                    }
                    else
                    {
                        accountProxy.AmountDue = reader.GetDecimal( COL_INSURED_TOTALAMOUNTDUE );
                    }

                    accountProxy.Payments = reader.GetDecimal( COL_PAYMENTS );
                    accountProxy.LengthOfStay = reader.GetInt64( COL_LENGTHOFSTAY );

                    string noLiability = reader.GetString( COL_LIABILITYFLAG );
                    accountProxy.PrimaryNoLiability = noLiability == "X" ? true : false;

                    accountProxy.PayorName = reader.GetString( COL_PAYORNAME );
                    accountProxy.PrimaryInsurancePlan = reader.GetString(
                        COL_PRIMARYINSURANCEPLAN );
                    accountProxy.PrimaryPlanName = reader.GetString(
                        COL_PRIMARYPLANNAME );
                    accountProxy.SecondaryPlan = reader.GetString( COL_SECONDARYPLAN );
                    accountProxy.SecondaryPlanName = reader.GetString(
                        COL_SECONDARYPLANNAME );
                    accountProxy.Confidential = reader.GetString( COL_CONFIDENTIAL );
                    accountProxy.OptOutInformation = reader.GetString(
                        COL_OPTOUT_INFORMATION );

                    accounts.Add( accountProxy );
                }
            }
            catch ( Exception ex )
            {
                const string message = "AccountsMatching for the given coverage category, " +
                                       "nursing station and facility method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( message, ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return accounts;
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ICollection AccountsMatching(
            CensusADTSearchCriteria adtSearchCriteria )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            ArrayList accountProxyListLocal = new ArrayList();

            try
            {
                string adtActivity = adtSearchCriteria.ADTActivity;
                string startTime = adtSearchCriteria.StartTime;
                string nursingStations = adtSearchCriteria.NursingStations;
                long facilityId = adtSearchCriteria.Facility.Oid;

                cmd = CommandFor( "CALL " + SP_ADT_ACCOUNTS_MATCHING +
                    "(" + PARAM_ADTACTIVITY +
                    "," + PARAM_STARTTIME +
                    "," + PARAM_NURSINGSTATIONS +
                    "," + PARAM_FACILITYID + ")",
                    CommandType.Text,
                    adtSearchCriteria.Facility );

                cmd.Parameters[PARAM_ADTACTIVITY].Value = adtActivity;
                cmd.Parameters[PARAM_STARTTIME].Value = startTime;
                cmd.Parameters[PARAM_NURSINGSTATIONS].Value = nursingStations;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityId;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    string patientFirstName = reader.GetString( COL_FIRSTNAME );
                    patientFirstName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientFirstName );

                    string patientLastName = reader.GetString( COL_LASTNAME );
                    patientLastName = StringFilter.
                        RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( patientLastName );

                    string patientMiddleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    patientMiddleInitial = StringFilter.RemoveFirstCharNonLetter( patientMiddleInitial );

                    long patientMedicalRecordNumber = reader.GetInt32( COL_MEDICALRECORDNUMBER );

                    Patient patient = new Patient(
                        PersistentModel.NEW_OID,
                        PersistentModel.NEW_VERSION,
                        patientFirstName,
                        patientLastName,
                        patientMiddleInitial,
                        patientMedicalRecordNumber,
                        DateTime.MinValue,
                        null,
                        null,
                        adtSearchCriteria.Facility,
                        null
                        );

                    AccountProxy accountProxy =
                        AccountProxyFrom( reader,
                        patient,
                        adtSearchCriteria.Facility );

                    Diagnosis diagnosis = new Diagnosis();
                    diagnosis.ChiefComplaint = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                        reader.GetString( COL_CHIEFCOMPLAINT ) );
                    accountProxy.Diagnosis = diagnosis;

                    accountProxy.AddPhysicianRelationship(
                        physicianBroker.BuildAttendingPhysicianRelationship( facilityId,
                        reader.GetInt32( COL_ATTENDING_DOCTOR_ID ) ) );

                    accountProxy.AddPhysicianRelationship(
                        physicianBroker.BuildAdmittingPhysicianRelationship( facilityId,
                        reader.GetInt32( COL_ADMITTING_DOCTOR_ID ) ) );

                    string nursingStationFrom = reader.GetString( COL_NURSINGSTATIONFROM ).Trim();
                    string roomFrom = reader.GetValue( COL_ROOMFROM ).ToString();
                    string bedFrom = reader.GetString( COL_BEDFROM ).Trim();

                    accountProxy.LocationFrom = new Location( nursingStationFrom, roomFrom, bedFrom );

                    string transactionTime = reader.GetString( COL_ADTTIME ).Trim().PadLeft( 4, '0' );
                    accountProxy.TransactionTime =
                        String.Format( "{0}:{1}", transactionTime.Substring( 0, 2 ), transactionTime.Substring( 2, 2 ) );

                    accountProxy.TransactionType =
                        reader.GetString( COL_TRANSACTIONTYPE ).Trim();

                    string nursingStationTo = reader.GetString( COL_NURSINGSTATIONTO ).Trim();
                    string roomTo = reader.GetValue( COL_ROOMTO ).ToString();
                    string bedTo = reader.GetString( COL_BEDTO ).Trim();

                    accountProxy.LocationTo = new Location( nursingStationTo, roomTo, bedTo );

                    accountProxy.Confidential =
                        reader.GetString( COL_CONFIDENTIAL );
                    accountProxy.OptOutInformation =
                        reader.GetString( COL_OPTOUT_INFORMATION );
                    accountProxyListLocal.Add( accountProxy );
                }
            }
            catch ( Exception ex )
            {
                const string msg = "AccountsMatching for the given " +
                    "CensusADTSearchCriteria method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return accountProxyListLocal;
        }

        /// <exception cref="Exception">BloodlessTreatmentAccountsFor for a given facility, patient type and admit date method failed with an unknown error.</exception>
        public ICollection BloodlessTreatmentAccountsFor( string facilityCode, string patientType, string admitDate )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            ArrayList accountProxyListLocal = new ArrayList();

            try
            {
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor( "CALL " + SP_BLOODLESS_ACCOUNTS_MATCHING +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_PATIENTTYPE +
                    "," + PARAM_ADMITDATE + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_PATIENTTYPE].Value = patientType;
                cmd.Parameters[PARAM_ADMITDATE].Value = admitDate;

                reader = ExecuteReader( cmd );


                while ( reader.Read() )
                {
                    long medicalRecordNumber = reader.GetInt32( COL_MEDICALRECORDNUMBER );
                    string firstName = reader.GetString( COL_FIRSTNAME );
                    firstName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( firstName );

                    string lastName = reader.GetString( COL_LASTNAME );
                    lastName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( lastName );

                    string middleInitial = reader.GetString( COL_MIDDLEINITIAL );
                    middleInitial = StringFilter.RemoveFirstCharNonLetter( middleInitial );

                    string dob = reader.GetString( COL_DATEOFBIRTH );

                    DateTime dateOfBirth = DateTimeUtilities.DateTimeFromString( dob );

                    Gender gender = demographicsBroker.GenderWith( facility.Oid,
                        reader.GetString( COL_GENDER_ID ).Trim() );

                    Patient patient = new Patient(
                        PersistentModel.NEW_OID, PersistentModel.NEW_VERSION,
                        firstName, lastName, middleInitial,
                        medicalRecordNumber, dateOfBirth, null, gender, facility );

                    AccountProxy accountProxy = AccountProxyFrom(
                        reader, patient, facility );

                    accountProxy.AddPhysicianRelationship(
                        physicianBroker.BuildAttendingPhysicianRelationship( facility.Oid,
                        reader.GetInt32( COL_ATTENDING_DOCTOR_ID ) ) );

                    for ( int i = 1; i <= TOTAL_NUMBER_OF_CONSULTING_PHYSICIANS; i++ )
                    {
                        int consultingPhysicianNumber = reader.GetInt32( CONSULTING_DR + i + ID );
                        if ( consultingPhysicianNumber > 0 )
                        {
                            accountProxy.AddPhysicianRelationship(
                                physicianBroker.BuildConsultingPhysicianRelationship( facility.Oid,
                                consultingPhysicianNumber ) );
                        }
                    }

                    accountProxy.Confidential =
                        reader.GetString( COL_CONFIDENTIAL );
                    accountProxy.OptOutInformation =
                        reader.GetString( COL_OPTOUT_INFORMATION );

                    accountProxyListLocal.Add( accountProxy );
                }

                return accountProxyListLocal;
            }
            catch ( Exception ex )
            {
                const string msg = "BloodlessTreatmentAccountsFor for a given facility, " +
                    "patient type and admit date method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// Verify offline MRN to check if the number is in use or invalid
        /// </summary>
        /// <param name="mrn">medical record number</param>
        /// <param name="facilityOid">The facility oid.</param>
        /// <exception cref="OfflineMRNAlreadyUsedException">OfflineMRNAlreadyUsedException</exception>
        /// <exception cref="OfflineMRNInvalidException">OfflineMRNInvalidException</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void VerifyOfflineMRN( long mrn, long facilityOid )
        {
            iDB2Command cmd = null;

            try
            {
                Facility facility = facilityBroker.FacilityWith( facilityOid );

                cmd = CommandFor( "CALL " + SP_VERIFY_OFFLINE_MRN +
                    "(" + PARAM_MRN +
                    "," + PARAM_HSPNUMBER +
                    "," + OUTPUT_PARAM_RESULT + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_MRN].Value = mrn;
                cmd.Parameters[PARAM_HSPNUMBER].Value = facilityOid;

                cmd.Parameters[OUTPUT_PARAM_RESULT].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                string result = cmd.Parameters[OUTPUT_PARAM_RESULT].Value.ToString();
                if ( result == "InUse" )
                {
                    throw new OfflineMRNAlreadyUsedException(
                        "OfflineMRNAlreadyUsedException", null, Severity.High );
                }

                if ( result == "Invalid" )
                {
                    throw new OfflineMRNInvalidException(
                        "OfflineMRNInvalidException", null, Severity.High );
                }
            }
            catch ( Exception ex )
            {
                const string msg = "Unexpected Exception in AccountPBARBroker VerifyOfflineMRN method.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, Log );
            }
            finally
            {
                Close( cmd );
            }
        }

        /// <summary>
        /// Verify offline AccountNumber to check if the number is in use or invalid
        /// </summary>
        /// <param name="accountNumber">account number</param>
        /// <param name="facilityOid">The facility oid.</param>
        /// <exception cref="OfflineAccountAlreadyUsedException">OfflineAccountAlreadyUsedException</exception>
        /// <exception cref="OfflineAccountInvalidException">OfflineAccountInvalidException</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void VerifyOfflineAccountNumber( long accountNumber, long facilityOid )
        {
            iDB2Command cmd = null;

            try
            {
                Facility facility = facilityBroker.FacilityWith( facilityOid );

                bool checkResult = Account.IsValidAccountNumber( facility, accountNumber );

                string result;
                if ( checkResult )
                {
                    string acctNum = accountNumber.ToString();
                    int accountNumSeed = Convert.ToInt32( acctNum.Substring( 0, acctNum.Length - 1 ) );

                    cmd = CommandFor( "CALL " + SP_VERIFY_OFFLINE_ACCTNUM +
                        "(" + PARAM_ACCOUNTNUMBER +
                        "," + PARAM_ACCOUNTNUMBERSEED +
                        "," + PARAM_HSPNUMBER +
                        "," + OUTPUT_PARAM_RESULT + ")",
                        CommandType.Text,
                        facility );

                    cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;
                    cmd.Parameters[PARAM_ACCOUNTNUMBERSEED].Value = accountNumSeed;
                    cmd.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;

                    cmd.Parameters[OUTPUT_PARAM_RESULT].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    result = cmd.Parameters[OUTPUT_PARAM_RESULT].Value.ToString();
                }
                else
                {
                    result = "Invalid";
                }
                if ( result == "InUse" )
                {
                    throw new OfflineAccountAlreadyUsedException(
                        "OfflineAccountAlreadyUsedException", null, Severity.High );
                }

                if ( result == "Invalid" )
                {
                    throw new OfflineAccountInvalidException(
                        "OfflineAccountInvalidException", null, Severity.High );
                }
            }
            catch ( Exception ex )
            {
                const string msg = "Unexpected Exception in AccountPBARBroker VerifyOfflineAccountNumber method";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, Log );
            }
            finally
            {
                Close( cmd );
            }
        }

        /// <summary>
        /// Gets a list of accountproxy based on Duplicate PreRegAccounts SearchCriteria.
        /// </summary>
        /// <param name="searchCriteria">
        /// This object provides the parameters for the search
        /// </param>
        /// <returns>Collection of DuplicateAccounts</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ICollection SelectDuplicatePreRegAccounts( DuplicatePreRegAccountsSearchCriteria searchCriteria )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            ArrayList preRegAccountsList = new ArrayList();
            Facility searchFacility = facilityBroker.FacilityWith( searchCriteria.FacilityID );
            if ( searchFacility != null )
            {
                try
                {
                    if ( searchCriteria.MedicalRecordNumber != 0 )
                    {
                        cmd = CommandFor( "CALL " + SP_DUPLICATEACCOUNTS_MATCHING_MRN +
                            "(" + PARAM_ACCOUNTNUMBER +
                              "," + PARAM_MRN +
                             "," + PARAM_FACILITYID +
                            "," + PARAM_ADMITDATE + ")",
                            CommandType.Text,
                            searchFacility );
                        cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = searchCriteria.AccountNumber;
                        cmd.Parameters[PARAM_MRN].Value = searchCriteria.MedicalRecordNumber;
                    }
                    else if ( !searchCriteria.SocialSecurityNumber.IsDefaultSsn() )
                    {
                        cmd = CommandFor( "CALL " + SP_DUPLICATEACCOUNTS_MATCHING_SSN +
                            "(" + PARAM_SSN +
                                "," + PARAM_FACILITYID +
                                "," + PARAM_ADMITDATE + ")",
                            CommandType.Text,
                            searchFacility );
                        cmd.Parameters[PARAM_SSN].Value = searchCriteria.SocialSecurityNumber.ToString();
                    }
                    else
                    {
                        cmd = CommandFor( "CALL " + SP_DUPLICATEACCOUNTS_MATCHING_NAMES +
                           "(" + PARAM_FIRSTNAME +
                             "," + PARAM_LASTNAME +
                           "," + PARAM_DOB +
                           "," + PARAM_FACILITYID +
                           "," + PARAM_ADMITDATE + ")",
                           CommandType.Text,
                           searchFacility );
                        cmd.Parameters[PARAM_FIRSTNAME].Value = searchCriteria.FirstName.Trim();
                        cmd.Parameters[PARAM_LASTNAME].Value = searchCriteria.LastName.Trim();
                        cmd.Parameters[PARAM_DOB].Value = DateTimeUtilities.PackedDateFromDate( searchCriteria.DateOfBirth );
                    }
                    cmd.Parameters[PARAM_FACILITYID].Value = searchFacility.Oid;
                    cmd.Parameters[PARAM_ADMITDATE].Value = searchCriteria.AdmitDate.ToString( "MM/dd/yyyy" );

                    reader = ExecuteReader( cmd );
                    AccountProxy accountDetails;

                    while ( reader.Read() )
                    {
                        accountDetails = new AccountProxy();
                        long accountNumber = reader.GetInt32( COL_ACCOUNTNUMBER );
                        accountDetails.AccountNumber = accountNumber;
                        accountDetails.HospitalService = hsvBroker.HospitalServiceWith(
                            searchFacility.Oid, reader.GetString( COL_MEDICALSERVICECODE ).Trim() );
                        string hospitalClinic = reader.GetString( COL_CLINICCODE ).Trim();
                        accountDetails.HospitalClinic = hcBroker.HospitalClinicWith( searchFacility.Oid, hospitalClinic );
                        DateTime admitDate = reader.GetDateTime( COL_ADMISSION_DATE );
                        accountDetails.AdmitDate = admitDate;

                        preRegAccountsList.Add( accountDetails );
                    }
                }
                catch ( Exception ex )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
                }
                finally
                {
                    Close( reader );
                    Close( cmd );
                }
            }
            return preRegAccountsList;
        }

        //method


        /// <summary>
        /// a wrapper for private method AddDemographicsTo
        /// </summary>
        /// <param name="anAccount"></param>
        /// <returns></returns>
        public IAccount AddDemographicDataTo( IAccount anAccount )
        {
            IAccount account = anAccount;
            AddDemographicsTo( account );
            return account;
        }


        /// <summary>
        /// Gets the new account number for the given facility
        /// </summary>
        /// <param name="facility">The facility.</param>
        /// <returns></returns>
        /// <exception cref="Exception">when a new account number could not be obtained.</exception>
        /// <exception cref="ArgumentNullException"><c>facility</c> is null.</exception>
        /// <exception cref="EnterpriseException">/// <exception cref="EnterpriseException">if an invalid account number (zero or non integer) is returned by PBAR.</exception> </exception>
        public long GetNewAccountNumberFor( Facility facility )
        {
            Guard.ThrowIfArgumentIsNull( facility, "facility" );

            IDbCommand newAccountNumberCommand = null;

            try
            {
                newAccountNumberCommand = GetCommandForNewAccountNumberStoredProcedure( facility );

                object rawAccountNumber = TryExecutingNewAccountNumberStoredProcedure( newAccountNumberCommand );

                if ( IntegerHelper.IsPositiveInteger( rawAccountNumber ) )
                {
                    return Convert.ToInt32( rawAccountNumber );
                }

                else
                {
                    string message = string.Format( "Invalid account number:{0} returned for facility: {1}", rawAccountNumber, facility.Code );
                    throw new EnterpriseException( message );
                }
            }

            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }

            finally
            {
                Close( newAccountNumberCommand );
            }
        }

        /// <exception cref="Exception">Unhandled Exception</exception>
        public ArrayList AccountsWithWorklistsWith( string facilityCode, WorklistSettings worklistSettings )
        {
            ArrayList accounts = new ArrayList();
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor( "CALL " + SP_SELECT_ACCOUNTS_WITH_WORKLISTS +
                                       "(" + PARAM_FACILITYID +
                                       "," + PARAM_STARTINGLETTERS +
                                       "," + PARAM_ENDINGLETTERS +
                                       "," + PARAM_STARTDATE +
                                       "," + PARAM_ENDDATE +
                                       "," + PARAM_WORKLISTID +
                                       "," + PARAM_RANGEID + ")",
                                       CommandType.Text,
                                       facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facility.Oid;
                cmd.Parameters[PARAM_STARTINGLETTERS].Value = worklistSettings.BeginningWithLetter;
                cmd.Parameters[PARAM_ENDINGLETTERS].Value = worklistSettings.EndingWithLetter;
                if ( worklistSettings.FromDate.Equals( EPOCH ) )
                {
                    cmd.Parameters[PARAM_STARTDATE].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters[PARAM_STARTDATE].Value = worklistSettings.FromDate;
                }
                if ( worklistSettings.ToDate.Equals( EPOCH ) )
                {
                    cmd.Parameters[PARAM_ENDDATE].Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters[PARAM_ENDDATE].Value = worklistSettings.ToDate;
                }
                cmd.Parameters[PARAM_WORKLISTID].Value = worklistSettings.WorkListID;
                cmd.Parameters[PARAM_RANGEID].Value = worklistSettings.WorklistSelectionRange.Oid;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    string lastName = reader.GetString( COL_LASTNAME ).Trim();
                    lastName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( lastName );

                    string firstName = reader.GetString( COL_FIRSTNAME ).Trim();
                    firstName = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( firstName );

                    string middleInitial = StringFilter.StripMiddleInitialFromFirstName( ref firstName );
                    middleInitial = StringFilter.RemoveFirstCharNonLetter( middleInitial );

                    Name aName = new Name( firstName, lastName, middleInitial );

                    string name = aName.AsFormattedName();
                    string primaryPayor = reader.GetString( COL_PAYORNAME ).Trim();
                    long accountNumber = reader.GetInt64( COL_ACCOUNTNUMBER );
                    long medicalRecordNumber = reader.GetInt64( COL_MEDICALRECORDNUMBER );

                    DateTime admitDate = DateTime.MinValue;
                    DateTime dischargeDate = DateTime.MinValue;

                    try
                    {
                        admitDate = reader.GetDateTime( COL_ADMISSION_DATE );
                        dischargeDate = reader.GetDateTime( COL_DISCHARGEDATE );
                    }
                    catch
                    {
                        Log.Error( "Error reading date field from data reader" );
                    }

                    string hsv = reader.GetString( COL_SERVICECODE ).Trim();
                    string hsvDesc = reader.GetString( COL_HOSPITALSERVICE ).Trim();
                    string hospitalService = hsv + " " + hsvDesc;
                    string financialCode = reader.GetString( COL_FINANCIALCODE ).Trim();
                    string financialCodeDescription = reader.GetString( COL_FINANCIALCLASS ).Trim();
                    string financialClass = financialCode + " " + financialCodeDescription;
                    string clinic = reader.GetString( COL_CLINICDESC ).Trim();
                    string dischargeStatus = string.Empty;
                    int todoCount = reader.GetInt32( COL_ACTIONCOUNT );

                    if ( dischargeDate != DateTime.MinValue )
                    {
                        dischargeStatus = dischargeDate.ToShortDateString();
                    }
                    else if ( reader.GetString( COL_PENDINGDISCHARGE ) == "Y" )
                    {
                        dischargeStatus = DischargeStatus.PendingDischarge().Description;
                    }
                    else if ( reader.GetString( COL_PREDISCHARGEFLAG ) == "Y" )
                    {
                        dischargeStatus = "PENDING";
                    }

                    string isLocked = reader.GetString( COL_LOCKINDICATOR );

                    WorklistItem item = new WorklistItem( name,
                                                          accountNumber,
                                                          medicalRecordNumber,
                                                          admitDate,
                                                          dischargeDate,
                                                          primaryPayor,
                                                          hospitalService,
                                                          financialClass,
                                                          clinic,
                                                          dischargeStatus,
                                                          isLocked,
                                                          todoCount );

                    accounts.Add( item );
                }
            }

            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return accounts;
        }

        ///////////////////////End of AccountsWithWorklistsWith //////////////

        public ActivityTimeout AccountLockTimeoutFor( Activity activity )
        {
            ActivityTimeout activityTimeout = new ActivityTimeout();

            if ( activity != null )
            {
                ITimeoutBroker timeoutBroker = BrokerFactory.BrokerOfType<ITimeoutBroker>();
                activityTimeout = timeoutBroker.TimeoutFor();
            }
            return activityTimeout;
        }

        /// <summary>
        /// returns AccountLock Object
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="facilityCode"> A string, facility code</param>
        /// <param name="pbaruserId"></param>
        /// <param name="workstationID"> A string, workstation id</param>
        /// <returns></returns>
        /// <exception cref="Exception">Unhandled Exception</exception>
        public AccountLock AccountLockStatusFor( long accountNumber,
                                                 string facilityCode,
                                                 string pbaruserId,
                                                 string workstationID )
        {
            AccountLock accountLock = new AccountLock();

            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                // the account number must be a 9 digit string padded on the left with zeros
                string acctNumber = IBMUtilities.MakePaddedNumber( accountNumber, 9, '0' );

                Facility facility = facilityBroker.FacilityWith( facilityCode );

                cmd = CommandFor( String.Format(
                                           "CALL {0}({1},{2})",
                                           SP_ACCOUNT_LOCK_STATUS_FOR,
                                           PARAM_HSPNUMBER,
                                           PARAM_ACCOUNTNUMBER ),
                                       CommandType.Text,
                                       facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = facility.Oid;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = acctNumber;
                reader = ExecuteReader( cmd );

                if ( reader.Read() )
                {
                    accountLock = AccountLockFrom( reader );

                    // read out the workstation id from this reader...

                    accountLock.LockedAt = reader.GetString( COL_LOCKEDWRKSTN ).Trim();
                }

                if ( accountLock.LockedBy.Trim().ToUpper() == pbaruserId.Trim().ToUpper()
                     && ( accountLock.LockedAt.ToUpper() == workstationID.Trim().ToUpper() ) )
                {
                    accountLock.AcquiredLock = true;
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return accountLock;
        }

        public void BuildPrimaryCarePhysicanRelationship(IAccount account)
        {
            if (account.AllPhysicianRelationships.Count == 0)
            {
                iDB2Command cmd = null;
                SafeReader pcpReader = null;
                SafeReader nonStaffReader = null;
    
                try
                {
                    var primaryCarePhysicianRelationship = new PhysicianRelationship();

                    var parameters = new[] { PARAM_HSPNUMBER, PARAM_MRC, PARAM_ACCOUNTNUMBER };

                    var cmdTextForCallingGetPcpFor = new Db2StoredProcedureCallBuilder(parameters, SP_GETPCPFOR).Build();

                    cmd = CommandFor(cmdTextForCallingGetPcpFor, CommandType.Text, account.Facility);
                    cmd.Parameters[PARAM_HSPNUMBER].Value = account.Facility.Oid;
                    cmd.Parameters[PARAM_MRC].Value = account.Patient.MedicalRecordNumber;
                    cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = account.Patient.MostRecentAccountNumber;

                    pcpReader = ExecuteReader(cmd);

                    while (pcpReader.Read())
                    {
                        long pcpDoctorId = pcpReader.GetInt32(COL_PCP_DOCTOR_ID);
                        if (pcpDoctorId != Physician.NO_PCP_NUMBER && pcpDoctorId != Physician.UNKNOWN_PCP_NUMBER)
                        {
                            if (pcpDoctorId == NON_STAFF_PHYSICIAN_NUMBER)
                            {
                                parameters = new[] { PARAM_HSPNUMBER, PARAM_ACCOUNTNUMBER };
                                var cmdTextForCallingNonStaffPhysicianInfo = new Db2StoredProcedureCallBuilder(parameters, SP_NONSTAFF_PHYSICIAN_INFO).Build();
                                cmd = CommandFor(cmdTextForCallingNonStaffPhysicianInfo, CommandType.Text, account.Facility);

                                cmd.Parameters[PARAM_HSPNUMBER].Value = account.Facility.Oid;
                                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = account.Patient.MostRecentAccountNumber;

                                nonStaffReader = ExecuteReader(cmd);

                                while (nonStaffReader.Read())
                                {
                                    var pcpRole = new PrimaryCarePhysician();
                                    var primaryCarePhysician = PrimaryCareNonStaffPhysicianFrom(nonStaffReader);
                                    primaryCarePhysicianRelationship = new PhysicianRelationship(pcpRole, primaryCarePhysician);
                                }
                            }

                            else
                            {
                                primaryCarePhysicianRelationship = physicianBroker.BuildPrimaryCarePhysicianRelationship(
                                    account.Facility.Oid, pcpDoctorId);
                            }

                            if (primaryCarePhysicianRelationship.Physician != null)
                            {
                                account.AddPhysicianRelationship(primaryCarePhysicianRelationship);
                            }
                        }
                    }

                }

                catch (Exception generalException)
                {
                    Log.Error(ErrorPCPRetreival, generalException);
                    throw;
                }

                finally
                {
                    Close(pcpReader);
                    Close(nonStaffReader);
                    Close(cmd);
                }
            }
        }
        /// <summary>
        /// Method to check if patient type changed in transfer activity. 
        /// Use this method in maintainance activity to check if patient type has changed in transfer
        /// </summary>
        /// <param name="account"></param>
        /// <returns>bool</returns>
        public bool HasPatientTypeChangedDuringTransfer(Account account)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            var accountHistory = new List<string>();

            try
            {
                cmd = CommandFor("CALL " + SP_GETRECENTACCOUNTHISTORY +
                                 "(" + PARAM_FACILITYID +
                                 "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    account.Facility);

                cmd.Parameters[PARAM_FACILITYID].Value = account.Facility.Oid;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = account.AccountNumber;

                reader = ExecuteReader(cmd);

                while (reader.Read())
                {
                    if (!reader.IsDBNull(COL_TRANSACTION_TYPE))
                    {
                        string accountActivity = reader.GetString(COL_TRANSACTION_TYPE);
                        accountHistory.Add(accountActivity.ToUpper().Trim());
                    }
                }
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }

            bool PTChanged = accountHistory.Contains(PTCHANGETRANSACTION);
            return PTChanged;
        }

        #endregion

        #region Private Methods

        private IDbCommand GetCommandForNewAccountNumberStoredProcedure( Facility facility )
        {
            string commandText = String.Format( "CALL {0}( {1},{2})", SP_GETNEWACCOUNTNUMBER, PARAM_HOSPITALCODE, OUT_PARAM_ACCOUNTNUMBER );

            iDB2Command cmd = CommandFor( commandText, CommandType.Text, facility );

            cmd.Parameters[PARAM_HOSPITALCODE].Value = facility.Code;
            cmd.Parameters[OUT_PARAM_ACCOUNTNUMBER].Direction = ParameterDirection.Output;
            return cmd;
        }

        /// <summary>
        /// Tries the executing new account number stored procedure. 
        /// This method is made public and virtual to enable testing it is not meant to be used by the clients of this class
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The MRN returned by PBAR as an object</returns>
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual object TryExecutingNewAccountNumberStoredProcedure( IDbCommand command )
        // ReSharper restore VirtualMemberNeverOverriden.Global
        {
            command.ExecuteNonQuery();

            IDbDataParameter parameter = ( IDbDataParameter )command.Parameters[OUT_PARAM_ACCOUNTNUMBER];

            return parameter.Value;
        }

        private static void BuildConsultingPhysicianRelationships( long facilityID, AccountProxy anAccountProxy, IPhysicianBroker physicianBroker, SafeReader reader )
        {
            //Consulting Physician #1 Relationship
            PhysicianRelationship cons1Relationsihp =
                physicianBroker.BuildConsultingPhysicianRelationship(
                    facilityID,
                    reader.GetInt32( COL_CONSULTINGDR1ID ) );

            if ( cons1Relationsihp.Physician != null )
            {
                anAccountProxy.AddPhysicianRelationship( cons1Relationsihp );
            }

            //Consulting Physician #2 Relationship
            PhysicianRelationship cons2Relationsihp =
                physicianBroker.BuildConsultingPhysicianRelationship(
                    facilityID,
                    reader.GetInt32( COL_CONSULTINGDR2ID ) );

            if ( cons2Relationsihp.Physician != null )
            {
                anAccountProxy.AddPhysicianRelationship( cons2Relationsihp );
            }

            //Consulting Physician #3 Relationship
            PhysicianRelationship cons3Relationsihp =
                physicianBroker.BuildConsultingPhysicianRelationship(
                    facilityID,
                    reader.GetInt32( COL_CONSULTINGDR3ID ) );

            if ( cons3Relationsihp.Physician != null )
            {
                anAccountProxy.AddPhysicianRelationship( cons3Relationsihp );
            }

            //Consulting Physician #4 Relationship
            PhysicianRelationship cons4Relationsihp =
                physicianBroker.BuildConsultingPhysicianRelationship(
                    facilityID,
                    reader.GetInt32( COL_CONSULTINGDR4ID ) );

            if ( cons4Relationsihp.Physician != null )
            {
                anAccountProxy.AddPhysicianRelationship( cons4Relationsihp );
            }

            //Consulting Physician #5 Relationship
            PhysicianRelationship cons5Relationsihp =
                physicianBroker.BuildConsultingPhysicianRelationship(
                    facilityID,
                    reader.GetInt32( COL_CONSULTINGDR5ID ) );

            if ( cons5Relationsihp.Physician != null )
            {
                anAccountProxy.AddPhysicianRelationship( cons5Relationsihp );
            }
        }

        private void AssignMedicalRecordNumberTo( Account anAccount )
        {
            anAccount.Patient.MedicalRecordNumber = patientBroker.GetNewMrnFor( anAccount.Facility );
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void BuildPhysicianRelationships( SafeReader reader, IPhysicianBroker physicianBrokerLocal, Facility aFacility, AccountProxy accountProxy )
        {
            SafeReader nonStaffReader = null;

            PhysicianRelationship admittingPhysicianRelationship = new PhysicianRelationship();
            PhysicianRelationship referringPhysicianRelationship = new PhysicianRelationship();
            PhysicianRelationship attendingPhysicianRelationship = new PhysicianRelationship();
            PhysicianRelationship operatingPhysicianRelationship = new PhysicianRelationship();
            PhysicianRelationship primaryCarePhysicianRelationship = new PhysicianRelationship();

            long admittingDoctorId = reader.GetInt32( COL_ADMITTING_DOCTOR_ID );
            long referringDoctorId = reader.GetInt32( COL_REFERRING_DOCTOR_ID );
            long attendingDoctorId = reader.GetInt32( COL_ATTENDING_DOCTOR_ID );
            long operatingDoctorId = reader.GetInt32( COL_OPERATING_DOCTOR_ID );
            long PCPDoctorId = reader.GetInt32( COL_PCP_DOCTOR_ID );

            if ( admittingDoctorId == NON_STAFF_PHYSICIAN_NUMBER ||
                 referringDoctorId == NON_STAFF_PHYSICIAN_NUMBER ||
                 attendingDoctorId == NON_STAFF_PHYSICIAN_NUMBER ||
                 operatingDoctorId == NON_STAFF_PHYSICIAN_NUMBER ||
                 PCPDoctorId == NON_STAFF_PHYSICIAN_NUMBER )
            {
                iDB2Command cmd = null;

                try
                {
                    cmd = CommandFor( "CALL " + SP_NONSTAFF_PHYSICIAN_INFO +
                                           "(" + PARAM_HSPNUMBER +
                                           "," + PARAM_ACCOUNTNUMBER + ")",
                                           CommandType.Text,
                                           accountProxy.Facility );

                    cmd.Parameters[PARAM_HSPNUMBER].Value = accountProxy.Facility.Oid;
                    cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountProxy.AccountNumber;

                    nonStaffReader = ExecuteReader( cmd );

                    while ( nonStaffReader.Read() )
                    {
                        if ( admittingDoctorId == NON_STAFF_PHYSICIAN_NUMBER )
                        {
                            AdmittingPhysician admittingRole = new AdmittingPhysician();
                            Physician admittingPhysician =
                                AdmittingNonStaffPhysicianFrom( nonStaffReader );
                            admittingPhysicianRelationship = new PhysicianRelationship( admittingRole, admittingPhysician );
                        }

                        if ( referringDoctorId == NON_STAFF_PHYSICIAN_NUMBER )
                        {
                            PhysicianRole referringRole = new ReferringPhysician();
                            Physician referringPhysician =
                                ReferringNonStaffPhysicianFrom( nonStaffReader );
                            referringPhysicianRelationship = new PhysicianRelationship( referringRole, referringPhysician );
                        }

                        if ( attendingDoctorId == NON_STAFF_PHYSICIAN_NUMBER )
                        {
                            PhysicianRole attendingRole = new AttendingPhysician();
                            Physician attendingPhysician =
                                AttendingNonStaffPhysicianFrom( nonStaffReader );
                            attendingPhysicianRelationship = new PhysicianRelationship( attendingRole, attendingPhysician );
                        }

                        if ( operatingDoctorId == NON_STAFF_PHYSICIAN_NUMBER )
                        {
                            PhysicianRole operatingRole = new OperatingPhysician();
                            Physician operatingPhysician =
                                OperationNonStaffPhysicianFrom( nonStaffReader );
                            operatingPhysicianRelationship = new PhysicianRelationship( operatingRole, operatingPhysician );
                        }

                        if ( PCPDoctorId == NON_STAFF_PHYSICIAN_NUMBER )
                        {
                            PhysicianRole PCPRole = new PrimaryCarePhysician();
                            Physician primaryCarePhysician =
                                PrimaryCareNonStaffPhysicianFrom( nonStaffReader );
                            primaryCarePhysicianRelationship = new PhysicianRelationship( PCPRole, primaryCarePhysician );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    Log.Error( "Error retrieving PBAR Non Staff Physician information for account", ex );
                    throw;
                }
                finally
                {
                    Close( nonStaffReader );
                    Close( cmd );
                }
            }

            //Admitting Physician Relationship
            if ( admittingDoctorId != NON_STAFF_PHYSICIAN_NUMBER )
            {
                admittingPhysicianRelationship = physicianBrokerLocal.BuildAdmittingPhysicianRelationship(
                    aFacility.Oid, admittingDoctorId );
            }
            if ( admittingPhysicianRelationship.Physician != null )
            {
                accountProxy.AddPhysicianRelationship( admittingPhysicianRelationship );
            }

            //Referring Physician Relationship
            if ( referringDoctorId != NON_STAFF_PHYSICIAN_NUMBER )
            {
                referringPhysicianRelationship = physicianBrokerLocal.BuildReferringPhysicianRelationship(
                    aFacility.Oid, referringDoctorId );
            }
            if ( referringPhysicianRelationship.Physician != null )
            {
                accountProxy.AddPhysicianRelationship( referringPhysicianRelationship );
            }

            //Attending Physician Relationship
            if ( attendingDoctorId != NON_STAFF_PHYSICIAN_NUMBER )
            {
                attendingPhysicianRelationship = physicianBrokerLocal.BuildAttendingPhysicianRelationship(
                    aFacility.Oid, attendingDoctorId );
            }
            if ( attendingPhysicianRelationship.Physician != null )
            {
                accountProxy.AddPhysicianRelationship( attendingPhysicianRelationship );
            }

            //Operating Physician Relationship
            if ( operatingDoctorId != NON_STAFF_PHYSICIAN_NUMBER )
            {
                operatingPhysicianRelationship = physicianBrokerLocal.BuildOperatingPhysicianRelationship(
                    aFacility.Oid, operatingDoctorId );
            }
            if ( operatingPhysicianRelationship.Physician != null )
            {
                accountProxy.AddPhysicianRelationship( operatingPhysicianRelationship );
            }

            //PrimaryCare Physician Relationship			
            if ( PCPDoctorId != NON_STAFF_PHYSICIAN_NUMBER )
            {
                primaryCarePhysicianRelationship = physicianBrokerLocal.BuildPrimaryCarePhysicianRelationship(
                    aFacility.Oid, PCPDoctorId );
            }
            if ( primaryCarePhysicianRelationship.Physician != null )
            {
                accountProxy.AddPhysicianRelationship( primaryCarePhysicianRelationship );
            }

            BuildConsultingPhysicianRelationships( aFacility.Oid, accountProxy, physicianBrokerLocal, reader );
        }

        private static string CreateKindOfVisit(
            string lppsto, string lppsti, string lpfbil,
            long lpvis, string lppoln, long lpubal, string lpmsv )
        {
            string internalStatusCode = null;
            string returnVal = " ";

            // TLG 1/19/06 revisions for purge pending accounts

            // if purge pending
            if ( lppoln == "P" )
            {
                // pre purge
                if ( lppsti == "A" )
                    internalStatusCode = "M";

                    // inpatient purge (editable)
                else if ( lppsti == "B" )
                    internalStatusCode = "P";

                    // outpatient purge (editable)
                else if ( lppsto == "D" )
                    internalStatusCode = "Q";

                    // all other purge pending accounts
                else
                    internalStatusCode = "R";
            }
            else // not purge pending
            {
                if ( lppsti == "A" && lpubal == 0 )
                    internalStatusCode = "A";
                else if ( lppsti == "A" && lpubal != 0 )
                    internalStatusCode = "I";
                else if ( lppsti == "B" )
                    internalStatusCode = "B";
                else if ( lppsti == "C" || lppsti == "J" )
                    internalStatusCode = "C";
                else if ( lppsti == "G" && lppsto == " " )
                    internalStatusCode = "G";
                else if ( lppsti == "H" )
                    internalStatusCode = "H";
                else if ( lppsto == "K" )
                    internalStatusCode = "K";
                else if ( lppsto == "E" && lpfbil == "Y" )
                    internalStatusCode = "L";
                else if ( lppsto == "E" && lpfbil != "Y" )
                    internalStatusCode = "E";
                else if ( lppsto == "F" && lpfbil == "Y" )
                    internalStatusCode = "L";
                else if ( lppsto == "F" && lpfbil != "Y" )
                    internalStatusCode = "D";
                else if ( lppsto == "D" && lpvis == 0 )
                    internalStatusCode = "D";
                else if ( lppsto == "D" && lpvis != 0 )
                    internalStatusCode = "E";
            }

            if ( internalStatusCode == "M" )
                returnVal = Account.PRE_PUR;
            else if ( internalStatusCode == "A" )
                returnVal = Account.PRE_NC;
            else if ( internalStatusCode == "I" )
                returnVal = Account.PRE_CHG;
            else if ( internalStatusCode == "B" )
                returnVal = Account.INP_INH;
            else if ( internalStatusCode == "C" )
                returnVal = Account.INP_DIS;
            else if ( internalStatusCode == "E" )
                returnVal = Account.OUT_REC;
            else if ( internalStatusCode == "G" )
                returnVal = Account.PRE_CAN;
            else if ( internalStatusCode == "H" )
                returnVal = Account.INP_CAN;
            else if ( internalStatusCode == "K" )
                returnVal = Account.OUT_PRE;
            else if ( internalStatusCode == "L" )
                returnVal = Account.OUT_FIN;
            else if ( internalStatusCode == "D" )
                returnVal = Account.O_HSV + lpmsv;
            else if ( internalStatusCode == "P" )
                returnVal = Account.INH_PUR;
            else if ( internalStatusCode == "Q" )
                returnVal = Account.OUT_PUR;
            else if ( internalStatusCode == "R" )
                returnVal = Account.PND_PUR;


            return returnVal;
        }

        /// <exception cref="SoapException"><c>SoapException</c>.</exception>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ArrayList GetSOSPriorAccounts( PriorAccountsRequest request )
        {
            ArrayList accounts;

            try
            {
                accounts = dvBroker.GetPriorBalanceAccounts( request );
            }
            catch ( SoapException se )
            {
                // need to rendesvouz
                Log.Error( se.Detail.Value );
                throw;
            }
            catch ( Exception ex )
            {
                string ts = ex.ToString();
                string msg = ex.Message;
                Console.WriteLine( ts );
                Console.WriteLine( msg );
                // need to rendesvouz
                Log.Error( "Error retrieving SOS prior accounts for Patient Liability", ex );
                throw;
            }

            return accounts;
        }

        /// <summary>
        /// select previous accounts balances with/without payment plan.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private ArrayList GetPbarPriorAccounts( PriorAccountsRequest request )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            ArrayList accounts = new ArrayList();

            try
            {
                cmd = CommandFor( "CALL " + SP_GET_PRIOR_ACCOUNTS +
                                       "(" + PARAM_HSPNUMBER +
                                       "," + PARAM_HSPCODE +
                                       "," + PARAM_MRC + ")",
                                       CommandType.Text,
                                       request.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = request.FacilityOid;
                cmd.Parameters[PARAM_HSPCODE].Value = request.Facility.Code;
                cmd.Parameters[PARAM_MRC].Value = request.MedicalRecordNumber;

                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    long accountNumber = reader.GetInt64( COL_ACCOUNTNUMBER );
                    long dischargeDateValue = reader.GetInt64( COL_DISCHARGEDATE );
                    DateTime dischargeDate;
                    if ( dischargeDateValue != 0 )
                    {
                        dischargeDate = DateTimeUtilities.DateTimeFromPacked( dischargeDateValue );
                    }
                    else
                    {
                        dischargeDate = DateTime.MaxValue;
                    }

                    string patientTypeCode = reader.GetString( COL_PATIENTTYPECODE ).TrimEnd();
                    string financialClassCode = reader.GetString( COL_FINANCIALCLASSCODE ).TrimEnd();
                    decimal balanceDue = reader.GetDecimal( COL_BALANCEDUE );

                    bool hasPaymentPlan;
                    if ( financialClassCode == FIN_CLASS_73 )
                    {
                        hasPaymentPlan = true;
                    }
                    else
                    {
                        hasPaymentPlan = false;
                    }

                    AccountProxy ap = new AccountProxy();

                    ap.AccountNumber = accountNumber;
                    ap.DischargeDate = dischargeDate;

                    VisitType kindOfVisit = patientBroker.PatientTypeWith( request.Facility.Oid, patientTypeCode );
                    ap.KindOfVisit = kindOfVisit;

                    FinancialClass financialClass = fcBroker.FinancialClassWith( request.FacilityOid,
                                                                                 financialClassCode );
                    ap.FinancialClass = financialClass;

                    ap.BalanceDue = balanceDue;
                    ap.HasPaymentPlan = hasPaymentPlan;
                    ap.Facility = ( Facility )request.Facility.Clone();

                    accounts.Add( ap );
                }
            }
            catch ( Exception ex )
            {
                Log.Error( "Error retrieving PBAR prior accounts for Patient Liability", ex );
                throw;
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }

            return accounts;
        }

        /// <summary>
        /// Assign new AccountNumber
        /// </summary>
        /// <param name="anAccount"></param>
        /// <returns></returns>
        private void AssignAccountNumberTo( IAccount anAccount )
        {
            anAccount.AccountNumber = GetNewAccountNumberFor( anAccount.Facility );
        }

        /// <summary>
        /// Get Patient Driving License Information.
        /// </summary>
        private DriversLicense GetPatientDrivingLicenseInfo( string driversLicenseInfo, long facilityID )
        {

            State driversLicenseState = null;
            DriversLicense driversLicense = new DriversLicense( string.Empty );

            // Get rid of trailing spaces
            driversLicenseInfo = driversLicenseInfo.TrimEnd();

            if ( !String.IsNullOrEmpty( driversLicenseInfo ) )
            {
                // See if the string ends in uppercase letters preceded by 15 characters. If so,
                // we are going to assume this contains a state code
                String driversLicenseNumber;
                if ( Regex.IsMatch( driversLicenseInfo, @"(?:^.{15}\p{Lu}\p{Lu}$)" ) )
                {

                    // Create the state object using the last two characters
                    driversLicenseState =
                        addressBroker.StateWith(facilityID,
                            driversLicenseInfo.Substring(
                                driversLicenseInfo.Length - LENGTH_OF_STATE_CODE,
                                LENGTH_OF_STATE_CODE ) );

                    // Remove the last two characters from the buffer
                    driversLicenseNumber =
                        driversLicenseInfo.Remove( driversLicenseInfo.Length - LENGTH_OF_STATE_CODE )
                            .TrimEnd();

                }//if
                else
                {
                    // Assume there is some "NONE" or "NONE AVAIL" comment and treat as the number
                    driversLicenseNumber = driversLicenseInfo.TrimEnd();

                }//else

                // Create our new drivers license object given the extracted data
                driversLicense =
                    new DriversLicense( driversLicenseNumber, driversLicenseState );

            }//if

            return driversLicense;
        }

        /// <summary>
        /// fill in demographics info to account
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Unexpected Exception</exception>
        /// <exception cref="Exception">Unexpected Exception</exception>
        private void AddDemographicsTo( IAccount anAccount )
        {
            SafeReader reader = null;
            iDB2Command cmd = null;

            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_DEMOGRAPHICS_INFO +
                  "(" + PARAM_MRN +
                  "," + PARAM_FACILITYID + ")",
                  CommandType.Text,
                   anAccount.Facility );

                cmd.Parameters[PARAM_FACILITYID].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;

                reader = ExecuteReader( cmd );

                if ( reader.Read() )
                {
                    string maritalStatusCode = reader.GetString( COL_MARITALSTATUSID );
                    string raceCode = reader.GetString( COL_RACECODE ).Trim();
                    string race2Code = reader.GetString(COL_RACE2CODE).Trim();
                    string nationality1Code = reader.GetString(COL_NATIONALITY1).Trim();
                    string nationality2Code = reader.GetString(COL_NATIONALITY2).Trim();
                    string ethnicityCode = reader.GetString( COL_ETHNICITYCODE ).Trim();
                    string ethnicityCode2 = reader.GetString(COL_ETHNICITY2CODE).Trim();
                    string descentCode = reader.GetString(COL_DESCENT1).Trim();
                    string descent2Code = reader.GetString(COL_DESCENT2).Trim();
                    string nationalID = reader.GetString( COL_NATIONALID );
                    string drivingLicenseStr = reader.GetString( COL_DRIVINGLICENSE );
                    string placeOfBirth = reader.GetString( COL_PLACEOFBIRTH );
                    string languageCode = reader.GetString( COL_LANGUAGECODE );
                    string otherLanguage = reader.GetString( COL_OTHERLANGUAGE );
                    string religionCode = reader.GetString( COL_RELIGIONCODE );
                    string placeOfWorshipCode = reader.GetString( COL_RELIGIOUSCONGREGATIONCODE );
                    string timeOfBirth = reader.GetString( COL_TIMEOFBIRTH );
                    string birthSex = reader.GetString(COL_BIRTHSEX);

                    anAccount.Patient.DriversLicense = GetPatientDrivingLicenseInfo(drivingLicenseStr, anAccount.Facility.Oid);

                    if ( maritalStatusCode.Length > 0 )
                    {
                        anAccount.Patient.MaritalStatus = demographicsBroker.MaritalStatusWith( anAccount.Facility.Oid, maritalStatusCode );
                    }

                    if ( raceCode.Length > 0 )
                    {
                        anAccount.Patient.Race = originBroker.RaceWith( anAccount.Patient.Facility.Oid, raceCode );
                    }

                    if (race2Code.Length > 0)
                    {
                        anAccount.Patient.Race2 = originBroker.RaceWith(anAccount.Patient.Facility.Oid, race2Code);
                    }

                    if (nationality1Code.Length > 0)
                    {
                        anAccount.Patient.Nationality = originBroker.RaceWith(anAccount.Patient.Facility.Oid, nationality1Code);
                    }

                    if (nationality2Code.Length > 0)
                    {
                        anAccount.Patient.Nationality2 = originBroker.RaceWith(anAccount.Patient.Facility.Oid, nationality2Code);
                    }

                    if ( ethnicityCode.Length > 0 )
                    {
                        anAccount.Patient.Ethnicity = originBroker.EthnicityWith( anAccount.Patient.Facility.Oid, ethnicityCode );
                    }
                    if (ethnicityCode2.Length > 0)
                    {
                        anAccount.Patient.Ethnicity2 = originBroker.EthnicityWith(anAccount.Patient.Facility.Oid, ethnicityCode2);
                    }
                    if (descentCode.Length > 0)
                    {
                        anAccount.Patient.Descent = originBroker.EthnicityWith(anAccount.Patient.Facility.Oid, descentCode);
                    }
                    if (descent2Code.Length > 0)
                    {
                        anAccount.Patient.Descent2 = originBroker.EthnicityWith(anAccount.Patient.Facility.Oid, descent2Code);
                    }
                    anAccount.Patient.NationalID = nationalID;

                    anAccount.Patient.PlaceOfBirth = placeOfBirth;

                    if ( languageCode.Length > 0 )
                    {
                        Language language = demographicsBroker.LanguageWith( anAccount.Facility.Oid, languageCode );
                        anAccount.Patient.Language = language;

                        if ( language.IsOtherLanguage() && otherLanguage != null )
                        {
                            anAccount.Patient.OtherLanguage = otherLanguage;
                        }
                    }

                    if (birthSex.Length > 0)
                    {
                        anAccount.Patient.BirthSex =
                            demographicsBroker.GenderWith(anAccount.Patient.Facility.Oid, birthSex);
                    }

                    if ( religionCode.Length > 0 )
                    {
                        anAccount.Patient.Religion = religionBroker.ReligionWith( anAccount.Facility.Oid, religionCode );
                    }

                    if ( placeOfWorshipCode.Length > 0 )
                    {
                        anAccount.Patient.PlaceOfWorship = religionBroker.PlaceOfWorshipWith( anAccount.Facility.Oid, placeOfWorshipCode );
                    }
                    anAccount.Patient.DateOfBirth = GetDateOfBirthWithTime(anAccount.Patient.DateOfBirth, timeOfBirth.Trim());
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, Log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
        }
        private DateTime GetDateOfBirthWithTime( DateTime DOB, string timeOfBirth )
        {
            var hour = 0;
            var minute = 0;

            if ( timeOfBirth != string.Empty
                && timeOfBirth != "0000" )
            {
                var paddedTimeOfBirth = timeOfBirth.PadLeft(4, '0');
                hour = Convert.ToInt32( paddedTimeOfBirth.Substring( 0, 2 ) );
                minute = Convert.ToInt32( paddedTimeOfBirth.Substring( 2, 2 ) );
                if ((hour >= 0 && hour <= 23) && (minute >= 0 && minute <= 59))
                {
                    return new DateTime(DOB.Year, DOB.Month, DOB.Day, hour, minute, 0);
                }
            }
            return DOB;
        }
        /// <summary>
        /// read insurance for account
        /// </summary>
        /// <param name="account"></param>
        /// <param name="reader"></param>
        /// <exception cref="Exception">Unhandled Exception</exception>
        private Insurance InsuranceFor( Account account, SafeReader reader )
        {
            Insurance insurance = new Insurance();
            try
            {
                ArrayList insurances = InsurancesFor( account, reader );

                foreach ( Coverage coverage in insurances )
                {
                    if ( coverage != null )
                    {
                        DataValidationTicketType tt = coverage.CoverageOrder.Oid == PRIMARYCOVERAGEID ?
                            DataValidationTicketType.GetNewPrimaryCoveragTicketType() :
                            DataValidationTicketType.GetNewSecondaryCoveragTicketType();
                        coverage.DataValidationTicket = dvBroker.GetDataValidationTicketFor( account, tt );
                        insurance.AddCoverage( coverage );
                    }
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, Log );
            }

            return insurance;
        }

        /// <summary>
        /// get general info for account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadGeneralAccountData( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_GENERAL_ACCT_DATA +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );

                ReadDetailAccountData( anAccount, reader );
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// get insured data for account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadAccountInsuredData( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_ACCOUNT_INSURED_DATA +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );

                anAccount.Insurance = InsuranceFor( anAccount, reader );
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// get emergency contact for account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadAccountEmergContactData( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_ACCT_EMERG_CONTACT +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );

                anAccount.EmergencyContact1 = ContactFrom( anAccount, reader );
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// get nearest relative for the account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadAccountNearestRelContactData( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_NEAREST_RELATIVE +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );

                anAccount.EmergencyContact2 = ContactFrom( anAccount, reader );
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// get financial data for account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadFinancialData( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_ACCT_FINANCIAL_DATA +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );

                FinancialCouncelingDataFrom( anAccount, reader );
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// get post mse data for account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadPreDischargeLocation( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECTPREVOPLOCATION +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );
                if ( reader.Read() )
                {
                    string ns = reader.GetString( COL_OBSERVATIONNS );
                    string room = reader.GetString( COL_OBSERVATIONROOM );
                    string bed = reader.GetString( COL_OBSERVATIONBED );
                    Location preDischargeLocation = new Location( ns, room, bed );

                    anAccount.PreDischargeLocation = preDischargeLocation;
                }
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// get post mse data for account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadPostMSEData( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_ACCT_PREME_ACCT +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );
                if ( reader.Read() )
                {
                    long copiedAccountNumber = reader.GetInt64( COL_COPIEDFROMACCOUNTNUMBER );
                    if ( copiedAccountNumber != 0 )
                    {
                        anAccount.PreMSECopiedAccountNumber = copiedAccountNumber;

                        FinancialClass finClass = fcBroker.FinancialClassWith( anAccount.Facility.Oid,
                                                   reader.GetString( COL_FINANCIALCODE ).Trim() );

                        Insurance ins = ReadPostMSEInsData( anAccount );

                        if ( ValidInsuranceFor( anAccount, ins ) )
                        {
                            anAccount.Insurance = ins;
                            anAccount.FinancialClass = finClass;
                        }

                        // Read in Guarantor information...
                        // save original account's pre-mse account number
                        long lOriginalAccountNum = anAccount.AccountNumber;
                        anAccount.AccountNumber = anAccount.PreMSECopiedAccountNumber;
                        IGuarantorBroker guarantorBrokerLocal = BrokerFactory.BrokerOfType<IGuarantorBroker>();
                        guarantorBrokerLocal.GuarantorFor( anAccount );
                        ClearGuarantorCellphoneConsent( anAccount );
                        anAccount.AccountNumber = lOriginalAccountNum;
                    }
                }
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// get post mse data for account
        /// </summary>
        /// <param name="anAccount"></param>
        private void ReadPostMseMSPData( Account anAccount )
        {
            if ( anAccount.PreMSECopiedAccountNumber != 0 )
            {
                long emergContactPresent = 0;
                long nearestRelativePresent = 0;
                long mspPresent = 0;
                long finDataPresent = 0;
                long bvDataPresent = 0;


                // Read in MSP information from the Pre-MSE account's Copied from Account Number
                long originalPreMseAccountNumber = anAccount.AccountNumber;
                anAccount.AccountNumber = anAccount.PreMSECopiedAccountNumber;
                CheckIfAccountComponentsArePresent(
                    anAccount,
                    ref emergContactPresent,
                    ref nearestRelativePresent,
                    ref mspPresent,
                    ref finDataPresent,
                    ref bvDataPresent );

                if ( mspPresent != NO_DATA_PRESENT )
                {
                    IMSPBroker mspBroker = BrokerFactory.BrokerOfType<IMSPBroker>();
                    MedicareSecondaryPayor oldMsp = mspBroker.MSPFor( anAccount );
                    if ( oldMsp != null )
                    {
                        anAccount.MedicareSecondaryPayor = MedicareSecondaryPayor.GetPartiallyCopiedForwardMSPFrom( oldMsp );
                    }
                }

                anAccount.AccountNumber = originalPreMseAccountNumber;
            }
        }

        /// <summary>
        /// get post mse insurance data from the account
        /// </summary>
        /// <param name="anAccount"></param>
        /// <returns></returns>
        private Insurance ReadPostMSEInsData( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_PREMSE_INS_DATA +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );

                Insurance ins = InsuranceFor( anAccount, reader );

                RemoveValuesBeforeCompletingPostMse( ref ins );

                anAccount.COSSigned = new ConditionOfService();

                return ins;
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        private static void RemoveValuesBeforeCompletingPostMse( ref Insurance ins )
        {
            // OTD# 37738 fix - Do not copy forward COS signed and Liability fields during Pre to PostMSE Registration
            Coverage primaryCoverage = ins.PrimaryCoverage;
            RemoveCoverageValues( primaryCoverage );
            if ( primaryCoverage != null )
                ins.ForceChangedStatusFor( "PrimaryCoverage" );

            Coverage secondaryCoverage = ins.SecondaryCoverage;
            RemoveCoverageValues( secondaryCoverage );
            if ( secondaryCoverage != null )
                ins.ForceChangedStatusFor( "SecondaryCoverage" );
        }

        private static void RemoveCoverageValues( Coverage coverage )
        {
            if ( coverage != null )
            {
                coverage.CoPay = 0M;
                coverage.Deductible = 0M;
                coverage.NoLiability = false;
                coverage.RemoveAuthorization();
            }
        }

        private void CheckIfAccountComponentsArePresent(
            Account anAccount,
            ref long emergContactPresent,
            ref long nearestRelativePresent,
            ref long mspPresent,
            ref long finDataPresent,
            ref long bvDataPresent )
        {
            iDB2Command cmd = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_GETACCTCOMPSPRESENT +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_HSPCODE +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER +
                    "," + OUTPUT_PARAM_EMERGCONACTPRESENT +
                    "," + OUTPUT_PARAM_NEARESTRELAVICEPRESENT +
                    "," + OUTPUT_PARAM_MSPPRESENT +
                    "," + OUTPUT_PARAM_FINDATAPRESENT +
                    "," + OUTPUT_PARAM_BVDATAPRESENT +
                    ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_HSPCODE].Value = anAccount.Facility.Code;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                cmd.Parameters[OUTPUT_PARAM_EMERGCONACTPRESENT].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_NEARESTRELAVICEPRESENT].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_MSPPRESENT].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_FINDATAPRESENT].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PARAM_BVDATAPRESENT].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                emergContactPresent = Convert.ToInt64( cmd.Parameters[OUTPUT_PARAM_EMERGCONACTPRESENT].Value );
                nearestRelativePresent = Convert.ToInt64( cmd.Parameters[OUTPUT_PARAM_NEARESTRELAVICEPRESENT].Value );
                mspPresent = Convert.ToInt64( cmd.Parameters[OUTPUT_PARAM_MSPPRESENT].Value );
                finDataPresent = Convert.ToInt64( cmd.Parameters[OUTPUT_PARAM_FINDATAPRESENT].Value );
                bvDataPresent = Convert.ToInt64( cmd.Parameters[OUTPUT_PARAM_BVDATAPRESENT].Value );
            }

            finally
            {
                Close( cmd );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anAccount"></param>
        private void AddBenefitsDataTo( Account anAccount )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_BENEFIT_VAL_DATA +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    anAccount.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;

                reader = ExecuteReader( cmd );

                AddBenefitsDataTo( anAccount, reader );
            }

            finally
            {
                Close( reader );
                Close( cmd );
            }
        }
        /// <summary>
        /// Get insurances for account
        /// </summary>
        /// <param name="anAccount"></param>
        /// <param name="reader"></param>
        private ArrayList InsurancesFor( Account anAccount, SafeReader reader )
        {
            ArrayList insurances = new ArrayList();

            while ( reader.Read() )
            {
                Coverage aCoverage = CoverageFrom( anAccount, reader );
                if ( aCoverage != null )
                    insurances.Add( aCoverage );
            }
            return insurances;
        }

        /// <summary>
        /// read coverage from account
        /// </summary>
        /// <param name="anAccount"></param>
        /// <param name="reader"></param>
        private Coverage CoverageFrom( Account anAccount, SafeReader reader )
        {
            string billingCity = string.Empty;
            string billingState = string.Empty;
            string billingCountry = string.Empty;
            string billingZip = string.Empty;
            string billingZipExt = string.Empty;

            string billingPhone = string.Empty;

            Coverage aCoverage = null;

            string planID = reader.GetString( COL_INSURANCECOMPANYNUMBER ).TrimEnd();
            string priorityCode = reader.GetString( COL_PRIORITYCODE ).TrimEnd();

            string evcNumber = reader.GetString( COL_EVCNUMBER ).TrimEnd();
            string authNumber = reader.GetString( COL_AUTHORIZATIONNUMBER ).TrimEnd();
            int authorizedDays = reader.GetInt32( COL_AUTHORIZEDDAYS );
            string issueDate = reader.GetString( COL_ISSUEDATE ).TrimEnd();
            string certSsnHidHicNumber = reader.GetString( COL_INSURANCECERTIFICATIONNUM ).TrimEnd();
            string insuranceGroupNumber = reader.GetString( COL_INSURANCEGROUPNUMBER ).TrimEnd();
            string groupNumber = reader.GetString( COL_GROUPNUMBER ).TrimEnd();
            string trackingNumber = reader.GetString( COL_TRACKINGNUMBER ).TrimEnd();
            string signedOverMedicareHicNumber = reader.GetString( COL_SIGNEDOVERMEDICAREHICNUMBER ).TrimEnd();
            string mbiNumber = reader.GetString(COL_MBINUMBER).TrimEnd();

            string billingAddress = reader.GetString( COL_BILLINGADDRESS1 ).TrimEnd();
            string billingLoc = reader.GetString( COL_BILLINGCITYSTATECOUNTRY ).TrimEnd();
            string billingZipPhone = reader.GetString( COL_BILLINGZIPZIPEXTPHONE ).TrimEnd();
            string billingCareOfName = reader.GetString( COL_BILLING_CAREOF_NAME ).TrimEnd();
            string billingName = reader.GetString( COL_BILLING_NAME ).TrimEnd();

            decimal deductible = reader.GetDecimal( COL_DEDUCTIBLE );
            decimal copay = reader.GetDecimal( COL_COPAY );
            string noLiability = reader.GetString( COL_NOLIABILITY ).Trim();
            string eligibility = reader.GetString( COL_ELIGIBILITY ).Trim();
            long authorizationPhone = reader.GetInt64( COL_AUTHPHONE );
            string authorizationCompany = reader.GetString( COL_AUTHCOMPANY ).TrimEnd();
            string authRequired = reader.GetString( COL_AUTHREQD ).TrimEnd();
            string promptExt = reader.GetString( COL_PROMPTEXT );
            long intDateVerification = reader.GetInt64( COL_VERIFICATIONDATE );
            DateTime dateVerification = DateTimeUtilities.DateTimeForMMDDYYFormat( intDateVerification );
            string verifiedStatus = reader.GetString( COL_VERIFICATIONFLAG ).TrimEnd();
            string verifiedBy = reader.GetString( COL_VERIFICATIONBY ).TrimEnd();

            decimal approvalDate = reader.GetDecimal( COL_APPROVALDATE );
            decimal effectiveDate = reader.GetDecimal( COL_EFFECTIVEDATE );

            string attorneyName = reader.GetString( COL_ATTORNEYNAME );
            string attorneyStreet = reader.GetString( COL_ATTORNEYSTREET );
            string attorneyCity = reader.GetString( COL_ATTORNEYCITY );
            string attorneyState = reader.GetString( COL_ATTORNEYSTATE );
            string attorneyZip5 = reader.GetString( COL_ATTORNEYZIP5 );
            string attorneyZip4 = reader.GetString( COL_ATTORNEYZIP4 );
            string attorneyCountryCode = reader.GetString( COL_ATTORNEYCOUNTRYCODE ).Trim();
            long attorneyPhone = reader.GetInt64( COL_ATTORNEYPHONE );

            string agentName = reader.GetString( COL_AGENTNAME );
            string agentStreet = reader.GetString( COL_AGENTSTREET );
            string agentCity = reader.GetString( COL_AGENTCITY );
            string agentState = reader.GetString( COL_AGENTSTATE );
            string agentZip5 = reader.GetString( COL_AGENTZIP5 );
            string agentZip4 = reader.GetString( COL_AGENTZIP4 );
            string agentCountryCode = reader.GetString( COL_AGENTCOUNTRYCODE ).Trim();
            long agentPhone = reader.GetInt64( COL_AGENTPHONE );

            string adjustersName = reader.GetString( COL_ADJUSTERSNAME );
            string employeesSupervisor = reader.GetString( COL_EMPLOYEESSUPERVISOR );
            string insuranceCompanyName = reader.GetString( COL_INSURANCECOMPANYNAME ).Trim();

            if ( billingLoc.Length >= 15 )
            {
                billingCity = billingLoc.Substring( 0, 15 );
            }

            if ( billingLoc.Length >= 17 )
            {
                billingState = billingLoc.Substring( 15, 2 );
            }

            if ( billingLoc.Length >= 18 )
            {
                billingCountry = billingLoc.Substring( 17 ); //considered as country code
            }

            if ( billingZipPhone.Length >= 5 )
            {
                billingZip = billingZipPhone.Substring( 0, 5 );
            }

            if ( billingZipPhone.Length >= 9 )
            {
                billingZipExt = billingZipPhone.Substring( 5, 4 );
            }

            if ( billingZipPhone.Length >= 19 )
            {
                billingPhone = billingZipPhone.Substring( 9, 10 );
            }

            InsurancePlan insurancePlan = insuranceBroker.PlanWith( planID,
                approvalDate, effectiveDate, anAccount.Facility.Oid, anAccount.AccountNumber );


            Insured insured = InsuredFrom( anAccount, reader );

            if ( insurancePlan != null )
            {
                if ( insuranceCompanyName != string.Empty )
                {
                    insurancePlan.Payor.Name = insuranceCompanyName;
                }

                aCoverage = Coverage.CoverageFor( insurancePlan, insured );
                aCoverage.CoverageOrder = priorityCode == COVERAGEPRIORITYONE ?
                    new CoverageOrder( PRIMARYCOVERAGEID, COVERAGEPRIMARY ) :
                    new CoverageOrder( SECONDARYCOVERAGEID, COVERAGESECONDARY );

                if ( aCoverage.GetType() != typeof( GovernmentMedicareCoverage ) &&
                    aCoverage.GetType() != typeof( SelfPayCoverage ) )
                {
                    ( ( CoverageGroup )aCoverage ).Authorization.AuthorizationNumber = authNumber;
                    ( ( CoverageGroup )aCoverage ).Authorization.NumberOfDaysAuthorized = authorizedDays;
                }

                if (aCoverage.IsMedicareCoverageValidForAuthorization)
                {
                    ((GovernmentMedicareCoverage) aCoverage).Authorization.AuthorizationNumber = authNumber;
                    ((GovernmentMedicareCoverage)aCoverage).Authorization.NumberOfDaysAuthorized = authorizedDays;
                    ((GovernmentMedicareCoverage)aCoverage).TrackingNumber = trackingNumber; //"234567";
                }

                if ( aCoverage.GetType() == typeof( GovernmentMedicaidCoverage ) )
                {
                    ( ( GovernmentMedicaidCoverage )aCoverage ).PolicyCINNumber = certSsnHidHicNumber;
                    ( ( GovernmentMedicaidCoverage )aCoverage ).EVCNumber = evcNumber;
                    ((GovernmentMedicaidCoverage) aCoverage).TrackingNumber = trackingNumber; //"234567";
                    if ( issueDate.Length >= 8 )
                    {
                        string month = issueDate.Substring( 4, 2 );
                        string day = issueDate.Substring( 6, 2 );
                        string year = issueDate.Substring( 0, 4 );

                        if ( DateTimeUtilities.IsValidDateTime( month, day, year ) )
                        {
                            DateTime issueDateFormatted = new DateTime( Convert.ToInt16( year ),
                                Convert.ToInt16( month ),
                                Convert.ToInt16( day ) );

                            ( ( GovernmentMedicaidCoverage )aCoverage ).IssueDate = issueDateFormatted;
                        }
                    }
                }
                else if ( aCoverage.GetType() == typeof( WorkersCompensationCoverage ) )
                {
                    ( ( WorkersCompensationCoverage )aCoverage ).PolicyNumber = certSsnHidHicNumber;

                    //fill real source later
                    ( ( WorkersCompensationCoverage )aCoverage ).Authorization.AuthorizationNumber = authNumber;
                    ( ( WorkersCompensationCoverage )aCoverage ).InsuranceAdjuster = adjustersName; //"John Li";
                    ( ( WorkersCompensationCoverage )aCoverage ).PatientsSupervisor = employeesSupervisor;
                }
                else if ( aCoverage.GetType() == typeof( GovernmentMedicareCoverage ) )
                {
                    ( ( GovernmentMedicareCoverage )aCoverage ).HICNumber = certSsnHidHicNumber; // might be diff from SSN
                    mbiNumber = Coverage.TestMBINumbers.Contains(mbiNumber) ? string.Empty : mbiNumber;
                    ( (GovernmentMedicareCoverage )aCoverage ).MBINumber = mbiNumber;
                }
                else if ( aCoverage.GetType() == typeof( CommercialCoverage ) ||
                         aCoverage.GetType() == typeof( OtherCoverage ) ||
                         aCoverage.GetType() == typeof( GovernmentOtherCoverage ) )
                {

                    ( ( CoverageForCommercialOther )aCoverage ).CertSSNID = certSsnHidHicNumber;
                    ( ( CoverageForCommercialOther )aCoverage ).GroupNumber = groupNumber;
                    //fill real source later
                    ( ( CoverageForCommercialOther )aCoverage ).TrackingNumber = trackingNumber; //"234567";
                    ( ( CoverageForCommercialOther )aCoverage ).GroupNumber = insuranceGroupNumber;
                    ( ( CoverageForCommercialOther )aCoverage ).SignedOverMedicareHICNumber = signedOverMedicareHicNumber;
                    mbiNumber = Coverage.TestMBINumbers.Contains(mbiNumber) ? string.Empty : mbiNumber;
                    ( (CoverageForCommercialOther ) aCoverage ).MBINumber = mbiNumber;
                }

                aCoverage.BillingInformation.BillingCOName = billingCareOfName;
                aCoverage.BillingInformation.BillingName = billingName;

                Address anAddress = new Address(
                    StringFilter.RemoveHL7Chars( billingAddress ),
                    String.Empty,
                    StringFilter.RemoveHL7Chars( billingCity ),
                    new ZipCode( billingZip + billingZipExt ),
                    addressBroker.StateWith(anAccount.Facility.Oid, billingState ),
                    addressBroker.CountryWith( anAccount.Facility.Oid, billingCountry ) );

                aCoverage.BillingInformation.Address = anAddress;
                aCoverage.BillingInformation.PhoneNumber = new PhoneNumber( billingPhone );
                if ( aCoverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
                {
                    CoverageGroup coverageGroup = ( CoverageGroup )aCoverage;
                    coverageGroup.Authorization.AuthorizationCompany = authorizationCompany;
                    coverageGroup.Authorization.AuthorizationPhone = new PhoneNumber( authorizationPhone.ToString() );

                    switch ( authRequired )
                    {

                        case ( "YES" ):
                        case ( YesNotApplicableFlag.CODE_YES ):
                            {
                                coverageGroup.Authorization.AuthorizationRequired.SetYes();
                                break;
                            }
                        case ( "N" ):
                        case ( YesNotApplicableFlag.CODE_NOTAPPLICABLE ):
                            {

                                coverageGroup.Authorization.AuthorizationRequired.SetNotApplicable();
                                break;
                            }

                        default:
                            {

                                coverageGroup.Authorization.AuthorizationRequired.SetBlank();
                                break;
                            }
                    }

                    coverageGroup.Authorization.PromptExt = promptExt;

                }

                aCoverage.AuthorizingPerson = verifiedBy;

                switch ( verifiedStatus )
                {
                    case ( "YES" ):
                    case ( YesNotApplicableFlag.CODE_YES ):
                    case ( "V" ):
                        {

                            aCoverage.BenefitsVerified.SetYes();
                            break;
                        }
                    case ( YesNotApplicableFlag.CODE_NOTAPPLICABLE ):
                    case ( "N" ):
                        {

                            aCoverage.BenefitsVerified.SetNotApplicable();
                            break;
                        }
                    default:
                        {
                            aCoverage.BenefitsVerified.SetBlank();
                            break;
                        }
                }

                aCoverage.DateTimeOfVerification = dateVerification;

                aCoverage.Deductible = deductible;
                aCoverage.CoPay = copay;
                aCoverage.NoLiability = noLiability == "X" ? true : false;

                if ( ( eligibility == "Y" ) || ( eligibility == "Yes" ) )
                    aCoverage.EligibilityVerified.SetYes();
                else
                    if ( eligibility == "N/A" )
                    {
                        aCoverage.EligibilityVerified.SetNotApplicable();
                    }
                    else
                    {
                        aCoverage.EligibilityVerified.SetBlank();
                    }

                Attorney attorney = new Attorney();
                attorney.FirstName = attorneyName;
                ContactPoint attorneyContactPoint =
                    CreateContactPointUsing(
                        StringFilter.RemoveHL7Chars( attorneyStreet ),
                        StringFilter.RemoveHL7Chars( attorneyCity ),
                        attorneyState,
                        attorneyCountryCode, attorneyZip5, attorneyZip4, attorneyPhone, anAccount.Facility.Oid );
                attorney.AddContactPoint( attorneyContactPoint );

                InsuranceAgent agent = new InsuranceAgent();
                agent.FirstName = agentName;
                ContactPoint agentContactPoint =
                    CreateContactPointUsing(
                        StringFilter.RemoveHL7Chars( agentStreet ),
                        StringFilter.RemoveHL7Chars( agentCity ),
                        agentState,
                        agentCountryCode, agentZip5, agentZip4, agentPhone, anAccount.Facility.Oid );
                agent.AddContactPoint( agentContactPoint );

                aCoverage.Attorney = attorney;
                aCoverage.InsuranceAgent = agent;

                // SR39480 - KJS - 7/20/2007
                
                if (aCoverage.GetType().IsSubclassOf(typeof(CoverageGroup)))
                {
                    CoverageGroup coverageGroup = (CoverageGroup)aCoverage;
                
                    coverageGroup.Authorization.ServicesAuthorized = reader.GetString(COL_SERVICESAUTHORIZED);
                    coverageGroup.Authorization.EffectiveDate = reader.GetDateTime(COL_EFFECTIVEDATEOFAUTH);
                    coverageGroup.Authorization.ExpirationDate = reader.GetDateTime(COL_EXPIRATIONDATEOFAUTH);
                    coverageGroup.Authorization.Remarks = StringFilter
                        .RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                            reader.GetString(COL_AUTHORIZATIONREMARKS));
                    coverageGroup.Authorization.NameOfCompanyRepresentative =
                        new Name(reader.GetString(COL_AUTHCOMPANYREPFIRSTNAME),
                            reader.GetString(COL_AUTHCOMPANYREPLASTNAME),
                            String.Empty);

                    IAuthorizationStatusBroker authStatusBroker =
                        BrokerFactory.BrokerOfType<IAuthorizationStatusBroker>();

                    coverageGroup.Authorization.AuthorizationStatus =
                        authStatusBroker.AuthorizationStatusWith(reader.GetString(COL_AUTHORIZATIONSTATUS));

                }

                if ( aCoverage.IsMedicareCoverageValidForAuthorization )
                {
                    var coverageGroup = (GovernmentMedicareCoverage) aCoverage;

                    coverageGroup.Authorization.ServicesAuthorized = reader.GetString(COL_SERVICESAUTHORIZED);
                    coverageGroup.Authorization.EffectiveDate = reader.GetDateTime(COL_EFFECTIVEDATEOFAUTH);
                    coverageGroup.Authorization.ExpirationDate = reader.GetDateTime(COL_EXPIRATIONDATEOFAUTH);
                    coverageGroup.Authorization.Remarks = StringFilter
                        .RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                            reader.GetString(COL_AUTHORIZATIONREMARKS));
                    coverageGroup.Authorization.NameOfCompanyRepresentative =
                        new Name(reader.GetString(COL_AUTHCOMPANYREPFIRSTNAME),
                            reader.GetString(COL_AUTHCOMPANYREPLASTNAME),
                            String.Empty);

                    IAuthorizationStatusBroker authStatusBroker =
                        BrokerFactory.BrokerOfType<IAuthorizationStatusBroker>();

                    coverageGroup.Authorization.AuthorizationStatus =
                        authStatusBroker.AuthorizationStatusWith(reader.GetString(COL_AUTHORIZATIONSTATUS));

                }
            }
            if ( aCoverage != null )
            {
                aCoverage.Account = anAccount;
            }
            return aCoverage;
        }

        /// <summary>
        /// read insured data from account
        /// </summary>
        /// <param name="anAccount"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Unhandled Exception</exception>
        private Insured InsuredFrom( Account anAccount, SafeReader reader )
        {
            Insured anInsured = new Insured();

            try
            {
                string insuredLastName = reader.GetString( COL_INSUREDLASTNAME ).TrimEnd();
                insuredLastName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( insuredLastName );

                string insuredFirstName = reader.GetString( COL_INSUREDFIRSTNAME ).TrimEnd();
                insuredFirstName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( insuredFirstName );

                string insuredNameSuffix = reader.GetString(COL_INSUREDNAMESUFFIX).TrimEnd();
                insuredNameSuffix = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen(insuredNameSuffix);

                string insuredSex = reader.GetString( COL_INSUREDSEX ).TrimEnd();
                string insuredRelationship = reader.GetString( COL_INSUREDRELATIONSHIP ).TrimEnd();
                string insuredIdentifier = reader.GetString( COL_INSUREDIDENTIFIER ).TrimEnd();
                string insuredBirthDate = reader.GetString( COL_INSUREDBIRTHDATE ).TrimEnd();
                string insuredSsn = reader.GetString( COL_INSURANCECERTIFICATIONNUM ).TrimEnd();

                string groupNumber = reader.GetString( COL_GROUPNUMBER ).TrimEnd();

                string priorityCode = reader.GetString( COL_PRIORITYCODE ).TrimEnd();
                string conditionOfService = reader.GetString( COL_CONDITIONOFSERVICE ).Trim();

                string employmentStatus;
                string employerName;
                if ( priorityCode == COVERAGEPRIORITYONE )
                {
                    employmentStatus = reader.GetString( COL_PRIMARYEMPSTATUS ).TrimEnd();
                    employerName =
                        StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( COL_PRIMARYEMPNAME ) );
                    employerName = Employer.ModifyPBAREmployerName( employerName );
                }
                else //="2"
                {
                    employmentStatus = reader.GetString( COL_SECONDARYEMPSTATUS ).TrimEnd();
                    employerName =
                        StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( COL_SECONDARYEMPNAME ) );
                    employerName = Employer.ModifyPBAREmployerName( employerName );
                }

                string insuredMiddleInintial = StringFilter.StripMiddleInitialFromFirstName( ref insuredFirstName );
                insuredMiddleInintial = StringFilter.RemoveFirstCharNonLetter( insuredMiddleInintial );

                anInsured.Name.LastName = insuredLastName;
                anInsured.Name.FirstName = insuredFirstName;
                anInsured.Name.MiddleInitial = insuredMiddleInintial;
                anInsured.Name.Suffix = insuredNameSuffix;
                anInsured.LastName = insuredLastName;  //make sure it has value
                anInsured.FirstName = insuredFirstName;//make sure it has value
                anInsured.Sex = demographicsBroker.GenderWith( anAccount.Facility.Oid, insuredSex );

                RelationshipType aRelationshipType = relTypeBroker.RelationshipTypeWith( anAccount.Facility.Oid, insuredRelationship );

                Relationship aRelationship = new Relationship( aRelationshipType, anAccount.Patient.GetType(), anInsured.GetType() );
                anInsured.AddRelationship( aRelationship );

                if ( conditionOfService != null &&
                    conditionOfService.Trim().Length > 0 &&
                    priorityCode.Equals( COVERAGEPRIORITYONE ) )
                {
                    IConditionOfServiceBroker cosBroker = BrokerFactory.BrokerOfType<IConditionOfServiceBroker>();
                    anAccount.COSSigned = cosBroker.ConditionOfServiceWith( conditionOfService );
                    if ( !anAccount.COSSigned.IsValid )
                    {
                        anAccount.COSSigned = cosBroker.ConditionOfServiceWith( ConditionOfService.BLANK );
                    }
                }

                anInsured.NationalID = insuredIdentifier;

                if ( insuredBirthDate.Length == 8 && insuredBirthDate != "00000000" )
                {
                    string mm = insuredBirthDate.Substring( 0, 2 );
                    string dd = insuredBirthDate.Substring( 2, 2 );
                    string yyyy = insuredBirthDate.Substring( 4, 4 );

                    if ( DateTimeUtilities.IsValidDateTime( mm, dd, yyyy ) )
                    {
                        DateTime dateOfBirth = new DateTime( Convert.ToInt16( yyyy ),
                            Convert.ToInt16( mm ),
                            Convert.ToInt16( dd ) );

                        anInsured.DateOfBirth = dateOfBirth;
                    }
                }

                anInsured.GroupNumber = groupNumber;

                InsuredAddressStrategy strategy = new InsuredAddressStrategy();

                strategy.LoadContactPointOn( anInsured, reader, anAccount.Facility.Oid );

                if ( insuredSsn.Length == 9 && IsNumeric( insuredSsn ) )
                {
                    anInsured.SocialSecurityNumber = new SocialSecurityNumber( insuredSsn );
                }

                Employment employment = new Employment( anInsured );

                anInsured.Employment = employment;

                anInsured.Employment.Status = employmentStatusBroker.EmploymentStatusWith( anAccount.Facility.Oid, employmentStatus );

                InsuredEmployerAddressStrategy strategy2 = new InsuredEmployerAddressStrategy();
                strategy2.LoadContactPointOn( anInsured, reader, anAccount.Facility.Oid );


                anInsured.Employment.Employer.Name = employerName;

            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, Log );
            }

            return anInsured;
        }

        /// <summary>
        /// verify if given object is a number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsNumeric( object value )
        {
            try
            {
                Convert.ToDouble( value.ToString() );
                return true;
            }
            // 2/6/2007 - kjs change exception type from format exception to System exception
            // anything that causes this conversion to fail 
            catch ( Exception )
            {
                return false;
            }
        }

        /// <summary>
        /// read benefits data from account
        /// </summary>
        /// <param name="anAccount"></param>
        /// <param name="reader"></param>
        private void AddBenefitsDataTo( Account anAccount, SafeReader reader )
        {
            while ( reader.Read() )
            {
                Coverage cv = anAccount.Insurance.CoverageFor( long.Parse( reader.GetString( COL_PRIORITYCODE ) ) );
                ReadBenefitsInfoFor( anAccount.Facility.Oid, cv, reader );

                if ( cv != null && cv.GetType().BaseType == typeof( CoverageForCommercialOther ) )
                {
                    CommercialCoverage cov = cv as CommercialCoverage;

                    if ( cov != null )
                    {
                        CommercialConstraints cc = new CommercialConstraints();
                        ArrayList al = new ArrayList( cov.BenefitsCategories.Values );
                        cc.BenefitsCategoryDetails = al;
                    }
                }
            }
        }

        /// <summary>
        /// set contact point
        /// </summary>
        /// <param name="aStreet"></param>
        /// <param name="aCity"></param>
        /// <param name="aState"></param>
        /// <param name="aCountryCode"></param>
        /// <param name="aZip5"></param>
        /// <param name="aZip4"></param>
        /// <param name="aPhone"></param>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        private ContactPoint CreateContactPointUsing( string aStreet, string aCity, string aState, string aCountryCode, string aZip5, string aZip4, long aPhone, long facilityID )
        {
            State state = new State();
            if ( aState.Trim() != String.Empty )
            {
                state = addressBroker.StateWith( facilityID,aState );
            }

            Country country = new Country();
            if ( aCountryCode.Trim() != String.Empty )
            {
                country = addressBroker.CountryWith( facilityID, aCountryCode );
            }

            string aPostalCode = aZip5.Trim() + aZip4.Trim();

            Address address = new Address( aStreet.Trim(), String.Empty, aCity.Trim(), new ZipCode( aPostalCode ), state, country );
            PhoneNumber phone = new PhoneNumber();
            if ( aPhone != 0 )
            {
                phone = new PhoneNumber( aPhone.ToString() );
            }
            ContactPoint cp = new ContactPoint( address, phone, new EmailAddress(), TypeOfContactPoint.NewBusinessContactPointType() );

            return cp;
        }

        /// <summary>
        /// read benefit data for coverage
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="coverage"></param>
        /// <param name="reader"></param>
        private void ReadBenefitsInfoFor( long facilityID, Coverage coverage, SafeReader reader )
        {
            if ( coverage is CommercialCoverage )
            {
                ReadBenefitsInfoFor( facilityID, coverage as CommercialCoverage, reader );
            }
            else if ( coverage is GovernmentOtherCoverage )
            {
                GovernmentOtherConstraints goc = new GovernmentOtherConstraints();
                ReadBenefitsInfoFor( facilityID, coverage as GovernmentOtherCoverage, reader );

                goc.BenefitsCategoryDetails = ( coverage as GovernmentOtherCoverage ).BenefitsCategoryDetails;
            }
            else if ( coverage is GovernmentMedicareCoverage )
            {
                ReadBenefitsInfoFor( coverage as GovernmentMedicareCoverage, reader );
            }
            else if ( coverage is GovernmentMedicaidCoverage )
            {
                ReadBenefitsInfoFor( coverage as GovernmentMedicaidCoverage, reader );
            }
            else if ( coverage is SelfPayCoverage )
            {
                ReadBenefitsInfoFor( coverage as SelfPayCoverage, reader );
            }
            else if ( coverage is WorkersCompensationCoverage )
            {
                ReadBenefitsInfoFor( coverage as WorkersCompensationCoverage, reader );
            }
        }

        /// <summary>
        /// read benefit data for government medicaid coverage
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="reader"></param>
        private void ReadBenefitsInfoFor( GovernmentMedicaidCoverage coverage, SafeReader reader )
        {
            string coveragePriority = reader.GetString( COL_MCD_PRIORITY_CODE );
            if ( coveragePriority != null && !coveragePriority.Equals( string.Empty ) )
            {
                coverage.EligibilityDate = reader.GetDateTime( COL_MCD_ELIGIBILTYDATE );
                coverage.PatienthasMedicare = new YesNoFlag( reader.GetString( COL_MCD_PATIENT_HAS_MEDICARE ) );
                coverage.PatienthasOtherInsuranceCoverage = new YesNoFlag( reader.GetString( COL_MCD_PATIENT_HAS_OTHER_INS ) );
                coverage.MedicaidCopay = ( float )reader.GetDecimal( COL_MCD_COPAY_AMOUNT );
                coverage.EVCNumber = reader.GetString( COL_MCD_EVC_NUMBER );
                coverage.InformationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith(
                        reader.GetString( COL_MCD_INFO_RECEIVED_FROM ) );
                coverage.Remarks = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                    reader.GetString( COL_MCD_REMARKS ).TrimEnd() );
            }
        }

        /// <summary>
        /// read benefit data for commercial coverage
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="coverage"></param>
        /// <param name="reader"></param>
        private void ReadBenefitsInfoFor( long facilityID, CommercialCoverage coverage, SafeReader reader )
        {
            CommercialCoverage comCov = coverage;

            string coveragePriority = reader.GetString( COL_COM_PRIORITY_CODE );
            if ( coveragePriority != null && !coveragePriority.Equals( string.Empty ) )
            {
                coverage.InformationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith(
                        reader.GetString( COL_COM_INFO_RECEIVED_FROM ) );
                coverage.EligibilityPhone = reader.GetString( COL_COM_ELIGIBILITY_PHONE_NUMBER ).TrimEnd();
                coverage.InsuranceCompanyRepName = reader.GetString( COL_COM_INSURANCE_COMP_REP_NAME ).TrimEnd();
                coverage.EffectiveDateForInsured = reader.GetDateTime( COL_COM_EFFECTIVE_DATE_OF_INSURED );
                coverage.TerminationDateForInsured = reader.GetDateTime( COL_COM_TERM_DATE_OF_INSURED );
                coverage.ServiceForPreExistingCondition = new YesNoFlag(
                    reader.GetString( COL_COM_ISSVC_FORPREEXISTING_COND ) );
                coverage.ServiceIsCoveredBenefit = new YesNoFlag(
                    reader.GetString( COL_COM_ISSERVICEA_COVERED_BENEFIT ) );
                coverage.ClaimsAddressVerified = new YesNoFlag(
                    reader.GetString( COL_COM_CLAIMSA_ADDRESS_VERIFIED ) );
                coverage.CoordinationOfbenefits = new YesNoFlag(
                    reader.GetString( COL_COM_COORD_OF_BENEFITS ) );

                ITypeOfVerificationRuleBroker typeOfVerificationRuleBroker = BrokerFactory.BrokerOfType<ITypeOfVerificationRuleBroker>();
                coverage.TypeOfVerificationRule = typeOfVerificationRuleBroker.TypeOfVerificationRuleWith(
                    reader.GetInt64( COL_COM_RULE_TO_DETERMINE_COB ) );

                ITypeOfProductBroker typeOfProductBroker = BrokerFactory.BrokerOfType<ITypeOfProductBroker>();
                coverage.TypeOfProduct = typeOfProductBroker.TypeOfProductWith(
                    reader.GetInt64( COL_COM_TYPE_OF_PRODUCT ) );

                coverage.PPOPricingOrBroker = reader.GetString( COL_COM_NAMEOFPPONETWORK_ORBROKER ).TrimEnd();
                coverage.FacilityContractedProvider = new YesNoFlag(
                    reader.GetString( COL_COM_HOSP_IS_CONTRACTED_PROV ) );
                coverage.AutoInsuranceClaimNumber = reader.GetString( COL_COM_AUTO_CLAIM_NUMBER ).TrimEnd();
                coverage.AutoMedPayCoverage = new YesNoFlag(
                    reader.GetString( COL_COM_AUTO_MEDPAY_COVERAGE ) );
                coverage.Remarks = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                    reader.GetString( COL_COM_REMARKS ).TrimEnd() );

                // now see if there is a benefits category section to read
                string cbcPriority = reader.GetString( COL_CBC_PRIORITY_CODE );
                if ( cbcPriority != null && !cbcPriority.Equals( string.Empty ) )
                {
                    BenefitsCategoryDetails bcd = new BenefitsCategoryDetails();
                    long benefitsCategoryID = reader.GetInt64( COL_BC_BENEFIT_CATEGORY_ID );
                    bcd.BenefitCategory = GetBeneiftCategoryById( facilityID, benefitsCategoryID );
                    bcd.Deductible = ( float )reader.GetDecimal( COL_BC_DEDUCTABLE_AMOUNT );
                    bcd.TimePeriod = new TimePeriodFlag( reader.GetString( COL_BC_TIME_PERIOD ) );
                    bcd.DeductibleMet = new YesNoFlag( reader.GetString( COL_BC_DEDUCTABLE_MET ) );
                    bcd.DeductibleDollarsMet = ( float )reader.GetDecimal( COL_BC_DEDUCTABLE_METAMOUNT );
                    bcd.CoInsurance = ( int )reader.GetInt64( COL_BC_COINSURANCE );
                    bcd.OutOfPocket = ( float )reader.GetDecimal( COL_BC_OUT_OF_POCKET );
                    bcd.OutOfPocketMet = new YesNoFlag( reader.GetString( COL_BC_OUTOF_POCKET_MET ) );
                    bcd.OutOfPocketDollarsMet = ( float )reader.GetDecimal( COL_BC_OUTOF_POCKETMET_AMOUNT );
                    bcd.AfterOutOfPocketPercent = ( int )reader.GetInt64( COL_BC_PCT_AFTER_OUTOF_POCKET );
                    bcd.CoPay = ( float )reader.GetDecimal( COL_BC_COPAY_AMOUNT );
                    bcd.WaiveCopayIfAdmitted = new YesNoFlag( reader.GetString( COL_BC_WAIVE_COPAYIF_ADMITTED ) );
                    bcd.VisitsPerYear = ( int )reader.GetInt64( COL_BC_NUMBER_OFVISITS_PER_YEAR );
                    bcd.LifeTimeMaxBenefit = ( double )reader.GetDecimal( COL_BC_LIFETIME_MAX_BENEFIT );
                    bcd.RemainingLifetimeValue = ( double )reader.GetDecimal( COL_BC_REM_LIFETIME_VALUE );
                    bcd.RemainingLifetimeValueMet = new YesNoFlag( reader.GetString( COL_BC_REM_LIFETIME_VALUEMET ) );
                    bcd.MaxBenefitPerVisit = ( double )reader.GetDecimal( COL_BC_MAXIMUM_BENEFIT_PERVISIT );
                    bcd.RemainingBenefitPerVisits = ( double )reader.GetDecimal( COL_BC_REM_BENEFIT_PERVISIT );
                    bcd.RemainingBenefitPerVisitsMet = new YesNoFlag( reader.GetString( COL_BC_REM_BENEFIT_PERVISITMET ) );

                    comCov.AddBenefitsCategory( bcd.BenefitCategory, bcd );
                }
            }
        }

        /// <summary>
        /// get benefit category for a specific id
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        private static BenefitsCategory GetBeneiftCategoryById( long facilityID, long oid )
        {
            IBenefitsCategoryBroker benefitsCategoryBroker = BrokerFactory.BrokerOfType<IBenefitsCategoryBroker>();
            BenefitsCategory result = benefitsCategoryBroker.BenefitsCategoryWith( facilityID, oid );
            return result;
        }

        /// <summary>
        /// read benefit data for government medicare coverage
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="reader"></param>
        private void ReadBenefitsInfoFor( GovernmentMedicareCoverage coverage, SafeReader reader )
        {
            string coveragePriority = reader.GetString( COL_MCR_PRIORITY_CODE );
            if ( coveragePriority != null && !coveragePriority.Equals( string.Empty ) )
            {
                coverage.PartACoverage = new YesNoFlag( reader.GetString( COL_MCR_PARTA_COVERAGE ) );
                coverage.PartACoverageEffectiveDate = reader.GetDateTime( COL_MCR_PARTA_EFFECTIVE_DATE );
                coverage.PartBCoverage = new YesNoFlag( reader.GetString( COL_MCR_PARTB_COVERAGE ) );
                coverage.PartBCoverageEffectiveDate = reader.GetDateTime( COL_MCR_PARTB_EFFECTIVE_DATE );
                coverage.PatientHasMedicareHMOCoverage = new YesNoFlag( reader.GetString( COL_MCR_PAT_HAS_MEDICARE_HMO_CVRG ) );
                coverage.MedicareIsSecondary = new YesNoFlag( reader.GetString( COL_MCR_MEDICARE_IS_SECONDARY ) );
                coverage.DateOfLastBillingActivity = reader.GetDateTime( COL_MCR_DATEOFLAST_BILL_ACTIVITY );
                coverage.RemainingBenefitPeriod = ( int )reader.GetInt64( COL_MCR_REM_HOSPITAL_DAYS );
                coverage.RemainingCoInsurance = ( int )reader.GetInt64( COL_MCR_REM_COINSURANCE_DAYS );
                coverage.RemainingLifeTimeReserve = ( int )reader.GetInt64( COL_MCR_REM_LIFETIME_RESERVE_DAYS );
                coverage.RemainingSNF = ( int )reader.GetInt64( COL_MCR_REM_SNF_DAYS );
                coverage.RemainingSNFCoInsurance = ( int )reader.GetInt64( COL_MCR_REM_SNF_COINS_DAYS );
                coverage.RemainingPartADeductible = ( float )reader.GetDecimal( COL_MCR_REM_PART_A_DEDUCTIBLE );
                coverage.RemainingPartBDeductible = ( float )reader.GetDecimal( COL_MCR_REM_PART_B_DEDUCTIBLE );
                coverage.PatientIsPartOfHospiceProgram = new YesNoFlag( reader.GetString( COL_MCR_PATIENT_IS_IN_HOSPICE ) );
                coverage.VerifiedBeneficiaryName = new YesNoFlag( reader.GetString( COL_MCR_BENEFICIARY_NAME_VERIFIED ) );
                coverage.InformationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith(
                    reader.GetString( COL_MRC_INFO_RECEIVED_FROM ) );
                coverage.Remarks = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                    reader.GetString( COL_MCR_REMARKS ).TrimEnd() );
            }
        }

        /// <summary>
        /// read benefit data for self pay coverage
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="reader"></param>
        private static void ReadBenefitsInfoFor( SelfPayCoverage coverage, SafeReader reader )
        {
            string coveragePriority = reader.GetString( COL_SELF_PRIORITY_CODE );
            if ( coveragePriority != null && !coveragePriority.Equals( string.Empty ) )
            {
                coverage.PatientHasMedicaid = new YesNoFlag( reader.GetString( COL_SELF_HAS_MEDICAID ) );
                coverage.InsuranceInfoUnavailable = new YesNoFlag( reader.GetString( COL_SELF_INSURANCE_INFO_AVAILABLE ) );
            }
        }

        /// <summary>
        /// read benefit data for government other coverage
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="coverage"></param>
        /// <param name="reader"></param>
        private void ReadBenefitsInfoFor( long facilityID, GovernmentOtherCoverage coverage, SafeReader reader )
        {
            string coveragePriority = reader.GetString( COL_GOV_PRIORITY_CODE );
            if ( coveragePriority != null && !coveragePriority.Equals( string.Empty ) )
            {
                coverage.InformationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith(
                    reader.GetString( COL_GOV_INFO_RECEIVED_FROM ) );
                coverage.EligibilityPhone = reader.GetString( COL_GOV_ELIGIBILITY_PHONE_NUM ).TrimEnd();
                coverage.InsuranceCompanyRepName = reader.GetString( COL_GOV_INS_COMPANY_REP_NAME ).TrimEnd();
                coverage.TypeOfCoverage = reader.GetString( COL_GOV_TYPE_OF_COVERAGE ).TrimEnd();
                coverage.EffectiveDateForInsured = reader.GetDateTime( COL_GOV_EFFECTIVE_DATEOF_INSURED );
                coverage.TerminationDateForInsured = reader.GetDateTime( COL_GOV_TERM_DATEOF_INSURED );
                coverage.BenefitsCategoryDetails = new BenefitsCategoryDetails();
                long benefitsCategoryID = reader.GetInt64( COL_BC_BENEFIT_CATEGORY_ID );
                coverage.BenefitsCategoryDetails.BenefitCategory = GetBeneiftCategoryById( facilityID, benefitsCategoryID );
                coverage.BenefitsCategoryDetails.Deductible = ( float )reader.GetDecimal( COL_GOV_DEDUCTABLE_AMOUNT );
                coverage.BenefitsCategoryDetails.DeductibleMet =
                    new YesNoFlag( reader.GetString( COL_GOV_ISDEDUCTABLE_MET ) );
                coverage.BenefitsCategoryDetails.DeductibleDollarsMet = ( float )reader.GetDecimal( COL_GOV_DEDUCTABLE_AMOUNT_MET );
                coverage.BenefitsCategoryDetails.CoInsurance = ( int )reader.GetInt64( COL_GOV_COINSURANCE );
                coverage.BenefitsCategoryDetails.OutOfPocket = ( float )reader.GetDecimal( COL_GOV_OUT_OF_POCKET );
                coverage.BenefitsCategoryDetails.OutOfPocketMet =
                    new YesNoFlag( reader.GetString( COL_GOV_IS_OUTOFPOCKET_MET ) );
                coverage.BenefitsCategoryDetails.OutOfPocketDollarsMet = ( float )reader.GetDecimal( COL_GOV_OUTOFPOCKET_AMOUNT_MET );
                coverage.BenefitsCategoryDetails.AfterOutOfPocketPercent = ( int )reader.GetInt64( COL_GOV_PERCENT_OUTOFPOCKET );
                coverage.BenefitsCategoryDetails.CoPay = ( float )reader.GetDecimal( COL_COPAY_AMOUNT );
                coverage.Remarks = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                    reader.GetString( COL_GOV_REMARKS ).TrimEnd() );

            }
        }

        /// <summary>
        /// read benefit data for workers compensation coverage
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="reader"></param>
        private void ReadBenefitsInfoFor( WorkersCompensationCoverage coverage, SafeReader reader )
        {
            string coveragePriority = reader.GetString( COL_WKC_PRIORITY_CODE );
            if ( coveragePriority != null && !coveragePriority.Equals( string.Empty ) )
            {
                coverage.PPOPricingOrBroker = reader.GetString( COL_WKC_NAME_OF_NETWORK ).TrimEnd();
                coverage.ClaimNumberForIncident = reader.GetString( COL_WKC_CLAIM_NUMBER_FOR_INCIDENT ).TrimEnd();
                coverage.ClaimsAddressVerified = new YesNoFlag( reader.GetString( COL_WKC_CLAIM_ADDRESS_VERIFIED ) );
                coverage.InsurancePhone = reader.GetString( COL_WKC_INSURANCE_PHONE_NUMBER ).TrimEnd();
                coverage.EmployerhasPaidPremiumsToDate = new YesNoFlag( reader.GetString( COL_WKC_PREMIUM_PAID_TODATE ) );
                coverage.InformationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith(
                    reader.GetString( COL_WKC_INFO_RECEIVED_FROM ) );
                coverage.Remarks = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                    reader.GetString( COL_WKC_REMARKS ).TrimEnd() );
            }
        }

        private EmergencyContact ContactFrom( Account anAccount, SafeReader reader )
        {
            EmergencyContact contact = new EmergencyContact();

            if ( reader.Read() )
            {
                string contactName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma( reader.GetString( COL_CONTACTNAME ).TrimEnd() );

                contact.Name = contactName;

                string relationshipCode = reader.GetString( COL_RELATIONSHIPCODE ).TrimEnd();
                RelationshipType aRelationshipType = relTypeBroker.RelationshipTypeWith( anAccount.Facility.Oid, relationshipCode );

                if ( aRelationshipType != null )
                {
                    Relationship aRelationship = new Relationship( aRelationshipType, anAccount.Patient.GetType(), contact.GetType() );

                    anAccount.Patient.RemoveRelationship( aRelationshipType );
                    anAccount.Patient.AddRelationship( aRelationship );  //add to patient directly

                    contact.RemoveRelationship( aRelationshipType );
                    contact.AddRelationship( aRelationship );
                    contact.RelationshipType = aRelationshipType;

                    LoadContactPointOn( anAccount.Facility.Oid, contact, reader );
                }
            }

            return contact;
        }

        private void LoadContactPointOn( long facilityID, Party aParty, SafeReader fromReader )
        {
            // see if contact point exists if it does not then create one
            ContactPoint cp;
            ArrayList cps = ( ArrayList )aParty.ContactPoints;
            if ( cps.Count == 0 )
            {
                cp = new ContactPoint();
                aParty.AddContactPoint( cp );
            }
            else
            {
                cp = ( ContactPoint )cps[0];
            }
            // set the address on the contact point;
            cp.TypeOfContactPoint = TypeOfContactPoint.NewPhysicalContactPointType();

            Address anAddress = ReadAddressFrom( facilityID, fromReader );
            cp.Address = anAddress;

            PhoneNumber pn = ReadPhoneNumberFrom( fromReader );
            cp.PhoneNumber = pn;
        }

        private Address ReadAddressFrom( long facilityID, SafeReader fromReader )
        {
            string countryCode = string.Empty;

            string address = fromReader.GetString( COL_ADDRESS ).TrimEnd();
            address = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( address );

            string city = fromReader.GetString( COL_CITY ).TrimEnd();
            city = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( city );

            string state = fromReader.GetString( COL_STATE ).TrimEnd();
            string zip = fromReader.GetString( COL_ZIP ).Trim();
            string zipExt = fromReader.GetString( COL_ZIPEXT ).Trim();
            string zipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( zip + zipExt );

            Address anAddress = new Address( address, String.Empty, city, new ZipCode( zipCode ),
                addressBroker.StateWith( facilityID,state ), addressBroker.CountryWith( facilityID, countryCode ) );

            return anAddress;
        }

        private static PhoneNumber ReadPhoneNumberFrom( SafeReader reader )
        {
            PhoneNumber aPhoneNumber = null;


            string areaCode = reader.GetInt64( COL_AREACODE ).ToString();
            areaCode = areaCode.PadLeft( 3, '0' );
            string phoneNumber = reader.GetInt64( COL_PHONENUMBER ).ToString();
            phoneNumber = phoneNumber.PadLeft( 7, '0' );

            if ( areaCode != "0" && phoneNumber != "0" )
            {
                aPhoneNumber = new PhoneNumber( areaCode, phoneNumber );
            }

            return aPhoneNumber;
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void FinancialCouncelingDataFrom( Account anAccount, SafeReader reader )
        {
            try
            {
                if ( reader.Read() )
                {
                    long billDropped = reader.GetInt64( COL_BILLDROPPED );
                    var lastChargeDate = DateTimeUtilities.DateTimeForYYMMDDFormat( reader.GetInt64( COL_LAST_CHARGEDATE ) );

                    decimal insuredTotalPaid = reader.GetDecimal( COL_INS_TOTAL_PAYMENTS_COLLECTED );
                    int insNumberOfMonthlyPayments = reader.GetInt32( COL_INS_NUMBER_OF_MONTHLY_PAYMENTS );
                    decimal insuredTotalMonthlyPaymentDue = reader.GetDecimal( COL_INS_TOTAL_MONTHLY_PAYMENTS_DUE );

                    string dayOfMonthPaymentDue = reader.GetString( COL_DAY_OF_MONTH_PAYMENT );
                    decimal totalCurrentAmtDue = reader.GetDecimal( COL_TOTAL_AMOUNT_DUE );
                    decimal discPlanAmount = reader.GetDecimal( COL_DISC_PLAN_AMOUNT );

                    decimal unInsuredTotalPaid = reader.GetDecimal( COL_UNINS_TOTAL_PAYMENTS_COLLECTED );
                    int unInsNumberOfMonthlyPayments = reader.GetInt32( COL_UNINS_NUMBER_OF_MONTHLY_PAYMENTS );
                    decimal unInsTotalMonthlyPaymentsDue = reader.GetDecimal( COL_UNINS_TOTAL_MONTHLYPAYMENTSDUE );

                    string resourceListProvided = reader.GetString( COL_RESOURCE_LIST_PROVIDED );

                    anAccount.BillHasDropped = billDropped != 0 ? true : false;
                    anAccount.LastChargeDate = lastChargeDate;
                    if ( fcBroker.IsUninsured( anAccount.Facility.Oid, anAccount.FinancialClass ) )
                    {
                        anAccount.TotalPaid = unInsuredTotalPaid;
                        anAccount.TotalCurrentAmtDue = discPlanAmount;
                        anAccount.PreviousTotalCurrentAmtDue = discPlanAmount;
                        anAccount.OriginalMonthlyPayment = unInsTotalMonthlyPaymentsDue;
                        anAccount.ResourceListProvided = new YesNoFlag();
                        anAccount.NumberOfMonthlyPayments =
                        anAccount.OriginalNumberOfMonthlyPayments =
                                                                    unInsNumberOfMonthlyPayments;
                        switch ( resourceListProvided.ToUpper() )
                        {
                            case "Y":
                                anAccount.ResourceListProvided.SetYes();
                                break;
                            case "N":
                                anAccount.ResourceListProvided.SetNo();
                                break;
                            case " ":
                                anAccount.ResourceListProvided.SetBlank( string.Empty );
                                break;
                        }
                    }
                    else
                    {
                        anAccount.BalanceDue = totalCurrentAmtDue;
                        anAccount.TotalCurrentAmtDue = totalCurrentAmtDue;
                        anAccount.PreviousTotalCurrentAmtDue = totalCurrentAmtDue;
                        anAccount.TotalPaid = insuredTotalPaid;
                        anAccount.OriginalMonthlyPayment = insuredTotalMonthlyPaymentDue;
                        anAccount.NumberOfMonthlyPayments =
                        anAccount.OriginalNumberOfMonthlyPayments =
                                                                    insNumberOfMonthlyPayments;
                    }
                    anAccount.DayOfMonthPaymentDue = dayOfMonthPaymentDue.Trim();
                }
            }
            catch ( Exception ex )
            {
                const string msg = "FinancialCouncelingDataFrom method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, Log );
            }
        }

        #region Physician Info
        /// <summary>
        /// read admitting non staff physician from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Physician AdmittingNonStaffPhysicianFrom( SafeReader reader )
        {
            Physician admittingPhysician = null;
            if ( reader != null )
            {
                string upin = reader.GetString( COL_ADMITTINGNSPUPIN ).Trim();

                string physicianNumber = reader.GetString( COL_ADMITTINGNSPNUM ).Trim();
                long areaCode = reader.GetInt64( COL_ADMITTINGNSPAREACODE );
                long phoneNumber = reader.GetInt64( COL_ADMITTINGNSPPHONENUM );
                string lastName = reader.GetString( COL_ADMITTINGNSPLASTNAME ).Trim();
                string firstName = reader.GetString( COL_ADMITTINGNSPFIRSTNAME ).Trim();
                string midInitial = reader.GetString( COL_ADMITTINGNSPMIDINITIAL ).Trim();
                string nationalProvId = reader.GetString( COL_ADMITTINGNSPNATLPROVID ).Trim();

                string stateLicenseNum = reader.GetString( COL_ADMITTINGNSPSTATELICNO ).Trim();

                PhoneNumber physicianPhoneNumber = new PhoneNumber
                    ( areaCode.ToString().PadLeft( 3, '0' ), phoneNumber.ToString().PadLeft( 7, '0' ) );

                admittingPhysician = new Physician();
                admittingPhysician.UPIN = upin;
                if ( physicianNumber != String.Empty )
                {
                    admittingPhysician.PhysicianNumber = Convert.ToInt64( physicianNumber );
                }
                else
                {
                    admittingPhysician.PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER;
                }
                admittingPhysician.PhoneNumber = physicianPhoneNumber;
                admittingPhysician.LastName = lastName;
                admittingPhysician.FirstName = firstName;
                admittingPhysician.Name.LastName = lastName;
                admittingPhysician.Name.FirstName = firstName;
                admittingPhysician.Name.MiddleInitial = midInitial;
                admittingPhysician.Name = new Name( firstName, lastName, midInitial, String.Empty );
                admittingPhysician.NPI = nationalProvId;
                admittingPhysician.StateLicense = stateLicenseNum;
            }

            return admittingPhysician;
        }

        /// <summary>
        /// read referring non staff physician from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Physician ReferringNonStaffPhysicianFrom( SafeReader reader )
        {
            Physician referringPhysician = null;
            if ( reader != null )
            {
                string upin = reader.GetString( COL_REFERRINGNSPUPIN ).Trim();

                string fullPhoneNumber = reader.GetString( COL_REFERRINGNSPPHONENUM );
                string lastName = reader.GetString( COL_REFERRINGNSPLASTNAME ).Trim();
                string firstName = reader.GetString( COL_REFERRINGNSPFIRSTNAME ).Trim();
                string midInitial = reader.GetString( COL_REFERRINGNSPMIDINITIAL ).Trim();
                string nationalProvId = reader.GetString( COL_REFERRINGNSPNATLPROVID ).Trim();

                string stateLicenseNum = reader.GetString( COL_REFFERRINGNSPSTATELICNO ).Trim();

                string areaCode = "0";
                string phoneNumber = "0";
                if ( fullPhoneNumber.Trim() != String.Empty )
                {
                    SplitPhoneNumber( fullPhoneNumber, ref areaCode, ref phoneNumber );
                }
                PhoneNumber physicianPhoneNumber = new PhoneNumber
                    ( areaCode.PadLeft( 3, '0' ), phoneNumber.PadLeft( 7, '0' ) );

                referringPhysician = new Physician();
                referringPhysician.UPIN = upin;
                referringPhysician.PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER;
                referringPhysician.PhoneNumber = physicianPhoneNumber;
                referringPhysician.LastName = lastName;
                referringPhysician.FirstName = firstName;
                referringPhysician.Name.LastName = lastName;
                referringPhysician.Name.FirstName = firstName;
                referringPhysician.Name.MiddleInitial = midInitial;
                referringPhysician.Name = new Name( firstName, lastName, midInitial, String.Empty );
                referringPhysician.NPI = nationalProvId;
                referringPhysician.StateLicense = stateLicenseNum;
            }

            return referringPhysician;
        }

        /// <summary>
        /// read attending non staff physician from  reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Physician AttendingNonStaffPhysicianFrom( SafeReader reader )
        {
            Physician attendingPhysician = null;
            if ( reader != null )
            {
                string upin = reader.GetString( COL_ATTENDINGNSPUPIN ).Trim();
                string fullPhoneNumber = reader.GetString( COL_ATTENDINGNSPPHONENUM );
                string lastName = reader.GetString( COL_ATTENDINGNSPLASTNAME ).Trim();
                string firstName = reader.GetString( COL_ATTENDINGNSPFIRSTNAME ).Trim();
                string midInitial = reader.GetString( COL_ATTENDINGNSPMIDINITIAL ).Trim();
                string nationalProvId = reader.GetString( COL_ATTENDINGNSPNATLPROVID ).Trim();

                string stateLicenseNum = reader.GetString( COL_ATTENDINGSTATELICNO ).Trim();

                string areaCode = "0";
                string phoneNumber = "0";
                if ( fullPhoneNumber.Trim() != String.Empty )
                {
                    SplitPhoneNumber( fullPhoneNumber, ref areaCode, ref phoneNumber );
                }
                PhoneNumber physicianPhoneNumber = new PhoneNumber
                    ( areaCode.PadLeft( 3, '0' ), phoneNumber.PadLeft( 7, '0' ) );

                attendingPhysician = new Physician();
                attendingPhysician.UPIN = upin;
                attendingPhysician.PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER;
                attendingPhysician.PhoneNumber = physicianPhoneNumber;
                attendingPhysician.LastName = lastName;
                attendingPhysician.FirstName = firstName;
                attendingPhysician.Name.LastName = lastName;
                attendingPhysician.Name.FirstName = firstName;
                attendingPhysician.Name.MiddleInitial = midInitial;
                attendingPhysician.Name = new Name( firstName, lastName, midInitial, String.Empty );
                attendingPhysician.NPI = nationalProvId;
                attendingPhysician.StateLicense = stateLicenseNum;
            }

            return attendingPhysician;
        }

        /// <summary>
        ///  read operation non staff physician from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Physician OperationNonStaffPhysicianFrom( SafeReader reader )
        {
            Physician operatingPhysician = null;
            if ( reader != null )
            {
                string upin = reader.GetString( COL_OPRERATINGNSPUPIN ).Trim();
                string fullPhoneNumber = reader.GetString( COL_OPRERATINGNSPPHONENUM );
                string lastName = reader.GetString( COL_OPRERATINGNSPLASTNAME ).Trim();
                string firstName = reader.GetString( COL_OPRERATINGNSPFIRSTNAME ).Trim();
                string midInitial = reader.GetString( COL_OPRERATINGNSPMIDINITIAL ).Trim();
                string nationalProvId = reader.GetString( COL_OPRERATINGNSPNATLPROVID ).Trim();

                string stateLicenseNum = reader.GetString( COL_OPRERATINGNSPSTATELICNO ).Trim();

                string areaCode = "0";
                string phoneNumber = "0";
                if ( fullPhoneNumber.Trim() != String.Empty )
                {
                    SplitPhoneNumber( fullPhoneNumber, ref areaCode, ref phoneNumber );
                }
                PhoneNumber physicianPhoneNumber = new PhoneNumber
                    ( areaCode.PadLeft( 3, '0' ), phoneNumber.PadLeft( 7, '0' ) );

                operatingPhysician = new Physician();
                operatingPhysician.UPIN = upin;
                operatingPhysician.PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER;
                operatingPhysician.PhoneNumber = physicianPhoneNumber;
                operatingPhysician.LastName = lastName;
                operatingPhysician.FirstName = firstName;
                operatingPhysician.Name.LastName = lastName;
                operatingPhysician.Name.FirstName = firstName;
                operatingPhysician.Name.MiddleInitial = midInitial;
                operatingPhysician.Name = new Name( firstName, lastName, midInitial, String.Empty );
                operatingPhysician.NPI = nationalProvId;
                operatingPhysician.StateLicense = stateLicenseNum;
            }

            return operatingPhysician;
        }

        /// <summary>
        /// read other non staff physician from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Physician PrimaryCareNonStaffPhysicianFrom( SafeReader reader )
        {
            Physician primaryCarePhysician = null;
            if ( reader != null )
            {
                string upin = reader.GetString( COL_PCPNSPUPIN ).Trim();
                string fullPhoneNumber = reader.GetString( COL_PCPNSPPHONENUM );
                string lastName = reader.GetString( COL_PCPNSPLASTNAME ).Trim();
                string firstName = reader.GetString( COL_PCPNSPFIRSTNAME ).Trim();
                string midInitial = reader.GetString( COL_PCPNSPMIDINITIAL ).Trim();
                string nationalProvId = reader.GetString( COL_PCPNSPNATLPROVID ).Trim();

                string stateLicenseNum = reader.GetString( COL_PCPNSPSTATELICNO ).Trim();

                string areaCode = "0";
                string phoneNumber = "0";
                if ( fullPhoneNumber.Trim() != String.Empty )
                {
                    SplitPhoneNumber( fullPhoneNumber, ref areaCode, ref phoneNumber );
                }
                PhoneNumber physicianPhoneNumber = new PhoneNumber
                    ( areaCode.PadLeft( 3, '0' ), phoneNumber.PadLeft( 7, '0' ) );

                primaryCarePhysician = new Physician();
                primaryCarePhysician.UPIN = upin;
                primaryCarePhysician.PhysicianNumber = Physician.NON_STAFF_PHYSICIAN_NUMBER;
                primaryCarePhysician.PhoneNumber = physicianPhoneNumber;
                primaryCarePhysician.LastName = lastName;
                primaryCarePhysician.FirstName = firstName;
                primaryCarePhysician.Name.LastName = lastName;
                primaryCarePhysician.Name.FirstName = firstName;
                primaryCarePhysician.Name.MiddleInitial = midInitial;
                primaryCarePhysician.Name = new Name( firstName, lastName, midInitial, String.Empty );
                primaryCarePhysician.NPI = nationalProvId;
                primaryCarePhysician.StateLicense = stateLicenseNum;
            }

            return primaryCarePhysician;
        }

        #endregion

        private static void SplitPhoneNumber( string fullPhoneNumber, ref string areaCode, ref string phoneNumber )
        {
            if ( fullPhoneNumber.Trim().Length < LENGTH_OF_PHONE_NUMBER )
            {
                phoneNumber = fullPhoneNumber;
            }
            else
            {
                phoneNumber = fullPhoneNumber.Substring(
                    fullPhoneNumber.Length - LENGTH_OF_PHONE_NUMBER, LENGTH_OF_PHONE_NUMBER );
                areaCode = fullPhoneNumber.Substring(
                    0, fullPhoneNumber.Length - LENGTH_OF_PHONE_NUMBER );
            }
        }

        private static Accomodation AccomodationFrom( SafeReader reader )
        {
            string accCode = reader.GetString( COL_ACCOMODATION_CODE ).Trim();
            string accDescription = reader.GetString( COL_ACCOMODATION_DESC ).Trim();

            Accomodation anAccomodation = new Accomodation(
                PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION,
                accDescription,
                accCode
                );

            return anAccomodation;
        }

        private bool ValidInsuranceFor( Account account, Insurance insurance )
        {
            bool coverage1IsValid = false;
            bool coverage2IsValid = false;

            Coverage coverage1 = insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
            if ( coverage1 != null )
            {
                InsurancePlan insPlan1 = insuranceBroker.PlanWith( coverage1.InsurancePlan.PlanID,
                                         account.Facility.Oid, account.AdmitDate );
                if ( insPlan1 == null )
                {   //coverage not valid for this account
                    insurance.RemoveCoverage( coverage1 );
                }
                else
                    coverage1IsValid = true;
            }

            Coverage coverage2 = insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
            if ( coverage2 != null )
            {
                InsurancePlan insPlan2 = insuranceBroker.PlanWith( coverage2.InsurancePlan.PlanID,
                                         account.Facility.Oid, account.AdmitDate );
                if ( insPlan2 == null )
                {   //coverage not valid for this account
                    insurance.RemoveCoverage( coverage2 );
                }
                else
                    coverage2IsValid = true;
            }

            return ( coverage1IsValid || coverage2IsValid );
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private iDB2Command CreateCmdForAccountsQuery( Patient aPatient )
        {
            iDB2Command cmd;

            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_PATIENT_ACCOUNTS_FOR +
                    "(" + PARAM_HSPNUMBER +
                    "," + PARAM_MRC +
                    "," + PARAM_PATIENTTYPE_PREREG +
                    "," + PARAM_PATIENTTYPE_INPAT +
                    "," + PARAM_PATIENTTYPE_OUTPATIENT +
                    "," + PARAM_PATIENTTYPE_PREMSE +
                    "," + PARAM_PATIENTTYPE_POSTMSE +
                    "," + PARAM_PATIENTTYPE_RECUR +
                    "," + PARAM_PATIENTTYPE_NONPAT + ")",
                    CommandType.Text,
                    aPatient.Facility );

                cmd.Parameters[PARAM_HSPNUMBER].Value = aPatient.Facility.Oid;
                cmd.Parameters[PARAM_MRC].Value = aPatient.MedicalRecordNumber;
                cmd.Parameters[PARAM_PATIENTTYPE_PREREG].Value = DBNull.Value;
                cmd.Parameters[PARAM_PATIENTTYPE_INPAT].Value = DBNull.Value;
                cmd.Parameters[PARAM_PATIENTTYPE_OUTPATIENT].Value = DBNull.Value;
                cmd.Parameters[PARAM_PATIENTTYPE_PREMSE].Value = DBNull.Value;
                cmd.Parameters[PARAM_PATIENTTYPE_POSTMSE].Value = DBNull.Value;
                cmd.Parameters[PARAM_PATIENTTYPE_RECUR].Value = DBNull.Value;
                cmd.Parameters[PARAM_PATIENTTYPE_NONPAT].Value = DBNull.Value;

            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Log );
            }

            return cmd;
        }

        private ArrayList ReadAccounts( IDbCommand cmd, Patient aPatient )
        {
            SafeReader reader = null;
            ArrayList accounts = new ArrayList();

            try
            {
                reader = ExecuteReader( cmd );

                while ( reader.Read() )
                {
                    AccountProxy ap = AccountProxyFrom( reader, aPatient, aPatient.Facility );
                    accounts.Add( ap );
                }
            }
            finally
            {
                Close( reader );
            }

            return accounts;
        }

        private AccountProxy AccountProxyFrom( SafeReader reader,
            Patient patient, Facility facility )
        {
            const bool isLocked = false;

            long accountNumber = reader.GetInt64( COL_ACCOUNTNUMBER );
            DateTime admitDate = reader.GetDateTime( COL_ADMISSION_DATE );
            DateTime dischargeDate = reader.GetDateTime( COL_DISCHARGEDATE );
            string serviceCode = reader.GetString( COL_SERVICECODE ).Trim();
            string financialCode = reader.GetString( COL_FINANCIALCODE ).Trim();
            string hospitalClinicCode = reader.GetString( COL_CLINICCODE ).Trim();
            string patientTypeCode = reader.GetString( COL_PATIENTTYPE ).Trim();

            string opStatusCode = reader.GetString( COL_OPSTATUSCODE );

            string ipStatusCode = reader.GetString( COL_IPSTATUSCODE ).Trim();
            string finalBillingFlag = reader.GetString( COL_FINALBILLINGFLAG );
            long opVisitNumber = reader.GetInt64( COL_OPVISITNUMBER );
            string pendingPurge = reader.GetString( COL_PENDINGPURGE );
            long unbilledBalance = reader.GetInt64( COL_UNBILLEDBALANCE );
            string medicalServiceCode = reader.GetString( COL_MEDICALSERVICECODE ).Trim();

            string pendingDischarge = reader.GetString( COL_PENDINGDISCHARGE ).Trim();
            string valuablesAreTaken = reader.GetString( COL_VALUABLESARETAKEN ).Trim();
            string abstractExists = reader.GetString( COL_ABSTRACTEXISTS ).Trim();
            string dischargeCode = reader.GetString( COL_DISCHARGECODE ).Trim();
            string nursingStation = reader.GetString( COL_NURSINGSTATION );
            string room = reader.GetValue( COL_ROOM ).ToString();
            string bed = reader.GetString( COL_BED ).Trim();
            long lastMaintenanceDate = reader.GetInt64( COL_LPLMD );
            long lastMaintLogNumber = reader.GetInt64( COL_LPLML );
            long updateLogNumber = reader.GetInt64( COL_LPLUL );

            FinancialClass financialClass = fcBroker.FinancialClassWith( facility.Oid, financialCode );

            HospitalClinic hospitalClinic = hcBroker.HospitalClinicWith( facility.Oid, hospitalClinicCode );

            HospitalService svc = serviceCode != null ? hsvBroker.HospitalServiceWith(
                facility.Oid, serviceCode.Trim() ) : null;

            VisitType visitType = patientBroker.PatientTypeWith( facility.Oid, patientTypeCode.Trim() );

            string derivedVisitType = CreateKindOfVisit(
                opStatusCode, ipStatusCode, finalBillingFlag, opVisitNumber, pendingPurge,
                unbilledBalance, medicalServiceCode );

            AccountProxy ap = new AccountProxy( accountNumber,
                patient, admitDate, dischargeDate, visitType,
                facility, financialClass, svc, derivedVisitType,
                isLocked, null, hospitalClinic );

            ap.AccountLock = AccountLockFrom( reader );

            SetShortRegisteredAccountFlag( reader, ap );
            SetNewbornRegisteredAccountFlag(reader, ap);
            ap.SetUCCVisitType();
            ap.PendingDischarge = pendingDischarge;
            if ( pendingDischarge.Equals( "Y" ) )
            {
                ap.DischargeStatus = DischargeStatus.PendingDischarge();
            }
            if ( valuablesAreTaken == YesNoFlag.CODE_YES )
            {
                ap.ValuablesAreTaken = new YesNoFlag( YesNoFlag.CODE_YES );
            }
            else if ( valuablesAreTaken == YesNoFlag.CODE_NO )
            {
                ap.ValuablesAreTaken = new YesNoFlag( YesNoFlag.CODE_NO );
            }
            else
            {
                ap.ValuablesAreTaken = new YesNoFlag();
            }

            ap.AbstractExists = abstractExists == "C" ? true : false;

            if ( ipStatusCode.Equals( ACCOUNT_CANCELED_INDICATOR ) )
            {
                ap.IsCanceled = true;
            }

            ap.Location = new Location( nursingStation, room, bed );

            if ( ap.Location.Bed != null )
            {
                ap.Location.Bed.Accomodation = AccomodationFrom( reader );
            }

            ap.DischargeDisposition = dischargeBroker.DischargeDispositionWith( facility.Oid, dischargeCode );

            ap.LastMaintenanceDate = lastMaintenanceDate;
            ap.LastMaintenanceLogNumber = lastMaintLogNumber;
            ap.UpdateLogNumber = updateLogNumber;

            return ap;
        }

        private static void SetShortRegisteredAccountFlag( SafeReader reader, AccountProxy accountProxy )
        {
            string isShortRegistered = reader.GetString( COL_SHORT_REG_TYPE ).Trim();
            accountProxy.IsShortRegistered = ( isShortRegistered == YesNoFlag.CODE_YES );
            accountProxy.IsQuickRegistered = ( isShortRegistered == QUICK_FLAG );
            accountProxy.IsPAIWalkinRegistered = (isShortRegistered == PAI_WALKIN_FLAG);
            accountProxy.SetUCCVisitType();
        }

        private static void SetNewbornRegisteredAccountFlag( SafeReader reader, AccountProxy accountProxy )
        {
            string isNewbornRegistered = reader.GetString( COL_NEWBORN_REG_TYPE ).Trim();
            accountProxy.IsNewBorn = ( isNewbornRegistered == YesNoFlag.CODE_YES );
        }

        private static void SetBirthTimeIsEnteredFlag( SafeReader reader, AccountProxy accountProxy )
        {
            //Todo: get the flag from database
            string birthTimeEntered = reader.GetString( COL_BIRTHTIMEENTERED ).Trim();
            accountProxy.BirthTimeIsEntered = ( birthTimeEntered == YesNoFlag.CODE_YES );
        }

        private static void SetIsolationCode(SafeReader reader, AccountProxy accountProxy)
        {
            accountProxy.IsolationCode = reader.GetString(COL_ISOLATIONCODE).Trim();
        }


        private void BuildAdmitSources( SafeReader reader, AccountProxy accountProxy )
        {
            string admitSourceCode = reader.GetString( COL_ADMIT_SOURCECODE ).TrimEnd();
            accountProxy.AdmitSource = asBroker.AdmitSourceWith(
                    accountProxy.Facility.Oid, admitSourceCode );
        }
        private void BuildAdmittingCategory(SafeReader reader, AccountProxy accountProxy)
        {
            accountProxy.AdmittingCategory = reader.GetString( COL_ADMITTING_CATEGORY ).Trim();
        }

        private static void SetPrimaryPlanID( SafeReader reader, AccountProxy accountProxy )
        {
            accountProxy.PrimaryInsurancePlan = reader.GetString( COL_PRIMARY_PLANID ).Trim();
        }

       
        /// <summary>
        /// read the details of the account from pbar
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="AccountNotFoundException"><c>AccountNotFoundException</c>.</exception>
        /// <exception cref="Exception">Unhandled Exception</exception>
        private void ReadDetailAccountData( Account account, SafeReader reader )
        {
            try
            {
                string occurrenceCode;

                string conditionCode;
                bool hasAccidentCrimeOccurrenceCode = false;

                if ( reader.Read() )
                {
                    for ( int x = 1; x <= OCCURRENCECOUNT; x++ )
                    {
                        string occurrenceCodeNum = COL_OCCURRENCECODE + x;
                        string occurrenceDateNum = COL_OCCURRENCEDATE + x;
                        if ( !reader.IsDBNull( occurrenceCodeNum ) )
                        {
                            occurrenceCode = reader.GetString( occurrenceCodeNum );
                            long occurrenceDate = reader.GetInt64( occurrenceDateNum );
                            if (occurrenceCode.Trim().Length > 0)
                            {
                                var occCode = ocBroker.CreateOccurrenceCode(account.Facility.Oid , occurrenceCode, occurrenceDate);
                                account.AddOccurrenceCode(occCode);
                                if (occCode.IsAccidentCrimeOccurrenceCode())
                                {
                                    hasAccidentCrimeOccurrenceCode = true;
                                }
                            }

                        }
                    }

                    if ( !hasAccidentCrimeOccurrenceCode )
                    {
                        account.AddOccurrenceCode( new OccurrenceCode() );
                    }

                    var occurrenceCodeComparerByCodeAndDate = new OccurrenceCodeComparerByCodeAndDate();

                    ( ( ArrayList )account.OccurrenceCodes ).Sort( occurrenceCodeComparerByCodeAndDate );

                    IConditionCodeBroker ccBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();

                    for ( int x = 1; x <= CONDITIONCOUNT; x++ )
                    {
                        string conditionCodeNum = COL_CONDITIONCODE + x;
                        if ( !reader.IsDBNull( conditionCodeNum ) )
                        {
                            conditionCode = reader.GetString( conditionCodeNum );
                            if ( conditionCode.Trim().Length > 0 )
                            {
                                ConditionCode cc = ccBroker.ConditionCodeWith( account.Facility.Oid, conditionCode );

                                account.AddConditionCode( cc );
                            }
                        }
                    }

                    SortConditionsByCode sortCondCodes = new SortConditionsByCode();
                    ( ( ArrayList )account.ConditionCodes ).Sort( sortCondCodes );

                    string clergyVisit = reader.GetString( COL_CLERGYVISIT );
                    switch ( clergyVisit.ToUpper() )
                    {
                        case "Y":
                            account.ClergyVisit.SetNo();
                            break;
                        case "N":
                            account.ClergyVisit.SetYes();
                            break;
                        default:
                            account.ClergyVisit.SetBlank( string.Empty );
                            break;
                    }

                    account.AlternateCareFacility = reader.GetString( COL_ALTERNATECAREFACILITYCODE ).Trim();
                    account.Diagnosis.ChiefComplaint = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                        reader.GetString( COL_DIAGNOSIS ) );
                    account.Diagnosis.Procedure = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                        reader.GetString( COL_PROCEDURE ) );
                    if ( account.Diagnosis.Procedure == null )
                    {
                        account.Diagnosis.Procedure = String.Empty;
                    }

                    /* Column PATIENTPORTALOPTIN is a 5 byte field named LPFILL in HPADLPP PBAR table *
                     * where the first byte represents PatientPortalOptIn value and 
                     * the second byte represents RightToRestrict value*/
                    string portalOptInAndRightToRestrict = reader.GetString( COL_PORTALOPTIN_RIGHTTORESTRICT );
                    SetPortalOptInAndRightToRestrictValue( account, portalOptInAndRightToRestrict );

                    //Column HOSPITALCOMMUNICATIONOPTIN is a 1 byte field named  MDMDFL  in HPADMDP PBAR table 
                    // where the first byte represents HOSPITALCOMMUNICATIONOPTIN value and 
                    // the second byte represents EMAILREASON value
                    string hospCommOptInAndEmailReason = reader.GetString(COL_HOSPCOMMOPTIN);
                    SetHospitalComminicationOptInValue(account, hospCommOptInAndEmailReason);

                    /* Column IMFMRECEIVED is 1st byte in the field LPFILL2 in HPADLPP PBAR table */
                    string IMFMReceived = reader.GetString(COL_LPFILL);
                    SetIMFMReceivedFlagValue(account, IMFMReceived);

                    // now that we have the basic information derive the
                    // condition type
                    DeriveVisitType( account, reader );

                    string opstatusCode = reader.GetString( COL_OPSTATUSCODE );

                    string ipStatusCode = reader.GetString( COL_IPSTATUSCODE ).Trim();
                    string finalBillingFlag = reader.GetString( COL_FINALBILLINGFLAG );
                    long opvisitNumber = reader.GetInt64( COL_OPVISITNUMBER );
                    string pendingPurge = reader.GetString( COL_PENDINGPURGE );
                    long unbilledBalance = reader.GetInt64( COL_UNBILLEDBALANCE );
                    decimal totalCharges = reader.GetDecimal( COL_TOTALCHARGES );

                    string valueCode1 = reader.GetString( COL_VALUECODE1 ).Trim();
                    string valueCode2 = reader.GetString( COL_VALUECODE2 ).Trim();
                    string valueCode3 = reader.GetString( COL_VALUECODE3 ).Trim();
                    string valueCode4 = reader.GetString( COL_VALUECODE4 ).Trim();
                    string valueCode5 = reader.GetString( COL_VALUECODE5 ).Trim();
                    string valueCode6 = reader.GetString( COL_VALUECODE6 ).Trim();
                    string valueCode7 = reader.GetString( COL_VALUECODE7 ).Trim();
                    string valueCode8 = reader.GetString( COL_VALUECODE8 ).Trim();

                    decimal valueAmount1 = reader.GetDecimal( COL_VALUEAMOUNT1 );
                    decimal valueAmount2 = reader.GetDecimal( COL_VALUEAMOUNT2 );
                    decimal valueAmount3 = reader.GetDecimal( COL_VALUEAMOUNT3 );
                    decimal valueAmount4 = reader.GetDecimal( COL_VALUEAMOUNT4 );
                    decimal valueAmount5 = reader.GetDecimal( COL_VALUEAMOUNT5 );
                    decimal valueAmount6 = reader.GetDecimal( COL_VALUEAMOUNT6 );
                    decimal valueAmount7 = reader.GetDecimal( COL_VALUEAMOUNT7 );
                    decimal valueAmount8 = reader.GetDecimal( COL_VALUEAMOUNT8 );

                    decimal mothersPatientAccount = reader.GetDecimal( COL_MOTHERSACCOUNTNUMBER );

                    string medicalServiceCode = reader.GetString( COL_MEDICALSERVICECODE ).Trim();
                    string smoker = reader.GetString( COL_SMOKER );
                    string bloodless = reader.GetString( COL_BLOODLESS );
                    string pregnant = reader.GetString( COL_ISPREGNANT );
                    string resistantOrganism = reader.GetString(COL_RESISTANTORGANISM).Trim();
                    string rightCareRightPlace = reader.GetString( COL_RCRP ).Trim();
                    string leftOrStayed = reader.GetString( COL_LEFTORSTAYED ).Trim();
                    string leftWithoutBeingSeen = reader.GetString( COL_LEFTWOSEEN ).Trim();
                    string leftWithoutFinancialClearance = reader.GetString( COL_LEFTWOFINCLR ).Trim();

                    string isPatientInClinicalResearchStudy = reader.GetString( COL_PATIENTINCLINICALSTUDY ).Trim();
                    account.IsPatientInClinicalResearchStudy = YesNoFlag.GetYesNoFlagFor( isPatientInClinicalResearchStudy );

                    string isShortRegistered = reader.GetString( COL_SHORT_REG_TYPE ).Trim();
                    if ( account.Activity != null &&
                         ( account.Activity.GetType().Equals( typeof( ShortRegistrationActivity ) ) ||
                           account.Activity.GetType().Equals( typeof( ShortPreRegistrationActivity ) ) ||
                           account.Activity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) ) )
                    {
                        account.IsShortRegistered = ( isShortRegistered == YesNoFlag.CODE_YES );
                    }

                    string researchCode01 = reader.GetString( COL_RESEARCHID01 ).Trim();
                    string researchCode02 = reader.GetString( COL_RESEARCHID02 ).Trim();
                    string researchCode03 = reader.GetString( COL_RESEARCHID03 ).Trim();
                    string researchCode04 = reader.GetString( COL_RESEARCHID04 ).Trim();
                    string researchCode05 = reader.GetString( COL_RESEARCHID05 ).Trim();
                    string researchCode06 = reader.GetString( COL_RESEARCHID06 ).Trim();
                    string researchCode07 = reader.GetString( COL_RESEARCHID07 ).Trim();
                    string researchCode08 = reader.GetString( COL_RESEARCHID08 ).Trim();
                    string researchCode09 = reader.GetString( COL_RESEARCHID09 ).Trim();
                    string researchCode10 = reader.GetString( COL_RESEARCHID10 ).Trim();

                    string researchConsent01 = reader.GetString( COL_RESEARCHCONSENTFLAG01 ).Trim();
                    string researchConsent02 = reader.GetString( COL_RESEARCHCONSENTFLAG02 ).Trim();
                    string researchConsent03 = reader.GetString( COL_RESEARCHCONSENTFLAG03 ).Trim();
                    string researchConsent04 = reader.GetString( COL_RESEARCHCONSENTFLAG04 ).Trim();
                    string researchConsent05 = reader.GetString( COL_RESEARCHCONSENTFLAG05 ).Trim();
                    string researchConsent06 = reader.GetString( COL_RESEARCHCONSENTFLAG06 ).Trim();
                    string researchConsent07 = reader.GetString( COL_RESEARCHCONSENTFLAG07 ).Trim();
                    string researchConsent08 = reader.GetString( COL_RESEARCHCONSENTFLAG08 ).Trim();
                    string researchConsent09 = reader.GetString( COL_RESEARCHCONSENTFLAG09 ).Trim();
                    string researchConsent10 = reader.GetString( COL_RESEARCHCONSENTFLAG10 ).Trim();

                    if ( researchCode01 != String.Empty )
                    {
                        ResearchStudy study1 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode01 );
                        if ( study1 != null )
                        {
                            ConsentedResearchStudy researchStudy1 =
                                new ConsentedResearchStudy( study1, YesNoFlag.GetYesNoFlagFor( researchConsent01 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy1 );
                            }
                        }
                    }
                    if ( researchCode02 != String.Empty )
                    {
                        ResearchStudy study2 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode02 );
                        if ( study2 != null )
                        {
                            ConsentedResearchStudy researchStudy2 =
                                new ConsentedResearchStudy( study2, YesNoFlag.GetYesNoFlagFor( researchConsent02 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy2 );
                            }
                        }
                    }
                    if ( researchCode03 != String.Empty )
                    {
                        ResearchStudy study3 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode03 );
                        if ( study3 != null )
                        {
                            ConsentedResearchStudy researchStudy3 =
                                new ConsentedResearchStudy( study3, YesNoFlag.GetYesNoFlagFor( researchConsent03 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy3 );
                            }
                        }
                    }
                    if ( researchCode04 != String.Empty )
                    {
                        ResearchStudy study4 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode04 );
                        if ( study4 != null )
                        {
                            ConsentedResearchStudy researchStudy4 =
                                new ConsentedResearchStudy( study4, YesNoFlag.GetYesNoFlagFor( researchConsent04 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy4 );
                            }
                        }
                    }
                    if ( researchCode05 != String.Empty )
                    {
                        ResearchStudy study5 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode05 );
                        if ( study5 != null )
                        {
                            ConsentedResearchStudy researchStudy5 =
                                new ConsentedResearchStudy( study5, YesNoFlag.GetYesNoFlagFor( researchConsent05 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy5 );
                            }
                        }
                    }
                    if ( researchCode06 != String.Empty )
                    {
                        ResearchStudy study6 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode06 );
                        if ( study6 != null )
                        {
                            ConsentedResearchStudy researchStudy6 =
                                new ConsentedResearchStudy( study6, YesNoFlag.GetYesNoFlagFor( researchConsent06 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy6 );
                            }
                        }
                    }
                    if ( researchCode07 != String.Empty )
                    {
                        ResearchStudy study7 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode07 );
                        if ( study7 != null )
                        {
                            ConsentedResearchStudy researchStudy7 =
                                new ConsentedResearchStudy( study7, YesNoFlag.GetYesNoFlagFor( researchConsent07 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy7 );
                            }
                        }
                    }
                    if ( researchCode08 != String.Empty )
                    {
                        ResearchStudy study8 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode08 );
                        if ( study8 != null )
                        {
                            ConsentedResearchStudy researchStudy8 =
                                new ConsentedResearchStudy( study8, YesNoFlag.GetYesNoFlagFor( researchConsent08 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy8 );
                            }
                        }
                    }
                    if ( researchCode09 != String.Empty )
                    {
                        ResearchStudy study9 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode09 );
                        if ( study9 != null )
                        {
                            ConsentedResearchStudy researchStudy9 =
                                new ConsentedResearchStudy( study9, YesNoFlag.GetYesNoFlagFor( researchConsent09 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy9 );
                            }
                        }
                    }
                    if ( researchCode10 != String.Empty )
                    {
                        ResearchStudy study10 = researchStudyBroker.ResearchStudyWith( account.Facility.Oid, researchCode10 );
                        if ( study10 != null )
                        {
                            ConsentedResearchStudy researchStudy10 =
                                new ConsentedResearchStudy( study10, YesNoFlag.GetYesNoFlagFor( researchConsent10 ) );
                            if ( account.CanAddConsentedResearchStudy() )
                            {
                                account.AddConsentedResearchStudy( researchStudy10 );
                            }
                        }
                    }

                    string clinicCommentLine1 = StringFilter.
                        RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod(
                        reader.GetString( COL_COMMENTLINE1 ) );

                    // Embosser Card info is padded with spaces in front to a specified length 
                    // and stored in PBAR. When retrieving back, these spaces need to be retained
                    // and be available for splitting and formatting the clinicCommentLine1 and 
                    // clinicCommentLine2 into Clinical Comments and Embosser Card values using
                    // appropriate lengths. (this code is available in this same method below)
                    string clinicCommentLine2 = StringFilter.
                        RemoveFirstCharNonLetterNumberSpaceAndRestNonLetterNumberSpaceHyphenAndPeriod(
                        reader.GetString( COL_COMMENTLINE2 ) );

                    long lastMaintenanceDate = reader.GetInt64( COL_LPLMD );
                    long lastMaintLogNumber = reader.GetInt64( COL_LPLML );
                    long updateLogNumber = reader.GetInt64( COL_LPLUL );
                    long motherDateOfBirth = reader.GetInt64( COL_MDMDOB );
                    long fatherDateOfBirth = reader.GetInt64( COL_MDFDOB );
                    string passportNumber = reader.GetString( COL_PASSPORTNUMBER );
                    string passportCountry = reader.GetString( COL_PASSPORTCOUNTRY );

                    account.LastMaintenanceDate = lastMaintenanceDate;
                    account.LastMaintenanceLogNumber = lastMaintLogNumber;
                    account.UpdateLogNumber = updateLogNumber;

                    account.Patient.MothersDateOfBirth = DateTimeUtilities.DateTimeFromString( motherDateOfBirth.ToString() );
                    account.Patient.FathersDateOfBirth = DateTimeUtilities.DateTimeFromString( fatherDateOfBirth.ToString() );

                    long mothersAccountNumber = ConvertToLongIfInRangeOfLongElseZero( mothersPatientAccount );

                    account.Patient.MothersAccount = new Account( mothersAccountNumber );

                    if ( passportNumber != String.Empty )
                    {
                        if ( passportCountry != string.Empty )
                        {
                            account.Patient.Passport = new Passport( passportNumber, addressBroker.CountryWith( account.Facility.Oid, passportCountry ) );
                        }
                        else
                        {
                            account.Patient.Passport = new Passport( passportNumber, null );
                        }
                    }

                    account.TotalCharges = totalCharges;

                    account.ValueCode1 = valueCode1;
                    account.ValueCode2 = valueCode2;
                    account.ValueCode3 = valueCode3;
                    account.ValueCode4 = valueCode4;
                    account.ValueCode5 = valueCode5;
                    account.ValueCode6 = valueCode6;
                    account.ValueCode7 = valueCode7;
                    account.ValueCode8 = valueCode8;

                    account.ValueAmount1 = valueAmount1;
                    account.ValueAmount2 = valueAmount2;
                    account.ValueAmount3 = valueAmount3;
                    account.ValueAmount4 = valueAmount4;
                    account.ValueAmount5 = valueAmount5;
                    account.ValueAmount6 = valueAmount6;
                    account.ValueAmount7 = valueAmount7;
                    account.ValueAmount8 = valueAmount8;

                    account.DerivedVisitType = CreateKindOfVisit(
                        opstatusCode, ipStatusCode, finalBillingFlag, opvisitNumber, pendingPurge,
                        unbilledBalance, medicalServiceCode );

                    int year = ( int )reader.GetInt64( COL_ADMITYEAR );
                    int month = ( int )reader.GetInt64( COL_ADMITMONTH );
                    int day = ( int )reader.GetInt64( COL_ADMITDAY );

                    string admitTime = reader.GetString( COL_TIMEOFADMISSION );

                    admitTime = "0000" + admitTime;
                    admitTime = admitTime.Substring( admitTime.Length - 4, 4 );
                    string hours = admitTime.Substring( 0, 2 );
                    string mins = admitTime.Substring( 2, 2 );
                    int hour = int.Parse( hours );
                    int min = int.Parse( mins );

                    if ( year != 0
                        && month != 0
                        && day != 0 )
                    {
                        account.AdmitDate = new DateTime(
                            DateTimeUtilities.LongYearFromPartYear( year ),
                            month,
                            day,
                            hour,
                            min,
                            0
                            );
                    }

                    account.FinancialClass = fcBroker.FinancialClassWith( account.Facility.Oid,
                        reader.GetString( COL_FINANCIALCLASSCODE ).Trim() );

                    account.HospitalService = hsvBroker.HospitalServiceWith(
                        account.Facility.Oid, reader.GetString( COL_MEDICALSERVICECODE ).Trim() );

                    string patientTypeCode = reader.GetString( COL_PATIENTTYPE ).Trim();
                    account.KindOfVisit = patientBroker.PatientTypeWith( account.Facility.Oid, patientTypeCode );
                    account.SetUCCVisitType();
                    string dischargeCode = reader.GetString( COL_DISCHARGEDISPOSITIONCODE ).Trim();
                    account.DischargeDisposition = dischargeBroker.DischargeDispositionWith( account.Facility.Oid, dischargeCode );

                    string referralSourceCode = reader.GetString( COL_REFERRALSOURCECODE ).Trim();
                    IReferralSourceBroker referralSourceBroker = BrokerFactory.BrokerOfType<IReferralSourceBroker>();
                    account.ReferralSource = referralSourceBroker.ReferralSourceWith( account.Facility.Oid, referralSourceCode );

                    account.Patient.NoticeOfPrivacyPracticeDocument = new NoticeOfPrivacyPracticeDocument();

                    string code = reader.GetString( COL_NPP_SIGNATURE_STATUS );
                    if ( !string.IsNullOrEmpty( code ) )
                    {
                        account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus( code );
                    }
                    else
                    {
                        account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus();
                    }

                    string nppSignedDate = reader.GetString( COL_NPP_DATE_SIGNED );
                    if ( nppSignedDate.Trim() != String.Empty && nppSignedDate.Trim() != DATETIME_MIN_VALUE )
                    {
                        account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate =
                            DateTimeUtilities.DateTimeForYYYYMMDDFormat( nppSignedDate );
                    }
                    else
                    {
                        account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate = new DateTime( 0 );
                    }

                    long lNppVersion = reader.GetInt64( COL_NPPVERSION );
                    string nppVersion = lNppVersion.ToString();
                    account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion =
                        nppBroker.NPPVersionWith( account.Facility.Oid, nppVersion );

                    IConfidentialCodeBroker confCodeBroker = BrokerFactory.BrokerOfType<IConfidentialCodeBroker>();
                    account.ConfidentialityCode = confCodeBroker.ConfidentialCodeWith( account.Facility.Oid, reader.GetString( COL_CONFIDENTIALFLAG ) );

                    string facilityFlag = reader.GetString( COL_FACILITYFLAG );
                    if ( facilityFlag != null && facilityFlag.Trim().Length > 0 )
                    {
                        IFacilityFlagBroker facilityFlagBroker = BrokerFactory.BrokerOfType<IFacilityFlagBroker>();
                        account.FacilityDeterminedFlag = facilityFlagBroker.FacilityFlagWith( account.Facility.Oid, facilityFlag.Trim() );
                    }

                    DeriveOptOutOptions( account, reader.GetString( COL_OPTOUTFLAGS ) );

                    account.Smoker = new YesNoFlag();

                    switch ( smoker.ToUpper() )
                    {
                        case "Y":
                            account.Smoker.SetYes();
                            break;
                        case "N":
                            account.Smoker.SetNo();
                            break;
                        case " ":
                            account.Smoker.SetBlank( string.Empty );
                            break;
                    }

                    account.Bloodless = new YesNoFlag();
                    switch ( bloodless.ToUpper() )
                    {
                        case "Y":
                            account.Bloodless.SetYes();
                            break;
                        case "N":
                            account.Bloodless.SetNo();
                            break;
                        case " ":
                            account.Bloodless.SetBlank( string.Empty );
                            break;
                    }
                    account.Pregnant = YesNoFlag.GetYesNoFlagFor( pregnant );
                    account.ResistantOrganism = resistantOrganism;
                    account.RightCareRightPlace.RCRP = YesNoFlag.GetYesNoFlagFor( rightCareRightPlace );
                    ILeftOrStayedBroker leftOrStayedBroker = BrokerFactory.BrokerOfType<ILeftOrStayedBroker>();
                    account.RightCareRightPlace.LeftOrStayed = leftOrStayedBroker.LeftOrStayedWith( leftOrStayed );
                    if ( !account.RightCareRightPlace.LeftOrStayed.IsValid )
                    {
                        account.RightCareRightPlace.LeftOrStayed = leftOrStayedBroker.LeftOrStayedWith( LeftOrStayed.BLANK );
                    }
                    account.LeftWithOutBeingSeen = YesNoFlag.GetYesNoFlagFor( leftWithoutBeingSeen );
                    account.LeftWithoutFinancialClearance = YesNoFlag.GetYesNoFlagFor( leftWithoutFinancialClearance );

                    clinicCommentLine2 = clinicCommentLine2.PadRight( TOTAL_LENGTH_OF_COMMENTS, ' ' );
                    account.EmbosserCard = clinicCommentLine2.Substring(
                        LENGTH_OF_COMMENTS_TWO, TOTAL_LENGTH_OF_EMBOSER_CARD ).Trim();

                    account.ClinicalComments = clinicCommentLine1.Trim() +
                        clinicCommentLine2.Substring( 0, LENGTH_OF_COMMENTS_TWO ).Trim();

                    string firstSpanCode = reader.GetString( COL_FIRSTSPANCODE ).Trim();
                    string secondSpanCode = reader.GetString( COL_SECONDSPANCODE ).Trim();

                    ISpanCodeBroker scBroker = BrokerFactory.BrokerOfType<ISpanCodeBroker>();
                    SpanCode sc1 = scBroker.SpanCodeWith( account.Facility.Oid, firstSpanCode );
                    SpanCode sc2 = scBroker.SpanCodeWith( account.Facility.Oid, secondSpanCode );

                    if ( sc1 != null && sc1.Code != string.Empty )
                    {
                        OccurrenceSpan os1 = new OccurrenceSpan();
                        os1.SpanCode = sc1;
                        os1.FromDate = DateTimeUtilities.DateTimeFromPacked( reader.GetInt64( COL_FIRSTSPANFROMDATE ) );
                        os1.ToDate = DateTimeUtilities.DateTimeFromPacked( reader.GetInt64( COL_FIRSTSPANTODATE ) );
                        os1.Facility = reader.GetString( COL_FIRSTSPANFACILITY );
                        account.OccurrenceSpans.Add( os1 );
                    }
                    else
                        account.OccurrenceSpans.Add( null );

                    if ( sc2 != null && sc2.Code != string.Empty )
                    {
                        OccurrenceSpan os2 = new OccurrenceSpan();
                        os2.SpanCode = sc2;
                        os2.FromDate = DateTimeUtilities.DateTimeFromPacked( reader.GetInt64( COL_SECONDSPANFROMDATE ) );
                        os2.ToDate = DateTimeUtilities.DateTimeFromPacked( reader.GetInt64( COL_SECONDSPANTODATE ) );
                        account.OccurrenceSpans.Add( os2 );
                    }
                    else
                    {
                        account.OccurrenceSpans.Add( null );
                    }



                    account.AccountLock =
                        AccountLockFrom( reader );

                    string clinicCode1 = reader.GetString( COL_CLINIC1 ).Trim();
                    string clinicCode2 = reader.GetString( COL_CLINIC2 ).Trim();
                    string clinicCode3 = reader.GetString( COL_CLINIC3 ).Trim();
                    string clinicCode4 = reader.GetString( COL_CLINIC4 ).Trim();
                    string clinicCode5 = reader.GetString( COL_CLINIC5 ).Trim();

                    if ( clinicCode1.Length > 0 )
                    {
                        account.Clinics[0] = hcBroker.HospitalClinicWith( account.Facility.Oid, clinicCode1 );
                        if (DOFRFeatureManager.IsDOFREnabled(account) && !string.IsNullOrEmpty(account.EmbosserCard))
                        {
                            IServiceCategoryBroker serviceCategoryBroker = BrokerFactory.BrokerOfType<IServiceCategoryBroker>();
                            account.ServiceCategory = serviceCategoryBroker.GetServiceCategoryForClinicCodeWith(account.Facility.Oid, clinicCode1, account.EmbosserCard);
                        }
                    }

                    if ( clinicCode2.Length > 0 )
                    {
                        account.Clinics[1] = hcBroker.HospitalClinicWith( account.Facility.Oid, clinicCode2 );
                    }

                    if ( clinicCode3.Length > 0 )
                    {
                        account.Clinics[2] = hcBroker.HospitalClinicWith( account.Facility.Oid, clinicCode3 );
                    }

                    if ( clinicCode4.Length > 0 )
                    {
                        account.Clinics[3] = hcBroker.HospitalClinicWith( account.Facility.Oid, clinicCode4 );
                    }

                    if ( clinicCode5.Length > 0 )
                    {
                        account.Clinics[4] = hcBroker.HospitalClinicWith( account.Facility.Oid, clinicCode5 );
                    }

                    string tenetCare = reader.GetString( COL_TENETCARE ).Trim();

                    if ( tenetCare == YesNoFlag.CODE_YES )
                    {
                        account.TenetCare.SetYes();
                    }
                    else if ( tenetCare == YesNoFlag.CODE_NO )
                    {
                        account.TenetCare.SetNo();
                    }
                    else
                    {
                        account.TenetCare.SetBlank();
                    }

                    string reRegister = reader.GetString( COL_REREGISTER ).Trim();

                    if ( reRegister == YesNoFlag.CODE_YES )
                    {
                        account.Reregister.SetYes();
                    }
                    else if ( reRegister == YesNoFlag.CODE_NO )
                    {
                        account.Reregister.SetNo();
                    }
                    else
                    {
                        account.Reregister.SetBlank();
                    }

                    string reAdmitCode = reader.GetString( COL_READMITCODE ).Trim();
                    string referralFac = reader.GetString( COL_REFERRALFACILITY ).Trim();
                    string referralType = reader.GetString( COL_REFERRALTYPE ).Trim();
                    string modeOfArrival = reader.GetString( COL_MODEOFARRIVAL ).Trim();

                    IReAdmitCodeBroker raBroker = BrokerFactory.BrokerOfType<IReAdmitCodeBroker>();
                    account.ReAdmitCode = raBroker.ReAdmitCodeWith( account.Facility.Oid, reAdmitCode );

                    IReferralFacilityBroker referralFacilityBroker = BrokerFactory.BrokerOfType<IReferralFacilityBroker>();
                    account.ReferralFacility = referralFacilityBroker.ReferralFacilityWith( account.Facility.Oid, referralFac );

                    IReferralTypeBroker referralTypeBroker = BrokerFactory.BrokerOfType<IReferralTypeBroker>();
                    account.ReferralType = referralTypeBroker.ReferralTypeWith( account.Facility.Oid, referralType );

                    IModeOfArrivalBroker modeOfArrivalBroker = BrokerFactory.BrokerOfType<IModeOfArrivalBroker>();
                    if ( !string.IsNullOrEmpty( modeOfArrival ) )
                    {
                        account.ModeOfArrival = modeOfArrivalBroker.ModeOfArrivalWith( account.Facility.Oid, modeOfArrival );
                    }

                    string arrivalTime = reader.GetString( COL_ARRIVALTIME );

                    arrivalTime = "0000" + arrivalTime;
                    arrivalTime = arrivalTime.Substring( arrivalTime.Length - 4, 4 );
                    hours = arrivalTime.Substring( 0, 2 );
                    mins = arrivalTime.Substring( 2, 2 );
                    hour = int.Parse( hours );
                    min = int.Parse( mins );

                    string schCode = reader.GetString( COL_SCHEDULECODE ).Trim();
                    IScheduleCodeBroker schBroker = BrokerFactory.BrokerOfType<IScheduleCodeBroker>();
                    account.ScheduleCode = schBroker.ScheduleCodeWith( account.Facility.Oid, schCode );
                    account.PreopDate = GetPreOpDateFrom( reader );
                    account.ArrivalTime = new DateTime( 1, 1, 1, hour, min, 0 );

                    string ipaCode = reader.GetString( COL_IPACODE ).Trim();
                    string ipaClinicCode = reader.GetString( COL_IPACLINICCODE ).Trim();
                    if ( ipaCode.Length > 0 && ipaClinicCode.Length > 0 )
                    {
                        account.MedicalGroupIPA = insuranceBroker.IPAWith( account.Facility.Oid,
                            ipaCode, ipaClinicCode );
                    }

                    // The yes no field is initialized to blank on construction
                    // on of these conditions (if met) will change the value to Yes or no
                    if ( reader.GetInt32( COL_DNR ) == 1 )
                    {
                        account.Patient.DoNotResuscitate.SetYes();
                    }
                    else if ( reader.GetInt32( COL_DNR ) == 2 )
                    {
                        account.Patient.DoNotResuscitate.SetNo();
                    }
                    if ( account.KindOfVisit.IsPreRegistrationPatient )
                    {
                        GetPregnancyIndicatorForPreAdmit( account );
                    }
                    GetCptCodesFor(account);
                    BuildAuthorizedPortalUsers(account);
                    BuildAdditionalRaceCodes(account);
                }
                else
                {
                    AccountNotFoundException ex =
                        new AccountNotFoundException( "Account Not Found.", null, Severity.High );
                    Log.Error( "AccountNotFoundException", ex );
                    throw ex;
                }
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception", ex, Log );
            }

            return;
        }
   
        /// <summary>
        /// Gets the pre op date string from ...
        /// </summary>
        /// <param name="reader">The reader.</param>
        private static DateTime GetPreOpDateFrom( SafeReader reader )
        {

            DateTime preOpDate;
            var preopDateString =
                reader.GetString( COL_PREOPDATE ) ?? "10101";

            try
            {
                preOpDate =
                    DateTimeUtilities.DateTimeForYYYYMMDDFormat( preopDateString );
            }
            catch
            {
                preOpDate = DateTime.MinValue;
            }

            return preOpDate;

        }

        private static long ConvertToLongIfInRangeOfLongElseZero( decimal mothersPatientAccount )
        {
            long mothersAccountNumber;
            if ( mothersPatientAccount < long.MinValue || mothersPatientAccount > long.MaxValue )
            {
                mothersAccountNumber = 0;
            }
            else
            {
                mothersAccountNumber = Decimal.ToInt64( mothersPatientAccount );
            }
            return mothersAccountNumber;
        }

        private static void DeriveOptOutOptions( Account account, string includeStr )
        {
            //start by breaking up the string

            char[] includeOptions = includeStr.ToCharArray();
            if ( char.ToUpper( includeOptions[OUTPUTOPTIONS] ) == 'N' )
            {
                account.OptOutName = false;
                account.OptOutHealthInformation = false;
                account.OptOutLocation = false;
                account.OptOutReligion = false;
            }
            else
            {
                account.OptOutName =
                    char.ToUpper( includeOptions[OPTOUTOPTIONNAMEINDEX] ) == 'Y' ? false : true;
                account.OptOutLocation =
                    char.ToUpper( includeOptions[OPTOUTOPTIONLOCATIONINDEX] ) == 'Y' ? false : true;
                account.OptOutHealthInformation =
                    char.ToUpper( includeOptions[OPTOUTOPTIONHEALTHINFOINDEX] ) == 'Y' ? false : true;
                account.OptOutReligion =
                    char.ToUpper( includeOptions[OPTOUTOPTIONRELIGIONINDEX] ) == 'Y' ? false : true;
            }

        }

        /// <summary>
        /// given an account that has its occurrence codes filled in and
        /// a reader, derive the type of visit this is
        /// There is an order of precidence for these. A record can contain
        /// multiple occurrence codes so if you see more than one make
        /// sure the one with the highest precidence wins
        /// 
        /// Accident
        /// Crime
        /// Illness
        /// Pregnancy
        /// PrimaryCare
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="reader"></param>
        private void DeriveVisitType( Account account, SafeReader reader )
        {
            bool foundType = false;


            foreach ( OccurrenceCode oc in account.OccurrenceCodes )
            {
                string country = reader.GetString( COL_ACCIDENTCOUNTRYCODE ).Trim();
                string state = reader.GetString( COL_AUTOACCIDENTSTATE );
                switch ( oc.Code )
                {
                    // these types have precidence over all other types so
                    // overwrite what, it anything, is there.
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO: // accidents
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT:
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL:
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_TORT_LIABILITY:
                    case OccurrenceCode.OCCURRENCECODE_ACCIDENT_OTHER:
                        Accident accident = new Accident
                        {
                            Country = addressBroker.CountryWith( account.Facility.Oid, country ),
                            State = addressBroker.StateWith(account.Facility.Oid, state),
                            OccurredOn = oc.OccurrenceDate,
                            OccurredAtHour = reader.GetString( COL_ACCIDENTTIME ),
                            Kind = ocBroker.AccidentTypeFor( account.Facility.Oid, oc )
                        };

                        account.Diagnosis.Condition = accident;

                        foundType = true;

                        break;
                    case OccurrenceCode.OCCURRENCECODE_CRIME: //crime    
                        // if there is not already a condition or there is a
                        // condition but it is not an accident...
                        if ( account.Diagnosis.Condition == null ||
                            ( account.Diagnosis.Condition.GetType() != typeof( Accident ) ) )
                        {
                            Crime crime = new Crime();

                            crime.Country = addressBroker.CountryWith( account.Facility.Oid, country );

                            crime.State = addressBroker.StateWith(account.Facility.Oid, state);

                            crime.OccurredOn = oc.OccurrenceDate;
                            crime.OccurredAtHour = reader.GetString( COL_ACCIDENTTIME );

                            account.Diagnosis.Condition = crime;

                            foundType = true;
                        }

                        break;

                }
            }

            // if we didn't find any other type set condition to unknown
            if ( foundType == false )
            {
                UnknownCondition unknownCondition = new UnknownCondition();
                account.Diagnosis.Condition = unknownCondition;
            }
        }


        private ArrayList SortAccountsByDate( ArrayList accounts )
        {
            ArrayList newList = new ArrayList();

            foreach ( AccountProxy proxy in accounts )
            {
                if ( proxy.DischargeDate.Equals( EPOCH ) )
                {
                    newList.Add( proxy );

                }
            }
            foreach ( AccountProxy proxy in accounts )
            {
                if ( !proxy.DischargeDate.Equals( EPOCH ) )
                {
                    newList.Add( proxy );

                }
            }
            return ( newList );
        }

        private static AccountLock AccountLockFrom( SafeReader reader )
        {
            AccountLock accountLock = new AccountLock();

            bool lockStatus = Convert.ToBoolean( reader.GetInt32( COL_LOCKINDICATOR ) );
            string lockedBy = reader.GetString( COL_LOCKERPBARID ).Trim();
            string lockedAt = string.Empty; // column needs to be addded in ~25 stored procs
            long lockDate = reader.GetInt32( COL_LOCKDATE );
            string lockTime = Convert.ToString( reader.GetInt32( COL_LOCKTIME ) ).Trim();

            accountLock.IsLocked = lockStatus;
            accountLock.LockedAt = lockedAt; //workstationID
            accountLock.LockedBy = lockedBy;
            accountLock.LockedOn = DateTimeUtilities.DateTimeForYYMMDDFormat( lockDate );

            if ( lockTime != ZERO && ( lockTime.Length == 5 || lockTime.Length == 6 ) )
            {
                if ( lockTime.Length == 5 )
                {
                    lockTime = String.Format( "0{0}", lockTime );
                }
                accountLock.LockedOn = accountLock.LockedOn.AddHours( Convert.ToDouble( lockTime.Substring( 0, 2 ) ) );
                accountLock.LockedOn = accountLock.LockedOn.AddMinutes( Convert.ToDouble( lockTime.Substring( 2, 2 ) ) );
                accountLock.LockedOn = accountLock.LockedOn.AddSeconds( Convert.ToDouble( lockTime.Substring( 4, 2 ) ) );
            }

            return accountLock;
        }

        private void SaveInsuranceData( Account anAccount, Activity currentActivity, AccountSaveResults results )
        {
            
            if (currentActivity is UCCPreMSERegistrationActivity)
            {
                anAccount = BuildCoverageForPreMSEActivity(anAccount);
                results.PreMSECoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
            }
            else if (currentActivity is PreMSERegisterActivity)
            {
                PullPriorVisitInsuranceToPreMseFeatureManager pullPriorVisitInsuranceToPreMseFeatureManager =
                    new PullPriorVisitInsuranceToPreMseFeatureManager();
                if ( pullPriorVisitInsuranceToPreMseFeatureManager.IsPullPriorVisitInsuranceToPreMseEnabledForDate(
                        anAccount) && (anAccount.HasValidInsurance()) )
                {
                    SetFinancialClassForPreMseActivity(anAccount);
                }
                else
                {
                    anAccount = BuildCoverageForPreMSEActivity(anAccount);
                }
                results.PreMSECoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
            }

            if ( anAccount.Guarantor != null
                && anAccount.Guarantor.DataValidationTicket != null
                && anAccount.Guarantor.DataValidationTicket.TicketId.Trim() != string.Empty
                )
            {
                DataValidationTicket ticket = anAccount.Guarantor.DataValidationTicket;
                UpdateDataValidationTicketFromAccount( anAccount, ticket );

                dvBroker.SaveDataValidationTicket( ticket );
            }

            // check primary coverage 

            if ( anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ) != null )
            {
                Coverage aCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );

                // TLG 08/03/2006 Check if the Insured's employer has a valid EmployerCode... if not,
                // we have to read it from PBAR

                if ( IsInsuredEmployerCodeInvalid( aCoverage ) )
                {
                    long insuredEmployerCode = GetInsuredEmployerCodeFor( anAccount, aCoverage );

                    aCoverage.Insured.Employment.Employer.EmployerCode = insuredEmployerCode;

                    results.PrimaryInsuranceEmployerCode = insuredEmployerCode;
                }

                //}
                if ( aCoverage.DataValidationTicket != null
                    && aCoverage.DataValidationTicket.TicketId != null
                    && aCoverage.DataValidationTicket.TicketId.Trim() != string.Empty )
                {
                    if ( aCoverage.DataValidationTicket.AccountNumber == 0 )
                    {
                        DataValidationTicket ticket = aCoverage.DataValidationTicket;
                        UpdateDataValidationTicketFromAccount( anAccount, ticket );
                    }

                    dvBroker.SaveDataValidationTicket( aCoverage.DataValidationTicket );

                    if ( aCoverage.DataValidationTicket.AccountNumber != 0 && aCoverage.DataValidationTicket.ResultsAvailable )
                    {
                        dvBroker.SendFUSInfo( aCoverage.DataValidationTicket, anAccount.Activity.AppUser );
                    }
                }
                if (DOFRFeatureManager.IsDOFREnabled(anAccount) && DOFRFeatureManager.IsPlanCommercial(anAccount))
                {
                    IDOFRInitiateBroker dOFRInitiateBroker = BrokerFactory.BrokerOfType<IDOFRInitiateBroker>();
                    dOFRInitiateBroker.SetInsurancePlanPartOfIPA(anAccount);
                }
            }

            // check secondary coverage 

            if ( anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID ) != null )
            {
                Coverage aCoverage = anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );

                // TLG 08/03/2006 Check if the Insured's employer has a valid EmployerCode... if not,
                // we have to read it from PBAR


                if ( IsInsuredEmployerCodeInvalid( aCoverage ) )
                {
                    long insuredEmployerCode = GetInsuredEmployerCodeFor( anAccount, aCoverage );
                    aCoverage.Insured.Employment.Employer.EmployerCode = insuredEmployerCode;
                    results.SecondaryInsuranceEmployerCode = insuredEmployerCode;
                }


                if ( aCoverage.DataValidationTicket != null
                    && aCoverage.DataValidationTicket.TicketId != null
                    && aCoverage.DataValidationTicket.TicketId.Trim() != string.Empty )
                {
                    if ( aCoverage.DataValidationTicket.AccountNumber == 0 )
                    {
                        DataValidationTicket ticket = aCoverage.DataValidationTicket;
                        UpdateDataValidationTicketFromAccount( anAccount, ticket );
                    }

                    dvBroker.SaveDataValidationTicket( aCoverage.DataValidationTicket );

                    if ( aCoverage.DataValidationTicket.AccountNumber != 0
                        && aCoverage.DataValidationTicket.ResultsAvailable )

                    // no longer necessary as the FUS note formatting is conditional, based on changed values
                    // && !aCoverage.DataValidationTicket.FUSNoteSent )
                    {
                        dvBroker.SendFUSInfo( aCoverage.DataValidationTicket, anAccount.Activity.AppUser );
                    }
                }
            }

            if (currentActivity is PreMSERegisterActivity || currentActivity is UCCPreMSERegistrationActivity)
            {
                results.PreMSEInsurance = anAccount.Insurance;
            }

            // if the account already has 2 coverages then any deleted secondary coverage is redundant
            // the transaction will perform an update of the 2nd insurace and hence there is no need to
            // delete the previous 2ndary insurance. 
            // also if this is a new account there might be one on the deletedsecondarycoverage attribute
            // but since the account has not been saved there is no need to delete it.
            if ( anAccount.Insurance.Coverages.Count == 2 || anAccount.IsNew )
            {
                anAccount.DeletedSecondaryCoverage = null;
                results.DeleteSecondaryCoverage = true;
            }
        }


        private static void UpdateDataValidationTicketFromAccount( Account anAccount, DataValidationTicket ticket )
        {
            ticket.AccountNumber = anAccount.AccountNumber;
            ticket.Facility = anAccount.Facility;
            ticket.MedicalRecordNumber = anAccount.Patient.MedicalRecordNumber;
        }


        private long GetInsuredEmployerCodeFor( Account anAccount, Coverage aCoverage )
        {
            Employer e = employerBroker.SelectEmployerByName( anAccount.Facility.Code,
                                                             aCoverage.Insured.Employment.Employer.Name.Trim() );

            return e.EmployerCode;
        }


        private static bool IsInsuredEmployerCodeInvalid( Coverage aCoverage )
        {
            return aCoverage.Insured.Employment != null
                   && aCoverage.Insured.Employment.Employer != null
                   && aCoverage.Insured.Employment.Employer.Name != string.Empty
                   && aCoverage.Insured.Employment.Employer.EmployerCode == 0;
        }

        private static bool AllRequiredFieldsPresent( Account anAccount )
        {
            if ( IsPreAdmitNewbornActifity( anAccount ) || IsEditPreAdmitNewbornActifity (anAccount) ||
                IsCancelPreAdmitNewbornActifity (anAccount))
            {
                return FinClassIsPresent(anAccount) &&
                       AdmitDateIsPresent(anAccount) &&
                       NameIsPresent(anAccount);
            }
           
            return FinClassIsPresent( anAccount ) &&
                AdmitDateIsPresent( anAccount ) &&
                NameIsPresent( anAccount ) &&
                HSVIsPresent( anAccount );
        }

        private static bool IsPreAdmitNewbornActifity(Account anAccount)
        {
            if(anAccount.Activity != null && anAccount.Activity.IsPreAdmitNewbornActivity())
                return true;
            return false;
        }

        private static bool IsEditPreAdmitNewbornActifity( Account anAccount )
        {
            if ( anAccount.Activity != null && anAccount.Activity.IsMaintenanceActivity() &&
                    anAccount.Activity.AssociatedActivityType != null &&
                        anAccount.Activity.AssociatedActivityType == typeof( PreAdmitNewbornActivity ) )
                return true;
            return false;
        }

        private static bool IsCancelPreAdmitNewbornActifity( Account anAccount )
        {
            if ( anAccount.Activity != null && anAccount.Activity.GetType() == typeof( CancelPreRegActivity ) 
                    && anAccount.IsNewBorn )
                return true;
            return false;
        }

        private static bool FinClassIsPresent( Account anAccount )
        {
            if ( anAccount.FinancialClass == null ||
                string.IsNullOrEmpty( anAccount.FinancialClass.Code ) )
            {
                return false;
            }

            return true;
        }

        private static bool AdmitDateIsPresent( Account anAccount )
        {
            if ( anAccount.AdmitDate == new DateTime() )
            {
                return false;
            }

            return true;
        }

        private static bool NameIsPresent( Account anAccount )
        {
            if ( anAccount.Patient == null ||
                string.IsNullOrEmpty( anAccount.Patient.FirstName ) ||
                string.IsNullOrEmpty( anAccount.Patient.LastName ) )
            {
                return false;
            }

            return true;
        }

        private static bool HSVIsPresent( Account anAccount )
        {
            if ( anAccount.HospitalService == null ||
                string.IsNullOrEmpty( anAccount.HospitalService.Code ) )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static BrokerException CreateRequiredFieldsException( Account anAccount )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append( "The following fields required for processing were not present: " );
            sb.Append( FinClassIsPresent( anAccount ) ? string.Empty : "FinancialClass " );
            sb.Append( AdmitDateIsPresent( anAccount ) ? string.Empty : "Admit Date " );
            sb.Append( NameIsPresent( anAccount ) ? string.Empty : "Patient Name " );
            sb.Append( HSVIsPresent( anAccount ) ? string.Empty : "Hospital Service " );


            BrokerException bex = new BrokerException( sb.ToString() );
            bex.Severity = Severity.Catastrophic;

            return bex;
        }

        /// <summary>
        /// Gets the account created date.
        /// </summary>
        /// <param name="anAccountProxy">An account.</param>
        /// <returns>DateTime</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void GetAccountCreatedDate( AccountProxy anAccountProxy )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            DateTime accountCreatedDate = DateTime.MinValue;
            long accountCreatedDateValue = 0;
            try
            {
                cmd = CommandFor( "CALL " + SP_GETACCOUNTCREATEDDATE +
                                      "(" + PARAM_HSPCODE +
                                      "," + PARAM_ACCOUNTNUMBER + ")",
                                      CommandType.Text,
                                      anAccountProxy.Facility );


                cmd.Parameters[PARAM_HSPCODE].Value = anAccountProxy.Facility.Code;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccountProxy.AccountNumber;

                reader = ExecuteReader( cmd );

                if ( reader.Read() )
                {
                    accountCreatedDateValue = reader.GetInt64( COL_ACTCREATEDDATE );
                }
                if ( accountCreatedDateValue != 0 )
                {
                    try
                    {
                        accountCreatedDate = DateTimeUtilities.DateTimeForYYMMDDFormat( accountCreatedDateValue );
                    }
                    catch ( Exception )
                    {
                        accountCreatedDate = DateTime.MinValue;
                    }
                }
                anAccountProxy.AccountCreatedDate = accountCreatedDate;
            }
            catch ( Exception ex )
            {
                Log.Error( "Error retrieving Account Created Date from FUS Activity file AC0005P", ex );
                throw;
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        /// <summary>
        /// Gets the CPT Codes.
        /// </summary>
        /// <param name="anAccount">An account.</param>
        /// <returns>CPT4Codes</returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void GetCptCodesFor(Account anAccount)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
          
            try
            {
                cmd = CommandFor("CALL " + SP_GETCPTCodes +
                                      "(" + PARAM_HSPCODE +
                                      "," + PARAM_ACCOUNTNUMBER + "," + PARAM_MRN + ")",
                                      CommandType.Text,
                                      anAccount.Facility);

                cmd.Parameters[PARAM_HSPCODE].Value = anAccount.Facility.Oid;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = anAccount.AccountNumber;
                cmd.Parameters[PARAM_MRN].Value = anAccount.Patient.MedicalRecordNumber;

                reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    try
                    {
                        var cptCodeSequence = reader.GetInt32( COL_CPTCODESEQUENCE );
                        var cptCode = reader.GetString( COL_CPTCODE );
                        anAccount.CptCodes[cptCodeSequence] = cptCode;
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error retrieving CPT Codes", ex);
                throw;
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
        }

        private void GetPregnancyIndicatorForPreAdmit( Account account )
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            string PregnancyIndicator = string.Empty;

            try
            {
                cmd = CommandFor( "CALL " + SP_GETPREREGPREGNANCYINDICATOR +
                   "(" + PARAM_FACILITYID +
                   "," + PARAM_MRN +
                     "," + PARAM_ACCOUNTNUMBER + ")",
                   CommandType.Text,
                   account.Facility );


                cmd.Parameters[PARAM_FACILITYID].Value = account.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = account.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = account.AccountNumber;


                reader = ExecuteReader( cmd );

                if ( reader.Read() )
                {
                    PregnancyIndicator = reader.GetString( COL_ISPREGNANT );
                }
                account.Pregnant = YesNoFlag.GetYesNoFlagFor( PregnancyIndicator );
            }
            catch ( Exception ex )
            {
                Log.Error( "Error retrieving Account Created Date from FUS Activity file AC0005P", ex );
                throw;
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
        }

        public void SetPortalOptInAndRightToRestrictValue(Account account, string portalOptInAndRightToRestrict)
        {
            if (!String.IsNullOrEmpty(portalOptInAndRightToRestrict))
            {
                var patientPortalOptIn = portalOptInAndRightToRestrict.Substring(0, 1);
                switch (patientPortalOptIn.ToUpper())
                {
                    case "Y":
                        account.PatientPortalOptIn.SetYes();
                        break;
                    case "N":
                        account.PatientPortalOptIn.SetNo();
                        break;
                    case "U":
                        account.PatientPortalOptIn.SetUnable();
                        break;
                    default:
                        account.PatientPortalOptIn.SetBlank();
                        break;
                }
            }

            if (portalOptInAndRightToRestrict.Length > 1)
            {
                var rightToRestrictValue = portalOptInAndRightToRestrict.Substring(1, 1);
                switch (rightToRestrictValue.ToUpper())
                {
                    case "Y":
                        account.RightToRestrict.SetYes();
                        break;
                    case "N":
                        account.RightToRestrict.SetNo();
                        break;
                    default:
                        account.RightToRestrict.SetBlank();
                        break;
                }
            }
            if (portalOptInAndRightToRestrict.Length > 2)
            {
                var shareDataWithPublicHieFlag = portalOptInAndRightToRestrict.Substring(2, 1);
                switch (shareDataWithPublicHieFlag.ToUpper())
                {
                    case "N":
                        account.ShareDataWithPublicHieFlag.SetNo();
                        break;
                    case "Y":
                        account.ShareDataWithPublicHieFlag.SetYes();
                        break;
                    default:
                        account.ShareDataWithPublicHieFlag.SetBlank();
                        break;
                }
            }
            if (portalOptInAndRightToRestrict.Length > 3)
            {
                var shareDataWithPCPFlag = portalOptInAndRightToRestrict.Substring(3, 1);
                switch (shareDataWithPCPFlag.ToUpper())
                {
                    case "Y":
                        account.ShareDataWithPCPFlag.SetYes();
                        break;
                    default:
                        account.ShareDataWithPCPFlag.SetNo();
                        break;
                }
            }
            if (portalOptInAndRightToRestrict.Length > 4)
            {
                var COBReceivedFlag = portalOptInAndRightToRestrict.Substring(4, 1);
                switch (COBReceivedFlag.ToUpper())
                {
                    case "N":
                        account.COBReceived.SetNo();
                        break;
                    case "Y":
                        account.COBReceived.SetYes();
                        break;
                    default:
                        account.COBReceived.SetBlank();
                        break;
                }
            }
        }

        public void SetIMFMReceivedFlagValue(Account account, string IMFMReceived)
        {
            if ( !String.IsNullOrEmpty(IMFMReceived) )
            {
                var IMFMReceivedFlag = IMFMReceived.Substring(0, 1);
                switch (IMFMReceivedFlag.ToUpper())
                {
                    case "N":
                        account.IMFMReceived.SetNo();
                        break;
                    case "Y":
                        account.IMFMReceived.SetYes();
                        break;
                    default:
                        account.IMFMReceived.SetBlank();
                        break;
                }
            }
        }
        internal void SetPortalOptIn(AccountProxy accountProxy, string portalOptInAndRightToRestrict)
        {
            accountProxy.PatientPortalOptIn = new YesNoFlag();
            if (!String.IsNullOrEmpty(portalOptInAndRightToRestrict))
            {
                string patientPortalOptIn = portalOptInAndRightToRestrict.Substring(0, 1);
                switch (patientPortalOptIn.ToUpper())
                {
                    case "Y":
                        accountProxy.PatientPortalOptIn.SetYes();
                        break;
                    case "N":
                        accountProxy.PatientPortalOptIn.SetNo();
                        break;
                    case "U":
                        accountProxy.PatientPortalOptIn.SetUnable();
                        break;
                    default:
                        accountProxy.PatientPortalOptIn.SetBlank();
                        break;
                }
            }

        }

        public void SetHospitalComminicationOptInValue(Account account, string hospCommOptInAndEmailReason)
        {
            if (!String.IsNullOrEmpty(hospCommOptInAndEmailReason))
            {
                var hospitalCommunicationOptIn = hospCommOptInAndEmailReason.Substring(0, 1);
                switch (hospitalCommunicationOptIn.ToUpper())
                {
                    case "Y":
                        account.Patient.HospitalCommunicationOptIn.SetYes();
                        break;
                    case "N":
                        account.Patient.HospitalCommunicationOptIn.SetNo();
                        break;
                    default:
                        account.Patient.HospitalCommunicationOptIn.SetBlank();
                        break;
                }
            }
            if (hospCommOptInAndEmailReason.Length > 1)
            {
                var emailReasonValue = hospCommOptInAndEmailReason.Substring(1, 1);
                var emailReason = new EmailReason() { Code = string.Empty, Description = String.Empty };
                switch (emailReasonValue.ToUpper())
                {
                    case "P":
                        emailReason.SetProvided();
                        break;
                    case "R":
                        emailReason.SetRequestRemove();
                        break;
                    case "D":
                        emailReason.SetDeclined();
                        break;
                }
                account.Patient.EmailReason = emailReason;

            }
            
        }
        
        private void ClearGuarantorCellphoneConsent(Account anAccount)
        {
            anAccount.Guarantor.ContactPointWith(
                TypeOfContactPoint.NewMobileContactPointType()).CellPhoneConsent = new CellPhoneConsent();
            anAccount.OldGuarantorCellPhoneConsent = new CellPhoneConsent();
        }

        private void BuildAuthorizedPortalUsers(Account account)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
            AuthorizedAdditionalPortalUser AuthPortalUser;
           
            try
            {
                cmd = CommandFor("CALL " + SP_AUTHORIZEDPORTALUSERS +
                                 "(" + PARAM_FACILITYID +
                                 "," + PARAM_MRN +
                                 "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    account.Facility);


                cmd.Parameters[PARAM_FACILITYID].Value = account.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = account.Patient.MedicalRecordNumber;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = account.AccountNumber;


                reader = ExecuteReader(cmd);
                int i = 0;
                while (reader.Read())
                {
                    long AUHospitalNumber = reader.GetInt64(COL_AUHOSPITALNUMBER);
                    long AUMRN = reader.GetInt64(COL_AUMEDICALRECORDNUMBER);
                    long AUAccountNumber = reader.GetInt64(COL_AUACCOUNTNUMBER);
                    long AUSequenceNumber = reader.GetInt64(COL_AUSEQUENCENUMBER);
                    string AUOPTFlag = reader.GetString(COL_AUOPTFLG);
                    string AULastName = reader.GetString(COL_AULNAME);
                    string AUFirstName = reader.GetString(COL_AUFIRSTNAME);
                    DateTime AUDateOfBirth = DateTimeUtilities
                        .DateTimeFromString(
                            reader.GetString(COL_AUDOB).Trim());
                    string AUEmailAddress = reader.GetString(COL_AUEMAIL);
                    AuthPortalUser = new AuthorizedAdditionalPortalUser();
                    AuthPortalUser.SequenceNumber = (int) AUSequenceNumber;
                    AuthPortalUser.LastName = AULastName.Trim();
                    AuthPortalUser.FirstName = AUFirstName.Trim();
                    AuthPortalUser.DateOfBirth = AUDateOfBirth;
                    AuthPortalUser.EmailAddress = new EmailAddress(AUEmailAddress.Trim());
                     
                    account.AuthorizedAdditionalPortalUsers.Add(i, AuthPortalUser);
                    i = i + 1;
                }
                 
            }
            catch (Exception ex)
            {
                Log.Error("Error retrieving Authorized portal Users", ex);
                throw;
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
        }
         private void BuildAdditionalRaceCodes(Account account)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
             
            try
            {
                cmd = CommandFor("CALL " + SP_ADDITIONALRACECODESFORAPATIENT +
                                 "(" + PARAM_FACILITYID +
                                 "," + PARAM_MRN + ")",
                    CommandType.Text,
                    account.Facility);


                cmd.Parameters[PARAM_FACILITYID].Value = account.Facility.Oid;
                cmd.Parameters[PARAM_MRN].Value = account.Patient.MedicalRecordNumber; 

                reader = ExecuteReader(cmd);
                
                while (reader.Read())
                { 
                    long ARSequenceNumber = reader.GetInt64(COL_ARSEQUENCENUMBER);
                    string ARRaceCode = reader.GetString(COL_ARRACECODE);
                    Race NewRace = null;
                    if (!String.IsNullOrEmpty(ARRaceCode))
                    {
                        NewRace = originBroker.RaceWith(account.Patient.Facility.Oid, ARRaceCode);
                    }

                    if ( NewRace != null && !String.IsNullOrEmpty(NewRace.Code))
                    {
                        switch (ARSequenceNumber)
                        {
                            case 3:
                                account.Patient.Race3 = NewRace;
                                break;
                            case 4:
                                account.Patient.Race4 = NewRace;
                                break;
                            case 5:
                                account.Patient.Race5 = NewRace;
                                break;
                        }
                    }
                }
                 
            }
            catch (Exception ex)
            {
                Log.Error("Error retrieving Additional Race Codes", ex);
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

        #region Construction and Finalization
        static AccountPBARBroker()
        {
            Model.IsTrackingEnabled = false;
        }

        public AccountPBARBroker()
        {
            Model.IsTrackingEnabled = false;
        }
        public AccountPBARBroker( string cxnString )
            : base( cxnString )
        {
            Model.IsTrackingEnabled = false;
        }
        public AccountPBARBroker( IDbTransaction txn )
            : base( txn )
        {
            Model.IsTrackingEnabled = false;
        }
        #endregion

        #region Data Elements

        private delegate ArrayList GetPriorAccountsDelegate();

        private readonly ArrayList withPaymentPlan = new ArrayList();
        private readonly ArrayList withoutPaymentPlan = new ArrayList();
        private ArrayList accountProxyList;

        private static readonly ILog Log = LogManager.GetLogger( typeof( AccountPBARBroker ) );

        private readonly IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
        private readonly IEmployerBroker employerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();
        private readonly IGuarantorBroker guarantorBroker = BrokerFactory.BrokerOfType<IGuarantorBroker>();
        private readonly IPhysicianBroker physicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
        private readonly IInsuranceBroker insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
        private readonly IFinancialClassesBroker fcBroker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
        private readonly IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
        private readonly IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
        private readonly IHSVBroker hsvBroker = BrokerFactory.BrokerOfType<IHSVBroker>();
        private readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private readonly IRelationshipTypeBroker relTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
        private readonly IDischargeBroker dischargeBroker = BrokerFactory.BrokerOfType<IDischargeBroker>();
        private readonly IAdmitSourceBroker asBroker = BrokerFactory.BrokerOfType<IAdmitSourceBroker>();
        private readonly INPPVersionBroker nppBroker = BrokerFactory.BrokerOfType<INPPVersionBroker>();
        private readonly IHospitalClinicsBroker hcBroker = BrokerFactory.BrokerOfType<IHospitalClinicsBroker>();
        private readonly IDataValidationBroker dvBroker = BrokerFactory.BrokerOfType<IDataValidationBroker>();
        private readonly IOccuranceCodeBroker ocBroker = BrokerFactory.BrokerOfType<IOccuranceCodeBroker>();
        private readonly IEmploymentStatusBroker employmentStatusBroker = BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
        private readonly IInfoReceivedSourceBroker infoReceivedSourceBroker = BrokerFactory.BrokerOfType<IInfoReceivedSourceBroker>();
        private readonly IReligionBroker religionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();
        private readonly IOriginBroker originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
        private readonly IResearchStudyBroker researchStudyBroker = BrokerFactory.BrokerOfType<IResearchStudyBroker>();
        #endregion

        #region Constants

        #region code constants

        private const int LENGTH_OF_STATE_CODE = 2;

        // ReadDetailAccountData()
        private const long
            OCCURRENCECOUNT = 20,
            CONDITIONCOUNT = 7;

        // indicator that data is not present for an account
        private const long
            NO_DATA_PRESENT = 0;
        // DeriveOptOutOptions()
        private const long
            OUTPUTOPTIONS = 0L,
            OPTOUTOPTIONNAMEINDEX = 1L,
            OPTOUTOPTIONLOCATIONINDEX = 2L,
            OPTOUTOPTIONHEALTHINFOINDEX = 3L,
            OPTOUTOPTIONRELIGIONINDEX = 4L;

        private readonly DateTime EPOCH = new DateTime( 1, 1, 1 );

        private const string
            ALL_NURSING_STATIONS = "All",
            SHOW_UNOCCUPIED_BEDS = "N",
            SHOW_ALL_BEDS = "Y",
            ACCOUNT_CANCELED_INDICATOR = "G",
            ZERO = "0",
            FIN_CLASS_73 = "73",
            DATETIME_MIN_VALUE = "00000000",
            PRE_MSE_INSURANCE_PLAN_ID = "EDL81",
            PRE_MSE_INSURANCE_BACKUP_PLAN_ID = "EDL99",
            PRE_MSE_RELATIONSHIP_CODE = "01",
            PRE_MSE_FINANCIAL_CLASS_CODE = "37",
            COVERAGE_ORDER_DESCRIPTION = "PRIMARY",
            CONSULTING_DR = "CONSULTINGDR",
            ID = "ID";

        private const int
            TOTAL_LENGTH_OF_COMMENTS = 65,
            TOTAL_LENGTH_OF_EMBOSER_CARD = 10,
            LENGTH_OF_COMMENTS_TWO = 55,
            LENGTH_OF_PHONE_NUMBER = 7,
            TOTAL_NUMBER_OF_CONSULTING_PHYSICIANS = 5;

        private const long
            NON_STAFF_PHYSICIAN_NUMBER = 8888;

        private const string
            COVERAGEPRIORITYONE = "1",
            COVERAGEPRIMARY = "Primary",
            COVERAGESECONDARY = "Secondary";

        private const long
            PRIMARYCOVERAGEID = 1,
            SECONDARYCOVERAGEID = 2;

        private const string ACCOUNT_NULL_ERROR_MESSAGE = "Account is null";
        private const string ErrorPCPRetreival = "Error retrieving Primary Care Physician information for account";
        #endregion

        #region stored proc names

        private const string
            SP_SELECT_PATIENT_ACCOUNTS_FOR = "SELECTACCOUNTSFOR",
            SP_MPI_ACCOUNTS = "MPIACCOUNTSFOR",
            SP_MPI_ACCOUNTDETAILS_FOR = "MPIACCOUNTDETAILSFOR",
            SP_GET_PRIOR_ACCOUNTS = "GETPRIORACCOUNTS",
            SP_NONSTAFF_PHYSICIAN_INFO = "SELECTNONSTAFFPHYSICIANINFO",
            SP_ADT_ACCOUNTS_MATCHING = "ACCOUNTSMATCHING",
            SP_INSURANCE_PLAN_ACCOUNTS_MATCHING = "AccountsMatchingInsurance",
            SP_RELIGION_ACCOUNTS_MATCHING = "ReligionAccountsFor",
            SP_SELECT_ACCOUNTS_WITH_WORKLISTS = "SELECTACCOUNTSWITHWORKLISTSFOR",
            SP_BLOODLESS_ACCOUNTS_MATCHING = "BloodlessTreatmentAccountsFor",
            SP_ACCOUNTS_MATCHING = "AccountsMatchingPatient",
            SP_NURSING_STATION_ACCOUNTS_MATCHING = "AccountsMatchingNursingStations",
            SP_PHYSICIAN_PATIENTS_ACCOUNTS_MATCHING = "PhysicianAccountsFor",
            SP_ACCOUNT_FOR = "AccountFor",
            SP_ACCOUNT_LOCK_STATUS_FOR = "AccountLockStatusFor",
            SP_SELECT_ACCOUNT_PROXY_FOR = "SelectAccountProxyFor",
            SP_VERIFY_OFFLINE_ACCTNUM = "VerifyOfflineAccountNumber",
            SP_VERIFY_OFFLINE_MRN = "VerifyOfflineMRN",
            SP_DUPLICATEACCOUNTS_MATCHING_MRN = "SelectDuplicatePreregAcctsMatchingMRN",
            SP_DUPLICATEACCOUNTS_MATCHING_SSN = "SelectDuplicatePreregAcctsMatchingSSN",
            SP_DUPLICATEACCOUNTS_MATCHING_NAMES = "SELECTDUPLICATEPREREGACCTSNAMES",
            SP_TXN_COUNT_FOR = "TXNCOUNTFOR",
            SP_SELECT_DEMOGRAPHICS_INFO = "SELECTDEMOGRAPHICSINFO",
            SP_SELECT_GENERAL_ACCT_DATA = "SELECTGENERALACCTDATA",
            SP_SELECT_ACCOUNT_INSURED_DATA = "SELECTACCOUNTINSUREDDATA",
            SP_SELECT_ACCT_EMERG_CONTACT = "SELECTACCTEMERGCONTACT",
            SP_SELECT_NEAREST_RELATIVE = "SELECTNEARESTRELATIVE",
            SP_SELECT_ACCT_FINANCIAL_DATA = "SELECTACCTFINANCIALDATA",
            SP_SELECT_PREMSE_INS_DATA = "SELECTPREMSEINSDATA",
            SP_SELECT_ACCT_PREME_ACCT = "SELECTACCTPREMEACCT",
            SP_SELECT_BENEFIT_VAL_DATA = "SELECTBENEFITVALDATA",
            SP_SELECTPREVOPLOCATION = "SELECTPREVOPLOCATION",
            SP_GETACCTCOMPSPRESENT = "GETACCTCOMPSPRESENT",
            SP_GETACCOUNTCREATEDDATE = "GETACCOUNTCREATEDDATE",
            SP_GETACCOUNTACTIVITYHISTORY = "GETACCOUNTACTIVITYHISTORY",
            SP_GETPREREGPREGNANCYINDICATOR = "GETPREREGPREGNANCYINDICATOR",
            SP_GETPCPFOR = "GETPCPFOR",
            SP_GETCPTCodes = "GETCPTCODESFOR",
            SP_AUTHORIZEDPORTALUSERS = "SELECTALLAUTHORIZEDUSERSFOR" , 
            SP_ADDITIONALRACECODESFORAPATIENT = "SELECTADDITIONALRACESFORAPATIENT" ;
  
        #endregion

        #region stored proc params

        protected const string
            PARAM_HSPNUMBER = "@P_HSP";

        private const string
            PARAM_HSPCODE = "@P_HSPCODE",
            PARAM_FACILITYID = "@P_FacilityID",
            PARAM_MRC = "@P_MRC",
            PARAM_MRN = "@P_MRN";

        protected const string
            PARAM_ACCOUNTNUMBER = "@P_ACCOUNTNUMBER";

        private const string
            PARAM_ACCOUNTNUMBERSEED = "@P_AccountNumberSeed",
            PARAM_ACCT = "@P_ACCT",

            PARAM_TXN = "@P_TXN",

            PARAM_FIRSTNAME = "@P_FirstName",
            PARAM_LASTNAME = "@P_LastName",
            PARAM_SSN = "@P_SSN",
            PARAM_DOB = "@P_DOB",
            PARAM_RELIGION = "@P_ReligionCode",

            PARAM_ADTACTIVITY = "@P_ADTActivity",

            PARAM_NURSINGSTATIONS = "@P_NursingStation",
            PARAM_NURSING_STATION = "@P_Nursing_Station",
            PARAM_IS_OCCUPIEDBEDS = "@P_IsOccupied",

            PARAM_PATIENTTYPE = "@P_PatientType",
            PARAM_PATIENTTYPE_PREREG = "@P_PATIENTTYPE_PREREG",
            PARAM_PATIENTTYPE_INPAT = "@P_PATIENTTYPE_INPAT",
            PARAM_PATIENTTYPE_OUTPATIENT = "@P_PATIENTTYPE_OUTPATIENT",
            PARAM_PATIENTTYPE_PREMSE = "@P_PATIENTTYPE_PREMSE",
            PARAM_PATIENTTYPE_POSTMSE = "@P_PATIENTTYPE_POSTMSE",
            PARAM_PATIENTTYPE_RECUR = "@P_PATIENTTYPE_RECUR",
            PARAM_PATIENTTYPE_NONPAT = "@P_PATIENTTYPE_NONPAT",

            PARAM_STARTINGLETTERS = "@P_StartingLetters",
            PARAM_ENDINGLETTERS = "@P_EndingLetters",
            PARAM_STARTDATE = "@P_STARTDATE",
            PARAM_ENDDATE = "@P_ENDDATE",
            PARAM_ADMITDATE = "@P_AdmitDate",
            PARAM_STARTTIME = "@P_StartTime",

            PARAM_WORKLISTID = "@P_WorklistID",
            PARAM_RANGEID = "@P_RangeID",

            PARAM_COVERAGECATEGORY = "@P_CoverageCategory",

            PARAM_PHYSICIAN_NUMBER = "@P_PhysicianNumber",
            PARAM_ADMITTING = "@P_Admitting",
            PARAM_ATTENDING = "@P_Attending",
            PARAM_REFERRING = "@P_Referring",
            PARAM_CONSULTING = "@P_Consulting",
            PARAM_OPERATING = "@P_Operating",

            OUTPUT_PARAM_RESULT = "@O_RESULT",
            OUTPUT_PARAM_EMERGCONACTPRESENT = "@O_EMERGCONTACTPRESENT",
            OUTPUT_PARAM_NEARESTRELAVICEPRESENT = "@O_NEARESTRELAVIVEPRESENT",
            OUTPUT_PARAM_MSPPRESENT = "@O_MSPPRESENT",
            OUTPUT_PARAM_FINDATAPRESENT = "@O_FINDATAPRESENT",
            OUTPUT_PARAM_BVDATAPRESENT = "@O_BVDATAPRESENT";

        #endregion

        #region columns for account data

        private const string
            COL_MULTISITE_FLAG = "MULTISITEFLG",
            COL_SITE_CODE = "SITECODE",

            COL_ACCOUNTNUMBER = "AccountNumber",
            COL_LPP_ACCOUNT_NUMBER = "LPPACCOUNTNUMBER",
            COL_COPIEDFROMACCOUNTNUMBER = "COPIEDFROMACCOUNTNUMBER",
            COL_MEDICALRECORDNUMBER = "MEDICALRECORDNUMBER",
            COL_PREVIOUSMRN = "PreviousMrn",
            COL_ORIGINAL_MRN1 = "ORIGINALMRN1",
            COL_ORIGINAL_MRN2 = "ORIGINALMRN2",

            COL_LPLMD = "LASTMAINTENANCEDATE",
            COL_LPLML = "LASTMAINTENANCELOGNUMBER",
            COL_LPLUL = "UPDATELOGNUMBER",
            COL_PATIENT_LASTUPDATED = "LastUpdated",
            COL_ACCOUNT_LASTUPDATED = "LastMaintenceDateAbstract",

            COL_SEEN_IN_ED = "SeenInED",

            COL_ACTIVITY_DESCRIPTION = "ACTIVITYDESCRIPTION",
            COL_SHORT_REG_TYPE = "SHORTREGISTRATIONTYPE",
            COL_NEWBORN_REG_TYPE = "NEWBORNREGISTRATIONTYPE",
            
            // demographics

            COL_FIRSTNAME = "FirstName",
            COL_MIDDLEINITIAL = "MIDDLEINITIAL",
            COL_LASTNAME = "LastName",
            COL_SUFFIX = "SUFFIX",
            COL_ALIAS_FIRSTNAME = "AKFirstName",
            COL_ALIAS_LASTNAME = "AKLastName",

            COL_ADMITMONTH = "ADMITMONTH",
            COL_ADMITDAY = "ADMITDAY",
            COL_ADMITYEAR = "ADMITYEAR",
            COL_TIMEOFADMISSION = "TIMEOFADMISSION",
            COL_ADMISSION_DATE = "AdmissionDate",
            COL_ADMIT_DATE = "ADMITDATE",
            COL_ADMIT_TIME = "ADMITTIME",
            COL_SCHEDULECODE = "SCHEDULECODE",
            COL_PREOPDATE = "PREOPDATE",
            COL_GENDER_ID = "GenderID",
            COL_GENDER = "Gender",
            COL_BIRTHSEX = "BIRTHSEX",
            COL_DATEOFBIRTH = "DOB",
            COL_MARITALSTATUSCODE = "MaritalStatus",
            COL_MARITALSTATUSID = "MARITALSTATUSCODE",
            COL_RACE = "Race",
            COL_RACECODE = "RACECODE",
            COL_RACE2CODE = "Race2",
            COL_NATIONALITY1 = "NATIONALITY1",
            COL_NATIONALITY2 = "NATIONALITY2",
            COL_ETHNICITY = "Ethnicity",
            COL_ETHNICITYCODE = "ETHNICITYCODE",
            COL_ETHNICITY2CODE = "ETHNICITY2",
            COL_DESCENT1 = "DESCENT1",
            COL_DESCENT2 = "DESCENT2",
            COL_SSN = "SSN",
            COL_NATIONALID = "NATIONALID",

            COL_LOCALADDRESS = "LocalAddress",
            COL_LOCALADDRESSCITY = "LocalAddressCity",
            COL_LOCALZIP = "LocalZip",
            COL_LOCALZIPEXTN = "LocalZipExtn",
            COL_LOCALSTATEID = "LocalStateID",
            COL_COUNTRYCODE = "CountryCode",
            COL_AREA_CODE = "AreaCode",
            COL_PHONE_NO = "PhoneNumber",
            COL_CELL_PHONE_NO = "CellPhone",

            COL_PERMANENTADDRESS = "PermanentAddress",
            COL_PERMANENTCITY = "PermanentCity",
            COL_PERMANENTZIP = "PermanentZip",
            COL_PERMANENTZIPEXTN = "PermanentZipExtn",
            COL_PERMANENTSTATECODE = "PermanentStateCode",
            COL_PERMANENTAREACODE = "PermanentAreaCode",
            COL_PERMANENTPHONE = "PermanentPhone",
            COL_BIRTHTIMEENTERED = "BIRTHTIMEENTERED",

            // employment

            COL_DRIVINGLICENSE = "DrivingLicense",
            COL_PASSPORTNUMBER = "PassportNumber",
            COL_PASSPORTCOUNTRY = "PassportCountry",

            COL_PLACEOFBIRTH = "PLACEOFBIRTH",
            COL_LANGUAGECODE = "LANGUAGECODE",
            COL_OTHERLANGUAGE = "OTHERLANGUAGE",
            COL_RELIGION_RELIGIONID = "ReligionID",
            COL_RELIGIONCODE = "RELIGIONCODE",
            COL_CLERGYVISIT = "CLERGYVISIT",
            COL_RELIGIOUSCONGREGATIONCODE = "RELIGIOUSCONGREGATIONCODE",
            COL_RELIGION_PLACEOFWORSHIPCODE = "PlaceofWorship",
            COL_VALUABLESARETAKEN = "VALUABLESARETAKEN",
            COL_TIMEOFBIRTH  = "TIMEOFBIRTH",

            // diagonosis

            COL_PATIENTTYPECODE = "PATIENTTYPECODE",
            COL_PATIENTTYPE = "PatientType",
            COL_REREGISTER = "Reregister",
            COL_OPSTATUSCODE = "OPStatusCode",
            COL_IPSTATUSCODE = "IPStatusCode",
            COL_OPVISITNUMBER = "OPVisitNumber",
            COL_PENDINGPURGE = "PendingPurge",
            COL_SERVICECODE = "ServiceCode",
            COL_MEDICALSERVICECODE = "MedicalServiceCode",
            COL_HOSPITALSERVICECODE = "HOSPITALSERVICECODE",
            COL_HOSPITALSERVICE = "HospitalService",
            COL_MOTHERSACCOUNTNUMBER = "MOTHERSPATIENTACCOUNT",

            COL_NURSINGSTATION = "NURSINGSTATION",
            COL_ROOM = "ROOM",
            COL_BED = "BED",
            COL_PENDING_ADMISSION = "PendingAdmission",
            COL_OVERFLOW = "OVERFLOWFLAG",
            COL_ISOLATIONCODE = "ISOLATIONCODE",
            COL_ACCOMODATION_CODE = "Accomodationcode",
            COL_ACCOMODATION_DESC = "AccomodationDesc",
            COL_ROOM_CONDITION = "RoomCondition",
            COL_DIAGNOSIS = "DIAGNOSIS",
            COL_PORTALOPTIN_RIGHTTORESTRICT = "PATIENTPORTALOPTIN",
            COL_LPFILL = "IMFMRECEIVED",
            //CptCodes
            COL_CPTCODESEQUENCE = "CPTSEQUENCE",
            COL_CPTCODE = "CPTCODE",
            // chief complaint
            COL_CHIEFCOMPLAINT = "ChiefComplaint",
            COL_PROCEDURE = "PROCEDUREDATA",
            COL_ALTERNATECAREFACILITYCODE = "ALTERNATEFACILITY",
            COL_ACCIDENTTIME = "ACCIDENTTIME",
            COL_ACCIDENTCOUNTRYCODE = "ACCIDENTCOUNTRYCODE",
            COL_AUTOACCIDENTSTATE = "AUTOACCIDENTSTATE",

            COL_TENETCARE = "TENETCARE",
            COL_ADMIT_SOURCE_CODE = "AdmitSource",
            COL_ADMIT_SOURCECODE = "AdmitSourceCode",
            COL_ADMITTING_CATEGORY ="AdmitCategory",
            COL_REFERRALSOURCECODE = "REFERRALSOURCE",

            COL_CLINIC1 = "ClinicCode1",
            COL_CLINIC2 = "ClinicCode2",
            COL_CLINIC3 = "ClinicCode3",
            COL_CLINIC4 = "ClinicCode4",
            COL_CLINIC5 = "ClinicCode5",

            COL_ARRIVALTIME = "ARRIVALTIME",
            COL_MODEOFARRIVAL = "MODEOFARRIVAL",
            COL_REFERRALTYPE = "REFERRALTYPE",
            COL_REFERRALFACILITY = "REFERRALFACILITY",
            COL_READMITCODE = "READMITCODE",

            // clinic

            COL_ATTENDING_DOCTOR_ID = "AttendingDRID",
            COL_ADMITTING_DOCTOR_ID = "AdmittingDRID",
            COL_REFERRING_DOCTOR_ID = "ReferingDRID",
            COL_OPERATING_DOCTOR_ID = "OperatingDRID",
            COL_PCP_DOCTOR_ID = "OtherDRID",
            COL_CONSULTINGDR1ID = "CONSULTINGDR1ID",
            COL_CONSULTINGDR2ID = "CONSULTINGDR2ID",
            COL_CONSULTINGDR3ID = "CONSULTINGDR3ID",
            COL_CONSULTINGDR4ID = "CONSULTINGDR4ID",
            COL_CONSULTINGDR5ID = "CONSULTINGDR5ID",

            COL_SMOKER = "Smoker",
            COL_BLOODLESS = "BloodLess",
            COL_BLOODLESSPATIENT = "BLOODLESS",
            COL_ISPREGNANT = "ISPREGNANT",
            COL_RESISTANTORGANISM = "RESISTANTORGANISM",
            COL_PATIENTINCLINICALSTUDY = "PATIENTINCLINICALRESEARCHSTUDY",
            COL_COMMENTLINE1 = "CommentLine1",
            COL_COMMENTLINE2 = "CommentLine2",

            COL_RESEARCHID01 = "RESEARCHSTUDYID01",
            COL_RESEARCHID02 = "RESEARCHSTUDYID02",
            COL_RESEARCHID03 = "RESEARCHSTUDYID03",
            COL_RESEARCHID04 = "RESEARCHSTUDYID04",
            COL_RESEARCHID05 = "RESEARCHSTUDYID05",
            COL_RESEARCHID06 = "RESEARCHSTUDYID06",
            COL_RESEARCHID07 = "RESEARCHSTUDYID07",
            COL_RESEARCHID08 = "RESEARCHSTUDYID08",
            COL_RESEARCHID09 = "RESEARCHSTUDYID09",
            COL_RESEARCHID10 = "RESEARCHSTUDYID10",

            COL_RESEARCHCONSENTFLAG01 = "RESEARCHSTUDYCONSENTFLAG01",
            COL_RESEARCHCONSENTFLAG02 = "RESEARCHSTUDYCONSENTFLAG02",
            COL_RESEARCHCONSENTFLAG03 = "RESEARCHSTUDYCONSENTFLAG03",
            COL_RESEARCHCONSENTFLAG04 = "RESEARCHSTUDYCONSENTFLAG04",
            COL_RESEARCHCONSENTFLAG05 = "RESEARCHSTUDYCONSENTFLAG05",
            COL_RESEARCHCONSENTFLAG06 = "RESEARCHSTUDYCONSENTFLAG06",
            COL_RESEARCHCONSENTFLAG07 = "RESEARCHSTUDYCONSENTFLAG07",
            COL_RESEARCHCONSENTFLAG08 = "RESEARCHSTUDYCONSENTFLAG08",
            COL_RESEARCHCONSENTFLAG09 = "RESEARCHSTUDYCONSENTFLAG09",
            COL_RESEARCHCONSENTFLAG10 = "RESEARCHSTUDYCONSENTFLAG10",

            // insurance
            COL_PAYORNAME = "PayorName",
            COL_PRIMARYINSURANCEPLAN = "PRIMARYINSURANCEPLAN",
            COL_PRIMARY_PLANID = "PRIMARYPLANID",
            COL_PRIMARYPLANNAME = "PRIMARYPLANNAME",
            COL_SECONDARYPLANNAME = "SECONDARYPLANNAME",
            COL_SECONDARYPLAN = "SECONDARYPLAN",

            COL_FINANCIALCODE = "FinancialCode",
            COL_FINANCIALCLASS = "FinancialClass",
            COL_FINANCIALCLASSCODE = "FINANCIALCLASSCODE",
            COL_MAIDENNAME = "MaidenName",
            COL_DNR = "DNR",
            COL_MOTHERSNAME = "MothersName",
            COL_FATHERSNAME = "FathersName",
            COL_MDFDOB = "FATHERDOB",
            COL_MDMDOB = "MOTHERDOB",

            COL_CLINICCODE = "ClinicCode",
            COL_CLINICDESC = "ClinicDescription",
            COL_IPACODE = "IPACODE",
            COL_IPACLINICCODE = "IPACLINICCODE",

            // billing

            COL_OCCURRENCECODE = "OCCURANCECODE",
            COL_OCCURRENCEDATE = "OCCURANCEDATE",
            COL_CONDITIONCODE = "CONDITIONCODE",

            COL_FIRSTSPANCODE = "FIRSTSPANCODE",
            COL_FIRSTSPANFROMDATE = "FIRSTSPANFROMDATE",
            COL_FIRSTSPANTODATE = "FIRSTSPANTODATE",
            COL_FIRSTSPANFACILITY = "FIRSTSPANFACILITY",
            COL_SECONDSPANCODE = "SECONDSPANCODE",
            COL_SECONDSPANFROMDATE = "SECONDSPANFROMDATE",
            COL_SECONDSPANTODATE = "SECONDSPANTODATE",

            COL_VALUECODE1 = "VALUECODE1",
            COL_VALUECODE2 = "VALUECODE2",
            COL_VALUECODE3 = "VALUECODE3",
            COL_VALUECODE4 = "VALUECODE4",
            COL_VALUECODE5 = "VALUECODE5",
            COL_VALUECODE6 = "VALUECODE6",
            COL_VALUECODE7 = "VALUECODE7",
            COL_VALUECODE8 = "VALUECODE8",

            COL_VALUEAMOUNT1 = "VALUEAMOUNT1",
            COL_VALUEAMOUNT2 = "VALUEAMOUNT2",
            COL_VALUEAMOUNT3 = "VALUEAMOUNT3",
            COL_VALUEAMOUNT4 = "VALUEAMOUNT4",
            COL_VALUEAMOUNT5 = "VALUEAMOUNT5",
            COL_VALUEAMOUNT6 = "VALUEAMOUNT6",
            COL_VALUEAMOUNT7 = "VALUEAMOUNT7",
            COL_VALUEAMOUNT8 = "VALUEAMOUNT8",

            // liability

            COL_BALANCEDUE = "TOTALDUE",
            COL_LIABILITYFLAG = "LIABILITYFLAG",
            COL_INSURED_TOTALAMOUNTDUE = "INSUREDTOTALAMOUNTDUE",
            COL_UNINSURED_TOTALAMOUNTDUE = "UNINSUREDTOTALAMOUNTDUE",
            COL_FINALBILLINGFLAG = "FinalBillingFlag",
            COL_UNBILLEDBALANCE = "UnbilledBalance",
            COL_TOTALCHARGES = "TotalCharges",

            // payment

            COL_PAYMENTS = "PAYMENTS",
            COL_LENGTHOFSTAY = "LengthOfStay",

            // contacts

            COL_CONTACTNAME = "CONTACTNAME",
            COL_RELATIONSHIPCODE = "RELATIONSHIPCODE",
            COL_ADDRESS = "ADDRESS",
            COL_CITY = "CITY",
            COL_STATE = "STATE",
            COL_ZIP = "ZIP",
            COL_ZIPEXT = "ZIPEXT",
            COL_AREACODE = "AREACODE",
            COL_PHONENUMBER = "PHONENUMBER",

            // regulatory

            COL_NPP_VERSION = "NPPVer",
            COL_NPPVERSION = "NPPVERSION",
            COL_NPP_SIGNATURE_STATUS = "NPPSignatureStatus",
            COL_NPP_DATE_SIGNED = "NPPDateSigned",

            COL_CONFIDENTIALFLAG = "CONFIDENTIALFLAG",
            COL_CONFIDENTIAL = "Confidential",
            COL_OPTOUTFLAGS = "OPTOUTFLAGS",
            COL_OPTOUT_INFORMATION = "OptOut",
            COL_FACILITYFLAG = "FACILITYFLAG",
            COL_HOSPCOMMOPTIN = "HOSPITALCOMMUNICATIONOPTIN",

            // worklist
            COL_ACTIONCOUNT = "ActionCount",
            COL_PREDISCHARGEFLAG = "PREDISCHARGESTATUS",
            COL_PENDINGDISCHARGE = "PENDINGDISCHARGE",

            // account lock
            COL_LOCKINDICATOR = "LockIndicator",
            COL_LOCKERPBARID = "LOCKERPBARID",
            COL_LOCKEDWRKSTN = "LOCKEDWORKSTATION",
            COL_LOCKDATE = "LockDate",
            COL_LOCKTIME = "LockTime",

            // transfer
            COL_TRANSACTIONTYPE = "TRANSACTIONTYPE",
            COL_ADTTIME = "ADTTIME",

            COL_NURSINGSTATIONFROM = "NURSINGSTATIONFROM",
            COL_ROOMFROM = "RoomFrom",
            COL_BEDFROM = "BEDFROM",
            COL_NURSINGSTATIONTO = "NURSINGSTATIONTO",
            COL_ROOMTO = "ROOMTO",
            COL_BEDTO = "BEDTO",

            // discharge
            COL_DISCHARGEDATE = "DischargeDate",
            COL_DISCHARGETIME = "DISCHARGETIME",
            COL_DISCHARGEDISPOSITIONCODE = "DISCHARGEDISPOSITIONCODE",
            COL_DISCHARGECODE = "DISCHARGECODE",
            COL_ABSTRACTEXISTS = "ABSTRACTEXISTS",

            // txn
            COL_RECCNT = "RECCNT",

            // predischarge bed location
            COL_OBSERVATIONNS = "OBSERVATIONNS",
            COL_OBSERVATIONROOM = "OBSERVATIONROOM",
            COL_OBSERVATIONBED = "OBSERVATIONBED",
            COL_ACTCREATEDDATE = "ACCTCREATEDDATE",
            COL_RCRP = "RIGHTCARERIGHTPLACE",
            COL_LEFTORSTAYED = "LEFTORSTAYED",
            COL_LEFTWOSEEN = "LEFTWOSEEN",
            COL_LEFTWOFINCLR = "LEFTWOFINCLR";
        #endregion

        #region columns for insurance data

        private const string

            COL_INSURANCECOMPANYNUMBER = "INSURANCECOMPANYNUMBER",
            COL_PRIORITYCODE = "PRIORITYCODE",

            // InsuredFrom
            COL_INSUREDLASTNAME = "INSUREDLASTNAME",
            COL_INSUREDFIRSTNAME = "INSUREDFIRSTNAME",
            COL_INSUREDNAMESUFFIX = "INSUREDNAMESUFFIX",
            COL_INSUREDSEX = "INSUREDSEX",
            COL_INSUREDRELATIONSHIP = "INSUREDRELATIONSHIP",
            COL_INSUREDIDENTIFIER = "INSUREDIDENTIFIER",
            COL_INSUREDBIRTHDATE = "INSUREDBIRTHDATE",
            COL_INSURANCECERTIFICATIONNUM = "INSURANCECERTIFICATIONNUM",
            COL_INSURANCEGROUPNUMBER = "INSURANCEGROUPNUMBER",
            COL_CONDITIONOFSERVICE = "CONDITIONOFSERVICE",

            COL_PRIMARYEMPSTATUS = "PRIMARYEMPSTATUS",
            COL_PRIMARYEMPNAME = "PRIMARYEMPNAME",

            COL_SECONDARYEMPSTATUS = "SECONDARYEMPSTATUS",
            COL_SECONDARYEMPNAME = "SECONDARYEMPNAME",

            // CoverageFrom
            COL_EVCNUMBER = "EVCNUMBER",
            COL_AUTHORIZEDDAYS = "AUTHORIZEDDAYS",
            COL_ISSUEDATE = "ISSUEDATE",
            COL_AUTHORIZATIONNUMBER = "AUTHORIZATIONNUMBER",
            COL_GROUPNUMBER = "GROUPNUMBER",
            COL_TRACKINGNUMBER = "TRACKINGNUMBER",
            COL_SIGNEDOVERMEDICAREHICNUMBER = "SIGNEDOVERMEDICAREHICNUMBER",
            COL_MBINUMBER = "MBINUMBER" ,
            COL_BILLINGADDRESS1 = "BILLINGADDRESS1",
            COL_BILLINGCITYSTATECOUNTRY = "BILLINGCITYSTATECOUNTRY",
            COL_BILLINGZIPZIPEXTPHONE = "BILLINGZIPZIPEXTPHONE",
            COL_BILLING_CAREOF_NAME = "BILLINGCAREOFNAME",
            COL_BILLING_NAME = "BILLINGNAME",

            COL_DEDUCTIBLE = "DEDUCTIBLE",
            COL_COPAY = "COPAY",
            COL_NOLIABILITY = "NOLIABILITY",
            COL_ELIGIBILITY = "ELIGIBILITY",

            COL_APPROVALDATE = "APPROVEDON",
            COL_EFFECTIVEDATE = "EFFECTIVEON",

            COL_ATTORNEYNAME = "ATTORNEYNAME",
            COL_ATTORNEYSTREET = "ATTORNEYSTREET",
            COL_ATTORNEYCITY = "ATTORNEYCITY",
            COL_ATTORNEYSTATE = "ATTORNEYSTATE",
            COL_ATTORNEYZIP5 = "ATTORNEYZIP5",
            COL_ATTORNEYZIP4 = "ATTORNEYZIP4",
            COL_ATTORNEYCOUNTRYCODE = "ATTORNEYCOUNTRYCODE",
            COL_ATTORNEYPHONE = "ATTORNEYPHONE",

            COL_AGENTNAME = "AGENTNAME",
            COL_AGENTPHONE = "AGENTPHONE",
            COL_AGENTSTREET = "AGENTSTREET",
            COL_AGENTCITY = "AGENTCITY",
            COL_AGENTSTATE = "AGENTSTATE",
            COL_AGENTZIP5 = "AGENTZIP5",
            COL_AGENTZIP4 = "AGENTZIP4",
            COL_AGENTCOUNTRYCODE = "AGENTCOUNTRYCODE",
            COL_ADJUSTERSNAME = "ADJUSTERSNAME",
            COL_EMPLOYEESSUPERVISOR = "EMPLOYEESSUPERVISOR",

            COL_AUTHPHONE = "AUTHORIZATIONPHONE",
            COL_AUTHCOMPANY = "AUTHORIZATIONCOMPANY",
            COL_AUTHREQD = "AUTHORIZATIONFLAG",
            COL_PROMPTEXT = "AUTHORIZATIONPMTEXT",
            COL_VERIFICATIONDATE = "VERIFICATIONDATE",
            COL_VERIFICATIONFLAG = "VERIFICATIONFLAG",
            COL_VERIFICATIONBY = "VERIFIEDBY",

            COL_SERVICESAUTHORIZED = "SERVICESAUTHORIZED",
            COL_EFFECTIVEDATEOFAUTH = "EFFECTIVEDATEOFAUTH",
            COL_EXPIRATIONDATEOFAUTH = "EXPIRATIONDATEOFAUTH",
            COL_AUTHORIZATIONREMARKS = "AUTHORIZATIONREMARKS",
            COL_AUTHCOMPANYREPFIRSTNAME = "AUTHCOMPANYREPFIRSTNAME",
            COL_AUTHCOMPANYREPLASTNAME = "AUTHCOMPANYREPLASTNAME",
            COL_AUTHORIZATIONSTATUS = "AUTHORIZATIONSTATUS",
            COL_INSURANCECOMPANYNAME = "INSURANCECOMPANYNAME",

            // ReadBenefitsInfoFor(GovernmentMedicaidCoverage)
            COL_MCD_PRIORITY_CODE = "MCD_PRIORITY_CODE",
            COL_MCD_ELIGIBILTYDATE = "MCD_ELIGIBILITYDATE",
            COL_MCD_PATIENT_HAS_MEDICARE = "MCD_PATIENT_HAS_MEDICARE",
            COL_MCD_PATIENT_HAS_OTHER_INS = "MCD_PATIENT_HAS_OTHER_INS",
            COL_MCD_COPAY_AMOUNT = "MCD_COPAY_AMOUNT",
            COL_MCD_EVC_NUMBER = "MCD_EVC_NUMBER",
            COL_MCD_INFO_RECEIVED_FROM = "MCD_INFO_RECEIVED_FROM",
            COL_MCD_REMARKS = "MCD_REMARKS",

            // ReadBenefitsInfoFor(GovernmentMedicareCoverage)
            COL_MCR_PRIORITY_CODE = "MCR_PRIORITY_CODE",
            COL_MCR_PARTA_COVERAGE = "MCR_PARTA_COVERAGE",
            COL_MCR_PARTA_EFFECTIVE_DATE = "MCR_PARTA_EFFECTIVE_DATE",
            COL_MCR_PARTB_COVERAGE = "MCR_PARTB_COVERAGE",
            COL_MCR_PARTB_EFFECTIVE_DATE = "MCR_PARTB_EFFECTIVE_DATE",
            COL_MCR_PAT_HAS_MEDICARE_HMO_CVRG = "MCR_PAT_HAS_MEDICARE_HMO_CVRG",
            COL_MCR_MEDICARE_IS_SECONDARY = "MCR_MEDICARE_IS_SECONDARY",
            COL_MCR_DATEOFLAST_BILL_ACTIVITY = "MCR_DATEOFLAST_BILL_ACTIVITY",
            COL_MCR_REM_HOSPITAL_DAYS = "MCR_REM_HOSPITAL_DAYS",
            COL_MCR_REM_COINSURANCE_DAYS = "MCR_REM_COINSURANCE_DAYS",
            COL_MCR_REM_LIFETIME_RESERVE_DAYS = "MCR_REM_LIFETIME_RESERVE_DAYS",
            COL_MCR_REM_SNF_DAYS = "MCR_REM_SNF_DAYS",
            COL_MCR_REM_SNF_COINS_DAYS = "MCR_REM_SNF_COINS_DAYS",
            COL_MCR_REM_PART_A_DEDUCTIBLE = "MCR_REM_PART_A_DEDUCTIBLE",
            COL_MCR_REM_PART_B_DEDUCTIBLE = "MCR_REM_PART_B_DEDUCTIBLE",
            COL_MCR_PATIENT_IS_IN_HOSPICE = "MCR_PATIENT_IS_IN_HOSPICE",
            COL_MCR_BENEFICIARY_NAME_VERIFIED = "MCR_BENEFICIARY_NAME_VERIFIED",
            COL_MRC_INFO_RECEIVED_FROM = "MCR_INFO_RECEIVED_FROM",
            COL_MCR_REMARKS = "MCR_REMARKS",

            // ReadBenefitsInfoFor(GovernmentOtherCoverage)
            COL_GOV_PRIORITY_CODE = "GOV_PRIORITY_CODE",
            COL_GOV_INFO_RECEIVED_FROM = "GOV_INFO_RECEIVED_FROM",
            COL_GOV_ELIGIBILITY_PHONE_NUM = "GOV_ELIGIBILITY_PHONE_NUM",
            COL_GOV_INS_COMPANY_REP_NAME = "GOV_INS_COMPANY_REP_NAME",
            COL_GOV_TYPE_OF_COVERAGE = "GOV_TYPE_OF_COVERAGE",
            COL_GOV_EFFECTIVE_DATEOF_INSURED = "GOV_EFFECTIVE_DATEOF_INSURED",
            COL_GOV_TERM_DATEOF_INSURED = "GOV_TERM_DATE_FOR_INSURED",
            COL_GOV_DEDUCTABLE_AMOUNT = "GOV_DEDUCTABLE_AMOUNT",
            COL_GOV_ISDEDUCTABLE_MET = "GOV_ISDEDUCTABLE_MET",
            COL_GOV_DEDUCTABLE_AMOUNT_MET = "GOV_DEDUCTABLE_AMOUNT_MET",
            COL_GOV_COINSURANCE = "GOV_COINSURANCE",
            COL_GOV_OUT_OF_POCKET = "GOV_OUT_OF_POCKET",
            COL_GOV_IS_OUTOFPOCKET_MET = "GOV_IS_OUTOFPOCKET_MET",
            COL_GOV_OUTOFPOCKET_AMOUNT_MET = "GOV_OUTOFPOCKET_AMOUNT_MET",
            COL_GOV_PERCENT_OUTOFPOCKET = "GOV_PERCENT_OUTOFPOCKET",
            COL_COPAY_AMOUNT = "GOV_COPAY_AMOUNT",
            COL_GOV_REMARKS = "GOV_REMARKS",

            // ReadBenefitsInfoFor(SelfPayCoverage)
            COL_SELF_PRIORITY_CODE = "SELF_PRIORITY_CODE",
            COL_SELF_HAS_MEDICAID = "SELF_HAS_MEDICAID",
            COL_SELF_INSURANCE_INFO_AVAILABLE = "SELF_INSURANCE_INFO_AVAILABLE",

            // ReadBenefitsInfoFor(CommercialCoverage)
            COL_COM_PRIORITY_CODE = "COM_PRIORITY_CODE",
            COL_COM_INFO_RECEIVED_FROM = "COM_INFO_RECEIVED_FROM",
            COL_COM_ELIGIBILITY_PHONE_NUMBER = "COM_ELIGIBILITY_PHONE_NUMBER",
            COL_COM_INSURANCE_COMP_REP_NAME = "COM_INSURANCE_COMP_REP_NAME",
            COL_COM_EFFECTIVE_DATE_OF_INSURED = "COM_EFFECTIVE_DATE_OF_INSURED",
            COL_COM_TERM_DATE_OF_INSURED = "COM_TERM_DATE_FOR_INSURED",
            COL_COM_ISSVC_FORPREEXISTING_COND = "COM_ISSVC_FORPREEXISTING_COND",
            COL_COM_ISSERVICEA_COVERED_BENEFIT = "COM_ISSERVICEA_COVERED_BENEFIT",
            COL_COM_CLAIMSA_ADDRESS_VERIFIED = "COM_CLAIMSA_ADDRESS_VERIFIED",
            COL_COM_COORD_OF_BENEFITS = "COM_COORD_OF_BENEFITS",
            COL_COM_RULE_TO_DETERMINE_COB = "COM_RULE_TO_DETERMINE_COB",
            COL_COM_TYPE_OF_PRODUCT = "COM_TYPE_OF_PRODUCT",
            COL_COM_NAMEOFPPONETWORK_ORBROKER = "COM_NAMEOFPPONETWORK_ORBROKER",
            COL_COM_HOSP_IS_CONTRACTED_PROV = "COM_HOSP_IS_CONTRACTED_PROV",
            COL_COM_AUTO_CLAIM_NUMBER = "COM_AUTO_CLAIM_NUMBER",
            COL_COM_AUTO_MEDPAY_COVERAGE = "COM_AUTO_MEDPAY_COVERAGE",
            COL_COM_REMARKS = "COM_REMARKS",

            COL_CBC_PRIORITY_CODE = "BC_PRIORITY_CODE",
            COL_BC_BENEFIT_CATEGORY_ID = "BC_BENEFIT_CATEGORY_ID",   // used by GovernmentOtherCoverage as well
            COL_BC_DEDUCTABLE_AMOUNT = "BC_DEDUCTABLE_AMOUNT",
            COL_BC_TIME_PERIOD = "BC_TIME_PERIOD",
            COL_BC_DEDUCTABLE_MET = "BC_DEDUCTABLE_MET",
            COL_BC_DEDUCTABLE_METAMOUNT = "BC_DEDUCTABLE_METAMOUNT",
            COL_BC_COINSURANCE = "BC_COINSURANCE",
            COL_BC_OUT_OF_POCKET = "BC_OUT_OF_POCKET",
            COL_BC_OUTOF_POCKET_MET = "BC_OUTOF_POCKET_MET",
            COL_BC_OUTOF_POCKETMET_AMOUNT = "BC_OUTOF_POCKETMET_AMOUNT",
            COL_BC_PCT_AFTER_OUTOF_POCKET = "BC_PCT_AFTER_OUTOF_POCKET",
            COL_BC_COPAY_AMOUNT = "BC_COPAY_AMOUNT",
            COL_BC_WAIVE_COPAYIF_ADMITTED = "BC_WAIVE_COPAYIF_ADMITTED",
            COL_BC_NUMBER_OFVISITS_PER_YEAR = "BC_NUMBER_OFVISITS_PER_YEAR",
            COL_BC_LIFETIME_MAX_BENEFIT = "BC_LIFETIME_MAX_BENEFIT",
            COL_BC_REM_LIFETIME_VALUE = "BC_REM_LIFETIME_VALUE",
            COL_BC_REM_LIFETIME_VALUEMET = "BC_REM_LIFETIME_VALUEMET",
            COL_BC_MAXIMUM_BENEFIT_PERVISIT = "BC_MAXIMUM_BENEFIT_PERVISIT",
            COL_BC_REM_BENEFIT_PERVISIT = "BC_REM_BENEFIT_PERVISIT",
            COL_BC_REM_BENEFIT_PERVISITMET = "BC_REM_BENEFIT_PERVISITMET",

            // ReadBenefitsInfoFor(WorkersCompensationCoverage)
            COL_WKC_PRIORITY_CODE = "WKC_PRIORITY_CODE",
            COL_WKC_NAME_OF_NETWORK = "WKC_NAME_OF_NETWORK",
            COL_WKC_CLAIM_NUMBER_FOR_INCIDENT = "WKC_CLAIM_NUMBER_FOR_INCIDENT",
            COL_WKC_CLAIM_ADDRESS_VERIFIED = "WKC_CLAIM_ADDRESS_VERIFIED",
            COL_WKC_INSURANCE_PHONE_NUMBER = "WKC_INSURANCE_PHONE_NUMBER",
            COL_WKC_PREMIUM_PAID_TODATE = "WKC_EMP_PREMIUM_PAID_TODATE",
            COL_WKC_INFO_RECEIVED_FROM = "WKC_INFO_RECEIVED_FROM",
            COL_WKC_REMARKS = "WKC_REMARKS";

        #endregion

        #region columns for financial counceling data

        private const string
            COL_BILLDROPPED = "BillDropped",
            COL_LAST_CHARGEDATE = "LastChargeDate",
            COL_INS_TOTAL_PAYMENTS_COLLECTED = "INSTOTALPAYMENTSCOLLECTED",
            COL_INS_NUMBER_OF_MONTHLY_PAYMENTS = "INSNUMBEROFMONTHLYPAYMENTS",
            COL_INS_TOTAL_MONTHLY_PAYMENTS_DUE = "INSUREDTOTALMONTHLYPYMTDUE",
            COL_DAY_OF_MONTH_PAYMENT = "DAYOFMONTHPAYMENT",
            COL_TOTAL_AMOUNT_DUE = "TOTALAMOUNTDUE",
            COL_DISC_PLAN_AMOUNT = "DISCPLANAMOUNT",
            COL_UNINS_TOTAL_PAYMENTS_COLLECTED = "UNINSTOTALPAYMENTSCOLLECTED",
            COL_UNINS_NUMBER_OF_MONTHLY_PAYMENTS = "UNINSNUMBEROFMONTHLYPAYMENTS",
            COL_UNINS_TOTAL_MONTHLYPAYMENTSDUE = "UNINSTOTALMONTHLYPAYMENTSDUE",
            COL_RESOURCE_LIST_PROVIDED = "RESOURCELISTPROVIDED";

        #endregion

        #region columns for physician data

        private const string
            COL_ADMITTINGNSPUPIN = "ADMITTINGNSPUPIN",
            COL_ADMITTINGNSPNUM = "ADMITTINGNSPNUM",
            COL_ADMITTINGNSPAREACODE = "ADMITTINGNSPAREACODE",
            COL_ADMITTINGNSPPHONENUM = "ADMITTINGNSPPHONENUM",
            COL_ADMITTINGNSPLASTNAME = "ADMITTINGNSPLASTNAME",
            COL_ADMITTINGNSPFIRSTNAME = "ADMITTINGNSPFIRSTNAME",
            COL_ADMITTINGNSPMIDINITIAL = "ADMITTINGNSPMIDINITIAL",
            COL_ADMITTINGNSPNATLPROVID = "ADMITTINGNSPNATLPROVID",
            COL_ADMITTINGNSPSTATELICNO = "ADMITTINGNSPSTATELICNO",

            COL_REFERRINGNSPUPIN = "REFERRINGNSPUPIN",
            COL_REFERRINGNSPLASTNAME = "REFERRINGNSPLASTNAME",
            COL_REFERRINGNSPFIRSTNAME = "REFERRINGNSPFIRSTNAME",
            COL_REFERRINGNSPMIDINITIAL = "REFERRINGNSPMIDINITIAL",
            COL_REFERRINGNSPNATLPROVID = "REFERRINGNSPNATLPROVID",
            COL_REFERRINGNSPPHONENUM = "REFERRINGNSPPHONENUM",
            COL_REFFERRINGNSPSTATELICNO = "REFFERRINGNSPSTATELICNO",

            COL_ATTENDINGNSPLASTNAME = "ATTENDINGNSPLASTNAME",
            COL_ATTENDINGNSPFIRSTNAME = "ATTENDINGNSPFIRSTNAME",
            COL_ATTENDINGNSPMIDINITIAL = "ATTENDINGNSPMIDINITIAL",
            COL_ATTENDINGNSPUPIN = "ATTENDINGNSPUPIN",
            COL_ATTENDINGNSPNATLPROVID = "ATTENDINGNSPNATLPROVID",
            COL_ATTENDINGNSPPHONENUM = "ATTENDINGNSPPHONENUM",
            COL_ATTENDINGSTATELICNO = "ATTENDINGSTATELICNO",

            COL_OPRERATINGNSPLASTNAME = "OPRERATINGNSPLASTNAME",
            COL_OPRERATINGNSPFIRSTNAME = "OPRERATINGNSPFIRSTNAME",
            COL_OPRERATINGNSPMIDINITIAL = "OPRERATINGNSPMIDINITIAL",
            COL_OPRERATINGNSPUPIN = "OPRERATINGNSPUPIN",
            COL_OPRERATINGNSPNATLPROVID = "OPRERATINGNSPNATLPROVID",
            COL_OPRERATINGNSPPHONENUM = "OPRERATINGNSPPHONENUM",
            COL_OPRERATINGNSPSTATELICNO = "OPRERATINGNSPSTATELICNO",

            COL_PCPNSPLASTNAME = "PCPNSPLASTNAME",
            COL_PCPNSPFIRSTNAME = "PCPNSPFIRSTNAME",
            COL_PCPNSPMIDINITIAL = "PCPNSPMIDINITIAL",
            COL_PCPNSPUPIN = "PCPNSPUPIN",
            COL_PCPNSPNATLPROVID = "PCPNSPNATLPROVID",
            COL_PCPNSPPHONENUM = "PCPNSPPHONENUM",
            COL_PCPNSPSTATELICNO = "PCPNSPSTATELICNO",
            COL_AUHOSPITALNUMBER = "AUHOSPITALNUMBER",
            COL_AUMEDICALRECORDNUMBER = "AUMEDICALRECORDNUMBER",
            COL_AUACCOUNTNUMBER = "AUACCOUNTNUMBER" ,
            COL_AUSEQUENCENUMBER = "AUSEQUENCENUMBER" ,
            COL_AUOPTFLG = "AUOPTFLAG",
            COL_AULNAME = "AULASTNAME",
            COL_AUFIRSTNAME = "AUFIRSTNAME",
            COL_AUDOB = "AUDOB",
            COL_AUEMAIL = "AUEMAIL",

            COL_ARSEQUENCENUMBER = "ARSEQUENCENUMBER",
            COL_ARRACECODE = "ARCODE";
        private const string SP_GETNEWACCOUNTNUMBER = "GETNEWACCT#";
        private const string PARAM_HOSPITALCODE = "@IHOSPCODE";
        private const string OUT_PARAM_ACCOUNTNUMBER = "@ONEWACCT#";
        private const string QUICK_FLAG = "Q";
        private const string PAI_WALKIN_FLAG = "W";
        private const string
            SP_GETRECENTACCOUNTHISTORY = "GETRECENTACCOUNTHISTORY",
            COL_TRANSACTION_TYPE = "TRANSACTIONTYPE",
            PTCHANGETRANSACTION = "CHANGE PATIENT TYPE";


        #endregion

        #endregion
    }
}
