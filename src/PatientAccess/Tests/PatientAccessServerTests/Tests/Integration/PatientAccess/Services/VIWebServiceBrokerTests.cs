using System;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence;
using PatientAccess.Services;
using PatientAccess.VIWeb;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Tests.Integration.PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Services
{
    /// <summary>
    /// Summary description for VIWebServiceBrokerTests
    /// </summary>
    [TestFixture]
    public class VIWebServiceBrokerTests : AbstractBrokerTests 
    {

        #region Constants 

        private const string TEST_FACILITY_CODE = "ACO";
        private const string TEST_PLANID        = "081LA" ;
        private const long   TEST_MEDRECNUMBER  = 830006 ;

        #endregion Constants 

        #region Fields 

        MockRepository i_MockRepository ;
        VIWebServiceBroker i_ObjectUnderTest;
        IPreviousVisitServiceService i_PreviousVisitServiceMock;

        #endregion Fields 

        #region Methods 

        [SetUp]
        public void TestInitialize()
        {

            this.i_MockRepository = new MockRepository();
            this.i_PreviousVisitServiceMock = this.i_MockRepository.Stub<IPreviousVisitServiceService>();
            this.i_ObjectUnderTest = new VIWebServiceBroker( this.i_PreviousVisitServiceMock ) ;

            using( this.i_MockRepository.Record() )
            {

                this.SetPreviousVisitResponseFor( VIWebPreviousDocument.VIWebPreviousDocumentType.ADV );
                this.SetPreviousVisitResponseFor( VIWebPreviousDocument.VIWebPreviousDocumentType.DL );
                this.SetPreviousVisitResponseFor( VIWebPreviousDocument.VIWebPreviousDocumentType.NPP );
                this.SetPreviousVisitResponseFor( VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD );

            }//using

        }

        [Test]
        public void TestPreviousDocumentCount()
        {
            InsurancePlan plan = this.GetCommercialInsurancePlan();
            
            VIWebPreviousDocumentCnts results = 
                this.i_ObjectUnderTest.GetPreviousVisitDocumentCnts(TEST_FACILITY_CODE, TEST_MEDRECNUMBER, plan);

            Assert.AreEqual( 1, results.ADVDocumentCnt, "Should be Exactly 1 Advanced Directive Doc for this patient" );
            Assert.AreEqual( 1, results.DLDocumentCnt, "Should be Exactly 1 Drivers License Doc for this patient" );
            Assert.AreEqual( 1, results.NPPDocumentCnt, "Should be Exactly 1 NPP Doc for this patient" );
            Assert.AreEqual( 1, results.INSCardDocumentCnt, "Should be Exactly 1 Insurance Card for this patient" );

        }

        [Test]
        public void TestPreviousDocumentsReturnNonNullValuesNew()
        {

            InsurancePlan plan = this.GetCommercialInsurancePlan();

            // ADV
            VIWebPreviousDocuments results = 
                this.i_ObjectUnderTest.GetPreviousVisitDocuments(TEST_FACILITY_CODE, TEST_MEDRECNUMBER, plan);
            
            Assert.IsNotNull(results.PreviousDocumentList);

            List<VIWebPreviousDocument> advDocs = results.GetDocumentsWith(VIWebPreviousDocument.VIWebPreviousDocumentType.ADV);
            Assert.IsNotNull(advDocs, "No ADV Documents returned");
            Assert.AreEqual(1, advDocs.Count, "Wrong number of ADV Documents returned");
            VIWebPreviousDocument advDoc = advDocs[0];
            Assert.AreEqual(advDoc.DocumentID, "B07332AA.AAB_ADV", "ADV Document ID is not correct");

            List<VIWebPreviousDocument> dlDocs = results.GetDocumentsWith(VIWebPreviousDocument.VIWebPreviousDocumentType.DL);
            Assert.IsNotNull(dlDocs, "No DL Documents returned");
            Assert.AreEqual(1, dlDocs.Count, "Wrong number of DL Documents returned");
            VIWebPreviousDocument dlDoc = dlDocs[0];
            Assert.AreEqual(dlDoc.DocumentID, "B07332AA.AAB_DL", "DL Document ID is not correct");

            List<VIWebPreviousDocument> insDocs = results.GetDocumentsWith(VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD);
            Assert.IsNotNull(insDocs, "No InsuranceCard Documents returned");
            Assert.AreEqual(1, insDocs.Count, "Wrong number of InsuranceCard Documents returned");
            VIWebPreviousDocument insDoc = insDocs[0];
            Assert.AreEqual(insDoc.DocumentID, "B07332AA.AAB_INSCARD", "Ins Document ID is not correct");

            List<VIWebPreviousDocument> nppDocs = results.GetDocumentsWith(VIWebPreviousDocument.VIWebPreviousDocumentType.NPP);
            Assert.IsNotNull(nppDocs, "No NPP Documents returned");
            Assert.AreEqual(1, nppDocs.Count, "Wrong number of NPP Documents returned");
            VIWebPreviousDocument nppDoc = nppDocs[0];
            Assert.AreEqual(nppDoc.DocumentID, "B07332AA.AAB_NPP", "NPP Document ID is not correct");

        }

        private InsurancePlan GetCommercialInsurancePlan()
        {

            InsurancePlan plan = new CommercialInsurancePlan();
            
            plan.PlanCategory = new InsurancePlanCategory();
            plan.PlanCategory.Oid = InsurancePlanCategory.PLANCATEGORY_COMMERCIAL;
            plan.PlanName = "Commercial";
            
            return plan;

        }

        private PreviousVisitDocumentResponse SetPreviousVisitResponseFor( VIWebPreviousDocument.VIWebPreviousDocumentType aDocType )
        {

            DateTime docDate = new DateTime(2007, 12, 28, 16, 04, 18);
            PreviousVisitDocument aPreviousVisitDocument = new PreviousVisitDocument();
            aPreviousVisitDocument.accountNumber = "00424085";
            aPreviousVisitDocument.documentId = String.Format("B07332AA.AAB_{0}",aDocType);
            aPreviousVisitDocument.documentDate = docDate;

            PreviousVisitDocumentResponse response = new PreviousVisitDocumentResponse();
            response.documentsWereFound = true;
            response.docType = aDocType.ToString();
            response.hspCode = TEST_FACILITY_CODE;
            response.medicalRecordNumber = TEST_MEDRECNUMBER.ToString();
            response.previousVisitDocuments = new PreviousVisitDocument[1];
            response.previousVisitDocuments.SetValue(aPreviousVisitDocument, 0);

            SetupResult.For( this.i_PreviousVisitServiceMock.getPreviousVisitDocuments( null ) )
                .Constraints( Property.Value("docType", aDocType.ToString() ) )
                .Return( response )
                .Repeat.Once();

            return response;

        }

        #endregion Methods 

    }
}