using System.ComponentModel;
using Extensions.UI.Winforms;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Appearance = Infragistics.Win.Appearance;
using System.Drawing;


namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for PatientAccessTreeDropdownBox.
    /// </summary>
    public class PatientAccessTreeDropdownBox : ControlView
    {
        #region Properties
       
        public UltraToolbarsManager UltraToolbarsManager
        {
            get
            {
                return ultraToolbarsManager;
            }
        }
        public UltraDropDownButton UltraDropDownButton
        {
            get
            {
                return ultraDropDownButton;
            }
        }
        public ButtonTool ButtonTool
        {
            get
            {
                return buttonTool;
            }
            set { buttonTool = value; }
        }

        public PopupMenuTool PopupMenuTool
        {
            get { return popupMenuTool; }
            set { popupMenuTool = value; }
        }
        #endregion

        #region Private Methods
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Construction and Finalization
        public PatientAccessTreeDropdownBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            popupMenuTool = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("PopupMenuTool");
            this.ultraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.ultraDropDownButton = new Infragistics.Win.Misc.UltraDropDownButton();
            this.ultraToolbarsManager.DesignerFlags = 1;
            this.ultraToolbarsManager.ShowFullMenusDelay = 500;
            ((ISupportInitialize)(this.ultraToolbarsManager)).BeginInit();
            //
            // ultraDropDownButton
            // 
            this.ultraDropDownButton.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            this.ultraDropDownButton.Location = new System.Drawing.Point(0, 0);
            this.ultraDropDownButton.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.ultraDropDownButton.Name = "ultraDropDownButton";
            this.ultraDropDownButton.PopupItemKey = "popupMenuTool";
            this.ultraDropDownButton.PopupItemProvider = this.ultraToolbarsManager;
            this.ultraDropDownButton.RightAlignPopup = Infragistics.Win.DefaultableBoolean.True;
            this.ultraDropDownButton.ShowOutline = false;
            this.ultraDropDownButton.Size = new System.Drawing.Size(131, 21);
            this.ultraDropDownButton.WrapText = false;
            this.ultraDropDownButton.Style = Infragistics.Win.Misc.SplitButtonDisplayStyle.DropDownButtonOnly;
            this.ultraDropDownButton.SupportThemes = false;
            ultraToolbarsManager.ImageSizeSmall = new Size(-7, 0);
            Appearance headerAppearance = new Appearance();
            headerAppearance.BackColor = Color.LightGray;
            headerAppearance.TextHAlign = HAlign.Left;
            ultraDropDownButton.Appearance = headerAppearance;
            this.ultraToolbarsManager.Tools.Add(popupMenuTool);
            // 
            // PatientAccessTreeDropdownBox
            // 
            this.Name = "PatientAccessTreeDropdownBox";
            this.Size = new System.Drawing.Size(95, 24);
            this.ResumeLayout(false);

        }

     
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private UltraToolbarsManager ultraToolbarsManager;
        private UltraDropDownButton ultraDropDownButton;
        private PopupMenuTool popupMenuTool;
        private ButtonTool buttonTool;

        #endregion

        #region Constants
        #endregion
    }
}
