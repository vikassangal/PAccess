using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for COSSignedPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class COSSignedPreferred : LeafRule
    {
        public event EventHandler COSSignedPreferredEvent;

        public COSSignedPreferred()
        {
        }

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.COSSignedPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.COSSignedPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.COSSignedPreferredEvent = null;            
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
                if( Model.HospitalService.Code =="SP" ||
                    Model.HospitalService.Code =="LB" ||
                    Model.HospitalService.Code =="AB" ||
                    Model.KindOfVisit.Code  == VisitType.NON_PATIENT )
                {
                    return true;
                }

                else
                    if( Model.COSSigned == null 
                    || Model.COSSigned.Code == null
                    || Model.COSSigned.Code.Trim() == string.Empty)
                {
                    if( this.FireEvents && COSSignedPreferredEvent != null )
                    {
                        this.COSSignedPreferredEvent(this, null);
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
