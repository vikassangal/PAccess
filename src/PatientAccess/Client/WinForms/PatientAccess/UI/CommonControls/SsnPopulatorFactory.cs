using PatientAccess.Domain;
using PatientAccess.Utilities;

namespace PatientAccess.UI.CommonControls
{
    public class SsnPopulatorFactory
    {
        public SsnPopulatorFactory( IAccount account )
        {
            Guard.ThrowIfArgumentIsNull( account, "account" );
            Guard.ThrowIfArgumentIsNull( account.Facility, "account.Facility" );

            this.account = account;
        }

        public ISsnViewPopulator GetPopulator()
        {
            ISsnViewPopulator ssnFactory;

            if ( account.Facility.IsBaylorFacility() )
            {
                ssnFactory = new BaylorSsnViewPopulator( new MessageBoxAdapter(), User.GetCurrent().Facility );
            }

            else  if ( account.Facility.FacilityState.IsSouthCarolina )
            {
                ssnFactory = new SouthCarolinaSsnViewPopulator( new MessageBoxAdapter(), User.GetCurrent().Facility );
            }

            else
            {
                ssnFactory = new SsnViewPopulator( new MessageBoxAdapter(), User.GetCurrent().Facility );
            }

            return ssnFactory;
        }

        private readonly IAccount account;
    }
}
