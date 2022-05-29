using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RBVCAFusNoteFormatter.
	/// </summary>
	//TODO: Create XML summary comment for RBVCAFusNoteFormatter
    [Serializable]
    [UsedImplicitly]
    public class RBVCAFusNoteFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList  Format()
        {
            ArrayList messages = new ArrayList();
            string msg = string.Empty ;
            FusNote note  = this.Context  as FusNote ;
            string code = note.FusActivity.Code ;

            Coverage cCov = ( Coverage )note.Context;
            Coverage origCov = ( Coverage )note.Context2;

            base.CheckOriginalCoverage( cCov, origCov );

            messages = this.CreateFusNameValueList( cCov, origCov );
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( Coverage cov, Coverage origCov )
        {            

            ArrayList nameValueList = new ArrayList();

            if( cov.BenefitsVerified == null
                || cov.BenefitsVerified.IsBlank )
            {
                return nameValueList;
            }

            if( cov.HasChangedFor( "BenefitsVerified" ) )
            {
                nameValueList.Add( 
                    FormatNameValuePair( FusLabel.IS_VERIFIED, 
                                         cov.BenefitsVerified.Code ) );
            }
            if( cov.HasChangedFor( "AuthorizingPerson" ) )
            {
                nameValueList.Add(
                    FormatNameValuePair( FusLabel.INITIALED_BY,
                                         cov.AuthorizingPerson ) );
            }
            if( cov.HasChangedFor( "DateTimeOfVerification" ) )
            {
                if( cov.DateTimeOfVerification != DateTime.MinValue )
                {
                    nameValueList.Add(
                        FormatNameValuePair( FusLabel.BENEFITS_VERIFIED_DATE, 
                                             cov.DateTimeOfVerification.ToString("MM/dd/yyyy") ) );
                }
                else
                {
                    nameValueList.Add(
                        FormatNameValuePair( FusLabel.BENEFITS_VERIFIED_DATE, String.Empty ) );
                }

            }

            return nameValueList;

        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RBVCAFusNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
