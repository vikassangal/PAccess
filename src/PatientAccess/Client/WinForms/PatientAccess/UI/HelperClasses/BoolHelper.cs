namespace PatientAccess.UI.HelperClasses
{
    public static class BoolHelper
    {
        public static bool TrueCountExceedsThreshold(int threshold, params bool[] input)
        {
            int trueCount = 0;
            foreach (bool b in input)
                if (b && (++trueCount > threshold))
                    return true;
            return false;
        }
    }
}
