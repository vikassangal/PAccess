using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for COSSignedRequired.
	/// </summary>
	[Serializable]
	[UsedImplicitly]
    public class COSSignedRequired : LeafRule
	{
        public event EventHandler COSSignedRequiredEvent;

		public COSSignedRequired()
		{
		}

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.COSSignedRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.COSSignedRequiredEvent -= eventHandler;
            return true;
        }
               
        public override void UnregisterHandlers()
        {
            this.COSSignedRequiredEvent = null;            
        }

        public override bool CanBeAppliedTo(object context)
        {
            
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            
            Account Model = ((Account)context);

            if( Model.Activity is PreMSERegisterActivity )
            {
                return true;
            }
                
            if( Model.KindOfVisit != null &&
                Model.HospitalService != null &&
                Model.HospitalService.Code != null &&
                Model.KindOfVisit.Code != null)
            {
                if( Model.COSSigned == null 
                    || Model.COSSigned.Code == null
                    || Model.COSSigned.Code.Trim() == string.Empty)
                {
                    if( this.FireEvents && COSSignedRequiredEvent != null )
                    {
                        this.COSSignedRequiredEvent(this, null);
                    }
            
                    return false;
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
            return false;
        }

        

	}
}
