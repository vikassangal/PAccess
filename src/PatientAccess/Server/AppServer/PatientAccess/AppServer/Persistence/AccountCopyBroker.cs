using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence.AccountCopy;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for AccountCopyBroker.
	/// </summary>
    [Serializable]
    public class AccountCopyBroker : MarshalByRefObject, IAccountCopyBroker
	{

		#region Public Methods 

		public Account CreateAccountCopyFor( IAccount anAccount )
		{

			IAccountCopyStrategy result;

            switch( anAccount.Activity.GetType().Name )
            {
                case "PreMSERegisterActivity":
                    result = new PreMseAccountCopyStrategy();
                    break;
                case "UCCPreMSERegistrationActivity":
                    result = new PreMseAccountCopyStrategy();
                    break; 
                case "AdmitNewbornActivity":
                    result = new NewbornAccountCopyStrategy();
                    break;
                case "PreAdmitNewbornActivity":
                    result = new NewbornAccountCopyStrategy();
                    break;
                case "ShortRegistrationActivity":
                    result = new ShortRegistrationAccountCopyStrategy();
                    break;
                case "ShortPreRegistrationActivity":
                    result = new ShortRegistrationAccountCopyStrategy();
                    break;
                case "QuickAccountCreationActivity":
                    result = new QuickAccountCopyStrategy();
                    break;
                case "PAIWalkinOutpatientCreationActivity":
                    result = new WalkinAccountCopyStrategy();
                    break;
                default:
                    result = new BinaryCloneAccountCopyStrategy();
                    break;
            }

			return result.CopyAccount( anAccount );

		}

		#endregion Public Methods 

	}
}
