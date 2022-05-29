using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for TransferERToOutpatientActivity
	[Serializable]
	public class TransferERToOutpatientActivity : Activity
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
                    throw new ArgumentNullException( "Transfer ER to Outpatient Remarks cannot be null." ) ;
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
		public TransferERToOutpatientActivity()
		{
            this.Description = "Trans&fer ER Patient to Bedded Outpatient";
            this.ContextDescription = "Transfer ER Patient to Bedded Outpatient";
		}
		#endregion

		#region Data Elements
        private string i_Remarks = string.Empty;
		#endregion

		#region Constants
		#endregion
	}
}
