using NUnit.Framework;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.PhysicianSearchViews;

namespace Tests.Unit.PatientAccess.UI.PhysicianSearchViews
{
    /// <summary>
    /// Summary description for PhysicianServiceTests.
    /// </summary>

    //TODO: Create XML summary comment for PhysicianServiceTests
    [TestFixture]
    [Category( "Fast" )]
    public class PhysicianServiceTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PhysicianServiceTests
        [SetUp()]
        public void SetUpPhysicianServiceTests()
        {
            this.Physician = new Physician();
            this.PhysicianRelationship = new PhysicianRelationship();
        }

        [TearDown()]
        public void TearDownPhysicianServiceTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void AssignValidAdmittingPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService,  "PhysicianService cannot be null" );

            this.Physician.ActiveInactiveFlag = "Y";
            this.Physician.AdmittingPrivileges = "Y";
            this.Physician.ExcludedStatus = "N";

            //aPhysicianService.AssignPhysician( 
        }

        [Test()]
        public void AssignValidAttendingPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignValidReferringPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignValidOperatingPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignValidPrimaryCarePhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignInvalidAdmittingPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignInvalidAttendingPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignInvalidReferringPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignInvalidOperatingPhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }

        [Test()]
        public void AssignInvalidPrimaryCarePhysician()
        {
            PhysicianService aPhysicianService = new PhysicianService();
            Assert.IsNotNull( aPhysicianService, "PhysicianService cannot be null" );
        }
        #endregion

        #region Support Methods

        private Physician Physician
        {
            get
            {
                return this.i_Physician;
            }
            set
            {
                this.i_Physician = value;
            }
        }

        private PhysicianRelationship PhysicianRelationship
        {
            get
            {
                return this.i_PhysicianRelationship;
            }
            set
            {
                this.i_PhysicianRelationship = value;
            }
        }
        #endregion

        #region Data Elements
        private Physician i_Physician;
        private PhysicianRelationship i_PhysicianRelationship;
        #endregion
    }
}