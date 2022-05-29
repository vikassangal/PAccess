using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// Implements social security number related data loading.
    /// </summary>
    [Serializable]
    public class SSNPBARBroker : PBARCodesBroker, ISSNBroker
    {

		#region Constants 

        private const string FLORIDA_CODE = "FL" ;
        private const string NEWBORN_DESCRIPTION = "NEWBORN" ;

		#endregion Constants 

		#region Fields 

        private static readonly SocialSecurityNumberStatus[] _SocialSecurityNumberStatuses = 
            new []
            { 
                new SocialSecurityNumberStatus( 2L, ReferenceValue.NEW_VERSION, "Known" ),
                new SocialSecurityNumberStatus( 3L, ReferenceValue.NEW_VERSION, "Newborn" ),
                new SocialSecurityNumberStatus( 4L, ReferenceValue.NEW_VERSION, "None" ),
                new SocialSecurityNumberStatus( 1L, ReferenceValue.NEW_VERSION, "Unknown" ),
                new SocialSecurityNumberStatus( 1L, ReferenceValue.NEW_VERSION, "Refused" )
            } ;

		#endregion Fields 

		#region Methods 

        /// <summary>
        /// Get a list of all SSN Status values
        /// </summary>
        /// <returns></returns>
        public ArrayList SSNStatuses(long facilityNumber, string stateCode)
        {

            ArrayList statuses = new ArrayList();

            if ( null != stateCode )
            {

                foreach (SocialSecurityNumberStatus ssn in _SocialSecurityNumberStatuses)
                {

                    SocialSecurityNumberStatus ssnStatus = ssn;

                    if (! ( stateCode.ToUpper().Equals(FLORIDA_CODE) ) &&
                          ssnStatus.Description.ToUpper().Equals(NEWBORN_DESCRIPTION))
                    {

                        // Refactor
                        continue;

                    }//if
             
                    if (null != ssnStatus)
                    {
                        statuses.Add(ssnStatus);

                    }//if

                }//foreach

            }//if

            return statuses;

        }
        /// <summary>
        /// Get a list of all SSN Status values
        /// </summary>
        /// <returns></returns>
        public IList<SocialSecurityNumberStatus> SSNStatuses()
        {
            return _SocialSecurityNumberStatuses.Where( ssnStatus => null != ssnStatus ).ToList();
        }

        /// <summary>
        /// Get a list of valid SSN Status values for Guarantor
        /// </summary>
        /// <returns></returns>
        public ArrayList SSNStatusesForGuarantor(long facilityNumber, string stateCode)
        {

            ICollection statuses =
                this.SSNStatuses(facilityNumber, stateCode);
            ArrayList ssnStatuses = 
                new ArrayList();
            
            foreach (SocialSecurityNumberStatus socialSecurityNumberStatus in statuses)
            {
                if( socialSecurityNumberStatus != null &&
                     !string.IsNullOrEmpty( socialSecurityNumberStatus.Description ) )
                {

                    if( socialSecurityNumberStatus.Description.Trim().ToUpper() != NEWBORN_DESCRIPTION )
                    {

                        ssnStatuses.Add( socialSecurityNumberStatus );

                    }//if

                }//if

            }//foreach

            return ssnStatuses;

        }

        /// <summary>
        /// Gets SSN status
        /// </summary>
        /// <param name="facilityNumber">The facility number.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public SocialSecurityNumberStatus SSNStatusWith( long facilityNumber, string description )
        {

            long newID = GetDescriptionIndex( description ) ;

            SocialSecurityNumberStatus status =
                new SocialSecurityNumberStatus( newID,
                                                PersistentModel.NEW_VERSION,
                                                description);

            SocialSecurityNumberStatus result = status;

            return result;

        }

        /// <summary>
        /// Helper method to obtain the index into the array table 
        /// given the string description.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private long GetDescriptionIndex( string description )
        {
            
            long newID = 0 ;
            
            if( null != description )
            {
                foreach( SocialSecurityNumberStatus securityNumberStatus in _SocialSecurityNumberStatuses )
                {

                    if( description.ToUpper().Equals( securityNumberStatus.Description.ToUpper() ) )
                    {

                        newID = securityNumberStatus.Oid;
                        break;

                    }//if

                }//foreach

            }//if

            return newID;

        }

		#endregion Methods 

    }
}
