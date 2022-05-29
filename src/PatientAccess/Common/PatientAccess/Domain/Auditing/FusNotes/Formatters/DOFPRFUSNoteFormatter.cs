using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for RPFCRFUSNoteFormatter.
    /// </summary>
    //TODO: Create XML summary comment for RPFCRFUSNoteFormatter
    [Serializable]
    [UsedImplicitly]
    public class DOFPRFUSNoteFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            ArrayList messages = new ArrayList();
            string msg = string.Empty;
            FusNote note = this.Context as FusNote;
            string code = note.FusActivity.Code;
            Account account = (Account)note.Context;
            messages = this.CreateFusNameValueList(account);
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList(Account account)
        {
            ArrayList nameValueList = new ArrayList();
            string formattedString = String.Empty;

            if (account != null)
            {
                formattedString = FormatNameValuePair(FusLabel.FACILITY,
                    account.Facility.Code);
                nameValueList.Add(formattedString);

                formattedString = FormatNameValuePair(FusLabel.PAYOR,
                 account.Insurance.PrimaryCoverage.InsurancePlan.PlanName);
                nameValueList.Add(formattedString);
         
                formattedString = FormatNameValuePair(FusLabel.PT,
                 account.KindOfVisit.Code);
                nameValueList.Add(formattedString);

                if (account.HospitalService !=null && !string.IsNullOrEmpty(account.HospitalService.Code))
                {
                    formattedString = FormatNameValuePair(FusLabel.HSV,
                 account.HospitalService.Code);
                    nameValueList.Add(formattedString);
                }

                formattedString = FormatNameValuePair(FusLabel.PLANID,
                 account.Insurance.PrimaryCoverage.InsurancePlan.PlanID);
                nameValueList.Add(formattedString);

                formattedString = FormatNameValuePair(FusLabel.FC,
                 account.FinancialClass.Code);
                nameValueList.Add(formattedString);

                if (account.HospitalClinic != null && !string.IsNullOrEmpty(account.HospitalClinic.Code))
                {
                    formattedString = FormatNameValuePair(FusLabel.CLINIC_CODE,
                        account.HospitalClinic.Code);
                    nameValueList.Add(formattedString);
               
                    if (!string.IsNullOrEmpty(account.EmbosserCard) && account.ServiceCategory !=null 
                        && !string.IsNullOrEmpty(((ClinicServiceCategory)account.ServiceCategory).Description))
                    {
                        formattedString = FormatNameValuePair(FusLabel.SERVICE_CATEGORY,
                             ((ClinicServiceCategory)account.ServiceCategory).Description);
                        nameValueList.Add(formattedString);
                    }
                }
                if (account.Insurance.PrimaryCoverage != null 
                    && account.Insurance.PrimaryCoverage.GetType()==typeof(CommercialCoverage)
                    && !string.IsNullOrEmpty(((CommercialCoverage)account.Insurance.PrimaryCoverage).AidCodeType))
                {
                    formattedString = FormatNameValuePair(FusLabel.AID_CODE,
                        ((CommercialCoverage)account.Insurance.PrimaryCoverage).AidCodeType);
                    nameValueList.Add(formattedString);
                }
                var ipaClinics = (ArrayList)account.MedicalGroupIPA.Clinics;
                if (ipaClinics != null && ipaClinics.Count > 0)
                {
                    formattedString = string.Format(FusLabel.IPA_CODE_AND_CLINIC,
                        account.MedicalGroupIPA.Code, ((Clinic)(ipaClinics[0])).Name);
                        nameValueList.Add(formattedString);
                }
            }

            return nameValueList;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DOFPRFUSNoteFormatter()
        {
          
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
