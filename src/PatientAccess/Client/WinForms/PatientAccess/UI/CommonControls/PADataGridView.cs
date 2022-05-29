using System;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    public partial class PADataGridView : DataGridView
    {
        //public event EventHandler TabAtTop;
        //public event EventHandler TabAtBottom;
        public event EventHandler DoEnterAction;

        public PADataGridView()
        {
            InitializeComponent();
        }

        protected override bool ProcessDialogKey( Keys keyData )
        {
            Keys key = ( keyData & Keys.KeyCode );

            if( key == Keys.Enter )
            {
                if( this.DoEnterAction != null )
                {
                    this.DoEnterAction( this, null );                   
                }

                return true;
            }
            else
            {
                return base.ProcessDialogKey( keyData );
            }
        }

        protected override bool ProcessDataGridViewKey( KeyEventArgs e )
        {
            if( e.KeyCode == Keys.Enter )
            {
                if( this.DoEnterAction != null )
                {
                    this.DoEnterAction( this, null );
                }

                return true;
            }
            else
            {
                return base.ProcessDataGridViewKey( e );
            }
        }
    }
}
