using System;
using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.UI.Reports.FaceSheet
{
    public interface IDataBuilder
    {
        void SetFacility(IDictionary data);
        void SetDemoGraphics(Hashtable data);
        void SetDiagnosis(IDictionary data);
        void SetClinical(IDictionary data);
        void SetInsurance(Hashtable data);
        void SetGuarantor(IDictionary data);
        void SetBilling(IDictionary data);
        void SetContacts(IDictionary data);
        void SetRegulatory(IDictionary data);
        void SetShortRegistrationContacts(IDictionary data);
        bool IsAccountProxy();
        Func<SocialSecurityNumber, string> SsnFormatter { get; set; }
    }
}