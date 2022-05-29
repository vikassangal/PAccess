using System;
using System.Collections.Generic;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for OnsetOfSymptomsOrIllnessWithNoAdmitDate.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class OnsetOfSymptomsOrIllnessWithNoAdmitDate : LeafRule
    {
        #region Events

        public event EventHandler OnsetOfSymptomsOrIllnessWithNoAdmitDateEvent ;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.OnsetOfSymptomsOrIllnessWithNoAdmitDateEvent = eventHandler ;
            return true ;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.OnsetOfSymptomsOrIllnessWithNoAdmitDateEvent -= eventHandler ;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.OnsetOfSymptomsOrIllnessWithNoAdmitDateEvent = null ;
        }

        public override bool ShouldStopProcessing()
        {
            return true ;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || !( context is Account ) )
            {
                return true;
            }

            Acct = context as Account ;
            if (Acct == null)
            {
                return false;
            }
            if ( ( AdmitDateNotEntered( Acct ) && DateOfOnSetEntered( Acct ) ) 
                || AdmitDatePrecedesOccurrenceCodeDate( Acct ) )
            {
                if ( this.FireEvents && OnsetOfSymptomsOrIllnessWithNoAdmitDateEvent != null )
                {
                    OnsetOfSymptomsOrIllnessWithNoAdmitDateEvent( this, null ) ;
                }
                return false;
            }

            return true ;
        }

        private bool AdmitDatePrecedesOccurrenceCodeDate( Account acct )
        {
            foreach ( OccurrenceCode occ in acct.OccurrenceCodes )
            {
                if ( occ.Code == "11" && occ.OccurrenceDate != DateTime.MinValue && AdmitDatePrecedesOccurrenceCodeDate( acct, occ.OccurrenceDate ) )
                {
                    return true ;
                }
            }
            return false;
        }

        public bool AdmitDatePrecedesOccurrenceCodeDate( Account acct, DateTime dt ) 
        {
            bool result = false ;
            if ( dt != null )
            {
                result = acct.AdmitDate < dt ;
            }
            return result ;
        }
        
        private bool AdmitDateNotEntered(Account acct)
        {
            return acct.AdmitDate.Equals(DateTime.MinValue);
        }

        private bool DateOfOnSetEntered(Account acct)
        {
            foreach( OccurrenceCode occ in acct.OccurrenceCodes )
            {
                if( occ.Code == "11" && occ.OccurrenceDate != DateTime.MinValue )
                {
                    return true ;
                }
            }
            return false ;
        }
        
        
        #endregion


        #region Properties

        private Account Acct
        {
            get
            {
                return i_Acct ;
            }
            set
            {
                i_Acct = value ;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OnsetOfSymptomsOrIllnessWithNoAdmitDate()
        {
            i_ListOfApplicableOccurrenceCodes.Add( "01" ) ;
            i_ListOfApplicableOccurrenceCodes.Add( "02" );
            i_ListOfApplicableOccurrenceCodes.Add( "03" );
            i_ListOfApplicableOccurrenceCodes.Add( "04" );
            i_ListOfApplicableOccurrenceCodes.Add( "05" );
            i_ListOfApplicableOccurrenceCodes.Add( "06" );
            i_ListOfApplicableOccurrenceCodes.Add( "11" );
        }
        #endregion

        #region Data Elements
        private Account i_Acct ;
        private List< string > i_ListOfApplicableOccurrenceCodes = new List< string >() ;
        #endregion

        #region Constants
        #endregion

    }
}
