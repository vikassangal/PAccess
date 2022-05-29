using System.Collections;
using System.Windows.Forms;

namespace PatientAccess.UI.HelperClasses
{
    public class ComboBoxHelper 
    {
        #region Methods
        public virtual void PopulateWithCollection( ICollection list )
        {
            ComboBox.ValueMember   = ValueMember;
            ComboBox.DisplayMember = DisplayMember;

            ComboBox.Items.Clear(); 

            foreach( object obj in list )
            {
                ComboBox.Items.Add( obj );
            }
            ComboBox.SelectedIndex = -1;
        }

        public virtual object GetSelectedObject()
        {
            return ComboBox.SelectedItem;
        }
        
        public virtual void SetSelectedObject(object obj)
        {
            ComboBox.SelectedIndex = -1;
            for( int idx=0; idx<ComboBox.Items.Count; idx++ )
            {
                if( !(AddBlankEntry && (idx == 0))  )
                {
                    object listObj = ComboBox.Items[ idx ];
                    if( ItemComparer.Compare( obj, listObj ) == 0 )
                    {
                        ComboBox.SelectedIndex = idx;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Properties

        protected IComparer ItemComparer
        {
            get
            {
                return i_ItemComparer;
            }
            set
            {
                i_ItemComparer = value;
            }
        }


        private ComboBox ComboBox
        {
            get
            {
                return i_ComboBox;
            }
            set
            {
                i_ComboBox = value;
            }
        }

        private string ValueMember
        {
            get
            {
                return i_ValueMember;
            }
            set
            {
                i_ValueMember = value;
            }
        }

        public string DisplayMember
        {
            get
            {
                return i_DisplayMember;
            }
            set
            {
                i_DisplayMember = value;
            }
        }

        private bool AddBlankEntry
        {
            get
            {
                return i_AddBlankEntry;
            }
            set
            {
                i_AddBlankEntry = value;
            }
        }

        public object SelectedItem
        {
            get
            {
                return GetSelectedObject();
            }
            set
            {
                SetSelectedObject( value );
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ComboBoxHelper( ComboBox comboBox, string valueMember, string displayMember )
        {
            this.i_ComboBox = comboBox;
            this.i_ValueMember = valueMember;
            this.i_DisplayMember = displayMember;
            this.i_AddBlankEntry = false;
            this.i_ItemComparer = Comparer.DefaultInvariant;
        }
        #endregion

        #region Data Elements
        private ComboBox i_ComboBox;
        private string i_ValueMember;
        private string i_DisplayMember;
        private bool i_AddBlankEntry;
        private IComparer i_ItemComparer;
        #endregion
    }
}
