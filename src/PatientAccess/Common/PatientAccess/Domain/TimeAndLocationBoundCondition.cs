using System;

namespace PatientAccess.Domain
{
    [Serializable]
    
    public abstract class TimeAndLocationBoundCondition : Condition
    {
        #region Event Handlers
        #endregion

        #region Methods 
        public string GetOccurredHour()
        {
            var returnValue = OccurredAtHour;

            //we get time from PBAR in military format (e.g. '700' and '1100')
            //we get '9900' or '2400' for 'Unknown' 
            if ( OccurredAtHour == "9900" || OccurredAtHour == "2400" )
            {
                returnValue = "Unknown";
            }

            else switch ( returnValue.Length )
            {
                case 3:
                    returnValue = "0" + returnValue.Substring( 0, 1 );
                    break;
                
                case 4:
                    returnValue = returnValue.Substring( 0, 2 );
                    break;
                
                case 1:
                    returnValue = "0" + returnValue;
                    break;
            }

            return returnValue;
        }
        #endregion

        #region Properties
        
        /// <summary>
        /// Country where the Accident or the Crime Occured
        /// </summary>
        public Country Country 
        {
            get
            {
                return i_Country;
            }
            set
            {
                i_Country = value;
            }
        }
        /// <summary>
        /// Time at which the Accident or the Crime Occured 
        /// </summary>
        public string OccurredAtHour 
        {
            get
            {
                return i_OccurredAtHour;
            }
            set
            {
                i_OccurredAtHour = value;
            }
        }
        /// <summary>
        /// Date when the Accident or the Crime Occured. This triggers generation of OccurrenceCode for accident Or Crime 
        /// </summary>
        public override DateTime OccurredOn 
        {
            get
            {
                return i_OccurredOn;
            }
            set
            {
                i_OccurredOn = value;
                if (this.GetType() == typeof(Crime))
                {
                    this.CallOccurrenceCodeManager();
                }
                else
                    if (this.GetType() == typeof(Accident))
                {
                    Accident a = (Accident)this;
                    if(a.Kind != null ) 
                    {
                        this.CallOccurrenceCodeManager();
                       
                    }
                }
           
                
            }
        }
        /// <summary>
        /// State in which the Accident or Crime Occured 
        /// </summary>
        public State State 
        {
            get
            {
                return i_State;
            }
            set
            {
                i_State = value;
            }
        }

		public DateTime DateOfOnset 
		{
			get
			{
				return i_DateOfOnset;
			}
			set
			{
				i_DateOfOnset = value;
			}
		}
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TimeAndLocationBoundCondition()
        {

        }
        #endregion

        #region Data Elements
        private DateTime i_OccurredOn;
        private string i_OccurredAtHour = string.Empty;
        private Country i_Country = new Country();
        private State i_State = new State();
		private DateTime i_DateOfOnset;
        #endregion

        #region Constants
        #endregion
    }
}






