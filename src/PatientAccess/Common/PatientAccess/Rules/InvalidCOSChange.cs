using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidCOSChange Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidCOSChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidCOSChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            if( this.InvalidCOSChangeEvent != null )
            {
                Delegate[] delegates = this.InvalidCOSChangeEvent.GetInvocationList();

                foreach( Delegate d in delegates )
                {
                    if( d.Target.GetType() == eventHandler.Target.GetType()
                        && d.Method.Name == eventHandler.Method.Name)
                    {
                        return true;
                    }
                }
            }
            this.InvalidCOSChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidCOSChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidCOSChangeEvent =null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;
                                    
            if( account.DischargeDate != DateTime.MinValue )
            {
                return true;
            }

            if( ( account != null ) &&  (account.COSSigned != null ))
            {
                if(!(account.COSSigned.IsValid))                          
                {
                    if( this.FireEvents && InvalidCOSChangeEvent != null )
                    {
                        this.InvalidCOSChangeEvent(this, null);
                    }
            
                    return false ;
                }
                else
                {
                    return true;
                }           
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

        public InvalidCOSChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


