using System;

namespace PatientAccess.UI
{
    /// <summary>
    /// ActivityEventAggregator
    /// </summary>
    public class ActivityEventAggregator : IActivityEventAggregator
    {
        #region Event Handlers
        public event EventHandler ActivityStarted;
        public event EventHandler ActivityCompleted;
        public event EventHandler AccountLocked;
        public event EventHandler AccountUnLocked;
        public event EventHandler BedLocked;
        public event EventHandler ActivityCancelled;
        public event EventHandler ReturnToMainScreen;
        public event EventHandler ErrorMessageDisplayed;

        #endregion

        #region Methods
        public static ActivityEventAggregator GetInstance()
        {
            if (i_instance == null)
            {
                lock (syncRoot)
                {
                    if (i_instance == null)
                    {
                        i_instance = new ActivityEventAggregator();
                    }
                }
            }
            return i_instance;
        }

        public void RemoveAllListeners()
        {
            ActivityStarted = null;
            ActivityCompleted = null;
            AccountLocked = null;
            AccountUnLocked = null;
            BedLocked = null;
            ActivityCancelled = null;
            ErrorMessageDisplayed = null;
        }

        public void RaiseActivityStartEvent(object sender, EventArgs e)
        {
            if (ActivityStarted != null)
            {
                ActivityStarted(sender, e);
            }
        }

        public void RaiseActivityCompleteEvent(object sender, EventArgs e)
        {
            if (ActivityCompleted != null)
            {
                ActivityCompleted(sender, e);
            }
        }

        public void RaiseAccountLockEvent(object sender, EventArgs e)
        {
            if (AccountLocked != null)
            {
                AccountLocked(sender, e);
            }
        }

        public void RaiseAccountUnLockEvent(object sender, EventArgs e)
        {
            if (AccountUnLocked != null)
            {
                AccountUnLocked(sender, e);
            }
        }

        public void RaiseBedLockEvent(object sender, EventArgs e)
        {
            if (BedLocked != null)
            {
                BedLocked(sender, e);
            }
        }
       
        public void RaiseActivityCancelEvent(object sender, EventArgs e)
        {
            if (ActivityCancelled != null)
            {
                ActivityCancelled(sender, e);
            }
        }
        
        public void RaiseReturnToMainScreen(object sender, EventArgs e)
        {
            if (ReturnToMainScreen != null)
            {
                ReturnToMainScreen(sender, e);
            }
        }

        public void RaiseErrorMessageDisplayedEvent(object sender, EventArgs e)
        {
            if (ErrorMessageDisplayed != null)
            {
                ErrorMessageDisplayed(sender, e);
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
        protected ActivityEventAggregator()
        {
        }
        #endregion

        #region Data Elements
        private static volatile ActivityEventAggregator i_instance = null;
        private static object syncRoot = new Object();
        #endregion

        #region Constants
        #endregion

    }
}

