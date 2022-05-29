using System.Windows.Forms;
using Extensions.UI.Winforms;
using NUnit.Framework;
using PatientAccessClientTests.Tests.Unit.PatientAccess.UI.CommonControls;

namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    [TestFixture]
    public class MaskedEditTextBoxTests
    {
        [Test]
        public void TestEnteringChractersOnebyOne_ShouldSkipInvalidCharacters()
        {
            var textBox = new FakeMaskedEditTextBox
                {
                    CharacterCasing = CharacterCasing.Upper,
                    EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd,
                    KeyPressExpression = "^[a-zA-Z][ a-zA-Z0-9\\ -]*$",
                    Location = new System.Drawing.Point( 335, 21 ),
                    Mask = string.Empty,
                    MaxLength = 13,
                    Name = "mtbFirstName",
                    Size = new System.Drawing.Size( 162, 20 ),
                    TabIndex = 2,
                    ValidationExpression = "^[a-zA-Z][ a-zA-Z0-9\\ -]*$"
                };

            const string invalidInput = "1he*llo";
            const string inputWithIllegalCharacterStripped = "hello";

            foreach ( char c in invalidInput )
            {
                textBox.RaiseKeyPressWith( c );
            }

            Assert.AreEqual( inputWithIllegalCharacterStripped, textBox.Text );
        }

        [Test]
        [Ignore]
//        [HostType( "Moles" )]
        public void TestPrePasteEditMethodIsCalledBeforePastedTextIsProcessed()
        {
//            var textBox = new MaskedEditTextBox
//                {
//                    CharacterCasing = CharacterCasing.Upper,
//                    EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd,
//                    KeyPressExpression = "^[a-zA-Z][ a-zA-Z0-9\\ -]*$",
//                    Location = new System.Drawing.Point( 335, 21 ),
//                    Mask = string.Empty,
//                    MaxLength = 13,
//                    Name = "mtbFirstName",
//                    Size = new System.Drawing.Size( 162, 20 ),
//                    TabIndex = 2,
//                    ValidationExpression = "^[a-zA-Z][ a-zA-Z0-9\\ -]*$"
//                };
//
//            const string pastedText = "t#e%st@╫";
//            IDataObject textData = new DataObject( DataFormats.Text, pastedText );
//            MClipboard.GetDataObject = () => { return textData; };
//            bool prePasteWasCalled = false;
//            textBox.prePasteEdit = s =>
//                {
//                    prePasteWasCalled = true;
//                    return string.Empty;
//                };
//
//            textBox.ProcContextMenu( "PASTE" );
//            Assert.IsTrue( prePasteWasCalled );
//
//            Assert.AreEqual( pastedText, textBox.Text );

        }
    }
} 
