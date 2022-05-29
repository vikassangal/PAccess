using System;
using System.Collections;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class OccurrenceCodeComparerByCodeAndDateTests
    {
        [Test]
        public void WithDuplicateCodesAndAllDatesEntered()
        {
            var oc1 = CreateOccurrenceCode( "16", DateTime.Parse( "Jan 21, 2013" ) );
            var oc2 = CreateOccurrenceCode( "46", DateTime.Parse( "Jan 22, 2013" ) );
            var oc3 = CreateOccurrenceCode( "50", DateTime.Parse( "Jan 23, 2013" ) );
            var oc4 = CreateOccurrenceCode( "50", DateTime.Parse( "Jan 24, 2013" ) );

            var expectedSortedList = new ArrayList { oc1, oc2, oc3, oc4 };
            var actualSortedList = new ArrayList { oc4, oc2, oc1, oc3 };

            actualSortedList.Sort( new OccurrenceCodeComparerByCodeAndDate() );

            CollectionAssert.AreEqual( expectedSortedList, actualSortedList );
        }

        [Test]
        public void WithDuplicateCodesAndAllOfThemWithEmptyDateTime()
        {
            var oc1 = CreateOccurrenceCode( "16", DateTime.MinValue );
            var oc2 = CreateOccurrenceCode( "46", DateTime.MinValue );
            var oc3 = CreateOccurrenceCode( "50", DateTime.MinValue );
            var oc4 = CreateOccurrenceCode( "50", DateTime.MinValue );

            var expectedSortedList = new ArrayList { oc1, oc2, oc3, oc4 };
            var actualSortedList = new ArrayList { oc4, oc1, oc2, oc3 };

            actualSortedList.Sort( new OccurrenceCodeComparerByCodeAndDate() );

            CollectionAssert.AreEqual( expectedSortedList, actualSortedList );
        }

        [Test]
        public void WithDuplicateCodesAndEmptyDateForDuplicates()
        {
            var oc1 = CreateOccurrenceCode( "16", DateTime.Parse( "Jan 22, 2013" ) );
            var oc2 = CreateOccurrenceCode( "46", DateTime.Parse( "Jan 21, 2013" ) );
            var oc3 = CreateOccurrenceCode( "50", DateTime.MinValue );
            var oc4 = CreateOccurrenceCode( "50", DateTime.MinValue );

            var expectedSortedList = new ArrayList { oc1, oc2, oc3, oc4 };
            var actualSortedList = new ArrayList { oc4, oc1, oc2, oc3 };

            actualSortedList.Sort( new OccurrenceCodeComparerByCodeAndDate() );

            CollectionAssert.AreEqual( expectedSortedList, actualSortedList );
        }

        [Test]
        public void WithDuplicateCodesAndOneDuplicateWithEmptyDateTime()
        {
            var oc1 = CreateOccurrenceCode( "16", DateTime.Parse( "Jan 25, 2013" ) );
            var oc2 = CreateOccurrenceCode( "46", DateTime.Parse( "Jan 21, 2013" ) );
            var oc3 = CreateOccurrenceCode( "50", DateTime.Parse( "Jan 22, 2013" ) );
            var oc4 = CreateOccurrenceCode( "50", DateTime.MinValue );

            var expectedSortedList = new ArrayList { oc1, oc2, oc3, oc4 };
            var actualSortedList = new ArrayList { oc4, oc1, oc2, oc3 };

            actualSortedList.Sort( new OccurrenceCodeComparerByCodeAndDate() );

            CollectionAssert.AreEqual( expectedSortedList, actualSortedList );
        }

        [Test]
        public void WithNoDuplicateCodesAndNonEmptyDateTimes()
        {
            var oc1 = CreateOccurrenceCode( "16", DateTime.Parse( "Jan 25, 2013" ) );
            var oc2 = CreateOccurrenceCode( "31", DateTime.Parse( "Jan 11, 2013" ) );
            var oc3 = CreateOccurrenceCode( "46", DateTime.Parse( "Jan 21, 2013" ) );
            var oc4 = CreateOccurrenceCode( "50", DateTime.Parse( "Jan 22, 2013" ) );

            var expectedSortedList = new ArrayList { oc1, oc2, oc3, oc4 };
            var actualSortedList = new ArrayList { oc4, oc1, oc2, oc3 };

            actualSortedList.Sort( new OccurrenceCodeComparerByCodeAndDate() );

            CollectionAssert.AreEqual( expectedSortedList, actualSortedList );
        }

        private static OccurrenceCode CreateOccurrenceCode( string code, DateTime occurrenceDate )
        {
            var oc = new OccurrenceCode( 1, PersistentModel.NEW_VERSION, "Description", code, occurrenceDate );
            return oc;
        }
    }
}