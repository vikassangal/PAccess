using System;
using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IDataValidationBroker.
    /// </summary>
    public interface IDataValidationBroker
    {
        // address

        AddressValidationResult ValidAddressesMatching(Address anAddress, string upn, string hspCode);
        
        //Compliance Checker

        bool SendAccountForComplianceCheck( AccountDetailsRequest accountRequest );

        // guarantor / credit

        DataValidationTicket InitiateGuarantorValidation( Guarantor aGuarantor, string upn, string hspCode, long accountNumber, long mrn );
        CreditValidationResponse GetCreditValidationResponse( string ticketId, string upn, string hspCode );

        // insurance benefits

        DataValidationTicket InitiateBenefitsValidation( AccountDetailsRequest accountDetailsRequest );
        BenefitsValidationResponse GetBenefitsValidationResponse( string ticketId, string upn, Type currentCoverageType );
        bool AreBenefitsValidationResultsAvailableFor( string ticketId, string userId, string hospitalCode );

        bool SendFUSInfo( DataValidationTicket aTicket, User aUser );

        // prior balance

        ArrayList GetPriorBalanceAccounts( PriorAccountsRequest request );

        // general

        void SaveResponseIndicator( string aTicketId, bool responseAvailable );

        DataValidationTicket GetDataValidationTicketFor( string aTicketId );
        DataValidationTicket GetDataValidationTicketFor( Account aAccount, DataValidationTicketType type );
        void SaveDataValidationTicket( DataValidationTicket aTicket );


    }
}
