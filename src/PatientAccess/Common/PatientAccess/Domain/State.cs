using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class State : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return Description;
        }
        #endregion

        #region Properties

        public static State California
        {
            get
            {
                return new State( 0L, DateTime.Now, CALIFORNIA_DESCRIPTION, CALIFORNIA_CODE );
            }
        }

        public static State Florida
        {
            get
            {
                return new State( 0L, DateTime.Now, FLORIDA_DESCRIPTION, FLORIDA_CODE );
            }
        }

        public static State NonFloridaNonCalifornia
        {
            get
            {
                return new State( 0L, DateTime.Now, TEXAS_DESCRIPTION, TEXAS_CODE );
            }
        }

        public static State SouthCarolina
        {
            get
            {
                return new State(0L, DateTime.Now, SOUTHCAROLINA_DESCRIPTION, SOUTHCAROLINA_CODE);
            }
        }
        public bool IsCalifornia
        {
            get { return Code == CALIFORNIA_CODE; }
        }

        public bool IsFlorida
        {
            get { return Code == FLORIDA_CODE; }
        }

        public bool IsSouthCarolina
        {
            get { return Code == SOUTHCAROLINA_CODE; }
        }

        public bool IsNonFloridaNonCalifornia
        {
            get
            {
                return ( Code != FLORIDA_CODE &&
                         Code != CALIFORNIA_CODE );
            }
        }
		
        public string StateNumber { get;  set; }
		
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public State( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public State( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }

        public State(long oid, DateTime version, string description, string code, string stateNumber)
            : base(oid, version, description, code)
        {
            this.StateNumber = stateNumber;
        } 

        public State( long oid, string description, string code )
            : base( oid, description, code )
        {
        }

        public State( string code )
        {
            base.Code = code;
        }

        public State()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string
            TEXAS_CODE = "TX",
            FLORIDA_CODE = "FL",
            SOUTHCAROLINA_CODE = "SC";
        public const string
            CALIFORNIA_CODE = "CA";

        private const string
            TEXAS_DESCRIPTION       = "Texas",
            CALIFORNIA_DESCRIPTION  = "California",
            FLORIDA_DESCRIPTION     = "Florida",
            SOUTHCAROLINA_DESCRIPTION     = "South Carolina";
        #endregion
    }
}
