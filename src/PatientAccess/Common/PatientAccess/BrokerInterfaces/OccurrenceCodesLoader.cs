using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class OccurrenceCodesLoader : IValueLoader
    {
        #region Event Handlers
        #endregion

        #region Methods
        // DODO: - Document Procedure
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Load()
        {
            IOccuranceCodeBroker broker = BrokerFactory.BrokerOfType< IOccuranceCodeBroker >() ;
            ArrayList list = (ArrayList)broker.AllOccurrenceCodes(User.GetCurrent().Facility.Oid);
            return list;
        }

        public object Load(object o)
        {
            Facility facility = null;
            if( o != null )
            {
                facility = o as Facility;
            }

            IOccuranceCodeBroker broker = BrokerFactory.BrokerOfType< IOccuranceCodeBroker >() ;
            ArrayList list = null;

            if( facility != null )
            {
                list = (ArrayList)broker.AllOccurrenceCodes( facility.Oid );
            }
            else
            {
                list = (ArrayList)broker.AllOccurrenceCodes( User.GetCurrent().Facility.Oid );
            }
            return list;
        }
        #endregion

        #region Properties
 
       
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
  
        public OccurrenceCodesLoader()
        {
        }
        #endregion

        #region Data Elements
       
     
        #endregion

        #region Constants
        #endregion
    }
}
