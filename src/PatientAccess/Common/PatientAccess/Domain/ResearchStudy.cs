using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// ResearchStudy Domain class used for storing the collection   
    /// of Clinical Research Study along with their Sponsors.
    /// </summary> 
    [Serializable]
    public class ResearchStudy : CodedReferenceValue, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return String.Format( "{0} {1}", Code, Description + "  " + RegistryNumber );
        }

        /// <exception cref="ArgumentException">Object is not a ResearchStudy</exception>
        public int CompareTo( object obj )
        {
            if ( obj is ResearchStudy )
            {
                ResearchStudy researchStudyCode = ( ResearchStudy )obj;

                return Code.CompareTo( researchStudyCode.Code );
            }

            throw new ArgumentException( "Object is not a ResearchStudy" );
        }

        private bool Equals( ResearchStudy other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return base.Equals( other ) && Equals( other.i_ResearchSponsor, i_ResearchSponsor );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            return Equals( obj as ResearchStudy );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                {
                    return ( base.GetHashCode() * 397 ) ^ ( i_ResearchSponsor != null ? i_ResearchSponsor.GetHashCode() : 0 );
                }
            }
        }

        public static bool operator ==( ResearchStudy left, ResearchStudy right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( ResearchStudy left, ResearchStudy right )
        {
            return !Equals( left, right );
        }

        #endregion

        #region Properties
        public string ResearchSponsor
        {
            get
            {
                return i_ResearchSponsor;
            }
            private set
            {
                i_ResearchSponsor = value;
            }
        }
        public string RegistryNumber
        {
            get
            {
                return i_RegistryNumber;
            }
            private set
            {
                i_RegistryNumber = value;
            }
        }

        public DateTime TerminationDate { get; private set; }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public ResearchStudy()
        {
        }

        public ResearchStudy( string code, string description )
            : base( description, code )
        {
        }

        public ResearchStudy( string code, string description, string researchSponsor )
            : this( code, description )
        {
            ResearchSponsor = researchSponsor;

        }
        public ResearchStudy( string code, string description, string researchSponsor, string registryNumber )
            : this( code, description )
        {
            ResearchSponsor = researchSponsor;
            RegistryNumber = registryNumber;
        }

        public ResearchStudy( string code, string description, string researchSponsor, string registryNumber, DateTime terminationDate )
            : this( code, description, researchSponsor, registryNumber )
        {
            TerminationDate = terminationDate;
        }

        #endregion

        #region Data Elements

        private string i_ResearchSponsor = String.Empty;
        private string i_RegistryNumber = String.Empty;

        #endregion

        #region Constants

        #endregion
    }
}
