using System;
using PatientAccess.Annotations;

namespace PatientAccess.Domain
{
    /// <summary>
    /// ConsentedResearchStudy is used to store individual  
    /// Clinical Research Study details for a patient.
    /// </summary>
    [Serializable]
    public class ConsentedResearchStudy : IComparable
    {
        public bool Equals( ConsentedResearchStudy other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return Equals( other.i_ProofOfConsent, i_ProofOfConsent ) && Equals( other.i_ResearchStudy, i_ResearchStudy );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType() != typeof( ConsentedResearchStudy ) ) return false;
            return Equals( ( ConsentedResearchStudy )obj );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ( i_ProofOfConsent.GetHashCode() * 397 ) ^ i_ResearchStudy.GetHashCode();
            }
        }

        public int CompareTo( object obj )
        {
            return this.i_ResearchStudy.CompareTo( obj );
        }

        public static bool operator ==( ConsentedResearchStudy left, ConsentedResearchStudy right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( ConsentedResearchStudy left, ConsentedResearchStudy right )
        {
            return !Equals( left, right );
        }

        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return String.Format( "{0} {1}", ResearchStudy.Code, ResearchStudy.Description );
        }
        #endregion

        #region Properties
        [UsedImplicitly]
        public YesNoFlag ProofOfConsent
        {
            get
            {
                return i_ProofOfConsent;
            }
            private set
            {
                i_ProofOfConsent = value;
            }
        }

        [UsedImplicitly]
        public ResearchStudy ResearchStudy
        {
            get
            {
                return i_ResearchStudy;
            }
            private set
            {
                i_ResearchStudy = value;
            }
        }

        /// <summary>
        /// Gets the research sponsor. This is a facade property added to ease data binding.
        /// </summary>
        /// <value>The research sponsor.</value>
        [UsedImplicitly]
        public string ResearchSponsor
        {
            get
            {
                return i_ResearchStudy.ResearchSponsor;
            }
        }

        /// <summary>
        /// Gets the Registry Number. This is a facade property added to ease data binding.
        /// </summary>
        /// <value>The Registry Number.</value>
        [UsedImplicitly]
        public string RegistryNumber
        {
            get
            {
                return i_ResearchStudy.RegistryNumber;
            }
        }

        /// <summary>
        /// Gets the code. This is a facade property added to ease data binding.
        /// </summary>
        /// <value>The code.</value>
        [UsedImplicitly]
        public string Code
        {
            get
            {
                return i_ResearchStudy.Code;
            }
        }

        /// <summary>
        /// Gets the description. This is a facade property added to ease data
        /// binding.
        /// </summary>
        /// <value>The description.</value>
        [UsedImplicitly]
        public string Description
        {
            get
            {
                return i_ResearchStudy.Description;
            }
        }


        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public ConsentedResearchStudy()
        {
        }

        public ConsentedResearchStudy( ResearchStudy researchStudy, YesNoFlag proofOfConsent )
        {
            ResearchStudy = researchStudy;
            ProofOfConsent = proofOfConsent;
        }
        #endregion

        #region Data Elements

        private YesNoFlag i_ProofOfConsent = YesNoFlag.No;
        private ResearchStudy i_ResearchStudy = new ResearchStudy();
        #endregion

        #region Constants

        #endregion
    }
}
