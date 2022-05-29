using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// interface for FinancialClassesBroker and related methods
    /// </summary>
    public interface IFinancialClassesBroker
    {
        // Get All types of financial classes.
        ICollection AllTypesOfFinancialClasses( long facilityID );
        //Get  the financial class corresponding to the oid
        FinancialClass FinancialClassWith( long oid );
        //Get  the financial classes corresponding to the facility and type.
        Hashtable FinancialClassesFor( long facilityID,long financialClassTypeID );
        //Get  the financial class corresponding to the Code.
        FinancialClass FinancialClassWith( long facilityID, string code );
        //Checks  the financial is  Insured
        bool IsUninsured(  long facilityID,FinancialClass aFinancialClass );
        //validate financial class.
        bool IsValidFinancialClass( FinancialClass aFinancialClass );
        /// <summary>
        /// Checks if the given financial class contained in list of financial classes which 
        /// indicates that the patient is insured. UC162 - Pre-Condtion.
        /// </summary>
        bool IsPatientInsured( FinancialClass aFinancialClass );

    }
}
