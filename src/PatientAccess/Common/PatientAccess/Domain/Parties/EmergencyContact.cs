using System;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class EmergencyContact : Person
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Type of Relationship for this EmergencyContactPoint 
        /// </summary>
        public RelationshipType RelationshipType
        {
            get
            {
                return i_RelationshipType;
            }
            set
            {
                i_RelationshipType = value;
            }
        }
        /// <summary>
        /// Name of the EmergencyContactPoint. The Name object  that is derived from the Person  
        /// is being replaced here to  have a string form for Name instead of 
        /// the Name structure (LN,FN,MI).
        /// </summary>
        public new  string Name
        {
            get
            {
                return i_name;
            }
            set
            {
                i_name = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
       
        #endregion

        #region Construction and Finalization
        public EmergencyContact() : base()
        {
        }

        public EmergencyContact( long oid, DateTime version )
            : base( oid, version )
        {
        }

        public EmergencyContact( long oid, DateTime version, Name EmergencyContactsName )
            : base( oid, version, EmergencyContactsName )
        {
        }
        #endregion

        #region Data Elements
        private RelationshipType i_RelationshipType  = new RelationshipType();
        private string i_name = string.Empty ;      
        
        #endregion

        #region Constants
        
        #endregion
    }

}