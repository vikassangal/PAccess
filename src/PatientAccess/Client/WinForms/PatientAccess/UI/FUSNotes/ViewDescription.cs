using System;
using System.Drawing;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.FUSNotes
{
    public partial class ViewDescription : TimeOutFormView
    {
        public ViewDescription()
        {
            InitializeComponent();
        }

        public ViewDescription( string message )
        {
            InitializeComponent();

            this.richTextBox1.Text = message;

        }

        private void btnOK_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        public void SetBackgroundToWhite()
        {
            this.richTextBox1.BackColor = Color.White;
        }
    }
}