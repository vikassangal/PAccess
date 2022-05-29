using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OnlinePreRegistrationDataCreator.PatientAccess.OnlinePreRegistration;
using PatientAccess.Persistence.OnlinePreregistration;
using Tests.Unit.PatientAccess.Services.PreRegistration;
using Tests.Utilities.OnlinePreRegistration;

namespace OnlinePreRegistrationDataCreator
{
    class Program
    {
        private static MessageBuilder GetBuilderForRequiredDataOnly()
        {
            return new MessageBuilder( GetDataFromEmbeddedResource( SampleMessageWithRequiredDataOnly ) );
        }

        private static MessageBuilder GetBuilderForRequiredAndOptionalData()
        {
            return new MessageBuilder( GetDataFromEmbeddedResource( SampleMessageWithRequiredAndOptionalData ) );
        }

        private static IList<string> MessagesToSend { get; set; }
        private static PreRegistrationHandlerServiceSoapClient MessageReceiver { get; set; }
        private const string SampleMessageWithRequiredAndOptionalData = "Tests.Resources.OnlinePreRegistration.SamplePreRegistration.xml";
        private const string SampleMessageWithRequiredDataOnly = "Tests.Resources.OnlinePreRegistration.SamplePreRegistrationRequiredDataOnly.xml";

        static Program()
        {
            MessagesToSend = new List<string>();
            MessageReceiver = new PreRegistrationHandlerServiceSoapClient();
        }

        static void Main( string[] args )
        {
            Console.WriteLine( "Deleting all submissions" );

            DeleteAllSubmissions();

            Console.WriteLine( "All submissions deleted" );

            CreateMessages();
            CreateMesssagesWithVariousLastNamesAndAdmitDates();

            Console.WriteLine( "Sending messages ..." );
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //                        SendMessages();
            SendMessagesConcurrently();
            stopwatch.Stop();

            Console.WriteLine( "Messages sent" );

            var duration = stopwatch.Elapsed;
            Console.WriteLine( "Duration: " + duration );
            Console.WriteLine( "Press enter to exit" );

            Console.ReadLine();
        }

