using System;
using System.Xml.Serialization;

namespace Peradigm.Framework.Domain.Parties
{
	[Serializable]
	public class Person : Party
	{
		#region Constants
		private const int FIRST_NAME = 0,
			LAST_NAME  = 1;
		private const string 
			COMMA          = ",",
			NAME_SEPARATOR = ", ",
			SPACE          = " ";
		private const char 
			COMMA_CHARACTER = ',',
			SPACE_CHARACTER  = ' ';
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		public override string ToString()
		{
            return this.FullName.ToCanonicalString();
		}
		#endregion

		#region Properties
		public string CanonicalName
		{
			get
			{
				return this.FirstName + SPACE + this.LastName;
			}
		}

        public string Degree
        {
            get
            {
                return this.FullName.Degree;
            }
            set
            {
                this.FullName.Degree = value;
            }
        }

        public string FirstName
		{
			get
			{
                return this.FullName.First;
			}
			set
			{
				this.FullName.First = value;
			}
		}

		public string LastName
		{
			get
			{
                return this.FullName.Last;
			}
			set
			{
				this.FullName.Last = value;
			}
		}

        public string MiddleName
        {
            get
            {
                return this.FullName.Middle;
            }
            set
            {
                this.FullName.Middle = value;
            }
        }

        override public string Name
		{
			get
			{
                return this.FullName.ToString();
			}
			set
			{
                this.FullName.Parse( value );
			}
		}

        public string Pedigree
        {
            get
            {
                return this.FullName.Pedigree;
            }
            set
            {
                this.FullName.Pedigree = value;
            }
        }

        [XmlIgnore()]
        public string Suffix
        {
            get
            {
                return this.FullName.Suffix;
            }
        }

        public string Title
        {
            get
            {
                return this.FullName.Title;
            }
            set
            {
                this.FullName.Title = value;
            }
        }
        #endregion

		#region Private Methods
		#endregion

		#region Private Properties
        private CanonicalName FullName
        {
            get
            {
                if( i_FullName == null )
                {
                    i_FullName = new CanonicalName();
                }
                return i_FullName;
            }
            set
            {
                i_FullName = value;
            }
        }
		#endregion

		#region Construction and Finalization
		//Required for Serialization
		public Person()
            : base()
		{
		}

        public Person( long oid, string fullName )
            : base( oid, NEW_VERSION, fullName )
        {
        }

		public Person( string firstName, string lastName )
			: this( NEW_OID, NEW_VERSION, firstName, lastName )
		{
        }

		public Person( long oid, 
			           byte[] version, 
			           string firstName, 
			           string lastName )
			: base( oid, version, firstName + SPACE + lastName )
		{
        }
		#endregion

		#region Data Elements
        private CanonicalName i_FullName;
        #endregion
	}
}
