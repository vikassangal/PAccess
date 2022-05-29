using System;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.ADTEvents
{
    /// <summary>
    /// Summary description for ADTPBARMessage
    /// </summary>
    [Serializable]
    public class ADTPBARMessage : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
     

       
        #endregion

        #region Properties
       
       
        public ADTPatient ADTPatient
        {
            get
            {
                return i_ADTPatient;
            }
            set
            {
                i_ADTPatient = value;
            }
        }
        public ADTGuarantor ADTGuarantor
        {
            get
            {
                return i_ADTGuarantor;
            }
            set
            {
                i_ADTGuarantor = value;
            }
        }
        public ADTEmployer ADTEmployer
        {
            get
            {
                return i_ADTEmployer;
            }
            set
            {
                i_ADTEmployer = value;
            }
        }

     
        #endregion

        #region Private Methods
   

        #endregion

        #region Construction and Finalization
      
        public ADTPBARMessage() : base()
        {
        }
        #endregion

        #region Data Elements
        private ADTPatient  i_ADTPatient = new ADTPatient();
        private ADTGuarantor  i_ADTGuarantor = new ADTGuarantor();
        private ADTEmployer  i_ADTEmployer = new ADTEmployer();

           

     
        #endregion

        #region Constants
        #endregion
    }
}
