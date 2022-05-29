using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccessClientTests.Tests.Unit.PatientAccess.UI.CommonControls
{
    public class FakeMaskedEditTextBox : MaskedEditTextBox
    {
        public void RaiseKeyPressWith( char key )
        {
            this.OnKeyPress( new KeyPressEventArgs( key ) );
        }

        public void SimulatePasteOperation( string input )
        {

        }
    }
}