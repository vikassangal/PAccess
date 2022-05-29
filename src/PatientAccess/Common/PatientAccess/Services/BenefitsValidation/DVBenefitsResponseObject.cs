using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PatientAccess.Services.BenefitsValidation
{

    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]

    public partial class BenefitResponse
    {

        private TranSetHeader[] tranSetHeaderField;

        private BeginningHierarchicalTran[] beginningHierarchicalTranField;

        private InformationSourceLevel informationSourceLevelField;

        private TranSetTrailer[] tranSetTrailerField;

        /// <remarks/>
        [XmlElement( "TranSetHeader" )]
        public TranSetHeader[] TranSetHeader
        {
            get
            {
                return this.tranSetHeaderField;
            }
            set
            {
                this.tranSetHeaderField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "BeginningHierarchicalTran" )]
        public BeginningHierarchicalTran[] BeginningHierarchicalTran
        {
            get
            {
                return this.beginningHierarchicalTranField;
            }
            set
            {
                this.beginningHierarchicalTranField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "InformationSourceLevel" )]
        public InformationSourceLevel InformationSourceLevel
        {
            get
            {
                return this.informationSourceLevelField;
            }
            set
            {
                this.informationSourceLevelField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TranSetTrailer" )]
        public TranSetTrailer[] TranSetTrailer
        {
            get
            {
                return this.tranSetTrailerField;
            }
            set
            {
                this.tranSetTrailerField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TranSetHeader
    {

        private TransactionSetIdentifierCode[] transactionSetIdentifierCodeField;

        private TransactionSetControlNumber[] transactionSetControlNumberField;

        /// <remarks/>
        [XmlElement( "TransactionSetIdentifierCode" )]
        public TransactionSetIdentifierCode[] TransactionSetIdentifierCode
        {
            get
            {
                return this.transactionSetIdentifierCodeField;
            }
            set
            {
                this.transactionSetIdentifierCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TransactionSetControlNumber" )]
        public TransactionSetControlNumber[] TransactionSetControlNumber
        {
            get
            {
                return this.transactionSetControlNumberField;
            }
            set
            {
                this.transactionSetControlNumberField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TransactionSetIdentifierCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TransactionSetControlNumber
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class BeginningHierarchicalTran
    {

        private HierarchicalStructureCode[] hierarchicalStructureCodeField;

        private TransactionSetPurposeCode[] transactionSetPurposeCodeField;

        private ReferenceIdentification[] referenceIdentificationField;

        private Date[] dateField;

        private Time[] timeField;

        private TransactionTypeCode[] transactionTypeCodeField;

        /// <remarks/>
        [XmlElement( "HierarchicalStructureCode" )]
        public HierarchicalStructureCode[] HierarchicalStructureCode
        {
            get
            {
                return this.hierarchicalStructureCodeField;
            }
            set
            {
                this.hierarchicalStructureCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TransactionSetPurposeCode" )]
        public TransactionSetPurposeCode[] TransactionSetPurposeCode
        {
            get
            {
                return this.transactionSetPurposeCodeField;
            }
            set
            {
                this.transactionSetPurposeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification" )]
        public ReferenceIdentification[] ReferenceIdentification
        {
            get
            {
                return this.referenceIdentificationField;
            }
            set
            {
                this.referenceIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Date" )]
        public Date[] Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Time" )]
        public Time[] Time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TransactionTypeCode" )]
        public TransactionTypeCode[] TransactionTypeCode
        {
            get
            {
                return this.transactionTypeCodeField;
            }
            set
            {
                this.transactionTypeCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class HierarchicalStructureCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TransactionSetPurposeCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentification
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Date
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Time
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TransactionTypeCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class InformationSourceLevel
    {

        private HierarchicalLevel[] hierarchicalLevelField;

        private RequestValidation[] requestValidationField;

        private InformationSourceName informationSourceNameField;

        private InformationReceiverLevel informationReceiverLevelField;

        /// <remarks/>
        [XmlElement( "HierarchicalLevel" )]
        public HierarchicalLevel[] HierarchicalLevel
        {
            get
            {
                return this.hierarchicalLevelField;
            }
            set
            {
                this.hierarchicalLevelField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RequestValidation" )]
        public RequestValidation[] RequestValidation
        {
            get
            {
                return this.requestValidationField;
            }
            set
            {
                this.requestValidationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "InformationSourceName" )]
        public InformationSourceName InformationSourceName
        {
            get
            {
                return this.informationSourceNameField;
            }
            set
            {
                this.informationSourceNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "InformationReceiverLevel" )]
        public InformationReceiverLevel InformationReceiverLevel
        {
            get
            {
                return this.informationReceiverLevelField;
            }
            set
            {
                this.informationReceiverLevelField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class HierarchicalLevel
    {

        private HierarchicalIdNumber[] hierarchicalIdNumberField;

        private HierarchicalParentIdNumber[] hierarchicalParentIdNumberField;

        private HierarchicalLevelCode[] hierarchicalLevelCodeField;

        private HierarchicalChildCode[] hierarchicalChildCodeField;

        /// <remarks/>
        [XmlElement( "HierarchicalIdNumber" )]
        public HierarchicalIdNumber[] HierarchicalIdNumber
        {
            get
            {
                return this.hierarchicalIdNumberField;
            }
            set
            {
                this.hierarchicalIdNumberField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "HierarchicalParentIdNumber" )]
        public HierarchicalParentIdNumber[] HierarchicalParentIdNumber
        {
            get
            {
                return this.hierarchicalParentIdNumberField;
            }
            set
            {
                this.hierarchicalParentIdNumberField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "HierarchicalLevelCode" )]
        public HierarchicalLevelCode[] HierarchicalLevelCode
        {
            get
            {
                return this.hierarchicalLevelCodeField;
            }
            set
            {
                this.hierarchicalLevelCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "HierarchicalChildCode" )]
        public HierarchicalChildCode[] HierarchicalChildCode
        {
            get
            {
                return this.hierarchicalChildCodeField;
            }
            set
            {
                this.hierarchicalChildCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class HierarchicalIdNumber
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class HierarchicalParentIdNumber
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class HierarchicalLevelCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class HierarchicalChildCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class RequestValidation
    {

        private YesNoConditionOrResponseCode[] yesNoConditionOrResponseCodeField;

        private AgencyQualifierCode[] agencyQualifierCodeField;

        private RejectReasonCode[] rejectReasonCodeField;

        private FollowUpActionCode[] followUpActionCodeField;

        /// <remarks/>
        [XmlElement( "YesNoConditionOrResponseCode" )]
        public YesNoConditionOrResponseCode[] YesNoConditionOrResponseCode
        {
            get
            {
                return this.yesNoConditionOrResponseCodeField;
            }
            set
            {
                this.yesNoConditionOrResponseCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AgencyQualifierCode" )]
        public AgencyQualifierCode[] AgencyQualifierCode
        {
            get
            {
                return this.agencyQualifierCodeField;
            }
            set
            {
                this.agencyQualifierCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RejectReasonCode" )]
        public RejectReasonCode[] RejectReasonCode
        {
            get
            {
                return this.rejectReasonCodeField;
            }
            set
            {
                this.rejectReasonCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "FollowUpActionCode" )]
        public FollowUpActionCode[] FollowUpActionCode
        {
            get
            {
                return this.followUpActionCodeField;
            }
            set
            {
                this.followUpActionCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class YesNoConditionOrResponseCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class AgencyQualifierCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class RejectReasonCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class FollowUpActionCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class InformationSourceName
    {

        private Name nameField;

        private AdditionalIdentification additionalIdentificationField;

        private ContactInformation contactInformationField;

        private RequestValidation requestValidationField;

        /// <remarks/>
        [XmlElement( "Name" )]
        public Name Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AdditionalIdentification" )]
        public AdditionalIdentification AdditionalIdentification
        {
            get
            {
                return this.additionalIdentificationField;
            }
            set
            {
                this.additionalIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ContactInformation" )]
        public ContactInformation ContactInformation
        {
            get
            {
                return this.contactInformationField;
            }
            set
            {
                this.contactInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RequestValidation" )]
        public RequestValidation RequestValidation
        {
            get
            {
                return this.requestValidationField;
            }
            set
            {
                this.requestValidationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Name
    {

        private EntityIdentifierCode1[] entityIdentifierCode1Field;

        private EntityTypeQualifier[] entityTypeQualifierField;

        private NameLastOrOrganizationName nameLastOrOrganizationNameField;

        private NameFirst nameFirstField;

        private NameMiddle nameMiddleField;

        private NamePrefix namePrefixField;

        private NameSuffix nameSuffixField;

        private IdentificationCodeQualifier identificationCodeQualifierField;

        private IdentificationCode identificationCodeField;

        private EntityRelationshipCode[] entityRelationshipCodeField;

        private EntityIdentifierCode2[] entityIdentifierCode2Field;

        /// <remarks/>
        [XmlElement( "EntityIdentifierCode1" )]
        public EntityIdentifierCode1[] EntityIdentifierCode1
        {
            get
            {
                return this.entityIdentifierCode1Field;
            }
            set
            {
                this.entityIdentifierCode1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "EntityTypeQualifier" )]
        public EntityTypeQualifier[] EntityTypeQualifier
        {
            get
            {
                return this.entityTypeQualifierField;
            }
            set
            {
                this.entityTypeQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "NameLastOrOrganizationName" )]
        public NameLastOrOrganizationName NameLastOrOrganizationName
        {
            get
            {
                return this.nameLastOrOrganizationNameField;
            }
            set
            {
                this.nameLastOrOrganizationNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "NameFirst" )]
        public NameFirst NameFirst
        {
            get
            {
                return this.nameFirstField;
            }
            set
            {
                this.nameFirstField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "NameMiddle" )]
        public NameMiddle NameMiddle
        {
            get
            {
                return this.nameMiddleField;
            }
            set
            {
                this.nameMiddleField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "NamePrefix" )]
        public NamePrefix NamePrefix
        {
            get
            {
                return this.namePrefixField;
            }
            set
            {
                this.namePrefixField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "NameSuffix" )]
        public NameSuffix NameSuffix
        {
            get
            {
                return this.nameSuffixField;
            }
            set
            {
                this.nameSuffixField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "IdentificationCodeQualifier" )]
        public IdentificationCodeQualifier IdentificationCodeQualifier
        {
            get
            {
                return this.identificationCodeQualifierField;
            }
            set
            {
                this.identificationCodeQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "IdentificationCode" )]
        public IdentificationCode IdentificationCode
        {
            get
            {
                return this.identificationCodeField;
            }
            set
            {
                this.identificationCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "EntityRelationshipCode" )]
        public EntityRelationshipCode[] EntityRelationshipCode
        {
            get
            {
                return this.entityRelationshipCodeField;
            }
            set
            {
                this.entityRelationshipCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "EntityIdentifierCode2" )]
        public EntityIdentifierCode2[] EntityIdentifierCode2
        {
            get
            {
                return this.entityIdentifierCode2Field;
            }
            set
            {
                this.entityIdentifierCode2Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class EntityIdentifierCode1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class EntityTypeQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class NameLastOrOrganizationName
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class NameFirst
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class NameMiddle
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class NamePrefix
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class NameSuffix
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class IdentificationCodeQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class IdentificationCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class EntityRelationshipCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class EntityIdentifierCode2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class AdditionalIdentification
    {

        private ReferenceIdentificationQualifier referenceIdentificationQualifierField;

        private ReferenceIdentification referenceIdentificationField;

        private Description descriptionField;

        private ReferenceIdentifier referenceIdentifierField;

        /// <remarks/>
        [XmlElement( "ReferenceIdentificationQualifier" )]
        public ReferenceIdentificationQualifier ReferenceIdentificationQualifier
        {
            get
            {
                return this.referenceIdentificationQualifierField;
            }
            set
            {
                this.referenceIdentificationQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification" )]
        public ReferenceIdentification ReferenceIdentification
        {
            get
            {
                return this.referenceIdentificationField;
            }
            set
            {
                this.referenceIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Description" )]
        public Description Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentifier" )]
        public ReferenceIdentifier ReferenceIdentifier
        {
            get
            {
                return this.referenceIdentifierField;
            }
            set
            {
                this.referenceIdentifierField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentificationQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Description
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentifier
    {

        private Identifier[] identifierField;

        /// <remarks/>
        [XmlElement( "Identifier" )]
        public Identifier[] Identifier
        {
            get
            {
                return this.identifierField;
            }
            set
            {
                this.identifierField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Identifier
    {

        private ReferenceIdentificationQualifier1[] referenceIdentificationQualifier1Field;

        private ReferenceIdentification1[] referenceIdentification1Field;

        private ReferenceIdentificationQualifier2[] referenceIdentificationQualifier2Field;

        private ReferenceIdentification2[] referenceIdentification2Field;

        private ReferenceIdentificationQualifier3[] referenceIdentificationQualifier3Field;

        private ReferenceIdentification3[] referenceIdentification3Field;

        /// <remarks/>
        [XmlElement( "ReferenceIdentificationQualifier1" )]
        public ReferenceIdentificationQualifier1[] ReferenceIdentificationQualifier1
        {
            get
            {
                return this.referenceIdentificationQualifier1Field;
            }
            set
            {
                this.referenceIdentificationQualifier1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification1" )]
        public ReferenceIdentification1[] ReferenceIdentification1
        {
            get
            {
                return this.referenceIdentification1Field;
            }
            set
            {
                this.referenceIdentification1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentificationQualifier2" )]
        public ReferenceIdentificationQualifier2[] ReferenceIdentificationQualifier2
        {
            get
            {
                return this.referenceIdentificationQualifier2Field;
            }
            set
            {
                this.referenceIdentificationQualifier2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification2" )]
        public ReferenceIdentification2[] ReferenceIdentification2
        {
            get
            {
                return this.referenceIdentification2Field;
            }
            set
            {
                this.referenceIdentification2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentificationQualifier3" )]
        public ReferenceIdentificationQualifier3[] ReferenceIdentificationQualifier3
        {
            get
            {
                return this.referenceIdentificationQualifier3Field;
            }
            set
            {
                this.referenceIdentificationQualifier3Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification3" )]
        public ReferenceIdentification3[] ReferenceIdentification3
        {
            get
            {
                return this.referenceIdentification3Field;
            }
            set
            {
                this.referenceIdentification3Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentificationQualifier1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentification1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentificationQualifier2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentification2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentificationQualifier3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ReferenceIdentification3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ContactInformation
    {

        private ContactFunctionCode contactFunctionCodeField;

        private ContactName contactNameField;

        private CommunicationNumberQualifier1 communicationNumberQualifier1Field;

        private CommunicationNumber1 communicationNumber1Field;

        private CommunicationNumberQualifier2 communicationNumberQualifier2Field;

        private CommunicationNumber2 communicationNumber2Field;

        private CommunicationNumberQualifier3 communicationNumberQualifier3Field;

        private CommunicationNumber3 communicationNumber3Field;

        private ContactInquiryReference contactInquiryReferenceField;

        /// <remarks/>
        [XmlElement( "ContactFunctionCode" )]
        public ContactFunctionCode ContactFunctionCode
        {
            get
            {
                return this.contactFunctionCodeField;
            }
            set
            {
                this.contactFunctionCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ContactName" )]
        public ContactName ContactName
        {
            get
            {
                return this.contactNameField;
            }
            set
            {
                this.contactNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CommunicationNumberQualifier1" )]
        public CommunicationNumberQualifier1 CommunicationNumberQualifier1
        {
            get
            {
                return this.communicationNumberQualifier1Field;
            }
            set
            {
                this.communicationNumberQualifier1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CommunicationNumber1" )]
        public CommunicationNumber1 CommunicationNumber1
        {
            get
            {
                return this.communicationNumber1Field;
            }
            set
            {
                this.communicationNumber1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CommunicationNumberQualifier2" )]
        public CommunicationNumberQualifier2 CommunicationNumberQualifier2
        {
            get
            {
                return this.communicationNumberQualifier2Field;
            }
            set
            {
                this.communicationNumberQualifier2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CommunicationNumber2" )]
        public CommunicationNumber2 CommunicationNumber2
        {
            get
            {
                return this.communicationNumber2Field;
            }
            set
            {
                this.communicationNumber2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CommunicationNumberQualifier3" )]
        public CommunicationNumberQualifier3 CommunicationNumberQualifier3
        {
            get
            {
                return this.communicationNumberQualifier3Field;
            }
            set
            {
                this.communicationNumberQualifier3Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CommunicationNumber3" )]
        public CommunicationNumber3 CommunicationNumber3
        {
            get
            {
                return this.communicationNumber3Field;
            }
            set
            {
                this.communicationNumber3Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ContactInquiryReference" )]
        public ContactInquiryReference ContactInquiryReference
        {
            get
            {
                return this.contactInquiryReferenceField;
            }
            set
            {
                this.contactInquiryReferenceField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ContactFunctionCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ContactName
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CommunicationNumberQualifier1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CommunicationNumber1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CommunicationNumberQualifier2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CommunicationNumber2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CommunicationNumberQualifier3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CommunicationNumber3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ContactInquiryReference
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class InformationReceiverLevel
    {

        private HierarchicalLevel[] hierarchicalLevelField;

        private InformationReceiverName[] informationReceiverNameField;

        private SubscriberLevel subscriberLevelField;

        /// <remarks/>
        [XmlElement( "HierarchicalLevel" )]
        public HierarchicalLevel[] HierarchicalLevel
        {
            get
            {
                return this.hierarchicalLevelField;
            }
            set
            {
                this.hierarchicalLevelField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "InformationReceiverName" )]
        public InformationReceiverName[] InformationReceiverName
        {
            get
            {
                return this.informationReceiverNameField;
            }
            set
            {
                this.informationReceiverNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SubscriberLevel" )]
        public SubscriberLevel SubscriberLevel
        {
            get
            {
                return this.subscriberLevelField;
            }
            set
            {
                this.subscriberLevelField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class InformationReceiverName
    {

        private Name[] nameField;

        private AdditionalIdentification[] additionalIdentificationField;

        private RequestValidation[] requestValidationField;

        /// <remarks/>
        [XmlElement( "Name" )]
        public Name[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AdditionalIdentification" )]
        public AdditionalIdentification[] AdditionalIdentification
        {
            get
            {
                return this.additionalIdentificationField;
            }
            set
            {
                this.additionalIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RequestValidation" )]
        public RequestValidation[] RequestValidation
        {
            get
            {
                return this.requestValidationField;
            }
            set
            {
                this.requestValidationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SubscriberLevel
    {

        private HierarchicalLevel[] hierarchicalLevelField;

        private TraceNumber[] traceNumberField;

        private SubscriberName subscriberNameField;

        /// <remarks/>
        [XmlElement( "HierarchicalLevel" )]
        public HierarchicalLevel[] HierarchicalLevel
        {
            get
            {
                return this.hierarchicalLevelField;
            }
            set
            {
                this.hierarchicalLevelField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TraceNumber" )]
        public TraceNumber[] TraceNumber
        {
            get
            {
                return this.traceNumberField;
            }
            set
            {
                this.traceNumberField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SubscriberName" )]
        public SubscriberName SubscriberName
        {
            get
            {
                return this.subscriberNameField;
            }
            set
            {
                this.subscriberNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TraceNumber
    {

        private TraceTypeCode[] traceTypeCodeField;

        private ReferenceIdentification1[] referenceIdentification1Field;

        private OriginatingCompanyIdentifier[] originatingCompanyIdentifierField;

        private ReferenceIdentification2[] referenceIdentification2Field;

        /// <remarks/>
        [XmlElement( "TraceTypeCode" )]
        public TraceTypeCode[] TraceTypeCode
        {
            get
            {
                return this.traceTypeCodeField;
            }
            set
            {
                this.traceTypeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification1" )]
        public ReferenceIdentification1[] ReferenceIdentification1
        {
            get
            {
                return this.referenceIdentification1Field;
            }
            set
            {
                this.referenceIdentification1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "OriginatingCompanyIdentifier" )]
        public OriginatingCompanyIdentifier[] OriginatingCompanyIdentifier
        {
            get
            {
                return this.originatingCompanyIdentifierField;
            }
            set
            {
                this.originatingCompanyIdentifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification2" )]
        public ReferenceIdentification2[] ReferenceIdentification2
        {
            get
            {
                return this.referenceIdentification2Field;
            }
            set
            {
                this.referenceIdentification2Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TraceTypeCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class OriginatingCompanyIdentifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SubscriberName
    {

        private Name nameField;

        private AdditionalIdentification additionalIdentificationField;

        private Address addressField;

        private CityStateZip cityStateZipField;

        private ContactInformation contactInformationField;

        private RequestValidation requestValidationField;

        private Demographic demographicField;

        private Relationship relationshipField;

        private DatePeriod datePeriodField;

        private SubscriberBenefitInformation[] subscriberBenefitInformationField;

        private DependentLevel dependentLevelField;

        /// <remarks/>
        [XmlElement( "Name" )]
        public Name Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AdditionalIdentification" )]
        public AdditionalIdentification AdditionalIdentification
        {
            get
            {
                return this.additionalIdentificationField;
            }
            set
            {
                this.additionalIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Address" )]
        public Address Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CityStateZip" )]
        public CityStateZip CityStateZip
        {
            get
            {
                return this.cityStateZipField;
            }
            set
            {
                this.cityStateZipField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ContactInformation" )]
        public ContactInformation ContactInformation
        {
            get
            {
                return this.contactInformationField;
            }
            set
            {
                this.contactInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RequestValidation" )]
        public RequestValidation RequestValidation
        {
            get
            {
                return this.requestValidationField;
            }
            set
            {
                this.requestValidationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Demographic" )]
        public Demographic Demographic
        {
            get
            {
                return this.demographicField;
            }
            set
            {
                this.demographicField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Relationship" )]
        public Relationship Relationship
        {
            get
            {
                return this.relationshipField;
            }
            set
            {
                this.relationshipField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DatePeriod" )]
        public DatePeriod DatePeriod
        {
            get
            {
                return this.datePeriodField;
            }
            set
            {
                this.datePeriodField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SubscriberBenefitInformation" )]
        public SubscriberBenefitInformation[] SubscriberBenefitInformation
        {
            get
            {
                return this.subscriberBenefitInformationField;
            }
            set
            {
                this.subscriberBenefitInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DependentLevel" )]
        public DependentLevel DependentLevel
        {
            get
            {
                return this.dependentLevelField;
            }
            set
            {
                this.dependentLevelField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Address
    {

        private AddressInformation1[] addressInformation1Field;

        private AddressInformation2[] addressInformation2Field;

        /// <remarks/>
        [XmlElement( "AddressInformation1" )]
        public AddressInformation1[] AddressInformation1
        {
            get
            {
                return this.addressInformation1Field;
            }
            set
            {
                this.addressInformation1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AddressInformation2" )]
        public AddressInformation2[] AddressInformation2
        {
            get
            {
                return this.addressInformation2Field;
            }
            set
            {
                this.addressInformation2Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class AddressInformation1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class AddressInformation2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CityStateZip
    {

        private CityName[] cityNameField;

        private StateOrProvinceCode[] stateOrProvinceCodeField;

        private PostalCode[] postalCodeField;

        private CountryCode[] countryCodeField;

        private LocationQualifier[] locationQualifierField;

        private LocationIdentifier[] locationIdentifierField;

        /// <remarks/>
        [XmlElement( "CityName" )]
        public CityName[] CityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "StateOrProvinceCode" )]
        public StateOrProvinceCode[] StateOrProvinceCode
        {
            get
            {
                return this.stateOrProvinceCodeField;
            }
            set
            {
                this.stateOrProvinceCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "PostalCode" )]
        public PostalCode[] PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CountryCode" )]
        public CountryCode[] CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "LocationQualifier" )]
        public LocationQualifier[] LocationQualifier
        {
            get
            {
                return this.locationQualifierField;
            }
            set
            {
                this.locationQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "LocationIdentifier" )]
        public LocationIdentifier[] LocationIdentifier
        {
            get
            {
                return this.locationIdentifierField;
            }
            set
            {
                this.locationIdentifierField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CityName
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class StateOrProvinceCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class PostalCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CountryCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class LocationQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class LocationIdentifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Demographic
    {

        private DateTimePeriodFormatQualifier[] dateTimePeriodFormatQualifierField;

        private DateTimePeriod dateTimePeriodField;

        private GenderCode[] genderCodeField;

        private MaritalStatusCode[] maritalStatusCodeField;

        private RaceOrEthnicityCode[] raceOrEthnicityCodeField;

        private CitizenshipStatusCode[] citizenshipStatusCodeField;

        private CountryCode[] countryCodeField;

        private BasisOfVerificationCode[] basisOfVerificationCodeField;

        private Quantity[] quantityField;

        /// <remarks/>
        [XmlElement( "DateTimePeriodFormatQualifier" )]
        public DateTimePeriodFormatQualifier[] DateTimePeriodFormatQualifier
        {
            get
            {
                return this.dateTimePeriodFormatQualifierField;
            }
            set
            {
                this.dateTimePeriodFormatQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DateTimePeriod" )]
        public DateTimePeriod DateTimePeriod
        {
            get
            {
                return this.dateTimePeriodField;
            }
            set
            {
                this.dateTimePeriodField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "GenderCode" )]
        public GenderCode[] GenderCode
        {
            get
            {
                return this.genderCodeField;
            }
            set
            {
                this.genderCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "MaritalStatusCode" )]
        public MaritalStatusCode[] MaritalStatusCode
        {
            get
            {
                return this.maritalStatusCodeField;
            }
            set
            {
                this.maritalStatusCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RaceOrEthnicityCode" )]
        public RaceOrEthnicityCode[] RaceOrEthnicityCode
        {
            get
            {
                return this.raceOrEthnicityCodeField;
            }
            set
            {
                this.raceOrEthnicityCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CitizenshipStatusCode" )]
        public CitizenshipStatusCode[] CitizenshipStatusCode
        {
            get
            {
                return this.citizenshipStatusCodeField;
            }
            set
            {
                this.citizenshipStatusCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CountryCode" )]
        public CountryCode[] CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "BasisOfVerificationCode" )]
        public BasisOfVerificationCode[] BasisOfVerificationCode
        {
            get
            {
                return this.basisOfVerificationCodeField;
            }
            set
            {
                this.basisOfVerificationCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Quantity" )]
        public Quantity[] Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DateTimePeriodFormatQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DateTimePeriod
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class GenderCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class MaritalStatusCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class RaceOrEthnicityCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CitizenshipStatusCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class BasisOfVerificationCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Quantity
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Relationship
    {

        private YesNoConditionOrResponseCode1 yesNoConditionOrResponseCode1Field;

        private IndividualRelationshipCode individualRelationshipCodeField;

        private MaintenanceTypeCode maintenanceTypeCodeField;

        private MaintenanceReasonCode maintenanceReasonCodeField;

        private BenefitStatusCode benefitStatusCodeField;

        private MedicarePlanCode medicarePlanCodeField;

        private ConsolidatedOmnibusBudgetReconciliationAct consolidatedOmnibusBudgetReconciliationActField;

        private EmploymentStatusCode employmentStatusCodeField;

        private StudentStatusCode studentStatusCodeField;

        private YesNoConditionOrResponseCode2 yesNoConditionOrResponseCode2Field;

        private DateTimePeriodFormatQualifier dateTimePeriodFormatQualifierField;

        private DateTimePeriod dateTimePeriodField;

        private ConfidentialityCode confidentialityCodeField;

        private CityName cityNameField;

        private StateOrProvinceCode stateOrProvinceCodeField;

        private CountryCode countryCodeField;

        private Number numberField;

        /// <remarks/>
        [XmlElement( "YesNoConditionOrResponseCode1" )]
        public YesNoConditionOrResponseCode1 YesNoConditionOrResponseCode1
        {
            get
            {
                return this.yesNoConditionOrResponseCode1Field;
            }
            set
            {
                this.yesNoConditionOrResponseCode1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "IndividualRelationshipCode" )]
        public IndividualRelationshipCode IndividualRelationshipCode
        {
            get
            {
                return this.individualRelationshipCodeField;
            }
            set
            {
                this.individualRelationshipCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "MaintenanceTypeCode" )]
        public MaintenanceTypeCode MaintenanceTypeCode
        {
            get
            {
                return this.maintenanceTypeCodeField;
            }
            set
            {
                this.maintenanceTypeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "MaintenanceReasonCode" )]
        public MaintenanceReasonCode MaintenanceReasonCode
        {
            get
            {
                return this.maintenanceReasonCodeField;
            }
            set
            {
                this.maintenanceReasonCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "BenefitStatusCode" )]
        public BenefitStatusCode BenefitStatusCode
        {
            get
            {
                return this.benefitStatusCodeField;
            }
            set
            {
                this.benefitStatusCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "MedicarePlanCode" )]
        public MedicarePlanCode MedicarePlanCode
        {
            get
            {
                return this.medicarePlanCodeField;
            }
            set
            {
                this.medicarePlanCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ConsolidatedOmnibusBudgetReconciliationAct" )]
        public ConsolidatedOmnibusBudgetReconciliationAct ConsolidatedOmnibusBudgetReconciliationAct
        {
            get
            {
                return this.consolidatedOmnibusBudgetReconciliationActField;
            }
            set
            {
                this.consolidatedOmnibusBudgetReconciliationActField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "EmploymentStatusCode" )]
        public EmploymentStatusCode EmploymentStatusCode
        {
            get
            {
                return this.employmentStatusCodeField;
            }
            set
            {
                this.employmentStatusCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "StudentStatusCode" )]
        public StudentStatusCode StudentStatusCode
        {
            get
            {
                return this.studentStatusCodeField;
            }
            set
            {
                this.studentStatusCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "YesNoConditionOrResponseCode2" )]
        public YesNoConditionOrResponseCode2 YesNoConditionOrResponseCode2
        {
            get
            {
                return this.yesNoConditionOrResponseCode2Field;
            }
            set
            {
                this.yesNoConditionOrResponseCode2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DateTimePeriodFormatQualifier" )]
        public DateTimePeriodFormatQualifier DateTimePeriodFormatQualifier
        {
            get
            {
                return this.dateTimePeriodFormatQualifierField;
            }
            set
            {
                this.dateTimePeriodFormatQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DateTimePeriod" )]
        public DateTimePeriod DateTimePeriod
        {
            get
            {
                return this.dateTimePeriodField;
            }
            set
            {
                this.dateTimePeriodField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ConfidentialityCode" )]
        public ConfidentialityCode ConfidentialityCode
        {
            get
            {
                return this.confidentialityCodeField;
            }
            set
            {
                this.confidentialityCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CityName" )]
        public CityName CityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "StateOrProvinceCode" )]
        public StateOrProvinceCode StateOrProvinceCode
        {
            get
            {
                return this.stateOrProvinceCodeField;
            }
            set
            {
                this.stateOrProvinceCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CountryCode" )]
        public CountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Number" )]
        public Number Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class YesNoConditionOrResponseCode1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class IndividualRelationshipCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class MaintenanceTypeCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class MaintenanceReasonCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class BenefitStatusCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class MedicarePlanCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ConsolidatedOmnibusBudgetReconciliationAct
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class EmploymentStatusCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class StudentStatusCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class YesNoConditionOrResponseCode2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ConfidentialityCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Number
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DatePeriod
    {

        private DateTimeQualifier dateTimeQualifierField;

        private DateTimePeriodFormatQualifier dateTimePeriodFormatQualifierField;

        private DateTimePeriod dateTimePeriodField;

        /// <remarks/>
        [XmlElement( "DateTimeQualifier" )]
        public DateTimeQualifier DateTimeQualifier
        {
            get
            {
                return this.dateTimeQualifierField;
            }
            set
            {
                this.dateTimeQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DateTimePeriodFormatQualifier" )]
        public DateTimePeriodFormatQualifier DateTimePeriodFormatQualifier
        {
            get
            {
                return this.dateTimePeriodFormatQualifierField;
            }
            set
            {
                this.dateTimePeriodFormatQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DateTimePeriod" )]
        public DateTimePeriod DateTimePeriod
        {
            get
            {
                return this.dateTimePeriodField;
            }
            set
            {
                this.dateTimePeriodField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DateTimeQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SubscriberBenefitInformation
    {

        private BenefitInformation benefitInformationField;

        private HealthCareServiceDelivery[] healthCareServiceDeliveryField;

        private AdditionalIdentification[] additionalIdentificationField;

        private DatePeriod[] datePeriodField;

        private RequestValidation[] requestValidationField;

        private MessageText messageTextField;

        private SubscriberBenefitAdditionalInformation[] subscriberBenefitAdditionalInformationField;

        private LoopHeader[] loopHeaderField;

        private SubscriberBenefitRelatedEntityName[] subscriberBenefitRelatedEntityNameField;

        private LoopTrailer[] loopTrailerField;

        /// <remarks/>
        [XmlElement( "BenefitInformation" )]
        public BenefitInformation BenefitInformation
        {
            get
            {
                return this.benefitInformationField;
            }
            set
            {
                this.benefitInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "HealthCareServiceDelivery" )]
        public HealthCareServiceDelivery[] HealthCareServiceDelivery
        {
            get
            {
                return this.healthCareServiceDeliveryField;
            }
            set
            {
                this.healthCareServiceDeliveryField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AdditionalIdentification" )]
        public AdditionalIdentification[] AdditionalIdentification
        {
            get
            {
                return this.additionalIdentificationField;
            }
            set
            {
                this.additionalIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DatePeriod" )]
        public DatePeriod[] DatePeriod
        {
            get
            {
                return this.datePeriodField;
            }
            set
            {
                this.datePeriodField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RequestValidation" )]
        public RequestValidation[] RequestValidation
        {
            get
            {
                return this.requestValidationField;
            }
            set
            {
                this.requestValidationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "MessageText" )]
        public MessageText MessageText
        {
            get
            {
                return this.messageTextField;
            }
            set
            {
                this.messageTextField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SubscriberBenefitAdditionalInformation" )]
        public SubscriberBenefitAdditionalInformation[] SubscriberBenefitAdditionalInformation
        {
            get
            {
                return this.subscriberBenefitAdditionalInformationField;
            }
            set
            {
                this.subscriberBenefitAdditionalInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "LoopHeader" )]
        public LoopHeader[] LoopHeader
        {
            get
            {
                return this.loopHeaderField;
            }
            set
            {
                this.loopHeaderField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SubscriberBenefitRelatedEntityName" )]
        public SubscriberBenefitRelatedEntityName[] SubscriberBenefitRelatedEntityName
        {
            get
            {
                return this.subscriberBenefitRelatedEntityNameField;
            }
            set
            {
                this.subscriberBenefitRelatedEntityNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "LoopTrailer" )]
        public LoopTrailer[] LoopTrailer
        {
            get
            {
                return this.loopTrailerField;
            }
            set
            {
                this.loopTrailerField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class BenefitInformation
    {

        private EligibilityOrBenefitInformation eligibilityOrBenefitInformationField;

        private CoverageLevelCode coverageLevelCodeField;

        private ServiceTypeCode serviceTypeCodeField;

        private InsuranceTypeCode insuranceTypeCodeField;

        private PlanCoverageDescription planCoverageDescriptionField;

        private TimePeriodQualifier timePeriodQualifierField;

        private MonetaryAmount monetaryAmountField;

        private Percent percentField;

        private QuantityQualifier quantityQualifierField;

        private Quantity quantityField;

        private YesNoConditionOrResponseCode1 yesNoConditionOrResponseCode1Field;

        private YesNoConditionOrResponseCode2 yesNoConditionOrResponseCode2Field;

        private CompositeMedicalProcedureIdentifier compositeMedicalProcedureIdentifierField;

        /// <remarks/>
        [XmlElement( "EligibilityOrBenefitInformation" )]
        public EligibilityOrBenefitInformation EligibilityOrBenefitInformation
        {
            get
            {
                return this.eligibilityOrBenefitInformationField;
            }
            set
            {
                this.eligibilityOrBenefitInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CoverageLevelCode" )]
        public CoverageLevelCode CoverageLevelCode
        {
            get
            {
                return this.coverageLevelCodeField;
            }
            set
            {
                this.coverageLevelCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ServiceTypeCode" )]
        public ServiceTypeCode ServiceTypeCode
        {
            get
            {
                return this.serviceTypeCodeField;
            }
            set
            {
                this.serviceTypeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "InsuranceTypeCode" )]
        public InsuranceTypeCode InsuranceTypeCode
        {
            get
            {
                return this.insuranceTypeCodeField;
            }
            set
            {
                this.insuranceTypeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "PlanCoverageDescription" )]
        public PlanCoverageDescription PlanCoverageDescription
        {
            get
            {
                return this.planCoverageDescriptionField;
            }
            set
            {
                this.planCoverageDescriptionField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TimePeriodQualifier" )]
        public TimePeriodQualifier TimePeriodQualifier
        {
            get
            {
                return this.timePeriodQualifierField;
            }
            set
            {
                this.timePeriodQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "MonetaryAmount" )]
        public MonetaryAmount MonetaryAmount
        {
            get
            {
                return this.monetaryAmountField;
            }
            set
            {
                this.monetaryAmountField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Percent" )]
        public Percent Percent
        {
            get
            {
                return this.percentField ?? PercentageAsDecimal;
            }
            set
            {
                this.percentField = value;
            }
        }

        [XmlElement("PercentageAsDecimal")]
        public Percent PercentageAsDecimal { get; set; }

        /// <remarks/>
        [XmlElement( "QuantityQualifier" )]
        public QuantityQualifier QuantityQualifier
        {
            get
            {
                return this.quantityQualifierField;
            }
            set
            {
                this.quantityQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Quantity" )]
        public Quantity Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "YesNoConditionOrResponseCode1" )]
        public YesNoConditionOrResponseCode1 YesNoConditionOrResponseCode1
        {
            get
            {
                return this.yesNoConditionOrResponseCode1Field;
            }
            set
            {
                this.yesNoConditionOrResponseCode1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "YesNoConditionOrResponseCode2" )]
        public YesNoConditionOrResponseCode2 YesNoConditionOrResponseCode2
        {
            get
            {
                return this.yesNoConditionOrResponseCode2Field;
            }
            set
            {
                this.yesNoConditionOrResponseCode2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CompositeMedicalProcedureIdentifier" )]
        public CompositeMedicalProcedureIdentifier CompositeMedicalProcedureIdentifier
        {
            get
            {
                return this.compositeMedicalProcedureIdentifierField;
            }
            set
            {
                this.compositeMedicalProcedureIdentifierField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class EligibilityOrBenefitInformation
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CoverageLevelCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ServiceTypeCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class InsuranceTypeCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class PlanCoverageDescription
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TimePeriodQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class MonetaryAmount
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Percent
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class QuantityQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CompositeMedicalProcedureIdentifier
    {

        private MedicalProcedureIdentifier[] medicalProcedureIdentifierField;

        /// <remarks/>
        [XmlElement( "MedicalProcedureIdentifier" )]
        public MedicalProcedureIdentifier[] MedicalProcedureIdentifier
        {
            get
            {
                return this.medicalProcedureIdentifierField;
            }
            set
            {
                this.medicalProcedureIdentifierField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class MedicalProcedureIdentifier
    {

        private ProductServiceIdQualifier[] productServiceIdQualifierField;

        private ProductServiceId[] productServiceIdField;

        private ProcedureModifier1[] procedureModifier1Field;

        private ProcedureModifier2[] procedureModifier2Field;

        private ProcedureModifier3[] procedureModifier3Field;

        private ProcedureModifier4[] procedureModifier4Field;

        private Description[] descriptionField;

        /// <remarks/>
        [XmlElement( "ProductServiceIdQualifier" )]
        public ProductServiceIdQualifier[] ProductServiceIdQualifier
        {
            get
            {
                return this.productServiceIdQualifierField;
            }
            set
            {
                this.productServiceIdQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ProductServiceId" )]
        public ProductServiceId[] ProductServiceId
        {
            get
            {
                return this.productServiceIdField;
            }
            set
            {
                this.productServiceIdField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ProcedureModifier1" )]
        public ProcedureModifier1[] ProcedureModifier1
        {
            get
            {
                return this.procedureModifier1Field;
            }
            set
            {
                this.procedureModifier1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ProcedureModifier2" )]
        public ProcedureModifier2[] ProcedureModifier2
        {
            get
            {
                return this.procedureModifier2Field;
            }
            set
            {
                this.procedureModifier2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ProcedureModifier3" )]
        public ProcedureModifier3[] ProcedureModifier3
        {
            get
            {
                return this.procedureModifier3Field;
            }
            set
            {
                this.procedureModifier3Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ProcedureModifier4" )]
        public ProcedureModifier4[] ProcedureModifier4
        {
            get
            {
                return this.procedureModifier4Field;
            }
            set
            {
                this.procedureModifier4Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Description" )]
        public Description[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProductServiceIdQualifier
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProductServiceId
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProcedureModifier1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProcedureModifier2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProcedureModifier3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProcedureModifier4
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class HealthCareServiceDelivery
    {

        private QuantityQualifier[] quantityQualifierField;

        private Quantity[] quantityField;

        private UnitOrBasisForMeasurementCode[] unitOrBasisForMeasurementCodeField;

        private SampleSelectionModulus[] sampleSelectionModulusField;

        private TimePeriodQualifier[] timePeriodQualifierField;

        private NumberOfPeriods[] numberOfPeriodsField;

        private ShipDeliveryOrCalendarPatternCode[] shipDeliveryOrCalendarPatternCodeField;

        private ShipDeliveryPatternTimeCode[] shipDeliveryPatternTimeCodeField;

        /// <remarks/>
        [XmlElement( "QuantityQualifier" )]
        public QuantityQualifier[] QuantityQualifier
        {
            get
            {
                return this.quantityQualifierField;
            }
            set
            {
                this.quantityQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Quantity" )]
        public Quantity[] Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "UnitOrBasisForMeasurementCode" )]
        public UnitOrBasisForMeasurementCode[] UnitOrBasisForMeasurementCode
        {
            get
            {
                return this.unitOrBasisForMeasurementCodeField;
            }
            set
            {
                this.unitOrBasisForMeasurementCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SampleSelectionModulus" )]
        public SampleSelectionModulus[] SampleSelectionModulus
        {
            get
            {
                return this.sampleSelectionModulusField;
            }
            set
            {
                this.sampleSelectionModulusField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TimePeriodQualifier" )]
        public TimePeriodQualifier[] TimePeriodQualifier
        {
            get
            {
                return this.timePeriodQualifierField;
            }
            set
            {
                this.timePeriodQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "NumberOfPeriods" )]
        public NumberOfPeriods[] NumberOfPeriods
        {
            get
            {
                return this.numberOfPeriodsField;
            }
            set
            {
                this.numberOfPeriodsField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ShipDeliveryOrCalendarPatternCode" )]
        public ShipDeliveryOrCalendarPatternCode[] ShipDeliveryOrCalendarPatternCode
        {
            get
            {
                return this.shipDeliveryOrCalendarPatternCodeField;
            }
            set
            {
                this.shipDeliveryOrCalendarPatternCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ShipDeliveryPatternTimeCode" )]
        public ShipDeliveryPatternTimeCode[] ShipDeliveryPatternTimeCode
        {
            get
            {
                return this.shipDeliveryPatternTimeCodeField;
            }
            set
            {
                this.shipDeliveryPatternTimeCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class UnitOrBasisForMeasurementCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SampleSelectionModulus
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class NumberOfPeriods
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ShipDeliveryOrCalendarPatternCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ShipDeliveryPatternTimeCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class MessageText
    {

        private FreeFormMessageText freeFormMessageTextField;

        private PrinterCarriageControlCode printerCarriageControlCodeField;

        private Number numberField;

        /// <remarks/>
        [XmlElement( "FreeFormMessageText" )]
        public FreeFormMessageText FreeFormMessageText
        {
            get
            {
                return this.freeFormMessageTextField;
            }
            set
            {
                this.freeFormMessageTextField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "PrinterCarriageControlCode" )]
        public PrinterCarriageControlCode PrinterCarriageControlCode
        {
            get
            {
                return this.printerCarriageControlCodeField;
            }
            set
            {
                this.printerCarriageControlCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Number" )]
        public Number Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class FreeFormMessageText
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class PrinterCarriageControlCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SubscriberBenefitAdditionalInformation
    {

        private BenefitAdditionalInformation[] benefitAdditionalInformationField;

        /// <remarks/>
        [XmlElement( "BenefitAdditionalInformation" )]
        public BenefitAdditionalInformation[] BenefitAdditionalInformation
        {
            get
            {
                return this.benefitAdditionalInformationField;
            }
            set
            {
                this.benefitAdditionalInformationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class BenefitAdditionalInformation
    {

        private CodeListQualifierCode[] codeListQualifierCodeField;

        private IndustryCode[] industryCodeField;

        private CodeCategory[] codeCategoryField;

        private FreeFormMessageText[] freeFormMessageTextField;

        private Quantity[] quantityField;

        private CompositeUnitOfMeasure[] compositeUnitOfMeasureField;

        private SurfaceLayerPositionCode1[] surfaceLayerPositionCode1Field;

        private SurfaceLayerPositionCode2[] surfaceLayerPositionCode2Field;

        private SurfaceLayerPositionCode3[] surfaceLayerPositionCode3Field;

        /// <remarks/>
        [XmlElement( "CodeListQualifierCode" )]
        public CodeListQualifierCode[] CodeListQualifierCode
        {
            get
            {
                return this.codeListQualifierCodeField;
            }
            set
            {
                this.codeListQualifierCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "IndustryCode" )]
        public IndustryCode[] IndustryCode
        {
            get
            {
                return this.industryCodeField;
            }
            set
            {
                this.industryCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CodeCategory" )]
        public CodeCategory[] CodeCategory
        {
            get
            {
                return this.codeCategoryField;
            }
            set
            {
                this.codeCategoryField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "FreeFormMessageText" )]
        public FreeFormMessageText[] FreeFormMessageText
        {
            get
            {
                return this.freeFormMessageTextField;
            }
            set
            {
                this.freeFormMessageTextField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Quantity" )]
        public Quantity[] Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CompositeUnitOfMeasure" )]
        public CompositeUnitOfMeasure[] CompositeUnitOfMeasure
        {
            get
            {
                return this.compositeUnitOfMeasureField;
            }
            set
            {
                this.compositeUnitOfMeasureField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SurfaceLayerPositionCode1" )]
        public SurfaceLayerPositionCode1[] SurfaceLayerPositionCode1
        {
            get
            {
                return this.surfaceLayerPositionCode1Field;
            }
            set
            {
                this.surfaceLayerPositionCode1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SurfaceLayerPositionCode2" )]
        public SurfaceLayerPositionCode2[] SurfaceLayerPositionCode2
        {
            get
            {
                return this.surfaceLayerPositionCode2Field;
            }
            set
            {
                this.surfaceLayerPositionCode2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "SurfaceLayerPositionCode3" )]
        public SurfaceLayerPositionCode3[] SurfaceLayerPositionCode3
        {
            get
            {
                return this.surfaceLayerPositionCode3Field;
            }
            set
            {
                this.surfaceLayerPositionCode3Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CodeListQualifierCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class IndustryCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CodeCategory
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class CompositeUnitOfMeasure
    {

        private UnitOfMeasure[] unitOfMeasureField;

        /// <remarks/>
        [XmlElement( "UnitOfMeasure" )]
        public UnitOfMeasure[] UnitOfMeasure
        {
            get
            {
                return this.unitOfMeasureField;
            }
            set
            {
                this.unitOfMeasureField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class UnitOfMeasure
    {

        private UnitOrBasisForMeasurementCode1[] unitOrBasisForMeasurementCode1Field;

        private Exponent1[] exponent1Field;

        private Multiplier1[] multiplier1Field;

        private UnitOrBasisForMeasurementCode2[] unitOrBasisForMeasurementCode2Field;

        private Exponent2[] exponent2Field;

        private Multiplier2[] multiplier2Field;

        private UnitOrBasisForMeasurementCode3[] unitOrBasisForMeasurementCode3Field;

        private Exponent3[] exponent3Field;

        private Multiplier3[] multiplier3Field;

        private UnitOrBasisForMeasurementCode4[] unitOrBasisForMeasurementCode4Field;

        private Exponent4[] exponent4Field;

        private Multiplier4[] multiplier4Field;

        private UnitOrBasisForMeasurementCode5[] unitOrBasisForMeasurementCode5Field;

        private Exponent5[] exponent5Field;

        private Multiplier5[] multiplier5Field;

        /// <remarks/>
        [XmlElement( "UnitOrBasisForMeasurementCode1" )]
        public UnitOrBasisForMeasurementCode1[] UnitOrBasisForMeasurementCode1
        {
            get
            {
                return this.unitOrBasisForMeasurementCode1Field;
            }
            set
            {
                this.unitOrBasisForMeasurementCode1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Exponent1" )]
        public Exponent1[] Exponent1
        {
            get
            {
                return this.exponent1Field;
            }
            set
            {
                this.exponent1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Multiplier1" )]
        public Multiplier1[] Multiplier1
        {
            get
            {
                return this.multiplier1Field;
            }
            set
            {
                this.multiplier1Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "UnitOrBasisForMeasurementCode2" )]
        public UnitOrBasisForMeasurementCode2[] UnitOrBasisForMeasurementCode2
        {
            get
            {
                return this.unitOrBasisForMeasurementCode2Field;
            }
            set
            {
                this.unitOrBasisForMeasurementCode2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Exponent2" )]
        public Exponent2[] Exponent2
        {
            get
            {
                return this.exponent2Field;
            }
            set
            {
                this.exponent2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Multiplier2" )]
        public Multiplier2[] Multiplier2
        {
            get
            {
                return this.multiplier2Field;
            }
            set
            {
                this.multiplier2Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "UnitOrBasisForMeasurementCode3" )]
        public UnitOrBasisForMeasurementCode3[] UnitOrBasisForMeasurementCode3
        {
            get
            {
                return this.unitOrBasisForMeasurementCode3Field;
            }
            set
            {
                this.unitOrBasisForMeasurementCode3Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Exponent3" )]
        public Exponent3[] Exponent3
        {
            get
            {
                return this.exponent3Field;
            }
            set
            {
                this.exponent3Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Multiplier3" )]
        public Multiplier3[] Multiplier3
        {
            get
            {
                return this.multiplier3Field;
            }
            set
            {
                this.multiplier3Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "UnitOrBasisForMeasurementCode4" )]
        public UnitOrBasisForMeasurementCode4[] UnitOrBasisForMeasurementCode4
        {
            get
            {
                return this.unitOrBasisForMeasurementCode4Field;
            }
            set
            {
                this.unitOrBasisForMeasurementCode4Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Exponent4" )]
        public Exponent4[] Exponent4
        {
            get
            {
                return this.exponent4Field;
            }
            set
            {
                this.exponent4Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Multiplier4" )]
        public Multiplier4[] Multiplier4
        {
            get
            {
                return this.multiplier4Field;
            }
            set
            {
                this.multiplier4Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "UnitOrBasisForMeasurementCode5" )]
        public UnitOrBasisForMeasurementCode5[] UnitOrBasisForMeasurementCode5
        {
            get
            {
                return this.unitOrBasisForMeasurementCode5Field;
            }
            set
            {
                this.unitOrBasisForMeasurementCode5Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Exponent5" )]
        public Exponent5[] Exponent5
        {
            get
            {
                return this.exponent5Field;
            }
            set
            {
                this.exponent5Field = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Multiplier5" )]
        public Multiplier5[] Multiplier5
        {
            get
            {
                return this.multiplier5Field;
            }
            set
            {
                this.multiplier5Field = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class UnitOrBasisForMeasurementCode1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Exponent1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Multiplier1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class UnitOrBasisForMeasurementCode2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Exponent2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Multiplier2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class UnitOrBasisForMeasurementCode3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Exponent3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Multiplier3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class UnitOrBasisForMeasurementCode4
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Exponent4
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Multiplier4
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class UnitOrBasisForMeasurementCode5
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Exponent5
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Multiplier5
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SurfaceLayerPositionCode1
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SurfaceLayerPositionCode2
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SurfaceLayerPositionCode3
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class LoopHeader
    {

        private LoopIdentifierCode[] loopIdentifierCodeField;

        /// <remarks/>
        [XmlElement( "LoopIdentifierCode" )]
        public LoopIdentifierCode[] LoopIdentifierCode
        {
            get
            {
                return this.loopIdentifierCodeField;
            }
            set
            {
                this.loopIdentifierCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class LoopIdentifierCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class SubscriberBenefitRelatedEntityName
    {

        private Name[] nameField;

        private Address[] addressField;

        private CityStateZip[] cityStateZipField;

        private ContactInformation[] contactInformationField;

        private Provider[] providerField;

        /// <remarks/>
        [XmlElement( "Name" )]
        public Name[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Address" )]
        public Address[] Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CityStateZip" )]
        public CityStateZip[] CityStateZip
        {
            get
            {
                return this.cityStateZipField;
            }
            set
            {
                this.cityStateZipField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ContactInformation" )]
        public ContactInformation[] ContactInformation
        {
            get
            {
                return this.contactInformationField;
            }
            set
            {
                this.contactInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Provider" )]
        public Provider[] Provider
        {
            get
            {
                return this.providerField;
            }
            set
            {
                this.providerField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class Provider
    {

        private ProviderCode[] providerCodeField;

        private ReferenceIdentificationQualifier[] referenceIdentificationQualifierField;

        private ReferenceIdentification[] referenceIdentificationField;

        private StateOrProvinceCode[] stateOrProvinceCodeField;

        private ProviderSpecialtyInformation[] providerSpecialtyInformationField;

        private ProviderOrganizationCode[] providerOrganizationCodeField;

        /// <remarks/>
        [XmlElement( "ProviderCode" )]
        public ProviderCode[] ProviderCode
        {
            get
            {
                return this.providerCodeField;
            }
            set
            {
                this.providerCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentificationQualifier" )]
        public ReferenceIdentificationQualifier[] ReferenceIdentificationQualifier
        {
            get
            {
                return this.referenceIdentificationQualifierField;
            }
            set
            {
                this.referenceIdentificationQualifierField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ReferenceIdentification" )]
        public ReferenceIdentification[] ReferenceIdentification
        {
            get
            {
                return this.referenceIdentificationField;
            }
            set
            {
                this.referenceIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "StateOrProvinceCode" )]
        public StateOrProvinceCode[] StateOrProvinceCode
        {
            get
            {
                return this.stateOrProvinceCodeField;
            }
            set
            {
                this.stateOrProvinceCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ProviderSpecialtyInformation" )]
        public ProviderSpecialtyInformation[] ProviderSpecialtyInformation
        {
            get
            {
                return this.providerSpecialtyInformationField;
            }
            set
            {
                this.providerSpecialtyInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ProviderOrganizationCode" )]
        public ProviderOrganizationCode[] ProviderOrganizationCode
        {
            get
            {
                return this.providerOrganizationCodeField;
            }
            set
            {
                this.providerOrganizationCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProviderCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProviderSpecialtyInformation
    {

        private ProviderSpecialty[] providerSpecialtyField;

        /// <remarks/>
        [XmlElement( "ProviderSpecialty" )]
        public ProviderSpecialty[] ProviderSpecialty
        {
            get
            {
                return this.providerSpecialtyField;
            }
            set
            {
                this.providerSpecialtyField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProviderSpecialty
    {

        private ProviderSpecialtyCode[] providerSpecialtyCodeField;

        private AgencyQualifierCode[] agencyQualifierCodeField;

        private YesNoConditionOrResponseCode[] yesNoConditionOrResponseCodeField;

        /// <remarks/>
        [XmlElement( "ProviderSpecialtyCode" )]
        public ProviderSpecialtyCode[] ProviderSpecialtyCode
        {
            get
            {
                return this.providerSpecialtyCodeField;
            }
            set
            {
                this.providerSpecialtyCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AgencyQualifierCode" )]
        public AgencyQualifierCode[] AgencyQualifierCode
        {
            get
            {
                return this.agencyQualifierCodeField;
            }
            set
            {
                this.agencyQualifierCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "YesNoConditionOrResponseCode" )]
        public YesNoConditionOrResponseCode[] YesNoConditionOrResponseCode
        {
            get
            {
                return this.yesNoConditionOrResponseCodeField;
            }
            set
            {
                this.yesNoConditionOrResponseCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProviderSpecialtyCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class ProviderOrganizationCode
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class LoopTrailer
    {

        private LoopIdentifierCode[] loopIdentifierCodeField;

        /// <remarks/>
        [XmlElement( "LoopIdentifierCode" )]
        public LoopIdentifierCode[] LoopIdentifierCode
        {
            get
            {
                return this.loopIdentifierCodeField;
            }
            set
            {
                this.loopIdentifierCodeField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DependentLevel
    {

        private HierarchicalLevel hierarchicalLevelField;

        private TraceNumber traceNumberField;

        private DependentName dependentNameField;

        /// <remarks/>
        [XmlElement( "HierarchicalLevel" )]
        public HierarchicalLevel HierarchicalLevel
        {
            get
            {
                return this.hierarchicalLevelField;
            }
            set
            {
                this.hierarchicalLevelField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TraceNumber" )]
        public TraceNumber TraceNumber
        {
            get
            {
                return this.traceNumberField;
            }
            set
            {
                this.traceNumberField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DependentName" )]
        public DependentName DependentName
        {
            get
            {
                return this.dependentNameField;
            }
            set
            {
                this.dependentNameField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DependentName
    {

        private Name nameField;

        private AdditionalIdentification additionalIdentificationField;

        private Address addressField;

        private CityStateZip cityStateZipField;

        private ContactInformation contactInformationField;

        private RequestValidation requestValidationField;

        private Demographic demographicField;

        private Relationship relationshipField;

        private DatePeriod datePeriodField;

        private DependentBenefitInformation[] dependentBenefitInformationField;

        /// <remarks/>
        [XmlElement( "Name" )]
        public Name Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AdditionalIdentification" )]
        public AdditionalIdentification AdditionalIdentification
        {
            get
            {
                return this.additionalIdentificationField;
            }
            set
            {
                this.additionalIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Address" )]
        public Address Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CityStateZip" )]
        public CityStateZip CityStateZip
        {
            get
            {
                return this.cityStateZipField;
            }
            set
            {
                this.cityStateZipField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ContactInformation" )]
        public ContactInformation ContactInformation
        {
            get
            {
                return this.contactInformationField;
            }
            set
            {
                this.contactInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RequestValidation" )]
        public RequestValidation RequestValidation
        {
            get
            {
                return this.requestValidationField;
            }
            set
            {
                this.requestValidationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Demographic" )]
        public Demographic Demographic
        {
            get
            {
                return this.demographicField;
            }
            set
            {
                this.demographicField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Relationship" )]
        public Relationship Relationship
        {
            get
            {
                return this.relationshipField;
            }
            set
            {
                this.relationshipField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DatePeriod" )]
        public DatePeriod DatePeriod
        {
            get
            {
                return this.datePeriodField;
            }
            set
            {
                this.datePeriodField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DependentBenefitInformation" )]
        public DependentBenefitInformation[] DependentBenefitInformation
        {
            get
            {
                return this.dependentBenefitInformationField;
            }
            set
            {
                this.dependentBenefitInformationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DependentBenefitInformation
    {

        private BenefitInformation benefitInformationField;

        private HealthCareServiceDelivery[] healthCareServiceDeliveryField;

        private AdditionalIdentification[] additionalIdentificationField;

        private DatePeriod datePeriodField;

        private RequestValidation[] requestValidationField;

        private MessageText[] messageTextField;

        private DependentBenefitAdditionalInformation[] dependentBenefitAdditionalInformationField;

        private LoopHeader[] loopHeaderField;

        private DependentBenefitRelatedEntityName[] dependentBenefitRelatedEntityNameField;

        private LoopTrailer[] loopTrailerField;

        /// <remarks/>
        [XmlElement( "BenefitInformation" )]
        public BenefitInformation BenefitInformation
        {
            get
            {
                return this.benefitInformationField;
            }
            set
            {
                this.benefitInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "HealthCareServiceDelivery" )]
        public HealthCareServiceDelivery[] HealthCareServiceDelivery
        {
            get
            {
                return this.healthCareServiceDeliveryField;
            }
            set
            {
                this.healthCareServiceDeliveryField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "AdditionalIdentification" )]
        public AdditionalIdentification[] AdditionalIdentification
        {
            get
            {
                return this.additionalIdentificationField;
            }
            set
            {
                this.additionalIdentificationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DatePeriod" )]
        public DatePeriod DatePeriod
        {
            get
            {
                return this.datePeriodField;
            }
            set
            {
                this.datePeriodField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "RequestValidation" )]
        public RequestValidation[] RequestValidation
        {
            get
            {
                return this.requestValidationField;
            }
            set
            {
                this.requestValidationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "MessageText" )]
        public MessageText[] MessageText
        {
            get
            {
                return this.messageTextField;
            }
            set
            {
                this.messageTextField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DependentBenefitAdditionalInformation" )]
        public DependentBenefitAdditionalInformation[] DependentBenefitAdditionalInformation
        {
            get
            {
                return this.dependentBenefitAdditionalInformationField;
            }
            set
            {
                this.dependentBenefitAdditionalInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "LoopHeader" )]
        public LoopHeader[] LoopHeader
        {
            get
            {
                return this.loopHeaderField;
            }
            set
            {
                this.loopHeaderField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "DependentBenefitRelatedEntityName" )]
        public DependentBenefitRelatedEntityName[] DependentBenefitRelatedEntityName
        {
            get
            {
                return this.dependentBenefitRelatedEntityNameField;
            }
            set
            {
                this.dependentBenefitRelatedEntityNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "LoopTrailer" )]
        public LoopTrailer[] LoopTrailer
        {
            get
            {
                return this.loopTrailerField;
            }
            set
            {
                this.loopTrailerField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DependentBenefitAdditionalInformation
    {

        private BenefitAdditionalInformation[] benefitAdditionalInformationField;

        /// <remarks/>
        [XmlElement( "BenefitAdditionalInformation" )]
        public BenefitAdditionalInformation[] BenefitAdditionalInformation
        {
            get
            {
                return this.benefitAdditionalInformationField;
            }
            set
            {
                this.benefitAdditionalInformationField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class DependentBenefitRelatedEntityName
    {

        private Name[] nameField;

        private Address[] addressField;

        private CityStateZip[] cityStateZipField;

        private ContactInformation[] contactInformationField;

        private Provider[] providerField;

        /// <remarks/>
        [XmlElement( "Name" )]
        public Name[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Address" )]
        public Address[] Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "CityStateZip" )]
        public CityStateZip[] CityStateZip
        {
            get
            {
                return this.cityStateZipField;
            }
            set
            {
                this.cityStateZipField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "ContactInformation" )]
        public ContactInformation[] ContactInformation
        {
            get
            {
                return this.contactInformationField;
            }
            set
            {
                this.contactInformationField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "Provider" )]
        public Provider[] Provider
        {
            get
            {
                return this.providerField;
            }
            set
            {
                this.providerField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class TranSetTrailer
    {

        private NumberOfIncludedSegments[] numberOfIncludedSegmentsField;

        private TransactionSetControlNumber[] transactionSetControlNumberField;

        /// <remarks/>
        [XmlElement( "NumberOfIncludedSegments" )]
        public NumberOfIncludedSegments[] NumberOfIncludedSegments
        {
            get
            {
                return this.numberOfIncludedSegmentsField;
            }
            set
            {
                this.numberOfIncludedSegmentsField = value;
            }
        }

        /// <remarks/>
        [XmlElement( "TransactionSetControlNumber" )]
        public TransactionSetControlNumber[] TransactionSetControlNumber
        {
            get
            {
                return this.transactionSetControlNumberField;
            }
            set
            {
                this.transactionSetControlNumberField = value;
            }
        }
    }

    /// <remarks/>
    [GeneratedCode( "xsd", "2.0.50727.42" )]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory( "code" )]
    [XmlType()]
    [XmlRoot( IsNullable = false )]
    public partial class NumberOfIncludedSegments
    {

        private string textField;

        /// <remarks/>
        [XmlText()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }
}

    

