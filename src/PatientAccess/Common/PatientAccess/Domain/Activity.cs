using System;
using System.Collections; 
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using Peradigm.Framework.Domain.Exceptions;

namespace PatientAccess.Domain
{
    [Serializable]
    public abstract class Activity : ReferenceValue, IRule
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// Determines whether this instance can accept the specified span code.
        /// </summary>
        /// <param name="spanCode">The span code.</param>
        /// <param name="visitType">Type of the visit.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can accept the specified span code; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanAccept( SpanCode spanCode, VisitType visitType )
        {
            
            bool canAccept = false;

            // Rule: This defines the maximum set of spancodes
            if( spanCode.IsPriorStayDates || spanCode.IsQualifyingStayDate || spanCode.IsNoncoveredLevelOfCare )
            {

                // Apply more specific rules
                canAccept = 
                    // Rule: This is an inpatient visit or a pre-regsitration visit
                    ( visitType.IsInpatient || 
                            ( visitType.IsPreRegistrationPatient && this.AssociatedActivityType==null) ||
                            ( visitType.IsPreRegistrationPatient && !this.AssociatedActivityType.Equals( typeof( ActivatePreRegistrationActivity ) ) ) ) ||
                    
                    // Rule: This is an outpatient,ER, or recurring patient and the span code is NonCoveredLevel of Care (74)
                    ( ( visitType.IsOutpatient || visitType.IsEmergencyPatient || visitType.IsRecurringPatient ) && spanCode.IsNoncoveredLevelOfCare );

            }

            return canAccept;

        }

        public ArrayList PreConditions()
        {
            return null;
        }
       
        public ArrayList ProcessRules()
        {
            return null;
        }

        public void ApplyTo( Object obj )
        {
            return;
        }

        public bool CanBeAppliedTo ( Object obj )
        {
            return false;
        }

        public bool ShouldStopProcessing()
        {
            return false;
        }  

        public abstract bool ReadOnlyAccount();

