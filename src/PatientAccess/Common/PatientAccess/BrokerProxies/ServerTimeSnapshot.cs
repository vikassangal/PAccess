using System;

namespace PatientAccess.BrokerProxies
{
    /// <summary>
    /// 
    /// </summary>
    internal class ServerTimeSnapshot
    {
		#region Properties 

        internal static DateTime DateTimeOverride { private get; set; }

        /// <summary>
        /// Gets the get current UTC date time.
        /// </summary>
        /// <value>The get current UTC date time.</value>
        private static DateTime CurrentUtcDateTime
        {
         
            get
            {
                if( DateTimeOverride.Equals(DateTime.MinValue) )
                {
                    return DateTime.UtcNow;
                }

                return DateTimeOverride;
            }
        }

         /// <summary>
        /// Gets or sets the server date time.
        /// </summary>
        /// <value>The server date time.</value>
        public DateTime ServerDateTime { private get; set; }

        /// <summary>
        /// Gets or sets the time snapshot created.
        /// </summary>
        /// <value>The time snapshot created.</value>
        public DateTime TimeSnapshotCreated { private get; set;}

		#endregion Properties 

		#region Public Methods 

        /// <summary>
        /// Calculates the local time.
        /// </summary>
        /// <returns></returns>
        public DateTime CalculateLocalTime()
        {
            return ServerDateTime.Add( CurrentUtcDateTime - TimeSnapshotCreated );
        }

		#endregion Public Methods 
    }
}