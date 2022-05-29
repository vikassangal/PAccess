using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{

    /// <summary>
    /// Summary description for AccidentOrCrimeDateWithNoAdmitDate.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AccidentOrCrimeDateWithNoAdmitDate : LeafRule
    {

        #region Events

        public event EventHandler AccidentOrCrimeDateWithNoAdmitDateEvent ;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// Registers the handler.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        public override bool RegisterHandler( EventHandler eventHandler )
        {

            this.AccidentOrCrimeDateWithNoAdmitDateEvent = eventHandler;
            
            return true;

        }


        /// <summary>
        /// Unregisters the handler.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        public override bool UnregisterHandler( EventHandler eventHandler )
        {

            this.AccidentOrCrimeDateWithNoAdmitDateEvent -= eventHandler;
            
            return true;

        }


        /// <summary>
        /// Unregisters the handlers.
        /// </summary>
        public override void UnregisterHandlers()
        {

            this.AccidentOrCrimeDateWithNoAdmitDateEvent = null;

        }


        /// <summary>
        /// Shoulds the stop processing.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldStopProcessing()
        {

            return true;

        }


        /// <summary>
        /// Applies to.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void ApplyTo( object context ){}


        /// <summary>
        /// Determines whether this instance [can be applied to] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can be applied to] the specified context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanBeAppliedTo( object context )
        {

            bool returnResult = 
                true;
            Account anAccount = 
                context as Account;

            if( null != anAccount )
            {

                if ( (IsAdmitDateMissingFor(anAccount) && IsAccidentCrimeDateEnteredFor(anAccount)) ||
                     IsAdmitDateBeforeOccurrenceCodeDateFor(anAccount) )
                {

                    if (this.FireEvents &&
                        (null != AccidentOrCrimeDateWithNoAdmitDateEvent))
                    {

                        AccidentOrCrimeDateWithNoAdmitDateEvent(this, null);

                    }//if

                    returnResult = false;

                }//if

            }//if

            return returnResult;

        }


        /// <summary>
        /// Determines whether [is admit date before occurrence code date] [the specified acct].
        /// </summary>
        /// <param name="acct">The acct.</param>
        /// <returns>
        /// 	<c>true</c> if [is admit date before occurrence code date] [the specified acct]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAdmitDateBeforeOccurrenceCodeDateFor( Account anAccount )
        {

            Accident accidentCondition =
                anAccount.Diagnosis.Condition as Accident;
            Crime crimeCondition =
                anAccount.Diagnosis.Condition as Crime;
            bool result = 
                false;
            
            if( ( null != accidentCondition ) &&
                ( null != accidentCondition.OccurredOn ) )
            {

                result = anAccount.AdmitDate < accidentCondition.OccurredOn;

            }
            else if( ( null != crimeCondition ) &&
                     ( null != crimeCondition.OccurredOn ) )
            {

                result = anAccount.AdmitDate < crimeCondition.OccurredOn;

            }

            return result;

        }


        /// <summary>
        /// Determines whether [is admit date missing] [the specified acct].
        /// </summary>
        /// <param name="acct">The acct.</param>
        /// <returns>
        /// 	<c>true</c> if [is admit date missing] [the specified acct]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAdmitDateMissingFor( Account anAccount )
        {

            return anAccount.AdmitDate.Equals( DateTime.MinValue );

        }


        /// <summary>
        /// Determines whether [is accident crime date entered] [the specified acct].
        /// </summary>
        /// <param name="acct">The acct.</param>
        /// <returns>
        /// 	<c>true</c> if [is accident crime date entered] [the specified acct]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAccidentCrimeDateEnteredFor( Account anAccount )
        {

            TimeAndLocationBoundCondition theCondition =
                anAccount.Diagnosis.Condition as TimeAndLocationBoundCondition;
            bool returnResult = 
                false;
            
            if( ( null != theCondition ) &&  
                ( theCondition.OccurredOn != DateTime.MinValue ) )
            {

                returnResult = true;

            }

            return returnResult;

        }

        #endregion

        #region Properties


        #endregion

        #region Private Methods

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AccidentOrCrimeDateWithNoAdmitDate()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion

    }

}
