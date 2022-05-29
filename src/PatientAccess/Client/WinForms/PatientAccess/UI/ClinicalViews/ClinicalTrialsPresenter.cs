using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.Utilities;

namespace PatientAccess.UI.ClinicalViews
{
    public class ClinicalTrialsPresenter : IClinicalTrialsPresenter
    {
        private IClinicalTrialsView View { get; set; }
        private RuleEngine RuleEngine { get; set; }
        private IClinicalTrialsFeatureManager ClinicalTrialsFeatureManager { get; set; }

        public YesNoFlag PatientIsInClinicalStudy
        {
            get
            {
                return Account.IsPatientInClinicalResearchStudy;
            }

            private set
            {
                Account.IsPatientInClinicalResearchStudy = View.IsPatientInClinicalResearchStudy = value;
            }
        }

        private long FacilityId { get; set; }

        public Account Account { get; set; }

        public IClinicalTrialsDetailsView DetailsView { get; set; }
        public ClinicalTrialsDetailsPresenter DetailsPresenter { get; set; }

        public IResearchStudyBroker StudyBroker { get; private set; }

        public ClinicalTrialsPresenter( IClinicalTrialsView view, IClinicalTrialsDetailsView detailsView, IClinicalTrialsFeatureManager clinicalTrialsFeatureManager, Account account, long facilityId, IResearchStudyBroker studyBroker )
        {
            Guard.ThrowIfArgumentIsNull( view, "view" );
            Guard.ThrowIfArgumentIsNull( detailsView, "detailsView" );
            Guard.ThrowIfArgumentIsNull( clinicalTrialsFeatureManager, "clinicalTrialsFeatureManager" );
            Guard.ThrowIfArgumentIsNull( account, "account" );
            Guard.ThrowIfArgumentIsNull( account.IsPatientInClinicalResearchStudy, "account.IsPatientInClinicalResearchStudy" );
            Guard.ThrowIfArgumentIsNull( studyBroker, "studyBroker" );

            FacilityId = facilityId;
            View = view;
            Account = account;
            ClinicalTrialsFeatureManager = clinicalTrialsFeatureManager;
            RuleEngine = RuleEngine.GetInstance();
            DetailsView = detailsView;
            StudyBroker = studyBroker;
            PatientIsInClinicalStudy = account.IsPatientInClinicalResearchStudy;
            DetailsPresenter = new ClinicalTrialsDetailsPresenter( FacilityId, Account, DetailsView, new List<ResearchStudy>() );

            EnableDisableCommands();
        }

        public void ShowDetails()
        {
            var studies = this.StudyBroker.AllResearchStudies( this.FacilityId );
            this.DetailsPresenter.SetStudySelectionList( studies );
            DetailsPresenter.ShowDetails();
        }

        public void UserChangedPatientInClinicalTrialsTo( YesNoFlag patientInClinicalResearchStudy )
        {
            var previousUserSelection = PatientIsInClinicalStudy;
            CheckInvariant();

            if ( patientInClinicalResearchStudy.IsNo )
            {
                if ( Account.ClinicalResearchStudies.Count() != 0 )
                {
                    bool discardStudies = View.GetConfirmationForDiscardingPatientStudies();

                    if ( discardStudies )
                    {
                        Account.ClearResearchStudies();
                        this.Account.RemoveClinicalTrialsConditionCode( this.FacilityId );
                        PatientIsInClinicalStudy = YesNoFlag.No;
                        DetailsPresenter.DiscardChanges();
                    }
                    else
                    {
                        PatientIsInClinicalStudy = previousUserSelection;
                    }
                }

                else
                {
                    PatientIsInClinicalStudy = Account.IsPatientInClinicalResearchStudy = YesNoFlag.No;
                }
            }

            else if ( patientInClinicalResearchStudy.IsBlank )
            {
                if ( Account.ClinicalResearchStudies.Count() != 0 )
                {
                    bool discardStudies = View.GetConfirmationForDiscardingPatientStudies();

                    if ( discardStudies )
                    {
                        Account.ClearResearchStudies();
                        PatientIsInClinicalStudy = YesNoFlag.Blank;
                        DetailsPresenter.DiscardChanges();
                    }
                    else
                    {
                        PatientIsInClinicalStudy = previousUserSelection;
                    }
                }

                else
                {
                    PatientIsInClinicalStudy = Account.IsPatientInClinicalResearchStudy = YesNoFlag.Blank;
                }
            }

            else if ( patientInClinicalResearchStudy.IsYes )
            {
                ShowDetails();

                if ( DetailsPresenter.PatientStudies.Count() != 0 )
                {
                    Account.IsPatientInClinicalResearchStudy = PatientIsInClinicalStudy = YesNoFlag.Yes;
                }
                else
                {
                    Account.IsPatientInClinicalResearchStudy = PatientIsInClinicalStudy = YesNoFlag.Blank;
                }
            }

            EnableDisableCommands();
            CheckInvariant();
        }

        public void EvaluateClinicalResearchFieldRules()
        {
            RuleEngine.EvaluateRule( typeof( PatientInClinicalstudyRequired ), Account );
            RuleEngine.EvaluateRule( typeof( PatientInClinicalstudyPreferred ), Account );
        }

        public void HandleClinicalResearchFields( DateTime admitDate )
        {
            CheckInvariant();

            bool showClinicalResearchFields = Account.Facility.IsValidForClinicalResearchFields();

            bool showEnabledClinicalResearchFields = 
                ClinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( admitDate, DateTime.Today );

            if ( showClinicalResearchFields )
            {
                View.ShowClinicalResearchFieldsAsVisible( true );
                View.PopulateClinicalResearchField();

                if ( showEnabledClinicalResearchFields )
                {
                    View.ShowClinicalResearchFieldEnabled();
                }
                else
                {
                    View.ShowClinicalResearchFieldDisabled();
                }
            }
            else
            {
                View.ShowClinicalResearchFieldsAsVisible( false );
            }

            CheckInvariant();
        }

        private void EnableDisableCommands()
        {
            View.ViewDetailsCommandVisible = PatientIsInClinicalStudy.IsYes ;

            View.ViewDetailsCommandEnabled =
                PatientIsInClinicalStudy.IsYes &&
                ClinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( Account.AdmitDate, DateTime.Today );
        }

        private void CheckInvariant()
        {
            bool patientInStudy = PatientIsInClinicalStudy.IsYes;
            int patientStudyCount = Account.ClinicalResearchStudies.Count();
            bool patientStudyCountIsZero = patientStudyCount == 0;

            bool invariant = ( patientInStudy && !patientStudyCountIsZero ) || ( !patientInStudy && patientStudyCountIsZero );

            Debug.Assert( invariant, String.Format( "Class invariant violated: Patient Is in Study = {0} and Patient study count is {1}", patientInStudy, patientStudyCount ) );
        }
    }
}