using System;
using System.Reflection;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ReferenceValueTests
    {
        #region Constants
        private const long          TESTOID = 1234;
        private const string
            TESTCODE                = "TESTCODE",
            TESTDESC                = "TESTDESC";
        #endregion

        #region SetUp and TearDown ReferenceValueTests
        [TestFixtureSetUp()]
        public static void SetUpReferenceValueTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownReferenceValueTests()
        {
        }
        #endregion

        #region Test Methods
        //These tests need to be run against a concrete class since. 
        // the ReferenceValue Class is abstract.
        // use Race
        [Test()]
        public void TestToCodedString()
        {
            Race v = new Race( 0L, DateTime.Now, "Some description", "01" );
            
            Assert.AreEqual(
                "Some description",
                v.ToString()
                );
            
            Assert.AreEqual(
                "01 Some description",
                v.ToCodedString()
                );
        }
        [Test()]
        public void TestAllDerivedObjects4Parms()
        {
            Assembly a = Assembly.Load("PatientAccess.Common");
            Type[] types = a.GetTypes();
            Type[] ctorTypes = new Type[4];
            ctorTypes[0] = typeof(long);
            ctorTypes[1] = typeof(DateTime);
            ctorTypes[2] = typeof(string);
            ctorTypes[3] = typeof(string);

            object[] callParams = new object[4];
            callParams[0] = TESTOID;
            callParams[1] = ReferenceValue.NEW_VERSION;
            callParams[2] = TESTDESC;
            callParams[3] = TESTCODE;

            foreach( Type t in types )
            {
                if( t.IsSubclassOf(typeof(ReferenceValue)) )
                {
                    try
                    {
                        PropertyInfo[] propInfos = t.GetProperties();
                        
                        ConstructorInfo cif = t.GetConstructor(
                            BindingFlags.Instance | BindingFlags.Public ,
                            null,ctorTypes,null);


                        if( cif != null )
                        {
                            ParameterInfo[] parms = cif.GetParameters();
                            if( parms.Length == 4 &&  parms[3].Name.ToUpper().Equals("CODE"))
                            {
                                             
                                object newobj = cif.Invoke(callParams);

                                PropertyInfo piCode = t.GetProperty("Code");
                                string s1 = (string)piCode.GetValue(newobj,null);
                                Assert.IsTrue(s1.Equals(TESTCODE), "Codes Vary for " + t.Name);

                                PropertyInfo piDesc = t.GetProperty("Description");
                                string s2 = (string)piDesc.GetValue(newobj,null);
                                Assert.IsTrue(s2.Equals(TESTDESC), "Descriptions Vary for " + t.Name);

                                PropertyInfo piOid = t.GetProperty("Oid");
                                long l1 = (long)piOid.GetValue(newobj,null);
                                Assert.IsTrue(l1 == TESTOID, "Oids Vary for " + t.Name);
                            }
                            else
                            {
                                Console.WriteLine("Non Conforming Ctor found " + t.Name);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Assert.Fail(ex.Message);

                    }
                }
            }
        }

        [Test()]
        public void TestAllDerivedObjects3Parms()
        {
            Assembly a = Assembly.Load("PatientAccess.Common");
            Type[] types = a.GetTypes();
            Type[] ctorTypes = new Type[3];
            ctorTypes[0] = typeof(long);
            ctorTypes[1] = typeof(DateTime);
            ctorTypes[2] = typeof(string);

            object[] callParams = new object[3];
            callParams[0] = TESTOID;
            callParams[1] = ReferenceValue.NEW_VERSION;
            callParams[2] = TESTDESC;

            foreach( Type t in types )
            {
                if( t.IsSubclassOf(typeof(ReferenceValue)) )
                {
                    try
                    {
                        PropertyInfo[] propInfos = t.GetProperties();
                        
                        ConstructorInfo cif = t.GetConstructor(
                            BindingFlags.Instance | BindingFlags.Public ,
                            null,ctorTypes,null);

                        if( cif != null )
                        {
                   
                            object newobj = cif.Invoke(callParams);

                            PropertyInfo piDesc = t.GetProperty("Description");
                            string s2 = (string)piDesc.GetValue(newobj,null);
                            Assert.IsTrue(s2.Equals(TESTDESC), "Descriptions Vary for " + t.Name);

                            PropertyInfo piOid = t.GetProperty("Oid");
                            long l1 = (long)piOid.GetValue(newobj,null);
                            Assert.IsTrue(l1 == TESTOID, "Oids Vary for " + t.Name);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Assert.Fail(ex.Message);

                    }
                }
            }
        }

        [Test()]
        public void TestAllDerivedObjects2Parms()
        {
            Assembly a = Assembly.Load("PatientAccess.Common");
            Type[] types = a.GetTypes();
            Type[] ctorTypes = new Type[2];
            ctorTypes[0] = typeof(long);
            ctorTypes[1] = typeof(string);

            object[] callParams = new object[2];
            callParams[0] = TESTOID;
            callParams[1] = TESTDESC;

            foreach( Type t in types )
            {
                if( t.IsSubclassOf(typeof(ReferenceValue)) )
                {
                    try
                    {
                        PropertyInfo[] propInfos = t.GetProperties();
                        
                        ConstructorInfo cif = t.GetConstructor(
                            BindingFlags.Instance | BindingFlags.Public ,
                            null,ctorTypes,null);

                        if( cif != null )
                        {
                   
                            object newobj = cif.Invoke(callParams);

                            PropertyInfo piDesc = t.GetProperty("Description");
                            string s2 = (string)piDesc.GetValue(newobj,null);
                            Assert.IsTrue(s2.Equals(TESTDESC), "Descriptions Vary for " + t.Name);

                            PropertyInfo piOid = t.GetProperty("Oid");
                            long l1 = (long)piOid.GetValue(newobj,null);
                            Assert.IsTrue(l1 == TESTOID, "Oids Vary for " + t.Name);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Assert.Fail(ex.Message);

                    }
                }
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}