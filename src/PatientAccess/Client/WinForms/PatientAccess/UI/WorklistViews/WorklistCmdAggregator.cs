using System;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// WorklistCmdAggregator
    /// Sends events to form that functions as the parent form when a worklist action
    /// object invokes the AccountView.  The events function to dispose of the AccountView
    /// and dispose of the parent form.
    /// </summary>
    [Serializable]
    public class WorklistCmdAggregator : object
    {
        #region Events
        public event EventHandler ActionSelected;
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public static WorklistCmdAggregator GetInstance()
        {
            if( c_instance == null )
            {
                lock( typeof( WorklistCmdAggregator ) )
                {
                    if( c_instance == null )
                    {
                        c_instance = new WorklistCmdAggregator();
                    }
                }
            }

            return c_instance;
        }

        public void RemoveAllListeners()
        {
            this.ActionSelected = null;
        }

        public void RaiseActionSelectedEvent( object sender, EventArgs e)
        {
            if( this.ActionSelected != null )
            {
                this.ActionSelected( sender, e );
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        protected WorklistCmdAggregator()
        {
        }
        #endregion

        #region Data Elements
        private static volatile WorklistCmdAggregator c_instance = null;
        #endregion

        #region Constants
        #endregion
    }
}
