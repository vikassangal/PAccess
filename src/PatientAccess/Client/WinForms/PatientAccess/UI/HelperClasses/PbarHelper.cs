using PatientAccess.Domain;
using PatientAccess.Utilities;

namespace PatientAccess.UI.HelperClasses
{
    public static class PbarHelper
    {
        public static void ChangeLogicalStringPropertiesToUpperCase(DataValidationBenefitsResponse benefitsResponse)
        {
            Guard.ThrowIfArgumentIsNull(benefitsResponse, "benefitsResponse");

            benefitsResponse.PlanCode = ToUpperIfNotNullOrEmpty(benefitsResponse.PlanCode);
            benefitsResponse.ResponseGroupNumber = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseGroupNumber);
            benefitsResponse.ResponseInsuredDOB = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseInsuredDOB);
            benefitsResponse.ResponsePayorName = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponsePayorName);
            benefitsResponse.ResponseSubscriberID = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseSubscriberID);
            benefitsResponse.RequestInsuredDOB = ToUpperIfNotNullOrEmpty(benefitsResponse.RequestInsuredDOB);
            benefitsResponse.RequestPayorName = ToUpperIfNotNullOrEmpty(benefitsResponse.RequestPayorName);
            benefitsResponse.RequestSubscriberID = ToUpperIfNotNullOrEmpty(benefitsResponse.RequestSubscriberID);
            benefitsResponse.RequestInsuredFirstName = ToUpperIfNotNullOrEmpty(benefitsResponse.RequestInsuredFirstName);
            benefitsResponse.RequestInsuredMiddleInitial = ToUpperIfNotNullOrEmpty(benefitsResponse.RequestInsuredMiddleInitial);
            benefitsResponse.RequestInsuredLastName = ToUpperIfNotNullOrEmpty(benefitsResponse.RequestInsuredLastName);
            benefitsResponse.ResponseInsuredFirstName = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseInsuredFirstName);
            benefitsResponse.ResponseInsuredMiddleInitial = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseInsuredMiddleInitial);
            benefitsResponse.ResponseInsuredLastName = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseInsuredLastName);
            benefitsResponse.ResponseAuthCoPhone = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseAuthCoPhone);
            benefitsResponse.ResponseAuthCo = ToUpperIfNotNullOrEmpty(benefitsResponse.ResponseAuthCo);
        }

        private static string ToUpperIfNotNullOrEmpty(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            
            return input.ToUpperInvariant();
            
        }
    }
}