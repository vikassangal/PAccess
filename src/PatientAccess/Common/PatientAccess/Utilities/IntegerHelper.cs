using System;

namespace PatientAccess.Utilities
{
    public class IntegerHelper
    {
        public static bool IsPositiveInteger( object rawAccountNumber )
        {
            return IsValidInteger( rawAccountNumber ) && Convert.ToInt32( rawAccountNumber ) > 0;
        }

        private static bool IsValidInteger( object rawAccountNumber )
        {
            bool isInteger;

            try
            {
                Convert.ToInt32( rawAccountNumber );
                isInteger = true;
            }

            catch
            {
                isInteger = false;
            }

            return isInteger;
        }
    }
}
