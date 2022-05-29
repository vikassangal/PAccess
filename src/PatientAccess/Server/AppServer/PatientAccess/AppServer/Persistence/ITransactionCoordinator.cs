using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    internal interface ITransactionCoordinator
    {
        void Add( string aSqlStatement );
        /// <summary>
        /// This method creates the SQL statements using strategies to create insert statments
        /// It can be overridden if there are special cases that can not be handled like the
        /// Bed swap txn that uses 2 accounts.
        /// </summary>
        void CreateSQL();
        /// <summary>
        /// This methods provides a 'hook' to perform after txn things.
        /// </summary>
        void AfterTxn();
        void WriteFUSNotesForAccount();
        void InsertFinancialFUSNotesInto( Account account );
        /// <summary>
        /// InsertPrivateRoomConditionCode
        /// </summary>
        /// <param name="account"></param>
        void InsertPrivateRoomConditionCodeInto( Account account );
        Account Account { get;set;}
        Account AccountTwo { get;set;}
        SqlBuilderStrategy[] InsertStrategies { get;set;}
        ArrayList SqlStatements { get;}
        int NumberOfInsurances { get;set;}
        int NumberOfNonInsurances { get;set;}
        int NumberOfOtherRecs { get;set;}
        bool IsTransactionHeaderRequired { get;set;}
        User AppUser { get;set;}
        void ReOrderInsurances();
    }
}
