using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RelationshipTypePBARBrokerTests : AbstractBrokerTests
    {
        #region Constants

        #endregion

        #region SetUp and TearDown RelationshipTypeBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpRelationshipTypeBrokerTests()
        {
            relationshipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownRelationshipTypeBrokerTests()
        { 
        }
        #endregion

        #region Test Methods
        [Test()]
        public void AllTypesOfRelationships()
        {
            ArrayList relationships = (ArrayList)relationshipTypeBroker.AllTypesOfRelationships( ACO_FACILITYID, TypeOfRole.Guarantor );
            
            RelationshipType employeeRelType = null;
            RelationshipType fosterChildRelType = null;
            RelationshipType spouseRelType = null;

            foreach( RelationshipType relType in relationships )
            {
                switch( relType.Code )
                {
                    case "02":
                        spouseRelType = relType;
                        break;
                    case "06":
                        fosterChildRelType = relType;
                        break;
                    case "08":
                        employeeRelType = relType;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull(employeeRelType,"Employee Rel Type not found");
            Assert.AreEqual(
                "EMPLOYEE",
                employeeRelType.Description,
                "employeeRelType description is incorrect"
                );
            Assert.IsNotNull(fosterChildRelType,"fosterChild Rel Type not found");
            Assert.AreEqual(
                "FOSTER CHILD",
                fosterChildRelType.Description,
                "fosterChildRelType description is incorrect"
                );

            Assert.IsNotNull(spouseRelType,"spouse Rel Type not found");
            Assert.AreEqual(
                "SPOUSE",
                spouseRelType.Description,
                "spouseRelType description is incorrect"
                ); 
        }

        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TestSpecificRelationshipTypeByNullCode()
        {
            RelationshipType relationshipType = relationshipTypeBroker.RelationshipTypeWith( ACO_FACILITYID, null );
        }

        [Test()]
        [ExpectedException( typeof( BrokerException ) )]
        public void TestSpecificRelationshipTypeById()
        {
            RelationshipType relationshipType = relationshipTypeBroker.RelationshipTypeWith( ACO_FACILITYID, 1 );
        }

        [Test()]
        public void TestRelationshipTypes()
        {
            ArrayList relationships = (ArrayList)relationshipTypeBroker.AllTypesOfRelationships( ACO_FACILITYID, TypeOfRole.EmergencyContact );
            Assert.IsNotNull( relationships, 
                              "null list returned for AllTypesOfRelationships (EmergencyContact)" );
            Assert.IsTrue( relationships.Count > 0, 
                           "No Relationships found for EmergencyContact" );

            relationships = (ArrayList)relationshipTypeBroker.AllTypesOfRelationships( ACO_FACILITYID, TypeOfRole.Insured );
            Assert.IsNotNull( relationships, 
                              "null list returned for AllTypesOfRelationships (Insured)" );
            Assert.IsTrue( relationships.Count > 0,
                           "No Relationships found for Insured");

            relationships = (ArrayList)relationshipTypeBroker.AllTypesOfRelationships( ACO_FACILITYID, TypeOfRole.NearestRelative );
            Assert.IsNotNull( relationships, 
                              "null list returned for AllTypesOfRelationships (NearestRelative)" );
            Assert.IsTrue( relationships.Count > 0,
                           "No Relationships found for NearestRelative");

            relationships = (ArrayList)relationshipTypeBroker.AllTypesOfRelationships( ACO_FACILITYID, TypeOfRole.Patient );
            Assert.IsNotNull( relationships, 
                              "null list returned for AllTypesOfRelationships (Patient)" );
            Assert.IsTrue( relationships.Count == 0, 
                           "Found Relationships found for rel type Patient");
           
        }

        [Test()]
        public void TestRelationshipTypeWith()
        {
            RelationshipType relType = new RelationshipType();
            relType = relationshipTypeBroker.RelationshipTypeWith( ACO_FACILITYID, "02" );
            Assert.IsNotNull( relType, "No relationshipType found for code == '02'" );
            Assert.AreEqual(
                "SPOUSE",
                relType.Description,
                "description is incorrect for rel type with code = '02'"
                );
        }
        [Test()]
        public void TestRelationshipTypeForBlank()
        {            
            string blank = String.Empty ;
            RelationshipType relType = relationshipTypeBroker.RelationshipTypeWith( ACO_FACILITYID, blank );

            Assert.AreEqual
                (blank,
                 relType.Code,
                 "Code  should be blank"
                );
            Assert.IsTrue(
                relType.IsValid            
                );
        }
        [Test()]
        public void TestRelationshipTypeForInvalid()
        {   string code = "55";
            RelationshipType relType = relationshipTypeBroker.RelationshipTypeWith( ACO_FACILITYID, code );

           
            Assert.IsFalse(
                relType.IsValid            
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IRelationshipTypeBroker relationshipTypeBroker = null;  
        #endregion
    }
}