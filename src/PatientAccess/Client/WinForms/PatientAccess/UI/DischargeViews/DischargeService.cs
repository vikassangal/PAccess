using System;
using System.Windows.Forms;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DischargeViews
{
	/// <summary>
	/// Summary description for DischargeService.
	/// </summary>
	public class DischargeService
	{
        public static bool IsDischargeDateBeforeChargeDates( TextBox mtbDischargeDate, TextBox mtbDischargeTime, DateTime lastChargeDateTime )
        {
            int verifyMonth  = Convert.ToInt32( mtbDischargeDate.Text.Substring( 0, 2 ) );
            int verifyDay    = Convert.ToInt32( mtbDischargeDate.Text.Substring( 3, 2 ) );
            int verifyYear   = Convert.ToInt32( mtbDischargeDate.Text.Substring( 6, 4 ) );
//            int verifyHour   = Convert.ToInt32( mtbDischargeDate.Text.Substring( 0, 2 ) );
//            int verifyMinute = Convert.ToInt32( mtbDischargeDate.Text.Substring( 3, 2 ) );

            try
            {   
                //DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay, verifyHour, verifyMinute, 0 );
				DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay, HOURS, MINUTES, 0 );

                if( theDate < lastChargeDateTime)
                {
                    SetErrBgColor( mtbDischargeDate );
                    SetErrBgColor( mtbDischargeTime );
                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return true;
                }
            }
            catch( ArgumentOutOfRangeException )
            {
                SetErrBgColor( mtbDischargeDate );
                MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return true;
            }

            SetNormalBgColor( mtbDischargeDate );
            SetNormalBgColor( mtbDischargeTime );
            return false;
        }

	    private static void SetErrBgColor( TextBox txtBox )
        {
            UIColors.SetErrorBgColor( txtBox );
        }

	    private static void SetNormalBgColor( TextBox txtBox )
        {
            UIColors.SetNormalBgColor( txtBox );
            txtBox.Refresh();
        }
    
		#region Constants
		private const int HOURS = 23;
		private const int MINUTES = 59;
		#endregion
	}
}
