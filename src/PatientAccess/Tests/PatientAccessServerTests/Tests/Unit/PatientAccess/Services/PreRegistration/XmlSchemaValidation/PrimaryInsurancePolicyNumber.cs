using System.Xml.Schema;
using NUnit.Framework;
using Tests.Utilities.OnlinePreRegistration;

namespace Tests.Unit.PatientAccess.Services.PreRegistration.XmlSchemaValidation
{
    [TestFixture]
    [Category( "Fast" )]
    public class PrimaryInsurancePolicyNumber
    {
        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotBeBlank()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "" )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotBeLongerThan20Characters()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "A1A1A1A1A1A1A1A1A1A1A1" )
                .BuildMessage();
        }

        [Test]
        public void CanStartWithALetter()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "A1" )
                .BuildMessage();
        }

        [Test]
        public void CanStartWithAHyphen()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "-A" )
                .BuildMessage();
        }

        [Test]
        public void CanStartWithANumber()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "12" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveANumberInTheMiddle()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "A1B" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveALetterInTheMiddle()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "1A2" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveAHyphenInTheMiddle()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "1A-2" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveASpaceInTheMiddle()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "1A 2" )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotContainAComma()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "1," )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotContainAPeriod()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "1'" )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotContainAnApostrophe()
        {
            new MessageBuilder()
                .SetPrimaryInsurancePolicyNumber( "1." )
                .BuildMessage();
        }
    }
}
