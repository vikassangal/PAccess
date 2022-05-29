using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class County : CodedReferenceValue , ICloneable
    {
        private string stateCode = string.Empty;

        #region Event Handlers
        #endregion

        #region Methods
        public override object Clone()
        {
            County newObject = new County();
            newObject.Code = (string) this.Code.Clone();
            newObject.Description = (string) this.Description.Clone();
            newObject.stateCode = (string)StateCode.Clone();
            newObject.Oid = this.Oid;
            return newObject;
        }

        public override string ToString()
        {
            return this.Code + " " +  this.Description;
        }

        private bool Equals(County other)
        {
            return base.Equals(other) && string.Equals(stateCode, other.stateCode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((County) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (stateCode != null ? stateCode.GetHashCode() : 0);
            }
        }

        #endregion

        #region Properties
		
        public string StateCode
        {
            get { return stateCode; }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public County(string code, string description)
            : base (NEW_OID, NEW_VERSION, description)
        {
            base.Code = code;
        }

        public County( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public County( long oid, string description, string code )
            : base( oid, description, code )
        {
        }

        public County( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        public County(long oid, DateTime version, string description, string code, string stateCode)
            : base(oid, version, description, code)
        {
            this.stateCode = stateCode;
        }

        public County()
            : this( NEW_OID, NEW_VERSION, String.Empty, String.Empty )
        {
        }

        public County( string code )
        {
            base.Code = code;
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
