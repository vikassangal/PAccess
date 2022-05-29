using System;
using System.Configuration;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    [Serializable]
    public class SSNFactory : ISsnFactory
    {
        #region Event Handlers
        #endregion

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
            var status =
                GetSSNStatusForUnvalidatedSSNUsing( unformattedSsn, state, admitDate, patientAge );
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
            var status = GetSSNStatusForUnvalidatedSSNUsing( unformattedSsn, state, admitDate );
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

            if ( state.IsFlorida )
            {
                ssn = GetFloridaSocialSecurityNumberForAdmitDate( admitDate, status );
            }

            else if ( state.IsCalifornia )
            {
                if ( status.IsNoneSSNStatus || status.IsUnknownSSNStatus )
                {
                    ssn = SocialSecurityNumber.NonFloridaNoneSSN;
                }

                else if ( status.IsNewbornSSNStatus )
                {
                    ssn = SocialSecurityNumber.NonFloridaNewbornOrUnknownSSN;
                }
            }

            else
            {
                if ( status.IsNoneSSNStatus )
                {
                    ssn = SocialSecurityNumber.NonFloridaNoneSSN;
                }

                else if ( status.IsUnknownSSNStatus || status.IsNewbornSSNStatus )
                {   // For Unknown or New born in any state other than FL. 
                    ssn = SocialSecurityNumber.NonFloridaNewbornOrUnknownSSN;
                }
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
                modelPerson.SocialSecurityNumber.SSNStatus = GetSSNStatusForUnvalidatedSSNUsing( modelPerson.SocialSecurityNumber.UnformattedSocialSecurityNumber, personState, admitDate );
            }
        }

        public bool IsKnownSSN( string ssn )
        {
            return ( !SocialSecurityNumber.IsFloridaNewbornOldSSN( ssn ) &&
                    !SocialSecurityNumber.IsFloridaNoneOldSSN( ssn ) &&
                    !SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn ) &&
                    !SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN( ssn ) &&
                    !SocialSecurityNumber.IsNonFloridaNoneSSN( ssn ) );
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
            if ( state.IsFlorida )
            {
                // SR 604 - May 2010 Release
                result = GetFloridaSSNValidityForAdmitDate( admitDate, status, ssn );
            }

            else if ( state.IsCalifornia )
            {
                if ( status.IsNewbornSSNStatus )
                {
                    result = ( SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN( ssn ) );
                }

                else if ( status.IsUnknownSSNStatus || status.IsNoneSSNStatus )
                {
                    result = ( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn ) );
                }
            }

            else
            {
                if ( status.IsNoneSSNStatus )
                {
                    result = ( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn ) );
                }

                else if ( status.IsUnknownSSNStatus )
                {
                    result = ( SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN( ssn ) );
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to get the general Social Security Number Status for a 
        /// given Social Security Number for states other than Florida and California.
        /// </summary>
        public static SocialSecurityNumberStatus GetGeneralSSNStatusFor( string ssn )
        {
            var ssnStatus = SocialSecurityNumberStatus.KnownSSNStatus;

            if ( SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            }

            else if ( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.NoneSSNStatus;
            }

            return ssnStatus;
        }

        public SocialSecurityNumberStatus GetSSNStatusForUnvalidatedSSNUsing( string ssn, State state, DateTime admitDate )
        {
            var ssnStatus = SocialSecurityNumberStatus.KnownSSNStatus;

            if ( state != null && state.IsFlorida )
            {
                // SR 604 - May 2010 Release
                ssnStatus = GetFloridaSocialSecurityNumberStatusForAdmitDate( admitDate, ssn );
            }
            // SR-44647 - July 2008 Release
            else if ( state != null && state.IsCalifornia )
            {
                if ( SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN( ssn ) )
                {
                    ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
                }

                else if ( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn ) )
                {
                    ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
                }
            }

            else
            {
                ssnStatus = GetGeneralSSNStatusFor( ssn );
            }

            return ssnStatus;
        }

        #endregion

        #region Properties
        private DateTime FloridaSSNRuleFeatureStartDate
        {
            get
            {
                if ( floridaSsnRuleFeatureStartDate.Equals( DateTime.MinValue ) )
                {
                    floridaSsnRuleFeatureStartDate =
                        DateTime.Parse( ConfigurationManager.AppSettings[FLORIDA_SSN_RULE_START_DATE] );
                }

                return floridaSsnRuleFeatureStartDate;
            }
        }

        private bool DoesFloridaNewSsnRuleApply( DateTime admitDate )
        {
            return ( admitDate == DateTime.MinValue ||
                     admitDate.Date >= FloridaSSNRuleFeatureStartDate );
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is used to determine the Social Security Number Status 
        /// corresponding to the incoming unvalidated Social Security Number, 
        /// for the given criteria of Admit Date, State and Patient Age.
        /// </summary>
        private SocialSecurityNumberStatus GetSSNStatusForUnvalidatedSSNUsing( string ssn, State state, DateTime admitDate, int patientAge )
        {
            var ssnStatus = SocialSecurityNumberStatus.KnownSSNStatus;

            if ( state != null && state.IsFlorida )
            {
                // SR 604 - May 2010 Release
                ssnStatus = GetFloridaSocialSecurityNumberStatusForAdmitDate( admitDate, ssn, patientAge );
            }
            // SR-44647 - July 2008 Release
            else if ( state != null && state.IsCalifornia )
            {

                if ( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn ) )
                {
                    ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
                }
            }

            else
            {
                ssnStatus = GetGeneralSSNStatusFor( ssn );
            }

            return ssnStatus;
        }

        private SocialSecurityNumber GetFloridaSocialSecurityNumberForAdmitDate(
            DateTime admitDate, SocialSecurityNumberStatus status )
        {
            var ssn = new SocialSecurityNumber();

            if ( DoesFloridaNewSsnRuleApply( admitDate ) )
            {
                if ( status.IsNewbornSSNStatus || status.IsNoneSSNStatus || status.IsUnknownSSNStatus )
                {
                    ssn = SocialSecurityNumber.FloridaUnknownNoneNewbornPost01012010SSN;
                }
            }

            else
            {
                switch ( status.Description.ToUpper() )
                {
                    case SocialSecurityNumberStatus.NEWBORN:
                        ssn = SocialSecurityNumber.FloridaNewbornSSN;
                        break;
                    case SocialSecurityNumberStatus.NONE:
                        ssn = SocialSecurityNumber.FloridaNoneSSN;
                        break;
                    case SocialSecurityNumberStatus.UNKNOWN:
                        ssn = SocialSecurityNumber.FloridaUnknownPre01012010SSN;
                        break;
                }
            }

            return ssn;
        }

        private bool GetFloridaSSNValidityForAdmitDate( DateTime admitDate, SocialSecurityNumberStatus status, string ssn )
        {
            var result = false;

            if ( DoesFloridaNewSsnRuleApply( admitDate ) )
            {
                if ( status.IsNewbornSSNStatus ||
                     status.IsNoneSSNStatus ||
                     status.IsUnknownSSNStatus )
                {
                    if ( SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010( ssn ) )
                        result = true;
                }
            }

            else
            {
                switch ( status.Description.ToUpper() )
                {
                    case SocialSecurityNumberStatus.NEWBORN:
                        if ( SocialSecurityNumber.IsFloridaNewbornOldSSN( ssn ) )
                            result = true;
                        break;

                    case SocialSecurityNumberStatus.NONE:
                        if ( SocialSecurityNumber.IsFloridaNoneOldSSN( ssn ) )
                            result = true;
                        break;

                    case SocialSecurityNumberStatus.UNKNOWN:
                        if ( SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn ) )
                            result = true;
                        break;
                }
            }

            return result;
        }

        private SocialSecurityNumberStatus GetFloridaSocialSecurityNumberStatusForAdmitDate( DateTime admitDate, string ssn, int patientAge )
        {
            SocialSecurityNumberStatus ssnStatus;

            if ( DoesFloridaNewSsnRuleApply( admitDate ) )
            {
                if ( SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010( ssn ) )
                {
                    if ( patientAge == 0 || patientAge == 1 )
                    {
                        ssnStatus = SocialSecurityNumberStatus.NewbornSSNStatus;
                    }

                    else
                    {
                        ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
                    }
                }

                else
                {
                    ssnStatus = GetPre01012010SocialSecurityNumberStatus( ssn );
                }
            }

            else
            {
                ssnStatus = GetPre01012010SocialSecurityNumberStatus( ssn );
            }

            return ssnStatus;
        }

        private SocialSecurityNumberStatus GetFloridaSocialSecurityNumberStatusForAdmitDate( DateTime admitDate, string ssn )
        {
            SocialSecurityNumberStatus ssnStatus;

            if ( DoesFloridaNewSsnRuleApply( admitDate ) )
            {
                if ( SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010( ssn ) )
                {
                    ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
                }
                else
                {
                    ssnStatus = GetPre01012010SocialSecurityNumberStatus( ssn );
                }
            }

            else
            {
                ssnStatus = GetPre01012010SocialSecurityNumberStatus( ssn );
            }

            return ssnStatus;
        }

        private static SocialSecurityNumberStatus GetPre01012010SocialSecurityNumberStatus( string ssn )
        {
            SocialSecurityNumberStatus ssnStatus;

            if ( SocialSecurityNumber.IsFloridaNewbornOldSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.NewbornSSNStatus;
            }

            else if ( SocialSecurityNumber.IsFloridaNoneOldSSN( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.NoneSSNStatus;
            }

            else if ( SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn ) )
            {
                ssnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            }

            else
            {
                ssnStatus = SocialSecurityNumberStatus.KnownSSNStatus;
            }

            return ssnStatus;
        }

        #endregion

        #region Data Elements

        private DateTime floridaSsnRuleFeatureStartDate = DateTime.MinValue;

        #endregion
 
        #region Constants

        private const string FLORIDA_SSN_RULE_START_DATE = "FLORIDA_SSN_RULE_START_DATE";

        #endregion
    }
}
