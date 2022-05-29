using System;

namespace PatientAccess.UI
{
    public interface IActivityEventAggregator {
        event EventHandler ActivityStarted;
        event EventHandler ActivityCompleted;
        event EventHandler AccountLocked;
        event EventHandler AccountUnLocked;
        event EventHandler BedLocked;
        event EventHandler ActivityCancelled;
        event EventHandler ReturnToMainScreen;
        event EventHandler ErrorMessageDisplayed;

        void RemoveAllListeners();
        void RaiseActivityStartEvent(object sender, EventArgs e);
        void RaiseActivityCompleteEvent(object sender, EventArgs e);
        void RaiseAccountLockEvent(object sender, EventArgs e);
        void RaiseAccountUnLockEvent(object sender, EventArgs e);
        void RaiseBedLockEvent(object sender, EventArgs e);
        void RaiseActivityCancelEvent(object sender, EventArgs e);
        void RaiseReturnToMainScreen(object sender, EventArgs e);
        void RaiseErrorMessageDisplayedEvent(object sender, EventArgs e);
    }
}