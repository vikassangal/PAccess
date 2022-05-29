using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.PatientSearch
{
    public class PatientSelectedEventArgs :EventArgs
    {

        public string AccountNumber
        {
            get 
            { 
                return i_AccountNumber; 
            }
        }


        public AbstractPatient Patient
        {
            get 
            { 
                return i_Patient; 
            }
        }


        public PatientSelectedEventArgs( AbstractPatient aPatient ) : this( aPatient, "" )
        {

        }

        public PatientSelectedEventArgs( AbstractPatient aPatient, string accountNumber )
        {
            i_Patient = aPatient;
            i_AccountNumber = accountNumber;
        }
        
        private string i_AccountNumber = String.Empty;
        private AbstractPatient i_Patient = null;
    }
}