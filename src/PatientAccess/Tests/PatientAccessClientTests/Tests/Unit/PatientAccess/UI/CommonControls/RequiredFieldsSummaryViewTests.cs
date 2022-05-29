using System.Collections.Generic;
using NUnit.Framework;
using PatientAccess.UI.CommonControls;

namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    [TestFixture]
    [Category("Fast")]
    public class RequiredFieldsSummaryViewTests
    {
        [Test]
        public void DatabindingShouldWorkWhenTheViewIsUpdated()
        {
            var view = new RequiredFieldsSummaryView();

            const string expectedTextForTabCell = "sometab";
            const string expectedTextForFieldCell = "somefield";

            var items = new List<RequiredFieldItem> {new RequiredFieldItem(expectedTextForTabCell, expectedTextForFieldCell)};

            view.Update(items);
            var actualTextForTabCell = view.dgvActionItems.Rows[0].Cells[0].Value;
            var actualTextForFieldCell = view.dgvActionItems.Rows[0].Cells[1].Value;
            
            Assert.AreEqual(expectedTextForFieldCell,actualTextForFieldCell);
            Assert.AreEqual(expectedTextForTabCell,actualTextForTabCell);
        }
    }
}
