using System;

namespace Extensions.PersistenceCommon
{
    [Serializable]
    public class ValueHolder : object
    {
        #region Event Handlers
        #endregion

        #region Methods

        public object GetValue()
        {
            if( this.i_Value == null )
            {
                i_Value = i_Loader.Load();
            }
            return i_Value;
        }

        public object GetValue( object o )
        {
            if( this.i_Value == null )
            {
                i_Value = i_Loader.Load(o);
            }
            return i_Value;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ValueHolder( IValueLoader loader )
        {
            this.i_Loader = loader;
        }
       
        #endregion

        #region Data Elements
        private object i_Value = null;
        private IValueLoader i_Loader = null;
        #endregion

        #region Constants
        #endregion
    }
}
