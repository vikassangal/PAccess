using System;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// PatientAccessViewPopulationAggregator 
    /// </summary>
    [Serializable]
    public class PatientAccessViewPopulationAggregator : object
    {
        #region Events
        public event EventHandler ActionSelected;
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public static PatientAccessViewPopulationAggregator GetInstance()
        {
            if( c_instance == null )
            {
                lock( typeof( PatientAccessViewPopulationAggregator ) )
                {
                    if( c_instance == null )
                    {
                        c_instance = new PatientAccessViewPopulationAggregator();
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
        protected PatientAccessViewPopulationAggregator()
        {
        }
        #endregion

        #region Data Elements
        private static volatile PatientAccessViewPopulationAggregator c_instance = null;
        #endregion

        #region Constants
        #endregion
    }
}
