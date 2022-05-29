using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Utilities;

namespace PatientAccess.BrokerProxies
{
    class ResearchStudyBrokerProxy: IResearchStudyBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public IEnumerable<ResearchStudy> AllResearchStudies( long facilityID )
        {
            IList<ResearchStudy> allResearchStudies =
                ( IList<ResearchStudy> )Cache.Get( RESEARCH_STUDY_BROKER_PROXY_ALL_RESEARCH_STUDIES );

            if ( allResearchStudies == null )
            {
                lock ( RESEARCH_STUDY_BROKER_PROXY_ALL_RESEARCH_STUDIES )
                {
                    allResearchStudies = new List<ResearchStudy>( ResearchStudyBroker.AllResearchStudies( facilityID ) );

                    if ( Cache.Get( RESEARCH_STUDY_BROKER_PROXY_ALL_RESEARCH_STUDIES ) == null )
                    {
                        Cache.Insert( RESEARCH_STUDY_BROKER_PROXY_ALL_RESEARCH_STUDIES, allResearchStudies );
                    }
                }
            }

            return allResearchStudies;
        }

        public ResearchStudy ResearchStudyWith( long facilityNumber, string researchStudyCode )
        {
            ResearchStudy researchStudy = null; 
            Guard.ThrowIfArgumentIsNull( researchStudyCode, "researchStudyCode" );

            // Look for the Research Study in the Cached collection first
            IEnumerable<ResearchStudy> researchStudies = AllResearchStudies( facilityNumber );
            foreach ( ResearchStudy study in researchStudies )
            {
                if ( study.Code.Equals( researchStudyCode ) )
                {
                    researchStudy = study;
                    break;
                }
            }

            // If Research Study is not found in Cache, use broker to retrieve it from the database
            if ( researchStudy == null )
            {
                return ResearchStudyBroker.ResearchStudyWith( facilityNumber, researchStudyCode );
            }

            return researchStudy;
        }

        #endregion

        #region Properties

        private ICache Cache { get; set; }

        private IResearchStudyBroker ResearchStudyBroker
        {
            get
            {
                return i_ResearchStudyBroker;
            }
            set
            {
                i_ResearchStudyBroker = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        /// <summary>
        /// Initializes a new instance of the <see cref="ResearchStudyBrokerProxy"/> class.
        /// </summary>
        /// <param name="researchStudyBroker">Research Study broker.</param>
        /// <param name="cache">The cache.</param>
        public ResearchStudyBrokerProxy( IResearchStudyBroker researchStudyBroker, ICache cache )
        {
            ResearchStudyBroker = researchStudyBroker;
            Cache = cache;
        }
        #endregion

        #region Data Elements
        private IResearchStudyBroker i_ResearchStudyBroker = BrokerFactory.BrokerOfType< IResearchStudyBroker >() ;
        #endregion

        #region Constants

        private const string
            RESEARCH_STUDY_BROKER_PROXY_ALL_RESEARCH_STUDIES = "RESEARCH_STUDY_BROKER_PROXY_ALL_RESEARCH_STUDIES";
        #endregion
    }
}
