using System;
using System.Data;
using Extensions.DB2Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class InsuredEmployerAddressStrategy : AddressReaderStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override void LoadContactPointOn( Party aParty, SafeReader fromReader, long facilityID )
        {
            Address anAddress = ReadAddressFrom( fromReader, facilityID );
            ContactPoint contactPoint = ( ( Insured )aParty ).Employment.Employer.PartyContactPoint;

            if ( contactPoint == null )
            {
                contactPoint = new ContactPoint
                               { TypeOfContactPoint = TypeOfContactPoint.NewBusinessContactPointType() };
                ( ( Insured )aParty ).Employment.Employer.PartyContactPoint = contactPoint;
            }

            contactPoint.TypeOfContactPoint = TypeOfContactPoint.NewBusinessContactPointType();
            contactPoint.Address = anAddress;

            PhoneNumber phoneNumber = ReadPhoneNumberFrom( fromReader );
            contactPoint.PhoneNumber = phoneNumber;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private Address ReadAddressFrom( SafeReader fromReader, long facilityID )
        {
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

            string employerAddress = string.Empty;
            string employerLoc = string.Empty;
            string employerCity = string.Empty;
            string employerState = string.Empty;
            string employerCountry = string.Empty;
            string employerZip = string.Empty;
            string employerZipExt = string.Empty;
            string employerZipCode = string.Empty;

            string priorityCode = fromReader.GetString( COLUMN_PRIORITYCODE ).TrimEnd();

            if ( priorityCode == "1" )
            {
                employerAddress = fromReader.GetString( COLUMN_PRIMARYEMPADDRESS ).TrimEnd();
                employerAddress = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash
                        (employerAddress);

                employerLoc = fromReader.GetString( COLUMN_PRIMARYLOC ).TrimEnd();

                employerZip = fromReader.GetString( COLUMN_PRIMARYEMPZIP ).TrimEnd();
                employerZip = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( employerZip );

                employerZipExt = fromReader.GetString( COLUMN_PRIMARYEMPZIPEXT ).TrimEnd();
                employerZipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( employerZip + employerZipExt );
            }
            else //if( priorityCode == "2" )
            {
                employerAddress = fromReader.GetString( COLUMN_SECONDARYEMPADDRESS ).TrimEnd();
                employerAddress = StringFilter.
                    RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( employerAddress );

                employerLoc = fromReader.GetString( COLUMN_SECONDARYLOC ).TrimEnd();

                employerZip = fromReader.GetString( COLUMN_SECONDARYEMPZIP ).TrimEnd();
                employerZip = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( employerZip );

                employerZipExt = fromReader.GetString( COLUMN_SECONDARYEMPZIPEXT ).TrimEnd();
                employerZipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( employerZip + employerZipExt );
            }

            if ( employerLoc.Length >= 16 )
            {
                employerCity = employerLoc.Substring( 0, 16 );
            }

            if ( employerLoc.Length >= 18 )
            {
                employerState = employerLoc.Substring( 16, 2 );
            }

            if ( employerLoc.Length >= 19 )
            {
                employerCountry = employerLoc.Substring( 18 ); //considered as country code
            }
            employerCity = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( employerCity );

            Address anAddress = new Address( employerAddress, String.Empty, employerCity, new ZipCode( employerZipCode ),
                addressBroker.StateWith(facilityID,employerState), addressBroker.CountryWith(facilityID, employerCountry));
            return anAddress;
        }

        private PhoneNumber ReadPhoneNumberFrom( SafeReader fromReader )
        {
            PhoneNumber aPhoneNumber = null;
            string employerPhone = null;
            string priorityCode;

            priorityCode = fromReader.GetString( COLUMN_PRIORITYCODE ).TrimEnd();

            if ( priorityCode == "1" )
            {
                employerPhone = fromReader.GetString( COLUMN_PRIMARYPHONE ).TrimEnd();
            }
            else //="2"
            {
                employerPhone = fromReader.GetString( COLUMN_SECONDARYPHONE ).TrimEnd();
            }

            if ( employerPhone.Length >= 10 )
            {
                aPhoneNumber = new PhoneNumber( employerPhone.Substring( 0, 3 ),
                    employerPhone.Substring( 3, 7 ) );
            }
            return aPhoneNumber;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuredEmployerAddressStrategy()
        {
        }

        public InsuredEmployerAddressStrategy( string cxnString )
            : base( cxnString )
        {
        }

        public InsuredEmployerAddressStrategy( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string

            COLUMN_PRIMARYEMPADDRESS = "PRIMARYEMPADDRESS",
            COLUMN_PRIMARYLOC = "PRIMARYLOC",
            COLUMN_PRIMARYEMPZIP = "PRIMARYEMPZIP",
            COLUMN_PRIMARYEMPZIPEXT = "PRIMARYEMPZIPEXT",

            COLUMN_SECONDARYEMPADDRESS = "SECONDARYEMPADDRESS",
            COLUMN_SECONDARYLOC = "SECONDARYLOC",
            COLUMN_SECONDARYEMPZIP = "SECONDARYEMPZIP",
            COLUMN_SECONDARYEMPZIPEXT = "SECONDARYEMPZIPEXT",

            COLUMN_PRIORITYCODE = "PRIORITYCODE",

            COLUMN_PRIMARYPHONE = "PRIMARYPHONE",
            COLUMN_SECONDARYPHONE = "SECONDARYPHONE";

        #endregion
    }

}
