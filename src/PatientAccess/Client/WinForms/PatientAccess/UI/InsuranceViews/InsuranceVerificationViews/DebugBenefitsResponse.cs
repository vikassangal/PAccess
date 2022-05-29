using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    public partial class DebugBenefitsResponse : Form
    {

        private void btnSetup_Click( object sender, EventArgs e )
        {
            this.pageSetupDialog1.Document = printDocument1;
            this.pageSetupDialog1.ShowDialog();
        }

        private void btnPrint_Click( object sender, EventArgs e )
        {           
            printDocument1.PrinterSettings.FromPage = 1;
            printDocument1.PrinterSettings.ToPage = 5;

            printDocument1.Print();            
        }

        [DllImport( "gdi32.dll" )]
        private static extern bool BitBlt(

             IntPtr hdcDest, // handle to destination DC 
             int nXDest, // x-coord of destination upper-left corner 
             int nYDest, // y-coord of destination upper-left corner 
             int nWidth, // width of destination rectangle 
             int nHeight, // height of destination rectangle 
             IntPtr hdcSrc, // handle to source DC 
             int nXSrc, // x-coordinate of source upper-left corner 
             int nYSrc, // y-coordinate of source upper-left corner 
             Int32 dwRop // raster operation code 

         ); 

        private void printDocument1_PrintPage( object sender, PrintPageEventArgs e )
        {
            Graphics graphic = this.CreateGraphics();
            Size s = this.Size;
            Image memImage = new Bitmap( s.Width, s.Height, graphic );
            Graphics memGraphic = Graphics.FromImage( memImage );
            IntPtr dc1 = graphic.GetHdc();
            IntPtr dc2 = memGraphic.GetHdc();

            BitBlt( 
                    dc2, 
                    0, 
                    0, 
                    1000,   //this.ClientRectangle.Width,
                    3500,  // this.ClientRectangle.Height, 
                    dc1, 
                    0, 
                    0, 
                    13369376 );

            graphic.ReleaseHdc( dc1 );

            memGraphic.ReleaseHdc( dc2 );

            e.Graphics.DrawImage( memImage, 0, 0 );
            e.HasMorePages = true;

        }

        public void UpdateView()
        {
            if( this.Model_Coverage != null
                && this.Model_Coverage.DataValidationTicket != null
                && this.Model_Coverage.DataValidationTicket.ResponseText != null )
            {
                this.richTextBox1.Text = this.Model_Coverage.DataValidationTicket.ResponseText;
                this.debugCommMgdCareVerifyView1.Model_Coverage = this.Model_Coverage;
                this.debugCommMgdCareVerifyView1.UpdateView();
            }
        }

        public DebugBenefitsResponse()
        {
            InitializeComponent();
        }

        private CommercialCoverage i_Model_Coverage;

        public CommercialCoverage Model_Coverage
        {
            private get
            {
                return i_Model_Coverage;
            }
            set
            {
                i_Model_Coverage = value;
            }
        }

    }
}