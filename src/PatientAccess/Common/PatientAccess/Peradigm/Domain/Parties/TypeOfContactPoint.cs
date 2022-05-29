using System;
using System.Collections;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Parties
{

    /// <summary>
    /// Type of Contact Point represents generalized enumeration for Home, Work, Primary, Secondary, etc.
    /// contact point types.
    /// Class keeps track of all instances, and only allows one instance per type description.  
    /// Foe example, there could be only one instance of Home type of contact point.
    /// To enforce this behavior, singleton patters is implemented, where 
    /// clients cannot construct instances directly. 
    /// Instead, they need to call TypeOfContactPoint.With() method.
    /// </summary>
    [Serializable]
    public class TypeOfContactPoint : ReferenceValue
    {
        #region Constants

        private const string
            HOME            = "Home",
            WORK            = "Work",
            PRIMARY         = "Primary",
            SECONDARY       = "Secondary",
            ALTERNATIVE     = "Alternative",
            EMERGENCY       = "Emergency",
            OTHER           = "Other";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public static TypeOfContactPoint With( string description )
        {
            return Register( description );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private static TypeOfContactPoint Register( string description )
        {
            if( description != null )
            {
                TypeOfContactPoint typeOfContactPoint = KnownTypesOfContactPoint[description] as TypeOfContactPoint;
                if( typeOfContactPoint == null )
                {
                    long oid = KnownTypesOfContactPoint.Count + 1;
                    typeOfContactPoint = new TypeOfContactPoint( oid, description );
                    lock( KnownTypesOfContactPoint.SyncRoot )
                    {
                        KnownTypesOfContactPoint[description] = typeOfContactPoint;
                    }
                }
                return typeOfContactPoint.Clone() as TypeOfContactPoint;
            }
            else
            {
                return new TypeOfContactPoint();
            }
        }
        #endregion

        #region Private Properties
        private static Hashtable KnownTypesOfContactPoint
        {
            get
            {
                return c_KnownTypesOfContactPoints;
            }
            set
            {
                c_KnownTypesOfContactPoints = value;
            }
        }

        #endregion

        #region Construction and Finalization
		protected TypeOfContactPoint( long oid, string description, byte[] version)
			: base( oid, description, version )
		{
		}

        private TypeOfContactPoint( long oid, string description )
			: base( oid, description )
		{
		}

        protected TypeOfContactPoint( string description )
			: base( description )
		{
		}

        protected TypeOfContactPoint( long oid )
			: base( oid )
		{
		}

		protected TypeOfContactPoint()
			: base()
		{
		}	

        static TypeOfContactPoint()
        {
            TypeOfContactPoint.KnownTypesOfContactPoint = new Hashtable();
            Register( HOME );
            Register( WORK );
            Register( PRIMARY );
            Register( SECONDARY );
            Register( ALTERNATIVE );
            Register( EMERGENCY );
            Register( OTHER );
        }
        #endregion

        #region Data Elements
        private static Hashtable c_KnownTypesOfContactPoints = null;
        #endregion
        
    }
}
