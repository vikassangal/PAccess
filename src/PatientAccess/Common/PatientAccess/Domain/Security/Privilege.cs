using System;

namespace Peradigm.Framework.Domain.Security
{
	/// <summary>
	/// Summary description for Privilege.
	/// </summary>
	//TODO: Create XML summary comment for Privilege
	[Serializable]
	public class Privilege
	{
        #region Constants
        [FlagsAttribute()]
        public enum Actions
        {
            Denied = 0,
            View   = 1,
            Add    = 2,
            Edit   = 4,
            Remove = 8
        }
        #endregion

		#region Event Handlers
		#endregion

		#region Methods
        /// <summary>
        /// Answer true if my action and context are equivelant to the
        /// specified privilege.
        /// </summary>
        /// <param name="rvalue">
        /// A privilege to which I am being compared.
        /// </param>
        /// <returns>
        /// True if both conditions are satisfied
        /// </returns>
        public override bool Equals( object rvalue )
        {
            Privilege privilege = (Privilege)rvalue;
            return this.Equals( privilege.Action, privilege.Context );
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">
        /// </param>
        /// <param name="context">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Equals( Actions action, object context )
        {
            return this.Action == action && 
                   this.Context.Equals( context );
        }
        #endregion

		#region Properties
        public Actions Action
        {
            get
            {
                return i_Action;
            }
            private set
            {
                i_Action = value;
            }
        }

        public object Context
        {
            get
            {
                return i_Context;
            }
            private set
            {
                i_Context = value;
            }
        }
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public Privilege( Actions action, object context )
		{
            this.Action  = action;
            this.Context = context;
		}
		#endregion

		#region Data Elements
        private Actions i_Action;
        private object  i_Context;
		#endregion
	}
}
