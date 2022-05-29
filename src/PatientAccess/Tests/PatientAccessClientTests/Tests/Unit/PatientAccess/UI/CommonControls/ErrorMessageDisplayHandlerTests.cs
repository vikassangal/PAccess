using System.Windows.Forms;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    [TestFixture]
    [Category( "Fast" )]
    public class ErrorMessageDisplayHandlerTests
    {
        [Test]
        public void TestDisplayYesNoErrorMessageFor_WhenAccidentIsAuto_ShouldDisplayAutoAccidentErrorMessage()
        {
            var accident = new Accident { Kind = TypeOfAccident.NewAuto() };
            accident.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;

            var messageDisplayHandler = GetErrorMessageDisplayHandlerWithMockDependenciesFor( accident );
            var mockActivityEventAggregator = messageDisplayHandler.EventAggregator;
            var mockMessageBoxAdapter = messageDisplayHandler.MessageBoxAdapter;
            var expectedArgs = new LooseArgs( new object() );

            //Rhino mocks has a defect when running under .net 4.0 where the following causing an exception
            //messageDisplayHandler.EventAggregator.AssertWasCalled(x => x.RaiseErrorMessageDisplayedEvent( Arg<IErrorMessageDisplayHandler>.Is.Anything , Arg<LooseArgs>.Matches(y=>y.Context==looseArgs.Context)));
            //the workaround I found was to use the WhenCalled method and get access to the argument passed and manually check it in an assert
            //http://blog.gmane.org/gmane.comp.windows.devel.dotnet.rhino-mocks/month=20100201
            //http://www.mail-archive.com/rhinomocks@googlegroups.com/msg01386.html

            mockActivityEventAggregator.Expect( f => f.RaiseErrorMessageDisplayedEvent( Arg<object>.Is.Anything, Arg<LooseArgs>.Is.NotNull ) ).WhenCalled( x => expectedArgs = (LooseArgs)x.Arguments[1] );


            mockMessageBoxAdapter.Expect( x =>
                x.ShowMessageBox( UIErrorMessages.NO_MEDICARE_FOR_AUTO_ACCIDENT_QUESTION,
                                  ErrorMessageDisplayHandler.CAPTION_WARNING, MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 ) ).Return( DialogResult.Yes );


            messageDisplayHandler.DisplayYesNoErrorMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );

            mockMessageBoxAdapter.VerifyAllExpectations();
            mockActivityEventAggregator.VerifyAllExpectations();
            Assert.AreEqual( expectedArgs.Context, typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );

        }

        [Test]
        public void TestDisplayYesNoErrorMessageFor_WhenAccidentIsEmploymentRelated_ShouldDisplayEmploymentRelatedAccidentErrorMessage()
        {
            var accident = new Accident { Kind = TypeOfAccident.NewAuto() };
            accident.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL;

            var messageDisplayHandler = GetErrorMessageDisplayHandlerWithMockDependenciesFor( accident );

            var mockActivityEventAggregator = messageDisplayHandler.EventAggregator;
            var mockMessageBoxAdapter = messageDisplayHandler.MessageBoxAdapter;
            var expectedArgs = new LooseArgs( new object() );

            mockMessageBoxAdapter.Expect(
                x => x.ShowMessageBox( UIErrorMessages.NO_MEDICARE_FOR_EMPLOYMENT_RELATED_ACCIDENT_QUESTION,
                                        ErrorMessageDisplayHandler.CAPTION_WARNING, MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 ) ).Return( DialogResult.OK );


            //Rhino mocks has a defect when running under .net 4.0 where the following causing an exception
            //messageDisplayHandler.EventAggregator.AssertWasCalled(x => x.RaiseErrorMessageDisplayedEvent( Arg<IErrorMessageDisplayHandler>.Is.Anything , Arg<LooseArgs>.Matches(y=>y.Context==looseArgs.Context)));
            //the workaround I found was to use the WhenCalled method and get access to the argument passed and manually check it in an assert
            //http://blog.gmane.org/gmane.comp.windows.devel.dotnet.rhino-mocks/month=20100201
            //http://www.mail-archive.com/rhinomocks@googlegroups.com/msg01386.html

            mockActivityEventAggregator.Expect(
                x => x.RaiseErrorMessageDisplayedEvent( Arg<IErrorMessageDisplayHandler>.Is.Anything, Arg<LooseArgs>.Is.NotNull ) ).WhenCalled( x => expectedArgs = (LooseArgs)x.Arguments[1] );

            messageDisplayHandler.DisplayYesNoErrorMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );


            mockMessageBoxAdapter.VerifyAllExpectations();
            mockActivityEventAggregator.VerifyAllExpectations();
            Assert.AreEqual( expectedArgs.Context, typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );

        }

        [Test]
        public void TestDisplayOkWarningMessageFor_WhenAccidentIsAuto_ShouldDisplayAutoAccidentWarningMessage()
        {
            var accident = new Accident { Kind = TypeOfAccident.NewAuto() };
            accident.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;

            var messageDisplayHandler = GetErrorMessageDisplayHandlerWithMockDependenciesFor( accident );
            var mockActivityEventAggregator = messageDisplayHandler.EventAggregator;
            var mockMessageBoxAdapter = messageDisplayHandler.MessageBoxAdapter;
            var expectedArgs = new LooseArgs( new object() );


            mockMessageBoxAdapter.Expect(
                x => x.ShowMessageBox( UIErrorMessages.NO_MEDICARE_FOR_AUTO_ACCIDENT_MSG,
                                        ErrorMessageDisplayHandler.CAPTION_WARNING, MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 ) ).Return( DialogResult.OK );

            //Rhino mocks has a defect when running under .net 4.0 where the following causing an exception
            //messageDisplayHandler.EventAggregator.AssertWasCalled(x => x.RaiseErrorMessageDisplayedEvent( Arg<IErrorMessageDisplayHandler>.Is.Anything , Arg<LooseArgs>.Matches(y=>y.Context==looseArgs.Context)));
            //the workaround I found was to use the WhenCalled method and get access to the argument passed and manually check it in an assert
            //http://blog.gmane.org/gmane.comp.windows.devel.dotnet.rhino-mocks/month=20100201
            //http://www.mail-archive.com/rhinomocks@googlegroups.com/msg01386.html

            mockActivityEventAggregator.Expect(
                x => x.RaiseErrorMessageDisplayedEvent( Arg<IErrorMessageDisplayHandler>.Is.Anything, Arg<LooseArgs>.Is.NotNull ) ).WhenCalled( x => expectedArgs = (LooseArgs)x.Arguments[1] );


            messageDisplayHandler.DisplayOkWarningMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );


            mockMessageBoxAdapter.VerifyAllExpectations();
            mockActivityEventAggregator.VerifyAllExpectations();
            Assert.AreEqual( expectedArgs.Context, typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );
        }

        [Test]
        public void TestDisplayOkWarningMessageFor_WhenAccidentIsEmploymentRelated_ShouldDisplayEmploymentRelatedAccidentWarningMessage()
        {
            var accident = new Accident { Kind = TypeOfAccident.NewAuto() };
            accident.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL;
            var expectedArgs = new LooseArgs( new object() );
            var messageDisplayHandler = GetErrorMessageDisplayHandlerWithMockDependenciesFor( accident );
            var mockMessageBoxAdapter = messageDisplayHandler.MessageBoxAdapter;
            var mockActivityEventAggregator = messageDisplayHandler.EventAggregator;


            mockMessageBoxAdapter.Expect(
                x => x.ShowMessageBox( UIErrorMessages.NO_MEDICARE_FOR_EMPLOYMENT_RELATED_ACCIDENT_MSG,
                                        ErrorMessageDisplayHandler.CAPTION_WARNING, MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 ) ).Return( DialogResult.OK );


            //Rhino mocks has a defect when running under .net 4.0 where the following causing an exception
            //messageDisplayHandler.EventAggregator.AssertWasCalled(x => x.RaiseErrorMessageDisplayedEvent( Arg<IErrorMessageDisplayHandler>.Is.Anything , Arg<LooseArgs>.Matches(y=>y.Context==looseArgs.Context)));
            //the workaround I found was to use the WhenCalled method and get access to the argument passed and manually check it in an assert
            //http://blog.gmane.org/gmane.comp.windows.devel.dotnet.rhino-mocks/month=20100201
            //http://www.mail-archive.com/rhinomocks@googlegroups.com/msg01386.html

            mockActivityEventAggregator.Expect(
                x => x.RaiseErrorMessageDisplayedEvent( Arg<IErrorMessageDisplayHandler>.Is.Anything, Arg<LooseArgs>.Is.NotNull ) ).WhenCalled( x => expectedArgs = (LooseArgs)x.Arguments[1] );

            messageDisplayHandler.DisplayOkWarningMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );

            mockMessageBoxAdapter.VerifyAllExpectations();
            mockActivityEventAggregator.VerifyAllExpectations();
            Assert.AreEqual( expectedArgs.Context, typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );

        }

        #region Private Methods
        private ErrorMessageDisplayHandler GetErrorMessageDisplayHandlerWithMockDependenciesFor( Accident condition )
        {
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var activityEventAggregator = MockRepository.GenerateMock<IActivityEventAggregator>();

            Account account = new Account();
            account.Diagnosis.Condition = condition;

            return new ErrorMessageDisplayHandler( account, messageBoxAdapter, activityEventAggregator );
        }
        #endregion
    }
}
