using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public abstract class Condition : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// This method is called by all the Condition types that are derived from this Class
        ///  (Illness,Pregnancy, Accident,Crime) while setting their required Properties,
        ///   to generate OccurrenceCode for that Condition Type. 
        ///   This Method  Gets the Singleton Instance(ocm) of  OccurrenceCodeManager and
        ///   calls the GenerateOccurrenceCode() for this Condition.
        /// </summary>
        public void CallOccurrenceCodeManager()
        {
            OccurrenceCodeManager ocm = OccurrenceCodeManager.Instance;


            if (ocm.Account != null)
            {
                ocm.GenerateOccurrenceCode(this);
            }
        }
        public bool IsAccidentOrCrime()
        {
            return (GetType().Equals(typeof (Accident)) ||
                    GetType().Equals(typeof (Crime))
                   );
        }
        #endregion

        #region Properties
        /// <summary>
        ///   /// <summary>
        ///  Returns the <see cref="OccurrenceCode"/> for the Type Of Condition . 
        ///  Implemented by all Condition Types.  
        /// </summary>
        public abstract string OccurrenceCodeStr
        {
            get;
        }
        public abstract DateTime OccurredOn 
        {
            get ;
            set;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Condition()
        {
        }
        #endregion

        #region Data Elements
      
        #endregion

        #region Constants
        
        #endregion
    }
}

