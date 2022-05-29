using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for TransferOutpatientToERActivity
	[Serializable]
	public class TransferOutpatientToERActivity : Activity
	{
		#region Event Handlers
		#endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return false;
        }  
		#endregion

		#region Properties
        public string Remarks
        {
            get
            {
                return i_Remarks;
            }
            set
            {
                if( value == null )
                {
                    throw new ArgumentNullException("Transfer Outpatient to ER Remarks cannot be null.");
                }
                else
                {
                    i_Remarks = value;
                }
            }
        }
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public TransferOutpatientToERActivity()
		{
            this.Description = "Transfer &Bedded Outpatient to ER Patient";
            this.ContextDescription = "Transfer Bedded Outpatient to ER Patient";
		}
		#endregion

		#region Data Elements
        private string i_Remarks = string.Empty;
		#endregion

		#region Constants
		#endregion
	}
}
