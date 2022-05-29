using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ResistantOrganism.
    /// </summary>
    //TODO: Create XML summary comment for ResistantOrganism
    [Serializable]
    public class ResistantOrganism : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            if( Code.Trim().Length == 1 )
            {
                Code = Code + "    ";
            }
            else if( Code.Trim().Length == 2 )
            {
                Code = Code + "  ";
            }

            return String.Format("{0} {1}", Code, Description);
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ResistantOrganism()
        {
        }

        public ResistantOrganism( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public ResistantOrganism( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
      
        #endregion
    }
}
