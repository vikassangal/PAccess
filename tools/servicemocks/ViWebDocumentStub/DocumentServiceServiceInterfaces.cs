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

namespace Hsd.PerotSystems.PatientAccess.Services.ViWeb
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.Web.Services.WebServiceBindingAttribute( Name = "DocumentServiceSoapBinding", Namespace = "http://vi.webservices.tenethealth.com" )]
    [System.Xml.Serialization.SoapIncludeAttribute( typeof( NonCashDocument ) )]
    [System.Xml.Serialization.SoapIncludeAttribute( typeof( FaxDocument ) )]
    [System.Xml.Serialization.SoapIncludeAttribute( typeof( CashDocument ) )]
    public interface IDocumentServiceSoapBinding
    {

        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute( "", RequestNamespace = "http://vi.webservices.tenethealth.com", ResponseNamespace = "http://vi.webservices.tenethealth.com" )]
        [return: System.Xml.Serialization.SoapElementAttribute( "findDocumentsForReturn" )]
        DocumentListResponse findDocumentsFor( DocumentListRequest dlReq );
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.SoapTypeAttribute( Namespace = "urn:DocumentService" )]
    public partial class DocumentListRequest
    {

        private string accountNumberField;

        private string appGUIDField;

        private string appKEYField;

        private string hospitalServiceCodeField;

        private string medicalRecordNumberField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string accountNumber
        {
            get
            {
                return this.accountNumberField;
            }
            set
            {
                this.accountNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string appGUID
        {
            get
            {
                return this.appGUIDField;
            }
            set
            {
                this.appGUIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string appKEY
        {
            get
            {
                return this.appKEYField;
            }
            set
            {
                this.appKEYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string hospitalServiceCode
        {
            get
            {
                return this.hospitalServiceCodeField;
            }
            set
            {
                this.hospitalServiceCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string medicalRecordNumber
        {
            get
            {
                return this.medicalRecordNumberField;
            }
            set
            {
                this.medicalRecordNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.SoapTypeAttribute( Namespace = "urn:DocumentService" )]
    public partial class NonCashDocument
    {

        private string amountField;

        private string checkBatchNumberField;

        private System.Nullable<System.DateTime> documentDateField;

        private string documentIdField;

        private string documentTypeField;

        private string patientNameField;

        private string payorNameField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string checkBatchNumber
        {
            get
            {
                return this.checkBatchNumberField;
            }
            set
            {
                this.checkBatchNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public System.Nullable<System.DateTime> documentDate
        {
            get
            {
                return this.documentDateField;
            }
            set
            {
                this.documentDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string documentId
        {
            get
            {
                return this.documentIdField;
            }
            set
            {
                this.documentIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string documentType
        {
            get
            {
                return this.documentTypeField;
            }
            set
            {
                this.documentTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string patientName
        {
            get
            {
                return this.patientNameField;
            }
            set
            {
                this.patientNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string payorName
        {
            get
            {
                return this.payorNameField;
            }
            set
            {
                this.payorNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.SoapTypeAttribute( Namespace = "urn:DocumentService" )]
    public partial class FaxDocument
    {

        private string accountNumberField;

        private System.Nullable<System.DateTime> documentDateField;

        private string documentIdField;

        private string documentTypeField;

        private string faxGUIDField;

        private string mrNumberField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string accountNumber
        {
            get
            {
                return this.accountNumberField;
            }
            set
            {
                this.accountNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public System.Nullable<System.DateTime> documentDate
        {
            get
            {
                return this.documentDateField;
            }
            set
            {
                this.documentDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string documentId
        {
            get
            {
                return this.documentIdField;
            }
            set
            {
                this.documentIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string documentType
        {
            get
            {
                return this.documentTypeField;
            }
            set
            {
                this.documentTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string faxGUID
        {
            get
            {
                return this.faxGUIDField;
            }
            set
            {
                this.faxGUIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string mrNumber
        {
            get
            {
                return this.mrNumberField;
            }
            set
            {
                this.mrNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.SoapTypeAttribute( Namespace = "urn:DocumentService" )]
    public partial class CashDocument
    {

        private string amountField;

        private string checkBatchNumberField;

        private System.Nullable<System.DateTime> documentDateField;

        private string documentIdField;

        private string documentTypeField;

        private string patientNameField;

        private string payorNameField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string checkBatchNumber
        {
            get
            {
                return this.checkBatchNumberField;
            }
            set
            {
                this.checkBatchNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public System.Nullable<System.DateTime> documentDate
        {
            get
            {
                return this.documentDateField;
            }
            set
            {
                this.documentDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string documentId
        {
            get
            {
                return this.documentIdField;
            }
            set
            {
                this.documentIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string documentType
        {
            get
            {
                return this.documentTypeField;
            }
            set
            {
                this.documentTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string patientName
        {
            get
            {
                return this.patientNameField;
            }
            set
            {
                this.patientNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string payorName
        {
            get
            {
                return this.payorNameField;
            }
            set
            {
                this.payorNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.SoapTypeAttribute( Namespace = "urn:DocumentService" )]
    public partial class Account
    {

        private string accountNumberField;

        private CashDocument[] cashDocumentsField;

        private FaxDocument[] faxDocumentsField;

        private string hspCodeField;

        private NonCashDocument[] nonCashDocumentsField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string accountNumber
        {
            get
            {
                return this.accountNumberField;
            }
            set
            {
                this.accountNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public CashDocument[] cashDocuments
        {
            get
            {
                return this.cashDocumentsField;
            }
            set
            {
                this.cashDocumentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public FaxDocument[] faxDocuments
        {
            get
            {
                return this.faxDocumentsField;
            }
            set
            {
                this.faxDocumentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public string hspCode
        {
            get
            {
                return this.hspCodeField;
            }
            set
            {
                this.hspCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public NonCashDocument[] nonCashDocuments
        {
            get
            {
                return this.nonCashDocumentsField;
            }
            set
            {
                this.nonCashDocumentsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute( "wsdl", "2.0.50727.42" )]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.SoapTypeAttribute( Namespace = "urn:DocumentService" )]
    public partial class DocumentListResponse
    {

        private Account accountField;

        private bool documentsWereFoundField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute( IsNullable = true )]
        public Account account
        {
            get
            {
                return this.accountField;
            }
            set
            {
                this.accountField = value;
            }
        }

        /// <remarks/>
        public bool documentsWereFound
        {
            get
            {
                return this.documentsWereFoundField;
            }
            set
            {
                this.documentsWereFoundField = value;
            }
        }
    }
}