using System;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    internal class AgeEntitlementPage1Presenter
    {
        private const string DATE_FORMAT = "{0:D2}{1:D2}{2:D4}";

        #region Construction

        internal AgeEntitlementPage1Presenter( IAgeEntitlementPage1View view, Account account )
        {
            View = view;
            Account = account;
        }

        #endregion

        #region Properties

        internal IAgeEntitlementPage1View View { get; private set; }

        internal Account Account
        {
            get;
            private set;
        }

        #endregion

        public void UpdateView()
        {
            PopulatePatientEmploymentFields();
            PopulateSpouseEmploymentFields();
        }

        public void PopulatePatientEmploymentFields()
        {
            if ( Account == null
                || Account.MedicareSecondaryPayor == null
                || ( Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement ) == null )
            {
                return;
            }

            if ( !Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( AgeEntitlement ) ) )
            {
                return;
            }

            Employment patientEmployment =
                ( Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement ).PatientEmployment;

            if ( patientEmployment != null )
            {
                EmploymentStatus patientEmpStatus = patientEmployment.Status;

                if ( patientEmpStatus != null )
                {
                    if ( patientEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) ||
                         patientEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) )
                    {
                        DisplayPatientFullOrPartTimeEmployment();
                    }
                    else if ( patientEmpStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                    {
                        DisplayPatientRetiredEmployment();
                    }
                    else if ( patientEmpStatus.Code.Equals( EmploymentStatus.OTHER_CODE ) &&
                              patientEmployment.RetiredDate == DateTime.MinValue )
                    {
                        DisplayPatientOtherEmployment();
                    }
                    else if ( patientEmpStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                    {
                        DisplayPatientNeverEmployed();
                    }
                }
            }
        }

        public void PopulateSpouseEmploymentFields()
        {
            if ( Account == null
                || Account.MedicareSecondaryPayor == null
                || ( Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement ) == null )
            {
                return;
            }

            if ( !Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( AgeEntitlement ) ) )
            {
                return;
            }

            Employment spouseEmployment =
                ( Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement ).SpouseEmployment;

            if ( spouseEmployment != null )
            {
                EmploymentStatus spouseEmpStatus = spouseEmployment.Status;

                if ( spouseEmpStatus != null )
                {
                    if ( spouseEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) ||
                         spouseEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) )
                    {
                        DisplaySpouseFullOrPartTimeEmployment();
                    }
                    else if ( spouseEmpStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                    {
                        DisplaySpouseRetiredEmployment();
                    }
                    else if ( spouseEmpStatus.Code.Equals( EmploymentStatus.OTHER_CODE ) &&
                              spouseEmployment.RetiredDate == DateTime.MinValue )
                    {
                        DisplaySpouseOtherEmployment();
                    }
                    else if ( spouseEmpStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                    {
                        DisplaySpouseNeverEmployed();
                    }
                }
            }
        }

        private void DisplayPatientNeverEmployed()
        {
            View.PatientNeverEmployed = true;
        }

        private void DisplayPatientOtherEmployment()
        {
            View.PatientOtherEmployed = true;
            View.PatientRetirementDateText = String.Empty;
        }

        private void DisplayPatientRetiredEmployment()
        {
            Employment employment = Account.MedicareSecondaryPayor.MedicareEntitlement.PatientEmployment;

            if ( employment.RetiredDate != DateTime.MinValue )
            {
                View.PatientRetirementDateText = String.Format( DATE_FORMAT,
                    employment.RetiredDate.Month, employment.RetiredDate.Day, employment.RetiredDate.Year );
            }

            View.PatientRetired = true;
        }

        private void DisplayPatientFullOrPartTimeEmployment()
        {
            View.PatientEmployed = true;
        }

        private void DisplaySpouseRetiredEmployment()
        {
            DateTime date = Account.MedicareSecondaryPayor.MedicareEntitlement.SpouseEmployment.RetiredDate;

            if ( date != DateTime.MinValue )
            {
                View.SpouseRetirementDateText = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
            }

            View.SpouseRetired = true;
        }

        private void DisplaySpouseFullOrPartTimeEmployment()
        {
            View.SpouseEmployed = true;

            View.DisplaySpouseEmployment( Account.MedicareSecondaryPayor.MedicareEntitlement.SpouseEmployment );
        }

        private void DisplaySpouseNeverEmployed()
        {
            View.SpouseNeverEmployed = true;
        }

        private void DisplaySpouseOtherEmployment()
        {
            View.SpouseOtherEmployed = true;
            View.SpouseRetirementDateText = String.Empty;
        }

    }
}
