using System.Collections;
using IBM.Data.DB2.iSeries;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// This interface will implemented by all TransactionBrokers
    /// </summary>
	internal interface ITransactionBroker
	{
        ITransactionCoordinator TransactionCoordinator
        { 
            get; set;
        }
        ArrayList SqlBuilderStrategies
        {
            get;
            set;
        }
        TransactionKeys TransactionKeys
        {
            get;
            set;
        }
        void TestSQL( ArrayList sqlStatements, Facility facility, Account anAccount, bool IsTransactionHeaderRequired );
        void Execute( ArrayList sqlStatements, Facility facility, Account anAccount, bool IsTransactionHeaderRequired );
        void ExecuteTransaction(ITransactionCoordinator txnCoord);
        void ExecuteTransactionTest(ITransactionCoordinator txnCoord);
		
        bool IsNewTransaction(ArrayList sqlStatements);
        void SelectTransactionHeader( int numberOfInsurances, int numberOfNonInsurances, int numberOfOthers, Facility facility, ITransactionBroker broker );

        string LogTransactionHeaderNumbersFor(
            string facilityCode,
            int numberOfNonInsurances,
            int numberOfInsurances,
            int numberOfOthers,
            ITransactionBroker broker);

        iDB2Transaction NewTransaction(Facility facility);
	}
}
