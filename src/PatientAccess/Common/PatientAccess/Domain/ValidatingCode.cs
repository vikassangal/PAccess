using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ValidatingCode.
    /// </summary>
    [Serializable]
    public class ValidatingCode : ReferenceValue
    {

		#region Fields 

        private bool i_IsValid = true;

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingCode"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="version">The version.</param>
        /// <param name="description">The description.</param>
        public ValidatingCode( long oid, DateTime version, string description ) :
            base( oid, version, description ){}

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingCode"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="description">The description.</param>
        /// <param name="version">The version.</param>
        public ValidatingCode( long oid , string description, DateTime version ) :
            base( oid, version, description ){}

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingCode"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="description">The description.</param>
        public ValidatingCode( long oid, string description ) :
            base( oid, description ){}

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {

            get
            {

                return this.i_IsValid;

            }
            set
            {

                bool oldValue = this.i_IsValid;

                this.i_IsValid = value;

                if( !this.i_IsValid.Equals( oldValue ) )
                    this.RaiseChangedEvent( "IsValid", oldValue, value );

            }

        }

		#endregion Properties 

    }//class

}//namespace