using System;
using System.Diagnostics;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.Factories
{
    /// <summary>
    /// This class should contain all the methods for configuring MaskedEditTextBox controls for SR352.
    /// Once all the configuration and the corresponding scrub and show error message methods are accumulated here 
    /// then we can remove the duplicate ones and name the remaining ones appropriately.
    /// During accumulation name the methods like this ViewNameFieldName.
    /// </summary>
    internal static class MaskedEditTextBoxBuilder
    {
        #region Methods

        public static void ConfigureChiefComplaint( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;
            textBox.KeyPressExpression = RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_PERIOD_ALLOWED );
        }

        public static void ConfigurePassport(MaskedEditTextBox textBox)
        {
            textBox.ValidationExpression = RegularExpressions.LettersAndOrNumbers;
            textBox.KeyPressExpression = RegularExpressions.LettersAndOrNumbers;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.HasLettersAndNumbers,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber,
                    UIErrorMessages.ONLY_LETTERS_NUMBERS_ALLOWED);
        }

        public static void ConfigureEmployeeID( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.LettersAndOrNumbers;
            textBox.KeyPressExpression = RegularExpressions.LettersAndOrNumbers;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.HasLettersAndNumbers,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber,
                    UIErrorMessages.ONLY_LETTERS_NUMBERS_ALLOWED );
        }

        public static void ConfigureUnMaskedUSZipCode( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.ZeroOrMoreNumbers;
            textBox.KeyPressExpression = RegularExpressions.ZeroOrMoreNumbers;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.IsZeroOrMoreNumbers,
                    input => input,
                    UIErrorMessages.ONLY_NUMBERS_ALLOWED );
        }

        public static void ConfigureUSZipCode( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.ZeroOrMoreNumbers;
            textBox.KeyPressExpression = RegularExpressions.ZeroOrMoreNumbers;

            // 1) Due to the presence of a Mask("     -") in the US Zip Code Text box, 
            // handling a Paste event is complex and cause undesired effects in the 
            // output string and cursor position after paste. Hence, we simply return
            // the incoming input with an error message for an invalid US Zip Code.
            // 
            // 2) Remove 5 spaces from input that get inserted as part of the
            // mask for US Zip Code before validating the incoming string

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RemoveLastFiveSpaces,
                    RegularExpressions.IsZeroOrMoreNumbers,
                    input => input,
                    UIErrorMessages.ONLY_NUMBERS_ALLOWED );
        }

        public static void ConfigureNonUSZipCode( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphen;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphen;

            // 1) Due to the presence of a Mask("     -") in the US Zip Code Text box, 
            // handling a Paste event is complex and cause undesired effects in the 
            // output string and cursor position after paste. Hence, we simply return
            // the incoming input with an error message for an invalid US Zip Code.
            //
            // 2) For a Non-US Zip Code, the paste event can be handled since there is no
            // Mask involved, but just to keep the Zip Code requirements uniform for both
            // US and Non-US during paste, we do not scrub the invalid characters but simply
            // return the incoming input with an errormessage for an invalid Non-US Zip Code too.

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphen,
                    input => input,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_ALLOWED );
        }

        public static void ConfigureEmployerName( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphen;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphen;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphen,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceAndHyphen,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_ALLOWED );
        }

        public static void ConfigureProcedure( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_PERIOD_ALLOWED );
        }

        public static void ConfigureEmergencyContactName( MaskedEditTextBox maskedEditTextBox )
        {
            maskedEditTextBox.ValidationExpression = RegularExpressions.StartWithLetterRestLettersSpaceHyphenComma;
            maskedEditTextBox.KeyPressExpression = RegularExpressions.StartWithLetterRestLettersSpaceHyphenComma;

            maskedEditTextBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterRestLettersSpaceHyphenComma,
                    StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_COMMA_ALLOWED );
        }

        public static void ConfigureFirstNameAndLastName( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.StartWithLetterRestLettersNumbersSpaceHyphen;
            textBox.KeyPressExpression = RegularExpressions.StartWithLetterRestLettersNumbersSpaceHyphen;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterRestLettersNumbersSpaceHyphen,
                    StringFilter.RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_ALLOWED );
        }

        public static void ConfigureMIAndSuffix( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.ZeroOrMoreLetters;
            textBox.KeyPressExpression = RegularExpressions.ZeroOrMoreLetters;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.IsZeroOrMoreLetters,
                    StringFilter.RemoveFirstCharNonLetterAndRestNonLetter,
                    UIErrorMessages.ONLY_LETTERS_ALLOWED );
        }

        public static void ConfigureEmail( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.EmailValidFormatExpression;
            textBox.KeyPressExpression = RegularExpressions.EmailValidCharactersExpression;

            //due to the complexity of email addresses we do not attempt to scrub out dis-allowed characters rather 
            //we simply return an empty string whenever we have an invalid email address as input
            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.HasValidEmailSpecialCharacters,
                    input => string.Empty,
                    UIErrorMessages.ONLY_VALID_EMAIL_CHARACTERS_ALLOWED );
        }

        public static void ConfigureAddressStreet( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriodForwardSlash;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriodForwardSlash;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriodForwardSlash,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenPeriodAndForwardSlash,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_FORWARDSLASH_PERIOD_ALLOWED );
        }

        public static void ConfigureAddressCity( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.StartWithLetterRestLettersSpaceHyphenPeriod;
            textBox.KeyPressExpression = RegularExpressions.StartWithLetterRestLettersSpaceHyphenPeriod;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterRestLettersSpaceHyphenPeriod,
                    StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndPeriod,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_PERIOD_ALLOWED );
        }

        public static void ConfigureClinicalViewComments( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_PERIOD_ALLOWED );
        }

        public static void ConfigureInsuranceVerificationRemarks( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_PERIOD_ALLOWED );
        }

        public static void ConfigureAuthorizationRemarks( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_PERIOD_ALLOWED );
        }

        public static void ConfigureTransferInpatientToOutpatientRemarks( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;
            textBox.KeyPressExpression =
                RegularExpressions.StartWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterOrNumberRestLettersNumbersSpaceHyphenPeriod,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterNumberSpaceHyphenAndPeriod,
                    UIErrorMessages.ONLY_SPACE_HYPHEN_PERIOD_ALLOWED );
        }
        public static void ConfigureOtherLanguage( MaskedEditTextBox maskedEditTextBox )
        {
            maskedEditTextBox.ValidationExpression = RegularExpressions.StartWithLetterRestLettersSpaceHyphenComma;
            maskedEditTextBox.KeyPressExpression = RegularExpressions.StartWithLetterRestLettersSpaceHyphenComma;

            maskedEditTextBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.StartsWithLetterRestLettersSpaceHyphenComma,
                    StringFilter.RemoveFirstCharNonLetterAndRestNonLetterSpaceHyphenAndComma,
                    UIErrorMessages.ONLY_Letters_SPACE_HYPHEN_COMMA_ALLOWED);
        }

        public static void ConfigureCptCode( MaskedEditTextBox textBox )
        {
            textBox.ValidationExpression = RegularExpressions.LettersAndOrNumbers;
            textBox.KeyPressExpression = RegularExpressions.LettersAndOrNumbers;

            textBox.prePasteEdit =
                CreatePrePasteDelegate(
                    RegularExpressions.HasLettersAndNumbers,
                    StringFilter.RemoveFirstCharNonLetterNonNumberAndRestNonLetterAndNumber,
                    UIErrorMessages.ONLY_LETTERS_NUMBERS_ALLOWED );
        }

        #endregion Methods

        #region Private Methods

        private static PrePasteEdit CreatePrePasteDelegate( Func<string, bool> isValid, Func<string, string> scrub, string errorMessage )
        {
            return CreatePrePasteDelegate( input => input, isValid, scrub, errorMessage );
        }

        private static PrePasteEdit CreatePrePasteDelegate( Func<string, string> processBeforeValidation, Func<string, bool> validate, Func<string, string> scrub, string errorMessage )
        {
            Debug.Assert( processBeforeValidation != null );
            Debug.Assert( validate != null );
            Debug.Assert( scrub != null );
            Debug.Assert( !String.IsNullOrEmpty( errorMessage ) );

            return input =>
                {

                    if ( !String.IsNullOrEmpty( input ) )
                    {
                        input = processBeforeValidation( input );
                        bool isValid = validate( input );

                        if ( !isValid )
                        {
                            string scrubbedInput = scrub( input );

                            MessageBox.Show( errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                            return scrubbedInput;
                        }
                    }

                    return input;
                };
        }

        private static string RemoveLastFiveSpaces( string input )
        {
            // If the user is trying to copy-paste a string with spaces, using Trim() here would
            // remove those invalid spaces but no error message will get displayed when the incoming
            // string is matched up with the Regex for a US Zip Code, which is not correct and would
            // make the user to assume that the pasted invalid string was a valid string. Hence, we
            // just need to remove the last five spaces which are inserted by the system as a Mask.

            return input.Length < 5 ? input : input.Substring( 0, input.Length - 5 );
        }

        #endregion Private Methods
    }
}