using System;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// SearchEventAggregator 
    /// </summary>
    [Serializable]
    public class SearchEventAggregator : object
    {
        #region Events
        public event EventHandler AccountSelected;
        public event EventHandler ActivatePreregisteredAccount;
        public event EventHandler PreMseRegistrationEvent;
        public event EventHandler UCPreMseRegistrationEvent;
        public event EventHandler CancelPreRegistrationEvent;
        public event EventHandler EditUCPreMSEAccount;
        public event EventHandler EditEDPreMSEAccount;
        public event EventHandler EditRegistrationEvent;
        public event EventHandler CreateNewBornAccountEvent;
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public static SearchEventAggregator GetInstance()
        {
            if( c_instance == null )
            {
                lock( typeof( SearchEventAggregator ) )
                {
                    if( c_instance == null )
                    {
                        c_instance = new SearchEventAggregator();
                    }
                }
            }

            return c_instance;
        }

        public void RemoveAllListeners()
        {
            this.AccountSelected = null;
            this.ActivatePreregisteredAccount = null;
            this.PreMseRegistrationEvent = null;
            this.UCPreMseRegistrationEvent = null;
            this.CancelPreRegistrationEvent = null;
            this.EditRegistrationEvent = null;
            this.CreateNewBornAccountEvent = null;
            this.EditUCPreMSEAccount = null;
            this.EditEDPreMSEAccount = null;
        }

        public void RaiseEditRegistrationEvent( object sender, EventArgs e)
        {
            if( EditRegistrationEvent != null )
            {
                EditRegistrationEvent(sender, e);
            }
        }

        public void RaisePreMseRegistrationEvent(object sender, EventArgs e)
        {
            if (PreMseRegistrationEvent != null)
            {
                PreMseRegistrationEvent(sender, e);
            }
           
        }
        public void RaiseUCCPreMseRegistrationEvent(object sender, EventArgs e)
        {
            if (UCPreMseRegistrationEvent != null)
            {
                UCPreMseRegistrationEvent(sender, e);
            }
        }
        public void RaiseEditUCPreMseRegistrationEvent(object sender, EventArgs e)
        {
            if (EditUCPreMSEAccount != null)
            {
                EditUCPreMSEAccount(sender, e);
            }
        }
        public void RaiseEditEDPreMseRegistrationEvent(object sender, EventArgs e)
        {
            if (EditEDPreMSEAccount != null)
            {
                EditEDPreMSEAccount(sender, e);
            }
        }


        public void RaiseAccountSelectedEvent( object sender, EventArgs e)
        {
            if( AccountSelected != null )
            {
                AccountSelected( sender, e );
            }
        }

        public void RaiseActivatePreregisteredAccountEvent( object sender, EventArgs e)
        {
            if( ActivatePreregisteredAccount != null )
            {
                ActivatePreregisteredAccount( sender, e );
            }
        }

        public void RaiseCancelPreRegistrationEvent( object sender, EventArgs e)
        {
            if( CancelPreRegistrationEvent != null )
            {
                CancelPreRegistrationEvent( sender, e );
            }
        }

        public void RaiseCreateNewBornAccountEvent( object sender, EventArgs e)
        {
            if( CreateNewBornAccountEvent != null )
            {
                CreateNewBornAccountEvent( sender, e );
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
        protected SearchEventAggregator()
        {
        }
        #endregion

        #region Data Elements
        private static volatile SearchEventAggregator c_instance = null;
        #endregion

        #region Constants
        #endregion
    }
}
