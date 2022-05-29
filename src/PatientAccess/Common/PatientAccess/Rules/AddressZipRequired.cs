using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Rule for address Zip
    /// </summary>
	
    [Serializable]
    [UsedImplicitly]
    public class AddressZipRequired : AddressFieldsRequired
    {
        #region Events
        
        public event EventHandler AddressZipRequiredEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AddressZipRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AddressZipRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AddressZipRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof(Address) )
            {
                return true;
            }

            string zip = String.Empty;
            if ( ( (Address)context ).ZipCode != null )
            {
                zip = ( (Address)context ).ZipCode.ZipCodePrimary;
            }

            if( zip != null )
            {
                zip = zip.Replace("-",string.Empty);
                zip = zip.Trim();
            }

            if( ( string.IsNullOrEmpty( zip )
                || zip == "0")
                && 
                ( ((Address)context).Country != null 
                    && ((Address)context).Country.Code != null
                    && ((Address)context).Country.Code == Country.USA_CODE ))
            {
                if( this.FireEvents && AddressZipRequiredEvent != null )
                {
                    this.AddressZipRequiredEvent(this, null);
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
        public AddressZipRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
