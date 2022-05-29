using System;
using PatientAccess.Persistence.Utilities;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.Utilities
{
    [TestFixture]
    [Category( "Fast" )]
    public class Db2StoredProcedureCallBuilderTests
    { 
        private const string StoredProcName = "StoredProcedureTest";
        
        [Test]
        public void TestBuild_WithValidProcNameAndNonEmptyParameterList_ShouldReturnCallText()
        {
            var parameters = new[] { "Param1", "Param2", "param3" };
            var callBuilder = new Db2StoredProcedureCallBuilder(parameters, StoredProcName);
            var expectedCommandText = string.Format( "CALL {0}(Param1,Param2,param3)", StoredProcName );
            
            var actualCommandText = callBuilder.Build();
            
            Assert.AreEqual( expectedCommandText, actualCommandText, "command text is not constructed properly." );
        }

        [Test]
        public void TestBuild_WithValidProcNameAndAnEmptyParameterList_ShouldReturnCallText()
        {
            var parameters = new string[0];
            var callBuilder = new Db2StoredProcedureCallBuilder( parameters, StoredProcName );
            var expectedCommandText = string.Format( "CALL {0}()", StoredProcName );

            var actualCommandText = callBuilder.Build();

            Assert.AreEqual( expectedCommandText, actualCommandText, "command text is not constructed properly." );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TestConstructor_WhenStoredProcParamsIsNull_ShouldThrowException()
        {
            new Db2StoredProcedureCallBuilder(null, StoredProcName);
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TestConstructor_WhenStoredProcNameIsNull_ShouldThrowException()
        {
            new Db2StoredProcedureCallBuilder(new[] {"Param1", "Param2", "param3"}, null);
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TestConstructor_WhenStoredProcNameIsAnEmptyString_ShouldThrowException()
        {
            new Db2StoredProcedureCallBuilder( new[] { "Param1", "Param2", "param3" }, string.Empty );
        }
    }
}