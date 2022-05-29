using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    [TestFixture]
    [Category( "Fast" )]
    public class SsnPresenterTests
    {
        [Test]
        public void when_ssn_status_is_changed_to_unknown_then_get_a_new_ssn()
        {

            var newSsn = new SocialSecurityNumber( SocialSecurityNumber.FLORIDA_NONE_SSN );

            var ssnView = MockRepository.GenerateStub<ISsnView>();
            ssnView.SsnFactory = GetStubSsnFactoryThatReturns( newSsn );

            var patient = new Patient();

            var presenter = new SsnPresenter( ssnView, new State(), new Account { AdmitDate = DateTime.Now }, patient );

            var unknownSsnStatus = SocialSecurityNumberStatus.UnknownSSNStatus;

            presenter.SsnStatusChanged( unknownSsnStatus );

            Assert.AreEqual( newSsn, patient.SocialSecurityNumber );

        }

        [Test]
        public void when_ssn_status_is_changed_to_known_and_ssn_is_a_default_ssn_then_get_a_new_ssn()
        {
            var ssnView = MockRepository.GenerateStub<ISsnView>();

            var newSsn = new SocialSecurityNumber( SocialSecurityNumber.FLORIDA_NONE_SSN );

            ssnView.SsnFactory = GetStubSsnFactoryThatReturns( newSsn );

            var patient = new Patient { SocialSecurityNumber = GetDefaultSsn() };

            var presenter = new SsnPresenter( ssnView, new State(), new Account { AdmitDate = DateTime.Now }, patient );

            var knownSsnStatus = SocialSecurityNumberStatus.KnownSSNStatus;

            presenter.SsnStatusChanged( knownSsnStatus );

            Assert.AreEqual( newSsn, patient.SocialSecurityNumber );
        }

        [Test]
        public void when_ssn_status_is_changed_to_known_and_ssn_is_not_default_ssn_then_dont_get_a_new_ssn()
        {
            var ssnView = MockRepository.GenerateStub<ISsnView>();

            var newSsn = new SocialSecurityNumber( "123342534" );

            ssnView.SsnFactory = GetStubSsnFactoryThatReturns( newSsn );

            var patient = new Patient { SocialSecurityNumber = GetNonDefaultSsn() };

            var presenter = new SsnPresenter( ssnView, new State(), new Account { AdmitDate = DateTime.Now }, patient );

            var knownSsnStatus = SocialSecurityNumberStatus.KnownSSNStatus;

            presenter.SsnStatusChanged( knownSsnStatus );

            Assert.AreNotEqual( newSsn, patient.SocialSecurityNumber );
        }

        private static SocialSecurityNumber GetNonDefaultSsn()
        {
            return SocialSecurityNumber.FloridaNewbornSSN;
        }

        private static SocialSecurityNumber GetDefaultSsn()
        {
            return new SocialSecurityNumber( SocialSecurityNumber.DEFAULT_SSN_NUMBERS[0] ) { SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus };
        }

        private static ISsnFactory GetStubSsnFactoryThatReturns( SocialSecurityNumber expectedSsn )
        {
            var ssnFactory = MockRepository.GenerateMock<ISsnFactory>();
            ssnFactory.Stub( x => x.GetValidSocialSecurityNumberFor( Arg<State>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<SocialSecurityNumberStatus>.Is.Anything ) ).Return( expectedSsn );
            return ssnFactory;
        }
    }
}
