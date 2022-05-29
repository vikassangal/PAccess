using System.Xml.Schema;
using NUnit.Framework;
using Tests.Utilities.OnlinePreRegistration;

namespace Tests.Unit.PatientAccess.Services.PreRegistration.XmlSchemaValidation
{
    [TestFixture]
    [Category( "Fast" )]
    public class SecondaryInsurancePolicyNumber
    {
        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotBeBlank()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "" )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotBeLongerThan20Characters()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "A1A1A1A1A1A1A1A1A1A1A1" )
                .BuildMessage();
        }

        [Test]
        public void CanStartWithALetter()
        {
            new MessageBuilder()

                .BuildMessage();
        }

        [Test]
        public void CanStartWithAHyphen()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "-A" )
                .BuildMessage();
        }

        [Test]
        public void CanStartWithANumber()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "12" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveANumberInTheMiddle()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "A1B" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveALetterInTheMiddle()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "1A2" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveAHyphenInTheMiddle()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "1A-2" )
                .BuildMessage();
        }

        [Test]
        public void CanHaveASpaceInTheMiddle()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "1A 2" )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotContainAComma()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "1," )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotContainAPeriod()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "1'" )
                .BuildMessage();
        }

        [Test]
        [ExpectedException( typeof( XmlSchemaValidationException ) )]
        public void CannotContainAnApostrophe()
        {
            new MessageBuilder()
                .SetSecondaryInsurancePolicyNumber( "1." )
                .BuildMessage();
        }
    }
}
