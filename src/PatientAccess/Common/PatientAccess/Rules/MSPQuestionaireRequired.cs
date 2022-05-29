using System;
using System.Collections;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MSPQuestionaireRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MSPQuestionaireRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler MSPQuestionaireRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MSPQuestionaireRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MSPQuestionaireRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MSPQuestionaireRequiredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }
            Model_Account = context as Account;

            if( InsuranceIsMedicare() == false )
            {
                return true;
            }
            else 
            {
                
                if( Model_Account.MedicareSecondaryPayor.MSPVersion == 0 )
                {
                    return false;
                }             
                else if( Model_Account.MedicareSecondaryPayor.MSPVersion == VERSION_1 )
                {
                    if( this.IsMSPDataComplete_V1() == false )
                    {
                        if( this.FireEvents && MSPQuestionaireRequiredEvent != null )
                        {
                            MSPQuestionaireRequiredEvent( this, null );
                        }

                        return false;
                    }
                    else
                    {
                        return true;
                    }                                
                }
                else if( Model_Account.MedicareSecondaryPayor.MSPVersion == VERSION_2 )
                {
                    if ( this.IsMSPDataComplete_V2() == false )
                    {
                        if( this.FireEvents && MSPQuestionaireRequiredEvent != null )
                        {
                            MSPQuestionaireRequiredEvent( this, null );                            
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
                    return false;
                }
            }
            
        }
        #endregion

        #region Properties
        public Account Model_Account
        {
            get
            {
                return i_account;
            }
            set
            {
                i_account = value;
            }
        }
        #endregion

        #region Private Methods

        public bool InsuranceIsMedicare( FinancialClass fc, VisitType vt, HospitalService hs, Coverage c )
        {
            if( c == null )                
            {
                return false;
            }

            if( fc != null )
            {
                if( fc.IsMedicare() )

                    if( vt != null
                        && vt.Code != VisitType.NON_PATIENT
                        && hs != null
                        && hs.Code != "LB"
                        && hs.Code != "SP"
                        && hs.Code != "CV" ) 
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }

            if( c.InsurancePlan.PlanID.Substring( 0, 3 ).Equals( "535" ) ||
                c.InsurancePlan.PlanID.Substring( 0, 3 ).ToUpper().Equals( InsurancePlan.MSP_PAYORCODE) )
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool InsuranceIsMedicare()
        {
            bool coverageIsMedicare = false;

            if( Model_Account == null
                || Model_Account.Insurance == null )                
            {
                return false;
            }

            Insurance insurance = Model_Account.Insurance;
            if ( Model_Account.FinancialClass != null )
            {
                if( Model_Account.FinancialClass.IsMedicare() )

                    if( Model_Account.KindOfVisit != null
                        && Model_Account.KindOfVisit.Code != VisitType.NON_PATIENT
                        && Model_Account.HospitalService != null
                        && Model_Account.HospitalService.Code != "LB"
                        && Model_Account.HospitalService.Code != "SP"
                        && Model_Account.HospitalService.Code != "CV") 
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }


            ICollection coverageCollection = insurance.Coverages;
            if( coverageCollection == null )
            {
                return false;
            }

            foreach( Coverage coverage in coverageCollection )
            {
                if( coverage == null || coverage.InsurancePlan == null )
                {
                    continue;
                }
                else if( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID) ||
                    coverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                {
                    if( Model_Account.KindOfVisit == null ||
                        Model_Account.KindOfVisit.Code == VisitType.NON_PATIENT ||
                        Model_Account.HospitalService == null ||
                        Model_Account.HospitalService.Code == HospitalService.LEASED_BEDS ||
                        Model_Account.HospitalService.Code == HospitalService.SPECIMEN_ONLY ||
                        Model_Account.HospitalService.Code == HospitalService.COVID_VACCINE_CLINIC )
                    {
                        continue;
                    }
                    else if( coverage.InsurancePlan.PlanID.Substring( 0, 3 ).Equals( "535" ) ||
                        coverage.InsurancePlan.PlanID.Substring( 0, 3 ).ToUpper().Equals( InsurancePlan.MSP_PAYORCODE ) )
                    {
                        coverageIsMedicare = true;
                        break;
                    }
                }
            }
            return coverageIsMedicare;
        }

        private bool IsMSPDataComplete_V1()
        {
            YesNoFlag flag = new YesNoFlag();

            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }
            else if( flag.Code.Equals( "Y" ) &&
                Model_Account.MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.Code.Equals( " " ) )
            {
                return false;
            }
            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.GovernmentProgram;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }
            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.DVAAuthorized;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }
            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.WorkRelated;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }
            else if( flag.Code.Equals( "N" ) )
            {
                flag = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated;
                if( flag.Code.Equals( " " ) )
                {
                    return false;
                }
                else if( flag.Code.Equals( "Y" ) )
                {
                    if( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AccidentType == null )
                    {
                        return false;
                    }
                    else if( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AccidentType.Oid == TypeOfAccident.OTHER )
                    {
                        flag = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AnotherPartyResponsibility;
                        if( flag.Code.Equals( " " ) )
                        {
                            return false;
                        }
                    }
                }
            }

            if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
            {
                return false;
            }
            else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( ESRDEntitlement ) ) )
            {
                flag = Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage;
                if( flag.Code.Equals( " " ) )
                {
                    return false;
                }
                else if( flag.Code.Equals( "N" ) )
                {
                    return true;
                }
                else
                {   // GHP flag is Yes
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).KidneyTransplant;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).DialysisTreatment;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code.Equals( "N" ) )
                    {
                        return true;
                    }
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).WithinCoordinationPeriod;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code.Equals( "N" ) )
                    {
                        return true;
                    }
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).ESRDandAgeOrDisability;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code.Equals( "N" ) )
                    {
                        return true;
                    }
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).BasedOnESRD;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code.Equals( "Y" ) )
                    {
                        return true;
                    }
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).BasedOnAgeOrDisability;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( DisabilityEntitlement ) ) )
            {
                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).PatientEmployment == null )
                {
                    return false;
                }
                else
                {
                    EmploymentStatus patientStatus = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).PatientEmployment.Status;

                    if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                        as DisabilityEntitlement).SpouseEmployment != null )
                    {
                        EmploymentStatus familyStatus = ((Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                            as DisabilityEntitlement).SpouseEmployment).Status;
                        if( (patientStatus.Code.Equals(EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            patientStatus.Code.Equals( EmploymentStatus.RETIRED_CODE) )
                            &&
                            (familyStatus.Code.Equals(EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            familyStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) ) )
                        {   // Neither is employed.  The screen ends with these answers
                            return true;
                        }
                        else
                        {
                            flag = Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage;
                            if( flag.Code.Equals( " " ) )
                            {
                                return false;
                            }
                            else if( flag.Code.Equals( "N" ) )
                            {
                                return true;
                            }
                            else if( flag.Code.Equals( "Y" ) )
                            {
                                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GHPCoverageOtherThanSpouse.Code.Equals( " " ) )
                                {
                                    return false;
                                }
                                flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GHPLimitExceeded;
                                if( flag.Code.Equals( " " ) )
                                {
                                    return false;
                                }
                                else if( flag.Code.Equals( "N" ) )
                                {
                                    return true;
                                }
                                flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GHPLimitExceeded;
                                if( flag.Code.Equals( " " ) )
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( AgeEntitlement ) ) )
            {
                Employment patientEmployment = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as 
                                                AgeEntitlement).PatientEmployment;
                if(  patientEmployment == null || patientEmployment.Status == null )
                {
                    return false;
                }
                else
                {
                    EmploymentStatus patientEmploymentStatus = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as 
                        AgeEntitlement).PatientEmployment.Status;

                    Employment spouseEmployment = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                                    AgeEntitlement).SpouseEmployment;

                    if(  spouseEmployment != null && spouseEmployment.Status != null )
                    {
                        EmploymentStatus spouseStatus = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                            AgeEntitlement).SpouseEmployment.Status;

                        if( (patientEmploymentStatus.Code.Equals(EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            patientEmploymentStatus.Code.Equals(EmploymentStatus.RETIRED_CODE ) )
                            &&
                            (spouseStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            spouseStatus.Code.Equals( EmploymentStatus.RETIRED_CODE) ) )
                        {
                            return true;
                        }
                        else
                        {
                            flag = Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage;
                            if( flag.Code.Equals( " " ) )
                            {
                                return false;
                            }
                            else if( flag.Code.Equals( "N" ) )
                            {
                                return true;
                            }
                            flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GHPLimitExceeded;
                            if( flag.Code.Equals( " " ) )
                            {
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
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsMSPDataComplete_V2()
        {
            YesNoFlag flag = new YesNoFlag();

            // Special Programs

            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }
            else if( flag.Code.Equals( "Y" ) &&
                Model_Account.MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.Code.Equals( " " ) )
            {
                return false;
            }
            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.GovernmentProgram;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }
            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.DVAAuthorized;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }
            flag = Model_Account.MedicareSecondaryPayor.SpecialProgram.WorkRelated;
            if( flag.Code.Equals( " " ) )
            {
                return false;
            }

                // Liability Insurer

            else if( flag.Code.Equals( "N" ) )
            {
                flag = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated;
                if( flag.Code.Equals( " " ) )
                {
                    return false;
                }
                else if( flag.Code.Equals( "Y" ) )
                {                                    
                    flag = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NoFaultInsuranceAvailable;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }

                    flag = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.LiabilityInsuranceAvailable;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                }               
            }

            // Entitlements

            if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
            {
                return false;
            }

            // ESRD Entitlement

            else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( ESRDEntitlement ) ) )
            {
                flag = Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage;
                if( flag.Code.Equals( " " ) )
                {
                    return false;
                }
                else if( flag.Code.Equals( "N" ) )
                {
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                       ESRDEntitlement).DialysisTreatment;
                    if (flag.Code.Equals(" "))
                    {
                        return false;
                    }
                    else if (flag.Code.Equals("N"))
                    {
                        return true;
                    }
                    else
                    {
                        if ((Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).DialysisDate
                                == DateTime.MinValue)
                        {
                            return false;
                        }
                         
                    }
                    return true;
                }
                else
                {   // GHP flag is Yes
                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).KidneyTransplant;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code == "N" )
                    {
                        return true;
                    }
                    else
                    {
                        if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).TransplantDate
                            == DateTime.MinValue )
                        {
                            return false;
                        }
                    }

                   

                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).WithinCoordinationPeriod;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code.Equals( "N" ) )
                    {
                        return true;
                    }

                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).BasedOnAgeOrDisability;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code.Equals( "N" ) )
                    {
                        return true;
                    }

                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).BasedOnESRD;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else if( flag.Code.Equals( "Y" ) )
                    {
                        return true;
                    }

                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        ESRDEntitlement).ProvisionAppliesFlag;
                    if( flag.Code.Equals( " " ) )
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            // Disability Entitlement

            else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( DisabilityEntitlement ) ) )
            {
                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).PatientEmployment == null )
                {
                    return false;
                }
                else
                {
                    EmploymentStatus patientStatus = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).PatientEmployment.Status;

                    if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                        as DisabilityEntitlement).SpouseEmployment != null )
                    {
                        EmploymentStatus familyStatus = ((Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                            as DisabilityEntitlement).SpouseEmployment).Status;
                        if( (patientStatus.Code.Equals(EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            patientStatus.Code.Equals( EmploymentStatus.RETIRED_CODE) )
                            &&
                            (familyStatus.Code.Equals(EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            familyStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) ) )
                        {   // Neither is employed.  The screen ends with these answers
                            return true;
                        }
                        else
                        {
                            flag = Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage;
                            if( flag.Code.Equals( " " ) )
                            {
                                return false;
                            }
                            else if( flag.Code.Equals( "N" ) )
                            {
                                return true;
                            }
                            else if( flag.Code.Equals( "Y" ) )
                            {
                                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GroupHealthPlanType == null
                                    || (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GroupHealthPlanType.Oid < 0)
                                {
                                    return false;
                                }

                                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GroupHealthPlanType.Oid== GroupHealthPlanType.SELF_OID
                                    && (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_BLANK )
                                {
                                    return false;
                                }

                                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GroupHealthPlanType.Oid== GroupHealthPlanType.SPOUSE_OID
                                    && (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).SpouseGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_BLANK )
                                {
                                    return false;
                                }

                                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement).GroupHealthPlanType.Oid== GroupHealthPlanType.BOTH_OID
                                    && (
                                        (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                            DisabilityEntitlement).GHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_BLANK 
                                        ||
                                        (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                            DisabilityEntitlement).SpouseGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_BLANK
                                        ) 
                                  )
                                {
                                    return false;
                                }                                
                            }
                        }

                        if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                            DisabilityEntitlement).FamilyMemberGHPFlag.Code == YesNoFlag.CODE_BLANK )
                        {
                            return false;
                        }
                        else if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                            DisabilityEntitlement).FamilyMemberGHPFlag.Code == YesNoFlag.CODE_NO )
                        {
                            return true;
                        }
                        else
                        {
                            if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                DisabilityEntitlement).FamilyMemberEmployment == null )
                            {
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
                        return false;
                    }
                }
            }

            // Age Entitlement

            else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( AgeEntitlement ) ) )
            {
                Employment patientEmployment = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as 
                    AgeEntitlement).PatientEmployment;
                if(  patientEmployment == null || patientEmployment.Status == null )
                {
                    return false;
                }
                else
                {
                    EmploymentStatus patientEmploymentStatus = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as 
                        AgeEntitlement).PatientEmployment.Status;

                    Employment spouseEmployment = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        AgeEntitlement).SpouseEmployment;

                    if(  spouseEmployment != null && spouseEmployment.Status != null )
                    {
                        EmploymentStatus spouseStatus = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                            AgeEntitlement).SpouseEmployment.Status;

                        if( (patientEmploymentStatus.Code.Equals(EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            patientEmploymentStatus.Code.Equals(EmploymentStatus.RETIRED_CODE ) )
                            &&
                            (spouseStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE) || 
                            spouseStatus.Code.Equals( EmploymentStatus.RETIRED_CODE) ) )
                        {
                            return true;
                        }
                        else
                        {
                            flag = Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage;
                            if( flag.Code.Equals( " " ) )
                            {
                                return false;
                            }
                            else if( flag.Code.Equals( "N" ) )
                            {
                                return true;
                            }

                            if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GroupHealthPlanType == null
                                || (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GroupHealthPlanType.Oid < 0 )
                            {
                                return false;
                            }

                            if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GroupHealthPlanType.Oid== GroupHealthPlanType.SELF_OID
                                && (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GHPEmploysX.Code == YesNoFlag.CODE_BLANK )
                            {
                                return false;
                            }

                            if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GroupHealthPlanType.Oid== GroupHealthPlanType.SPOUSE_OID
                                && (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GHPSpouseEmploysX.Code == YesNoFlag.CODE_BLANK )
                            {
                                return false;
                            }

                            if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GroupHealthPlanType.Oid== GroupHealthPlanType.BOTH_OID
                                && (
                                (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GHPEmploysX.Code == YesNoFlag.CODE_BLANK 
                                ||
                                (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                AgeEntitlement).GHPSpouseEmploysX.Code == YesNoFlag.CODE_BLANK
                                ) 
                                )
                            {
                                return false;
                            }                 
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MSPQuestionaireRequired()
        {
        }
        #endregion

        #region Data Elements
        private Account        i_account;
        #endregion

        #region Constants

        private const int                                   VERSION_1 = 1;
        private const int                                   VERSION_2 = 2;

        #endregion
    }
}
