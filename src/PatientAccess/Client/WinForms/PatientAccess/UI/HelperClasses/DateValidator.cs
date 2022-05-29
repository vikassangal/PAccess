using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.HelperClasses
{
    /// <summary>
    /// Static class for date validation operations
    /// </summary>
    public class DateValidator
    {
        #region Public Methods
        /* Every year divisible by 4 is a leap year. 
           But every year divisible by 100 is NOT a leap year 
           Unless the year is also divisible by 400, then it is still a leap year */

        private static bool IsLeapYear( int year )
        {
            if( (year % 4 == 0 && year % 100 != 0) || year % 400 == 0 )
                return true;
            else
                return false;
        }

        /// <summary>
        /// True if date is a future date, otherwise false
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsFutureDate( DateTime date )
        {
            if( date.Date > DateTime.Today.Date )
            {
                return true;
                }
            else
            {
                return false;
            }
        }

        public static bool IsValidDate( string inDate )
        {
            bool blnIsValid = true;
            DateTime aDate;

            try
            {
                aDate = DateTime.Parse( inDate );
                blnIsValid = IsValidDate( aDate );
            }
            catch
            {
                blnIsValid = false;
            }

            return blnIsValid;
        }
        /// <summary>
        /// Determines if a given date is a valid calendar date
        /// </summary>
        public static bool IsValidDate( DateTime date )
        {
            bool result = true;

            if( date.Month == FEBRUARY )
            {   // Check for leap year condition on February
                if( IsLeapYear( date.Year ) )
                {
                    if( date.Day > 29 )
                        result = false;
                }
                else if( date.Day > 28 )
                {
                    result = false;
                }
            }
            else
            {
                int numberOfdays = (int)calendarTable[ date.Month ];
                if( date.Day > numberOfdays )
                {
                    result = false;
                }
            }
            return result;
        }


		/// <summary>
		/// Returns the last day of the current month.
		/// </summary>
		public static DateTime EndOfMonth( DateTime date )
		{
			//DateTime endOfMonth;
			int numberOfDays = 0;
			//int month = date.Month;
			//int year = date.Year;

			numberOfDays += DateTime.DaysInMonth( date.Year, date.Month );
			return Convert.ToDateTime( date.Month + "/" + numberOfDays + "/" + date.Year );

		}
        internal static TimeValidationResult IsValidTime(string mtbTime)
        {
            TimeValidationResult result = TimeValidationResult.Valid ;

            if (mtbTime.Length >= 1 &&
                mtbTime.Length <= 3)
            {
                return TimeValidationResult.TimeIsInvalid ;
            }
            else if (mtbTime.Length < 1)
            {
                return TimeValidationResult.Valid ;
            }
            try
            {
                int hour = Convert.ToInt32(mtbTime.Substring(0, 2));
                int minute = Convert.ToInt32(mtbTime.Substring(2, 2));

                if (hour > 23)
                {
                  result = TimeValidationResult.HourIsInvalid ;
                }
                else if (minute > 59)
                {
                  result = TimeValidationResult.MinuteIsInvalid;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                 result = TimeValidationResult.TimeIsInvalid;
            }
            return result;
        }

        public static bool IsValidTime( MaskedEditTextBox mtbTime )
        {
            bool result = true;

            if( mtbTime.UnMaskedText.Length >= 1 && 
                mtbTime.UnMaskedText.Length <= 3 )
            {
                    UIColors.SetErrorBgColor( mtbTime );
                    MessageBox.Show( UIErrorMessages.TIME_NOT_VALID_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );               

                // Defect 4017 fix:  Calling Focus on control causes multiple error message boxes
                // to popup as the focus leaves the control when the user closes the MessageBox.
                // All UI manipulation should be done in the Form, not here.
                //mtbTime.Focus();
				 return false;
            }
            else if( mtbTime.UnMaskedText.Length < 1 )
            {
                return true;
            }

            try
            {
                int hour   = Convert.ToInt32( mtbTime.Text.Substring( 0, 2 ) );
                int minute = Convert.ToInt32( mtbTime.Text.Substring( 3, 2 ) );

                if( hour > 23 )
                {
                    UIColors.SetErrorBgColor( mtbTime );
                    MessageBox.Show( UIErrorMessages.HOUR_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                     //mtbTime.Focus();
                    result = false;
                }
                else if ( minute > 59 )
                {
                    UIColors.SetErrorBgColor( mtbTime );
                    MessageBox.Show( UIErrorMessages.MINUTE_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    result = false;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                UIColors.SetErrorBgColor( mtbTime );
                MessageBox.Show( UIErrorMessages.TIME_NOT_VALID_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                result = false;
            }
			return result;
        }
        /// <summary>
        /// Added new overloaded method to receive the error message from the caller.
        /// </summary>
        public static bool IsValidTime(MaskedEditTextBox mtbTime, string errorMessage, string messageTitle, IMessageBoxAdapter messageBox)
        {
            bool result = true;

            if ( mtbTime.UnMaskedText.Length >= 1 &&
                mtbTime.UnMaskedText.Length <= 3 )
            {
                UIColors.SetErrorBgColor( mtbTime );
                messageBox.Show( errorMessage, messageTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
               
                return false;
            }
            else if ( mtbTime.UnMaskedText.Length < 1 )
            {
                return true;
            }

            try
            {
                int hour = Convert.ToInt32( mtbTime.Text.Substring( 0, 2 ) );
                int minute = Convert.ToInt32( mtbTime.Text.Substring( 3, 2 ) );

                if ( hour > 23 )
                {
                    UIColors.SetErrorBgColor( mtbTime );
                    messageBox.Show( errorMessage , "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                     
                    result = false;
                }
                else if ( minute > 59 )
                {
                    UIColors.SetErrorBgColor( mtbTime );
                    messageBox.Show( errorMessage , "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                   
                    result = false;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                UIColors.SetErrorBgColor( mtbTime );
                messageBox.Show( errorMessage , "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                
                result = false;
            }
            return result;
        }     

        /// <summary>
        /// AuditTimeEntry - this method should be invoked from the TextChangedEvent on
        /// any PA time entry field.
        /// </summary>
        /// <param name="aTimeEntryTextBox"></param>
        public static void AuditTimeEntry( MaskedEditTextBox aTimeEntryTextBox )
        {
            aTimeEntryTextBox.UnMaskedText = aTimeEntryTextBox.UnMaskedText.Trim();
            aTimeEntryTextBox.MaxLength = 5;

            try
            {
                if( aTimeEntryTextBox.UnMaskedText.Length == 1 )
                {
                    if( Convert.ToInt32( aTimeEntryTextBox.UnMaskedText ) > 2 )
                    {                    
                        aTimeEntryTextBox.MaxLength = 1;
                        aTimeEntryTextBox.UnMaskedText = string.Empty;
                        return;
                    }
                }
                else if( aTimeEntryTextBox.UnMaskedText.Length == 2 )
                {
                    if( Convert.ToInt32( aTimeEntryTextBox.UnMaskedText.Substring(0,1) ) > 2 )
                    {
                        aTimeEntryTextBox.MaxLength = 2;
                        aTimeEntryTextBox.UnMaskedText = aTimeEntryTextBox.UnMaskedText.Substring(0,1);
                        return;
                    }
                    else
                    {
                        if( Convert.ToInt32( aTimeEntryTextBox.UnMaskedText.Substring(1,1) ) > 3 )
                        {
                            aTimeEntryTextBox.MaxLength = 2;
                            aTimeEntryTextBox.UnMaskedText = aTimeEntryTextBox.UnMaskedText.Substring(0,1);
                            return;
                        }
                    }
                }
                else if( aTimeEntryTextBox.UnMaskedText.Length == 3 )
                {
                    if( Convert.ToInt32( aTimeEntryTextBox.UnMaskedText.Substring(0,1) ) == 2
                        && Convert.ToInt32( aTimeEntryTextBox.UnMaskedText.Substring(1,1) ) > 3 )
                    {
                        aTimeEntryTextBox.MaxLength = 3;
                        aTimeEntryTextBox.UnMaskedText = aTimeEntryTextBox.UnMaskedText.Substring(0,2);
                        return;
                    }
                    else
                    {
                        if( Convert.ToInt32( aTimeEntryTextBox.UnMaskedText.Substring(2,1) ) > 5 )
                        {
                            aTimeEntryTextBox.MaxLength = 3;
                            aTimeEntryTextBox.UnMaskedText = aTimeEntryTextBox.UnMaskedText.Substring(0,2);
                            return;
                        }
                    }
                }
                else if( aTimeEntryTextBox.UnMaskedText.Length == 4 )
                {
                    if( Convert.ToInt32( aTimeEntryTextBox.UnMaskedText.Substring(2,1) ) > 5 )
                    {
                        aTimeEntryTextBox.UnMaskedText = aTimeEntryTextBox.UnMaskedText.Substring(0,3);
                        return;
                    }
                }
            }
            catch
            {
                aTimeEntryTextBox.UnMaskedText = string.Empty;
            }
        }

        #endregion

        #region Construction and Finalization
        static DateValidator()
        {
            calendarTable = new Hashtable( 12 );
            calendarTable.Add( 1,  31 );
            calendarTable.Add( 3,  31 );
            calendarTable.Add( 4,  30 );
            calendarTable.Add( 5,  31 );
            calendarTable.Add( 6,  30 );
            calendarTable.Add( 7,  31 );
            calendarTable.Add( 8,  31 );
            calendarTable.Add( 9,  30 );
            calendarTable.Add( 10, 31 );
            calendarTable.Add( 11, 30 );
            calendarTable.Add( 12, 31 );
        }
        #endregion

        #region Data Elements
        private static Hashtable    calendarTable;
        private const int           FEBRUARY = 2;

        public const string DATEValidationExpression = 
            @"^(((0[13578]|10|12)(0[1-9]|[12][0-9]|3[01])(\d{4}))" +    // months with 31 days
            @"|((0[469]|11)([0][1-9]|[12][0-9]|30)(\d{4}))" +     // months with 30 days
            @"|((02)(0[1-9]|1[0-9]|2[0-8])(\d{4}))" +    // Feb 1-28
            @"|((02)(29)([02468][048]00))" +    // Feb 29 with year divisible by 400
            @"|((02)(29)([13579][26]00))" +       //  Feb 29 with year divisibe by 400
            @"|((02)(29)([0-9][0-9][0][48]))" +       // Feb 29 with year divisible by 4 but not 100
            @"|((02)(29)([0-9][0-9][2468][048]))" +     // Feb 29 with year divisible by 4 but not 100
            @"|((02)(29)([0-9][0-9][13579][26])))$";    // Feb 29 with year divisible by 4 but not 100

        public const string DATEKeyPressExpression = "^\\d*$";
            //@"^([0-1]?" +       // 1st digit
            //@"|0[1-9]?" +      // 2nd digit
            //@"|1[0-2]?" +
            //// months other than Feb
            //@"|(0[13456789]|1[0-2])[0-3]?" +     // 3rd digit
            //@"|(0[13456789]|1[0-2])[0][1-9]?" +     // 4th digit
            //@"|(0[13456789]|1[0-2])[12][0-9]?" +
            //@"|(0[13578]|10|12])[3][01]?" +
            //@"|(0[469]|11)3[0]?" +
            //@"|(0[13578]|10|12)(0[1-9]|[12][0-9]|3[01])(\d{0,4})" +    // last 4 digits
            //@"|(0[469]|11)([0][1-9]|[12][0-9]|30)(\d{0,4})" +
            //// Feb 1-28
            //@"|02[0-2]?" +     // 3rd digit
            //@"|020[1-9]?" +     // 4th digit
            //@"|021[0-9]?" +     
            //@"|022[0-8]?" +
            //@"|02(0[1-9]|1[0-9]|2[0-8])(\d{0,4})" +      // last 4 digits    
            //// Feb 29
            //@"|0229[0-9]?" + 
            //@"|0229[0-9][0-9]?" + 
            //@"|0229[0-9][0-9][0-9]?" + 
            //@"|0229[02468][048]0[0]?" +      // last digit    
            //@"|0229[13579][26]0[0]?" +      
            //@"|0229[0-9][0-9][0][48]?" +       
            //@"|0229[0-9][0-9][2468][048]?" +    
            //@"|0229[0-9][0-9][13579][26]?)$";    

        public const string TIMEValidationExpression =
            "^([0-1][0-9]|2[0-3])([0-5][0-9])$";

        public const string TIMEKeyPressExpression = "^\\d*$";
            //"^([0-2]?" + 
            //"|[0-1][0-9]?" + 
            //"|[0-1][0-9][0-5]?" + 
            //"|[0-1][0-9][0-5][0-9]?" + 
            //"|2[0-3]?" + 
            //"|2[0-3][0-5]?" + 
            //"|2[0-3][0-5][0-9]?)$";

        
        #endregion
    }

    internal enum TimeValidationResult
    {
        TimeIsInvalid,
        HourIsInvalid,
        MinuteIsInvalid,
        Valid 
    };
}
