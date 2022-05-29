using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitSourceRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidNPPVersionChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidNPPVersionChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            if( InvalidNPPVersionChangeEvent != null )
            {
                Delegate[] delegates = this.InvalidNPPVersionChangeEvent.GetInvocationList();

                foreach( Delegate d in delegates )
                {
                    if( d.Target.GetType() == eventHandler.Target.GetType()
                        && d.Method.Name == eventHandler.Method.Name)
                    {
                        return true;
                    }
                }
            }

            this.InvalidNPPVersionChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidNPPVersionChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidNPPVersionChangeEvent = null;    
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

            if( ( account != null ) &&  
                ( account.Patient.NoticeOfPrivacyPracticeDocument != null ) &&
                ( account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion  != null ))
            {
                if(!(account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion.IsValid))
                          
                {
                    if( this.FireEvents && InvalidNPPVersionChangeEvent != null )
                    {
                        this.InvalidNPPVersionChangeEvent(this, null);
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

        public InvalidNPPVersionChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


