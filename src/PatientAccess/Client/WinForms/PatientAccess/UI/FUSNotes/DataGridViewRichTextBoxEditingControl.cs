using System;
using System.Windows.Forms;

namespace PatientAccess.UI.FUSNotes
{
    public class DataGridViewRichTextBoxEditingControl : RichTextBox, IDataGridViewEditingControl
    {
        private DataGridView editingControlDataGridView;
        private int rowIndex;


        private bool editingControlValueChanged;
        private Cursor editingPanelCursor = Cursors.Default;
        private bool repositionEditingControlOnValueChange = false;

        public DataGridView EditingControlDataGridView
        {
            get { return editingControlDataGridView; }
            set { editingControlDataGridView = value; }
        }

        public object EditingControlFormattedValue
        {
            get { return base.Text; }
            set { base.Text = (string)value; }
        }

        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        public bool EditingControlValueChanged
        {
            get { return editingControlValueChanged; }
            set { editingControlValueChanged = value; }
        }

        public Cursor EditingPanelCursor
        {
            get { return editingPanelCursor; }
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return repositionEditingControlOnValueChange; }
        }

        public void ApplyCellStyleToEditingControl( DataGridViewCellStyle dataGridViewCellStyle )
        {
            Font = dataGridViewCellStyle.Font;
            BackColor = dataGridViewCellStyle.BackColor;
            ForeColor = dataGridViewCellStyle.ForeColor;
        }

        public bool EditingControlWantsInputKey( Keys keyData, bool dataGridViewWantsInputKey )
        {
            return true;
        }

        public object GetEditingControlFormattedValue( DataGridViewDataErrorContexts context )
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit( bool selectAll )
        {
            Focus();
            if( selectAll )
            {
                SelectAll();
            }
            editingControlValueChanged = false;
        }

        protected override void OnTextChanged( EventArgs e )
        {
            base.OnTextChanged( e );
            editingControlValueChanged = true;
            Console.WriteLine( "OnTextChanged: {0};", Text );
            if( editingControlDataGridView != null )
            {
                editingControlDataGridView.CurrentCell.Value = Text;
            }
        }
    }
}