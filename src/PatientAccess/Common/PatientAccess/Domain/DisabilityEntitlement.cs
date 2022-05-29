using System;
using PatientAccess.Annotations;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public class DisabilityEntitlement : MedicareEntitlement, IRecommendationCollector, ICloneable
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
                if( recommendation.IsInSpecialProgramOrLiability )
                {
                    // UC231 - Condition 11
                    // Are you currently employed? = No-Retired or No Never Employed AND If married, is your 
                    // spouse currently employed? = No-(Spouse) Retired or No-(Spouse) Never Employed
                    // AND MSP does not meet conditions 1-6

                    if( (PatientEmployment != null && FamilyMemberEmployment != null)
                        &&
                        (PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) ||
                        PatientEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        &&
                        (SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) ||
                        SpouseEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        )
                    {
                        recommendation.IsMedicareRecommended = true;

                        if( PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                                "Are you currently employed?", "No - Retired" ) );
                        }
                        else if( PatientEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                                "Are you currently employed?", "No - Never employed" ) );
                        }
                        if( SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                                "If married, is your spouse currently employed?", "No - Retired" ) );
                        }
                        else if( SpouseEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                                "If married, is your spouse currently employed?", "No - Never employed" ) );
                        }

                        i_medicareResult = MEDICARE;
                    }

                    // UC231 - Condition 12
                    // Do you have GHP coverage based on your own, or a family member’s current employment? = No
                    // AND MSP does not meet conditions 1-6

                    if( GroupHealthPlanCoverage.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;

                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Do you have group health plan (GHP) coverage based on your own, or a family member's " +
                            "current employment?", "No" ) );

                        i_medicareResult = MEDICARE;
                    }
                    // UC231 - Condition 13
                    // Does the employer that sponsors your GHP employ 100 or more employees? = No AND 
                    // MSP does not meet conditions 1-6

                    if( GHPLimitExceeded.Code.ToUpper().Equals( "N" ) )
                    {
                        recommendation.IsMedicareRecommended = true;

                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Does the employer that sponsors your GHP employ 100 or more employees?", "No" ) );

                        i_medicareResult = MEDICARE;
                    }
                }
                // UC231 - Condition 14
                // Does the employer that sponsors your GHP employ 100 or more employees? = Yes

                if( GHPLimitExceeded.Code.ToUpper().Equals( "Y" ) )
                {
                    recommendation.IsMedicareRecommended = false;

                    if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }

                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                        "Does the employer that sponsors your GHP employ 100 or more employees?", "Yes" ) );

                    i_medicareResult = GHP;
                }
            }

                // MSP Version 2

            else if( recommendation.MSPVersion == VERSION_2 )
            {
                bool blnPatEmployed         = false;
                bool blnSpouseEmployed      = false;

                if( recommendation.IsInSpecialProgramOrLiability )
                {
                    // determine the conclusion

                    recommendation.IsMedicareRecommended    = true;
                    this.i_medicareResult                   = MEDICARE;

                    if( this.GHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES
                        || this.SpouseGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES
                        || this.FamilyMemberGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES)
                    {
                        if( recommendation.FirstSetOfQuestionsFailed || recommendation.SecondSetOfQuestionsFailed )
                        {
                            recommendation.ContributingConditions.Clear();
                        }

                        recommendation.IsMedicareRecommended    = false;
                        this.i_medicareResult                   = GHP;
                    }
                }
                else
                {
                    recommendation.IsMedicareRecommended    = false;
                    this.i_medicareResult                   = GHP;
                }

                // Patient employed?

                if( PatientEmployment != null
                    && ( PatientEmployment.Status.Code == EmploymentStatus.RETIRED_CODE  
                    || PatientEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE ) 
                    )
                {
                    if( PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Are you currently employed?", "No - Retired" ) );
                    }
                    else if( PatientEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Are you currently employed?", "No - Never employed" ) );
                    }
                }
                else
                {
                    blnPatEmployed = true;

                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                        "Are you currently employed?", "Yes" ) );
                }

                // Spouse employed?

                if( SpouseEmployment != null 
                    && ( SpouseEmployment.Status.Code ==  EmploymentStatus.RETIRED_CODE
                        || SpouseEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
                    )
                {
                    if( SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Is your spouse currently employed?", "No - (Spouse) Retired" ) );
                    }
                    else if( SpouseEmployment != null &&
                        SpouseEmployment.Status.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Is your spouse currently employed?", "No - (Spouse) Never employed" ) );
                    }                        
                }
                else
                {
                    blnSpouseEmployed = true;

                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                        "Is your spouse currently employed?", "Yes" ) );
                }
                                        
                if( this.GroupHealthPlanCoverage.Code == YesNoFlag.CODE_YES )
                {
                    if( this.GroupHealthPlanType.Oid == GroupHealthPlanType.SELF_OID )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Do you have group health plan coverage (GHP) based on your own, or a spouse’s current employment?", "Yes - Self" ) );
                    }
                    else if( this.GroupHealthPlanType.Oid == GroupHealthPlanType.SPOUSE_OID )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Do you have group health plan coverage (GHP) based on your own, or a spouse’s current employment?", "Yes - Spouse" ) );
                    }
                    else if( this.GroupHealthPlanType.Oid == GroupHealthPlanType.BOTH_OID )
                    {
                        blnPatEmployed = true;
                        blnSpouseEmployed = true;

                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "Do you have group health plan coverage (GHP) based on your own, or a spouse’s current employment?", "Yes - Both" ) );
                    }
                }

                if( blnPatEmployed )
                {
                    if( this.GHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "If you have GHP coverage based on your own current employment, does the employer "
                            + "that sponsors or contributes to the GHP, employ 100 or more employees? ",
                            "Yes" ) );
                    }
                    else
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "If you have GHP coverage based on your own current employment, does the employer "
                            + "that sponsors or contributes to the GHP, employ 100 or more employees? ",
                            "No" ) );

                    }
                }
                    
                if( blnSpouseEmployed )
                {
                    if( this.SpouseGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "If you have GHP coverage based on your spouse's current employment, does your spouse's employer "
                            + "that sponsors or contributes to the GHP, employ 100 or more employees? ",
                            "Yes" ) );
                    }
                    else
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "If you have GHP coverage based on your spouse's current employment, does your spouse's employer "
                            + "that sponsors or contributes to the GHP, employ 100 or more employees? ",
                            "No" ) );
                    }
                }                                               

                if( this.FamilyMemberGHPFlag.Code == YesNoFlag.CODE_NO  )
                {
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                        "Are you covered under the GHP of a family member other than your spouse?", "No" ) );                        
                }
                else
                {
                    recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                        "Are you covered under the GHP of a family member other than your spouse?", "Yes" ) );                        

                    if( this.FamilyMemberGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "If you have GHP coverage based on a family member’s current employment, does your "
                            + "family member’s employer, that sponsors or contributes to the GHP, employ 100 or "
                            + "more employees?", "Yes" ) );                        
                    }
                    else
                    {
                        recommendation.AddCondition( new ContributingCondition( "Entitlement by Disability",
                            "If you have GHP coverage based on a family member’s current employment, does your "
                            + "family member’s employer, that sponsors or contributes to the GHP, employ 100 or "
                            + "more employees?", "No" ) );                        
                    }
                }                    
            }
        }

        public object Clone()
        {
            DisabilityEntitlement newObject = new DisabilityEntitlement();

            SetYesNoState( newObject.GHPLimitExceeded, this.GHPLimitExceeded );
            SetYesNoState( newObject.GHPCoverageOtherThanSpouse, this.GHPCoverageOtherThanSpouse );

            if( this.MedicareResult != null )
            {
                newObject.MedicareResult = (string) this.MedicareResult.Clone();
            }
            newObject.GroupHealthPlanCoverage = (YesNoFlag) this.GroupHealthPlanCoverage.Clone();

            if( this.FamilyMemberEmployment != null )
            {
                newObject.FamilyMemberEmployment = (Employment) this.FamilyMemberEmployment.Clone();
            }
            else
            {
                newObject.FamilyMemberEmployment = null;
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
            DisabilityEntitlement de = obj as DisabilityEntitlement;

            bool employment = (de.FamilyMemberEmployment == null && this.FamilyMemberEmployment == null ? true : 
                de.FamilyMemberEmployment == null && this.FamilyMemberEmployment != null ? false :
                de.FamilyMemberEmployment != null && this.FamilyMemberEmployment == null ? false :
                de.FamilyMemberEmployment.Equals( this.FamilyMemberEmployment ));

            bool medResult = (de.MedicareResult == null && this.MedicareResult == null ? true : 
                de.MedicareResult == null && this.MedicareResult != null ? false :
                de.MedicareResult != null && this.MedicareResult == null ? false :
                de.MedicareResult.Equals( this.MedicareResult ));

            bool ghp    = this.GHPLimitExceeded.Equals( de.GHPLimitExceeded );
            bool other  = this.GHPCoverageOtherThanSpouse.Equals( de.GHPCoverageOtherThanSpouse );
            bool bass   = base.Equals( obj );
            bool result = (employment && medResult && ghp && other && bass);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties

        public YesNoFlag GHPEmploysMoreThanXFlag
        {
            get
            {
                return this.i_GHPEmploysMoreThanXFlag;
            }
            set
            {
                this.i_GHPEmploysMoreThanXFlag = value;
            }
        }

        public YesNoFlag SpouseGHPEmploysMoreThanXFlag
        {
            get
            {
                return this.i_SpouseGHPEmploysMoreThanXFlag;
            }
            set
            {
                this.i_SpouseGHPEmploysMoreThanXFlag = value;
            }
        }

        public YesNoFlag FamilyMemberGHPEmploysMoreThanXFlag
        {
            get
            {
                return this.i_FamilyMemberGHPEmploysMoreThanXFlag;
            }
            set
            {
                this.i_FamilyMemberGHPEmploysMoreThanXFlag = value;
            }
        }

        public YesNoFlag FamilyMemberGHPFlag
        {
            get
            {
                return this.i_FamilyMemberGHPFlag;
            }
            set
            {
                this.i_FamilyMemberGHPFlag = value;
            }
        }

        public YesNoFlag PatientEmployedFlag
        {
            get
            {
                return this.i_PatientEmployedFlag;
            }
            set
            {
                this.i_PatientEmployedFlag = value;
            }
        }

        public YesNoFlag SpouseEmployedFlag
        {
            get
            {
                return this.i_SpouseEmployedFlag;
            }
            set
            {
                this.i_SpouseEmployedFlag = value;
            }
        }

        public Employment FamilyMemberEmployment 
        {
            get
            {
                return this.i_FamilyMemeberEmployment;
            }
            set
            {
                this.i_FamilyMemeberEmployment = value;
            }
        }

        public YesNoFlag GHPCoverageOtherThanSpouse
        {
            get
            {
                return this.i_GHPCoverageOtherThanSpouse;
            }
            set
            {
                this.i_GHPCoverageOtherThanSpouse = value;
            }
        }

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

        public UnemployedType PatientUnemployedType
        {
            get
            {
                return this.i_PatientUnemployedType;
            }
            set
            {
                this.i_PatientUnemployedType = value;
            }
        }

        public UnemployedType SpouseUnemployedType
        {
            get
            {
                return this.i_SpouseUnemployedType;
            }
            set
            {
                this.i_SpouseUnemployedType = value;
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

        public DisabilityEntitlement()
        {
        }

        #endregion

        #region Data Elements

        private string                      i_medicareResult;

        private Employment                  i_FamilyMemeberEmployment               = new Employment();
        
        private YesNoFlag                   i_GHPLimitExceeded                      = new YesNoFlag();
        private YesNoFlag                   i_GHPCoverageOtherThanSpouse            = new YesNoFlag();
        private YesNoFlag                   i_GHPEmploysMoreThanXFlag               = new YesNoFlag();
        private YesNoFlag                   i_SpouseGHPEmploysMoreThanXFlag         = new YesNoFlag();
        private YesNoFlag                   i_FamilyMemberGHPEmploysMoreThanXFlag   = new YesNoFlag();
        private YesNoFlag                   i_PatientEmployedFlag                   = new YesNoFlag();
        private YesNoFlag                   i_SpouseEmployedFlag                    = new YesNoFlag();
        private YesNoFlag                   i_FamilyMemberGHPFlag                   = new YesNoFlag();

        private UnemployedType              i_PatientUnemployedType                 = new UnemployedType();
        private UnemployedType              i_SpouseUnemployedType                  = new UnemployedType();
        
        #endregion

        #region Constants
        
        private const string                    GHP         = "GHP is primary";
        private const string                    MEDICARE    = "Medicare is primary";
        private const int                       VERSION_1   = 1;
        private const int                       VERSION_2   = 2;

        #endregion
    }
}