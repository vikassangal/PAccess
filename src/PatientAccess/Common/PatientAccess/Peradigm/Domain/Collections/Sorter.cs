//-----------------------------------------------------------------------------
// Copyright © 2003-2005 Perot Systems Coproration. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections;

namespace Peradigm.Framework.Domain.Collections
{
    //TODO:  Code Review - This should be plural.
	// ans : changing this to plural will break clients. can this be changed now?
    [Serializable]
    public enum SortOrder: int
    {
        Ascending  =  1,
        Descending = -1
    }

    /// <summary>
    /// Sorts a collection of items based on specified sort criteria (Sort descriptions).
    /// This can also be used as comparer to compare any two instances.
    /// </summary>
    [Serializable]
    public class Sorter: IComparer
    {
        #region Constants
        private const int   EQUIVALENT = 0;
        #endregion

        #region Methods

		/// <summary>
		/// Compares two objects (lhs, rhs) using <see cref="SortDescription"/> collection.
		/// </summary>
		/// <param name="lhs">Left hand side object for comparision</param>
		/// <param name="rhs">Right hand side object for comparision</param>
		/// <returns>int 0 = for equal, -1 for lhs less than rhs, 
		///	1 for lhs greater than rhs </returns>
        public int Compare( object lhs, object rhs )
        {
            int result = EQUIVALENT;

			SortOrder lastPropertySortOrder = SortDescription.DEFAULT_SORT_ORDER;
		    
		    foreach( SortDescription sortDesc in this.primSortDescriptions )
            {
				result = this.CompareObjectsWith( lhs, rhs, sortDesc );
			    
                if( result != EQUIVALENT )
                {
					lastPropertySortOrder = sortDesc.SortOrder;
                    break;
                }
            }			
            return result * (int)lastPropertySortOrder;
        }

        /// <summary>
        /// Add sort criteria (SortDescription containing Property, SortOrder) to Sorter.
        /// </summary>
        /// <param name="newSortDesc">SortDescription specifying Property and SortOrder</param>
        private void AddSortCriteria( SortDescription newSortDesc )
		{
			if( ! this.primSortDescriptions.Contains( newSortDesc ) )
			{
				this.primSortDescriptions.Add( newSortDesc );
			}
		}

		/// <summary>
		/// Removes specific SortDescription from Sorter to exclude it from sort operation.
		/// </summary>
		/// <param name="sortDesc">SortDescription to be removed from Sorter</param>
		public void RemoveSortCriteria( SortDescription sortDesc )
		{
			if( this.primSortDescriptions.Contains( sortDesc ) )
			{
				this.primSortDescriptions.Remove( sortDesc );
			}
		}
		
        #endregion

        #region Properties
		/// <summary>
		/// Collection of <see cref="SortDescription"/> classes.
		/// </summary>
		public ArrayList SortDescriptions
		{
			get
			{
				return (ArrayList)this.primSortDescriptions.Clone();
			}
		}
        #endregion

        #region Private Methods

		/// <summary>
		/// Compare objects lhs, rhs using SortDesciption class.
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <param name="sortDesc"></param>
		/// <returns></returns>
		private int CompareObjectsWith( object lhs, object rhs, SortDescription sortDesc )
		{
			IComparable propertylhs = null;
			IComparable propertyrhs = null;

			propertylhs = sortDesc.PropertyValueFrom( lhs );
			propertyrhs = sortDesc.PropertyValueFrom( rhs );

			
            return Comparer.Default.Compare( propertylhs, propertyrhs );
		}

		#endregion
		
        #region Private Properties
        
		/// <summary>
		/// Internal collection of SortDescription classes.
		/// </summary>
		private ArrayList primSortDescriptions
		{
			get
			{
				return this.i_SortDescriptions;
			}
			set
			{
				this.i_SortDescriptions = value;
			}
		}

        #endregion

        #region Construction and Finalization
        
	    public Sorter()
        {
			this.primSortDescriptions = new ArrayList();
        }

		public Sorter( SortDescription sortDescription )
			: this()
		{
			this.AddSortCriteria( sortDescription );
		}

        public Sorter( SortOrder order, string propertyOne )
            : this( order, propertyOne, null, null )
        {

        }

        public Sorter( SortOrder order,
                       string propertyOne,
                       string propertyTwo )
            : this( order, propertyOne, propertyTwo, null )
        {

        }

        public Sorter( SortOrder order, 
                       string propertyOne,
                       string propertyTwo,
                       string propertyThree ) 
            : this()
        {
			this.AddSortCriteria( new SortDescription( propertyOne, order ) );
			if( propertyTwo != null )
            {
				this.AddSortCriteria( new SortDescription( propertyTwo, order ) );
            }
            if( propertyThree != null )
            {
				this.AddSortCriteria( new SortDescription( propertyThree, order ) );
            }
	    }
        #endregion

        #region Data Elements
        private ArrayList i_SortDescriptions;
		#endregion
    }
}
