using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class NPPVersionPreferred : LeafRule
    {
        public event EventHandler NPPVersionPreferredEvent;

        public NPPVersionPreferred()
        {
        }

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            NPPVersionPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            NPPVersionPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            NPPVersionPreferredEvent = null;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }

            Account Model = ( (Account)context );
            if ( Model.KindOfVisit != null &&
                Model.HospitalService != null &&
                Model.HospitalService.Code != null &&
                Model.KindOfVisit.Code != null )
            {
                if ( Model.HospitalService.Code == "SP" ||
                    Model.HospitalService.Code == "LB" ||
                    Model.HospitalService.Code == "AB" ||
                    Model.KindOfVisit.Code == VisitType.NON_PATIENT )
                {
                    return true;
                }

                else
                {
                    NoticeOfPrivacyPracticeDocument nppDocument = Model.Patient.NoticeOfPrivacyPracticeDocument;

                    if ( nppDocument != null &&
                        ( nppDocument.NPPVersion == null
                          || nppDocument.NPPVersion.Code == null
                          || nppDocument.NPPVersion.Code.Trim() == String.Empty ) )
                    {
                        if ( FireEvents && NPPVersionPreferredEvent != null )
                        {
                            NPPVersionPreferredEvent( this, null );
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

        public override void ApplyTo( object context )
        {

        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }
    }
}
