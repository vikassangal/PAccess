using NUnit.Framework;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;

namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for MessageDisplayStateManagerTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class MessageDisplayStateManagerTests
    {
        [Test]
        public void TestPopulateRuleMessageDisplayMap()
        {
            MessageDisplayStateManager messageStateManager = new MessageDisplayStateManager();
            bool isRuleChecked = messageStateManager.HasErrorMessageBeenDisplayedEarlierFor( 
                typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );

            Assert.IsFalse( isRuleChecked );
        }

        [Test]
        public void TestSetErrorMessageDisplayedFor()
        {
            MessageDisplayStateManager messageStateManager = new MessageDisplayStateManager();
            bool isRuleChecked = messageStateManager.HasErrorMessageBeenDisplayedEarlierFor(
                typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );

            Assert.IsFalse(isRuleChecked);

            messageStateManager.SetErrorMessageDisplayedFor( typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString(), true );
            isRuleChecked = messageStateManager.HasErrorMessageBeenDisplayedEarlierFor(
                typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );

            Assert.IsTrue( isRuleChecked );
        }
    }
}
