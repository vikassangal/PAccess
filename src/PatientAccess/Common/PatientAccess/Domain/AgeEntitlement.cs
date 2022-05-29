using System;
using PatientAccess.Annotations;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class AgeEntitlement : MedicareEntitlement, IRecommendationCollector, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string Conclusion()
        {
            return i_medicareResult;
        }

        public void CollectRecommendation( MSPRecommendation recommendation )
        {
            if( recommendation.MSPVersion ==  VERSION_1 )
            {
                if( recommendation.IsInSpecialProgramOrLiability )// Does not meet all of questions 1 - 6
                {
                    // UC231 - Condition 7
                    // Are you currently employed? = No-Retired or No–Never Employed AND Is your spouse 
                    // currently employed? = No-Retired or No–Never Employed AND MSP does not meet conditions 1-6 

                    if( PatientEmployment != null &&
                        (PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) ||
                        PatientEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) ) &&
                        SpouseEmployment != null &&
                        (SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) ||
                        SpouseEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        )
                    {
                        recommendation.IsMedicareRecommended = true;

                        if( PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Are you currently employed?", "No - Retired" ) );
                        }
                        else if( PatientEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Are you currently employed?", "No - Never employed" ) );
                        }
                        if( SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Is your spouse currently employed?", "No - (Spouse) Retired" ) );
                        }
                        else if( SpouseEmployment != null &&
                            SpouseEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Is your spouse currently employed?", "No - (Spouse) Never employed" ) );
                        }
                    }

                    // UC231 - Condition 8
                    // Do you have GHP coverage based on your own, or a spouse’s current employment? = No
                    // AND MSP does not meet conditions 1-6

                    if( GroupHealthPlanCoverage.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                            "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                            "currrent employment?", "No" ) );
                    }

                    // UC231 - Condition 9
                    // Does the employer that sponsors your GHP employ 20 or more employees? = No
                    // AND MSP does not meet conditions 1-6

                    if( GHPLimitExceeded.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                            "Does the employer that sponsors your GHP employ 20 or more employees?",
                            "No" ) );
                    }
                }

                // UC230 - Condition 10
                // Does the employer that sponsors your GHP employ 20 or more employees? = Yes

                if( GHPLimitExceeded.Code.ToUpper().Equals( "Y" ) )
                {
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                        "Does the employer that sponsors your GHP employ 20 or more employees?",
                        "Yes" ) );
                }

                if( GroupHealthPlanCoverage.Code == YES && GHPLimitExceeded.Code == YES )
                {
                    i_medicareResult = GHP;
                }
                else
                {
                    i_medicareResult = MEDICARE;
                }
            }
                // MSP Version 2

            else if( recommendation.MSPVersion == VERSION_2 )
            {
                if( recommendation.IsInSpecialProgramOrLiability )// Does not meet all of questions 1 - 6
                {
                    // Condition 7

                    if( PatientEmployment != null &&
                        (PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) ||
                        PatientEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) ) &&
                        SpouseEmployment != null &&
                        (SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) ||
                        SpouseEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        )
                    {
                        recommendation.IsMedicareRecommended = true;

                        if( PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Are you currently employed?", "No - Retired" ) );
                        }
                        else if( PatientEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Are you currently employed?", "No - Never employed" ) );
                        }
                        if( SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Is your spouse currently employed?", "No - (Spouse) Retired" ) );
                        }
                        else if( SpouseEmployment != null &&
                            SpouseEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                "Is your spouse currently employed?", "No - (Spouse) Never employed" ) );
                        }

                        i_medicareResult = MEDICARE;
                    }

                    // Condition 8      

                    if( GroupHealthPlanCoverage.Code == YesNoFlag.CODE_NO )
                    {
                        recommendation.IsMedicareRecommended = true;

                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                            "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                            "currrent employment?", "No" ) );

                        i_medicareResult = MEDICARE;
                    }
                    else
                    {
                        // Condition 9 (modified for MSP2)                

                        if( this.GroupHealthPlanType.Oid == GroupHealthPlanType.SELF_OID )
                        {
                            if( this.GHPEmploysX.Code == YesNoFlag.CODE_NO )
                            {
                                recommendation.IsMedicareRecommended = true;

                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                                    "currrent employment?", "Yes - Self" ) );
                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "If you have GHP coverage based on your own current employment, does the employer "
                                    + "that sponsors or contributes to the GHP employ 20 or more employees? ",
                                    "No" ) );

                                i_medicareResult = MEDICARE;
                            }
                            else
                            {
                                // Condition 10 (modified for MSP2)

                                recommendation.IsMedicareRecommended = false;

                                if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                                {
                                    recommendation.ContributingConditions.Clear();
                                }

                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                                    "currrent employment?", "Yes - Self" ) );
                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "If you have GHP coverage based on your own current employment, does the employer "
                                    + "that sponsors or contributes to the GHP employ 20 or more employees? ",
                                    "Yes" ) );

                                i_medicareResult = GHP;
                            }                        
                        }
                            // Condition 11 (modified for MSP2)

                        else if( this.GroupHealthPlanType.Oid == GroupHealthPlanType.SPOUSE_OID )
                        {
                            if ( this.GHPSpouseEmploysX.Code == YesNoFlag.CODE_NO )
                            {
                                recommendation.IsMedicareRecommended = true;

                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                                    "currrent employment?", "Yes - Spouse" ) );
                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "If you have GHP coverage based on your spouse’s current employment, does your "
                                    + "spouse’s employer, that sponsors or contributes to the GHP, employ 20 or more "
                                    + "employees?  ",
                                    "No" ) );

                                i_medicareResult = MEDICARE;
                            }
                            else
                            {
                                // Condition 12 (modified for MSP2)

                                recommendation.IsMedicareRecommended = false;

                                if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                                {
                                    recommendation.ContributingConditions.Clear();
                                }

                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                                    "currrent employment?", "Yes - Spouse" ) );
                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "If you have GHP coverage based on your spouse’s current employment, does your "
                                    + "spouse’s employer, that sponsors or contributes to the GHP, employ 20 or more "
                                    + "employees?  ",
                                    "Yes" ) );

                                i_medicareResult = GHP;
                            }
                        }

                        // Condition 13 (modified for MSP2)

                        else if( this.GroupHealthPlanType.Oid == GroupHealthPlanType.BOTH_OID )
                        {
                            if (this.GHPEmploysX.Code == YesNoFlag.CODE_NO 
                                && this.GHPSpouseEmploysX.Code == YesNoFlag.CODE_NO )
                            {
                                recommendation.IsMedicareRecommended = true;

                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                                    "currrent employment?", "Yes - Both" ) );
                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "If you have GHP coverage based on your own current employment, does the employer "
                                    + "that sponsors or contributes to the GHP employ 20 or more employees? ",
                                    "No" ) );
                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "If you have GHP coverage based on your spouse’s current employment, does your "
                                    + "spouse’s employer, that sponsors or contributes to the GHP, employ 20 or more "
                                    + "employees?  ",
                                    "No" ) );

                                i_medicareResult = MEDICARE;
                            }  
                            else
                            {
                                recommendation.IsMedicareRecommended = false;

                                recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                    "Do you have group health group (GHP) coverage on your own, or a spouse's " +
                                    "currrent employment?", "Yes - Both" ) );

                                if(  this.GHPEmploysX.Code == YesNoFlag.CODE_NO )
                                {
                                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                        "If you have GHP coverage based on your own current employment, does the employer "
                                        + "that sponsors or contributes to the GHP employ 20 or more employees? ",
                                        "No" ) );
                                }
                                else
                                {
                                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                        "If you have GHP coverage based on your own current employment, does the employer "
                                        + "that sponsors or contributes to the GHP employ 20 or more employees? ",
                                        "Yes" ) );
                                }
                                
                                if( this.GHPSpouseEmploysX.Code == YesNoFlag.CODE_NO )
                                {
                                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                        "If you have GHP coverage based on your spouse’s current employment, does your "
                                        + "spouse’s employer, that sponsors or contributes to the GHP, employ 20 or more "
                                        + "employees?  ",
                                        "No" ) );
                                }
                                else
                                {
                                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Age",
                                        "If you have GHP coverage based on your spouse’s current employment, does your "
                                        + "spouse’s employer, that sponsors or contributes to the GHP, employ 20 or more "
                                        + "employees?  ",
                                        "Yes" ) );
                                }

                                i_medicareResult = GHP;
                            }
                        }
                    }
                }                
            }            
        }

        public object Clone()
        {
            AgeEntitlement newObject = new AgeEntitlement();

            if( this.MedicareResult != null )
            {
                newObject.MedicareResult = (string) this.MedicareResult.Clone();
            }
            newObject.GroupHealthPlanCoverage = (YesNoFlag) this.GroupHealthPlanCoverage.Clone();
            SetYesNoState( newObject.GHPLimitExceeded, this.GHPLimitExceeded );

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
            AgeEntitlement ae = obj as AgeEntitlement;

            bool medResult = (ae.MedicareResult == null && this.MedicareResult == null ? true : 
                              ae.MedicareResult == null && this.MedicareResult != null ? false :
                              ae.MedicareResult != null && this.MedicareResult == null ? false :
                              ae.MedicareResult.Equals( this.MedicareResult ));

            bool ghp = GHPLimitExceeded.Equals( ae.GHPLimitExceeded );
            bool bass = base.Equals( obj );
            bool result = (medResult && ghp && bass);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties

        public YesNoFlag GHPLimitExceeded 
        {
            get
            {
                return this.i_GHPLimitExceeded;
            }
            set
            {
                this.i_GHPLimitExceeded = value;
            }
        }

        public YesNoFlag GHPEmploysX 
        {
            get
            {
                return this.i_GHPEmploysX;
            }
            set
            {
                this.i_GHPEmploysX = value;
            }
        }

        public YesNoFlag GHPSpouseEmploysX 
        {
            get
            {
                return this.i_GHPSpouseEmploysX;
            }
            set
            {
                this.i_GHPSpouseEmploysX = value;
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
        public AgeEntitlement()
        {
        }
        #endregion

        #region Data Elements

        private string                                          i_medicareResult;
        private YesNoFlag                                       i_GHPEmploysX = new YesNoFlag();
        private YesNoFlag                                       i_GHPSpouseEmploysX = new YesNoFlag();
        private YesNoFlag                                       i_GHPLimitExceeded = new YesNoFlag();

        
        #endregion

        #region Constants

        private const string                    GHP      = "GHP is primary";
        private const string                    MEDICARE = "Medicare is primary";
        private const string                    YES      = "Y";

        private const int                       VERSION_1 = 1;
        private const int                       VERSION_2 = 2;

        #endregion
    }
}