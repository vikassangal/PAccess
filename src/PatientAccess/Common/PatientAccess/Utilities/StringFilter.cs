using System;
using System.Text.RegularExpressions;

namespace PatientAccess.Utilities
{
    /// <summary>
    /// Summary description for StringFilter.
    /// </summary>
    public class StringFilter
    {

        #region Constants

        private const string REGEX_NON_LETTER = "[^a-zA-Z]";
        private const string REGEX_NON_NUMBER = "[^0-9]";
        private const string REGEX_NON_LETTER_AND_NUMBER = "[^a-zA-Z0-9]";
        private const string REGEX_NON_LETTER_NUMBER_AND_SPACE = "[^a-zA-Z0-9 ]";
        private const string REGEX_NON_LETTER_SPACE_AND_HYPHEN = "[^a-zA-Z -]";
        private const string REGEX_NON_LETTER_NUMBER_SPACE_AND_HYPHEN = "[^a-zA-Z0-9 -]";
        private const string REGEX_NON_LETTER_SPACE_HYPHEN_AND_PERIOD = "[^.a-zA-Z -]";
        private const string REGEX_NON_LETTER_NUMBER_SPACE_HYPHEN_AND_PERIOD = "[^.a-zA-Z0-9 -]";
        private const string REGEX_NON_LETTER_NUMBER_SPACE_HYPHEN_PERIOD_AND_FORWARDSLASH = "[^./a-zA-Z0-9 -]";
        private const string REGEX_NON_LETTER_SPACE_HYPHEN_AND_COMMA = "[^,a-zA-Z -]";
        private const string REGEX_EMAIL = "[^~._@!#$%&`*+/=?'{}a-zA-Z0-9 -]";
        private const string REGEX_NON_LETTER__AMPERSAND_HYPHEN_FORWARDSLASH_BLANKSPACE_UNDERSCORE_COMMA = "[^&/_,a-zA-Z -]";

        #endregion

        #region Methods

        public static string mangleName( string name )
        {
            return Regex.Replace( name, "[^a-zA-Z0-9]", String.Empty );
        }

        public static string RemoveHL7Chars( string input )
        {
            string addr = input;
            if ( addr != null )
            {
                addr = addr.Replace( "|", String.Empty );
                addr = addr.Replace( "^", String.Empty );
                addr = addr.Replace( "~", String.Empty );
                addr = addr.Replace( "\\", String.Empty );
                addr = addr.Replace( "&", String.Empty );
            }
            return addr;
        }

        public static string PadString( string input, char paddingChar, int lengthOfString, bool padRight )
        {
            string result = input.Trim();
            if ( lengthOfString > result.Length )
            {
                if ( padRight )
                {
                    result = result.PadRight( lengthOfString, paddingChar );
                }
                else
                {
                    result = result.PadLeft( lengthOfString, paddingChar );
                }
            }
            return result;
        }

        public static string StripMiddleInitialFromFirstName( ref string firstName )
        {
            string middleInitial = String.Empty;
            string workName = firstName.Trim();

            int len = workName.Length;
            if ( len > 2 )
            {
                string miTestForBlankChar = workName.Substring( len - 2, 1 );
                string miLastChar = workName.Substring( len - 1, 1 );

                if ( miTestForBlankChar.Trim() == String.Empty )
                {
                    firstName = workName.Substring( 0, len - 2 ).Trim();
                    middleInitial = miLastChar.Trim();
                }
                else
                {
                    firstName = workName.Trim();
                }
            }
            else
            {
                firstName = workName.Trim();
            }

            return middleInitial;
        }

        public static string RemoveFirstCharNonLetter( string input )
        {
            if ( input.Length == 0 )
            {
                return String.Empty;
            }

            while ( input.Length > 0 )
            {
                string firstChar = input.Substring( 0, 1 );
                if ( BeginsWithNonLetter( firstChar ) )
                {
                    input = input.Length > 1 ? input.Substring( 1 ) : String.Empty;
                }
                else
                {
                    break;
                }
            }
            return input;
        }

