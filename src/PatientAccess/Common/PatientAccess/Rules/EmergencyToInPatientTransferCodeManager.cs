using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Utilities;

namespace PatientAccess.Rules
{
    /// <summary>
    /// The EmergencyToInPatientTransferCodeManager class handles all scenarios for adding
    /// or removing P7 condition code from an account or filtering the master condition codes 
    /// depending on the Feature Start Date, Admit Date, Patient Type and Activity.
    /// 
    /// P7 condition code is generated when a Patient is transferred to Inpatient from 
    /// being an Emergency Patient ever.
    ///  </summary>

    [Serializable]
    public class EmergencyToInPatientTransferCodeManager
    {
        private IConditionCodeBroker ConditionCodeBroker { get; set; }
        private ConditionCode P7ConditionCode { get; set; }

        private IAccountBroker AccountBroker { get; set; }

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="EmergencyToInPatientTransferCodeManager"/> class.
        /// </summary>
        /// <param name="featureStartDate">The feature start date.</param>
        /// <param name="account">The account.</param>
        /// <param name="accountBroker"></param>
        /// <param name="conditionCodeBroker"></param>
        /// <exception cref="ArgumentNullException"><paramref name="account" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="accountBroker" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="conditionCodeBroker" /> is <c>null</c>.</exception>
        public EmergencyToInPatientTransferCodeManager( DateTime featureStartDate, Account account, IAccountBroker accountBroker, IConditionCodeBroker conditionCodeBroker)
        {
            Guard.ThrowIfArgumentIsNull( account, "account" );
            Guard.ThrowIfArgumentIsNull( accountBroker, "accountBroker" );
            Guard.ThrowIfArgumentIsNull( conditionCodeBroker, "conditionCodeBroker" );
            
            Debug.Assert(account.Facility != null);
//            Debug.Assert(account.KindOfVisit != null);

            FeatureStartDate = featureStartDate;
            Account = account;
            AccountBroker = accountBroker;
            ConditionCodeBroker = conditionCodeBroker;
             
            //this should be move a sperate initialize method if the constructor call gets too slow
            P7ConditionCode = GetP7ConditionCode();
        }

        /// <summary>
        /// Filters Condition Codes list to include or exclude condition code P7 by
        /// determining whether Condition Code P7 for Transfer ER to IP is enabled
        /// for date [the specified admit date >= Feature Start Date] and Patient Type.
        /// </summary>
        /// <param name="inputCodes"></param>
        /// <returns>
        /// 	Filtered condition code list.
        /// </returns>
        public IEnumerable<ConditionCode> FilterConditionCodesMasterList( IEnumerable<ConditionCode> inputCodes )
        {
            Guard.ThrowIfArgumentIsNull( inputCodes, "inputCodes" );

            if ( ShouldCodeBeRemoved() )
            {
                return RemoveCode( inputCodes );
            }

            return inputCodes;
        }

        /// <summary>
        /// This method will be the entry point for P7 condition code related logic
        /// so that the views do not have to manipulate condition codes directly.
        /// 
        /// Depending on the activity being performed on the account, this method either removes
        /// or adds P7 Condition code based on Admit Date, Feature Start Date and Patient Type.
        ///  </summary>
        public void UpdateConditionCodes()
        {
            if ( Account.Activity == null ) return;

            // Auto-generate Condition Code P7 when ER patient is transferred to Inpatient
            if ( Account.Activity is TransferOutToInActivity )
            {
                WasEmergencyPatient = AccountBroker.WasAccountEverAnERType( Account );
                AutoGenerateConditionCodeOnTransfer();
                return;
            }

            // Remove Condition Code P7 when Inpatient is transferred to OutPatient type
            if ( Account.Activity is TransferInToOutActivity )
            {
                RemoveConditionCodeOnTransfer();
                return;
            }

            // Remove previously selected P7 ConditionCode if selected patient type is not Inpatient.
            RemoveConditionCodeOnPatientTypeChange();
        }

        /// <summary>
        /// This method determines whether Condition Code P7 should be added on the account. P7 can
        /// be added if the specified admit date >= Feature Start Date and the account is currently
        /// an Emergency Patient or was once an Emergency Patient during the account life cycle.
        /// </summary>
        private bool ShouldAddP7ConditionCode()
        {
            return ( WasEmergencyPatient && FeatureIsValidForAdmitDate( Account.AdmitDate ) );
        }

        /// <summary>
        /// This method determines whether Condition Code P7 should be removed from the account. 
        /// P7 should be removed if the admit date is less than the Feature Start Date and the 
        /// account is currently not an Inpatient.
        /// </summary>
        /// <returns> boolean </returns>
        internal bool ShouldCodeBeRemoved()
        {
            var patientType = Account.KindOfVisit;

            return patientType == null ||
                   ( String.IsNullOrEmpty( patientType.Code ) ||
                     !patientType.IsInpatient ||
                     !FeatureIsValidForAdmitDate( Account.AdmitDate ) );
        }

        /// <summary>
        /// This method removes previously selected P7 ConditionCode from the
        /// account if selected patient type on the account is not Inpatient.
        /// </summary>
        internal void RemoveConditionCodeOnPatientTypeChange()
        {
            if ( Account.KindOfVisit != null && Account.KindOfVisit.IsInpatient ) return;

            var codesToRemove = Account.ConditionCodes.Cast<ConditionCode>().ToList().Where( x => x.IsEmergencyToInpatientTransferConditionCode() ).ToList();

            codesToRemove.ForEach(x=>  Account.RemoveConditionCode( x ));
        }

        #endregion

        #region Properties

        private DateTime FeatureStartDate { get; set; }
        public Account Account { get; private set; }
        private bool WasEmergencyPatient { get; set; }

        #endregion

        #region Private Methods

        // Auto-generate Condition Code P7 when ER patient is transferred to Inpatient
        private void AutoGenerateConditionCodeOnTransfer()
        {
            if ( ShouldAddP7ConditionCode() )
            {
                Account.AddConditionCode( P7ConditionCode );
            }
        }

        // Remove Condition Code P7 when Inpatient is transferred to any other patient type
        private void RemoveConditionCodeOnTransfer()
        {
           Account.RemoveConditionCode( P7ConditionCode );
        }

        private ConditionCode GetP7ConditionCode()
        {
            return ConditionCodeBroker.ConditionCodeWith(
                Account.Facility.Oid, ConditionCode.ADMITTED_AS_IP_FROM_ER );
        }

        private bool FeatureIsValidForAdmitDate( DateTime admitDate )
        {
            return ( admitDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) ||
                   ( admitDate >= FeatureStartDate );
        }

        private static IEnumerable<ConditionCode> RemoveCode( IEnumerable<ConditionCode> inputCodes )
        {
            return inputCodes.Where( x => !x.IsEmergencyToInpatientTransferConditionCode() ).ToList();
        }

        #endregion
    }
}
