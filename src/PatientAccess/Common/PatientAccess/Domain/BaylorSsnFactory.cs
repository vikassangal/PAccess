using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class BaylorSsnFactory : ISsnFactory
    {
        #region Methods

        /// <summary>
        /// This method is used to determine if the incoming Social Security Number 
        /// is valid for the criteria of given Admit Date, State and Patient Age.
        /// If valid, it returns the same Social Security Number back to the patient.
        /// If not, it determines the Social Security Number Status corresponding 
        /// to the incoming unvalidated Social Security Number, and gets the valid 
        /// Social Security Number for that particular Social Security Number Status.
        /// 
        /// Example: When an account with Newborn 000-00-0000 created after 01/01/2010 in Florida 
        /// is opened for Edit/Maintain after the SR604 Feature Release date, this SSN becomes 
        /// invalid and the method returns a valid 777-77-7777 SSN to be assigned to the patient.
        /// </summary>
        public SocialSecurityNumber GetValidatedSocialSecurityNumberUsing( SocialSecurityNumber oldSsn, State state, DateTime admitDate, int patientAge )
        {
            var unformattedSsn = oldSsn.UnformattedSocialSecurityNumber;
            var status = GetSSNStatusForUnvalidatedSSNUsing( unformattedSsn, patientAge );
            var validSsn = IsSSNValidForSSNStatus( unformattedSsn, status, state, admitDate );

            if ( !validSsn )
            {
                var ssn = GetValidSocialSecurityNumberFor( state, admitDate, status );
                return ssn;
            }

            oldSsn.SSNStatus = status;

            return oldSsn;
        }

        public SocialSecurityNumber GetValidatedSocialSecurityNumberUsing( SocialSecurityNumber oldSsn, State state, DateTime admitDate )
        {
            var unformattedSsn = oldSsn.UnformattedSocialSecurityNumber;
            var status = GetSSNStatusForUnvalidatedSSNUsing( unformattedSsn );
            var validSsn = IsSSNValidForSSNStatus( unformattedSsn, status, state, admitDate );

            if ( !validSsn )
            {
                var ssn = GetValidSocialSecurityNumberFor( state, admitDate, status );
                return ssn;
            }

            oldSsn.SSNStatus = status;
            return oldSsn;
        }

        /// <summary>
        /// This method is used to update the Social Security Number on a patient, 
        /// whose SSN Status is already selected/available, based on the Admit Date.
        /// 
        /// Example: When Admit Date is changed on an account.
        /// </summary>
        public SocialSecurityNumber GetValidSocialSecurityNumberFor( State state, DateTime admitDate, SocialSecurityNumberStatus status )
        {
            var ssn = new SocialSecurityNumber();

            if ( status.IsNoneSSNStatus )
            {
                ssn = SocialSecurityNumber.BaylorNoneNewbornSSN;
            }

            else if ( status.IsUnknownSSNStatus )
            {
                ssn = SocialSecurityNumber.BaylorUnknownSSN;
            }

            else if ( status.IsNewbornSSNStatus )
            {
                ssn = SocialSecurityNumber.BaylorNoneNewbornSSN;
            }

            else if ( status.IsRefusedSSNStatus )
            {
                ssn = SocialSecurityNumber.BaylorRefusedSSN;
            }

            ssn.SSNStatus = status;

            return ssn;
        }

        public void UpdateSsn( DateTime admitDate, SsnViewContext ssnContext, State personState, Person modelPerson )
        {
            if ( !modelPerson.SocialSecurityNumber.SSNStatus.IsKnownSSNStatus )
            {
                modelPerson.SocialSecurityNumber = GetValidSocialSecurityNumberFor( personState, admitDate, modelPerson.SocialSecurityNumber.SSNStatus );
            }

            if ( ( ssnContext == SsnViewContext.GuarantorView || ssnContext == SsnViewContext.ShortGuarantorView ) && modelPerson.SocialSecurityNumber.SSNStatus.IsNewbornSSNStatus )
            {
                modelPerson.SocialSecurityNumber.SSNStatus = GetSSNStatusForUnvalidatedSSNUsing( modelPerson.SocialSecurityNumber.UnformattedSocialSecurityNumber );
            }
        }

        public bool IsKnownSSN( string ssn )
        {
            return ( !SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn ) &&
                    !SocialSecurityNumber.IsBaylorUnknownSSN( ssn ) ) &&
                   !SocialSecurityNumber.IsBaylorRefusedSSN( ssn );
        }

        /// <summary>
        /// For a given Social Security Number and Social Security Number Status,
        /// this method determines if the SSN is valid for the SSN Status 
        /// for the given criteria of Admit Date and State.
        /// </summary>
        public bool IsSSNValidForSSNStatus( string ssn, SocialSecurityNumberStatus status, State state, DateTime admitDate )
        {
            if ( status.IsKnownSSNStatus )
            {
                return true;
            }

            var result = false;

            if ( status.IsUnknownSSNStatus )
            {
                result = ( SocialSecurityNumber.IsBaylorUnknownSSN( ssn ) );
            }

            else if ( status.IsNewbornSSNStatus || status.IsNoneSSNStatus )
            {
                result = ( SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn ) );
            }

            else if ( status.IsRefusedSSNStatus )
            {
                result = ( SocialSecurityNumber.IsBaylorRefusedSSN( ssn ) );
            }



            return result;
        }

        public SocialSecurityNumberStatus GetSSNStatusForUnvalidatedSSNUsing( string ssn )
        {
            var ssnStatus = SocialSecurityNumberStatus.KnownSSNStatus;

            if ( SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.NoneSSNStatus;
            }
            else if ( SocialSecurityNumber.IsBaylorUnknownSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            }
            else if ( SocialSecurityNumber.IsBaylorRefusedSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.RefusedSSNStatus;
            }


            return ssnStatus;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This method is used to determine the Social Security Number Status 
        /// corresponding to the incoming unvalidated Social Security Number, 
        /// for the given criteria of Admit Date, State and Patient Age.
        /// </summary>
        private SocialSecurityNumberStatus GetSSNStatusForUnvalidatedSSNUsing( string ssn, int patientAge )
        {
            var ssnStatus = SocialSecurityNumberStatus.KnownSSNStatus;

            if ( SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn ) )
            {
                if ( patientAge == 0 || patientAge == 1 )
                {
                    ssnStatus = SocialSecurityNumberStatus.NewbornSSNStatus;
                }

                else
                {
                    ssnStatus = SocialSecurityNumberStatus.NoneSSNStatus;
                }
            }

            else if ( SocialSecurityNumber.IsBaylorUnknownSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            }

            else if ( SocialSecurityNumber.IsBaylorRefusedSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.RefusedSSNStatus;
            }

            return ssnStatus;
        }

        #endregion
    }
}
