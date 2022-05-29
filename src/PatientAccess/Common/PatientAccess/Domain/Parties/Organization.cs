using System;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class Organization : Party, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            Organization org = obj as Organization;
            bool result = (org.Name == this.Name 
                            && 
                            ( org.EmailAddress != null
                                && this.EmailAddress != null
                                && org.EmailAddress.ToString() ==  this.EmailAddress.ToString() ) 
                            &&
                           base.Equals( org ));
            return result;
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public new object Clone()
        {
            Organization org = new Organization();
            org.Name = (string) this.Name.Clone();
            org.EmailAddress = (EmailAddress) this.EmailAddress.Clone();

            foreach( Relationship rl in this.primRelationships )
            {
                org.primRelationships.Add( rl );
            }

            foreach( ContactPoint cp in this.primContactPoints )
            {
                org.primContactPoints.Add( cp );
            }
            return org;
        }
        #endregion

        #region Properties
        public string Name
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
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Organization()
            : base( NEW_OID, NEW_VERSION )
        {
        }

        public Organization( long oid, DateTime version )
            : base( oid, version )
        {
        }

        public Organization( long oid, DateTime version, string nameOfParty )
            : this( oid, version )
        {
            this.Name = nameOfParty;
        }
        #endregion

        #region Data Elements
        private string i_Name = String.Empty;
        #endregion

        #region Constants
        #endregion
    }
}
