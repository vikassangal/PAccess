using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for RelationshipView.
    /// </summary>
    //TODO-AC clean this class after release, delete unused properties
    [Serializable]
    public class RelationshipView : ControlView
    {
        #region Event Handlers
        public event EventHandler RelationshipSelected;
        public event CancelEventHandler RelationshipValidating;

        private void cmbRelationship_Validating(object sender, CancelEventArgs e)
        {
            if( RelationshipValidating != null )
            {
                RelationshipValidating( this, null );        
            }          
        }

        private void cmbRelationship_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;

            object key = new object();

            if( cb.SelectedIndex >= 0 )
            {
                DictionaryEntry selectedItem = (DictionaryEntry) cb.SelectedItem;
                key = selectedItem.Key;
            }

            if( RelationshipSelected == null )
            {
                key = new Relationship();                
            }

            RelationshipSelected( this, new LooseArgs( key ) );
        }
        #endregion

        #region Methods


        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            InitializeRelationshipComboBox();

            UIColors.SetNormalBgColor( cmbRelationship );

            if( Model_Relationships != null && Model_Relationships.Count > 0 )
            {
                foreach( Relationship relationship in Model_Relationships )
                {
                    if( relationship != null && relationship.Type != null )
                    {
                        cmbRelationship.SelectedItem = relationship.Type.AsDictionaryEntry();
                    }
                    break;
                }
            }

            if( lblRelationship.Text.Length > 0 )
            {   // The user of this control assigns the label text, so reposition the 
                // ComboBox to that it is at the proper distance from the text.
                SuspendLayout();
                Graphics g = CreateGraphics();
                SizeF size = g.MeasureString( lblRelationship.Text, Font ).ToSize();
                int xOffect = (int)size.Width + CONTROL_SPACE;
                cmbRelationship.Location = new Point( xOffect, 1 );
                ResumeLayout( true );
            }
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }

        /// <summary>
        /// Reset Relationship combobox
        /// </summary>
        public void ResetView()
        {
            UIColors.SetNormalBgColor( cmbRelationship );
            cmbRelationship.SelectedIndex = -1;
        }
        #endregion

        #region Properties
        [Browsable(false)]
        private ICollection Model_Relationships
        {
            get
            {
                return (ICollection)Model;
            }
        }

        [Browsable(false)]
        public string RelationshipName
        {
            get
            {
                return i_RelationshipName;
            }
            set
            {
                i_RelationshipName = value;
            }
        }

        [Browsable(false)]
        public string PatientIs
        {
            get
            {
                return i_PatientIs;
            }
            set
            {
                i_PatientIs = value;
            }
        }

        [Browsable(false)]
        public string LabelText
        {
            get
            {
                return lblRelationship.Text;
            }
            set
            {
                lblRelationship.Text = value;
            }
        }

        [Browsable(false)]
        public PatientAccessComboBox ComboBox
        {
            get
            {
                return cmbRelationship;
            }
        }

        public Party PartyForRelationships
        {
            private get
            {
                return i_PartyForRelationships;
            }
            set
            {
                i_PartyForRelationships = value;
            }
        }

        #endregion

        #region Private Methods
        private void InitializeRelationshipComboBox()
        {
            if( IsInRuntimeMode ) 
            {
                ICollection relationships;

                IRelationshipTypeBroker broker = new RelationshipTypeBrokerProxy() ;

                if( PartyForRelationships == null )
                {
                    relationships = new ArrayList();
                }
                else
                {
                    Type partyType      = PartyForRelationships.GetType();
                    string typeOfRole   = string.Empty;

                    if( partyType != null )
                    {
                        typeOfRole = partyType.ToString();
                    }

                    switch( typeOfRole )
                    {
                        case "PatientAccess.Domain.Parties.Guarantor":
                            relationships = broker.AllTypesOfRelationships( User.GetCurrent().Facility.Oid, TypeOfRole.Guarantor );
                            break;
                        case "PatientAccess.Domain.Parties.Insured":
                            relationships = broker.AllTypesOfRelationships( User.GetCurrent().Facility.Oid,TypeOfRole.Insured);
                            break;
                        case "PatientAccess.Domain.Parties.EmergencyContact":
                            relationships = broker.AllTypesOfRelationships( User.GetCurrent().Facility.Oid,TypeOfRole.EmergencyContact );
                            break;
                        case "PatientAccess.Domain.Parties.Patient":
                            relationships = broker.AllTypesOfRelationships( User.GetCurrent().Facility.Oid,TypeOfRole.Patient );
                            break;

                        default:
                            relationships = new ArrayList();
                            break;
                    }
                }
            
                cmbRelationship.Items.Clear();

                cmbRelationship.ValueMember   = "Key";
                cmbRelationship.DisplayMember = "Value";

                foreach( RelationshipType relationship in relationships )
                {
                    cmbRelationship.Items.Add( relationship.AsDictionaryEntry() );
                }
                cmbRelationship.Sorted = true;
            }
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRelationship = new System.Windows.Forms.Label();
            this.cmbRelationship = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // lblRelationship
            // 
            this.lblRelationship.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblRelationship.Location = new System.Drawing.Point(2, 5);
            this.lblRelationship.Name = "lblRelationship";
            this.lblRelationship.Size = new System.Drawing.Size(161, 13);
            this.lblRelationship.TabIndex = 0;
            // 
            // cmbRelationship
            // 
            this.cmbRelationship.BackColor = System.Drawing.SystemColors.Window;
            this.cmbRelationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRelationship.Location = new System.Drawing.Point(163, 1);
            this.cmbRelationship.Name = "cmbRelationship";
            this.cmbRelationship.Size = new System.Drawing.Size(178, 21);
            this.cmbRelationship.TabIndex = 1;
            this.cmbRelationship.Validating += new System.ComponentModel.CancelEventHandler(this.cmbRelationship_Validating);
            this.cmbRelationship.SelectedIndexChanged += new System.EventHandler(this.cmbRelationship_SelectedIndexChanged);
            // 
            // RelationshipView
            // 
            this.Controls.Add(this.cmbRelationship);
            this.Controls.Add(this.lblRelationship);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.Name = "RelationshipView";
            this.Size = new System.Drawing.Size(345, 24);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public RelationshipView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private Label      lblRelationship;
        private PatientAccessComboBox           cmbRelationship;
        private string                          i_RelationshipName;
        private string                          i_PatientIs;
        private Party                           i_PartyForRelationships = null;

        #endregion

        #region Constants
        const int CONTROL_SPACE = 1; // Distance between end of label text & X coordinate of ComboBox
        #endregion
      
    }
}