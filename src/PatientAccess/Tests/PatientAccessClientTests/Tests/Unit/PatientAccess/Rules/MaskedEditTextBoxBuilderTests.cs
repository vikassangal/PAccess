using NUnit.Framework;
using PatientAccess.UI.Factories;
using PatientAccessClientTests.Tests.Unit.PatientAccess.UI.CommonControls;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class MaskedEditTextBoxBuilderTests
    {
        [Test]
        public void StreetAddressShouldAllowTheHyphenCharacter()
        {
            var textBox = new FakeMaskedEditTextBox
            {
                Mask = string.Empty,
                MaxLength = 50,
            };

            MaskedEditTextBoxBuilder.ConfigureAddressStreet( textBox );
            const string userInput = "he*ll-o";
            const string textShown = "hell-o";

            foreach ( char c in userInput )
            {
                textBox.RaiseKeyPressWith( c );
            }

            Assert.AreEqual( textShown, textBox.Text );
        }

        [Test]
        public void StreetAddressShouldAllowTheForwardSlashCharacter()
        {
            var textBox = new FakeMaskedEditTextBox
            {
                Mask = string.Empty,
                MaxLength = 50,
            };

            MaskedEditTextBoxBuilder.ConfigureAddressStreet( textBox );
            const string userInput = "hell/o";
            const string textShown = "hell/o";

            foreach ( char c in userInput )
            {
                textBox.RaiseKeyPressWith( c );
            }

            Assert.AreEqual( textShown, textBox.Text );
        }
    }
}
