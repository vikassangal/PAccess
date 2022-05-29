using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;


namespace Hsd.PerotSystems.PatientAccess.Services.Cfdb
{

    /// <summary>
    /// 
    /// </summary>
    public class CfdbService : ICFDBLookupServiceSoap
    {

        private static XmlDocument c_AddressDocument;
        private static XmlDocument c_PhoneDocument;
        private static XmlNode c_AddressNotFoundNode;
        private static XmlNode c_PhoneNotFoundNode;

        static CfdbService()
        {

            string appDataPath =
                HttpContext.Current.Request.PhysicalApplicationPath + "\\App_Data";
            
            c_AddressDocument = new XmlDocument();
            c_AddressDocument.Load( appDataPath + "\\FacilityAddresses.xml" );
            
            c_PhoneDocument = new XmlDocument();
            c_PhoneDocument.Load( appDataPath + "\\FacilityPhones.xml" );

            c_AddressNotFoundNode = c_AddressDocument.CreateElement( "Addresses" );
            c_PhoneNotFoundNode = c_PhoneDocument.CreateElement( "Phones" );

        }//method

        public string GetFacilityDetails( string HspCD, string FacilityID, string PartialName )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public string GetChildFacilities( string ParentHspCD )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode AllFacilitiesUnderLevel( string OrgTypeCD, string LevelCD )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode GetApplicationFacilityData( int AppID, string HspCode, string FacilityStatusCD, string FacilityTypeCD )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode FacilitiesForLevelParentsOnly( string OrgTypeCD, string LevelCD )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode FacilitiesForLevel( string OrgTypeCD, string LevelCD )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode GetFacilityList( object[] typeCodes, object[] statusCodes )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode GetFacilityList( string typeCodes, string statusCodes )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode GetActiveFacilitiesListInteractive( string typeCodes )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode CorporateLevelList( string OrganizationTypeCD )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode CorporateLevelAndFacilityList( string OrganizationTypeCD )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode CorporateLevelAndFacilityApplicationList( string OrganizationTypeCD, string AppIDorName, string ShowAffiliationFrom )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode CFDBApplicationList()
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode GetHistoryXML( int historyID )
        {
            throw new NotImplementedException( NOT_IMPLEMENTED );
        }

        public System.Xml.XmlNode GetFacilityAddresses( string HspCD )
        {
            
            XmlNode returnNode = 
                c_AddressDocument.SelectSingleNode(
                    String.Format("/Facilities/Facility[@name='{0}']/Addresses", HspCD ) );

            if( null == returnNode )
            {

                returnNode = c_AddressNotFoundNode;

            }//if

            return returnNode;
            
        }

        public System.Xml.XmlNode GetFacilityPhones( string HspCD )
        {

            XmlNode returnNode =
                c_PhoneDocument.SelectSingleNode(
                    String.Format( "/Facilities/Facility[@name='{0}']/Phones", HspCD ) );

            if( null == returnNode )
            {

                returnNode = c_PhoneNotFoundNode;

            }//if

            return returnNode;

        }

        private const string NOT_IMPLEMENTED = "This is a stub service. This method is not implemented";

    }//class

}//namespace
