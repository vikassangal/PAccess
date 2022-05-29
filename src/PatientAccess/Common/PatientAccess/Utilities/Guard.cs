using System;

namespace PatientAccess.Utilities
{
    public static class Guard
    {
        /// <exception cref="ArgumentNullException"><c>argument</c> is null.</exception>
        public static void ThrowIfArgumentIsNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }


        /// <exception cref="ArgumentNullException"><c>argument</c> is null or empty.</exception>
        public static void ThrowIfArgumentIsNullOrEmpty(string argument, string argumentName)
        {
            if (String.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(argumentName);
            }
        }


        /// <exception cref="ArgumentException"><c>period</c> is not positive</exception>
        public static void ThrowIfTimeSpanIsNotPositive(TimeSpan period, string argumentName)
        {
            if (period <= TimeSpan.Zero)
            {
                throw new ArgumentException(argumentName + " can only take positive TimeSpan values");
            }
        }



        /// <exception cref="ArgumentException"><c>argument</c> is <c>Guid.Empty</c></exception>
        public static void ThrowIfArguementIsEmpty(Guid argument, string argumentName)
        {
            if ( argument == Guid.Empty )
            {
                throw new ArgumentException(argumentName + "cannot be empty");
            }
        }

        /// <exception cref="ArgumentException"><c>integer</c> is not positive</exception>
        public static void ThrowIfIntegerIsNotPositive(int integer, string argumentName)
        {
            if ( integer <= 0 )
            {
                throw new ArgumentException( argumentName + " can only take positive integer values" );
            }
        }
    }
}