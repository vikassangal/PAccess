using System;

namespace PatientAccess.Domain
{

    /// <summary>
    /// Summary description for CodedReferenceValue.
    /// </summary>
    [Serializable]
    public class CodedReferenceValue : ValidatingCode
    {

		#region Fields 

        private string i_Code = String.Empty;

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="description">The description.</param>
        /// <param name="code">The code.</param>
        /// <param name="version">The version.</param>
        protected CodedReferenceValue( long oid, string description, string code, DateTime version ) :
            base( oid, version ,  description)
        {

            this.Code = code;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="version">The version.</param>
        /// <param name="description">The description.</param>
        /// <param name="code">The code.</param>
        protected CodedReferenceValue( long oid , DateTime version , string description, string code ) :
            base( oid, version ,  description)
        {

            this.Code = code;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="version">The version.</param>
        /// <param name="description">The description.</param>
        protected CodedReferenceValue( long oid, DateTime version, string description ) :
            base( oid, version, description ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="description">The description.</param>
        /// <param name="code">The code.</param>
        protected CodedReferenceValue( long oid, string description,string code ):
            base( oid, description,  NEW_VERSION )
        {

            this.Code = code;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="code">The code.</param>
        protected CodedReferenceValue( string description, string code ) :
            base( NEW_OID, description , NEW_VERSION )
        {

             this.Code = code;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="description">The description.</param>
        public CodedReferenceValue( long oid, string description ) :
            base( oid, NEW_VERSION, description ){}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        /// <param name="oid">The oid.</param>
        protected CodedReferenceValue( long oid ) :
            base( oid,  String.Empty , NEW_VERSION ){}

        /// <summary>
        /// Initializes a new instance of the <see cref="CodedReferenceValue"/> class.
        /// </summary>
        protected CodedReferenceValue() :
            base( NEW_OID,  String.Empty, NEW_VERSION ){}

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public virtual string Code
        {

            get
            {

                return this.i_Code;

            }
            set
            {

                this.i_Code = value ?? String.Empty;

            }

        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Compare against another CodedReferenceValue and ensure
        /// the types are the same and that the values of the Code property
        /// are the same.  NOTE: This does not take into account the Oid, Version, Description
        /// or any other properties.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Equals( CodedReferenceValue value )
        {

            if( null == value )
            {

                return false;

            }//if
            
            return this.GetHashCode().Equals(value.GetHashCode());

        }

        /// <summary>
        /// Custom Equals
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override bool Equals( object anObject )
        {

            CodedReferenceValue rightHandOperand = anObject as CodedReferenceValue;

            if( null == rightHandOperand )
            {

                return false;

            }//if

            // Compare using the strongly typed Equals method.
            return this.Equals( rightHandOperand );

        }

        /// <summary>
        /// Combines the HashCode of the Code property and the HashCode of the type of the instance
        /// derived from CodedReferenceValue.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {

            return this.Code.GetHashCode() ^ this.GetType().Name.GetHashCode();

        }

        /// <summary>
        /// Returns a string with the Code and the Description seperated by a single "space".
        /// </summary>
        /// <returns></returns>
        public virtual string ToCodedString()
        {
            
            return String.Format( "{0} {1}", this.Code, this.Description );

        }

		#endregion Methods 

    }//class

}//namespace
