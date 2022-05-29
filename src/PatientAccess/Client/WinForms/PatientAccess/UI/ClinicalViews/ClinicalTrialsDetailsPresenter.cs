using System;
using System.Collections.Generic;
using System.Linq;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.Utilities;

namespace PatientAccess.UI.ClinicalViews
{
    public class ClinicalTrialsDetailsPresenter
    {
        private IClinicalTrialsDetailsView View { get; set; }
        private bool UserSavedChanges { get; set; }
        private bool UserMadeChanges { get; set; }
        private IList<ResearchStudy> studySelectionList;
        private List<ConsentedResearchStudy> patientStudies;

        private long FacilityId { get; set; }

        private Account Account { get; set; }
        private ResearchStudy SelectedStudy { get; set; }

        private ConsentedResearchStudy SelectedPatientStudy { get; set; }

        public IEnumerable<ConsentedResearchStudy> PatientStudies
        {
            get
            {
                return patientStudies;
            }
        }

        public IEnumerable<ResearchStudy> StudySelectionList
        {
            get { return studySelectionList; }
        }

        public ClinicalTrialsDetailsPresenter( long facilityID, Account patientAccount, IClinicalTrialsDetailsView detailsView, IList<ResearchStudy> selection )
        {
            Guard.ThrowIfArgumentIsNull( patientAccount, "patientAccount" );
            Guard.ThrowIfArgumentIsNull( detailsView, "patientAccount" );
            Guard.ThrowIfArgumentIsNull( patientAccount, "patientAccount" );
            Guard.ThrowIfArgumentIsNull( patientAccount.IsPatientInClinicalResearchStudy, "patientAccount.IsPatientInClinicalResearchStudy" );
            Guard.ThrowIfArgumentIsNull( patientAccount.ClinicalResearchStudies, "patientAccount.ClinicalResearchStudies" );

            FacilityId = facilityID;
            Account = patientAccount;
            View = detailsView;
            View.Presenter = this;
            studySelectionList = selection;
            UserMadeChanges = false;
            UserSavedChanges = false;

            patientStudies = new List<ConsentedResearchStudy>( patientAccount.ClinicalResearchStudies );
        }

        /// <summary>
        /// Updates the selected study.
        /// </summary>
        /// <param name="study">The study.</param>
        /// <exception cref="ArgumentNullException"><c>study</c> is null.</exception>
        public void UpdateSelectedStudyInSelectionList( ResearchStudy study )
        {
            Guard.ThrowIfArgumentIsNull( study, "study" );
            SelectedStudy = study;
            EnableDisableCommands();
        }

        public void UpdateSelectedPatientStudy( ConsentedResearchStudy study )
        {
            Guard.ThrowIfArgumentIsNull( study, "study" );
            SelectedPatientStudy = study;
        }

        public void SetStudySelectionList( IEnumerable<ResearchStudy> selectionList )
        {
            Guard.ThrowIfArgumentIsNull( selectionList, "selectionList" );
            studySelectionList = new List<ResearchStudy>( selectionList );
        }

        public void EnrollWithConsent()
        {
            var consent = YesNoFlag.Yes;

            AddedPatientStudyWith( consent );
            UpdateView();
        }

        public void EnrollWithoutConsent()
        {
            var consent = YesNoFlag.No;

            AddedPatientStudyWith( consent );

            UpdateView();
        }

        public void ShowDetails()
        {
            UserMadeChanges = false;
            UserSavedChanges = false;

            RemovePatientStudiesFromSelectionList();

            UpdateViewWith( Account.ClinicalResearchStudies.AsEnumerable() );

            EnableDisableCommands();

            View.ShowMe();
        }

        public void DiscardChanges()
        {
            patientStudies.Clear();
        }

        public void SaveChangesAndExit()
        {
            UserSavedChanges = true;
            SaveChanges();
            View.CloseMe();
            UserMadeChanges = false;
            UserSavedChanges = false;
        }

        public void RemoveSelectedPatientStudy()
        {
            patientStudies.Remove( SelectedPatientStudy );
            studySelectionList.Add( SelectedPatientStudy.ResearchStudy );
            UpdateView();
            UserMadeChanges = true;
            EnableDisableCommands();
        }

        public bool ShowWarningMessage()
        {
            return !UserSavedChanges && UserMadeChanges;
        }

        private void RemovePatientStudiesFromSelectionList()
        {
            var patientStudiesCodes = Account.ClinicalResearchStudies.Select( x => x.Code );

            var listWithoutPatientStudies = StudySelectionList.Where( x => !patientStudiesCodes.Contains( x.Code ) ).ToList();
            studySelectionList = new List<ResearchStudy>( listWithoutPatientStudies ).ToList();
        }

        public void UpdateView()
        {
            UpdateViewWith( PatientStudies );
        }

        private void UpdateViewWith( IEnumerable<ConsentedResearchStudy> patientAssociatedStudies )
        {
            Guard.ThrowIfArgumentIsNull( patientAssociatedStudies, "patientAssociatedStudies" );

            patientStudies = new List<ConsentedResearchStudy>( patientAssociatedStudies );

            EnableDisableCommands();

            if ( View.ShowExpiredStudies )
            {
                var selectectionListWithAllStudies = new List<ResearchStudy>( studySelectionList );
                selectectionListWithAllStudies.Sort( new ResearchSponsorComparer() );
                View.Update( patientStudies, selectectionListWithAllStudies );
            }

            else
            {
                var selectionListWithoutExpiredStudies = new List<ResearchStudy>( studySelectionList.Where( x => !IsResearchStudyExpired( x ) ) );
                selectionListWithoutExpiredStudies.Sort( new ResearchSponsorComparer() );
                View.Update( patientStudies, selectionListWithoutExpiredStudies );
            }
        }

        private void SaveChanges()
        {
            Account.ClearResearchStudies();

            foreach ( var study in patientStudies )
            {
                if ( Account.CanAddConsentedResearchStudy() )
                {
                    Account.AddConsentedResearchStudy( study );
                }
            }

            if ( patientStudies.Count() != 0 )
            {
                Account.AddClinicalTrialsConditionCode( FacilityId );
            }
        }

        private void AddedPatientStudyWith( YesNoFlag consent )
        {
            if ( patientStudies.Count < 10 )
            {
                var consentedResearchStudy = new ConsentedResearchStudy( SelectedStudy, consent );
                studySelectionList.Remove( SelectedStudy );
                patientStudies.Add( consentedResearchStudy );

                EnableDisableCommands();
                UserMadeChanges = true;
            }
        }

        private void EnableDisableCommands()
        {
            View.SaveCommandEnabled = patientStudies.Count > 0;

            View.RemoveCommandEnabled = patientStudies.Count > 1;

            bool isSelectStudyExpired = false;

            if ( SelectedStudy != null )
            {
                isSelectStudyExpired = IsResearchStudyExpired( SelectedStudy );
            }

            View.EnrollCommandsEnabled = studySelectionList.Count > 0 && patientStudies.Count < 10 && !isSelectStudyExpired;

        }

        public bool IsResearchStudyExpired( ResearchStudy researchStudy )
        {
            var expirationCalculator = new ResearchStudyExpirationCalculator( Account.AdmitDate, researchStudy.TerminationDate );
            return expirationCalculator.IsStudyExpired();
        }
    }

    public class ResearchSponsorComparer : IComparer<ResearchStudy>
    {
        public int Compare( ResearchStudy x, ResearchStudy y )
        {
            return x.ResearchSponsor.CompareTo( y.ResearchSponsor );
        }
    }
}