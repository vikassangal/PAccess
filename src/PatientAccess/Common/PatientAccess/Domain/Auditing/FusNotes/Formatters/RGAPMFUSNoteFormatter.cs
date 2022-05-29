using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    [UsedImplicitly]
    public class RGAPMFUSNoteFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            var note = Context;
            var account = (Account)note.Context;
            var messages = CreateFusNameValueList(account);

            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList(Account account)
        {
            var nameValueList = new ArrayList();
            var contactPoint = account.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());

            if (account.Guarantor != null &&
                contactPoint != null &&
                contactPoint.CellPhoneConsent.Code != null)
            {
                var formattedString = String.Format(FusLabel.CHANGE_TO_GUARANTOR_CONSENT_FLAG, account.OldGuarantorCellPhoneConsent.Code, contactPoint.CellPhoneConsent.Code);
                nameValueList.Add(formattedString);
            }

            return nameValueList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}