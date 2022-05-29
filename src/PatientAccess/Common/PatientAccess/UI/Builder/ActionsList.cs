using System;
using System.Collections;
using System.Xml.Serialization;

namespace Extensions.UI.Builder
{
    [Serializable]
    [XmlInclude(typeof(IAction))]
    public class ActionsList : IList, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public object Clone()
        {
            return this.i_primActions.Clone();
        }

        public bool Contains( IAction anAction )
        {
            return this.i_primActions.Contains( anAction );
        }

        public bool Contains( object value )
        {
            IAction action = value as IAction;
            if( action != null )
            {
                return this.Contains( action );
            }

            throw new ArgumentException( VALUE_NOT_IACTION_EXCEPTION, "value" );
        }

        public void RemoveAt( int index )
        {
            this.i_primActions.RemoveAt( index );
        }

        public void Insert( int index, object value )
        {
            IAction action = value as IAction;
            if( action != null )
            {
                this.Insert( index, value );
            }

            throw new ArgumentException( VALUE_NOT_IACTION_EXCEPTION, "value" );
        }
        
        public void Insert( int index, IAction value )
        {
            this.Insert( index, value );
        }

        public void Remove( object value )
        {
            IAction action = value as IAction;
            if( action != null )
            {
                this.Remove( action );
            }

            throw new ArgumentException( VALUE_NOT_IACTION_EXCEPTION, "value" );
        }

        public void Remove( IAction anAction )
        {
            this.i_primActions.Remove( anAction );
        }

        public void Clear()
        {
            this.i_primActions.Clear();
        }

        public int IndexOf( object value )
        {
            IAction action = value as IAction;
            if( action != null )
            {
                return this.IndexOf( action );
            }

            throw new ArgumentException( VALUE_NOT_IACTION_EXCEPTION, "value" );
        }

        private int IndexOf( IAction anAction )
        {
            return this.i_primActions.IndexOf( anAction );
        }

        public int Add( object value )
        {
            IAction action = value as IAction;
            if( action != null )
            {
                return this.Add( action );
            }

            throw new ArgumentException( VALUE_NOT_IACTION_EXCEPTION, "value" );
        }

        public int Add( IAction aIAction )
        {
            return this.i_primActions.Add( aIAction );
        }

        public IEnumerator GetEnumerator()
        {
            return this.i_primActions.GetEnumerator();
        }

        public void CopyTo( Array array, int index )
        {
            this.i_primActions.CopyTo(array, index);
        }

        public IAction ActionWith( long oid )
        {
            IAction foundAction = null;

            foreach( IAction action in this.i_primActions )
            {
                CompositeAction composite = action as CompositeAction;
                if( composite != null )
                {
                    foundAction = composite.ActionWith( oid );
                    if( foundAction != null )
                    {
                        break;
                    }
                }
            }

            return foundAction;
        }
        #endregion

        #region Properties
        public object this[int n]
        {
            get
            {
                if ( n < i_primActions.Count )
                    return i_primActions[n];
                return null;
            }
            set
            {
                this.i_primActions[n] = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.i_primActions.IsReadOnly;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.i_primActions.SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.i_primActions.IsSynchronized;
            }
        }

        public int Count
        {
            get
            {
                return this.i_primActions.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return this.i_primActions.IsFixedSize;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ActionsList()
        {
        }
        #endregion

        #region Data Elements
        private ArrayList i_primActions = new ArrayList();
        #endregion

        #region Constants
        private const string VALUE_NOT_IACTION_EXCEPTION = "The object supplied was not of type IAction";
        #endregion
    }
}
