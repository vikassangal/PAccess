using System;
using System.Reflection;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for Name.
    /// </summary>
    [Serializable]
    public class Name : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        public string AsFormattedName()
        {
            string formattedName = String.Empty;

            if( this.IsEmpty() )
            {
                formattedName = String.Empty;
            }
            else
            {
                formattedName = String.Format( 
                    "{0}, {1} {2}",
                    this.LastName.Trim(),
                    this.FirstName.Trim(),
                    this.MiddleInitial.Trim() );
            }
            
            return formattedName;
        }

        public string AsFormattedNameWithSuffix()
        {
            string formattedName = String.Empty;

            if( this.IsEmpty() )
            {
                formattedName = String.Empty;
            }
            else
                if (this.Suffix.Trim().Length == 0 )
            {
                formattedName = String.Format( 
                    "{0}, {1} {2}",
                    this.LastName,
                    this.FirstName,
                    this.MiddleInitial );
            }
            else
                if (this.Suffix.Trim().Length > 0 )
            {
                formattedName = String.Format( 
                    "{0}, {1} {2}, {3}",
                    this.LastName,
                    this.FirstName,
                    this.MiddleInitial ,
                    this.Suffix);
            }
                        
            return formattedName;
        }

        public override bool Equals( object obj )
        {

            Name rightSideValue = obj as Name;

            return this.Equals( rightSideValue );

        }

        public bool Equals( Name rightSideValue )
        {

            if( rightSideValue == null ) return false;

            return this.FirstName.Equals( rightSideValue.FirstName ) &&
                   this.MiddleInitial.Equals( rightSideValue.MiddleInitial ) &&
                   this.LastName.Equals( rightSideValue.LastName) &&
                   this.Suffix.Equals( rightSideValue.Suffix ) &&
                   this.TypeOfName.Equals( rightSideValue.TypeOfName );

        }

        public override int GetHashCode()
        {
            return this.FirstName.GetHashCode() ^
                   this.MiddleInitial.GetHashCode() ^
                   this.LastName.GetHashCode() ^
                   this.Suffix.GetHashCode() ^
                   this.TypeOfName.GetHashCode();
        }

        public bool IsEmpty()
        {
            return FirstName == String.Empty &&
                LastName == String.Empty &&
                MiddleInitial == String.Empty;
        }
        #endregion

        #region Properties
        public string FirstName 
        {
            get
            {
                if( i_FirstName != null )
                {
                    return i_FirstName.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {

                this.SetAndTrack<string>( ref this.i_FirstName, value ?? string.Empty, MethodBase.GetCurrentMethod() );

            }
        }

        public string LastName 
        {
            get
            {
                if( i_LastName != null )
                {
                    return i_LastName.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                
                this.i_LastName = value ?? String.Empty;

            }
        }

        public string MiddleInitial 
        {
            get
            {
                if( i_MiddleInitial != null )
                {
                    return i_MiddleInitial.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {

                string oldValue = this.i_MiddleInitial;

                this.i_MiddleInitial = value ?? String.Empty;

                if( !this.i_MiddleInitial.Equals( oldValue ) )
                    this.RaiseChangedEvent( "MiddleInitial", oldValue, this.i_MiddleInitial );

            }
        }

        public string Suffix
        {
            get
            {
                return i_Suffix.Trim();
            }
            set
            {
                i_Suffix = value;
            }
        }

        public bool IsConfidential
        {
            get
            {
                return i_IsConfidential;
            }
            set
            {
                i_IsConfidential = value;
            }
        }

        public TypeOfName TypeOfName
        {
            get
            {
                return i_TypeOfName;
            }
            private set
            {
                i_TypeOfName = value;
            }
        }

        public DateTime EntryDate
        {
            get
            {
                return i_EntryDate;
            }
            set
            {
                i_EntryDate = value;
            }
        }
        
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Name( long oid, DateTime version, string firstName, 
            string lastName, string middleInitial , string suffix )
            : base( oid, version )
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
            this.Suffix     = suffix;
        }
        public Name( long oid, DateTime version, string firstName,
            string lastName, string middleInitial )
            : base( oid, version )
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
        }

        public Name( string firstName, string lastName, string middleInitial )
            : base()
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
        }
        public Name( string firstName, string lastName, string middleInitial
            , string suffix )
            : base()
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
            this.Suffix = suffix ;
        }
        public Name( string firstName, string lastName, string middleInitial
            , string suffix , string isConfidential )
            : base()
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
            this.Suffix = suffix ;
            if( isConfidential.ToUpper() == "Y" )
            {
                this.IsConfidential = true;
            }
            else
            {
                 this.IsConfidential = false;
            }
        }

        public Name( string firstName, string lastName, string middleInitial
            , string suffix , TypeOfName typeOfName)
            : base()
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
            this.Suffix = suffix ;
            this.TypeOfName = typeOfName;            
        }

        public Name( string firstName, string lastName, string middleInitial
            , string suffix , string isConfidential, TypeOfName typeOfName )
            : base()
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
            this.Suffix = suffix ;
            this.TypeOfName = typeOfName;

            if( isConfidential.ToUpper() == "Y" )
            {
                this.IsConfidential = true;
            }
            else
            {
                this.IsConfidential = false;
            }
        }
        #endregion

        #region Data Elements
        private string i_FirstName  = String.Empty;
        private string i_LastName = String.Empty;
        private string i_MiddleInitial = String.Empty;
        private string i_Suffix = String.Empty;

        private bool i_IsConfidential;
        
        private DateTime i_EntryDate = DateTime.Now ;
        private TypeOfName i_TypeOfName;
        #endregion

        #region Constants
        #endregion
    }
}