using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PatientSearch
{
	/// <summary>
	/// Summary description for CommandButtonManager.
	/// </summary>
	//TODO: Create XML summary comment for CommandButtonManager
	[Serializable]
	public class CommandButtonManager : object
	{
		#region Events
		public event EventHandler ButtonClicked;
		#endregion

		#region Event Handlers
		private void genericButton_Clicked( object sender, EventArgs e)
		{
			if( ButtonClicked != null )
			{
				ButtonClicked( sender, e );
			}
		}
		#endregion

		#region Methods
        public void DisableButtons()
        {
            foreach( ClickOnceLoggingButton button in sortedCommands )
            {                
                button.Enabled = false;                
            }
        }

		public void LoadButtons( Activity activity )
		{
            sortedCommands = new ArrayList();

			if( activity == null )
			{
				return;
			}
			switch( activity.GetType().Name )
			{
				case "RegistrationActivity":
                    this.AddCommand("btnActivatePrereg", "  Acti&vate Account - Standard Registration");
			        this.AddCommand("btnActivatePreregShort", "  Activate Account - &Diagnostic Registration");
					this.AddCommand( "btnCreateNewAccount", "Create &New Account..." );
					this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
					this.AddCommand( "btnCancel", "Cancel" );
					break;
				case "PreRegistrationActivity":
					this.AddCommand( "btnCreateNewAccount", "Create &New Account..." );
					this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
					this.AddCommand( "btnCancel", "Cancel" );
					break;
                case "ShortRegistrationActivity":
                    this.AddCommand("btnActivatePrereg", "  Acti&vate Account - Standard Registration");
                    this.AddCommand("btnActivatePreregShort", "  Activate Account - &Diagnostic Registration");
                    this.AddCommand( "btnCreateNewAccount", "Create &New Account..." );
                    this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
                    this.AddCommand( "btnCancel", "Cancel" );
                    break;
                case "ShortPreRegistrationActivity":
                    this.AddCommand( "btnCreateNewAccount", "Create &New Account..." );
                    this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
                    this.AddCommand( "btnCancel", "Cancel" );
                    break;
                case "QuickAccountCreationActivity":
                       this.AddCommand( "btnCreateNewAccount", "Create &New Account..." );
                       this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
                       this.AddCommand( "btnCancel", "Cancel" );
                    break;
                case "PreMSERegisterActivity":
					this.AddCommand( "btnCreateNewAccount", "Create &New Account..." );
					this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
					this.AddCommand( "btnCancel", "Cancel" );
					break;
				case "PostMSERegistrationActivity":
					this.AddCommand( "btnPostMSE", "Comp&lete Post-MSE Registration" );
					this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
					this.AddCommand( "btnCancel", "Cancel" );
					break;
                case "PreAdmitNewbornActivity":
                    this.AddCommand( "btnCreatePreNewbornAccount", "Create Pre-Admit &Newborn Account..." );
                    this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
                    this.AddCommand( "btnCancel", "Cancel" );
                    break;
				case "AdmitNewbornActivity":
					this.AddCommand( "btnCreateNewbornAccount", "Create &Newborn Account..." );
					this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
					this.AddCommand( "btnCancel", "Cancel" );
					break;
				case "MaintenanceActivity":
                    this.AddCommand("btnConvertToDiagPrereg", "    Convert Account - Diagnostic Preregistration");
					this.AddCommand( "btnEditAccount", "Edit/Maintain &Account..." );
					this.AddCommand( "btnCancel", "Cancel" );
					break;
                case "UCCPreMSERegistrationActivity":
                    this.AddCommand("btnCreateNewAccount", "Create &New Account...");
                    this.AddCommand("btnEditAccount", "Edit/Maintain &Account...");
                    this.AddCommand("btnCancel", "Cancel");
                    break;
                case "UCCPostMseRegistrationActivity":
                    this.AddCommand("btnUCCPostMSE", "Comp&lete UC Post-MSE Registration");
                    this.AddCommand("btnEditAccount", "Edit/Maintain &Account...");
                    this.AddCommand("btnCancel", "Cancel");
                    break;
				case "TransferActivity":
				case "TransferBedSwapActivity":
				case "TransferInToOutActivity":	
				case "TransferOutToInActivity":
                case Activity.TransferErPatientToOutPatient:
                case Activity.TransferOutPatientToErPatient:
				case "CancelPreRegActivity":
				case "CancelInpatientDischargeActivity":
				case "CancelOutpatientDischargeActivity":
				case "CancelInpatientStatusActivity":
				case "EditDischargeDataActivity":
				case "EditRecurringDischargeActivity":
				case "DischargeActivity":
               
				case "PreDischargeActivity":
					this.AddCommand( "btnContinue", "Contin&ue >" );
					this.AddCommand( "btnCancel", "Cancel" );
					break;
                case "PAIWalkinOutpatientCreationActivity":
                    this.AddCommand("btnCreateNewAccount", "Create &New Account...");
                    this.AddCommand("btnEditAccount", "Edit/Maintain &Account...");
                    this.AddCommand("btnCancel", "Cancel");
                    break;
				default:
					MessageBox.Show("Activity Not Selected!");
					break;
			}
		}

	    private void AddCommand( string name, string caption )
		{
			int tabIndx = 5;

			ClickOnceLoggingButton b = new ClickOnceLoggingButton();

            b.Enabled = false;
			b.Text = caption;
			b.Name = name;
			b.TabIndex = ( tabIndx + commands.Count );

            this.SetButtonSize(b);
                
			b.BackColor = SystemColors.Control;
			b.Click += new EventHandler( genericButton_Clicked );
			commands.Add( name, b );

            sortedCommands.Add( b );
		}


	    /// <summary>
	    /// SetButtonSize - determine the width of a string based on the specified font
	    /// </summary>
	    /// <param name="b"></param>
	    /// <returns></returns>
	    private void SetButtonSize( ClickOnceLoggingButton b )
        {
            Graphics graphics = b.CreateGraphics();
            StringFormat format = new StringFormat();
            RectangleF rect = new RectangleF(0, 0,
                1000, 1000);
            CharacterRange[] ranges = 
                                       {
                                           new CharacterRange(0, 
                                           b.Text.Length) };
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(b.Text, b.Font, rect, format);
            rect = regions[0].GetBounds(graphics);
            b.Width = (int)(rect.Right + 20.0f);
            if ( b.Name == "btnActivatePrereg" || b.Name == "btnActivatePreregShort" || b.Name == "btnConvertToDiagPrereg" )
            {
                //make button text two lines.
                b.Width = 135;
                b.Height += 9;
            }
        }

		public void SetPanel( Panel panel )
		{			
			sortedCommands.Reverse();

            foreach( ClickOnceLoggingButton button in sortedCommands )
			{
				int xposition = GetStartingXLocation( panel );
                //If its height <=23, move it lower, so it can align with 'Back to Search' and 'Refresh' buttons
                button.SetBounds(xposition - (button.Width + spacing), button.Height>23?0:3, button.Width, button.Height);
				panel.Controls.Add( button );
                button.Visible = true;
                if (button.Name != "btnCancel")
                    button.Enabled = false;
                button.BringToFront();
			}

            sortedCommands.Reverse();
		}

		public void RemoveButtonsFromPanel( Panel panel )
		{
			ArrayList buttonList = new ArrayList( commands.Values );

			foreach( ClickOnceLoggingButton button in buttonList )
			{
				panel.Controls.Remove( button );
			}
		}

		public ClickOnceLoggingButton Command( string name)
		{
			return (ClickOnceLoggingButton)commands[name];
		}

		#endregion

		#region Properties
		#endregion

		#region Private Methods
		private int GetStartingXLocation( Panel panel )
		{
			int x = panel.Width;

			
			foreach( ClickOnceLoggingButton button in panel.Controls )
			{
                if( button.Location.X < x )
                {
                    x = button.Location.X;
                }				
			}

			return x;
		}

		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public CommandButtonManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion

		#region Data Elements

		private Hashtable commands = new Hashtable();
		private int spacing = 7;
        private ArrayList sortedCommands;

		#endregion

		#region Constants
		#endregion
	}
}
