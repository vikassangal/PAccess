using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls.Email.Views
{
    public interface IEmailReasonView
    {
        void ClearEmailReasonSelectionValues();
        void PopulateEmailReasonSelections(IEnumerable<EmailReason> emailReasonValues);
        EmailReason EmailReason { get; }
        void SetEmailReasonNormal();
        void SetEmailReasonToRequired();
        void SetEmailReasonToPreferred();
        void SetEmailAddressAsRequired();
        void SetEmailAddressAsPreferred();
        void SetEmailAddressAsNormal();
    }
}