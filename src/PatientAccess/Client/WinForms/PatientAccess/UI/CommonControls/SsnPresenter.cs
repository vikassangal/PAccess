using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.CommonControls
{

    public class SsnPresenter
    {
        public SsnPresenter( ISsnView ssnView, State state, Account account, Person person )
        {
            SsnView = ssnView;
            State = state;
            Account = account;
            Person = person;
        }

        private Person Person { get; set; }

        private Account Account { get; set; }

        private State State { get; set; }

        private ISsnView SsnView { get; set; }

        public void SsnStatusChanged( SocialSecurityNumberStatus newSsnStatus )
        {
            var currentSsn = Person.SocialSecurityNumber;

            if ( ( !newSsnStatus.IsKnownSSNStatus || currentSsn.IsDefaultSsn() ) )
            {
                Person.SocialSecurityNumber = SsnView.SsnFactory.GetValidSocialSecurityNumberFor( State, Account.AdmitDate, newSsnStatus );
            }

            SsnView.SsnText = Person.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim();
            SsnView.SsnEnabled = ( Person.SocialSecurityNumber.SSNStatus.IsKnownSSNStatus );
        }
    }
}
