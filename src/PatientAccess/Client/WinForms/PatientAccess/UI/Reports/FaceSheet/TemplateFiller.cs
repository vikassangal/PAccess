using System;
using System.Collections;
using System.Windows.Forms;

namespace PatientAccess.UI.Reports.FaceSheet
{
    public class TemplateFiller
    {
        public void FillTemplateWith ( IDictionary accountData, HtmlDocument htmlDocument )
        {
            foreach ( object key in accountData.Keys )
            {
                if (accountData[key] != null)
                {
                    SetFieldValue( key.ToString(), accountData[key].ToString(), htmlDocument );
                }
                else
                {
                    SetFieldValue( key.ToString(), String.Empty, htmlDocument );
                }
            }
        }

        /// 
        /// <param name="field"></param>
        /// <param name="fieldValue"></param>
        /// <param name="htmlDocument"></param>
        private void SetFieldValue( string field, string fieldValue, HtmlDocument htmlDocument )
        {
            HtmlElement label = htmlDocument.All[field];

            if( label != null )
            {
                label.InnerText = fieldValue;
            }
        }
    }
}