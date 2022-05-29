using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// RARRAFUSNoteFormatter - RARRA Authorization Requirements reviewed for account 
	/// </summary>
	//TODO: Create XML summary comment for RARRAFUSNoteFormatter
    [Serializable]
    [UsedImplicitly]
    public class RARRAFUSNoteFormatter : FusFormatterStrategy
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
            string formattedString = String.Empty;
            ArrayList nameValueList = new ArrayList();

            string covType = cov.GetType().ToString();

            if(  covType == SELF_PAY_COVERAGE )
            {
                nameValueList = CreateFusNoteForSelfPayAndMedicare( cov );
            }
            if ( covType == MEDICARE_COVERAGE)
            {
                var medicareCoverage = cov as GovernmentMedicareCoverage;
                if (medicareCoverage != null)
                {
                    if( !medicareCoverage.IsMedicareCoverageValidForAuthorization)
                    {
                        nameValueList = CreateFusNoteForSelfPayAndMedicare(cov);
                    }
                    else
                    {
                        nameValueList = CreateFUSNoteForMedicare53544(cov, origCov);
                    }
                }
            }
            
            if( cov.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
            {
                CoverageGroup coverageGroup = cov as CoverageGroup;
                Authorization covAuth = coverageGroup.Authorization;

                CoverageGroup origCoverageGroup = origCov as CoverageGroup;
                Authorization origCovAuth = origCoverageGroup.Authorization;
                if( IsModified( cov.InsurancePlan, origCov.InsurancePlan )
                    && ( cov.InsurancePlan != null &&
                         cov.InsurancePlan.Payor != null ) )
                {
                    formattedString = FormatNameValuePair( FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name );
                    nameValueList.Add( formattedString );
                }   
                if (IsModified(covAuth.AuthorizationCompany, origCovAuth.AuthorizationCompany))
                {
                    formattedString = FormatNameValuePair(  FusLabel.AUTH_COMPANY_NAME, 
                                                            covAuth.AuthorizationCompany);
                    nameValueList.Add(formattedString);
                }
                if ( IsModified( covAuth.AuthorizationNumber, origCovAuth.AuthorizationNumber ) )
                {
                    formattedString = FormatNameValuePair( FusLabel.AUTHORIZATION_NUMBER,
                                                            covAuth.AuthorizationNumber ) ;
                    nameValueList.Add( formattedString ) ;
                }
                // TrackingNumber is only contained in type CoverageForCommercialOther
                if (cov is CoverageForCommercialOther)
                {
                    CoverageGroup commercialOtherCoverageGroup = cov as CoverageForCommercialOther;
                    CoverageGroup origCommercialOtherCoverageGroup = origCov as CoverageForCommercialOther;
                    if (null != origCommercialOtherCoverageGroup)
                    {
                        if (((CoverageForCommercialOther)commercialOtherCoverageGroup).HasChangedFor("TrackingNumber"))
                        {
                            formattedString = FormatNameValuePair(FusLabel.TRACKING_NUMBER,
                                    ((CoverageForCommercialOther)commercialOtherCoverageGroup).TrackingNumber);
                            nameValueList.Add(formattedString);
                        }
                    }
                }
                if (cov is GovernmentMedicaidCoverage)
                {
                    var governmentMedicaidCoverage = cov as GovernmentMedicaidCoverage;
                    if (null != governmentMedicaidCoverage)
                    {
                        if (((GovernmentMedicaidCoverage)governmentMedicaidCoverage).HasChangedFor("TrackingNumber"))
                        {
                            formattedString = FormatNameValuePair(FusLabel.TRACKING_NUMBER,
                                ((GovernmentMedicaidCoverage)governmentMedicaidCoverage).TrackingNumber);
                            nameValueList.Add(formattedString);
                        }
                    }
                }
                if (IsModified(covAuth.NumberOfDaysAuthorized, origCovAuth.NumberOfDaysAuthorized))
                {
                    formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_NUMBER_OF_DAYS_AUTHORIZED,
                                                            covAuth.NumberOfDaysAuthorized.ToString());
                    nameValueList.Add(formattedString);
                }

                if (IsModified( covAuth.NameOfCompanyRepresentative.AsFormattedName(), 
                                origCovAuth.NameOfCompanyRepresentative.AsFormattedName()))
                {
                    formattedString = FormatNameValuePair( FusLabel.AUTH_CO_REP_NAME, 
                                                    covAuth.NameOfCompanyRepresentative.AsFormattedName());
                    nameValueList.Add(formattedString);
                }
                if (IsModified(covAuth.ServicesAuthorized, origCovAuth.ServicesAuthorized))
                {
                    formattedString = FormatNameValuePair(  FusLabel.SERVICES_AUTHORIZED, 
                                                            covAuth.ServicesAuthorized);
                    nameValueList.Add(formattedString);
                }
                if (IsModified(covAuth.EffectiveDate, origCovAuth.EffectiveDate))
                {
                    formattedString = FormatNameValuePair(  FusLabel.AUTHORIZATION_EFFECTIVE_DATE, 
                                                            covAuth.EffectiveDate.ToShortDateString());
                    nameValueList.Add(formattedString);
                }
                if (IsModified(covAuth.ExpirationDate, origCovAuth.ExpirationDate))
                {
                    formattedString = FormatNameValuePair(  FusLabel.AUTHORIZATION_EXPIRATION_DATE,
                                                            covAuth.ExpirationDate.ToShortDateString());
                    nameValueList.Add(formattedString);
                }
                if (IsModified(covAuth.AuthorizationStatus, origCovAuth.AuthorizationStatus))
                {
                    formattedString = FormatNameValuePair(  FusLabel.AUTHORIZATION_STATUS, 
                                                            covAuth.AuthorizationStatus.ToString());
                    nameValueList.Add(formattedString);
                }
                if (IsModified(covAuth.Remarks, origCovAuth.Remarks))
                {
                    formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_REMARKS, covAuth.Remarks);
                    nameValueList.Add(formattedString);
                }                             
                if (IsModified(covAuth.AuthorizationRequired, origCovAuth.AuthorizationRequired)
                    && covAuth.AuthorizationRequired != null )
                {
                    formattedString = FormatNameValuePair( FusLabel.IS_AUTH_REQUIRED,
                        covAuth.AuthorizationRequired.Code );
                    nameValueList.Add( formattedString );
                }
                if( IsModified( covAuth.AuthorizationPhone.ToString(), origCovAuth.AuthorizationPhone.ToString() ) )
                {
                    formattedString = FormatNameValuePair( FusLabel.PHONE_NUMBER,
                        covAuth.AuthorizationPhone.ToString() );
                    nameValueList.Add( formattedString );
                }
                if( IsModified( covAuth.PromptExt, origCovAuth.PromptExt ) )
                {
                    formattedString = FormatNameValuePair( FusLabel.PHONE_EXTENSION, covAuth.PromptExt );
                    nameValueList.Add( formattedString );
                }                
            }

            return nameValueList;
        }
        /// <summary>
        /// Create RARR fus note for Coverage Type Self Pay & Medicare.
        /// </summary>
        /// <param name="cov"></param>
        /// <returns></returns>
        private ArrayList CreateFusNoteForSelfPayAndMedicare(Coverage cov)
        {
            ArrayList nameValueList = new ArrayList();
            string formattedString = String.Empty;
            formattedString = FormatNameValuePair(FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name);
            nameValueList.Add(formattedString);
            formattedString = FormatNameValuePair(FusLabel.IS_AUTH_REQUIRED,
                           YesNotApplicableFlag.CODE_NOTAPPLICABLE);
            nameValueList.Add(formattedString);
            return nameValueList;
        }

        private ArrayList CreateFUSNoteForMedicare53544(Coverage cov, Coverage origCov)
        {
            var formattedString = String.Empty;
            var nameValueList = new ArrayList();


            var medicareCoverageGroup = cov as GovernmentMedicareCoverage;
            var covAuth = medicareCoverageGroup.Authorization;

            var origmedicareCoverageGroup = origCov as GovernmentMedicareCoverage;
            var origCovAuth = origmedicareCoverageGroup.Authorization;
            if (IsModified(cov.InsurancePlan, origCov.InsurancePlan)
                && (cov.InsurancePlan != null &&
                    cov.InsurancePlan.Payor != null))
            {
                formattedString = FormatNameValuePair(FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name);
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.AuthorizationCompany, origCovAuth.AuthorizationCompany))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTH_COMPANY_NAME,
                    covAuth.AuthorizationCompany);
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.AuthorizationNumber, origCovAuth.AuthorizationNumber))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_NUMBER,
                    covAuth.AuthorizationNumber);
                nameValueList.Add(formattedString);
            }

            if (medicareCoverageGroup.HasChangedFor("TrackingNumber"))
            {
                formattedString = FormatNameValuePair(FusLabel.TRACKING_NUMBER,
                    (medicareCoverageGroup).TrackingNumber);
                nameValueList.Add(formattedString);
            }


            if (IsModified(covAuth.NumberOfDaysAuthorized, origCovAuth.NumberOfDaysAuthorized))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_NUMBER_OF_DAYS_AUTHORIZED,
                    covAuth.NumberOfDaysAuthorized.ToString());
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.NameOfCompanyRepresentative.AsFormattedName(),
                origCovAuth.NameOfCompanyRepresentative.AsFormattedName()))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTH_CO_REP_NAME,
                    covAuth.NameOfCompanyRepresentative.AsFormattedName());
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.ServicesAuthorized, origCovAuth.ServicesAuthorized))
            {
                formattedString = FormatNameValuePair(FusLabel.SERVICES_AUTHORIZED,
                    covAuth.ServicesAuthorized);
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.EffectiveDate, origCovAuth.EffectiveDate))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_EFFECTIVE_DATE,
                    covAuth.EffectiveDate.ToShortDateString());
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.ExpirationDate, origCovAuth.ExpirationDate))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_EXPIRATION_DATE,
                    covAuth.ExpirationDate.ToShortDateString());
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.AuthorizationStatus, origCovAuth.AuthorizationStatus))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_STATUS,
                    covAuth.AuthorizationStatus.ToString());
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.Remarks, origCovAuth.Remarks))
            {
                formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_REMARKS, covAuth.Remarks);
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.AuthorizationRequired, origCovAuth.AuthorizationRequired)
                && covAuth.AuthorizationRequired != null)
            {
                formattedString = FormatNameValuePair(FusLabel.IS_AUTH_REQUIRED,
                    covAuth.AuthorizationRequired.Code);
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.AuthorizationPhone.ToString(), origCovAuth.AuthorizationPhone.ToString()))
            {
                formattedString = FormatNameValuePair(FusLabel.PHONE_NUMBER,
                    covAuth.AuthorizationPhone.ToString());
                nameValueList.Add(formattedString);
            }

            if (IsModified(covAuth.PromptExt, origCovAuth.PromptExt))
            {
                formattedString = FormatNameValuePair(FusLabel.PHONE_EXTENSION, covAuth.PromptExt);
                nameValueList.Add(formattedString);
            }

            return nameValueList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RARRAFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
