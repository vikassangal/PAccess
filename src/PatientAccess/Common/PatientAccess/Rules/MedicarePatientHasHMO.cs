using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicarePatientHasHMO.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicarePatientHasHMO : LeafRule
    {

		#region Delegates and Events 

        public event EventHandler MedicarePatientHasHMOEvent;

		#endregion Delegates and Events 

		#region Methods 

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {

            if( context == null || !(context is Insurance) )
            {
                return true;
            }

            Insurance insurance = context as Insurance;

            Coverage primaryCoverage = insurance.PrimaryCoverage;
            Coverage secondaryCoverage = insurance.SecondaryCoverage;
            bool otherCoverageHasMedicareHMO = false;

            if( primaryCoverage is GovernmentMedicareCoverage ||
                secondaryCoverage is GovernmentMedicareCoverage )
            {

                // Use the AS operator to take a guess at which coverage is Medicare
                GovernmentMedicareCoverage medicareCoverage = 
                    primaryCoverage as GovernmentMedicareCoverage;
                Coverage otherCoverage = 
                    secondaryCoverage;

                // Okay, the cast failed so flip the assignments
                if( medicareCoverage == null )
                {
                    medicareCoverage = secondaryCoverage as GovernmentMedicareCoverage;
                    otherCoverage = primaryCoverage;
                }
                else 
                {
                    // Check to see if  OtherCoverage is also Medicare coverage 
                    GovernmentMedicareCoverage OtherMedicareCoverage =
                        otherCoverage as GovernmentMedicareCoverage;
                    // Check to see if PatientHasMedicareHmo is set for the OtherCoverage
                    if(OtherMedicareCoverage != null &&  PatientHasMedicareHMO(OtherMedicareCoverage))
                    {
                        otherCoverageHasMedicareHMO = true;
                    }
                }
                // Perform the actual rule check
                if( PatientHasMedicareHMO( medicareCoverage ) || otherCoverageHasMedicareHMO )
                {

                    // The 4th digit of the Plan ID determines HMO Plans. 
                    // Medicare HMO plans have the following codes as the 
                    // 4th digit: 6, E, F, I (letter), O (letter), S or W
                    if( otherCoverage == null ||
                         ! otherCoverage.InsurancePlan.IsMedicareHMOPlan() )
                    {

                        if( this.FireEvents &&
                            MedicarePatientHasHMOEvent != null )
                        {

                            MedicarePatientHasHMOEvent( 
                                this, 
                                null );

                        }

                    }

                    return false;
                    
                }

            }

            return true;

        }

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicarePatientHasHMOEvent += eventHandler;
            return true;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicarePatientHasHMOEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.MedicarePatientHasHMOEvent = null;
        }

        private static bool PatientHasMedicareHMO( GovernmentMedicareCoverage medicare )
        {
            return medicare.PatientHasMedicareHMOCoverage == null || medicare.PatientHasMedicareHMOCoverage.IsYes;
        }
    
		#endregion Methods 
        
    }
}
