/******************************************************************************
 * Extensions.Collections - Set class
 ******************************************************************************
 * Developers: Russ McClelland (russ.mcclelland@ps.net)
 *             Dmitry Frenkel  (dima.frenkel@ps.net)
 ******************************************************************************
 * Copyright (C) 2004, Perot Systems Corporation. All rights reserved.
 ******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Peradigm.Framework.Domain.Collections
{
	public delegate bool Selector( object element );

    [Serializable]
	public class Set : object, ICollection  
	{
		#region Constants
		private const long EMPTY_SET = 0;
		#endregion

		#region Event Handlers
		#endregion
		
		#region Operators
		public static Set operator + ( Set lhOperand, Set rhOperand )
		{
			if( lhOperand != null )
			{
				return lhOperand.Union( rhOperand );
			}
			else if( rhOperand != null )
			{
				return rhOperand.Union( lhOperand );
			}
			else
			{
				return new Set();
			}
		}
		public static Set operator - ( Set lhOperand, Set rhOperand )
		{
			if( lhOperand != null )
			{
				return lhOperand.Difference( rhOperand );
			}
			else
			{
				return new Set();
			}
		}
		public static Set operator * ( Set lhOperand, Set rhOperand )
		{
			if( lhOperand != null )
			{
				return lhOperand.Intersection( rhOperand );
			}
			else if( rhOperand != null )
			{
				return rhOperand.Intersection( lhOperand );
			}
			else
			{
				return new Set();
			}
		}
		#endregion

		#region Methods
		
		public Set Add( object element )
		{
			if( element != null )
			{
				this.primElements[ element ] = element;
			}
			return this;
		}

		public Set AddRange( ICollection elements )
		{
			return this.AddSet( new Set( elements ) );
		}

		public Set AddSet( Set setOfElements )
		{
			if( setOfElements != null )
			{
				foreach( object element in setOfElements )
				{
					this.Add( element );
				}
			}
			return this;
		}

		public ArrayList AsArrayList()
		{
			return new ArrayList( this );
		}

        public List<T> AsListOf<T>()
        {
            List<T> list = new List<T>();
            foreach( Object element in this )
            {
                if( element is T )
                {
                    list.Add( (T)element );
                }
            }
            return list;
        }

		public void Clear()
		{
			this.primElements.Clear();
		}

        private bool Contains( object element )
		{
			if( element != null )
			{
				return this.primElements.Contains( element );
			}
			else
			{
				return false;
			}
		}

		public bool ContainsSet( Set subSet )
		{
			bool containsAllElements = ( subSet != null );
			if( subSet != null )
			{
				foreach( object element in subSet )
				{
					containsAllElements = containsAllElements && this.Contains( element );
					if( !containsAllElements )
					{
						break;
					}
				}
			}
			return containsAllElements;
		}
		
		public void CopyTo( Array array, int index )
		{
			this.primElements.Keys.CopyTo( array, index );
		}

        private Set Difference( Set anotherSet )
		{
			Set result = new Set( this );
			result.RemoveSet( anotherSet );

			return result;
		}
		
		public object ElementLike( object sampleElement )
		{
			if( sampleElement != null )
			{
				return primElements[sampleElement];
			}
			else
			{
				return null;
			}
		}
		
		public override bool Equals( object obj )
		{
			Set anotherSet = obj as Set;
			if( anotherSet != null )
			{
				return this.ContainsSet( anotherSet ) && this.IsSubsetOf( anotherSet );
			}
			else
			{
				return false;
			}
		}
		
		public IEnumerator GetEnumerator()
		{
			return this.primElements.Keys.GetEnumerator();
		}

		
		public override int GetHashCode()
		{
			return primElements.GetHashCode();
		}

        private Set Intersection( Set anotherSet )
		{
			Set result = new Set();
			if( anotherSet != null )
			{
				foreach( object element in this )
				{
					if( anotherSet.Contains( element ) )
					{
						result.Add( element );
					}
				}
			}
			return result;
		}
		
		public bool IsEmpty()
		{
			return this.Count == EMPTY_SET;
		}

        private bool IsSubsetOf( Set superSet )
		{
			if( superSet != null )
			{
				return superSet.ContainsSet( this );
			}
			else
			{
				return false;
			}
		}
        
        public Set Reject( ISelector selector )
        {
            return this.primSelect( selector, false );
        }

        public Set Reject( Selector selector )
        {
            return this.primSelect( selector, false );
        }

		public Set Remove( object element )
		{
			if( element != null )
			{
				this.primElements.Remove( element );
			}
			return this;
		}

        private Set RemoveSet( Set setOfElements )
		{
			if( setOfElements != null )
			{
				foreach( object element in setOfElements )
				{
					this.Remove( element );
				}
			}
			return this;
		}

		public Set Select( ISelector selector )
		{
            return this.primSelect( selector, true );
		}

        public Set Select( Selector selector )
        {
            return this.primSelect( selector, true );
        }

        private Set Union( Set setOfElements )
		{
			Set result = new Set();
			result.AddSet( this );
			result.AddSet( setOfElements );

			return result;
		}

		#endregion

		#region Properties
		public object SyncRoot
		{
			get
			{
				return this.primElements.SyncRoot;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.primElements.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.primElements.Count;
			}
		}

		#endregion

		#region Private Methods
        private Set primSelect( ISelector selector, bool expectedResult )
        {
            Set result = new Set();
            if( selector != null )
            {
                foreach( object element in this )
                {
                    if( selector.MatchesCriteria( element ) == expectedResult )
                    {
                        result.Add( element );
                    }
                }
            }

            return result;
        }

        private Set primSelect( Selector selector, bool expectedResult )
        {
            Set result = new Set();
            if( selector != null )
            {
                foreach( object element in this )
                {
                    if( selector( element ) == expectedResult )
                    {
                        result.Add( element );
                    }
                }
            }

            return result;
        }

		#endregion

		#region Private Properties
        private Hashtable primElements
        {
            get
            {
                return i_primElements;
            }
            set
            {
                i_primElements = value;
            }
        }
        #endregion

		#region Construction and Finalization
		public Set()
		{
			this.primElements = new Hashtable();
		}

		
		public Set( ICollection elements )
			: this()
		{
			if( elements != null )
			{
				foreach( object element in elements )
				{
					this.Add( element );
				}
			}
		}
		#endregion

		#region Data Elements
		private Hashtable i_primElements;
		#endregion
	}
}