        public virtual bool CanCreateNewPatient()
        {
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is subject to HSV plan financial class restriction.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is subject to HSV plan financial class restriction; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSubjectToHSVPlanFinancialClassRestriction
        {

            get
            {

                return false;

            }

        }
        public bool IsActivatePreRegisterActivity()
        {
            if (this.AssociatedActivityType != null
                      && this.AssociatedActivityType == typeof(ActivatePreRegistrationActivity))
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// Determines whether [is admit newborn activity].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is admit newborn activity]; otherwise, <c>false</c>.
        /// </returns>
        public  bool IsAdmitNewbornActivity()
        {
            return this.GetType().Equals(typeof(AdmitNewbornActivity));
        }

        public bool IsPreAdmitNewbornActivity()
        {
            return this.GetType().Equals( typeof( PreAdmitNewbornActivity ) );
        }

        public bool IsPreMSEActivities()
        {
            return (GetType().Equals(typeof (PreMSERegisterActivity)) ||
                    GetType().Equals(typeof (PreMSERegistrationWithOfflineActivity)) ||
                    GetType().Equals(typeof(EditPreMseActivity))
                                    );
        }

        public bool IsEditPreMseActivity()
        {
            return (GetType().Equals(typeof (EditPreMseActivity)));
        }

        public bool IsPostMSEActivity()
        {
            return (GetType().Equals(typeof (PostMSERegistrationActivity)));
        }

        public bool IsUCCPreMSEActivity()
        {
            return (GetType() == typeof (UCCPreMSERegistrationActivity) || 
                    (GetType() == typeof (MaintenanceActivity) && 
                     AssociatedActivityType != null && 
                     AssociatedActivityType.GetType() == typeof(UCCPreMSERegistrationActivity)));
        }
        public bool IsUccPostMSEActivity()
        {
            return (GetType().Equals(typeof (UCCPostMseRegistrationActivity)));
        }
        public bool IsEditUCCPreMSEActivity()
        {
           
                return (GetType() == typeof (UCCPreMSERegistrationActivity) &&
                        AssociatedActivityType != null &&
                        AssociatedActivityType == typeof (EditUCCPreMSEActivity));
             
        }

        public bool IsCreateUCCPreMSEActivity()
        {
            return (GetType() == typeof (UCCPreMSERegistrationActivity) &&
                    (AssociatedActivityType == null ||
                     (AssociatedActivityType != null && AssociatedActivityType != typeof (EditUCCPreMSEActivity))));

        }

        public bool IsNewBornRelatedActivity()
        {
            return GetType().Equals(typeof (AdmitNewbornActivity)) //Register Newborn or Activate Pre-Admit Newborn
                   || GetType().Equals(typeof (PreAdmitNewbornActivity)) //Pre-Admit Newborn
                   || (GetType().Equals(typeof (MaintenanceActivity)) 
                        && AssociatedActivityType != null && AssociatedActivityType == typeof (PreAdmitNewbornActivity));//Edit Pre-Admit Newborn
        }

        public bool IsRegistrationActivity()
        {
            return this.GetType().Equals( typeof( RegistrationActivity ) );
        }
        public bool IsMaintenanceActivity()
        {
            return this.GetType().Equals( typeof( MaintenanceActivity ) );
        }
        public bool IsShortPreRegistrationActivity()
        {
            return this.GetType().Equals( typeof( ShortPreRegistrationActivity ) );
        }
        public bool IsShortMaintenanceActivity()
        {
            return this.GetType().Equals( typeof( ShortMaintenanceActivity ) );
        }
        public bool IsQuickAccountCreationActivity()
        {
            return this.GetType().Equals(typeof(QuickAccountCreationActivity));
        }
        public bool IsQuickAccountMaintenanceActivity()
        {
            return this.GetType().Equals(typeof(QuickAccountMaintenanceActivity));
        }

        public bool IsTransferERToOutpatientActivity()
        {
            return this.GetType().Equals(typeof(TransferERToOutpatientActivity));
        }

        public bool IsTransferOutpatientToERActivity()
        {
            return this.GetType().Equals(typeof(TransferOutpatientToERActivity));
        }

        public bool IsTransferOutToInActivity()
        {
            return this.GetType().Equals(typeof(TransferOutToInActivity));
        }

        public bool IsPAIWalkinOutpatientCreationActivity()
        {
            return this.GetType().Equals(typeof( PAIWalkinOutpatientCreationActivity ));
        }

        public bool IsValidForUpdateFromEMPI
        {
            get { return ValidActivityForEMPIUpdate && ValidAssociatedActivityForEMPIFeature; }
        }

        private bool ValidActivityForEMPIUpdate
        {
            get
            {
                return GetType() == typeof (PreRegistrationActivity) ||
                       GetType() == typeof (RegistrationActivity) ||
                       GetType() == typeof (ShortRegistrationActivity) ||
                       GetType() == typeof (ShortPreRegistrationActivity) ||
                       GetType() == typeof (QuickAccountCreationActivity) ||
                       GetType() == typeof (PAIWalkinOutpatientCreationActivity) ||
                       GetType() == typeof (PreMSERegisterActivity) ||
                        GetType() == typeof(UCCPreMSERegistrationActivity) ||
                       GetType() == typeof (AdmitNewbornActivity) ||
                       GetType() == typeof (PreAdmitNewbornActivity) ||
                       GetType() == typeof (ActivatePreRegistrationActivity);
            }
        }

        private bool ValidActivityForEMPISearch
        {
            get
            {
                return GetType() == typeof (PreRegistrationActivity) ||
                       GetType() == typeof (RegistrationActivity) ||

                       GetType() == typeof (ShortRegistrationActivity) ||
                       GetType() == typeof (ShortPreRegistrationActivity) ||

                       GetType() == typeof (QuickAccountCreationActivity) ||
                       GetType() == typeof (PAIWalkinOutpatientCreationActivity) ||

                       GetType() == typeof (PreMSERegisterActivity) ||
                        GetType() == typeof(UCCPreMSERegistrationActivity) ||

                       GetType() == typeof (ActivatePreRegistrationActivity);
            }

        }

        private bool ValidAssociatedActivityForEMPIFeature
        {
            get
            {
                return !(AssociatedActivityType != null &&
                        (AssociatedActivityType == typeof (PreAdmitNewbornWithOfflineActivity) ||
                         AssociatedActivityType == typeof (AdmitNewbornWithOfflineActivity) ||

                         AssociatedActivityType == typeof (PreMSERegistrationWithOfflineActivity) ||
                         AssociatedActivityType == typeof (PreRegistrationWithOfflineActivity) ||

                         AssociatedActivityType == typeof (RegistrationWithOfflineActivity) ||

                         AssociatedActivityType == typeof (ShortPreRegistrationWithOfflineActivity) ||
                         AssociatedActivityType == typeof (ShortRegistrationWithOfflineActivity)));
            }

        }

        public bool IsValidRegistrationActivityForDischargeDisposition
        {
            get
            {
                return (GetType() == typeof(RegistrationActivity)
                        || GetType() == typeof(RegistrationWithOfflineActivity)

                        || GetType() == typeof(ActivatePreRegistrationActivity)

                        || GetType() == typeof(CancelPreRegActivity)
                        || GetType() == typeof(CancelInpatientStatusActivity)


                        || GetType() == typeof(PreAdmitNewbornActivity)
                        || GetType() == typeof(PreAdmitNewbornWithOfflineActivity)
                        || GetType() == typeof(AdmitNewbornActivity)
                        || GetType() == typeof(AdmitNewbornWithOfflineActivity)

                        || GetType() == typeof(PreRegistrationActivity)
                        || GetType() == typeof(PreRegistrationWithOfflineActivity)

                        || GetType() == typeof(QuickAccountCreationActivity)
                        || GetType() == typeof(QuickAccountMaintenanceActivity)

                        || GetType() == typeof(PAIWalkinOutpatientCreationActivity) 

                        || GetType() == typeof(ShortPreRegistrationActivity)
                        || GetType() == typeof(ShortRegistrationActivity)
                        || GetType() == typeof(ShortMaintenanceActivity)
                        || GetType() == typeof(ShortPreRegistrationWithOfflineActivity)
                        || GetType() == typeof(ShortRegistrationWithOfflineActivity)

                        || GetType() == typeof(EditPreMseActivity)
                        || GetType() == typeof(PreMSERegisterActivity)
                        || GetType() == typeof(PreMSERegistrationWithOfflineActivity)

                        || GetType() == typeof(PostMSERegistrationActivity)

                        || this.IsUCCPreMSEActivity()
                        || this.IsUccPostMSEActivity()
                        || this.IsEditUCCPreMSEActivity()

                        || GetType() == typeof(EditAccountActivity)
                        || GetType() == typeof(MaintenanceActivity)

                );
            }
        }

        public bool IsValidForEMPISearch
        {
            get { return ValidActivityForEMPISearch && ValidAssociatedActivityForEMPIFeature; }
        }

        //Patient Type can change for all Activities. Overridden to return false for activities 
        //which cannot allow Patient Type to change.(AdmitNewBorn,PostMSERegistration, PreRegistration).
        public virtual bool CanPatientTypeChange()
        {
            return true;
        }
        public bool IsDiagnosticRegistrationActivity()
        {
            return (GetType() == typeof(ShortRegistrationActivity)
                || GetType() == typeof(ShortRegistrationWithOfflineActivity));
        }

        public bool IsDiagnosticPreRegistrationActivity()
        {
            return GetType() == typeof(ShortPreRegistrationActivity)
                || GetType() == typeof(ShortPreRegistrationWithOfflineActivity);
        }

        public bool IsPreRegistrationActivity()
        {
            return GetType() == typeof (PreRegistrationActivity)
                || GetType() == typeof(PreRegistrationWithOfflineActivity);
        }
        
        /// <summary>
        /// Create a copy of the provided account using rules appropriate to the activity.
        /// </summary>
        /// <param name="existingAccount"></param>
        /// <returns></returns>
        public IAccount CreateAccountForActivityFrom( IAccount existingAccount )
        {
            
            if( existingAccount == null )
            {
                throw new ArgumentNullException("existingAccount");
            }

            // Sub classes should override the ExecuteAccountCopyStrategy method. This
            // preserves the call contract behavior for this method.
            IAccount newAccount = ExecuteAccountCopyStrategy(existingAccount);

            if( newAccount == null )
            {
                throw new EnterpriseException( "Unable to create copy of account", 
                                                Severity.Catastrophic );
            }

            return newAccount;

        }

        /// <summary>
        /// Execute the strategy used to create accounts for this activity. The default
        /// is simply to perform a binary copy.
        /// </summary>
        /// <param name="existingAccount"></param>
        /// <returns></returns>
        protected virtual IAccount ExecuteAccountCopyStrategy(IAccount existingAccount)
        {

            var copyAccountBroker =
                    BrokerFactory.BrokerOfType<IAccountCopyBroker>();

            return copyAccountBroker.CreateAccountCopyFor(existingAccount);

        }

        public bool IsValidTransferActivityForEmailAddress
        {
            get { return IsTransferERToOutpatientActivity() || IsTransferOutToInActivity(); }
        }

        public bool IsValidForAutopopulatePatientName()
        {
            return (IsNewbornActivity() || IsPreAdmitNewbornActivity() || IsEditPreAdmitNewbornActivity() ||
                    IsActivatePreAdmitNewbornActivity()
                );
        }

        public bool IsValidForAdditionalRaceCodes()
        {
            return !(IsNewbornActivity() || IsPreAdmitNewbornActivity() || IsEditPreAdmitNewbornActivity() ||
                     IsActivatePreAdmitNewbornActivity() || IsPreRegistrationActivity() ||
                     IsDiagnosticPreRegistrationActivity());
        }

        public bool IsEditPreAdmitNewbornActivity()
        {
            if (IsMaintenanceActivity() &&
                AssociatedActivityType != null &&
                AssociatedActivityType == typeof(PreAdmitNewbornActivity))
            {
                return true;
            }
            return false;
        }

        public bool IsActivatePreAdmitNewbornActivity()
        {
            if (GetType() == typeof(AdmitNewbornActivity) &&
                AssociatedActivityType != null &&
                AssociatedActivityType == typeof(ActivatePreRegistrationActivity))
            {
                return true;
            }
            return false;

        }
      
        public bool IsPreMSEAndUCPreMSEAccount
        {
            get
            {
                return IsPreMSEActivities() || IsUCCPreMSEActivity();
            }
        }
        
        private bool IsNewbornActivity()
        {
            if (IsAdmitNewbornActivity() &&
                (AssociatedActivityType == null
                 || AssociatedActivityType != typeof(ActivatePreRegistrationActivity)))
            {
                return true;
            }
            return false;
        }

        public bool IsValidForAuthorizeAdditionalPortalUserFeature
        {
            get
            {
                return IsActivatePreRegisterActivity() || IsActivatePreAdmitNewbornActivity() ||
                       IsPostMSEActivity() || IsUccPostMSEActivity() || IsNewbornActivity();
            }
        }
        public bool IsDischargeActivity()
        {
            return (GetType().Equals(typeof(DischargeActivity)));
        }
        #endregion

        #region Properties
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

        public Activity AssociatedActivity { get; set; }

        public Type AssociatedActivityType
        {
            get
            {
                return i_AssociatedActivityType;
            }
            set
            {
                i_AssociatedActivityType = value;
            }
        }

	    public ActivityTimeout Timeout
	    {
	        get
	        {
		    return i_Timeout;
	        }
	        set
	        {
		    i_Timeout = value;
	        }
	    }

        public string ContextDescription
        {
            get
            {
                return i_ContextDescription;
            }
            set
            {
                i_ContextDescription = value;
            }
        }
        public EMPIPatient EmpiPatient
        {
            get
            {
                return iEmpiPatient;
            }
            set
            {
                iEmpiPatient = value;
            }
        }

        public bool IsAnyNewBornActivity
        {
            get
            {
                return (IsNewbornActivity() || IsPreAdmitNewbornActivity() || IsEditPreAdmitNewbornActivity() ||
                        IsActivatePreAdmitNewbornActivity()
                    );
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Activity(){}
        public Activity(string pbarEmployeeID, string pbarSecurityCode)
        {
            i_PBAREmployeeID = pbarEmployeeID;
            i_PBARSecurityCode = pbarSecurityCode;
        }
        #endregion

        #region Data Elements
        
        private string              i_PBAREmployeeID;
        private string              i_PBARSecurityCode;
        private User                i_User = User.GetCurrent();
        private Type                i_AssociatedActivityType;
        private ActivityTimeout     i_Timeout = new ActivityTimeout();
        private string              i_ContextDescription;
        private EMPIPatient iEmpiPatient = new EMPIPatient();

        #endregion

        #region Constants
         public const string TransferErPatientToOutPatient = "TransferERToOutpatientActivity";
         public const string TransferOutPatientToErPatient = "TransferOutpatientToERActivity";
        #endregion
    }
}

