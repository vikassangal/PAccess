using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for ClinicView.
	/// </summary>
	/// 
	//TODO: Create XML summary comment for ClinicView
	[Serializable]
	public class ClinicView : ControlView
	{
		#region Events
        public event EventHandler HospitalClinicSelected;
        public event CancelEventHandler HospitalClinicValidating;
		#endregion

		#region Event Handlers  
		public void cmbClinic_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedHospitalClinic = this.cmbClinic.SelectedItem as HospitalClinic;
//			if( selectedHospitalClinic != null )
//			{
				this.HospitalClinicSelected( this, new LooseArgs( selectedHospitalClinic ) );
//			}
		}
        private void cmbClinic_Validating( object sender, CancelEventArgs e )
        {
            this.HospitalClinicValidating( this, null );        
        }
		#endregion

		#region Methods
		public override void UpdateView()
		{
			this.PopulateClinics();
		}

		public void SetClinicValueFromModel( HospitalClinic hospitalClinic)
		{
			if( hospitalClinic != null )
			{
                this.cmbClinic.SelectedItem = hospitalClinic;
			}
		}

		/// <summary>
		/// Return the ArrayList used to populate the clinic combo box.
		/// </summary>
		public ArrayList AllHospitalClinics()
		{
			return i_AllClinics;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The label text is the text that will be displayed for the label.
		/// </summary>
		public string LabelText
		{
			get
			{
				return this.lblClinic.Text;
			}
			set
			{
				this.lblClinic.Text = value;
			}
		}

		/// <summary>
		/// Facility parameter to be used to return all clinics for that facility.
		/// </summary>
		public Facility PatientFacility
		{
			private get
			{
				return i_Facility;
			}
			set
			{
				i_Facility = value;
			}
		}

		/// <summary>
		/// ArrayList that could be used to populate the clinics combo box instead of calling the broker.
		/// </summary>
		public ArrayList ListOfClinics
		{
			private get
			{
				return i_AllClinics;
			}
			set
			{
				i_AllClinics = value;
			}
		}
		#endregion

		#region Private Methods
		private void PopulateClinics()
		{
			if( ListOfClinics == null || ListOfClinics.Count == 0 )
			{
				//IHospitalClinicsBroker clinicBroker = BrokerFactory.BrokerOfType<IHospitalClinicsBroker >();
				//ListOfClinics = ( ArrayList )clinicBroker.HospitalClinicsFor( PatientFacility.Oid );
                IHospitalClinicsBroker clinicBroker = new HospitalClinicsBrokerProxy();
                ListOfClinics = (ArrayList)clinicBroker.HospitalClinicsFor( PatientFacility.Oid );
				//ListOfClinics = ( ArrayList )clinicBroker.HospitalClinicsFor( this.i_Facility.Oid );
			}
		    
			cmbClinic.Items.Clear();
		    
			HospitalClinic clinic = new HospitalClinic();
			for( int i = 0; i < ListOfClinics.Count; i++ )
			{
				clinic = (HospitalClinic) ListOfClinics[i];

				cmbClinic.Items.Add( clinic );                           
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.cmbClinic = new PatientAccessComboBox();
            this.lblClinic = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbClinic
            // 
            this.cmbClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClinic.Location = new System.Drawing.Point(48, 0);
            this.cmbClinic.Name = "cmbClinic";
            this.cmbClinic.Size = new System.Drawing.Size(193, 21);
            this.cmbClinic.TabIndex = 0;
            this.cmbClinic.Validating += new System.ComponentModel.CancelEventHandler(this.cmbClinic_Validating);
            this.cmbClinic.SelectedIndexChanged += new System.EventHandler(this.cmbClinic_SelectedIndexChanged);
            // 
            // lblClinic
            // 
            this.lblClinic.Location = new System.Drawing.Point(2, 4);
            this.lblClinic.Name = "lblClinic";
            this.lblClinic.Size = new System.Drawing.Size(46, 14);
            this.lblClinic.TabIndex = 1;
            this.lblClinic.Text = "Clinic:";
            // 
            // ClinicView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblClinic);
            this.Controls.Add(this.cmbClinic);
            this.Name = "ClinicView";
            this.Size = new System.Drawing.Size(242, 22);
            this.ResumeLayout(false);

        }
		#endregion

		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public ClinicView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
		private Label lblClinic;
		public  PatientAccessComboBox cmbClinic;
		private HospitalClinic		selectedHospitalClinic;
		private Facility	i_Facility;
		private ArrayList	i_AllClinics;
		#endregion

       
		#region Constants
		#endregion
	}
}
