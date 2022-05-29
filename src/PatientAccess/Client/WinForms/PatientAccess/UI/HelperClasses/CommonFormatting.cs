using System;
using System.Windows.Forms;

namespace PatientAccess.UI.HelperClasses
{
	/// <summary>
	/// Summary description for CommonFormatting.
	/// </summary>
	public class CommonFormatting
	{
        public static string LongDateFormat(DateTime time)
        {
            return String.Format 
                ( LONGDATEFORMATPATTERN
                , time.Month, time.Day, time.Year
                );
        }

        public static string MaskedDateFormat(DateTime dateTime)
        {
            if( dateTime != DateTime.MinValue )
            {
                return String.Format 
                    ( MASKEDDATEFORMATPATTERN
                    , dateTime.Month, dateTime.Day, dateTime.Year
                    );
            }
            else
            {
                return String.Empty;
            }
        }

        public static string DisplayedTimeFormat(DateTime time)
        {
            return String.Format 
                ( DISPLAYEDTIMEFORMATPATTERN
                , time.Hour, time.Minute
                );
        }

        public static string MaskedTimeFormat(DateTime time)
        {
            return String.Format 
                ( MASKEDTIMEFORMATPATTERN
                , time.Hour, time.Minute
                );
        }

        /// <summary>
        /// Convert String to ProperCase
        /// </summary>
        /// <param name="strValue">String to be converted to ProperCase</param>
        /// <returns>The ProperCase String</returns>
        /// 

        public static string ProperCase(string strValue)
        {
            string strProper = strValue.Substring( 0, 1 ).ToUpper();

            strValue = strValue.Substring( 1 ).ToLower();
			
            string strPrev = "";

            for( int intIndex=0; intIndex<strValue.Length; intIndex++ )
            {
                if( intIndex > 1 )
                {
                    strPrev = strValue.Substring( intIndex-1, 1);
                }
                if( strPrev.Equals(" ") || strPrev.Equals("\t") || 
                    strPrev.Equals("\n") || strPrev.Equals(".") )
                {
                    strProper += strValue.Substring(intIndex, 1).ToUpper();
                }
                else
                {
                    strProper += strValue.Substring(intIndex, 1);
                }
            }
            return strProper;
        }


        public static void FormatTextBoxCurrency( TextBox txtBox )
        {
            decimal currencyAmt = ConvertTextToCurrencyDecimal( txtBox );
            txtBox.Text = currencyAmt.ToString( DEFAULT_CURRENCY_FORMAT );  
        }

        public static void FormatTextBoxCurrency( TextBox txtBox, string currencyFormat )
        {
            decimal currencyAmt = ConvertTextToCurrencyDecimal( txtBox );
            txtBox.Text = currencyAmt.ToString( currencyFormat );  
        }

        public static decimal ConvertTextToCurrencyDecimal( TextBox txtBox )
        {
            return txtBox.Text == "" || txtBox.Text == "."? 0: Convert.ToDecimal( txtBox.Text );
        }

        public static string PreFilter(string s)
        {
            string modText = s;
            if (s != null)
            {
                modText = modText.Replace('\n', ' ');
                modText = modText.Replace('\r', ' ');
                modText = modText.Replace('|', ' ');
                modText = modText.Replace('^', ' ');
                modText = modText.Replace('&', ' ');
                modText = modText.Replace('\\', ' ');
                modText = modText.Replace('~', ' ');
                modText = modText.Replace("  ", " ");
                modText = modText.Replace('\t', ' ');
                modText = modText.Trim();
            }
            return modText;
        }

	    private readonly static string LONGDATEFORMATPATTERN = "{0:D2}/{1:D2}/{2:D4}";
	    private readonly static string MASKEDDATEFORMATPATTERN = "{0:D2}{1:D2}{2:D4}";
	    private readonly static string DISPLAYEDTIMEFORMATPATTERN = "{0:D2}:{1:D2}";
	    private readonly static string MASKEDTIMEFORMATPATTERN = "{0:D2}{1:D2}";
	    private readonly static string DEFAULT_CURRENCY_FORMAT = "##,###,##0.00";
    }
}
