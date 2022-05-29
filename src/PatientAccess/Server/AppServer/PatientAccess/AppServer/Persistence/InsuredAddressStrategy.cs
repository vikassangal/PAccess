using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class InsuredAddressStrategy : AddressReaderStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void LoadContactPointOn( Party aParty, SafeReader fromReader, long facilityID )
    {
            // see if contact point exits if it does not then create one
            ContactPoint contactPoint = null;
            ArrayList contactPoints = (ArrayList)aParty.ContactPoints;
            if(contactPoints.Count == 0)
            {
                contactPoint = new ContactPoint();
                aParty.AddContactPoint(contactPoint);
            }
            else
            {
                contactPoint = (ContactPoint)contactPoints[0];
            }
            // set the address on the contact point;
            contactPoint.TypeOfContactPoint = TypeOfContactPoint.NewPhysicalContactPointType();

            Address anAddress = ReadAddressFrom( fromReader , facilityID);
            contactPoint.Address = anAddress;
            PhoneNumber phoneNumber = ReadPhoneNumberFrom( fromReader );
            contactPoint.PhoneNumber = phoneNumber;

            PhoneNumber mobilPhone = ReadMobilPhoneFrom( fromReader );
            if( mobilPhone != null )
            {
                ContactPoint cp2 = new ContactPoint();
                cp2.PhoneNumber = mobilPhone;
                cp2.TypeOfContactPoint = TypeOfContactPoint.NewMobileContactPointType();
                aParty.AddContactPoint(cp2);
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private Address ReadAddressFrom( SafeReader fromReader, long facilityID )
        {
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();

            string insuredAddress1 = fromReader.GetString( COLUMN_INSUREDADDRESS1EXTENDED ).TrimEnd();
            if (string.IsNullOrEmpty(insuredAddress1))
            {
                insuredAddress1 = fromReader.GetString(COLUMN_INSUREDADDRESS).TrimEnd();
            }

            insuredAddress1 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( insuredAddress1 );

            string insuredAddress2 = fromReader.GetString(COLUMN_INSUREDADDRESS2EXTENDED).TrimEnd();
            
            insuredAddress2 = StringFilter.
                RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( insuredAddress2 );

            string insuredCity = fromReader.GetString( COLUMN_INSUREDCITY ).TrimEnd();
            insuredCity = StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( insuredCity );

            string insuredState = fromReader.GetString( COLUMN_INSUREDSTATE ).TrimEnd();

            string insuredZip = fromReader.GetString( COLUMN_INSUREDZIP ).TrimEnd();
            string insuredZipExt = fromReader.GetString( COLUMN_INSUREDZIPEXT ).TrimEnd();
            if( insuredZipExt == "0" )
            {
                insuredZipExt = string.Empty;
            }
            string insuredZipCode = StringFilter.RemoveAllNonLetterNumberSpaceAndHyphen( insuredZip + insuredZipExt );

            string insuredCountryCode = fromReader.GetString( COLUMN_INSUREDCOUNTRYCODE ).TrimEnd();

            Address anAddress = new Address(insuredAddress1, insuredAddress2, insuredCity, new ZipCode(insuredZipCode),
                addressBroker.StateWith(facilityID, insuredState ), addressBroker.CountryWith( facilityID, insuredCountryCode ) );
           
            return anAddress;
        }

        private PhoneNumber ReadPhoneNumberFrom( SafeReader reader )
        {
            PhoneNumber aPhoneNumber = null;

            string insuredAreaCode1 = reader.GetInt64( COLUMN_INSUREDAREACODE1 ).ToString();
            insuredAreaCode1 = insuredAreaCode1.PadLeft(3,'0');

            string insuredPhoneNumber1 = reader.GetInt64( COLUMN_INSUREDPHONENUMBER1 ).ToString();            
            insuredPhoneNumber1 = insuredPhoneNumber1.PadLeft(7,'0');

            string insuredAreaCode2 = reader.GetInt64( COLUMN_INSUREDAREACODE2 ).ToString();
            insuredAreaCode2 = insuredAreaCode2.PadLeft(3,'0');

            string insuredPhoneNumber2 = reader.GetInt64( COLUMN_INSUREDPHONENUMBER2 ).ToString();        
            insuredPhoneNumber2 = insuredPhoneNumber2.PadLeft(7,'0');

            if( insuredAreaCode1 != "0" && insuredPhoneNumber1 != "0" )
            {
                aPhoneNumber = new PhoneNumber( insuredAreaCode1, insuredPhoneNumber1 );
            }

            return aPhoneNumber;
        } 

        private PhoneNumber ReadMobilPhoneFrom( SafeReader reader )
        {
            PhoneNumber aPhoneNumber = null;

            string insuredCellPhone;

            insuredCellPhone = reader.GetString( COLUMN_INSUREDCELLPHONE ).TrimEnd();                      

            // sadly, pbar can not be relied upon to have phone numbers in the phone number
            // field. In some cases this field is used to hold a workstation ID. 
            // So it is possible to read a non numeric phone number. 
            // if this happens create a default, empty phone number
            try
            {
                aPhoneNumber = new PhoneNumber(insuredCellPhone );
            }
            catch (FormatException ex)
            {
                c_log.Error("Received Phone number with incorrect format: " + insuredCellPhone, ex);
                aPhoneNumber = new PhoneNumber();
            }
            return aPhoneNumber;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuredAddressStrategy()
        {
        }

        public InsuredAddressStrategy( string cxnString ) : base( cxnString )
        {
        }

        public InsuredAddressStrategy( IDbTransaction txn ) : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( InsuredAddressStrategy ) );
        #endregion

        #region Constants       

        private const string
                   
            COLUMN_INSUREDADDRESS = "INSUREDADDRESS",
            COLUMN_INSUREDADDRESS1EXTENDED = "INSUREDADDRESS1EXTENDED",
            COLUMN_INSUREDADDRESS2EXTENDED = "INSUREDADDRESS2EXTENDED",
            COLUMN_INSUREDCITY = "INSUREDCITY",
            COLUMN_INSUREDSTATE = "INSUREDSTATE",
            COLUMN_INSUREDZIP = "INSUREDZIP",
            COLUMN_INSUREDZIPEXT = "INSUREDZIPEXT",

            COLUMN_INSUREDCOUNTRYCODE = "INSUREDCOUNTRYCODE",
            COLUMN_INSUREDAREACODE1 = "INSUREDAREACODE1",
            COLUMN_INSUREDPHONENUMBER1 = "INSUREDPHONENUMBER1",            
            COLUMN_INSUREDAREACODE2 = "INSUREDAREACODE2",
            COLUMN_INSUREDPHONENUMBER2 = "INSUREDPHONENUMBER2",                
            COLUMN_INSUREDCELLPHONE = "INSUREDCELLPHONE";
            
        #endregion
    }

}
