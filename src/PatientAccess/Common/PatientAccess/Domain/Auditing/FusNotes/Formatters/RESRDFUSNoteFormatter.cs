using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for RESRDFUSNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class RESRDFUSNoteFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            var messages = new ArrayList();
            var note = this.Context as FusNote;
            var account = (Account)note.Context;
            messages = this.CreateFusNameValueList( account );
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( Account account )
        {
            var nameValueList = new ArrayList();
            if (account != null && account.MedicareSecondaryPayor != null)
            {
                var msp = account.MedicareSecondaryPayor;
                if (msp.MedicareEntitlement.GetType() == typeof(ESRDEntitlement))
                {
                    var esrdEntitlement = (ESRDEntitlement)msp.MedicareEntitlement;
                    if (esrdEntitlement.DialysisTreatment != null &&
                        esrdEntitlement.DialysisTreatment.Code == YesNoFlag.CODE_YES)
                    {
                      nameValueList.Add(FusLabel.DIALYSISCENTERPATIENT);
                    }
                }
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