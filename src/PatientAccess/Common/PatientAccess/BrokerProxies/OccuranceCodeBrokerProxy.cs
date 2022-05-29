using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class OccuranceCodeBrokerProxy : AbstractBrokerProxy, IOccuranceCodeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        public ICollection AllOccurrenceCodes( long facilityID )
        {
            return new ArrayList( this.OccurrenceCodeCollection( facilityID ) );
        }

        public ICollection GetAccidentTypes( long facilityID )
        {
            return new ArrayList( this.AccidentTypeCollection( facilityID ) );
        }

        public ICollection AllSelectableOccurrenceCodes( long facilityID )
        {
            return new ArrayList( this.SelectableCodeCollection( facilityID ) );
        }

        public OccurrenceCode OccurrenceCodeWith( long facilityID, long occurrenceCodeID )
        {
			foreach ( OccurrenceCode oc in this.OccurrenceCodeCollection( facilityID ) )
			{
				if( oc.Oid == occurrenceCodeID )
				{
					return oc;
				}
			}

            return this.i_remoteOccurrenceCodeBroker.OccurrenceCodeWith( facilityID, occurrenceCodeID );
        }

        public OccurrenceCode OccurrenceCodeWith( long facilityID, string occurrenceCode )
        {
			foreach ( OccurrenceCode oc in this.OccurrenceCodeCollection( facilityID ) )
			{
				if( oc.Code == occurrenceCode )
				{
					return oc;
				}
			}

            return this.i_remoteOccurrenceCodeBroker.OccurrenceCodeWith( facilityID, occurrenceCode );
        }

        public TypeOfAccident AccidentTypeFor(long facilityID, OccurrenceCode occurrenceCode)
        {
            return this.i_remoteOccurrenceCodeBroker.AccidentTypeFor(facilityID, occurrenceCode);
        }
        public OccurrenceCode CreateOccurrenceCode(long facilityId, string occurrenceCode, long occurrenceDate)
        {
            return this.i_remoteOccurrenceCodeBroker.CreateOccurrenceCode(facilityId, occurrenceCode, occurrenceDate);
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ICollection AccidentTypeCollection( long facilityID )
        {
            ICollection aList = (ICollection)this.Cache[ACCIDENT_TYPE_COLLECTION];
            if( aList == null )
            {
                lock( ACCIDENT_TYPE_COLLECTION )
                {
                    aList = (ICollection)this.i_remoteOccurrenceCodeBroker.GetAccidentTypes( facilityID );
                    if( this.Cache[ACCIDENT_TYPE_COLLECTION] == null )
                    {
                        this.Cache.Insert( ACCIDENT_TYPE_COLLECTION, aList );
                    }
                }
            }
            return aList;
        }

        private ICollection OccurrenceCodeCollection( long facilityID )
        {
            var cacheKey = "ALL_OCCURRENCE_CODES_AND_FACILITY_" + facilityID;
            ICollection aList = null;
            if ((aList = (ICollection)Cache[cacheKey]) == null)
            {
                lock (cacheKey)
                {
                    aList = (ICollection)this.i_remoteOccurrenceCodeBroker.AllOccurrenceCodes( facilityID );
                    this.Cache.Insert(cacheKey, aList);
                }
            }
            return aList;
        }

        private ICollection SelectableCodeCollection( long facilityID )
        {
            ICollection aList = (ICollection)this.Cache[ALL_SELECTABLE_CODES];

            if( aList == null )
            {
                lock( ALL_SELECTABLE_CODES )
                {
                    aList = (ICollection)this.i_remoteOccurrenceCodeBroker.AllSelectableOccurrenceCodes( facilityID );
                    if( this.Cache[ALL_SELECTABLE_CODES] == null )
                    {
                        this.Cache.Insert( ALL_SELECTABLE_CODES, aList );
                    }
                }
            }
            return aList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OccuranceCodeBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IOccuranceCodeBroker i_remoteOccurrenceCodeBroker = BrokerFactory.BrokerOfType< IOccuranceCodeBroker >() ;            
        #endregion

        #region Constants
        private const string             
            ACCIDENT_TYPE_COLLECTION = "ACCIDENT_TYPE_COLLECTION",
			ALL_SELECTABLE_CODES = "ALL_SELECTABLE_CODES";
        #endregion
    }
}
