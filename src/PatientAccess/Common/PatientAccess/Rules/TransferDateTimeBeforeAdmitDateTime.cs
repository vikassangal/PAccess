using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for TransferDateTimeBeforeAdmitDateTime.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class TransferDateTimeBeforeAdmitDateTime : LeafRule
    {
        #region Events

        public event EventHandler TransferDateTimeBeforeAdmitDateTimeEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.TransferDateTimeBeforeAdmitDateTimeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.TransferDateTimeBeforeAdmitDateTimeEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.TransferDateTimeBeforeAdmitDateTimeEvent = null;            
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            
            Account anAccount = (Account)context;

            if( anAccount.TransferDate != DateTime.MinValue 
                &&  ( anAccount.TransferDate.Date < anAccount.AdmitDate.Date ) 
                    ||
                    ( anAccount.TransferDate.Year != 0
                    && ( anAccount.TransferDate.Hour != 0 || anAccount.TransferDate.Minute != 0 )
                    && anAccount.TransferDate < anAccount.AdmitDate )
                )
            {
                if( this.FireEvents && TransferDateTimeBeforeAdmitDateTimeEvent != null )
                {
                    this.TransferDateTimeBeforeAdmitDateTimeEvent(this, new PropertyChangedArgs( this.AssociatedControl ));
                }
            
                return false;
            }
            else
            {
                return true;
            }           
        }

        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public TransferDateTimeBeforeAdmitDateTime()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
