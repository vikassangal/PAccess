﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PatientAccess.PriorAccountBalanceProxy {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:PriorAccountBalanceService/types", ConfigurationName="PriorAccountBalanceProxy.PriorAccountBalanceServiceSoap")]
    public interface PriorAccountBalanceServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(serviceRequest))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="result")]
        PatientAccess.PriorAccountBalanceProxy.priorAccountBalanceResult identifyPriorAccountBalances(PatientAccess.PriorAccountBalanceProxy.priorAccountBalanceRequest PriorAccountBalanceRequest_1);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:PriorAccountBalanceService/types")]
    public partial class priorAccountBalanceRequest : serviceRequest {
        
        private string medicalRecordNumberField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string medicalRecordNumber {
            get {
                return this.medicalRecordNumberField;
            }
            set {
                this.medicalRecordNumberField = value;
                this.RaisePropertyChanged("medicalRecordNumber");
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(priorAccountBalanceRequest))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:PriorAccountBalanceService/types")]
    public partial class serviceRequest : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int customerIdField;
        
        private string hspcdField;
        
        private string userIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public int customerId {
            get {
                return this.customerIdField;
            }
            set {
                this.customerIdField = value;
                this.RaisePropertyChanged("customerId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string hspcd {
            get {
                return this.hspcdField;
            }
            set {
                this.hspcdField = value;
                this.RaisePropertyChanged("hspcd");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string userId {
            get {
                return this.userIdField;
            }
            set {
                this.userIdField = value;
                this.RaisePropertyChanged("userId");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:PriorAccountBalanceService/types")]
    public partial class patientType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string codeField;
        
        private string descriptionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
                this.RaisePropertyChanged("code");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
                this.RaisePropertyChanged("description");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:PriorAccountBalanceService/types")]
    public partial class financialClass : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string codeField;
        
        private string descriptionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
                this.RaisePropertyChanged("code");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
                this.RaisePropertyChanged("description");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:PriorAccountBalanceService/types")]
    public partial class account : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string aceDebtorIdField;
        
        private string dischargeDateField;
        
        private financialClass financialClassField;
        
        private string patientAccountNumberField;
        
        private patientType patientTypeField;
        
        private bool paymentPlanField;
        
        private float principalBalanceField;
        
        private float totalBalanceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string aceDebtorId {
            get {
                return this.aceDebtorIdField;
            }
            set {
                this.aceDebtorIdField = value;
                this.RaisePropertyChanged("aceDebtorId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string dischargeDate {
            get {
                return this.dischargeDateField;
            }
            set {
                this.dischargeDateField = value;
                this.RaisePropertyChanged("dischargeDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public financialClass financialClass {
            get {
                return this.financialClassField;
            }
            set {
                this.financialClassField = value;
                this.RaisePropertyChanged("financialClass");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string patientAccountNumber {
            get {
                return this.patientAccountNumberField;
            }
            set {
                this.patientAccountNumberField = value;
                this.RaisePropertyChanged("patientAccountNumber");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public patientType patientType {
            get {
                return this.patientTypeField;
            }
            set {
                this.patientTypeField = value;
                this.RaisePropertyChanged("patientType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public bool paymentPlan {
            get {
                return this.paymentPlanField;
            }
            set {
                this.paymentPlanField = value;
                this.RaisePropertyChanged("paymentPlan");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public float principalBalance {
            get {
                return this.principalBalanceField;
            }
            set {
                this.principalBalanceField = value;
                this.RaisePropertyChanged("principalBalance");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public float totalBalance {
            get {
                return this.totalBalanceField;
            }
            set {
                this.totalBalanceField = value;
                this.RaisePropertyChanged("totalBalance");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:PriorAccountBalanceService/types")]
    public partial class priorAccountBalanceResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private account[] accountsField;
        
        private bool statusField;
        
        private bool statusFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("accounts", IsNullable=true, Order=0)]
        public account[] accounts {
            get {
                return this.accountsField;
            }
            set {
                this.accountsField = value;
                this.RaisePropertyChanged("accounts");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public bool status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
                this.RaisePropertyChanged("status");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool statusSpecified {
            get {
                return this.statusFieldSpecified;
            }
            set {
                this.statusFieldSpecified = value;
                this.RaisePropertyChanged("statusSpecified");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface PriorAccountBalanceServiceSoapChannel : PatientAccess.PriorAccountBalanceProxy.PriorAccountBalanceServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PriorAccountBalanceServiceSoapClient : System.ServiceModel.ClientBase<PatientAccess.PriorAccountBalanceProxy.PriorAccountBalanceServiceSoap>, PatientAccess.PriorAccountBalanceProxy.PriorAccountBalanceServiceSoap {
        
        public PriorAccountBalanceServiceSoapClient() {
        }
        
        public PriorAccountBalanceServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PriorAccountBalanceServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PriorAccountBalanceServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PriorAccountBalanceServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public PatientAccess.PriorAccountBalanceProxy.priorAccountBalanceResult identifyPriorAccountBalances(PatientAccess.PriorAccountBalanceProxy.priorAccountBalanceRequest PriorAccountBalanceRequest_1) {
            return base.Channel.identifyPriorAccountBalances(PriorAccountBalanceRequest_1);
        }
    }
}