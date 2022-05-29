using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for PrintFaceSheetActivity.
    /// </summary>
    //TODO: Create XML summary comment for PrintFaceSheetActivity
    [Serializable]
    public class PrintFaceSheetActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return true;
        }  
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PrintFaceSheetActivity()
        {
            this.Description    = "Print &Face Sheet";
            this.ContextDescription  = "Print Face Sheet";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

