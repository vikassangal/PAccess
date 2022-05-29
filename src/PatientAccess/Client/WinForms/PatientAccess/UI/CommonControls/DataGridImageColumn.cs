using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using PatientAccess.UI.WorklistViews;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for DataGridImageColumn.
	/// </summary>
	[Serializable]
    public class DataGridImageColumn : DataGridColumnStyle
    {

        protected override void Abort(int rowNum)
        {            
        }

        protected override bool Commit(CurrencyManager dataSource, int rowNum)
        {           
            return true;
        }

        protected override int GetMinimumHeight() 
        {
            //
            // return here your minimum height
            //
            return 16;
        }
        
        protected override int GetPreferredHeight(Graphics g ,object Value) 
        {
            //
            // return here your preferred height
            //
            return 16;
        }

        protected override Size GetPreferredSize(Graphics g, object Value) 
        {
            //
            // return here your preferred size
            //
            Size PicSize = new Size(16,16);
            return PicSize;
        }

        protected override void Edit(
            CurrencyManager source,
            int rowNum,
            Rectangle bounds,
            bool readOnly,
            string displayText,
            bool cellIsVisible)
        {            
        }

        protected override void Paint(Graphics g,Rectangle Bounds,CurrencyManager Source,int RowNum) 
        {
            SolidBrush BackBrush = new SolidBrush(Color.White);

            Bitmap ImagePic = this.getImage(Source, RowNum);
             
            g.FillRectangle(BackBrush, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            g.DrawImage((Image) ImagePic, Bounds.X + ((Bounds.Width - ImagePic.Width)>>1), Bounds.Y, ImagePic.Width, ImagePic.Height);
        }

        protected override void Paint(Graphics g,Rectangle Bounds,CurrencyManager Source,int RowNum,bool AlignToRight) 
        {
            SolidBrush BackBrush = new SolidBrush(Color.White);

            Bitmap ImagePic = this.getImage(Source, RowNum);

            g.FillRectangle(BackBrush, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            g.DrawImage((Image) ImagePic, Bounds.X + ((Bounds.Width - ImagePic.Width)>>1), Bounds.Y, ImagePic.Width, ImagePic.Height);
        }

        protected override void Paint(Graphics g,Rectangle Bounds,CurrencyManager Source,int RowNum, Brush BackBrush ,Brush ForeBrush ,bool AlignToRight) 
        {            
            Bitmap ImagePic = this.getImage(Source, RowNum);

            g.FillRectangle(BackBrush, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            g.DrawImage((Image) ImagePic, Bounds.X + ((Bounds.Width - ImagePic.Width)>>1), Bounds.Y, ImagePic.Width, ImagePic.Height);
        }

        private Bitmap getImage(CurrencyManager Source,int RowNum)
        {
            string isLocked = string.Empty;

            object o = GetColumnValueAtRow(Source, RowNum);  

            if( o != null )
            {
                isLocked = (string)o;
            }

            Bitmap ImagePic = null;
            ImageList il = new ImageList();
            ResourceManager         rm         = new ResourceManager(typeof(WorklistView));

            ImageListStreamer strm = (ImageListStreamer)rm.GetObject("imlLock.ImageStream");
            ImageList newImageList = new ImageList();
            newImageList.ImageStream = strm;
            

            if( isLocked == "Y" )
            {
                ImagePic = (Bitmap)newImageList.Images[0];
            }
            else
            {
                ImagePic = (Bitmap)newImageList.Images[1];
            }

            return ImagePic;
        }
    }
}