        private static string RemoveFirstCharNonNumber( string input )
        {
            if ( input.Length == 0 )
            {
                return String.Empty;
            }

            while ( input.Length > 0 )
            {
                string firstChar = input.Substring( 0, 1 );
                if ( BeginsWithNonNumber( firstChar ) )
                {
                    input = input.Length > 1 ? input.Substring( 1 ) : String.Empty;
                }
                else
                {
                    break;
                }
            }
            return input;
        }

        public static string RemoveFirstCharNonLetterNonNumber( string input )
        {
            if ( input.Length == 0 )
            {
                return String.Empty;
            }

            while ( input.Length > 0 )
            {
                string firstChar = input.Substring( 0, 1 );
                if ( BeginsWithNonLetterNonNumber( firstChar ) )
                {
                    input = input.Length > 1 ? input.Substring( 1 ) : String.Empty;
                }
                else
                {
                    break;
                }
            }
            return input;
        }

        private static string RemoveFirstCharNonLetterNumberAndSpace( string input )
        {
            if ( input.Length == 0 )
            {
                return String.Empty;
            }

            while ( input.Length > 0 )
            {
                string firstChar = input.Substring( 0, 1 );
                if ( BeginsWithNonLetterNumberAndSpace( firstChar ) )
                {
                    input = input.Length > 1 ? input.Substring( 1 ) : String.Empty;
                }
                else
                {
                    break;
                }
            }
            return input;
        }

        public static string RemoveFirstCharNonLetterAndRestNonLetter( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetter( input );
            input = RemoveAllNonLetter( input );

            return input;
        }

