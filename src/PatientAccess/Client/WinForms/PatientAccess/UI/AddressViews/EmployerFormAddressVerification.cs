using System;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.AddressViews
{
    /// <summary>
    /// Summary description for FormAddressVerification.
    /// </summary>
    [Serializable]

    public class EmployerFormAddressVerification : FormAddressVerification
    {
        public EmployerFormAddressVerification( Facility facility, RuleEngine ruleEngine )
            : base( facility, ruleEngine )
        {

        }

        protected override AddressEntryView TheAddressView
        {
            get
            {
                return new EmployerAddressEntryView( Facility, RuleEngine );
            }
        }
    }
}