using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class FinancialClassesBrokerProxy : AbstractBrokerProxy, IFinancialClassesBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        
        public ICollection AllTypesOfFinancialClasses( long facilityID )
        {
            var cacheKey = "FINANCIAL_CLASSES_BROKER_PROXY_ALL_TYPES_OF_FINANCIAL_CLASSES" + "_AND_FACILITY_" + facilityID;
            ICollection allTypesOfFinancialClasses = (ICollection)this.Cache[cacheKey];

            if( allTypesOfFinancialClasses == null )
            {
                lock (cacheKey)
                {
                    allTypesOfFinancialClasses = this.i_financialClassesBroker.AllTypesOfFinancialClasses( facilityID );
                    if (this.Cache[cacheKey] == null)
                    {
                        this.Cache.Insert(cacheKey, allTypesOfFinancialClasses);
                    }
                }
            }
            
            return allTypesOfFinancialClasses;
        }

        public FinancialClass FinancialClassWith( long oid )
        {
         return this.i_financialClassesBroker.FinancialClassWith( oid );
         }

        public FinancialClass FinancialClassWith( long facilityID, string code )
        {
            FinancialClass result = null;
            ICollection allFinancialClasses = this.AllTypesOfFinancialClasses( facilityID );
            foreach( FinancialClass financialClass in allFinancialClasses )
            {
                if( financialClass.Code.Equals( code ) )
                {
                    result = financialClass;
                    break;
                }
            }
            
            if( null == result )
            {
                result = this.i_financialClassesBroker.FinancialClassWith(facilityID, code );
            }

            return result;
        }

        public Hashtable FinancialClassesFor( long facilityID,long financialClassTypeID )
        {
            Hashtable allFinancialClassesByTypeId = this.AllFinancialClassesByTypeId;
            Hashtable financialClassesByCode = null;
            
            if( allFinancialClassesByTypeId[financialClassTypeID] == null )
            {
				// CodeReview: this should be changed to use a constant

                lock( c_financialClassesByTypeSyncRoot )
                {
                    if( allFinancialClassesByTypeId[financialClassTypeID] == null )
                    {
                        financialClassesByCode = this.i_financialClassesBroker.FinancialClassesFor(facilityID, financialClassTypeID );
                        allFinancialClassesByTypeId[financialClassTypeID] = financialClassesByCode;
                        this.Cache.Insert( FINANCIAL_CLASSES_BROKER_PROXY_FINANCIAL_CLASSES_BY_TYPE_HASHTABLE, allFinancialClassesByTypeId );
                    }
                    else
                    {
                        financialClassesByCode = (Hashtable)allFinancialClassesByTypeId[financialClassTypeID];
                    }
                }
            }
            else
            {
                financialClassesByCode = (Hashtable)allFinancialClassesByTypeId[financialClassTypeID];
            }

            return financialClassesByCode;
        }

        /// <summary>
        /// AllFinancialClassesByTypeId returns a Hashtable where:
        ///      key: financialClassTypeID, 
        ///      value: a Hashtable of FinancialClass(es) where:
        ///          key: a FinancialClass.Code
        ///          value: a FinancialClass
        /// </summary>
        private Hashtable AllFinancialClassesByTypeId
        {
            get
            {
                Hashtable financialClassesByTypeId = (Hashtable)this.Cache[FINANCIAL_CLASSES_BROKER_PROXY_FINANCIAL_CLASSES_BY_TYPE_HASHTABLE];

                if( financialClassesByTypeId == null )
                {
                    lock( FINANCIAL_CLASSES_BROKER_PROXY_FINANCIAL_CLASSES_BY_TYPE_HASHTABLE )
                    {
                        financialClassesByTypeId = new Hashtable();
                        if (this.Cache[FINANCIAL_CLASSES_BROKER_PROXY_FINANCIAL_CLASSES_BY_TYPE_HASHTABLE] == null)
                        {
                            this.Cache[FINANCIAL_CLASSES_BROKER_PROXY_FINANCIAL_CLASSES_BY_TYPE_HASHTABLE] = financialClassesByTypeId;
                        }
                    }
                }
                
                return financialClassesByTypeId;
            }
        }

	    public bool IsUninsured( long facilityID,FinancialClass aFinancialClass )
        {
            FinancialClassType uninsType = FinancialClassType.NewUninsuredFCType();
            Hashtable ht = this.FinancialClassesFor(facilityID, uninsType.Oid );

            return ht.Contains( aFinancialClass.Code );
        }

	
        public bool IsValidFinancialClass( FinancialClass aFinancialClass )
        {
            return this.i_financialClassesBroker.IsValidFinancialClass( aFinancialClass );
        }

        /// <summary>
        /// Checks if the given financial class contained in list of financial classes which 
        /// indicates that the patient is insured. UC162 - Pre-Condtion.
        /// </summary>
        public bool IsPatientInsured( FinancialClass financialClass )
        {
            return this.i_financialClassesBroker.IsPatientInsured( financialClass );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FinancialClassesBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IFinancialClassesBroker i_financialClassesBroker = BrokerFactory.BrokerOfType< IFinancialClassesBroker >() ;

		// CodeReview:  the following two objects should be deleted

        private static Object c_syncRoot = new Object();
        private static Object c_financialClassesByTypeSyncRoot = new Object();
        #endregion

        #region Constants
        private const string
          
            FINANCIAL_CLASSES_BROKER_PROXY_FINANCIAL_CLASSES_BY_TYPE_HASHTABLE = "FINANCIAL_CLASSES_BROKER_PROXY_FINANCIAL_CLASSES_BY_TYPE_HASHTABLE";
        #endregion
    }
}
