using System.Collections;
using System.Windows.Forms;
using PatientAccess.Domain;

namespace PatientAccess.UI.HelperClasses
{
    public class ReferenceValueComboBox : ComboBoxHelper
    {
        #region ComparerClass
        private class ReferenceValueComparer : IComparer
        {
            public int Compare( object x, object y)
            {
                long xOid = 0, yOid = 0;
                if( x is ReferenceValue )
                {
                    ReferenceValue xRef = x as ReferenceValue;
                    xOid = xRef.Oid;
                }

                if( y is ReferenceValue )
                {
                    ReferenceValue yRef = y as ReferenceValue;
                    yOid = yRef.Oid;
                }

                return xOid.CompareTo(yOid);
            }
        }
        #endregion

        #region Construction and Finalization
        public ReferenceValueComboBox( ComboBox comboBox ) 
            : base( comboBox, VALUE_NAME, MEMBER_NAME )
        {
            this.ItemComparer = new ReferenceValueComparer();
        }
        #endregion

        #region Constants
        private const string
            VALUE_NAME = "Code",
            MEMBER_NAME = "Description";
        #endregion
    }
}
