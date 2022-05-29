using System;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
   public class OKTAServiceFeatureManager
    {
        /// <summary>
        /// Determines whether [OKTA enabled for Facility].
        /// </summary>
        /// <param name="facility">The facility.</param>
        /// <returns>
        /// 	<c>true</c> if [OKTA service enabled for facility] [the specified facility]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOKTAEnabled(Facility facility)
        {
            if(facility.Code!=null)
            {
                return facility.IsOKTAEnabled;
            }
            return false;

        }

    }
}
