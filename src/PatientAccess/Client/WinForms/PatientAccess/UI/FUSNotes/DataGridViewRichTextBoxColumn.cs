using System;
using System.Windows.Forms;

namespace PatientAccess.UI.FUSNotes
{
    class DataGridViewRichTextBoxColumn : DataGridViewColumn
    {
		private DataGridViewColumnSortMode sortMode;

        public DataGridViewRichTextBoxColumn() : base( new DataGridViewRichTextBoxCell() ) 
        {
		}

		public override DataGridViewCell CellTemplate 
        {
            get { return base.CellTemplate; }

            set
            {

                // Ensure that the cell used for the template is a RichTextBox Cell. 

                if( ( value != null ) && !value.GetType().IsAssignableFrom( typeof( DataGridViewRichTextBoxCell ) ) )
                {
                    throw new InvalidCastException( "Must be a RichTextBoxCell" );
                }

                base.CellTemplate = value;
            } 
		}

		public new DataGridViewColumnSortMode SortMode {
			get { return sortMode; }
			set {
				if (DataGridView != null && DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect && value != DataGridViewColumnSortMode.NotSortable) {
					throw new InvalidOperationException("Value conflicts with DataGridView.SelectionMode.");
				}
				sortMode = value;
			}
		}

		public override string ToString () {
			return GetType().Name;
		}

    }
}
