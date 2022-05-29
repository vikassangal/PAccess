using System;
using Extensions;

namespace PatientAccess.Domain
{
    [Serializable]
    public class DriversLicense : Model, INullable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public static DriversLicense NewMissingDriversLicense()
        {
            return new MissingDriversLicense() as DriversLicense;
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            DriversLicense dl = obj as DriversLicense;
            return ( dl.Number == this.Number  
                && ( dl.State == null && this.State == null
                ||   ( dl.State != null
                        && this.State != null
                        && dl.State.Code == this.State.Code )));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties
        public virtual bool IsNull()
        {
            return false;
        }

        public string Number
        {
            get
            {
                return i_Number;
            }
            set
            {
                i_Number = value;
            }
        }

        public State State
        {
            get
            {
                return i_State;
            }
            set
            {
                i_State = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DriversLicense( string number, State state )
        {
            this.Number = number;
            if( state != null )
            {
                this.State = state;
            }            
        }

		public DriversLicense( string number )
		{
			this.Number = number;
		}
        #endregion

        #region Data Elements
        private string i_Number = String.Empty;
        private State i_State = new State();
        #endregion

        #region Constants
        #endregion
    }
}
