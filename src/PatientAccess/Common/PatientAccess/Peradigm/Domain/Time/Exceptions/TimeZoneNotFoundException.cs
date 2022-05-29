using System;
using System.Runtime.Serialization;
using Peradigm.Framework.Domain.Exceptions;

namespace Peradigm.Framework.Domain.Time.Exceptions
{
//TODO: Create XML summary comment for TimeZoneNotFoundException
    [Serializable]
    public class TimeZoneNotFoundException : EnterpriseException
    {
        #region Constants
        private const string 
            DEFAULT_MESSAGE = "Requested TimeZone could not be found.",
            CONTEXT_OBSERVES_DAYLIGHT_SAVINGS   = "ObservesDaylightSavings",
            CONTEXT_NAME                        = "Name";
        #endregion

        #region Methods
        #endregion

        #region Properties

        private string Name
        {
            get
            {
                return i_Name;
            }
            set
            {
                i_Name = value;
            }
        }

        private bool ObservesDaylightSavings
        {
            get
            {
                return i_ObservesDaylightSavings;
            }
            set
            {
                i_ObservesDaylightSavings = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TimeZoneNotFoundException()
			: this( String.Empty, false )
		{
		}

		private TimeZoneNotFoundException( string name, bool ObservesDaylightSavings )
			: this( name, ObservesDaylightSavings, null )
		{
		}

		private TimeZoneNotFoundException( string name, bool ObservesDaylightSavings, Exception innerException )
			: this( name, ObservesDaylightSavings, innerException, Severity.Low )
		{
		}

		public TimeZoneNotFoundException( string name, bool ObservesDaylightSavings, Severity severity )
			: this( name, ObservesDaylightSavings, null, severity )
		{
		}
		
		private TimeZoneNotFoundException( string name, bool ObservesDaylightSavings, Exception innerException, Severity severity )
			: base( DEFAULT_MESSAGE, innerException, severity )
		{
			this.Name = name;
            this.ObservesDaylightSavings = ObservesDaylightSavings;
			
            this.AddContextItem( CONTEXT_NAME, name );
            this.AddContextItem( CONTEXT_OBSERVES_DAYLIGHT_SAVINGS, ObservesDaylightSavings.ToString() );

		}
		
		public TimeZoneNotFoundException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}
        #endregion

        #region Data Elements
        private string i_Name;
        private bool i_ObservesDaylightSavings;
        #endregion
    }
}
