using System;
using System.Collections;

namespace PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for AbstractPatient.
    /// </summary>
    [Serializable]
    public abstract class AbstractPatient : Person
    {
        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        #endregion

        #region Methods
        /// <summary>
        /// abstract AsPatient method.
        /// </summary>
        public abstract Patient AsPatient();

        public void AddAlias( Name aName )
        {
            if( this.i_Names.Count == 0 )
            {
                Name realNamePlaceHolder = new Name( String.Empty, String.Empty, String.Empty, String.Empty );
                this.i_Names.Add( realNamePlaceHolder );
            }
            this.i_Names.Add( aName );
        }

        public void RemoveAlias( Name aName )
        {
            this.i_Names.Remove( aName );
        }

        public bool HasAliases()
        {
            return this.Aliases.Count > 0;
        }
      
        public void AddPreviousNameToAKA()
        {
            if( ( ( this.PreviousName != null ) && ( this.Name.FirstName != null ) && ( this.Name.LastName != null ) &&
                ( this.PreviousName.FirstName != null ) && ( this.PreviousName.LastName != null ) ) )
            {
                //When there is change in the saved previous name
                if( ( this.PreviousName.FirstName != this.Name.FirstName ) ||
                ( this.PreviousName.LastName != this.Name.LastName ) )
                {
                    // If the  new real name is same as one of the names in the alias list, Remove this alias name from the list.
                    if (this.AliasContainsName(this.Name))
                    {
                        this.RemoveAlias(this.Name);
                    }
                    // Add the previous saved name to the alias list, only if it is not already contained in the list. 
                    if( !this.AliasContainsName(this.PreviousName) )
                    {
                        this.AddAlias( this.PreviousName );
                    }
                }
                //When the name is reverted back to the saved previous name during the same session (before saving the transaction)
                else
                {
                    // Since the previous name is already contained in the list. Which is now same as the real name, 
                    // Remove the previous name from the list.
                    if( this.AliasContainsName(this.PreviousName) )
                    {
                        this.RemoveAlias( this.PreviousName );
                    }

                }
            }

        }
                
        #endregion

        #region Properties
        public override Name Name
        {
            get
            {
                Name name;
                if( this.i_Names.Count != 0 )
                {
                    name = (Name)this.i_Names[ INDEX_OF_REAL_NAME ];
                }
                else
                {
                    name = new Name(
                        String.Empty,
                        String.Empty,
                        String.Empty,
                        String.Empty
                        );
                }
                return name;
            }
            set
            {
                Name newName = value;

                if( value == null )
                {
                    newName = new Name(
                        String.Empty,
                        String.Empty,
                        String.Empty,
                        String.Empty
                        );
                }

                if( this.i_Names.Count == 0 )
                {
                    this.i_Names.Add( newName );
                }
                else
                {
                    Name realName = ( (Name)this.i_Names[INDEX_OF_REAL_NAME] );
                    Name oldValue = realName;

                    realName = newName;
                    if( realName != null && !realName.Equals( oldValue ) )
                        this.TrackChange( "Name", oldValue, newName );
                    
                }
            }
        }

        public ArrayList Aliases 
        {
            get
            {
                ArrayList aliases = (ArrayList)i_Names.Clone();
                if( aliases.Count > 0 )
                {
                    aliases.RemoveAt( INDEX_OF_REAL_NAME );
                }
                return aliases;
            }
        }

        public override string FirstName 
        {
            get
            {
                if( this.i_Names.Count != 0 )
                {
                    return ((Name)this.i_Names[ INDEX_OF_REAL_NAME ]).FirstName;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if( this.i_Names.Count == 0 )
                {
                    Name newName =
                        new Name( NEW_OID,
                                  NEW_VERSION,
                                  value,
                                  String.Empty,
                                  String.Empty,
                                  String.Empty );

                    this.i_Names.Add( newName );
                }
                else
                {

                    Name realName = ( (Name)this.i_Names[INDEX_OF_REAL_NAME] );
                    string oldValue = realName.FirstName;

                    realName.FirstName = value;
                    if( realName != null && !realName.FirstName.Equals( oldValue ) )
                        this.TrackChange( "FirstName", oldValue, value );                    
                    
                }
            }
        }


        public override string LastName 
        {
            get
            {
                if( this.i_Names.Count != 0 )
                {
                    return ((Name)this.i_Names[ INDEX_OF_REAL_NAME ]).LastName;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if( this.i_Names.Count == 0 )
                {

                    Name newName =
                        new Name( NEW_OID,
                                  NEW_VERSION,
                                  String.Empty,
                                  value,
                                  String.Empty,
                                  String.Empty );

                    this.i_Names.Add( newName );

                }
                else
                {

                    Name realName = ( (Name)this.i_Names[INDEX_OF_REAL_NAME] );
                    string oldValue = realName.LastName;

                    realName.LastName = value;
                    if( realName != null && !realName.LastName.Equals( oldValue ) )
                        this.TrackChange( "LastName", oldValue, value );

                }
            }
        }


        public string MiddleInitial
        {
            get
            {
                if( this.i_Names.Count != 0 )
                {
                    return ((Name)this.i_Names[ INDEX_OF_REAL_NAME ]).MiddleInitial;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if( this.i_Names.Count == 0 )
                {
                    this.i_Names.Add( 
                        new Name( NEW_OID, NEW_VERSION, String.Empty, String.Empty, value , String.Empty)
                        );
                }
                else
                {
                    ((Name)this.i_Names[ INDEX_OF_REAL_NAME ]).MiddleInitial = value;
                }
            }
        }
        public string Suffix
        {
            get
            {
                if( this.i_Names.Count != 0 )
                {
                    return ((Name)this.i_Names[ INDEX_OF_REAL_NAME ]).Suffix;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if( this.i_Names.Count == 0 )
                {
                    this.i_Names.Add( 
                        new Name( NEW_OID, NEW_VERSION, String.Empty, String.Empty, String.Empty, value )
                        );
                }
                else
                {
                    ((Name)this.i_Names[ INDEX_OF_REAL_NAME ]).Suffix = value;
                }
            }
        }

        public override string FormattedName
        {
            get
            {
                if( this.i_Names.Count > 0 )
                {
                    return ((Name)this.i_Names[ INDEX_OF_REAL_NAME ]).AsFormattedName();
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        public string AsFormattedNameWithSuffix
        {
            get
            {
                string formattedName = String.Empty;

                if (this.Name.IsEmpty())
                {
                    formattedName = String.Empty;
                }
                else if (this.Suffix.Trim().Length == 0)
                {
                    formattedName = String.Format(
                        "{0}, {1} {2}",
                        this.LastName,
                        this.FirstName,
                        this.MiddleInitial);
                }
                else if (this.Suffix.Trim().Length > 0)
                {
                    formattedName = String.Format(
                        "{0}, {1} {2}, {3}",
                        this.LastName,
                        this.FirstName,
                        this.MiddleInitial,
                        this.Suffix);
                }

                return formattedName;
            }
        }
        public MedicalGroupIPA MedicalGroupIPA { get; set; }

        public long MedicalRecordNumber 
        {
            get
            {
                return i_MedicalRecordNumber;
            }
            set
            {
                i_MedicalRecordNumber = value;
            }
        }


        public Facility Facility
        {
            get 
            {
                return i_Facility;
            }
            set
            {
                i_Facility = value;
            }
        }
        public Name PreviousName
        {
            get
            {
                return i_PreviousName;
            }
            set
            {
                i_PreviousName = value;
            }
        }
        #endregion

        #region Private Methods
        private bool AliasContainsName( Name name )
        {
            foreach( Name aliasName in this.Aliases )
            {
                if( ( aliasName.FirstName == name.FirstName ) &&
                   ( aliasName.LastName == name.LastName ) )
                {
                    return true;
                }
            }
            return false;

        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AbstractPatient( long oid, DateTime version, string firstName, string lastName, string middleInitial, long medicalRecordNumber, DateTime dateOfBirth, SocialSecurityNumber ssn, Gender sex, Facility facility )
            : base( oid, version )
        {
            this.FirstName              = firstName;
            this.LastName               = lastName;
            this.MiddleInitial          = middleInitial;
            this.MedicalRecordNumber    = medicalRecordNumber;
            this.SocialSecurityNumber   = ssn;
            this.DateOfBirth            = dateOfBirth;
            this.Sex                    = sex;
            this.Facility               = facility;
        }
        public AbstractPatient( long oid, DateTime version, string firstName, string lastName, string middleInitial,string suffix , long medicalRecordNumber, DateTime dateOfBirth, SocialSecurityNumber ssn, Gender sex, Facility facility )
            : base( oid, version )
        {
            this.FirstName              = firstName;
            this.LastName               = lastName;
            this.MiddleInitial          = middleInitial;
            this.Suffix                 = suffix ;
            this.MedicalRecordNumber    = medicalRecordNumber;
            this.SocialSecurityNumber   = ssn;
            this.DateOfBirth            = dateOfBirth;
            this.Sex                    = sex;
            this.Facility               = facility;
        }

        public AbstractPatient( long oid, DateTime version, Name patientsName, long medicalRecordNumber, DateTime dateOfBirth, SocialSecurityNumber ssn, Gender sex, Facility facility )
            : this( oid, version, patientsName.FirstName, patientsName.LastName, patientsName.MiddleInitial, medicalRecordNumber, dateOfBirth, ssn, sex, facility )
        {
            this.Suffix = patientsName.Suffix;
        }

        public AbstractPatient() : base()
        {
        }
        #endregion

        #region Data Elements
        private ArrayList               i_Names = new ArrayList();
        private long                    i_MedicalRecordNumber = 0L;
        private Facility                i_Facility = new Facility();
        private Name                    i_PreviousName = null;
        #endregion

        #region Constants
        private const int INDEX_OF_REAL_NAME  = 0;
        #endregion
    }
}
