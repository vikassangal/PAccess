using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
	/// <summary>
	/// Summary description for InsuranceDateVerify.
	/// </summary>
	public class InsuranceDateVerify
	{
        public static bool VerifyInsuranceDate( ref MaskedEditTextBox mtb,
                                                ref int insuranceYear,
                                                ref int insuranceMonth,
                                                ref int insuranceDay)
        {
            bool result = true;
			if( mtb.Text.Length == 0 || mtb.Text == String.Empty )
			{
				return true;
			}

            if( mtb.Text.Length != 10 )
            {
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
				mtb.Focus();
                UIColors.SetErrorBgColor( mtb );

                return false;
            }

            UIColors.SetNormalBgColor( mtb );

            insuranceMonth = Convert.ToInt32( mtb.Text.Substring( 0, 2 ) );
            insuranceDay   = Convert.ToInt32( mtb.Text.Substring( 3, 2 ) );
            insuranceYear  = Convert.ToInt32( mtb.Text.Substring( 6, 4 ) );

            try
            {   // Check the date entered is not in the future
                DateTime theDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );

				if( DateValidator.IsValidDate( theDate ) == false )
				{
					UIColors.SetErrorBgColor( mtb );
					MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					mtb.Focus();
					//UIColors.SetNormalBgColor( mtb );
					result = false;
				}
				
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
				mtb.Focus();
				//UIColors.SetNormalBgColor( mtb );
                result = false;
            }
            return result;
        }

        public static bool IsValidDateTime( MaskedEditTextBox mtb )
        {
            try
            {
                Convert.ToDateTime( mtb.Text );   
                return true;
            }
            catch
            {
                return false;
            }
        }
	}
}
