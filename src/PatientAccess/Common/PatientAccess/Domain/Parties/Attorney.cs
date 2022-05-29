using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace PatientAccess.Domain.Parties
{
	/// <summary>
	/// Summary description for Attorney.
	/// </summary>
    [Serializable]
    public class Attorney : Person
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
        /// Name of the Attorney. The Name object that is derived from the Person  
        /// is being replaced here to  have a string form for Name instead of 
        /// the Name structure (LN,FN,MI).
        /// </summary>
        public string AttorneyName
        {
            get
            {
                return i_name;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_name, value, MethodBase.GetCurrentMethod() );

                if( i_name == string.Empty )
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
        public Attorney() : base()
		{
		}

        public Attorney( long oid, DateTime version ) 
            : base( oid, version )
        {
        }

        public Attorney( long oid, DateTime version, Name attorneyName ) 
            : base( oid, version, attorneyName )
        {
        }
        #endregion

        #region Data Elements
        private ArrayList i_AttorneyContactPoints = new ArrayList();
        private string i_name = string.Empty ;      
        #endregion

        #region Constants
        #endregion
    }
}