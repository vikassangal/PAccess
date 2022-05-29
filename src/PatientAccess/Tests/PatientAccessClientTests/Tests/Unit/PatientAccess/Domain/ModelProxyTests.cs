using System;
using System.Reflection;
using Extensions;
using NUnit.Framework;
using PatientAccessClientTests.Tests.Unit.PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ModelProxyTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ModelProxyTests
    {
        #region Fields 

        #endregion Fields 

        #region Constructors 

        public ModelProxyTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #endregion Constructors 

        #region Properties 

        #endregion Properties 

        #region Methods 

        /// <summary>
        /// Tests the change tracking for bug1482. The TrackChange method was not doing 
        /// the compare correctly when it was trying detect if a value had been reset to
        /// the value it had when populated from the persistence layer.
        /// </summary>
        [Test]
        public void TestChangeTrackingForBug1482()
        {
            ModelProxy unit = new ModelProxy();
            ModelProxy origialChild = new ModelProxy();
            ModelProxy newChild = new ModelProxy();

            unit.AnotherModel = origialChild;
            unit.ResetChangeTracking();
            
            Assert.IsFalse( unit.HasChangedFor( "AnotherModel" ) );
            unit.AnotherModel = newChild;
            Assert.IsTrue( unit.HasChangedFor( "AnotherModel" ) );
            unit.AnotherModel = origialChild;
            Assert.IsFalse(unit.HasChangedFor( "AnotherModel" ));

        }

        #endregion Methods 


    }
}

namespace PatientAccessClientTests.Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Since we are actually testing something from another assembly, 
    /// this "shim" class gives us a way to test the base behavior.
    /// Model really needs to come into the Domain propert to avoid
    /// this hackery.
    /// </summary>
    public class ModelProxy : Model
    {
        #region Fields

        private Model _modelProperty;

        #endregion Fields

        #region Constructors

        public ModelProxy()
        {
            this.StringOne = Guid.NewGuid().ToString();
            this.StringTwo = Guid.NewGuid().ToString();
        }

        #endregion Constructors

        #region Properties
        public Model AnotherModel
        {
            get
            {
                return this._modelProperty;
            }
            set
            {
                this.SetAndTrack(ref this._modelProperty, value, MethodBase.GetCurrentMethod() );
            }
        }


        private string StringOne { get; set; }

        private string StringTwo { get; set; }

        #endregion Properties

        #region Methods

        public override bool Equals(object obj)
        {
            ModelProxy right = obj as ModelProxy;
            if (right == null) return false;
            bool isEqual =
                    this.StringOne.Equals(right.StringOne) &&
                    this.StringTwo.Equals(right.StringTwo);
            return isEqual;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Methods

    }

}
