using System;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CoverageDefaults.
	/// </summary>
    public class CoverageDefaults
    {
        public CoverageDefaults()
        {
            
        }
        public void SetCoverageDefaultsForActivity(Account anAccount)
        {
            if( anAccount.IsSetInsuranceVerifiedToDefault() )
            {
                
                if(anAccount.Activity != null)
                {
                    if (!anAccount.Activity.GetType().Equals(typeof(RegistrationActivity)) &&
                        !anAccount.Activity.GetType().Equals(typeof(AdmitNewbornActivity)) &&
                        !anAccount.Activity.GetType().Equals(typeof(PreAdmitNewbornActivity)) &&
                        !anAccount.Activity.GetType().Equals(typeof(PreRegistrationActivity)) &&
                        !anAccount.Activity.GetType().Equals(typeof(ShortRegistrationActivity)) &&
                        !anAccount.Activity.GetType().Equals(typeof(ShortPreRegistrationActivity)) &&
                        !anAccount.Activity.GetType().Equals(typeof(QuickAccountCreationActivity)) &&
                        !anAccount.Activity.GetType().Equals(typeof(ActivatePreRegistrationActivity))
                        )
                    {
                        Coverage existingPrimaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                        if (existingPrimaryCoverage != null)
                        {
                            Coverage defaultPrimaryCoverage = SetCoverageDefaults(existingPrimaryCoverage);
                            anAccount.Insurance.SetAsPrimary(defaultPrimaryCoverage);
                        }

                        Coverage existingSecondaryCoverage = anAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID);
                        if (existingSecondaryCoverage != null)
                        {
                            Coverage defaultSecondaryCoverage = SetCoverageDefaults(existingSecondaryCoverage);
                            anAccount.Insurance.SetAsSecondary(defaultSecondaryCoverage);
                        }

                    }
                }
            }
             
        }
        private Coverage  SetCoverageDefaults(Coverage coverage)
        {
            if( coverage != null )
            {
                if( coverage.GetType() == typeof( SelfPayCoverage ) )
                {                         
                    coverage.BenefitsVerified.SetBlank();
                    coverage.AuthorizingPerson = string.Empty;
                    coverage.DateTimeOfVerification = DateTime.MinValue;
                  
                }
                else  if( coverage.GetType() == typeof( GovernmentMedicareCoverage )  )
                {
                    coverage.BenefitsVerified.SetBlank();
                    coverage.AuthorizingPerson = string.Empty;
                    coverage.DateTimeOfVerification = DateTime.MinValue;
                                                 
                }
                else
                {
                    coverage.BenefitsVerified.SetBlank();
                    coverage.AuthorizingPerson = string.Empty;
                    coverage.DateTimeOfVerification = DateTime.MinValue;
                    if (coverage.GetType().IsSubclassOf(typeof(CoverageGroup)))
                    {
                        CoverageGroup coverageGroup = coverage as CoverageGroup;
                        coverageGroup.Authorization.AuthorizationRequired.SetBlank();
                        coverageGroup.Authorization.AuthorizationCompany = string.Empty;
                        coverageGroup.Authorization.AuthorizationPhone = new Parties.PhoneNumber();
                        coverageGroup.Authorization.PromptExt = string.Empty;
                        
                    }

                }                         

            }
            return coverage;
        }
    }

	
}
