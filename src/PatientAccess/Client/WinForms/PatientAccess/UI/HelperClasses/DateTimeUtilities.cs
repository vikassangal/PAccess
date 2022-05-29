using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.HelperClasses
{
    /// <summary>
    /// Static class for date validation operations
    /// </summary>
    public class DateTimeUtilities
    {
        #region Public Methods

        private static Hashtable MonthsHashtable()
        {
            monthsTable = new Hashtable();

            monthsTable.Add( JAN_CODE,  new Month( JAN_CODE, JAN_DESCRIPTION, 1 ) );
            monthsTable.Add( FEB_CODE,  new Month( FEB_CODE, FEB_DESCRIPTION, 2 ) );
            monthsTable.Add( MAR_CODE,  new Month( MAR_CODE, MAR_DESCRIPTION, 3 ) );
            monthsTable.Add( APR_CODE,  new Month( APR_CODE, APR_DESCRIPTION, 4 ) );
            monthsTable.Add( MAY_CODE,  new Month( MAY_CODE, MAY_DESCRIPTION, 5 ) );
            monthsTable.Add( JUN_CODE,  new Month( JUN_CODE, JUN_DESCRIPTION, 6 ) );
            monthsTable.Add( JUL_CODE,  new Month( JUL_CODE, JUL_DESCRIPTION, 7 ) );
            monthsTable.Add( AUG_CODE,  new Month( AUG_CODE, AUG_DESCRIPTION, 8 ) );
            monthsTable.Add( SEP_CODE,  new Month( SEP_CODE, SEP_DESCRIPTION, 9 ) );
            monthsTable.Add( OCT_CODE,  new Month( OCT_CODE, OCT_DESCRIPTION, 10 ) );
            monthsTable.Add( NOV_CODE,  new Month( NOV_CODE, NOV_DESCRIPTION, 11 ) );
            monthsTable.Add( DEC_CODE,  new Month( DEC_CODE, DEC_DESCRIPTION, 12 ) );

            return monthsTable;
        }

        public static bool ValidateDateOfBirth(MaskedEditTextBox mtbDateOfBirth)
        {

            DateTime dobDate;
            if (mtbDateOfBirth.UnMaskedText.Length == 0)
            {
                dobDate = DateTime.MinValue;
                return true;
            }

            if (mtbDateOfBirth.Text.Length != 10)
            {
                mtbDateOfBirth.Focus();
                UIColors.SetErrorBgColor(mtbDateOfBirth);
                MessageBox.Show(UIErrorMessages.DOB_INCOMPLETE_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1);
                return false;
            }

            try
            {
                string month = mtbDateOfBirth.Text.Substring(0, 2);
                string day = mtbDateOfBirth.Text.Substring(3, 2);
                string year = mtbDateOfBirth.Text.Substring(6, 4);

                dobDate = new DateTime(Convert.ToInt32(year),
                                        Convert.ToInt32(month),
                                        Convert.ToInt32(day));

                if (dobDate < earliestDateOfBirth)
                {
                    mtbDateOfBirth.Focus();
                    UIColors.SetErrorBgColor(mtbDateOfBirth);
                    MessageBox.Show(UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return false;
                }

                if (dobDate > DateTime.Today)
                {
                    // Remove the Admit Time Leave handler so error isn't generated
                    // when user comes back to the time field to correct the error.
                    mtbDateOfBirth.Focus();
                    UIColors.SetErrorBgColor(mtbDateOfBirth);
                    MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return false;
                }

                if (DateValidator.IsValidDate(dobDate) == false)
                {
                    mtbDateOfBirth.Focus();
                    UIColors.SetErrorBgColor(mtbDateOfBirth);
                    MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                mtbDateOfBirth.Focus();
                UIColors.SetErrorBgColor(mtbDateOfBirth);
                MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }
        public static ArrayList MonthsList()
        {
            monthsList = new ArrayList( MonthsHashtable().Values );
            monthsList.Sort();

            return monthsList;
        }

        #endregion

        #region Construction and Finalization
        static DateTimeUtilities()
        {
        }
        #endregion

        #region Data Elements
        private static Hashtable monthsTable;
        private static ArrayList monthsList;
        public static readonly DateTime earliestDateOfBirth = new DateTime(1800, 01, 01);
        #endregion

        #region Constants
        private const string
            JAN_CODE        = "Jan",
            FEB_CODE        = "Feb",
            MAR_CODE        = "Mar",
            APR_CODE        = "Apr",
            MAY_CODE        = "May",
            JUN_CODE        = "Jun",
            JUL_CODE        = "Jul",
            AUG_CODE        = "Aug",
            SEP_CODE        = "Sep",
            OCT_CODE        = "Oct",
            NOV_CODE        = "Nov",
            DEC_CODE        = "Dec";

        private const string
            JAN_DESCRIPTION        = "January",
            FEB_DESCRIPTION        = "February",
            MAR_DESCRIPTION        = "March",
            APR_DESCRIPTION        = "April",
            MAY_DESCRIPTION        = "May",
            JUN_DESCRIPTION        = "June",
            JUL_DESCRIPTION        = "July",
            AUG_DESCRIPTION        = "August",
            SEP_DESCRIPTION        = "September",
            OCT_DESCRIPTION        = "October",
            NOV_DESCRIPTION        = "November",
            DEC_DESCRIPTION        = "December";
        #endregion
    }
}
