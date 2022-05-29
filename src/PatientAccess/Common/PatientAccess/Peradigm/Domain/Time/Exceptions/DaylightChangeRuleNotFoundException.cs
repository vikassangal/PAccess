using System;
using System.Runtime.Serialization;
using Peradigm.Framework.Domain.Exceptions;

namespace Peradigm.Framework.Domain.Time.Exceptions
{
	//TODO: Create XML summary comment for DaylightChangeRuleNotFoundException
	[Serializable]
	public class DaylightChangeRuleNotFoundException : EnterpriseException
	{
		#region Constants

	    private const string
			DEFAULT_MESSAGE = "Daylight change rule not found.",
			CONTEXT_YEAR	= "Year";
		#endregion

		#region Methods
		#endregion

		#region Properties

	    private int Year
		{
			get
			{
				return i_Year;
			}
			set
			{
				i_Year = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public DaylightChangeRuleNotFoundException()
			: this( -1 )
		{
		}

		public DaylightChangeRuleNotFoundException( int year )
			: this( year, null )
		{
		}

		private DaylightChangeRuleNotFoundException( int year, Exception innerException )
			: this( year, innerException, Severity.Low )
		{
		}

		public DaylightChangeRuleNotFoundException( int year, Severity severity )
			: this( year, null, severity )
		{
		}
		
		private DaylightChangeRuleNotFoundException( int year, Exception innerException, Severity severity )
			: base( DEFAULT_MESSAGE, innerException, severity )
		{
			this.Year = year;
			this.AddContextItem( CONTEXT_YEAR, year.ToString() );
		}
		
		public DaylightChangeRuleNotFoundException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}
		#endregion

		#region Data Elements
		private int i_Year;
		#endregion
	}
}
