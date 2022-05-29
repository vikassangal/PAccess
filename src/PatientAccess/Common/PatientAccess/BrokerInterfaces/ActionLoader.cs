using System;
using Extensions.PersistenceCommon;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for ActionLoader.
	/// </summary>
	//TODO: Create XML summary comment for ActionLoader
    [Serializable]
    public class ActionLoader : IValueLoader
    {
        #region Event Handlers
        #endregion

        #region Methods

        public object Load( object o )
        {
            return this.Load();
        }

        public object Load()
        {
            IWorklistBroker broker = BrokerFactory.BrokerOfType< IWorklistBroker >() ;
            return broker.RemainingActionsFor(this.i_Account.AccountNumber, 
                this.i_Account.Patient.MedicalRecordNumber, this.i_Account.Facility.Oid );
            //IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();
            //return broker.ActionsFor(this.i_Facility,this.AccountID);
        }
        #endregion

        #region Properties
        

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ActionLoader( IAccount account )
        {
            i_Account = account;
        }
        #endregion

        #region Data Elements
        private IAccount i_Account;
        #endregion

        #region Constants
        #endregion
    }
}
