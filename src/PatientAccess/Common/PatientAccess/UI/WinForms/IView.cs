using System.Windows.Forms;

namespace Extensions.UI.Winforms
{
    public interface IView
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        void UpdateModel();

        void UpdateView();

        void EnableThemesOn( Control aControlAndItsChildren );
        #endregion

        #region Properties
        object Model
        {
            get;
            set;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        #endregion
    }
}
