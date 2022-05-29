using System.Text.RegularExpressions;

namespace PatientAccess.Utilities
{
    public static class RegularExpressions
    {

        /// <summary>
        /// Initializes the <see cref="RegularExpressions"/> class.
        /// </summary>
        static RegularExpressions()
        {
            ZeroOrMoreLetters = "^[a-zA-Z]*$";

            ZeroOrMoreNumbers = "^\\d*$";

            LettersAndOrNumbers = "^[a-zA-Z0-9]*$";

            StartWithLetterRestLettersNumbersSpaceHyphen = "^[a-zA-Z][ a-zA-Z0-9-]*$";

            StartWithLetterOrNumberRestLettersNumbersSpaceHyphen = "^[a-zA-Z0-9][ a-zA-Z0-9-]*$";

            StartWithLetterRestLettersSpaceHyphenComma = "^[a-zA-Z][ a-zA-Z-,]*$";

            StartWithLetterRestLettersSpaceHyphenPeriod = "^[a-zA-Z][ a-zA-Z-.]*$";

            StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod = "^[a-zA-Z0-9][ a-zA-Z0-9-.]*$";

            StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriodForwardSlash = "^[a-zA-Z0-9][ a-zA-Z0-9-./]*$";

            //this was taken from the book Regular Expressions Cookbook
            //http://www.amazon.com/Regular-Expressions-Cookbook-Jan-Goyvaerts/dp/0596520689/ref=sr_1_1?ie=UTF8&s=books&qid=1270765218&sr=8-1
            //added (?i) to make is case insensitive without having to set regular expression options on the object
            EmailValidFormatExpression = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@" + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\." + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|" + @"([a-zA-Z0-9-_]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,2})$";

            EmailValidCharactersExpression = "^[~._@!#$%&`*+/=?'{}a-zA-Z0-9 -]*$";

            LettersAndOrNumbersOfLengthFive = "^[a-zA-Z0-9]{5}$";
        }

        #region Properties
        
        //TODO-AC-SR352 Do we need two different email regexes?
        public static string EmailValidFormatExpression { get; private set; }

        public static string EmailValidCharactersExpression { get; private set; }

        public static string ZeroOrMoreLetters { get; private set; }

        public static string ZeroOrMoreNumbers { get; private set; }

        public static string LettersAndOrNumbers { get; private set; }

        public static string StartWithLetterRestLettersNumbersSpaceHyphen { get; private set; }

        public static string StartWithLetterRestLettersSpaceHyphenComma { get; private set;}

        public static string StartWithLetterRestLettersSpaceHyphenPeriod { get; private set; }

        public static string StartWithLetterOrNumberRestLettersNumbersSpaceHyphen { get; private set; }

        public static string StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod { get; private set; }

        public static string StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriodForwardSlash { get; private set; }

        public static string LettersAndOrNumbersOfLengthFive { get; private set; }

        #endregion

        #region Methods

        public static bool StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod(string input)
        {
            return Regex.IsMatch(input, StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod);
        }

        public static bool StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriodForwardSlash( string input )
        {
            return Regex.IsMatch( input, StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriodForwardSlash );
        }

        public static bool StartsWithLetterRestLettersSpaceHyphenPeriod( string input )
        {
            return Regex.IsMatch( input, StartWithLetterRestLettersSpaceHyphenPeriod );
        }

        public static bool StartsWithLetterRestLettersSpaceHyphenComma( string input )
        {
            return Regex.IsMatch( input, StartWithLetterRestLettersSpaceHyphenComma );
        }

        public static bool StartsWithLetterOrNumberRestLettersNumbersSpaceHyphen(string input)
        {
            return Regex.IsMatch(input, StartWithLetterOrNumberRestLettersNumbersSpaceHyphen);
        }

        public static bool HasLettersAndNumbers(string input)
        {
            return Regex.IsMatch(input, LettersAndOrNumbers);
        }

        public static bool StartsWithLetterRestLettersNumbersSpaceHyphen( string input )
        {
            return Regex.IsMatch( input, StartWithLetterRestLettersNumbersSpaceHyphen );
        }

        public static bool IsZeroOrMoreLetters(string input)
        {
            return Regex.IsMatch(input, ZeroOrMoreLetters);
        }

        public static bool IsZeroOrMoreNumbers( string input )
        {
            return Regex.IsMatch( input, ZeroOrMoreNumbers );
        }

        public static bool HasValidEmailSpecialCharacters( string input )
        {
            return Regex.IsMatch( input, EmailValidCharactersExpression );
        }

        #endregion
    }
}
