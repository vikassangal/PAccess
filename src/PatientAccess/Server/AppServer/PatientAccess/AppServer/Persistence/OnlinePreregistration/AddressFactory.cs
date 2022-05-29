using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Messaging;
using PatientAccess.Utilities;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class AddressFactory
    {

        #region Constants

        private const string HYPHEN = "-";

        #endregion

        #region Public Methods
        public Address BuildAddress( addressType inputAddress )
        {
            Guard.ThrowIfArgumentIsNull( inputAddress, "inputAddress" );

            Address address = new Address();

            if ( inputAddress.Item is usAddressType )
            {
                address = BuildAddress( ( usAddressType )inputAddress.Item );
            }
            else if ( inputAddress.Item is canadianAddressType )
            {
                address = BuildAddress( ( canadianAddressType )inputAddress.Item );
            }
            else if ( inputAddress.Item is mexicanAddressType )
            {
                address = BuildAddress( ( mexicanAddressType )inputAddress.Item );
            }
            else
            {
                address = BuildAddress( ( nonUSNonCanadaAddressType )inputAddress.Item );
            }

            return address;
        }
        #endregion

        #region Private Methods

        private Address BuildAddress( usAddressType usAddress )
        {
            Guard.ThrowIfArgumentIsNull( usAddress, "usAddress" );

            Address address = new Address
            {
                Address1 = usAddress.addressLine,
                City = usAddress.city,
                State = addressBroker.StateWith( EnumToCode( usAddress.stateOrTerritory ), Facility ),
                Country = addressBroker.CountryWith( Country.USA_CODE, Facility ),
                ZipCode = ( usAddress.zipCode != null ) ? new ZipCode( StripHyphenFromZipCode( usAddress.zipCode ) ) : new ZipCode()
            };

            return address;
        }

        private Address BuildAddress( canadianAddressType canadaAddress )
        {
            Guard.ThrowIfArgumentIsNull( canadaAddress, "canadaAddress" );

            Address address = new Address
            {
                Address1 = canadaAddress.addressLine,
                City = canadaAddress.city,
                State = addressBroker.StateWith( EnumToCode( canadaAddress.province ), Facility ),
                Country = addressBroker.CountryWith( Country.CANADA_CODE, Facility ),
                ZipCode = ( canadaAddress.zipCode != null ) ? new ZipCode( canadaAddress.zipCode ) : new ZipCode()
            };

            return address;
        }

        private Address BuildAddress( mexicanAddressType mexicanAddress )
        {
            Guard.ThrowIfArgumentIsNull( mexicanAddress, "mexicanAddress" );

            Address address = new Address
            {
                Address1 = mexicanAddress.addressLine,
                City = mexicanAddress.city,
                State = addressBroker.StateWith( EnumToCode( mexicanAddress.state ), Facility ),
                Country = addressBroker.CountryWith( Country.MEXICO_CODE, Facility ),
                ZipCode = ( mexicanAddress.zipCode != null ) ? new ZipCode( StripHyphenFromZipCode( mexicanAddress.zipCode ) ) : new ZipCode()
            };

            return address;
        }

        private Address BuildAddress( nonUSNonCanadaAddressType nonUsNonCanadaAddress )
        {
            Guard.ThrowIfArgumentIsNull( nonUsNonCanadaAddress, "nonUsNonCanadaAddress" );

            Address address = new Address
            {
                Address1 = nonUsNonCanadaAddress.addressLine,
                City = nonUsNonCanadaAddress.city,
                State = new State(),
                Country = addressBroker.CountryWith( EnumToCode( nonUsNonCanadaAddress.country ), Facility ),
                ZipCode = ( nonUsNonCanadaAddress.zipCode != null ) ? new ZipCode( nonUsNonCanadaAddress.zipCode ) : new ZipCode()
            };

            return address;
        }

        private static string EnumToCode( usStateOrTerritoryType enumValue )
        {
            return PatientFactory.SplitEnumValueAsCode( enumValue.ToString() );
        }

        private static string EnumToCode( canadianProvinceType enumValue )
        {
            return PatientFactory.SplitEnumValueAsCode( enumValue.ToString() );
        }

        private static string EnumToCode( mexicanStateType enumValue )
        {
            return PatientFactory.SplitEnumValueAsCode( enumValue.ToString() );
        }

        private static string EnumToCode( nonUSNonCanadaCountryType enumValue )
        {
            return PatientFactory.SplitEnumValueAsCode( enumValue.ToString() );
        }

        private static string StripHyphenFromZipCode( string zipCode )
        {
            zipCode = zipCode.Replace( HYPHEN, string.Empty );
            return zipCode;
        }

        #endregion

        #region Private Properties

        private Facility Facility { get; set; }

        #endregion

        #region Construction and Finalization

        public AddressFactory( Facility facility )
        {
            Facility = facility;
        }

        #endregion

        #region Data Elements

        private readonly IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

        #endregion
    }
}
