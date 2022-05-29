using System;
using System.Collections;
using System.ComponentModel;
using Extensions.UI.Winforms;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for GenderControl.
    /// </summary>
    public class GenderControl : ControlView
    {
        #region Event Handlers
        public event EventHandler GenderSelectedEvent;
        public event CancelEventHandler GenderControlValidating;

        private void cmbGender_Validating(object sender, CancelEventArgs e)
        {
            if( this.GenderControlValidating != null )
            {
                this.GenderControlValidating( this, null );        
            }            
        }

        private void SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( GenderSelectedEvent != null )
            {
                if( cmbGender.SelectedItem != null )
                {
                    DictionaryEntry selectedItem = (DictionaryEntry)cmbGender.SelectedItem;
                    GenderSelectedEvent( this, new LooseArgs( selectedItem.Key ) );
                }
                else
                {
                    GenderSelectedEvent(this, null);
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// InitializeGendersComboBox() - Initialize the combobox with data
        /// </summary>
        public void InitializeGendersComboBox()
        {
            if( IsInRuntimeMode )
            {
                DemographicsBrokerProxy broker = new DemographicsBrokerProxy( );
                ICollection genders = broker.AllTypesOfGenders( User.GetCurrent().Facility.Oid );

                cmbGender.Items.Clear();

                cmbGender.ValueMember   = "Key";
                cmbGender.DisplayMember = "Value";

                foreach( Gender gender in genders )
                {
                    cmbGender.Items.Add( gender.AsDictionaryEntry() );
                }
            }
        }

        public void ResetView()
        {
            cmbGender.SelectedIndex = -1;
        }
        #endregion

        #region Properties
        public int Count
        {
            get
            {
                return cmbGender.Items.Count;
            }
        }

        public PatientAccessComboBox ComboBox
        {
            get
            {
                return cmbGender;
            }
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
        public GenderControl()
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
            this.cmbGender = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // cmbGender
            // 
            this.cmbGender.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.DropDownWidth = 74;
            this.cmbGender.Location = new System.Drawing.Point(0, 0);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(95, 21);
            this.cmbGender.TabIndex = 1;
            this.cmbGender.Validating += new System.ComponentModel.CancelEventHandler(this.cmbGender_Validating);
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // GenderControl
            // 
            this.Controls.Add(this.cmbGender);
            this.Name = "GenderControl";
            this.Size = new System.Drawing.Size(95, 24);
            this.ResumeLayout(false);

        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private PatientAccessComboBox   cmbGender;
        #endregion

        #region Constants
        #endregion
    }
}
