using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Peradigm.Framework.Domain.Parties
{
    /// <summary>
    /// Name Parser is class that would parse people names.
    /// Based on this group:
    /// http://groups.google.com/group/microsoft.public.vb.general.discussion/browse_thread/thread/dd1b0c5fd419f8cd/a4cd258d95b4028c
    /// </summary>
    [Serializable]
    public class CanonicalName : Object
    {
        #region Constants
        private const int NOT_FOUND = -1;
        private readonly ArrayList KNOWN_TITLES = new ArrayList(
            new string[]
                {
                    "MR", "MR.", 
                    "MRS", "MRS.", 
                    "MS", "MS.", "MISS", 
                    "HON", "HON.", 
                    "SEN", "SEN.", "REP", "REP.",
                    "DR", "DR.", "DOCTOR",
                    "GENERAL", "GEN", "GEN.",
                    "MAJOR", "MAJ", "MAJ.",
                    "COLONEL", "COL", "COL.",
                    "LIEUTENANT", "LT", "LT.",
                    "SERGEANT", "SGT", "SGT.",
                    "SIR",
                    "MADAM",
                    "MAYOR",
                    "PRESIDENT"
                }
            );

        private readonly ArrayList KNOWN_PEDIGREES = new ArrayList(
            new string[]
                {
                    "JR",  "JR.", "JR,",  "JR.,",  
                    "SR",  "SR.", "SR,",  "SR.,",
                    "II",  "2ND", "II,",  "2ND,",
                    "III", "3RD", "III,", "3RD,",
                    "IV",  "4TH", "IV,",  "4TH,",
                    "V",   "5TH", "V,",   "5TH,"
                }
            );

        private readonly ArrayList KNOWN_DEGREES = new ArrayList(
            new string[]
                {
                    "ESQ", "ESQ.", 
                    "MD", "MD.", "M.D.",
                    "BS", "B.S.",
                    "PHD"
                }
            );

        private const string
            COMMA = ",",
            COMMA_WITH_SPACE = ", ",
            SPACE = " ";
        private readonly char[]
            SPACE_CHARACTER = new char[] { ' ' };
        private string PATTERN_FIRST_CHARACTER = "^[a-z]{1}";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public static CanonicalName Create( string name )
        {
            CanonicalName cn = new CanonicalName();
            cn.Parse( name );
            return cn;
        }

        public void Parse( string name )
        {
            this.Reset();
            if( !String.IsNullOrEmpty( name ) )
            {
                this.PrimParse(
                    name.Split( SPACE_CHARACTER, StringSplitOptions.RemoveEmptyEntries )
                    );
                this.Cleanup();
            }
        }

        public override string ToString()
        {
            string fullName =
                this.Title + SPACE +
                this.Last + COMMA_WITH_SPACE +
                this.First + SPACE +
                this.Middle + SPACE +
                this.Suffix;
            fullName = fullName.Replace( SPACE + SPACE, SPACE ).Trim();
            if( fullName.StartsWith( COMMA ) )
            {
                fullName = fullName.Remove( 0, COMMA.Length ).Trim();
            }
            return fullName;
        }

        public string ToCanonicalString()
        {
            string fullName =
                this.Title + SPACE +
                this.First + SPACE +
                this.Middle + SPACE +
                this.Last + SPACE +
                this.Suffix;
            return fullName.Replace( SPACE+SPACE, SPACE ).Trim();
        }

        #endregion

        #region Properties
        public string Title
        {
            get
            {
                return i_Title;
            }
            set
            {
                i_Title = (value == null) ? String.Empty : value;
            }
        }

        public string First
        {
            get
            {
                return i_First;
            }
            set
            {
                i_First = ( value == null ) ? String.Empty : value;
            }
        }

        public string Middle
        {
            get
            {
                return i_Middle;
            }
            set
            {
                i_Middle = ( value == null ) ? String.Empty : value;
            }
        }

        public string Last
        {
            get
            {
                return i_Last;
            }
            set
            {
                i_Last = ( value == null ) ? String.Empty : value; ;
            }
        }

        public string Pedigree
        {
            get
            {
                return i_Pedigree;
            }
            set
            {
                i_Pedigree = ( value == null ) ? String.Empty : value; ;
            }
        }

        public string Degree
        {
            get
            {
                return i_Degree;
            }
            set
            {
                i_Degree = ( value == null ) ? String.Empty : value; ;
            }
        }

        public string Suffix
        {
            get
            {
                return ( this.Pedigree + SPACE + this.Degree ).Trim();
            }
        }
	
        #endregion

        #region Private Methods
        private void Cleanup()
        {
            this.First = this.RemoveCommas( this.First );
            this.Last = this.RemoveCommas( this.Last );
            this.Middle = this.RemoveCommas( this.Middle );
            this.Title = this.RemoveCommas( this.Title );
            this.Pedigree = this.RemoveCommas( this.Pedigree );
            this.Degree = this.RemoveCommas( this.Degree );
        }

        private string ConsumeOne( string[] nameTokens, ref int index )
        {
            string result = String.Empty;
            if( nameTokens != null && index < nameTokens.Length )
            {
                result = nameTokens[index].Trim();
                index++;
            }
            return result;
        }

        private void PrimParse( string[] nameTokens )
        {
            if( nameTokens != null && nameTokens.Length > 0 )
            {
                int currentIndex = 0;
                String token = String.Empty;

                while( currentIndex < nameTokens.Length )
                {
                    if( this.IsTitleAtIndex( nameTokens, currentIndex ) )
                    {
                        this.Title = this.ConsumeOne( nameTokens, ref currentIndex );
                    }
                    this.First = this.ConsumeOne( nameTokens, ref currentIndex );

                    // Accumulate one or more middlenames, up to the last name ( possible with a suffix )
                    while( currentIndex < nameTokens.Length )
                    {
                        if( this.IsSuffixAtIndex( nameTokens, currentIndex ) )
                        {
                            this.Last = token;
                            while( this.IsSuffixAtIndex( nameTokens, currentIndex ) )
                            {
                                if( this.IsDegreeAtIndex( nameTokens, currentIndex ) )
                                {
                                    this.Degree = this.ConsumeOne( nameTokens, ref currentIndex );
                                }
                                if( this.IsPedigreeAtIndex( nameTokens, currentIndex ) )
                                {
                                    this.Pedigree = this.ConsumeOne( nameTokens, ref currentIndex );
                                }
                            }
                            break;
                        }
                        else
                        {
                            // Accumulate middlenames
                            if( token != String.Empty )
                            {
                                this.Middle +=
                                    ( ( this.Middle != String.Empty ) ? SPACE : String.Empty )
                                    +
                                    token;
                            }
                            token = this.ConsumeOne( nameTokens, ref currentIndex );
                        }
                    }

                    // If we haven't read as far as a suffix then token 
                    // will have a middlename or lastname in it
                    if( String.IsNullOrEmpty( this.Suffix ) )
                    {
                        this.Last = token;
                    }

                    //Switch out First Name and Last Name if FirstName ends with comma
                    if( !String.IsNullOrEmpty( this.First ) &&
                        this.First.IndexOf( COMMA ) != NOT_FOUND )
                    {
                        string swap = this.First;
                        this.First = this.Last;
                        this.Last = this.RemoveCommas( swap );
                        if( !String.IsNullOrEmpty( this.Middle ) )
                        {
                            swap = this.First;
                            this.First = this.Middle;
                            this.Middle = swap;
                        }
                    }
                    
                    //Check for any names elements that need moving from middle name to lastname
                    if( !String.IsNullOrEmpty( this.Last ) && 
                        !String.IsNullOrEmpty( this.Middle ) )
                    {
                        string[] middleTokens = this.Middle.Split( SPACE_CHARACTER, StringSplitOptions.RemoveEmptyEntries );
                        for( int i = middleTokens.Length - 1; i >= 0; i-- )
                        {
                            if( !Regex.IsMatch( middleTokens[i], PATTERN_FIRST_CHARACTER ) )
                            {
                                break;
                            }
                            middleTokens[i] = this.RemoveCommas( middleTokens[i] );
                            this.Last = middleTokens[i] + SPACE + this.Last;
                            middleTokens[i] = String.Empty;
                        }
                        this.Middle = String.Join( SPACE, middleTokens );
                    }

                    //Check for any names elements that need moving from first name to last name
                    if( !String.IsNullOrEmpty( this.First ) &&
                        !String.IsNullOrEmpty( this.Middle ) )
                    {
                        string[] firstTokens = this.First.Split( SPACE_CHARACTER, StringSplitOptions.RemoveEmptyEntries );
                        if( firstTokens.Length > 0 )
                        {
                            for( int i = firstTokens.Length - 1; i > 0; i-- )
                            {
                                this.Middle = firstTokens[i] + SPACE + this.Middle;
                                firstTokens[i] = String.Empty;
                            }
                            this.First = String.Join( SPACE, firstTokens );
                        }
                    }
                }
            }
        }

        private string RemoveCommas( string value )
        {
            string result = value != null ? value.Trim() : String.Empty;
            if( !String.IsNullOrEmpty( value )
                && value.IndexOf( COMMA ) != NOT_FOUND )
            {
                result = value.Substring( 0, value.IndexOf( COMMA ) ).Trim();
            }
            return result;
        }

        private void Reset()
        {
            this.Title = String.Empty;
            this.First = String.Empty;
            this.Middle = String.Empty;
            this.Last = String.Empty;
            this.Pedigree = String.Empty;
            this.Degree = String.Empty;
        }

        private bool IsTitleAtIndex( string[] nameTokens, int currentIndex )
        {
            bool isTitle = false;
            if( currentIndex < nameTokens.Length )
            {
                string piece = nameTokens[currentIndex];
                isTitle =
                    this.MatchesOneOf( piece, KNOWN_TITLES );
            }
            return isTitle;
        }

        private bool IsPedigreeAtIndex( string[] nameTokens, int currentIndex )
        {
            bool isPedigree = false;
            if( currentIndex < nameTokens.Length )
            {
                string piece = nameTokens[currentIndex];
                isPedigree =
                    this.MatchesOneOf( piece, KNOWN_PEDIGREES );
            }
            return isPedigree;
        }

        private bool IsDegreeAtIndex( string[] nameTokens, int currentIndex )
        {
            bool isDegree = false;
            if( currentIndex < nameTokens.Length )
            {
                string piece = nameTokens[currentIndex];
                isDegree =
                    this.MatchesOneOf( piece, KNOWN_DEGREES );
            }
            return isDegree;
        }

        private bool IsSuffixAtIndex( string[] nameTokens, int currentIndex )
        {
            return this.IsDegreeAtIndex( nameTokens, currentIndex ) ||
                   this.IsPedigreeAtIndex( nameTokens, currentIndex );
        }

        private bool MatchesOneOf( string piece, ArrayList etalonElements )
        {
            return etalonElements.IndexOf( piece.Trim().ToUpper() ) != NOT_FOUND;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CanonicalName()
        {
            this.Reset();
        }
        #endregion

        #region Data Elements
        private string i_Title;
        private string i_First;
        private string i_Middle;
        private string i_Last;
        private string i_Pedigree;
        private string i_Degree;
        #endregion
    }
}
