using System;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
	/// CencusEventAggregator
	/// </summary>
	public class CensusEventAggregator
	{
		#region Event Handlers
        public event EventHandler CloseEventHandler;
        
		#endregion
	
		#region Methods
        public static CensusEventAggregator GetInstance()
        {
            if( i_instance == null )
            {
                lock( typeof( CensusEventAggregator ) )
                {
                    if( i_instance == null )
                    {
                        i_instance = new CensusEventAggregator();
                    }
                }
            }
            return i_instance;
        }

        public void RemoveAllListeners()
        {
            this.CloseEventHandler = null;
        }

        public void RaiseCloseEvent( object sender, EventArgs e )
        {
            if (this.CloseEventHandler != null)
            {
                this.CloseEventHandler(sender, e);
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
		protected CensusEventAggregator()
		{
		}
		#endregion
	
		#region Data Elements
        private static volatile CensusEventAggregator i_instance = null;
		#endregion
	
		#region Constants
		#endregion

	}
}


