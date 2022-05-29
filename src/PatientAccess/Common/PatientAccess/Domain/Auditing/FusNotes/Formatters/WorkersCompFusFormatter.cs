using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    //TODO: Create XML summary comment for WorkersCompFusFormatter
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompFusFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            FusNote note = Context;
            string code = note.FusActivity.Code;

            WorkersCompensationCoverage wCov = note.Context as WorkersCompensationCoverage;
            WorkersCompensationCoverage origCov = note.Context2 as WorkersCompensationCoverage;

            CheckOriginalCoverage( wCov, origCov );

            ArrayList messages = CreateFusNameValueList( wCov, code );

            return messages;

        }
        #endregion

        #region Properties

        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( WorkersCompensationCoverage cov, string code )
        {
            ArrayList nameValueList = new ArrayList();
            writeNote = false;

            if( code == RDOTVActivityCode )
            {
                if( cov.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                {
                    nameValueList.Add( FusLabel.PRIMARY_PAYOR_VERIFICATION );
                }
                else if( cov.CoverageOrder.Oid == CoverageOrder.SECONDARY_OID )
                {
                    nameValueList.Add( FusLabel.SECONDARY_PAYOR_VERIFICATION );
                }

                if( cov.InsurancePlan != null )
                {
                    if( cov.InsurancePlan.Payor != null )
                    {
                        nameValueList.Add( FormatNameValuePair( FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name ) );
                    }
                }
            }
            else if( cov.InsurancePlan != null )
            {
                if( cov.InsurancePlan.Payor != null )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name ) );
                }
            }

            if( cov.HasChangedFor( "InformationReceivedSource" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INFO_RECEIVED_FROM, cov.InformationReceivedSource.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "PPOPricingOrBroker" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PPO_NETWORK_BROKER, cov.PPOPricingOrBroker ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "ClaimNumberForIncident" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INCIDENT_CLAIM_NUMBER, cov.ClaimNumberForIncident ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "ClaimsAddressVerified" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.IS_ADDRESS_VERIFIED, cov.ClaimsAddressVerified.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "InsurancePhone" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INSURANCE_PHONE_NUMBER, cov.InsurancePhone ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "EmployerhasPaidPremiumsToDate" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.HAS_EMP_PAID_PREMIUMS, cov.EmployerhasPaidPremiumsToDate.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "Remarks" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.LABEL_REMARKS, cov.Remarks ) );
                writeNote = true;
            }

            if( cov.Attorney.HasChangedFor( "AttorneyName" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ATTORNEYNAME, cov.Attorney.AttorneyName ) );
                writeNote = true;
            }

            ContactPoint attorneyInfo =
                cov.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() ) ?? new ContactPoint();

            if( attorneyInfo.PhoneNumber.HasChangedFor( "AreaCode" ) || attorneyInfo.PhoneNumber.HasChangedFor( "Number" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ATTORNEYPHONE, attorneyInfo.PhoneNumber.AsFormattedString() ) );
                writeNote = true;
            }

            if( attorneyInfo.HasChangedFor( "Address" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ATTORNEYADDRESS, attorneyInfo.Address.OneLineAddressLabel() ) );
                writeNote = true;
            }

            if( writeNote )
            {
                return nameValueList;
            }

            return new ArrayList();
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        private bool writeNote;
        #endregion

        #region Constants
        #endregion
    }
}
