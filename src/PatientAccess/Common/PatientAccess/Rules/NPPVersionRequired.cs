using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for NPPVersionRequired.
	/// </summary>
	[Serializable]
    [UsedImplicitly]
    public class NPPVersionRequired : LeafRule
	{
        public event EventHandler NPPVersionRequiredEvent;

		public NPPVersionRequired()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.NPPVersionRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.NPPVersionRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.NPPVersionRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {   
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }             
            
            Account Model = ((Account)context);
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
                {
                    NoticeOfPrivacyPracticeDocument nppDocument = Model.Patient.NoticeOfPrivacyPracticeDocument;
           
                    if( nppDocument != null &&
                        ( nppDocument.NPPVersion == null 
                          || nppDocument.NPPVersion.Code == null 
                          || nppDocument.NPPVersion.Code.Trim() == String.Empty ) )
                    {
                        if( this.FireEvents && NPPVersionRequiredEvent != null )
                        {
                            this.NPPVersionRequiredEvent( this, null );
                        }

                        return false;
                    }
               
                    else
                    {
                         return true;
                    }
                
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


	}
}
