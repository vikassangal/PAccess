//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace PatientAccess.AddressValidationProxy {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="AddressValidationServicePortBinding", Namespace="urn:AddressValidationService/types")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(serviceRequest))]
    public partial class AddressValidationService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback validateAddressOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public AddressValidationService() {
            this.Url = global::PatientAccess.Properties.Settings.Default.PatientAccess_AppServer_AddressValidationProxy_AddressValidationService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event validateAddressCompletedEventHandler validateAddressCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="urn:AddressValidationService/types", ResponseNamespace="urn:AddressValidationService/types", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("result", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public addressValidationResult validateAddress([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] addressValidationRequest AddressValidationRequest_1) {
            object[] results = this.Invoke("validateAddress", new object[] {
                        AddressValidationRequest_1});
            return ((addressValidationResult)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginvalidateAddress(addressValidationRequest AddressValidationRequest_1, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("validateAddress", new object[] {
                        AddressValidationRequest_1}, callback, asyncState);
        }
        
        /// <remarks/>
        public addressValidationResult EndvalidateAddress(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((addressValidationResult)(results[0]));
        }
        
        /// <remarks/>
        public void validateAddressAsync(addressValidationRequest AddressValidationRequest_1) {
            this.validateAddressAsync(AddressValidationRequest_1, null);
        }
        
        /// <remarks/>
        public void validateAddressAsync(addressValidationRequest AddressValidationRequest_1, object userState) {
            if ((this.validateAddressOperationCompleted == null)) {
                this.validateAddressOperationCompleted = new System.Threading.SendOrPostCallback(this.OnvalidateAddressOperationCompleted);
            }
            this.InvokeAsync("validateAddress", new object[] {
                        AddressValidationRequest_1}, this.validateAddressOperationCompleted, userState);
        }
        
        private void OnvalidateAddressOperationCompleted(object arg) {
            if ((this.validateAddressCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.validateAddressCompleted(this, new validateAddressCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:AddressValidationService/types")]
    public partial class addressValidationRequest : serviceRequest {
        
        private address addressField;
        
        private int truncationLimitField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public address address {
            get {
                return this.addressField;
            }
            set {
                this.addressField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int truncationLimit {
            get {
                return this.truncationLimitField;
            }
            set {
                this.truncationLimitField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(extendedAddress))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:AddressValidationService/types")]
    public partial class address {
        
        private string cityField;
        
        private string stateField;
        
        private string streetField;
        
        private string street2Field;
        
        private string zipField;
        
        private string zipExtensionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string city {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string state {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string street {
            get {
                return this.streetField;
            }
            set {
                this.streetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string street2 {
            get {
                return this.street2Field;
            }
            set {
                this.street2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string zip {
            get {
                return this.zipField;
            }
            set {
                this.zipField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string zipExtension {
            get {
                return this.zipExtensionField;
            }
            set {
                this.zipExtensionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:AddressValidationService/types")]
    public partial class addressValidationStatus {
        
        private string statusCodeField;
        
        private string statusDescriptionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string statusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string statusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:AddressValidationService/types")]
    public partial class addressValidationResult {
        
        private extendedAddress[] matchingAddressesField;
        
        private addressValidationStatus statusField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("matchingAddresses", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public extendedAddress[] matchingAddresses {
            get {
                return this.matchingAddressesField;
            }
            set {
                this.matchingAddressesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public addressValidationStatus status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:AddressValidationService/types")]
    public partial class extendedAddress : address {
        
        private fipsStateAndCounty fipsStateAndCountyField;
        
        private string postDirectionalField;
        
        private string preDirectionalField;
        
        private string secAddressNumberField;
        
        private string secAddressTypeField;
        
        private string streetNameField;
        
        private string streetNumberField;
        
        private string streetSuffixField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public fipsStateAndCounty fipsStateAndCounty {
            get {
                return this.fipsStateAndCountyField;
            }
            set {
                this.fipsStateAndCountyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string postDirectional {
            get {
                return this.postDirectionalField;
            }
            set {
                this.postDirectionalField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string preDirectional {
            get {
                return this.preDirectionalField;
            }
            set {
                this.preDirectionalField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string secAddressNumber {
            get {
                return this.secAddressNumberField;
            }
            set {
                this.secAddressNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string secAddressType {
            get {
                return this.secAddressTypeField;
            }
            set {
                this.secAddressTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string streetName {
            get {
                return this.streetNameField;
            }
            set {
                this.streetNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string streetNumber {
            get {
                return this.streetNumberField;
            }
            set {
                this.streetNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string streetSuffix {
            get {
                return this.streetSuffixField;
            }
            set {
                this.streetSuffixField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:AddressValidationService/types")]
    public partial class fipsStateAndCounty {
        
        private string countyCodeField;
        
        private string stateCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string countyCode {
            get {
                return this.countyCodeField;
            }
            set {
                this.countyCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string stateCode {
            get {
                return this.stateCodeField;
            }
            set {
                this.stateCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(addressValidationRequest))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:AddressValidationService/types")]
    public partial class serviceRequest {
        
        private int customerIdField;
        
        private string hspcdField;
        
        private string userIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int customerId {
            get {
                return this.customerIdField;
            }
            set {
                this.customerIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string hspcd {
            get {
                return this.hspcdField;
            }
            set {
                this.hspcdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string userId {
            get {
                return this.userIdField;
            }
            set {
                this.userIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void validateAddressCompletedEventHandler(object sender, validateAddressCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class validateAddressCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal validateAddressCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public addressValidationResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((addressValidationResult)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591