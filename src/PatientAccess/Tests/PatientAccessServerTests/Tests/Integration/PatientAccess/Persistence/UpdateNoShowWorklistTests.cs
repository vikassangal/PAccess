using System;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for UpdateNoShowWorklistTests.
    /// </summary>

    //TODO: Create XML summary comment for UpdateNoShowWorklistTests
    [TestFixture()]
    public class UpdateNoShowWorklistTests : AbstractPBARBroker
    {
        #region Constants
        private const string
            PARAM_FACILITYID            = "@P_HSP",
            COL_ACCOUNTNUMBER              = "ACCOUNTNUMBER",
            SP_UPDATENOSHOWWORKLIST  =       "UPDATENOSHOWWORKLIST";
        #endregion

        #region SetUp and TearDown UpdateNoShowWorklistTests
        [TestFixtureSetUp()]
        public static void SetUpUpdateNoShowWorklistTests()
        {
          
        }

        [TestFixtureTearDown()]
        public static void TearDownUpdateNoShowWorklistTests()
        {
        }
        #endregion

        #region Test Methods
       
        [Test()]
        // [Ignore("To make sure NoShow action not created anytime other than 5pm")]
        public void TestUpdateNoShowWorklist()
        {
            
            SafeReader reader = null;
            iDB2Command cmd = null;
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith(900);
                cmd = this.CommandFor(
                    "CALL " + SP_UPDATENOSHOWWORKLIST +
                    "(" + PARAM_FACILITYID + ")",
                    CommandType.Text,
                    facility);
               
                cmd.Parameters[PARAM_FACILITYID].Value = 900;
                
                reader = this.ExecuteReader(cmd);
                            
            }
            catch( iDB2Exception db2ex )
            {
                throw new BrokerException( db2ex.Message );
            }
            catch (DatabaseServiceException ex)
            {
                throw new BrokerException(ex.Message);
            }
            catch( Exception ex )
            {
                string s = ex.Message;
                throw new BrokerException("Unexpected Exception",ex);
            }
            finally
            {
                this.Close(reader);
                this.Close(cmd);
            }
            
        }
      

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
       
        #endregion
    }
}