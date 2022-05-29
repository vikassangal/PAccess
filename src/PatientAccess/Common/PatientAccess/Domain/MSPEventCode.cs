namespace PatientAccess.Domain
{
    /// <summary>
    /// These are the values used by the Medicare Secondary Payor Wizard Dialog's
    /// Finite State Machine.  This is also consumed by MedicareSecondaryPayor.EntitlementType
    /// to determine which class derived from MedicareEntitlement to instantiate.
    /// The selected value from the Wizard's screen determines which type is instantiated.
    /// </summary>
    public class MSPEventCode
    {
        public static int YesStimulus()
        {
            return YES;
        }

        public static int NoStimulus()
        {
            return NO;
        }

        public static int ESRDStimulus()
        {
            return ESRD;
        }

        public static int DisabilityStimulus()
        {
            return DISABILITY;
        }

        public static int AgeStimulus()
        {
            return AGE;
        }

        private static int YES        = 1;
        private static int NO         = 2;
        private static int ESRD       = 3;
        private static int DISABILITY = 4;
        private static int AGE        = 5;
    }
}
