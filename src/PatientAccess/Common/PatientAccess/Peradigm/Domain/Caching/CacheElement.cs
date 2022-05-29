using System;

namespace Peradigm.Framework.Domain.Caching
{
	//TODO: Create XML summary comment for CacheElement
	[Serializable]
	internal class CacheElement 
	{
		#region Event Handlers
		#endregion

		#region Methods
		public bool IsExpired()
		{
			return this.CachingPolicy.Execute( this );
		}
		#endregion

		#region Properties
		
		public DateTime TimeAdded
		{
			get
			{
				return i_TimeAdded;
			}
			set
			{
				i_TimeAdded = value;
			}
		}

		public DateTime TimeLastAccessed
		{
			get
			{
				return i_TimeLastAccessed;
			}
			set
			{
				i_TimeLastAccessed = value;
			}
		}

	    private AbstractCachingPolicy CachingPolicy
		{
			get
			{
				return i_CachingPolicy;
			}
			set
			{
				i_CachingPolicy = value;
			}
		}
		
		public object Value
		{
			get
			{
				return i_Value;
			}
		}
		
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public CacheElement( object value, AbstractCachingPolicy policy )
		{
			i_Value = value;
			this.CachingPolicy = policy;
		}
		#endregion

		#region Data Elements
		private object i_Value;
		private AbstractCachingPolicy i_CachingPolicy;
		private DateTime i_TimeLastAccessed;
		private DateTime i_TimeAdded;
		#endregion
	}
}