        public static string RemoveFirstCharNonNumberAndRestNonNumber( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonNumber( input );
            input = RemoveAllNonNumber( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetterNonNumber( input );
            input = RemoveAllNonLetterAndNumber( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterAndRestNonLetterSpaceAndHyphen( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetter( input );
            input = RemoveAllNonLetterSpaceAndHyphen( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetter( input );
            input = RemoveAllNonLetterNumberSpaceAndHyphen( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetterNonNumber( input );
            input = RemoveAllNonLetterNumberSpaceAndHyphen( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetter( input );
            input = RemoveAllNonLetterSpaceHyphenAndPeriod( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetterNonNumber( input );
            input = RemoveAllNonLetterNumberSpaceHyphenAndPeriod( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetterNonNumber( input );
            input = RemoveAllNonLetterNumberSpaceHyphenPeriodAndForwardSlash( input );

            return input;
        }

        public static string RemoveFirstCharNonLetterNumberSpaceAndRestNonLetterNumberSpaceHyphenAndPeriod( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetterNumberAndSpace( input );
            input = RemoveAllNonLetterNumberSpaceHyphenAndPeriod( input );

            return input;
        }

        

        public static string RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            input = RemoveFirstCharNonLetter( input );
            input = RemoveAllNonLetterSpaceHyphenAndComma( input );

            return input;
        }

        public static string RemoveAllNonEmailSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (HasInvalidSpecialCharacters(input, REGEX_EMAIL))
            {
                input = GetCleanedUpString(input, REGEX_EMAIL);
            }

            return input;
        }

        public static string RemoveFirstCharNonLetterAndRestNonLetterAmpersandHyphenSlashBlankUnderscoreComma(
            string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = RemoveFirstCharNonLetter(input);
            input = RemoveAllNonLetterAndRestNonLetterAmpersandHyphenSlashBlankUnderscoreComma(input);

            return input;
        }

        #endregion Methods

        #region Private Methods

        private static bool HasInvalidSpecialCharacters( string input, string regexString )
        {
            Regex regexObj = new Regex( regexString, RegexOptions.Singleline | RegexOptions.IgnoreCase );
            return regexObj.IsMatch( input );
        }

        private static bool BeginsWithNonLetter( string input )
        {
            Regex regexObj = new Regex( REGEX_NON_LETTER, RegexOptions.Singleline | RegexOptions.IgnoreCase );
            return regexObj.IsMatch( input );
        }

        private static bool BeginsWithNonNumber( string input )
        {
            Regex regexObj = new Regex( REGEX_NON_NUMBER, RegexOptions.Singleline | RegexOptions.IgnoreCase );
            return regexObj.IsMatch( input );
        }

        private static bool BeginsWithNonLetterNonNumber( string input )
        {
            Regex regexObj = new Regex( REGEX_NON_LETTER_AND_NUMBER, RegexOptions.Singleline | RegexOptions.IgnoreCase );
            return regexObj.IsMatch( input );
        }

        private static bool BeginsWithNonLetterNumberAndSpace( string input )
        {
            Regex regexObj = new Regex( REGEX_NON_LETTER_NUMBER_AND_SPACE, RegexOptions.Singleline | RegexOptions.IgnoreCase );
            return regexObj.IsMatch( input );
        }

        private static string GetCleanedUpString( string input, string regexString )
        {
            Regex regexObj2 = new Regex( regexString, RegexOptions.Singleline | RegexOptions.IgnoreCase );
            var cleanedText = regexObj2.Replace( input, String.Empty );
            return cleanedText;
        }

        private static string RemoveAllNonLetter( string input )
        {
            if ( HasInvalidSpecialCharacters( input, REGEX_NON_LETTER ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_LETTER );
            }

            return input;
        }

        private static string RemoveAllNonNumber( string input )
        {
            if ( HasInvalidSpecialCharacters( input, REGEX_NON_NUMBER ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_NUMBER );
            }

            return input;
        }

        private static string RemoveAllNonLetterAndNumber( string input )
        {
            if ( HasInvalidSpecialCharacters( input, REGEX_NON_LETTER_AND_NUMBER ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_LETTER_AND_NUMBER );
            }

            return input;
        }

        private static string RemoveAllNonLetterSpaceAndHyphen( string input )
        {
            if ( HasInvalidSpecialCharacters( input, REGEX_NON_LETTER_SPACE_AND_HYPHEN ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_LETTER_SPACE_AND_HYPHEN );
            }

            return input;
        }

        public static string RemoveAllNonLetterNumberSpaceAndHyphen( string input )
        {
            if ( string.IsNullOrEmpty( input ) )
            {
                return input;
            }

            if ( HasInvalidSpecialCharacters( input, REGEX_NON_LETTER_NUMBER_SPACE_AND_HYPHEN ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_LETTER_NUMBER_SPACE_AND_HYPHEN );
            }

            return input;
        }

        private static string RemoveAllNonLetterSpaceHyphenAndPeriod( string input )
        {
            if ( HasInvalidSpecialCharacters( input, REGEX_NON_LETTER_SPACE_HYPHEN_AND_PERIOD ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_LETTER_SPACE_HYPHEN_AND_PERIOD );
            }

            return input;
        }

        private static string RemoveAllNonLetterNumberSpaceHyphenAndPeriod(string input)
        {
            if ( HasInvalidSpecialCharacters( input, REGEX_NON_LETTER_NUMBER_SPACE_HYPHEN_AND_PERIOD ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_LETTER_NUMBER_SPACE_HYPHEN_AND_PERIOD );
            }

            return input;
        }

        private static string RemoveAllNonLetterNumberSpaceHyphenPeriodAndForwardSlash( string input )
        {
            if ( HasInvalidSpecialCharacters( input, REGEX_NON_LETTER_NUMBER_SPACE_HYPHEN_PERIOD_AND_FORWARDSLASH ) )
            {
                input = GetCleanedUpString( input, REGEX_NON_LETTER_NUMBER_SPACE_HYPHEN_PERIOD_AND_FORWARDSLASH );
            }

            return input;
        }

        private static string RemoveAllNonLetterSpaceHyphenAndComma(string input)
        {
            if (HasInvalidSpecialCharacters(input, REGEX_NON_LETTER_SPACE_HYPHEN_AND_COMMA))
            {
                input = GetCleanedUpString(input, REGEX_NON_LETTER_SPACE_HYPHEN_AND_COMMA);
            }

            return input;
        }

        private static string RemoveAllNonLetterAndRestNonLetterAmpersandHyphenSlashBlankUnderscoreComma(string input)
        {
            if (HasInvalidSpecialCharacters(input,
                REGEX_NON_LETTER__AMPERSAND_HYPHEN_FORWARDSLASH_BLANKSPACE_UNDERSCORE_COMMA))
            {
                input = GetCleanedUpString(input,
                    REGEX_NON_LETTER__AMPERSAND_HYPHEN_FORWARDSLASH_BLANKSPACE_UNDERSCORE_COMMA);
            }

            return input;
        }

        #endregion Private Methods
    }
}