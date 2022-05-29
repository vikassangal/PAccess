using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.UI.PatientSearch;

namespace Tests.Unit.PatientAccess.UI.PatientSearch
{
    [TestFixture]
    [Category( "Fast" )]
    public class CommandButtonManagerTest
    {
        [Test]
        public void LoadButtons_WithRegistrationActivity_ShouldAddActivatePreregButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();
            RegistrationActivity activity = new RegistrationActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnActivatePrereg") != null, "Activate Account - Standard Registration button should be added.");

        }

        [Test]
        public void LoadButtons_WithRegistrationActivity_ShouldAddActivatePreregShortButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();
            RegistrationActivity activity = new RegistrationActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnActivatePreregShort") != null, "Activate Account - Diagnosis Registration button should be added.");

        }

        [Test]
        public void LoadButtons_WithMaintenanceActivity_ShouldAddConvertToShortPreregButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();
            MaintenanceActivity activity = new MaintenanceActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnConvertToDiagPrereg") != null, "Convert Account - Diagnostic Preregistration button should be added.");
        }

        [Test]
        public void LoadButtons_WithShortRegistrationActivity_ShouldAddActivatePreregButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();

            ShortRegistrationActivity activity = new ShortRegistrationActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnActivatePrereg") != null, "Activate Account - Standard Registration button should be added.");

        }

        [Test]
        public void LoadButtons_WithShortRegistrationActivity_ShouldAddActivatePreregShortButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();
            ShortRegistrationActivity activity = new ShortRegistrationActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnActivatePreregShort") != null, "Activate Account - Diagnosis Registration button should be added.");

        }

        //PreRegistrationActivity
        [Test]
        public void LoadButtons_WithNonRegistrationActivity_ShouldNotAddActivatePreregButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();

            PreRegistrationActivity activity = new PreRegistrationActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnActivatePrereg") == null, "Activate Account - Standard Registration button should not be added.");

        }

        [Test]
        public void LoadButtons_WithNonRegistrationActivity_ShouldNotAddActivatePreregShortButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();
            PreRegistrationActivity activity = new PreRegistrationActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnActivatePreregShort") == null, "Activate Account - Diagnosis Registration button should not be added.");

        }

        [Test]
        public void LoadButtons_WithNonMaintenanceActivity_ShouldNotAddConvertToShortPreregButton()
        {
            CommandButtonManager cbm = new CommandButtonManager();
            PreRegistrationActivity activity = new PreRegistrationActivity();
            cbm.LoadButtons(activity);
            Assert.IsTrue(cbm.Command("btnConvertToDiagPrereg") == null, "Convert Account - Diagnostic Preregistration button should not be added.");

        }
    }
}
