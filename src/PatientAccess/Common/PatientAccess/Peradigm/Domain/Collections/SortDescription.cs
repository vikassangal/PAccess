using System;
using System.Collections;
using System.Reflection;

namespace Peradigm.Framework.Domain.Collections
{
	/// <summary>
	/// Class to describe sort attributes such as Sort property, value and Sort order.
	/// </summary>
    [Serializable]
    public class SortDescription
	{
		#region Constants

		public const SortOrder DEFAULT_SORT_ORDER = SortOrder.Ascending;

		#endregion

		#region Methods

		/// <summary>
		/// Parses property and returns value of the property from instance passed in 
		/// as parameter.
		/// </summary>
		/// <param name="instance">instance from which property value needs 
		/// to be returned.</param>
		/// <returns></returns>
	   	public IComparable PropertyValueFrom( object instance )
		{
            //TODO:  Code Review - Why did we split the functionality into a separate method?
			// ans : we need to call a method to parse property recursively by passing the property name.
			return (IComparable) this.ParsePropertyValue( this.Property, instance );
		}

		#endregion

		#region Properties

		/// <summary>
		/// Property of the instance (Generally the column name). It can be nested property
		/// of inner objects.
		/// </summary>
		public string Property
		{
			get
			{
				return this.i_SortProperty;
			}
			set
			{
				this.SetSortOrderFor( value );
				this.i_SortProperty = value;
			}
		}

		/// <summary>
		/// Sort Order to indicate asc or desc when used by Sorter class.
		/// </summary>
		public SortOrder SortOrder
		{
			get
			{
				return this.i_SortOrder;
			}
			set
			{
				this.i_SortOrder = value;
			}
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// Answer the value associated with the given property name using reflection.  This
		/// method will traverse an object graph if given a complete path using "." property
		/// names.
		/// </summary>
		/// <param name="property"> String containing a property name or a "." separated navigation path. </param>
		/// <param name="instance"> object from which property value to be read.</param>
		/// <returns></returns>
		private object ParsePropertyValue( string property, object instance )
		{
			object result = null;
            if( instance != null )
            {
                ArrayList twoPartPropertyList = new ArrayList( property.Split( ".".ToCharArray(), 2 ) );

			    string firstPartOfProperty = twoPartPropertyList[0].ToString();

                result = this.PropertyPartValueFrom( firstPartOfProperty, instance );

			    if( ( twoPartPropertyList.Count > 1 ) && ( result != null ) )
			    {
				    // recursively invoke parsing if property contains nested objects
				    string secondPartOfProperty = twoPartPropertyList[1].ToString();
				    result = this.ParsePropertyValue( secondPartOfProperty, result );
			    }
            }
            return result;
		}

		/// <summary>
		/// Uses reflection to find value of given property from given instance.
		/// </summary>
		/// <param name="propertyPart">Property name</param>
		/// <param name="instance">instance containing property value.</param>
		/// <returns>property value as an instance of object</returns>
		
        //TODO:  Code Review - Do operands in the comparison have to be elements in a collection?
		// ans : changed name of operand. it need not be an item of collection.
		private object PropertyPartValueFrom( string propertyPart, object instance )
		{
			Type itemType = instance.GetType();
			BindingFlags flags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

			return itemType.InvokeMember( propertyPart, 
											flags, 
											null, 
											instance,
											null );
		}

		/// <summary>
		/// Determines and sets SortOrder based on new property passed in.
		/// </summary>
		/// <param name="newSortProperty">new property</param>
		/// 
		//TODO:  Code Review - Why expect clients to do this instead of having this be
		// a transparent part of simply setting the sort property?  Using this interface,
		// I would confused why sometimes your object changed the sort order and sometimes
		// it didn't, which it says it does.
		// ans : changed this method to private and setter method of Property calls it to 
		// make toggling behaviour implicit.
		private void SetSortOrderFor( string newSortProperty )
		{
			string currentSortProperty = this.Property;
			SortOrder newSortOrder = DEFAULT_SORT_ORDER;

			if( this.NeedsSortOrderToggling( newSortProperty ) )
			{
				newSortOrder = this.ToggleCurrentSortOrder( this.SortOrder );
			}
			
			this.SortOrder = newSortOrder;
		}

		/// <summary>
		/// Determines if sort order needs to be toggled based on newProperty.
		/// </summary>
		/// <param name="newSortProperty">New SortProperty</param>
		/// <returns>true/false indicating if SortOrder needs to be toggled.</returns>
		private bool NeedsSortOrderToggling( string newSortProperty )
		{
			return ( this.Property == newSortProperty );
		}

		/// <summary>
		/// Toggles current sort order and returns the alternate sort order.
		/// </summary>
		/// <returns>toggled SortOrder</returns>
		private SortOrder ToggleCurrentSortOrder( SortOrder currentSortOrder )
		{
			return (SortOrder)((int)currentSortOrder * (int)SortOrder.Descending);
		}
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		
		public SortDescription( string sortProperty ) : this( sortProperty, DEFAULT_SORT_ORDER )
		{
		}

		public SortDescription( string sortProperty, SortOrder sortOrder )
		{
			this.i_SortProperty = sortProperty;
			this.i_SortOrder = sortOrder;
		}
		
		#endregion

		#region Data Elements
		private string i_SortProperty;
		private SortOrder i_SortOrder;
		#endregion
	}
}
