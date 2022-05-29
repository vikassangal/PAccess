using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for NPPSignatureStatusRequired.
	/// </summary>
    [Serializable]
    [UsedImplicitly]
    public class NPPSignatureStatusRequired : LeafRule
	{
        #region Event Handlers
        public event EventHandler NPPSignatureStatusRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.NPPSignatureStatusRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.NPPSignatureStatusRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.NPPSignatureStatusRequiredEvent = null;   
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

                    if( nppDocument != null
                        && ( nppDocument.NPPVersion != null 
                             && nppDocument.NPPVersion.Code != null 
                             && nppDocument.NPPVersion.Code.Trim() != String.Empty )
                        && ( nppDocument.SignatureStatus == null 
                             || nppDocument.SignatureStatus.Code == null 
                             || nppDocument.SignatureStatus.IsEmptyStatus() ) )
                    {
                        if( this.FireEvents && NPPSignatureStatusRequiredEvent != null )
                        {
                            this.NPPSignatureStatusRequiredEvent( this, null );
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
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NPPSignatureStatusRequired()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}