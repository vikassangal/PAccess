using System;
using System.Collections;
using System.Diagnostics;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.DiagnosisViews
{
    public class EDLogsDisplayPresenter : IEDLogsDisplayPresenter
    {
        private bool DoNotShowEDLogFields { get; set; }

        private bool ClearEDLogFields { get; set; }

        private bool ShowDisabledEDLogFields { get; set; }

        private VisitType VisitType { get; set; }

        private IEDLogView View { get; set; }

        private Activity Activity { get; set; }

        private bool ShowEnabledEDLogFields { get; set; }

        internal IRuleEngine RuleEngine { get; private set; }

        public Account Model { get; private set; }
        private bool ShowEnabledForUCCPostMse { get; set; }

        /// <exception cref="ArgumentNullException"><c>view</c> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><c>account</c> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><c>ruleEngine</c> is <see langword="null"/>.</exception>
        public EDLogsDisplayPresenter( IEDLogView view, Account account, IRuleEngine ruleEngine )
        {
            Guard.ThrowIfArgumentIsNull( view, "view" );
            Guard.ThrowIfArgumentIsNull( account, "account" );
            Guard.ThrowIfArgumentIsNull( ruleEngine, "ruleEngine" );

            View = view;
            Model = account;
            Activity = account.Activity;
            VisitType = new VisitType();
            RuleEngine = ruleEngine;
        }

        /// <exception cref="ArgumentNullException"><c>visitType</c> is <see langword="null"/>.</exception>
        public void UpdateEDLogDisplay( VisitType visitType, bool isViewBeingUpdated, bool edLogFieldsHaveData )
        {
            Guard.ThrowIfArgumentIsNull( visitType, "visitType" );

            VisitType = visitType;

            DoNotShowEDLogFields = ShouldWeNotShowEDLogFields( edLogFieldsHaveData );

            ShowEnabledEDLogFields = ShouldWeShowEnabledEDLogFields();

            ShowDisabledEDLogFields = ShouldWeShowDisableEDLogFields( edLogFieldsHaveData );

            ShowEnabledForUCCPostMse = ShouldWeEnableEDLogForUCCPostMse();

            ClearEDLogFields = ShouldWeClearEDLogFields( isViewBeingUpdated );

            if ( DoNotShowEDLogFields )
            {
                View.DoNotShow();
            }

            else if ( ShowEnabledEDLogFields )
            {
                View.ShowEnabled();
            }

            else if ( ShowDisabledEDLogFields )
            {
                View.ShowDisabled();
            }

            else if (ShowEnabledForUCCPostMse)
            {
                View.ShowEnabledForUCCPostMse();
            }
            if ( ClearEDLogFields )
            {
                View.ClearFields();
                ClearModel();
            }

            EvaluateModeOfArrivalRequired();

            Invariant();
        }

        /// <exception cref="ArgumentNullException"><c>modeOfArrival</c> is <see langword="null"/>.</exception>
        public void UpdateSelectedModeOfArrival( ModeOfArrival modeOfArrival )
        {
            Guard.ThrowIfArgumentIsNull( modeOfArrival, "modeOfArrival" );

            Model.ModeOfArrival = modeOfArrival;
            EvaluateModeOfArrivalRequired();
        }

        public void PopulateEdLogData()
        {
            PopulateModeOfArrival();
            PopulateReferralType();
            PopulateReferralFacility();
            PopulateReadmitCode();
            DisplayArrivalTime();
        }

        public void RegisterEDLogRules()
        {
            RuleEngine.RegisterEvent( typeof( ModeOfArrivalRequired ), new EventHandler( ModeOfArrivalRequiredEventHandler ) );
        }

        public void UnRegisterEDLogRules()
        {
            RuleEngine.UnregisterEvent( typeof( ChiefComplaintRequired ), new EventHandler( ModeOfArrivalRequiredEventHandler ) );
        }

        [Conditional( "DEBUG" )]
        private void Invariant()
        {
            Debug.Assert( View != null );
            Debug.Assert( Activity != null );
            Debug.Assert( VisitType != null );

            bool moreThanOneOperationCanbeExecuted = BoolHelper.TrueCountExceedsThreshold( 1, DoNotShowEDLogFields, ShowDisabledEDLogFields, ShowEnabledEDLogFields );

            Debug.Assert( !moreThanOneOperationCanbeExecuted, "Condition for only one operation should be true at any given time" );
        }

        private void ModeOfArrivalRequiredEventHandler( object sender, EventArgs e )
        {
            View.MakeModeOfArrivalRequired();
        }

        private void EvaluateModeOfArrivalRequired()
        {
            RuleEngine.OneShotRuleEvaluation<ModeOfArrivalRequired>( Model, ModeOfArrivalRequiredEventHandler );
        }

        private bool ShouldWeClearEDLogFields( bool isViewBeingUpdated )
        {
            return !isViewBeingUpdated && IsNonEmergencyRegistration();
        }


        private bool ShouldWeShowDisableEDLogFields( bool edLogFieldsHaveData )
        {
            return IsMaintenanceActivity() && IsOutPatient() && edLogFieldsHaveData;
        }

        private bool ShouldWeEnableEDLogForUCCPostMse()
        {
            return ((IsUCCPostMSEActivity));
        }

        private bool IsUCCPostMSEActivity
        {
            get { return Activity.GetType() == typeof (UCCPostMseRegistrationActivity); }
        }
 
        private bool ShouldWeShowEnabledEDLogFields()
        {
            return IsPostMseRegistrationActivity() ||
                   IsEmergencyMaintenance() ||
                   IsEmergencyRegistration();
        }


        private bool ShouldWeNotShowEDLogFields( bool edLogFieldsHaveData )
        {
            if ( IsOutPatientMaintenance() )
            {
                return !edLogFieldsHaveData;
            }

            return IsPreRegistrationActivity() || Activity.IsNewBornRelatedActivity() ||
                   IsNonEmergencyRegistration() ||
                   IsNonEmergencyNonOutPatientMaintenance();
        }


        private bool IsEmergencyRegistration()
        {
            return IsRegistrationActivity() && IsEmergencyPatient();
        }


        private bool IsNonEmergencyRegistration()
        {
            return IsRegistrationActivity() && !IsEmergencyPatient();
        }


        private bool IsEmergencyMaintenance()
        {
            return IsMaintenanceActivity() && IsEmergencyPatient();
        }


        private bool IsOutPatientMaintenance()
        {
            return ( IsMaintenanceActivity() && VisitType.IsOutpatient );
        }


        private bool IsNonEmergencyNonOutPatientMaintenance()
        {
            return IsMaintenanceActivity() && !( IsOutPatient() || IsEmergencyPatient() );
        }


        private bool IsMaintenanceActivity()
        {
            return Activity.GetType().Equals( typeof( MaintenanceActivity ) );
        }


        private bool IsPostMseRegistrationActivity()
        {
            return Activity.GetType().Equals( typeof( PostMSERegistrationActivity ) );
        }


        private bool IsAdmitNewbornActivity()
        {
            return Activity.GetType().Equals( typeof( AdmitNewbornActivity ) );
        }


        private bool IsPreRegistrationActivity()
        {
            return Activity.GetType().Equals( typeof( PreRegistrationActivity ) );
        }


        private bool IsRegistrationActivity()
        {
            return Activity.GetType().Equals( typeof( RegistrationActivity ) );
        }


        private bool IsEmergencyPatient()
        {
            return VisitType != null && VisitType.IsEmergencyPatient;
        }


        private bool IsOutPatient()
        {
            return ( VisitType != null && VisitType.IsOutpatient );
        }

        private void PopulateModeOfArrival()
        {
            var broker = new ModeOfArrivalBrokerProxy();
            var arrivalModes = broker.ModesOfArrivalFor( User.GetCurrent().Facility.Oid );

            View.PopulateArrivalModes( arrivalModes );

            var modeOfArrival = Model.ModeOfArrival;

            if ( modeOfArrival != null )
            {
                {
                    View.SetModeOfArrival( modeOfArrival );
                }
            }
        }

        private void PopulateReferralType()
        {
            var broker = new ReferralTypeBrokerProxy();
            var referralTypes = (ArrayList)broker.ReferralTypesFor( User.GetCurrent().Facility.Oid );

            View.PopulateReferralTypes( referralTypes );

            var referralType = Model.ReferralType;

            if ( referralType != null )
            {
                View.SetReferralType( referralType );
            }
        }

        private void PopulateReferralFacility()
        {
            var broker = BrokerFactory.BrokerOfType<IReferralFacilityBroker>();
            var referralFacilities = broker.ReferralFacilitiesFor( User.GetCurrent().Facility.Oid );

            View.PopulateReferralFacilities( referralFacilities );

            var referralFacility = Model.ReferralFacility;

            if ( referralFacility != null )
            {
                View.SetReferralFacility( referralFacility );
            }
        }

        private void PopulateReadmitCode()
        {
            var broker = BrokerFactory.BrokerOfType<IReAdmitCodeBroker>();
            var readmitCodes = broker.ReAdmitCodesFor( User.GetCurrent().Facility.Oid );

            View.PopulateReadmitCode( readmitCodes );
            var admitCode = Model.ReAdmitCode;

            if ( admitCode != null )
            {
                View.SetReadmitCode( admitCode );
            }
        }

        private void DisplayArrivalTime()
        {
            DateTime arrivalTime = Model.ArrivalTime;

            if ( arrivalTime.Hour != 0 || arrivalTime.Minute != 0 )
            {
                var displayedTimeFormat = CommonFormatting.DisplayedTimeFormat( arrivalTime );

                View.SetArrivalTime( displayedTimeFormat );
            }
            else
            {
                View.ClearArrivalTimeMask();
            }
        }

        private void ClearModel()
        {
            Model.ArrivalTime = DateTime.MinValue;
            Model.ModeOfArrival = new ModeOfArrival();
            Model.ReferralType = new ReferralType();
            Model.ReferralFacility = new ReferralFacility();
            Model.ReAdmitCode = new ReAdmitCode();
        }
    }
}