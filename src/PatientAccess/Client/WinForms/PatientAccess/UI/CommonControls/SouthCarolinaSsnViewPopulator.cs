using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using System.Linq;

namespace PatientAccess.UI.CommonControls
{
    public class SouthCarolinaSsnViewPopulator : ISsnViewPopulator
    {
        public SouthCarolinaSsnViewPopulator(IMessageBoxAdapter messageBoxAdapter, Facility facility)
        {
            MessageBoxAdapter = messageBoxAdapter;
            Facility = facility;
        }

        private Facility Facility { get; set; }

        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        public void Populate( ISsnView ssnView )
        {
          

            ISSNBroker ssnBroker = new SSNBrokerProxy();

            IList<SocialSecurityNumberStatus> ssnStatusCollection;

            if (ssnView.SsnContext == SsnViewContext.GuarantorView || ssnView.SsnContext == SsnViewContext.ShortGuarantorView)
            {
                ssnStatusCollection = ssnBroker.SSNStatusesForGuarantor( Facility.Oid, Facility.GetPersonState().Code ).OfType<SocialSecurityNumberStatus>().ToList();
            }

            else
            {
                ssnStatusCollection = ssnBroker.SSNStatuses();
            }

            if ( ssnStatusCollection == null || ssnStatusCollection.Count == 0 )
            {
                MessageBoxAdapter.ShowMessageBox( "No SSN statuses were found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            ssnView.ClearSsnStatus();
            foreach (var ssnStatus in ssnStatusCollection)
            {
                if ( ssnStatus.IsNewbornSSNStatus && 
                    ssnView.ModelAccount.Patient.DateOfBirth != DateTime.MinValue && 
                    ssnView.ModelAccount.Patient.AgeInYearsFor() > 2 )
                {
                    continue;
                }
                ssnView.AddSsnStatus( ssnStatus );
                 
            } 

            if (ssnView.SsnStatusCount > 0 )
            {
                ssnView.DeselectSelectedStatus();
            }
        }
    }
}
