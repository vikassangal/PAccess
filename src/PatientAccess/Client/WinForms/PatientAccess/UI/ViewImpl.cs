using System;
using System.Windows.Forms;

namespace Extensions.UI.Winforms
{
    [Serializable]
    internal class ViewImpl : IView
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void UpdateModel()
        {
            this.View.UpdateModel();
        }

        public void UpdateView()
        {
            this.View.UpdateView();
        }

        public void EnableThemesOn( Control aControlAndItsChildren )
        {
            foreach( Control aControl in aControlAndItsChildren.Controls )
            {
                ButtonBase  button  = aControl as ButtonBase;
                GroupBox    group   = aControl as GroupBox;
                Label       label   = aControl as Label;

                if( button != null )
                {
                    button.FlatStyle = FlatStyle.System;
                }
                else if( group != null )
                {
                    group.FlatStyle = FlatStyle.System;
                }
                else if( label != null )
                {
                    label.FlatStyle = FlatStyle.System;
                }

                this.View.EnableThemesOn( aControl );
            }
        }
        #endregion

        #region Properties
        public object Model
        {
            get
            {
                return this.i_Model;
            }
            set
            {
                this.i_Model = value;
            }
        }

        private IView View
        {
            get
            {
                return i_View;
            }
            set
            {
                i_View = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ViewImpl( IView aView )
        {
            this.i_View = aView;
        }
        #endregion

        #region Data Elements
        private IView   i_View;
        private object  i_Model;
        #endregion

        #region Constants
        #endregion
    }
}
