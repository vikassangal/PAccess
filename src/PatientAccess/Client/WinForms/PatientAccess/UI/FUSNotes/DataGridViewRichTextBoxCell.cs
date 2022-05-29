using System;
using System.Drawing;
using System.Windows.Forms;

namespace PatientAccess.UI.FUSNotes
{
    public class DataGridViewRichTextBoxCell : DataGridViewTextBoxCell
    {
        private int maxInputLength = 32767;
        private static DataGridViewRichTextBoxEditingControl editingControl;

        static DataGridViewRichTextBoxCell()
        {
            editingControl = new DataGridViewRichTextBoxEditingControl();
            editingControl.Multiline = false;
            editingControl.BorderStyle = BorderStyle.None;
            editingControl.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
         
        }


        protected override void Paint( Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts )
        {      
            base.Paint( graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts );
        }

        public override Type FormattedValueType
        {
            get { return typeof( string ); }
        }

        public override int MaxInputLength
        {
            get { return maxInputLength; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentOutOfRangeException( "MaxInputLength coudn't be less than 0." );
                }
                maxInputLength = value;
            }
        }

        public override Type ValueType
        {
            get { return typeof( string ); }
        }

        public override object Clone()
        {
            DataGridViewRichTextBoxCell result = (DataGridViewRichTextBoxCell)base.Clone();
            result.maxInputLength = maxInputLength;
            return result;
        }

        public override void DetachEditingControl()
        {
            if( DataGridView == null )
            {
                throw new InvalidOperationException( "There is no associated DataGridView." );
            }
            if( DataGridView.Controls.Contains( editingControl ) )
            {
                DataGridView.Controls.Remove( editingControl );
            }
            Console.WriteLine( "Detached: ({0}, {1});", RowIndex, ColumnIndex );
        }

        public override void InitializeEditingControl( int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle )
        {
            // Set the value of the editing control to the current cell value. 

            base.InitializeEditingControl( rowIndex, initialFormattedValue, dataGridViewCellStyle );
            DataGridViewRichTextBoxEditingControl ctl = (DataGridViewRichTextBoxEditingControl)DataGridView.EditingControl;
            ctl.Text = this.Value.ToString();
        }


        public override Type EditType
        {
            // Return the type of the editing contol that Cell uses. 

            get { return typeof( DataGridViewRichTextBoxEditingControl ); }
        }        

        public override void PositionEditingControl( bool setLocation, bool setSize, Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow )
        {
            if( setSize )
            {
                editingControl.Size = new Size( cellBounds.Width, cellBounds.Height + 2 );
            }
            if( setLocation )
            {
                editingControl.Location = new Point( cellBounds.X, cellBounds.Y );
            }
            editingControl.Invalidate();
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        protected override void OnDataGridViewChanged()
        {
            editingControl.EditingControlDataGridView = DataGridView;
        }

    }
}
