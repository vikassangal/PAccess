using System;
using System.Collections;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration; 

namespace PatientAccess.Persistence
{   
    [Serializable]
    internal abstract class TransactionCoordinator : ITransactionCoordinator
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// This static method returns the appropriate <see cref="TransactionCoordinator"/>
        /// based on Activity object passed 
        /// </summary>
        /// <param name="currentActivity"></param>
        /// <returns></returns>
        public static TransactionCoordinator TransactionCoordinatorFor( Activity currentActivity )
        {
            if ( currentActivity != null )
            {
                if( currentActivity.GetType().Equals ( typeof(ActivatePreRegistrationActivity ) ) )
                {
                    return new RegistrationTransactionCoordinator(
                        currentActivity.AppUser);
                }
                else if( currentActivity.GetType().Equals ( typeof(RegistrationActivity ) ) )
                {
                    return new RegistrationTransactionCoordinator(
                        currentActivity.AppUser);
                }
                else if ( currentActivity.GetType().Equals( typeof( PreAdmitNewbornActivity ) ) )
                {
                    return new PreregistrationTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( AdmitNewbornActivity ) )
                    && currentActivity.AssociatedActivityType != null
                    && currentActivity.AssociatedActivityType.Equals( typeof( ActivatePreRegistrationActivity ) ) )
                {
                    return new NewbornTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( AdmitNewbornActivity ) ) )
                {
                    return new NewbornTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( MaintenanceActivity ) ) )
                {
                    return new MaintenanceTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( PreRegistrationActivity ) ) )
                {
                    return new PreregistrationTransactionCoordinator(
                        currentActivity.AppUser);
                }
                else if ( currentActivity.GetType().Equals( typeof( QuickAccountCreationActivity ) ) )
                {
                    return new PreregistrationTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( QuickAccountMaintenanceActivity ) ) )
                {
                    return new MaintenanceTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if (currentActivity.GetType().Equals(typeof( PAIWalkinOutpatientCreationActivity )))
                {
                    return new PreregistrationTransactionCoordinator(
                        currentActivity.AppUser);
                }
                else if ( currentActivity.GetType().Equals( typeof( ShortRegistrationActivity ) ) )
                {
                    return new RegistrationTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) )
                {
                    return new MaintenanceTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( ShortPreRegistrationActivity ) ) )
                {
                    return new PreregistrationTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( PreDischargeActivity ) ) )
                {
                    return new IntentToDischargeTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( CancelPreRegActivity ) ) )
                {
                    return new CancelPreRegTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( PreMSERegisterActivity ) ) )
                {
                    return new PreMSETransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( CancelInpatientDischargeActivity ) ) )
                {
                    return new CancelDischargeTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( TransferBedSwapActivity ) ) )
                {
                    return new SwapBedTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( DischargeActivity ) ) )
                {
                    return new DischargeTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( TransferInToOutActivity ) )
                    || ( currentActivity.GetType().Equals( typeof( CancelInpatientStatusActivity ) ) ) )
                {
                    return new TransferInPatientToOutPatientTransactionCoordinator(
                        currentActivity.AppUser );
                }
               
                else if ( currentActivity.GetType().Equals( typeof( TransferOutToInActivity ) ) )
                {
                    return new TransferOutPatientToInPatientTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( TransferActivity ) ) )
                {
                    return new TransferToNewLocationTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( EditDischargeDataActivity ) ) )
                {
                    return new EditDischargeInfoTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( ReleaseReservedBedActivity ) ) )
                {
                    return new ReleaseReservedBedTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( EditRecurringDischargeActivity ) ) )
                {
                    return new EditRecurringDischargeTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) )
                {
                    return new PostMSETransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( EditPreMseActivity ) ) )
                {
                    return new EditPreMseTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if ( currentActivity.GetType().Equals( typeof( CancelOutpatientDischargeActivity ) ) )
                {
                    return new CancelOutpatientDischargeTransactionCoordinator(
                        currentActivity.AppUser );
                }
                else if (currentActivity.GetType().Equals(typeof(UCCPreMSERegistrationActivity)))
                {
                    return new PreMSETransactionCoordinator(
                        currentActivity.AppUser);
                }
                else if (currentActivity.GetType().Equals(typeof(EditUCCPreMSEActivity)))
                {
                    return new EditPreMseTransactionCoordinator(
                        currentActivity.AppUser);
                }
                else if (currentActivity.GetType().Equals(typeof(UCCPostMseRegistrationActivity)))
                {
                    return new PostMSETransactionCoordinator(
                        currentActivity.AppUser);
                }
              
                else
                {
                    throw new ApplicationException( "Unknown activity type." );
                }
            }
            return null;
        }
		
        public static IList<TransactionCoordinator>  TransactionCoordinatorsFor( Activity currentActivity )
        {
            if(currentActivity.GetType() == typeof(TransferERToOutpatientActivity))
            {
                IList<TransactionCoordinator> transactionCoordinators = new List<TransactionCoordinator>();
                transactionCoordinators.Add(new TransferToNewLocationTransactionCoordinator(
                                                currentActivity.AppUser));
                transactionCoordinators.Add(new MaintenanceTransactionCoordinator(
                                                currentActivity.AppUser));
                return transactionCoordinators;
            }
			
            if (currentActivity.GetType() == typeof(TransferOutpatientToERActivity))
            {
                IList<TransactionCoordinator> transactionCoordinators = new List<TransactionCoordinator>();
                transactionCoordinators.Add(new TransferToNewLocationTransactionCoordinator(
                                                currentActivity.AppUser));
                transactionCoordinators.Add(new MaintenanceTransactionCoordinator(
                                                currentActivity.AppUser));
                return transactionCoordinators;
            }
            if (currentActivity.GetType() == typeof (TransferInToOutActivity))
            {
                IList<TransactionCoordinator> transactionCoordinators = new List<TransactionCoordinator>();
                transactionCoordinators.Add(new TransferInPatientToOutPatientTransactionCoordinator(
                                                currentActivity.AppUser));
                transactionCoordinators.Add(new MaintenanceTransactionCoordinator(
                                                currentActivity.AppUser));
                return transactionCoordinators;
            }
            if (currentActivity.GetType() == typeof(TransferOutToInActivity))
            {
                IList<TransactionCoordinator> transactionCoordinators = new List<TransactionCoordinator>();
                transactionCoordinators.Add(new TransferOutPatientToInPatientTransactionCoordinator(
                        currentActivity.AppUser ));
                transactionCoordinators.Add(new MaintenanceTransactionCoordinator(
                                                currentActivity.AppUser));
                return transactionCoordinators;
            }
            return null;
        }
		
        /* This adds SQL statement to the SqlStatements ArrayList */
        public void Add( string aSqlStatement )
        {
            SqlStatements.Add( aSqlStatement );
        }

        /// <summary>
        /// This method creates the SQL statements using strategies to create insert statments
        /// It can be overridden if there are special cases that can not be handled like the
        /// Bed swap txn that uses 2 accounts.
        /// </summary>
        public virtual void CreateSQL(  )
        {
            Account account = this.Account;

            // Retrieve PBARSecurityCode and set it to the AppUser
            string pbarEmployeeID = this.AppUser.PBAREmployeeID;
            string pbarSecurityCode = String.Empty;
            if( !( pbarEmployeeID == null || pbarEmployeeID.Equals( String.Empty ) ) )
            {
                ISecurityCodeBroker securityCodeBroker = 
                    BrokerFactory.BrokerOfType< ISecurityCodeBroker >() ;
                pbarSecurityCode = securityCodeBroker.GetPrintedSecurityCode( pbarEmployeeID, account.Facility );
            }
            this.AppUser.PBARSecurityCode = pbarSecurityCode; 

            ArrayList sqlStatements  = new ArrayList();

            this.ReOrderInsurances();

            SqlBuilderStrategy[] strategies = this.InsertStrategies;

            foreach( SqlBuilderStrategy sqlBuildStrategy in strategies )
            {
                if( sqlBuildStrategy != null )
                {
                    sqlStatements = sqlBuildStrategy.BuildSqlFrom(
                        this.Account, null );
                    foreach(string aSqlStatement in sqlStatements)
                    {
                        this.Add( aSqlStatement );
                    }
                }
            }
        }
        /// <summary>
        /// This methods provides a 'hook' to perform after txn things.
        /// </summary>
        public virtual void AfterTxn()
        {
        }

        public void WriteFUSNotesForAccount()
        {
            foreach( Coverage coverage in this.Account.Insurance.Coverages )
            {
                Coverage originalCoverage = this.Account.Insurance.OrigCoverageFor( coverage.CoverageOrder );

                // If the current coverage type has changed from the original
                // coverage type, pass in null for Original coverage
                if( originalCoverage == null ||
                    originalCoverage != null && originalCoverage.GetType() != coverage.GetType() )
                {
                    string coverageType = coverage.GetType().ToString();

                    switch( coverageType )
                    {
                        case COMMERCIAL_COVERAGE:
                            originalCoverage = new CommercialCoverage();
                            break;
                        case MEDICAID_COVERAGE:
                            originalCoverage = new GovernmentMedicaidCoverage();
                            break;
                        case MEDICARE_COVERAGE:
                            originalCoverage = new GovernmentMedicareCoverage();
                            break;
                        case GOV_OTHER_COVERAGE:
                            originalCoverage = new GovernmentOtherCoverage();
                            break;
                        case OTHER_COVERAGE:
                            originalCoverage = new OtherCoverage();
                            break;
                        case SELFPAY_COVERAGE:
                            originalCoverage = new SelfPayCoverage();
                            break;
                        case WORKERS_COMP_COVERAGE:
                            originalCoverage = new WorkersCompensationCoverage();
                            break;
                    }
                }

                coverage.InsertFusNotesInto( this.Account, originalCoverage );
            }

            this.Account.Payment.IsCurrentAccountPayment = true;
            this.InsertFinancialFUSNotesInto( this.Account );
            this.InsertPrivateRoomConditionCodeInto( this.Account );
            
            //this  will be uncommented for the July release.
            this.InsertConditionOfServiceSignedFusNote( this.Account );            
            this.InsertOptOutFusNote( this.Account );            
            this.InsertNppSignedFusNote( this.Account );
            this.InsertNameChangeFusNote( this.Account );
            this.InsertValuablesCollectedFusNote( this.Account );            
            this.InsertIRATAinto( this.Account );
            this.InsertRNPCAinto( this.Account );
            this.InsertCREFPInto( this.Account );
            this.InsertRUPPDinto( this.Account );
            this.InsertRESRDinto(this.Account);  
            this.InsertRightToRestrictFusNote(this.Account);
            InsertRGAPMFusNote( Account );
                
            //this  will be uncommented for the July release.
            IFUSNoteBroker fusNoteBroker = BrokerFactory.BrokerOfType< IFUSNoteBroker >() ;
            fusNoteBroker.WriteFUSNotes(this.Account, this.AppUser.PBAREmployeeID);

            ArrayList accountsWithPaymentsMade = ( ArrayList )this.Account.Patient.AccountsWithNoPaymentPlanWithPayments;
            if( accountsWithPaymentsMade.Count > 0 )
            {
                FusNoteFactory fac = new FusNoteFactory();
                foreach( Account previousAccount in accountsWithPaymentsMade )
                {
                    previousAccount.Payment.IsCurrentAccountPayment = false;
                    if( previousAccount.Payment != null )
                    {
                        // When a payment is made for a previous account
                        fac.AddRPFCRNoteTo( previousAccount );
                    }

                    fusNoteBroker.WriteFUSNotes( previousAccount, this.AppUser.PBAREmployeeID );
                }
            }
        }

        private void InsertOptOutFusNote( Account account )
        {

            if( ( account.OptOutHealthInformation && account.HasChangedFor( "OptOutHealthInformation" ) ) || 
                ( account.OptOutLocation && account.HasChangedFor( "OptOutLocation" ) ) || 
                ( account.OptOutName && account.HasChangedFor( "OptOutName" ) ) || 
                ( account.OptOutReligion && account.HasChangedFor( "OptOutReligion" ) ) )
            {
                
                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddIHIPPFUSNoteTo( account );

            }//if

        }

        public void InsertNppSignedFusNote( Account account )
        {

            if( account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.IsSignedStatus() && 
                account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate!=DateTime.MinValue &&
                ( account.Patient.NoticeOfPrivacyPracticeDocument.HasChangedFor( "SignatureStatus" ) ||
                  account.Patient.NoticeOfPrivacyPracticeDocument.HasChangedFor( "SignedOnDate" ) ) )
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddINPOFFUSNoteTo( account );

            }//if

        }

        private void InsertNameChangeFusNote( Account account )
        {

            if( !account.Patient.IsNew &&
                ( account.Patient.HasChangedFor( "FirstName" ) ||
                  account.Patient.HasChangedFor( "LastName" ) ||
                  account.Patient.HasChangedFor( "Name" ) ) )
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddRYCHNFUSNoteTo( account );

            }//if

        }
        private void InsertRightToRestrictFusNote(Account account)
        {

            if (account.HasChangedFor("RightToRestrict") &&
                account.RightToRestrict.Code == YesNoFlag.CODE_YES)
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddRPRESFUSNoteTo(account);

            }//if

        }

         private void InsertRGAPMFusNote(Account account)
        {
            ContactPoint cp = account.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
             if (cp != null &&
                 cp.CellPhoneConsent != null &&
                 cp.HasChangedFor("CellPhoneConsent") && 
                 !cp.CellPhoneConsent.Code.Trim().Equals(string.Empty) &&
                 account.OldGuarantorCellPhoneConsent != cp.CellPhoneConsent)
             {
                 var fusNoteFactory = new FusNoteFactory();
                 fusNoteFactory.AddRGAPMFusNoteTo(account);
             }
        }

        private void InsertValuablesCollectedFusNote( Account account )
        {

            if( account.HasChangedFor( "ValuablesAreTaken" ) &&
                account.ValuablesAreTaken.Code == YesNoFlag.CODE_YES )
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddRSSVSFUSNoteTo( account );

            }//if

        }


        public void InsertConditionOfServiceSignedFusNote( Account account )
        {

            if( !account.HasChangedFor( "COSSigned" ) || account.COSSigned.Code.Trim().Equals( string.Empty ) )
            {
                return;
            }
            if( account.COSSigned.Code.Equals( ConditionOfService.YES ) )
            {
                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddICOSCFUSNoteTo( account );
            } //if yes
            else
            {
                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddIICOSFUSNoteTo( account );
                if( account.COSSigned.Code.Equals( ConditionOfService.UNABLE ) )
                {
                    fusNoteFactory.AddRPUTSFUSNoteTo( account );

                }
                else if( account.COSSigned.Code.Equals( ConditionOfService.REFUSED ) )
                {
                    fusNoteFactory.AddRPRTSFUSNoteTo( account );

                }
                else if( account.COSSigned.Code.Equals( ConditionOfService.NOT_AVAILABLE ) )
                {
                    fusNoteFactory.AddRPNASFUSNoteTo( account );
                }
            } //else if

        }
 
        public void InsertFinancialFUSNotesInto( Account account )
        {
            FusNoteFactory fac = new FusNoteFactory();
            decimal totalCurrentAmountDue = account.TotalCurrentAmtDue;
            decimal previousTotalCurrentAmountDue = account.PreviousTotalCurrentAmtDue;
            Payment payment = account.Payment;

            if( payment != null )
            {
                decimal totalPaid = account.TotalPaid;
                decimal amountCollectedToday = payment.CalculateTotalPayments();
                decimal previousTotalPaid = totalPaid - amountCollectedToday;
                decimal balanceDue = account.BalanceDue;

                if( payment.IsCurrentAccountPayment )
                {
                    // When a payment is made for an account
                   if (!account.BillHasDropped && ((account.OriginalMonthlyPayment != account.MonthlyPayment) ||
                                                    (totalPaid != previousTotalPaid) ||
                                                    (previousTotalCurrentAmountDue != totalCurrentAmountDue) ||
                                                    (totalCurrentAmountDue != 0 &&
                                                     (balanceDue == 0M || balanceDue == 0.00M))) ||
                        account.HasChangedFor("DayOfMonthPaymentDue"))
                    {
                        fac.AddRPFCRNoteTo(account);
                    }

                    // When Total Monthly Due for an account has changed
                   if (!account.BillHasDropped &&
                       account.OriginalMonthlyPayment != account.MonthlyPayment &&
                       account.MonthlyPayment != 0 && account.NumberOfMonthlyPayments > 3)
                   {
                       fac.AddRFCMONoteTo(account);
                   }

                    // When Total Amount Collected for an account has changed
                    if( totalPaid != previousTotalPaid )
                    {
                        fac.AddRFCACNoteTo( account );
                    }

                    // When Total Current Amount Due for an account has changed
                    if( previousTotalCurrentAmountDue != totalCurrentAmountDue )
                    {
                        fac.AddRCALCNoteTo( account );
                    }
                }
            }

            // When 'Patient has No Liability' check box is checked on Liability screen
            // (or)
            // When Total Current Amount Due has become zero
            if ( ( account.Insurance.HasChangedFor( "HasNoLiability" ) && account.Insurance.HasNoLiability )
                    ||
                 ( account.HasChangedFor( "TotalCurrentAmtDue" ) && account.TotalCurrentAmtDue == 0.00M ) )
            {
                fac.AddRFCNLNoteTo( account );
            }
        }

        /// <summary>
        /// InsertPrivateRoomConditionCode
        /// </summary>
        /// <param name="account"></param>
        public void InsertPrivateRoomConditionCodeInto( Account account )
        {
            if( account.Diagnosis != null && account.Diagnosis.isPrivateAccommodationRequested )
            {
                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddPTRPRFUSNoteTo(account);
            } 
        }

        /// <summary>
        /// InsertCRFEPInto
        /// </summary>
        /// <param name="account"></param>
        private void InsertCREFPInto( Account account )
        {

            if( account.Payment != null &&
                account.TotalCurrentAmtDue > 0M && 
                account.Payment.ZeroPaymentReason.Description.Equals( REFUSED_TO_PAY, StringComparison.InvariantCultureIgnoreCase ) &&
                account.Payment.ZeroPaymentReason.HasChangedFor( "Description" ) )
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddCREFPFUSNote( account );

            }

        }

        /// <summary>
        /// InsertRUPPDinto
        /// </summary>InsertRUPPDinto
        /// <param name="account"></param>

        private void InsertRUPPDinto( Account account )
        {

            decimal totalCurrentAmountDue = account.TotalCurrentAmtDue;
            SelfPayCoverage primaryCoverage = account.Insurance.PrimaryCoverage as SelfPayCoverage;

            if( totalCurrentAmountDue > 0 && 
                primaryCoverage != null &&
                ( account.Insurance.HasChangedFor( "PrimaryCoverage" ) ||
                  account.HasChangedFor( "TotalCurrentAmtDue" ) ) )
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddRUPPDFUSNote( account );

            }//if         
           
        }
        private void InsertRESRDinto(Account account)
        {
            if (account != null && account.MedicareSecondaryPayor != null)
            {
                var msp = account.MedicareSecondaryPayor;
                if (msp.MedicareEntitlement != null && msp.MedicareEntitlement.GetType() == typeof(ESRDEntitlement))
                {
                    var esrdEntitlement = (ESRDEntitlement) msp.MedicareEntitlement;
                    if (esrdEntitlement.DialysisTreatment != null &&
                        ( esrdEntitlement.HasChangedFor("DialysisDate") 
                         ||  esrdEntitlement.HasChangedFor("DialysisCenterName") 
                          || ( account.IsNew && !string.IsNullOrEmpty(esrdEntitlement.DialysisCenterName) )) &&
                        esrdEntitlement.DialysisTreatment.Code == YesNoFlag.CODE_YES) 
                    {
                        FusNoteFactory fusNoteFactory = new FusNoteFactory();
                        fusNoteFactory.AddRESRDFUSNote(account);
                    }
                }
            }
        }


        private void InsertRNPCAinto( Account account )
        {

            Coverage primaryCoverage = account.Insurance.PrimaryCoverage;
            CoverageGroup primaryCoverageGroup = primaryCoverage as CoverageGroup;
            bool shouldGenerateFusNote = false;

            if( primaryCoverage == null ) 
            {

                return; 

            }//if

            // Coverage is a coveragegroup subclass, so get the information from the Auhtorization property
            if( primaryCoverageGroup != null )
            {

                if( primaryCoverageGroup.Authorization.AuthorizationRequired.IsNotApplicable &&
                    ( account.Insurance.HasChangedFor( "PrimaryCoverage" ) ||
                      primaryCoverageGroup.Authorization.HasChangedFor( "AuthorizationRequired" ) ) )
                {

                    shouldGenerateFusNote = true;

                }//if

            }//if
            
            // Coverage does not have an authorization field, so the value is N/A be default. If the coverage
            // has changed, then create the FUS note
            else
            {

                if( account.Insurance.HasChangedFor( "PrimaryCoverage" ) )
                {

                    shouldGenerateFusNote = true;
                    
                }//if

            }//else

            if( shouldGenerateFusNote )
            {

                FusNoteFactory fusNoteFactory = new FusNoteFactory();
                fusNoteFactory.AddRNPCAFUSNote( account );
                
            }//if

        }

        private void InsertIRATAinto( Account account )
        {

            CoverageGroup primaryCoverage = 
                account.Insurance.PrimaryCoverage as CoverageGroup;
                
            if( primaryCoverage != null )
            {

                if( primaryCoverage.Authorization.AuthorizationStatus.Code.Equals( "A" ) &&
                    primaryCoverage.Authorization.HasChangedFor( "AuthorizationStatus" ) )
                {
                    FusNoteFactory fusNoteFactory = new FusNoteFactory();
                    fusNoteFactory.AddIRATAFUSNote( account );
                }
                

            }
            else
            {
                var medicareCoverage =
                    account.Insurance.PrimaryCoverage as GovernmentMedicareCoverage;

                if (medicareCoverage != null && medicareCoverage.IsMedicareCoverageValidForAuthorization)
                {

                    if (medicareCoverage.Authorization.AuthorizationStatus.Code.Equals("A") &&
                        medicareCoverage.Authorization.HasChangedFor("AuthorizationStatus"))
                    {
                        FusNoteFactory fusNoteFactory = new FusNoteFactory();
                        fusNoteFactory.AddIRATAFUSNote(account);
                    }
                }
            }

        }
    
        #endregion

        #region Properties
        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        public Account AccountTwo
        {
            get
            {
                return i_AccountTwo;
            }
            set
            {
                i_AccountTwo = value;
            }
        }
        public abstract SqlBuilderStrategy[] InsertStrategies
        {
            get;
            set;
        }

        public ArrayList SqlStatements
        {
            get
            {
                return i_SqlStatements;
            }
        }

       
        public virtual int NumberOfInsurances
        {
            get
            {
                if( ( i_Account.Insurance != null ) && 
                    ( i_Account.Insurance.Coverages != null ) )
                {
                    i_NumberOfInsurances = i_Account.Insurance.Coverages.Count;
                }
                if( i_NumberOfInsurances == 1 && 
                    i_Account.IsNew == false &&
                    i_Account.DeletedSecondaryCoverage != null)
                {
                    i_NumberOfInsurances++;
                }
                return i_NumberOfInsurances;
            }
            set
            {
                i_NumberOfInsurances = value;
            }
        }

        public int NumberOfNonInsurances
        {
            get
            {
                return i_NumberOfNonInsurances;
            }
            set
            {
                i_NumberOfNonInsurances = value;
            }
        }       
 
        public int NumberOfOtherRecs
        {
            get
            {
                return i_NumberOfOtherRecs;
            }
            set
            {
                i_NumberOfOtherRecs = value;
            }
        }
        public bool IsTransactionHeaderRequired
        {
            get
            {
                return i_IsTransactionHeaderRequired;
            }
            set
            {
                i_IsTransactionHeaderRequired = value;
            }
        }
        public User AppUser
        {
            get
            {
                return i_User;
            }
            set
            {
                i_User = value;
            }
        }
        
        #endregion

        #region Private Methods
        public void ReOrderInsurances()
        {
            if( this.Account.IsNew || Account.Insurance.Coverages.Count == 2 )
            {
                // if this is new account just use the primary coverages.
                // ignore a deleted coverage since we don't really have 
                // delete it.
                // OR
                // if there are already 2 coverages in the list just use them
                // and ignore a deleted one if it exists.
                for( int i = 1; i <= Account.Insurance.Coverages.Count; i++ )
                {
                    Coverage coverage = Account.Insurance.CoverageFor(i);
                    coverage.Priority = i;
                }
                return;
            }
            else if ( Account.Insurance.Coverages.Count == 1 )
            {
                Coverage primageCoverage = Account.Insurance.CoverageFor(1);
                primageCoverage.Priority = 1;

                if( Account.DeletedSecondaryCoverage != null )
                {
                    Account.DeletedSecondaryCoverage.Priority = 0;
                    Account.Insurance.AddCoverage(Account.DeletedSecondaryCoverage);
                }
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TransactionCoordinator( Account anAccount )
        {
            i_Account = anAccount;
        }

        public TransactionCoordinator( Account anAccountOne, Account anAccountTwo )
        {
            i_Account = anAccountOne;
            i_AccountTwo = anAccountTwo;
        }

        public TransactionCoordinator( User user )
        {
            i_User = user;
        }

        public TransactionCoordinator()
        {
        }

        #endregion

        #region Data Elements
        private Account i_Account;
        private Account i_AccountTwo;
        private ArrayList i_SqlStatements = new ArrayList();
        private int i_NumberOfInsurances= 0;
        private int i_NumberOfNonInsurances = 0;
        private int i_NumberOfOtherRecs = 0;
        private User i_User;
        private bool i_IsTransactionHeaderRequired = true;
        #endregion

        #region Constants

        protected const string LAST_TRANSACTION_IN_GROUP_FLAG = "Y";

        private const string
            COMMERCIAL_COVERAGE     = "PatientAccess.Domain.CommercialCoverage",
            MEDICAID_COVERAGE       = "PatientAccess.Domain.GovernmentMedicaidCoverage",
            MEDICARE_COVERAGE       = "PatientAccess.Domain.GovernmentMedicareCoverage",
            GOV_OTHER_COVERAGE      = "PatientAccess.Domain.GovernmentOtherCoverage",
            OTHER_COVERAGE          = "PatientAccess.Domain.OtherCoverage",
            SELFPAY_COVERAGE        = "PatientAccess.Domain.SelfPayCoverage",
            WORKERS_COMP_COVERAGE   = "PatientAccess.Domain.WorkersCompensationCoverage",
            REFUSED_TO_PAY          = "REFUSED TO PAY";
        private const bool isFalse = false;
        #endregion
    }
}
