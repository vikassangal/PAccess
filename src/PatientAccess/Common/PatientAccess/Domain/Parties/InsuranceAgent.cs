using System;
using System.Collections;
using System.Reflection;

namespace PatientAccess.Domain.Parties
{
	/// <summary>
	/// Summary description for InsuranceAgent.
	/// </summary>
    [Serializable]
    public class InsuranceAgent : Person
	{
        #region Event Handlers
        #endregion

        #region Methods


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void AddContactPoint( ContactPoint aContactPoint )
        {
            if( !this.primContactPoints.Contains( aContactPoint ) )
            {
                this.primContactPoints.Add( aContactPoint );
            }
        }

        public override void RemoveContactPoint( ContactPoint aContactPoint )
        {
            if( this.primContactPoints.Contains( aContactPoint ) )
            {
                this.primContactPoints.Remove( aContactPoint );
            }
        }
        public override ICollection ContactPoints
        {
            get
            {
                return (ICollection)primContactPoints.Clone();
            }
        }
        #endregion

        #region Properties


        /// <summary>
        /// Name of the InsuranceAgent. The Name object that is derived from the Person  
        /// is being replaced here to  have a string form for Name instead of 
        /// the Name structure (LN,FN,MI).
        /// </summary>
        public string AgentName
        {
            get
            {
                return i_name;
            }
            set
            {
                if( value != null )
                {

                    this.SetAndTrack<string>( ref this.i_name, value, MethodBase.GetCurrentMethod() );
                }
                else
                {
                    base.Name.FirstName = string.Empty;
                }
                
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuranceAgent() : base()
		{
		}

        public InsuranceAgent( long oid, DateTime version ) 
            : base( oid, version )
        {
        }

        public InsuranceAgent( long oid, DateTime version, Name agentName ) 
            : base( oid, version, agentName )
        {
        }
        #endregion

        #region Data Elements
        private ArrayList i_AgentContactPoints = new ArrayList();
        private string i_name = string.Empty ;      
        #endregion

        #region Constants
        #endregion
    }
}
