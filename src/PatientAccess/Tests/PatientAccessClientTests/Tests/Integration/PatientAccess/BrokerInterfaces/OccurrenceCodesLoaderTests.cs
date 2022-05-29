using System;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    [TestFixture]
    [Category( "Fast" )]
    public class OccurrenceCodesLoaderTests
    {
        #region Constants
        #endregion
        
        #region Test Methods
        [Test()]
        [Ignore()]
        public void TestOccurrenceCodeManager()
        {
            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();

            PatientSearchCriteria criteria = new PatientSearchCriteria(
                "DEL",
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                "5129966"
                );

            PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor( criteria );

            Patient aPatient = patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0] );

            AccountProxy accountProxy = aPatient.Accounts[0] as AccountProxy;
            
            Account realAccount = accountProxy.AsAccount();

            //Test Illnes
            Illness ill = new Illness();
            ill.Onset = DateTime.Now;
         
            OccurrenceCode foundOC = null;
            foreach( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if(oc.Code == OccurrenceCode.OCCURRENCECODE_ILLNESS)
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull(foundOC );
            
            Assert.AreEqual(
                2,
                realAccount.OccurrenceCodes.Count
                );
           

            //Pregnancy Tests
            Pregnancy p = new Pregnancy();
            p.LastPeriod =  DateTime.Now;
        
            foundOC = null;
            foreach( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if(oc.Code == OccurrenceCode.OCCURRENCECODE_LASTMENSTRUATION)
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull(foundOC );
            Assert.AreEqual(
                2,
                realAccount.OccurrenceCodes.Count
                );


            //Crime Tests
            Crime cr = new Crime();
            cr.OccurredOn =  DateTime.Now;
        
            foundOC = null;
            foreach( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if(oc.Code == OccurrenceCode.OCCURRENCECODE_CRIME)
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull(foundOC );
            Assert.AreEqual(
                2,
                realAccount.OccurrenceCodes.Count
                );


            //Accident Tests
            Accident a = new Accident();
            a.Kind = new TypeOfAccident();
            a.Kind.Description ="Auto";
            a.Kind.Code =  OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;
            a.Kind.OccurrenceCode =  new OccurrenceCode(0L,DateTime.Now,"Auto",
                                                      OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO);
            a.OccurredOn =  DateTime.Now;
            foundOC = null;
            foreach( OccurrenceCode oc in realAccount.OccurrenceCodes )
            {
                if(oc.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO)
                {
                    foundOC = oc;
                }
            }
            Assert.IsNotNull(foundOC );
            Assert.AreEqual(
                2,
                realAccount.OccurrenceCodes.Count
                );
           
        }
           

        
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}