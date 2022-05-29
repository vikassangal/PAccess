using System;
using System.Collections;
using Extensions.UI.Builder;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitSourceRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidFacilityDeterminedFlagChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidFacilityDeterminedFlagChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            if( InvalidFacilityDeterminedFlagChangeEvent != null )
            {
                Delegate[] delegates = this.InvalidFacilityDeterminedFlagChangeEvent.GetInvocationList();

                foreach( Delegate d in delegates )
                {
                    if( d.Target.GetType() == eventHandler.Target.GetType()
                        && d.Method.Name == eventHandler.Method.Name)
                    {
                        return true;
                    }
                }
            }
            this.InvalidFacilityDeterminedFlagChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidFacilityDeterminedFlagChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidFacilityDeterminedFlagChangeEvent = null;   
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

            if( ( account != null ) &&  (account.FacilityDeterminedFlag != null ))
            {
//                IFacilityFlagBroker broker = BrokerFactory.BrokerOfType<IFacilityFlagBroker>();
//                ICollection flags = broker.FacilityFlagsFor(account.Facility.Oid);

                FacilityFlagBrokerProxy broker = new FacilityFlagBrokerProxy();
                ICollection flags = broker.FacilityFlagsFor( account.Facility.Oid );

                if(flags.Count > 1)
                {
                    if(!(account.FacilityDeterminedFlag.IsValid))
                          
                    {
                        if( this.FireEvents && InvalidFacilityDeterminedFlagChangeEvent != null )
                        {
                            this.InvalidFacilityDeterminedFlagChangeEvent(this, null);
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

        public InvalidFacilityDeterminedFlagChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


