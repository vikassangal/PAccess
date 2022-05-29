using PatientAccess.Utilities;

namespace PatientAccess.Domain
{
    public class SsnFactoryCreator
    {
        public SsnFactoryCreator(IAccount account)
        {
            Guard.ThrowIfArgumentIsNull( account, "account" );
            Guard.ThrowIfArgumentIsNull( account.Facility, "account.Facility" );

            this.account = account;
        }

        public ISsnFactory GetSsnFactory()
        {
            ISsnFactory ssnFactory;

            if ( account.Facility.IsBaylorFacility() )
            {
                ssnFactory = new BaylorSsnFactory();
            }
            else if (account.Facility.FacilityState.IsSouthCarolina)
            {
                ssnFactory = new SouthCarolinaSsnFactory();
            }
            else
            {
                ssnFactory = new SSNFactory();
            }

            return ssnFactory;
        }

        private readonly IAccount account; 
    }
}
