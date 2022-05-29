using System;
using PatientAccess.Annotations;

namespace PatientAccess.UI.CommonControls
{
    public class RequiredFieldItem : IComparable
    {
        #region Methods

        // sorting in ascending order
        public int CompareTo( object obj )
        {
            RequiredFieldItem requiredFieldItem = ( RequiredFieldItem )obj;
            return ToString().CompareTo( requiredFieldItem.ToString() );
        }

        public override string ToString()
        {
            return Tab + Field;
        }

        public RequiredFieldItem( string tab, string field )
        {
            Tab = tab;
            Field = field;
        }

        #endregion

        #region Properties

        /// <summary>
        /// This is made public because it is used for databinding
        /// </summary>
        /// <value>The tab.</value>
        [UsedImplicitly]
        public string Tab
        
        {
            get
            {
                return i_Tab;
            }

            set
            {
                i_Tab = value;
            }
        }

        /// <summary>
        /// This is made public because it is used for databinding
        /// </summary>
        /// <value>The field.</value>
        [UsedImplicitly]
        public string Field
        {
            get
            {
                return i_Field;
            }
            set
            {
                i_Field = value;
            }
        }

        #endregion

        #region Data Elements

        private string i_Tab;
        private string i_Field;

        #endregion
    }
}