using System.Collections;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.UI.Reports.FaceSheet
{
    public class PrintService
    {
        #region Event Handlers

        #endregion

        #region Methods

        public void Print()
        {
            var dataBuilder = new DataBuilder( iAccount );
            
            dataBuilder.SsnFormatter = x => x.AsFormattedMaskedString();

            var faceSheetData = GetFaceSheetData( dataBuilder );

            string templateUrl = GetTemplateUrl();

            var templateFiller = new TemplateFiller();

            Printer.GetInstance().Print( faceSheetData, templateUrl, templateFiller );
        }

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        private string GetTemplateUrl ()
        {
            string url = ConfigurationManager.AppSettings[ApplicationConfigurationKeys.PATIENTACCESS_APPSERVER];

            if ( iAccount.IsShortRegisteredNonDayCareAccount() )
            {
                url += SHORT_REGISTRATION_TEMPLATE;
            }

            else
            {
                url += REGULAR_REGISTRATION_TEMPLATE;
            }

            return url;
        }

        private IDictionary GetFaceSheetData ( DataBuilder dataBuilder )
        {
            IDictionary data;

            if (iAccount.IsShortRegisteredNonDayCareAccount())
            {
                data = new ShortRegistrationFaceSheetDataCreator( dataBuilder ).GetData();
            }

            else
            {
                data = new RegistrationFaceSheetDataCreator( dataBuilder ).GetData();
            }
            return data;
        }

        #endregion
        
        #region Construction and Finalization

        /// 
        /// <param name="anAccount"></param>
        public PrintService( IAccount anAccount )
        {
            iAccount = anAccount;
        }

        #endregion

        #region Data Elements

        private readonly IAccount iAccount;
        private const string SHORT_REGISTRATION_TEMPLATE = "ShortRegistrationFaceSheet.htm";
        private const string REGULAR_REGISTRATION_TEMPLATE = "FaceSheet.htm";

        #endregion
    }
}