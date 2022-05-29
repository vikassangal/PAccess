//-----------------------------------------------------------------------------
// Copyright © 2003-2005 Perot Systems Coproration. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections;

namespace Peradigm.Framework.Persistence
{
    /// <summary>
    /// ReferenceValue is a special type of PersistentModel designed to incorporate lookup data of "Code-Description" type.
    /// Unique ID of PeristentModel is considered code, and Description property is added.
    /// This class is useful in place of traditional enumerations because it provides ways to add more lookup types without
    /// having to recompile solutions.
    /// </summary>
    [Serializable]
    public abstract class ReferenceValue : PersistentModel
    {
        #region Constants
		private const string FORMAT_COMPOSITE_KEY = "{0}.{1}";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.Description.ToString();
        }

        public virtual bool Equals( ReferenceValue aReferenceValue )
        {
			if( aReferenceValue != null )
			{
				return aReferenceValue.Oid == this.Oid && aReferenceValue.Description == this.Description;
			}
			return base.Equals( aReferenceValue );
        }

		public virtual bool Equals( long anOid )
		{
			return anOid == this.Oid;
		}

        public override bool Equals( object obj )
        {
            ReferenceValue aReferenceValue = obj as ReferenceValue;
            if( aReferenceValue != null )
            {
                return this.Equals( aReferenceValue );
            }
            return base.Equals( obj );
        }

        public override int GetHashCode()
        {
            string compositeKey = String.Format( FORMAT_COMPOSITE_KEY, this.Oid, this.Description );
			return compositeKey.GetHashCode();
        }

        #endregion

        #region Properties
        public string Description
        {
            get
            {
                return i_Description;
            }
            set
            {
                i_Description = value;
				if( value == null )
				{
					i_Description = String.Empty;
				}
            }
        }

        public DictionaryEntry AsDictionaryEntry
        {
            get
            {
                return new DictionaryEntry( this.Oid, this.Description );
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReferenceValue( long oid, string description, byte[] version)
            : base( oid, version )
        {
            this.Description = description;
        }

        public ReferenceValue( long oid, string description )
            : this( oid, description, PersistentModel.NEW_VERSION )
        {
        }

        public ReferenceValue( string description )
            : this( PersistentModel.NEW_OID, description, PersistentModel.NEW_VERSION )
        {
        }

        public ReferenceValue( long oid )
            : this( oid, String.Empty , PersistentModel.NEW_VERSION )
        {
        }

        public ReferenceValue()
            : this( PersistentModel.NEW_OID, String.Empty, PersistentModel.NEW_VERSION )
        {
        }

        #endregion

        #region Data Elements
        private string i_Description;
        #endregion
    }
}
