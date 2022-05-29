using System;
using System.Collections;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for OrderedList.
	/// </summary>
	public class OrderedList : IList 
	{
		public OrderedList()
		{
		}

        public ArrayList Keys
        {
            get
            {
                return (ArrayList)this.Indexes.Clone();
            }
        }
        public ArrayList Values
        {
            get
            {
                return (ArrayList)this.aValues.Clone();
            }
        }

        #region IList Members
        private ArrayList Indexes = new ArrayList();
        private ArrayList aValues = new ArrayList();

        public bool IsReadOnly
        {
            get
            {
                // TODO:  Add OrderedList.IsReadOnly getter implementation
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                if( index < this.aValues.Count )
                {
                    return this.aValues[index];
            }
                return null;
            }
            set
            {
                if( index < this.Indexes.Count && index < this.aValues.Count )
                {
                    this.aValues[index] = value;
                }
                
            }
        }
        public object this[string index]
        {
            get
            {
                if( this.Indexes.Contains(index) )
                {
                    int i = this.Indexes.IndexOf(index);
                    return this.aValues[i];
                }
                return null;
            }
            set
            {
                if( this.Indexes.Contains(index) )
                {
                    int i = this.Indexes.IndexOf(index);
                    this.aValues[i] = value;
                }
                else
                {
                    throw new Exception("No entry exists with key: " + index);
                }
            }
        }

        public void RemoveAt(int index)
        {
            // TODO:  Add OrderedList.RemoveAt implementation
            throw new Exception("This method is not yet implemented");
        }

        public void Insert(int index, object value)
        {
            // TODO:  Add OrderedList.Insert implementation
            throw new Exception("This method is not yet implemented");
        }

        public void Remove(object value)
        {
            // TODO:  Add OrderedList.Remove implementation
            throw new Exception("This method is not yet implemented");
        }

        public bool Contains(object value)
        {
            // TODO:  Add OrderedList.Contains implementation
            if( Indexes.Contains(value) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            // TODO:  Add OrderedList.Clear implementation
            this.aValues.Clear();
            this.Indexes.Clear();
        }

        public int IndexOf(object value)
        {
            // TODO:  Add OrderedList.IndexOf implementation

            return this.Indexes.IndexOf(value);
        }

        public int Add(object value)
        {
            
            return 0;
        }
        public int Add(object key, object value)
        {
            if( this.Indexes.Contains(key) )
            {
                throw new Exception("OrderedList Already contains element of index: " + key);
            }
            this.Indexes.Add(key);
            this.aValues.Add(value);

            return this.Indexes.IndexOf(key);
        }

        public bool IsFixedSize
        {
            get
            {
                return this.aValues.IsFixedSize;
            }
        }

        #endregion

        #region ICollection Members

        public bool IsSynchronized
        {
            get
            {
                // TODO:  Add OrderedList.IsSynchronized getter implementation
                return false;
            }
        }

        public int Count
        {
            get
            {
                return this.Indexes.Count;
            }
        }

        public void CopyTo(Array array, int index)
        {
            // TODO:  Add OrderedList.CopyTo implementation
        }

        public object SyncRoot
        {
            get
            {
                // TODO:  Add OrderedList.SyncRoot getter implementation
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            // TODO:  Add OrderedList.GetEnumerator implementation
            return null;
        }

        #endregion
    }
}
