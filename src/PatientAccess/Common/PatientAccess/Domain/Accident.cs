using System;

namespace PatientAccess.Domain
{
    [Serializable]

    public class Accident : TimeAndLocationBoundCondition
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Overridden from its Base class Condition. returns the <see cref="OccurrenceCode"/>
        /// for the <see cref="TypeOfAccident"/> 
        /// </summary>
        public override string OccurrenceCodeStr
        {
            get
            {
                if( this.Kind == null || this.Kind.OccurrenceCode == null  )
                {
                    return string.Empty ;
                }

                if(this.Kind.OccurrenceCode.Code != string.Empty)
                {
                    return  this.Kind.OccurrenceCode.Code ;
                }
                else
                    if(this.Kind.Code != string.Empty)
                {
                    return this.Kind.Code ;
                }
                else
                    return string.Empty ;

            }
        }
        /// <summary>
        /// Describes the Type of Accident.
        /// set accessor Calls the CallOccurrenceCodeManager() of the 
        /// Base class Condition to GenerateOccurrenceCodes() for this Accident Type 
        ///  </summary>
        ///  
        public TypeOfAccident Kind 
        {
            get
            {
                return i_Kind;
            }
            
            set
            {
                i_Kind = value;
                Accident a = (Accident)this;

                if( !a.OccurredOn.Equals(DateTime.MinValue))
                {
                    this.CallOccurrenceCodeManager();
                }
                
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Accident()
        {
        }
        #endregion

        #region Data Elements
        private TypeOfAccident i_Kind = new TypeOfAccident() ;
        
        #endregion

        #region Constants
        #endregion
    }
}

