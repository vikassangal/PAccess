using System;

namespace Peradigm.Framework.Domain.Caching
{
	/// <summary>
	/// Policy that governs how cache elements are stored in cache
	/// </summary>
	[Serializable]
	public abstract class AbstractCachingPolicy 
	{
		#region Event Handlers
		#endregion

		#region Methods
		internal abstract bool Execute( CacheElement ce );
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		#endregion

		#region Data Elements
		#endregion
	}
}
