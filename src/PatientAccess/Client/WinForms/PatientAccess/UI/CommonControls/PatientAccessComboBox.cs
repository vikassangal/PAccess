using System;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for PatientAccessComboBox.
	/// </summary>
	//TODO: Create XML summary comment for PatientAccessComboBox
    [Serializable]
    public class PatientAccessComboBox : ComboBox
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        
        public new object SelectedItem
        {
            get
            {
                return base.SelectedItem;
            }
            set
            {
                this.EnsureItemIsPresent( value );
                base.SelectedItem = value;
            }
        }
        #endregion

        #region Private Methods

        private void EnsureItemIsPresent ( object obj)
        {
            if( obj != null )
            {
                bool found = false;
                for( int i = 0; i < this.Items.Count ; i++ )
                {
                    if( obj.Equals( this.Items[i] ) )
                    {
                        found = true;
                        break;
                    }
                }
                if( !found )
                {
                    this.Items.Add( obj );
                }
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PatientAccessComboBox()
            :base()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
