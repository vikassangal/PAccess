using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public abstract class ReferenceValue : PersistentModel
    {

		#region Fields 

        private string i_Description = String.Empty;

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceValue"/> class.
        /// </summary>
        public ReferenceValue() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="version">The version.</param>
        /// <param name="description">The description.</param>
        public ReferenceValue( long oid, DateTime version, string description ) :
            base( oid, version )
        {

            this.Description = description;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="description">The description.</param>
        public ReferenceValue( long oid, string description ) :
            this( oid, NEW_VERSION, description ){}

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="version">The version.</param>
        public ReferenceValue( long oid, DateTime version ) :
            base( oid, version ){}


		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public virtual string Description
        {
            get
            {

                return this.i_Description;

            }
            set
            {

                string oldValue = this.i_Description;

                this.i_Description = value ?? String.Empty;

                if( !this.i_Description.Equals( oldValue ) )
                    this.RaiseChangedEvent( "Description", oldValue, value );

            }

        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=( ReferenceValue obj1, ReferenceValue obj2 )
        {

            bool isEqual = ( obj1 == obj2 );

            return !isEqual;

        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==( ReferenceValue obj1, ReferenceValue obj2 )
        {

            bool isEqual;

            if (ReferenceEquals(obj1, null))
            {

                if (ReferenceEquals(obj2, null))
                {

                    isEqual = true;

                }
                else
                {

                    isEqual = false;

                }

            }
            else
            {

                if( ReferenceEquals( obj2, null ) )
                {

                    isEqual = false;

                }
                else
                {

                    isEqual = obj1.Equals( obj2 );
                   
                }

            }

            return isEqual;

        }

        /// <summary>
        /// Returns this instance as a dictionary entry.
        /// </summary>
        /// <returns></returns>
        public DictionaryEntry AsDictionaryEntry()
        {

            return new DictionaryEntry( this, this.Description );

        }

        /// <summary>
        /// Custom Equals
        /// </summary>
        /// <param name="anotherReferenceValue">Another reference value.</param>
        /// <returns></returns>
        public virtual bool Equals( ReferenceValue anotherReferenceValue )
        {

            return anotherReferenceValue.Description.ToUpper() == this.Description.ToUpper();

        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals( object obj )
        {

            ReferenceValue aReferenceValue = obj as ReferenceValue;

            if( aReferenceValue != null )
            {

                return this.Equals( aReferenceValue );

            }

            return base.Equals( obj );

        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            
            return Description.ToUpper().GetHashCode();

        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {

            return this.Description;

        }

		#endregion Methods 

    }//class

}//namespace
