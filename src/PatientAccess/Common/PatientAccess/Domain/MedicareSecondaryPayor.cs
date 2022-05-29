using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MedicareSecondaryPayor : PersistentModel, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void EntitlementType( Type type )
        {
            int typeCode = -1;

            if ( type.Equals( typeof( AgeEntitlement ) ) )
            {
                typeCode = MSPEventCode.AgeStimulus();
            }
            else if ( type.Equals( typeof( DisabilityEntitlement ) ) )
            {
                typeCode = MSPEventCode.DisabilityStimulus();
            }
            else if ( type.Equals( typeof( ESRDEntitlement ) ) )
            {
                typeCode = MSPEventCode.ESRDStimulus();
            }

            if ( typeCode != -1 )
            {
                EntitlementSelector = typeCode;
                i_MedicareEntitlement =
                    Activator.CreateInstance( ( Type )c_EntitlementTypeMap[typeCode] ) as MedicareEntitlement;
            }
        }

        public new object Clone()
        {
            // Create an instance of this specific type.
            var newObject = new MedicareSecondaryPayor
                                {
                                    SpecialProgram = (SpecialProgram) SpecialProgram.Clone(),
                                    LiabilityInsurer = (LiabilityInsurer) LiabilityInsurer.Clone(),
                                    MSPVersion = MSPVersion
                                };


            if ( MedicareEntitlement is AgeEntitlement )
            {
                newObject.EntitlementType( typeof( AgeEntitlement ) );
                newObject.MedicareEntitlement = ( AgeEntitlement )( MedicareEntitlement as AgeEntitlement ).Clone();
            }
            else if ( MedicareEntitlement is DisabilityEntitlement )
            {
                newObject.EntitlementType( typeof( DisabilityEntitlement ) );
                newObject.MedicareEntitlement = ( DisabilityEntitlement )( MedicareEntitlement as DisabilityEntitlement ).Clone();
            }
            else if ( MedicareEntitlement is ESRDEntitlement )
            {
                newObject.EntitlementType( typeof( ESRDEntitlement ) );
                newObject.MedicareEntitlement = ( ESRDEntitlement )( MedicareEntitlement as ESRDEntitlement ).Clone();
            }

            return newObject;
        }

        public MSPRecommendation MakeRecommendation()
        {
            // Clear the ContributingCondition list and flags
            mspRecommendation.Reset();

            mspRecommendation.MSPVersion = MSPVersion;

            SpecialProgram.CollectRecommendation( mspRecommendation );
            LiabilityInsurer.CollectRecommendation( mspRecommendation );

            if ( mspRecommendation.FirstSetOfQuestionsFailed && mspRecommendation.SecondSetOfQuestionsFailed )
            {
                mspRecommendation.IsInSpecialProgramOrLiability = true;
            }
            else
            {
                mspRecommendation.IsInSpecialProgramOrLiability = false;
            }

            if ( MedicareEntitlement is AgeEntitlement )
            {
                ( MedicareEntitlement as AgeEntitlement ).CollectRecommendation( mspRecommendation );
            }
            else if ( MedicareEntitlement is DisabilityEntitlement )
            {
                ( MedicareEntitlement as DisabilityEntitlement ).CollectRecommendation( mspRecommendation );
            }
            else if ( MedicareEntitlement is ESRDEntitlement )
            {
                ( MedicareEntitlement as ESRDEntitlement ).CollectRecommendation( mspRecommendation );
            }

            return mspRecommendation;
        }

        public override bool Equals( object obj )
        {
            if ( obj == null )
            {
                return false;
            }
            var msp = ( MedicareSecondaryPayor )obj;
            bool result = SpecialProgram.Equals( msp.SpecialProgram ) &&
                LiabilityInsurer.Equals( msp.LiabilityInsurer );

            if ( MedicareEntitlement == null )
            {
                Type entitlementType = null;

                if ( msp.MedicareEntitlement != null )
                {
                    entitlementType = msp.MedicareEntitlement.GetType();
                    EntitlementType( entitlementType );
                    result = false;
                }

                else
                {
                    result = MedicareEntitlement.GetType().Equals( entitlementType );

                    if ( result == false )
                    {   // Entitlement type was changed by the user.  Modify Account entitlement type to match.
                        EntitlementType( entitlementType );
                    }
                    else
                    {
                        result = MedicareEntitlement.Equals( msp.MedicareEntitlement );
                    }
                }
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// This method is used to Copy Forward only certain Medicare 
        /// Secondary Payor values from the old account to the new account, 
        /// as per Copy-Forward requirements, either via AccountCopyStrategy
        /// or AccountPBARBroker (for Post-MSE registration).
        /// </summary>
        /// <param name="oldMsp">Medicare Secondary Payor from old account</param>
        /// <returns>Partially Copied Over Medicare Secondary Payor for new account</returns>
        public static MedicareSecondaryPayor GetPartiallyCopiedForwardMSPFrom( MedicareSecondaryPayor oldMsp )
        {
            var newMsp = new MedicareSecondaryPayor();
            if ( oldMsp == null )
            {
                return newMsp;
            }

            newMsp.MSPVersion = oldMsp.MSPVersion;

            if ( oldMsp.SpecialProgram != null )
            {
                newMsp.SpecialProgram = new SpecialProgram
                                            {
                                                BlackLungBenefits = oldMsp.SpecialProgram.BlackLungBenefits,
                                                BLBenefitsStartDate = oldMsp.SpecialProgram.BLBenefitsStartDate
                                            };
            }

            if ( oldMsp.MedicareEntitlement != null )
            {
                // entitlement-specific fields
                if ( oldMsp.MedicareEntitlement.GetType() == typeof( AgeEntitlement ) )
                {
                    var oldAgeEntitlement = ( AgeEntitlement )oldMsp.MedicareEntitlement;
                    var newAgeEntitlement = new AgeEntitlement();

                    if ( oldAgeEntitlement.PatientEmployment != null &&
                         oldAgeEntitlement.PatientEmployment.Status != null )
                    {
                        if ( oldAgeEntitlement.PatientEmployment.Status.Code == EmploymentStatus.RETIRED_CODE )
                        {
                            newAgeEntitlement.PatientEmployment.Status = oldAgeEntitlement.PatientEmployment.Status;
                            newAgeEntitlement.PatientEmployment.RetiredDate = oldAgeEntitlement.PatientEmployment.RetiredDate;
                        }
                        else if ( oldAgeEntitlement.PatientEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
                        {
                            newAgeEntitlement.PatientEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
                        }
                    }

                    if ( oldAgeEntitlement.SpouseEmployment != null &&
                         oldAgeEntitlement.SpouseEmployment.Status != null )
                    {
                        if ( oldAgeEntitlement.SpouseEmployment.Status.Code == EmploymentStatus.RETIRED_CODE )
                        {
                            newAgeEntitlement.SpouseEmployment.Status = oldAgeEntitlement.SpouseEmployment.Status;
                            newAgeEntitlement.SpouseEmployment.RetiredDate =
                                oldAgeEntitlement.SpouseEmployment.RetiredDate;
                        }
                        else if ( oldAgeEntitlement.SpouseEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
                        {
                            newAgeEntitlement.SpouseEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
                        }
                    }

                    newMsp.MedicareEntitlement = newAgeEntitlement;
                }
                else if ( oldMsp.MedicareEntitlement.GetType() == typeof( DisabilityEntitlement ) )
                {
                    var oldDisabilityEntitlement = ( DisabilityEntitlement )oldMsp.MedicareEntitlement;
                    var newDisabilityEntitlement = new DisabilityEntitlement();

                    if ( oldDisabilityEntitlement.PatientEmployment != null &&
                         oldDisabilityEntitlement.PatientEmployment.Status != null )
                    {
                        if ( oldDisabilityEntitlement.PatientEmployment.Status.Code == EmploymentStatus.RETIRED_CODE )
                        {
                            newDisabilityEntitlement.PatientEmployment.Status = oldDisabilityEntitlement.PatientEmployment.Status;
                            newDisabilityEntitlement.PatientEmployment.RetiredDate =
                                oldDisabilityEntitlement.PatientEmployment.RetiredDate;
                        }
                        else if ( oldDisabilityEntitlement.PatientEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
                        {
                            newDisabilityEntitlement.PatientEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
                        }
                    }

                    if ( oldDisabilityEntitlement.SpouseEmployment != null &&
                         oldDisabilityEntitlement.SpouseEmployment.Status != null )
                    {
                        if ( oldDisabilityEntitlement.SpouseEmployment.Status.Code == EmploymentStatus.RETIRED_CODE )
                        {
                            newDisabilityEntitlement.SpouseEmployment.Status = oldDisabilityEntitlement.SpouseEmployment.Status;
                            newDisabilityEntitlement.SpouseEmployment.RetiredDate =
                                oldDisabilityEntitlement.SpouseEmployment.RetiredDate;
                        }
                        else if ( oldDisabilityEntitlement.SpouseEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
                        {
                            newDisabilityEntitlement.SpouseEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
                        }
                    }

                    newMsp.MedicareEntitlement = newDisabilityEntitlement;
                }
                else if ( oldMsp.MedicareEntitlement.GetType() == typeof( ESRDEntitlement ) )
                {
                    var oldEsrdEntitlement = ( ESRDEntitlement )oldMsp.MedicareEntitlement;
                    var newEsrdEntitlement = new ESRDEntitlement
                                                             {
                                                                 TransplantDate = oldEsrdEntitlement.TransplantDate,
                                                                 DialysisDate = oldEsrdEntitlement.DialysisDate,
                                                                 DialysisCenterName = oldEsrdEntitlement.DialysisCenterName
                                                             };

                    if ( oldEsrdEntitlement.KidneyTransplant != null && 
                         oldEsrdEntitlement.KidneyTransplant.Code == YesNoFlag.CODE_YES )
                    {
                        newEsrdEntitlement.KidneyTransplant = oldEsrdEntitlement.KidneyTransplant;
                    }

                    if ( oldEsrdEntitlement.DialysisTreatment != null && 
                         oldEsrdEntitlement.DialysisTreatment.Code == YesNoFlag.CODE_YES )
                    {
                        newEsrdEntitlement.DialysisTreatment = oldEsrdEntitlement.DialysisTreatment;
                    }

                    if ( oldEsrdEntitlement.WithinCoordinationPeriod != null && 
                         oldEsrdEntitlement.WithinCoordinationPeriod.Code == YesNoFlag.CODE_YES )
                    {
                        newEsrdEntitlement.WithinCoordinationPeriod = oldEsrdEntitlement.WithinCoordinationPeriod;

                        switch ( oldMsp.MSPVersion )
                        {
                            case 1:
                                if ( oldEsrdEntitlement.ESRDandAgeOrDisability != null &&
                                     oldEsrdEntitlement.ESRDandAgeOrDisability.Code == YesNoFlag.CODE_YES )
                                {
                                    newEsrdEntitlement.BasedOnAgeOrDisability = oldEsrdEntitlement.ESRDandAgeOrDisability;

                                    if ( oldEsrdEntitlement.BasedOnESRD != null )
                                    {
                                        newEsrdEntitlement.BasedOnESRD = oldEsrdEntitlement.BasedOnESRD;
                                    }
                                }
                                break;

                            case 2:
                                if ( oldEsrdEntitlement.BasedOnAgeOrDisability != null &&
                                     oldEsrdEntitlement.BasedOnAgeOrDisability.Code == YesNoFlag.CODE_YES )
                                {
                                    newEsrdEntitlement.BasedOnAgeOrDisability = oldEsrdEntitlement.BasedOnAgeOrDisability;

                                    if ( oldEsrdEntitlement.BasedOnESRD != null )
                                    {
                                        newEsrdEntitlement.BasedOnESRD = oldEsrdEntitlement.BasedOnESRD;
                                    }
                                }
                                break;
                        }
                    }

                    newMsp.MedicareEntitlement = newEsrdEntitlement;
                }
            }

            return newMsp;
        }
        #endregion

        #region Properties
        public SpecialProgram SpecialProgram
        {
            get
            {
                return i_SpecialProgram;
            }
            set
            {
                i_SpecialProgram = value;
            }
        }

        public LiabilityInsurer LiabilityInsurer
        {
            get
            {
                return i_LiabilityInsurer;
            }
            set
            {
                i_LiabilityInsurer = value;
            }
        }

        public MedicareEntitlement MedicareEntitlement
        {
            get
            {
                return i_MedicareEntitlement;
            }
            set
            {
                i_MedicareEntitlement = value;
            }
        }

        private int EntitlementSelector
        {
            get
            {
                return i_entitlementSelector;
            }
            set
            {
                i_entitlementSelector = value;
            }
        }

        public bool HasBeenCompleted
        {
            get
            {
                return i_HasBeenCompleted;
            }
            set
            {
                i_HasBeenCompleted = value;
            }
        }

        public int MSPVersion
        {
            get
            {
                return i_MSPVersion;
            }
            set
            {
                i_MSPVersion = value;
            }
        }
        #endregion

        #region Private Methods
        private void PopulateEntitlementTypeMap()
        {
            if ( c_EntitlementTypeMap == null )
            {
                lock ( c_Sync )
                {
                    if ( c_EntitlementTypeMap == null )
                    {
                        c_EntitlementTypeMap = new Hashtable( 3 );
                        c_EntitlementTypeMap.Add( MSPEventCode.ESRDStimulus(), typeof( ESRDEntitlement ) );
                        c_EntitlementTypeMap.Add( MSPEventCode.DisabilityStimulus(), typeof( DisabilityEntitlement ) );
                        c_EntitlementTypeMap.Add( MSPEventCode.AgeStimulus(), typeof( AgeEntitlement ) );
                    }
                }
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MedicareSecondaryPayor()
        {
            PopulateEntitlementTypeMap();
        }
        #endregion

        #region Data Elements

        private Hashtable c_EntitlementTypeMap;
        private LiabilityInsurer i_LiabilityInsurer = new LiabilityInsurer();
        private MSPRecommendation mspRecommendation = new MSPRecommendation();
        private MedicareEntitlement i_MedicareEntitlement;
        private SpecialProgram i_SpecialProgram = new SpecialProgram();

        private int i_entitlementSelector = -1;
        private int i_MSPVersion;

        private bool i_HasBeenCompleted;

        private object c_Sync = new Object();

        #endregion

        #region Constants
        #endregion
    }
}