        private static void CreateMessages()
        {
            var sample1 = GetBuilderForRequiredAndOptionalData().BuildMessage();

            var sample2 = GetBuilderForRequiredDataOnly().BuildMessage();

            var sample3 = GetBuilderForRequiredDataOnly()
                .SetPatientName( "ACCT", "TESTD" )
                .SetMale()
                .SetPatientDateOfBirth( new DateTime( 1980, 10, 10 ) )
                .SetVisitedBefore( true )
                .SetPatientSsn( "999-99-9999" )
                .SetPatientAddressUS( "2300 W PLANO PKWY", "PLANO", "TX", "75075" )
                .SetPatientAdmitDate( new DateTime( 2010, 10, 01 ) )
                .BuildMessage();

            var sample4 = GetBuilderForRequiredDataOnly()
                .SetPatientName( "AGE", "TESTD" )
                .SetMale()
                .SetPatientDateOfBirth( new DateTime( 1990, 08, 19 ) )
                .SetVisitedBefore( false )
                .SetPatientSsn( "999-99-9999" )
                .SetPatientAddressUS( "2300 W PLANO PKWY", "PLANO", "AK", "75075" )
                .SetPatientAdmitDate( new DateTime( 2010, 10, 11 ) )
                .BuildMessage();

            var sample5 = GetBuilderForRequiredAndOptionalData()
                .SetPatientName( "SPANS7071", "TESTD" )
                .SetFemale()
                .SetPatientDateOfBirth( new DateTime( 1989, 01, 01 ) )
                .SetVisitedBefore( false )
                .SetPatientSsn( "999-99-9999" )
                .SetPatientAddressUS( "6908 WELLESLEY DR", "PLANO", "TX", "75024" )
                .SetPatientAdmitDate( new DateTime( 2010, 10, 11 ) )
                .BuildMessage();

            var sample6 = GetBuilderForRequiredAndOptionalData()
                .SetPatientName( "FIRST", "UNINSURED" )
                .SetFemale()
                .SetPatientDateOfBirth( new DateTime( 1999, 09, 09 ) )
                .SetVisitedBefore( true )
                .SetPatientSsn( "111-22-3333" )
                .SetPatientAddressUS( "6000 ABC DR", "PLANO", "TX", "75000" )
                .SetPatientAdmitDate( new DateTime( 2010, 11, 11 ) )
                .SetNoPrimaryAndSecondaryInsurance()
                .BuildMessage();

            var sample7 = GetBuilderForRequiredAndOptionalData()
                .SetPatientName( "SECOND", "UNINSURED" )
                .SetFemale()
                .SetPatientDateOfBirth( new DateTime( 1995, 05, 05 ) )
                .SetVisitedBefore( true )
                .SetPatientSsn( "222-22-3333" )
                .SetPatientAddressUS( "7777 DEB DR", "DALLAS", "TX", "75757" )
                .SetPatientAdmitDate( new DateTime( 2010, 12, 12 ) )
                .SetInsuranceNotSelected()
                .BuildMessage();

            //for other language field testing in a California hospital (ACO)
            var sample8 = GetBuilderForRequiredAndOptionalData()
                .SetFacilityCode( "ACO" )
                .SetOtherLanguage( "SOME-OTHER LANGUAGE," )
                .SetPatientName( "AGE", "TESTD" )
                .SetMale()
                .SetPatientDateOfBirth( new DateTime( 1990, 08, 19 ) )
                .SetVisitedBefore( false )
                .SetPatientSsn( "999-99-9999" )
                .SetPatientAddressUS( "2300 W PLANO PKWY", "PLANO", "AK", "75075" )
                .SetPatientAdmitDate( new DateTime( 2010, 10, 11 ) )
                .BuildMessage();
            
            //this message with a non existent facility should not generate an exception, it should just be logged
            var sample9 = GetBuilderForRequiredAndOptionalData()
                .SetFacilityCode( "YYY" )
                .SetOtherLanguage( "SOME-OTHER LANGUAGE," )
                .SetPatientName( "AGE", "TESTD" )
                .SetMale()
                .SetPatientDateOfBirth( new DateTime( 1990, 08, 19 ) )
                .SetVisitedBefore( false )
                .SetPatientSsn( "999-99-9999" )
                .SetPatientAddressUS( "2300 W PLANO PKWY", "PLANO", "AK", "75075" )
                .SetPatientAdmitDate( new DateTime( 2010, 10, 11 ) )
                .BuildMessage();


            MessagesToSend = new List<string> { sample1, sample2, sample3, sample4, sample5, sample6, sample7, sample8, sample9 };
        }
        private static void CreateMesssagesWithVariousLastNamesAndAdmitDates()
        {
            string[] allLettersOfTheAlphabet = Enumerable.Range( 0, 26 ).Select( i => ( (char)( 'A' + i ) ).ToString() ).ToArray();

            for ( int i = 0; i < allLettersOfTheAlphabet.Length; i++ )
            {
                var letter = allLettersOfTheAlphabet[i];
                var sample1 = GetBuilderForRequiredAndOptionalData()
                    .SetPatientName( "FIRST NAME", letter + "LAST NAME", "I" )
                    //make half of the entries start before today and the other half after today to help with date related testing
                    .SetPatientAdmitDate( DateTime.Today + TimeSpan.FromDays( i - allLettersOfTheAlphabet.Length / 2 ) )
                    .BuildMessage();

                MessagesToSend.Add( sample1 );
            }
        }

        private static void SendMessagesConcurrently()
        {
            var tasks = MessagesToSend.Select( message => new Task( () => MessageReceiver.ProcessMessage( message, MessageProcessor.MessageType ) ) ).ToArray();

            foreach ( var task in tasks )
            {
                task.Start();
            }

            Task.WaitAll( tasks );
        }

        private static void DeleteAllSubmissions()
        {
            using ( var sqlConnection = new SqlConnection( "Database=PatientAccessTrunkLocal;Server=localhost;Integrated Security=SSPI" ) )
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand( "DELETE from Messaging.PreRegistrationSubmissions", sqlConnection );
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }

        private static string GetDataFromEmbeddedResource( string embeddedResourceName )
        {
            var assembly = Assembly.GetAssembly( typeof( MessageProcessorTests ) );
            var manifestResourceStream = assembly.GetManifestResourceStream( embeddedResourceName );
            var streamReader = new StreamReader( manifestResourceStream );
            return streamReader.ReadToEnd();
        }
    }
}
