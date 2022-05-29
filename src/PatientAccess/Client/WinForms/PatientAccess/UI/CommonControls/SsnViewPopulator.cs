using System.Collections;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls
{
    public class SsnViewPopulator : ISsnViewPopulator
    {
        public SsnViewPopulator ( IMessageBoxAdapter messageBoxAdapter, Facility facility )
        {
            MessageBoxAdapter = messageBoxAdapter;
            Facility = facility;
        }

        private Facility Facility { get; set; }

        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        public void Populate( ISsnView ssnView )
        {
            if (ssnView.SsnStatusCount > 0 )
            {
                return;
            }

            ISSNBroker ssnBroker = new SSNBrokerProxy();

            ICollection ssnStatusCollection;

            if ( ssnView.SsnContext == SsnViewContext.GuarantorView || ssnView.SsnContext == SsnViewContext.ShortGuarantorView )
            {
                ssnStatusCollection = ssnBroker.SSNStatusesForGuarantor( User.GetCurrent().Facility.Oid, User.GetCurrent().Facility.GetPersonState().Code );
            }

            else
            {
                ssnStatusCollection = ssnBroker.SSNStatuses( Facility.Oid, Facility.GetPersonState().Code );
            }

            if ( ssnStatusCollection == null || ssnStatusCollection.Count == 0 )
            {
                MessageBoxAdapter.ShowMessageBox( "No SSN statuses were found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            foreach (SocialSecurityNumberStatus ssnStatus in ssnStatusCollection)
            {
                if (!ssnStatus.IsRefusedSSNStatus)
                {
                    ssnView.AddSsnStatus( ssnStatus );
                } //if
            }

            if (ssnView.SsnStatusCount > 0 )
            {
                ssnView.DeselectSelectedStatus();
            }
        }
    }
}
