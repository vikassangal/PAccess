﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.42.
// 

namespace Hsd.PerotSystems.PatientAccess.Services.Cfdb
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.Web.Services.WebServiceBindingAttribute( Name = "CFDBLookupServiceSoap", Namespace = "http://tempuri.org/" )]
    [System.Xml.Serialization.XmlIncludeAttribute( typeof( object[] ) )]
    public interface ICFDBLookupServiceSoap
    {

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "http://tempuri.org/GetFacilityDetails", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped )]
        string GetFacilityDetails( string HspCD, string FacilityID, string PartialName );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "http://tempuri.org/GetChildFacilities", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped )]
        string GetChildFacilities( string ParentHspCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "AllFacilitiesUnderLevel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode AllFacilitiesUnderLevel( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string OrgTypeCD, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string LevelCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "GetApplicationFacilityData", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode GetApplicationFacilityData( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] int AppID, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string HspCode, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string FacilityStatusCD, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string FacilityTypeCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "FacilitiesForLevelParentsOnly", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode FacilitiesForLevelParentsOnly( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string OrgTypeCD, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string LevelCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "FacilitiesForLevel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode FacilitiesForLevel( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string OrgTypeCD, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string LevelCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "http://tempuri.org/GetFacilityList", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped )]
        System.Xml.XmlNode GetFacilityList( object[] typeCodes, object[] statusCodes );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute( MessageName = "GetFacilityList1" )]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "http://tempuri.org/GetFacilityListInteractive", RequestElementName = "GetFacilityListInteractive", RequestNamespace = "http://tempuri.org/", ResponseElementName = "GetFacilityListInteractiveResponse", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped )]
        [return: System.Xml.Serialization.XmlElementAttribute( "GetFacilityListInteractiveResult" )]
        System.Xml.XmlNode GetFacilityList( string typeCodes, string statusCodes );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "http://tempuri.org/GetActiveFacilitiesListInteractive", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped )]
        System.Xml.XmlNode GetActiveFacilitiesListInteractive( string typeCodes );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "CorporateLevelList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode CorporateLevelList( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string OrganizationTypeCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "CorporateLevelAndFacilityList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode CorporateLevelAndFacilityList( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string OrganizationTypeCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "CorporateLevelAndFacilityApplicationList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode CorporateLevelAndFacilityApplicationList( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string OrganizationTypeCD, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string AppIDorName, [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string ShowAffiliationFrom );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "CFDBApplicationList", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode CFDBApplicationList();

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "GetHistoryXML", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode GetHistoryXML( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] int historyID );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "GetFacilityAddresses", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode GetFacilityAddresses( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string HspCD );

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute( "GetFacilityPhones", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare )]
        [return: System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )]
        System.Xml.XmlNode GetFacilityPhones( [System.Xml.Serialization.XmlElementAttribute( Namespace = "http://tempuri.org/" )] string HspCD );
    }
}