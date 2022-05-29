using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace Tests.Unit.PatientAccess.UI.HelperClasses
{
    [TestFixture]
    [Category( "Fast" )]
    public class PbarHelperTests
    {
        private static readonly String[] ExcludedProperties = new[] { "BenefitsResponseParseStrategy" };


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestChangeLogicalStringPropertiesToUpperCase_WithNullInput_ShouldThrowException()
        {
            PbarHelper.ChangeLogicalStringPropertiesToUpperCase(null);
        }

        [Test]
        public void TestChangeLogicalStringPropertiesToUpperCase()
        {
            DataValidationBenefitsResponse benefitsResponse = new DataValidationBenefitsResponse();
            var lowercaseStringValue = "something 12/12/2008";
            SetStringPropertiesTo(benefitsResponse, lowercaseStringValue);

            IList<PropertyInfo> readableProperties = GetReadableStringPropertiesForBenefitsResponse();
            IList<PropertyInfo> propertiesThatShouldBeInUpperCase = this.RemoveExcludedProperties(readableProperties);

            PbarHelper.ChangeLogicalStringPropertiesToUpperCase(benefitsResponse);

            Trace.WriteLine("Properties that should be in upperCase:");
            foreach (PropertyInfo info in propertiesThatShouldBeInUpperCase)
            {
                string value = (string)info.GetValue(benefitsResponse, null);

                Trace.Indent();
                Trace.WriteLine(info.Name + " : " + value);
                Trace.Unindent();

                Assert.IsTrue(value.ToUpperInvariant() == value, string.Format("The property {0} is not in upper case", info.Name));
            }
        }

        [Test]
        public void TestChangeLogicalStringPropertiesToUpperCase_ShouldNotChangeCaseForExcludedProperties()
        {
            DataValidationBenefitsResponse benefitsResponse = new DataValidationBenefitsResponse();
            var lowercaseStringValue = "something";
            SetStringPropertiesTo(benefitsResponse, lowercaseStringValue);

            IList<PropertyInfo> propertiesThatShouldBeExcluded = this.GetExcludedPropertiesForBenefitsResponse();

            PbarHelper.ChangeLogicalStringPropertiesToUpperCase(benefitsResponse);
            Trace.WriteLine("Properties that should be excluded:");
            foreach (PropertyInfo info in propertiesThatShouldBeExcluded)
            {
                string value = (string)info.GetValue(benefitsResponse, null);

                Trace.Indent();
                Trace.WriteLine(info.Name + " : " + value);
                Trace.Unindent();

                Assert.IsFalse(value.ToUpperInvariant() == value, string.Format("The case for property {0} should not be changed", info.Name));
            }
        }

        [Test]
        public void TestChangeStringPropertiesToUpperCase_WhenPropertiesAreNull_ShouldNotThrowException()
        {
            DataValidationBenefitsResponse benefitsResponse = new DataValidationBenefitsResponse();
            const string lowercaseStringValue = null;
            SetStringPropertiesTo(benefitsResponse, lowercaseStringValue);

            PbarHelper.ChangeLogicalStringPropertiesToUpperCase(benefitsResponse);
        }


        private List<PropertyInfo> GetExcludedPropertiesForBenefitsResponse()
        {
            return GetReadableStringPropertiesForBenefitsResponse().Where(property => ExcludedProperties.Contains(property.Name)).ToList();
        }

        private List<PropertyInfo> RemoveExcludedProperties(IEnumerable<PropertyInfo> readableProperties)
        {
            return readableProperties.SkipWhile(property => ExcludedProperties.Contains(property.Name)).ToList();
        }

        private static void SetStringPropertiesTo(DataValidationBenefitsResponse benefitsResponse, string value)
        {
            IList<PropertyInfo> properties = GetWritableStringPropertiesForBenefitsResponse();
            properties.ToList().ForEach(property => property.SetValue(benefitsResponse, value, null));
        }

        private static IList<PropertyInfo> GetReadableStringPropertiesForBenefitsResponse()
        {

            return typeof(DataValidationBenefitsResponse)
                .GetProperties().Where(property => property.CanRead &&
                                                   property.PropertyType == typeof(string))
                                .ToList();
        }


        private static IList<PropertyInfo> GetWritableStringPropertiesForBenefitsResponse()
        {
            return typeof(DataValidationBenefitsResponse).GetProperties().Where(property => property.CanRead &&
                                                          property.CanWrite &&
                                                          property.PropertyType == typeof(string))
                                       .ToList();
        }
    }
}