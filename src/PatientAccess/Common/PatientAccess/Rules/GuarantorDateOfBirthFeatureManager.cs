using System;
using System.Configuration;
using System.Linq;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    public interface IGuarantorDateOfBirthFeatureManager
    {
        bool IsGuarantorDateOfBirthEnabledForDate(DateTime date);
        bool IsGuarantorDateOfBirthEnabledForInsuranceAndGuarantorRelationship(Account account);
        bool ShouldWeEnableGuarantorDateOfBirthFeature(Account account);
    }

    [Serializable]
    public class GuarantorDateOfBirthFeatureManager : IGuarantorDateOfBirthFeatureManager
    {
        private DateTime GuarantorDateOfBirthFeatureStartDate = DateTime.MinValue;


        public bool IsGuarantorDateOfBirthEnabledForInsuranceAndGuarantorRelationship(Account account)
        {
            var hasWorkersCompensationCoverage = HasWorkersCompensationCoverage(account.Insurance);
            var guarantorIsPatientsEmployer = GuarantorIsPatientsEmployer(account.Guarantor);
            if (hasWorkersCompensationCoverage && guarantorIsPatientsEmployer)
            {
                return false;
            }
            return true;
        }

        public bool GuarantorIsPatientsEmployer(Guarantor guarantor)
        {
            return
                guarantor != null && guarantor.Relationships.Cast<Relationship>()
                    .Any(
                        r =>
                            r != null && r.Type != null &&
                            (r.Type.Code == RelationshipType.RELATIONSHIPTYPE_EMPLOYEE));
        }

        private bool HasWorkersCompensationCoverage(Insurance insurance)
        {
            return insurance != null &&
                   insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder()) != null &&
                   insurance.CoverageFor(CoverageOrder.NewPrimaryCoverageOrder()).GetType() ==
                   typeof (WorkersCompensationCoverage);
        }

        /// <summary>
        /// Determines whether [is alternate care facility enabled for date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if [is alternate care facility enabled for date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGuarantorDateOfBirthEnabledForDate(DateTime date)
        {
            if ((date == DateTime.MinValue && DateTime.Today >= FeatureStartDate) ||
                 (date >= FeatureStartDate))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shoulds the we enable alternate care facility feature.
        /// </summary>
 
        /// <param name="account"></param>
        /// <returns></returns>
        public bool ShouldWeEnableGuarantorDateOfBirthFeature(Account account)
        {
            return (IsGuarantorDateOfBirthEnabledForDate(account.AccountCreatedDate)) &&
                   IsGuarantorDateOfBirthEnabledForInsuranceAndGuarantorRelationship(account);
        }

        /// <summary>
        /// Gets the feature start date.
        /// </summary>
        /// <value>The feature start date.</value>
    
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[GUARANTOR_DATEOFBIRTH_START_DATE];
                if (GuarantorDateOfBirthFeatureStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    GuarantorDateOfBirthFeatureStartDate = DateTime.Parse(startDate);
                }
                return GuarantorDateOfBirthFeatureStartDate;
            }
        }
        internal const string GUARANTOR_DATEOFBIRTH_START_DATE = "GUARANTOR_DATEOFBIRTH_START_DATE";
    } 
}
