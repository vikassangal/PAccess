using System;
using System.Diagnostics;
using System.Reflection;
using PatientAccess.Annotations;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class ESRDEntitlement :   MedicareEntitlement, IRecommendationCollector, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string Conclusion()
        {
            if( WithinCoordinationPeriod.Code != YES )
            {
                i_medicareResult = MEDICARE;
            }
            
            if( ESRDandAgeOrDisability.Code != YES )
            {
                i_medicareResult = GHP;
            }

            if( BasedOnESRD.Code == YES)
            {
                i_medicareResult = GHP;
            }
            else if( BasedOnAgeOrDisability.Code == YES )
            {
                i_medicareResult = GHP;
            }
            else
            {
                i_medicareResult = MEDICARE;
            }
            return i_medicareResult;
        }

        public void CollectRecommendation( MSPRecommendation recommendation )
        {
            if( recommendation.MSPVersion ==  VERSION_1 )
            {
                if( recommendation.IsInSpecialProgramOrLiability )
                {
                    // UC231 = condition 15 ( Q1 )
                    // Do you have GHP coverage? = No AND MSP does not meet conditions 1-6

                    if( GroupHealthPlanCoverage.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                            "Do you have group health plan (GHP) coverage?", "No" ) );
                    }

                    // UC231 - condition 16 ( Q4 )
                    // Are you within your 30-month coordination period? = No AND MSP does not meet conditions 1-6

                    if( WithinCoordinationPeriod.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                            "Are you within the 30-month coordination period?", "No" ) );
                    }

                    // UC231 - condition 19 ( Q7 )
                    // Does the working aged or disability MSP provision apply? = Yes

                    if( BasedOnAgeOrDisability.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                            "Does the working aged or disability MSP provision apply (i.e., is the GHP " +
                            "primarily based on age or disability entitlement)?", "No" ) );
                    }
                }

                // UC231 - condition 17 ( Q5 )
                // Are you entitled to Medicare on the basis of either ESRD and age or ESRD and disability? = No

                if( ESRDandAgeOrDisability.Code.ToUpper().Equals( "N" ) )
                {
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                        "Are you entitled to Medicare on the basis of either ESRD and age or ESRD " +
                        "disability?", "No" ) );
                }

                // UC231 - Condition 18 ( Q6 )
                // Was your initial entitlement to Medicare based on ESRD? = Yes

                if( BasedOnESRD.Code.ToUpper().Equals( "Y" ) )
                {
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                        "Was your initial entitlement to Medicare (including simultaneous entitlement) " +
                        "based on ESRD?", "Yes" ) );
                }

                // UC231 - condition 19 ( Q7 )
                // Does the working aged or disability MSP provision apply? = No AND MSP does not meet conditions 1-6

                if( BasedOnAgeOrDisability.Code.ToUpper().Equals( "Y" ) )
                {
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                        "Does the working aged or disability MSP provision apply (i.e., is the GHP " +
                        "primarily based on age or disability entitlement)?", "Yes" ) );
                }
            }
            else
            {
                if( recommendation.IsInSpecialProgramOrLiability )
                {
                    // UC231 = condition 15 ( Q1 )
                    // Do you have GHP coverage? = No AND MSP does not meet conditions 1-6

                    if( GroupHealthPlanCoverage.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                            "Do you have group health plan (GHP) coverage?", "No" ) );
                    }

                    // UC231 - condition 16 ( Q4 )
                    // Are you within your 30-month coordination period? = No AND MSP does not meet conditions 1-6

                    if( WithinCoordinationPeriod.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                            "Are you within the 30-month coordination period?", "No" ) );
                    }

                    // UC231 - condition 19 ( Q7 )
                    // Does the working aged or disability MSP provision apply? = Yes

                    if( this.ProvisionAppliesFlag.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                            "Does the working aged or disability MSP provision apply (i.e., is the GHP " +
                            "primarily based on age or disability entitlement)?", "No" ) );
                    }
                }

                // UC231 - condition 17 ( Q5 )
                // Are you entitled to Medicare on the basis of either ESRD and age or ESRD and disability? = No

                if( this.BasedOnAgeOrDisability.Code.ToUpper().Equals( "N" ) )
                {
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed && recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                        "Are you entitled to Medicare on the basis of either ESRD and age or ESRD " +
                        "disability?", "No" ) );
                }

                // UC231 - Condition 18 ( Q6 )
                // Was your initial entitlement to Medicare based on ESRD? = Yes

                if( BasedOnESRD.Code.ToUpper().Equals( "Y" ) )
                {
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed && recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                        "Was your initial entitlement to Medicare (including simultaneous entitlement) " +
                        "based on ESRD?", "Yes" ) );
                }

                // UC231 - condition 19 ( Q7 )
                // Does the working aged or disability MSP provision apply? = No AND MSP does not meet conditions 1-6

                if( this.ProvisionAppliesFlag.Code.ToUpper().Equals( "Y" ) )
                {
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed && recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by ESRD",
                        "Does the working aged or disability MSP provision apply (i.e., is the GHP " +
                        "primarily based on age or disability entitlement)?", "Yes" ) );
                }
            }
        }

        public object Clone()
        {
            ESRDEntitlement newObject = new ESRDEntitlement();

            SetYesNoState( newObject.BasedOnESRD, this.BasedOnESRD );
            SetYesNoState( newObject.BasedOnAgeOrDisability, this.BasedOnAgeOrDisability );
            SetYesNoState( newObject.DialysisTreatment, this.DialysisTreatment );
            SetYesNoState( newObject.ESRDandAgeOrDisability, this.ESRDandAgeOrDisability );
            SetYesNoState( newObject.GroupHealthPlanCoverage, this.GroupHealthPlanCoverage );
            SetYesNoState( newObject.KidneyTransplant, this.KidneyTransplant );
            SetYesNoState( newObject.WithinCoordinationPeriod, this.WithinCoordinationPeriod );

            newObject.TransplantDate            = this.TransplantDate;
            newObject.DialysisDate              = this.DialysisDate;
            newObject.DialysisTrainingStartDate = this.DialysisTrainingStartDate;
            newObject.DialysisCenterName = this.DialysisCenterName;

            if( this.MedicareResult != null )
            {
                newObject.MedicareResult = (string) this.MedicareResult.Clone();
            }
            if( this.PatientEmployment != null )
            {
                newObject.PatientEmployment = (Employment) this.PatientEmployment.Clone();
            }
            else
            {
                newObject.PatientEmployment = null;
            }
            if( this.SpouseEmployment != null )
            {
                newObject.SpouseEmployment = (Employment) this.SpouseEmployment.Clone();
            }
            else
            {
                newObject.SpouseEmployment = null;
            }
            return newObject;
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            ESRDEntitlement esrd = obj as ESRDEntitlement;

            bool medResult = (esrd.MedicareResult == null && this.MedicareResult == null ? true : 
                esrd.MedicareResult == null && this.MedicareResult != null ? false :
                esrd.MedicareResult != null && this.MedicareResult == null ? false :
                esrd.MedicareResult.Equals( this.MedicareResult ));

            int comparison = DateTime.Compare( this.TransplantDate, esrd.TransplantDate );
            bool transplantDateComparison = (comparison == 0);

            comparison = DateTime.Compare( this.DialysisDate, esrd.DialysisDate );
            bool dialysisDateComparison = (comparison == 0);

            comparison = DateTime.Compare( this.DialysisTrainingStartDate, esrd.DialysisTrainingStartDate );
            bool dialysisTrainingStartDateComparison = (comparison == 0);
            comparison = String.Compare(this.DialysisCenterName, esrd.DialysisCenterName);
            bool dialysisCenterNameComparison = (comparison == 0);

            bool result =
                (medResult && transplantDateComparison &&
                dialysisDateComparison && dialysisTrainingStartDateComparison && dialysisCenterNameComparison &&
                (this.KidneyTransplant.Equals( esrd.KidneyTransplant )) &&
                (this.DialysisTreatment.Equals( esrd.DialysisTreatment )) &&
                (this.WithinCoordinationPeriod.Equals( esrd.WithinCoordinationPeriod )) &&
                (this.ESRDandAgeOrDisability.Equals( esrd.ESRDandAgeOrDisability )) &&
                (this.BasedOnESRD.Equals( esrd.BasedOnESRD )) &&
                (this.BasedOnAgeOrDisability.Equals( esrd.BasedOnAgeOrDisability )) &&
                base.Equals( obj ));

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties

        public YesNoFlag InitialEntitlementWasESRDFlag
        {
            get
            {
                return this.i_InitialEntitlementWasESRDFlag;
            }
            set
            {
                this.i_InitialEntitlementWasESRDFlag = value;
            }
        }

        public YesNoFlag ProvisionAppliesFlag 
        {
            get
            {
                return this.i_ProvisionAppliesFlag;
            }
            set
            {
                this.i_ProvisionAppliesFlag = value;
            }
        }

        public YesNoFlag KidneyTransplant
        {
            get
            {
                return this.i_KidneyTransplant;
            }
            set
            {
                this.i_KidneyTransplant = value;
            }
        }

        public DateTime TransplantDate
        {
            get
            {
                return this.i_TransplantDate;
            }
            set
            {
                this.i_TransplantDate = value;
            }
        }

        public YesNoFlag DialysisTreatment
        {
            get
            {
                return this.i_DialysisTreatment;
            }
            set
            {
                Debug.Assert(value != null);
                this.SetAndTrack<YesNoFlag>(ref this.i_DialysisTreatment, value, MethodBase.GetCurrentMethod());
            }
        }

        public DateTime DialysisDate
        {
            get
            {
                return this.i_DialysisDate;
            }
            set
            {
                Debug.Assert(value != null);
                this.SetAndTrack<DateTime>(ref this.i_DialysisDate, value, MethodBase.GetCurrentMethod());
                 
            }
        }

        public DateTime DialysisTrainingStartDate
        {
            get
            {
                return this.i_DialysisTrainingStartDate;
            }
            set
            {
                this.i_DialysisTrainingStartDate = value;
            }
        }

        public YesNoFlag WithinCoordinationPeriod
        {
            get
            {
                return this.i_WithinCoordinationPeriod;
            }
            set
            {
                this.i_WithinCoordinationPeriod = value;
            }
        }

        public YesNoFlag ESRDandAgeOrDisability
        {
            get
            {
                return this.i_ESRDandAgeOrDisability;
            }
            set
            {
                this.i_ESRDandAgeOrDisability = value;
            }
        }

        public YesNoFlag BasedOnESRD
        {
            get
            {
                return this.i_BasedOnESRD;
            }
            set
            {
                this.i_BasedOnESRD = value;
            }
        }

        public YesNoFlag BasedOnAgeOrDisability
        {
            get
            {
                return this.i_BasedOnAgeOrDisability;
            }
            set
            {
                this.i_BasedOnAgeOrDisability = value;
            }
        }
        
        public string DialysisCenterName
        {
            get
            {
                return this.i_DialysisCenterName;
            }
            set
            {
                Debug.Assert(value != null);
                this.SetAndTrack<string>(ref this.i_DialysisCenterName, value, MethodBase.GetCurrentMethod());

            }
        }
        #endregion

        #region Methods
        #endregion

        #region Private Properties

        private string MedicareResult
        {
            get
            {
                return i_medicareResult;
            }
            set
            {
                i_medicareResult = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public ESRDEntitlement()
        {
        }
        #endregion

        #region Data Elements

        private DateTime                    i_TransplantDate;
        private DateTime                    i_DialysisDate;
        private DateTime                    i_DialysisTrainingStartDate;

        private YesNoFlag                   i_WithinCoordinationPeriod            = new YesNoFlag();
        private YesNoFlag                   i_ESRDandAgeOrDisability              = new YesNoFlag();
        private YesNoFlag                   i_BasedOnESRD                         = new YesNoFlag();
        private YesNoFlag                   i_BasedOnAgeOrDisability              = new YesNoFlag();
        private YesNoFlag                   i_KidneyTransplant                    = new YesNoFlag();        
        private YesNoFlag                   i_DialysisTreatment                   = new YesNoFlag();
        private YesNoFlag                   i_InitialEntitlementWasESRDFlag       = new YesNoFlag();
        private YesNoFlag                   i_ProvisionAppliesFlag                = new YesNoFlag();
        private string                      i_DialysisCenterName                  = string.Empty;

        #endregion

        #region Constants

        private string                          i_medicareResult;

        private const string                    GHP      = "GHP is primary";
        private const string                    MEDICARE = "Medicare is primary";
        private const string                    YES      = "Y";

        private const int                       VERSION_1   = 1;
        private const int                       VERSION_2   = 2;

        #endregion
    }
}