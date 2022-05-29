using System.Collections;
using PatientAccess.UI.Reports.FaceSheet;
using PatientAccess.Utilities;

namespace PatientAccess.UI.Reports
{
    public class RegistrationFaceSheetDataCreator
    {
        private IDataBuilder DataBuilder { get; set; }

        public RegistrationFaceSheetDataCreator( IDataBuilder dataBuilder )
        {
            DataBuilder = dataBuilder;
        }

        public Hashtable GetData()
        {
            var data = new Hashtable();

            DataBuilder.SetFacility( data );
            DataBuilder.SetDemoGraphics( data );
            DataBuilder.SetDiagnosis( data );
            DataBuilder.SetClinical( data );
            DataBuilder.SetInsurance( data );
            DataBuilder.SetRegulatory(data);
            if ( !DataBuilder.IsAccountProxy() )
            {
                DataBuilder.SetGuarantor( data );
                DataBuilder.SetBilling( data );
                DataBuilder.SetContacts( data );
            }

            if ( !Pbar.IsAvailable() )
            {
                data.Add( "lblSystemMessage", "Partial Face Sheet. Limited patient information provided." );
            }

            return data;
        }
    }
}