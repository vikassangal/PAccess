using System;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.Utilities;

namespace PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// 
    /// </summary>
    public class PatientNamePresenter
    {

        public PatientNamePresenter(IPatientNameView view, PatientNameFeatureManager patientNameFeatureManager)
        {
            Guard.ThrowIfArgumentIsNull(view, "PatientNameView");
            View = view;
            Guard.ThrowIfArgumentIsNull(patientNameFeatureManager, "patientNameFeatureManager");
            PatientNameFeatureManager = patientNameFeatureManager;
        }

        #region PatientNamePresenter Members

        #endregion
        #region private Members
        private String newbornName;
        #endregion private Members
        #region Properties
        private IPatientNameView View { get; set; }
        private Account ModelAccount
        {
            get
            {
                return View.ModelAccount;
            }
        }

        public void SetNewbornName()
        {
            var ShouldAutoPopulateNewbornName =
                PatientNameFeatureManager.IsAutoPopulatePatientName_Enabled(View.ModelAccount);

            if (ShouldAutoPopulateNewbornName)
            {
                AutoPopulateNewBornName();
            }
        }


        public void AutoPopulateNewBornName()
        {
            newbornName = String.Empty;
            var gender = View.PatientGender;
            string genderCode = gender.Code;

            switch (genderCode)
            {
                case Gender.FEMALE_CODE:
                {
                    newbornName = NewBornName(BABYGIRL);
                    break;
                }
                case Gender.MALE_CODE:
                {
                    newbornName = NewBornName(BABYBOY);
                    break;
                }
                default:
                {
                    break;
                }
            }

            if (String.IsNullOrEmpty(View.PatientFirstName) || View.PatientFirstName != newbornName)
            {
                View.PatientFirstName = newbornName;
                ModelAccount.Patient.FirstName = View.PatientFirstName;
            }

            View.RegisterPatientNameRequiredEvent();
        }

        private String NewBornName(String name)
        {
             var mothersFirstName = String.Empty;
            if (ModelAccount != null && ModelAccount.Patient != null && ModelAccount.Patient.MothersAccount != null &&
                ModelAccount.Patient.MothersAccount.Patient != null)
            {
                mothersFirstName = ModelAccount.Patient.MothersAccount.Patient.FirstName;
            }
            newbornName = name + mothersFirstName;

            if (newbornName.Length > NEWBORNNAME_LENGTH)
            {
                newbornName = newbornName.Substring(0, NEWBORNNAME_LENGTH);
            }
            return newbornName;
        }
        private PatientNameFeatureManager PatientNameFeatureManager { get; set; }
        #endregion Properties
        #region Constants
        private const string BABYGIRL = "BABYG";
        private const string BABYBOY = "BABYB";
        private const int NEWBORNNAME_LENGTH = 13;

        #endregion Constants
    }
}