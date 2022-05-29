using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for Month.
    /// </summary>
    //TODO: Create XML summary comment for Month
    [Serializable]
    public class Month : CodedReferenceValue, IComparable 
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return String.Format("{0}", Description);
        }

        public int CompareTo( object obj )
        {
            if( obj is Month )
            {
                Month month = ( Month )obj;

                return this.MonthOrder.CompareTo( month.MonthOrder );
            }

            throw new ArgumentException( "Object is not a Month" );
        }
        #endregion

        #region Properties

        private int MonthOrder
        {
            get
            {
                return i_monthOrder;
            }
            set
            {
                i_monthOrder = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Month()
        {
        }

        private Month( string code )
        {
            base.Code = code;
        }

        private Month( string code, string description ) : this( code )
        {
            base.Description = description;
        }

        public Month( string code, string description, int monthOrder ) : this( code, description )
        {
            this.MonthOrder = monthOrder;
        }

        public Month( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Month( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        private int i_monthOrder;
        #endregion

        #region Constants
        #endregion
    }
}
