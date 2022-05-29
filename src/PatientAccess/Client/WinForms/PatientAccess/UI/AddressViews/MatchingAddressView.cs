using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.AddressViews
{
    /// <summary>
    /// Summary description for MatchingAddressView.
    /// </summary>
	[Serializable]
	public class MatchingAddressView : CoverControlView
	{
		#region Event Handlers
		public event EventHandler AddressSelected;

		private void lbxMatchingAddresses_SelectedIndexChanged(object sender, EventArgs e)
		{
			Address selectedAddress = (Address)this.lbxMatchingAddresses.SelectedItem;
			this.AddressSelected( this, new LooseArgs( selectedAddress ) );
		}

		private void lbxMatchingAddresses_DrawItem(object sender, DrawItemEventArgs e)
		{

			if( e.Index >= 0 )
			{
				Rectangle bounds = e.Bounds;
				Color textColor = SystemColors.ControlText;

				// render background and select text color based on selected and focus states
				if( (e.State & DrawItemState.Selected) == DrawItemState.Selected )
				{
					if( (e.State & DrawItemState.Focus) == DrawItemState.Focus )
					{
						e.DrawBackground();
						e.DrawFocusRectangle();         
					}
					else
					{
						using ( Brush b = new SolidBrush(SystemColors.InactiveCaption) )
						{
							e.Graphics.FillRectangle(b, e.Bounds);
						}   
					}
					textColor = SystemColors.HighlightText;
				}
				else
				{
					e.DrawBackground();
				}

                if( this.Ignoring )
                {
                    textColor = SystemColors.GrayText;
                }
                
				using(SolidBrush textBrush = new SolidBrush( textColor ))               
               // using(SolidBrush textBrush =  (SolidBrush)System.Drawing.SystemBrushes.InactiveCaption )
				{                                        
					Address address = (Address)lbxMatchingAddresses.Items[ e.Index ];
					int top = 2;

					renderLine(e, address.Address1, textBrush, ref top);
					string addr2 = address.Address2;
					if( addr2.Length > 0 )
					{
						renderLine(e, address.Address2, textBrush, ref top);
					}

                    // Make sure the zip code is broken out and displayed appropriately

				    string formattedAddress;
                    
                    if ( address.County != null && address.County.Code != string.Empty )
				    {
				        formattedAddress = String.Format
				            ( "{0}, {1}, {2}-{3}\nCounty: {4}",
				              address.City,
				              address.State,
				              address.ZipCode.ZipCodePrimary,
				              address.ZipCode.ZipCodeExtended,
				              address.County
				            );
				    }

                    else
                    {
                        formattedAddress = String.Format
                            ( "{0}, {1}, {2}-{3}",
                              address.City,
                              address.State,
                              address.ZipCode.ZipCodePrimary,
                              address.ZipCode.ZipCodeExtended
                            );
                    }

					renderLine(e, formattedAddress, textBrush, ref top);

				}        
			}
		}

		private void renderLine(DrawItemEventArgs e, string text, SolidBrush TextBrush, ref int top)
		{
			if( top+e.Font.Height < e.Bounds.Height  )
			{
				e.Graphics.DrawString(
					text,
					e.Font,
					TextBrush,
					e.Bounds.Left,
					e.Bounds.Top + top);

				top += e.Font.Height;
			}
		}

		#endregion

		#region Methods
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

		public void IgnoreMatchingAddresses()
		{		
            //this.PopulateAddressListBox();
            
            this.lbxMatchingAddresses.SelectedIndex = -1;
			this.lbxMatchingAddresses.Enabled = false;
			//this.lbxMatchingAddresses.ForeColor = Color.Gray;
		}

		public void SelectFirstListBoxItem()
		{
			if( this.lbxMatchingAddresses.Items.Count > 0 )
			{
				//this.lbxMatchingAddresses.SelectedItem = 1;
				this.lbxMatchingAddresses.SelectedIndex = 0;
			}
			this.lbxMatchingAddresses.Enabled = true;
			//this.lbxMatchingAddresses.ForeColor = Color.Black;
		}

		public void SetTabOrder()
		{
			this.lbxMatchingAddresses.TabIndex = 1;
			this.lbxMatchingAddresses.TabStop = true;
		}

		public bool AddressListPopulated()
		{
			return ( this.lbxMatchingAddresses.Items.Count > 0 );
		}
		#endregion

		#region Properties

        public bool Ignoring
        {
            private get
            {
                return i_Ignoring;
            }
            set
            {
                i_Ignoring = value;
            }
        }

		public ICollection Model_Addresses
		{
			get
			{
				return (ICollection)this.Model_Addresses;
			}
			set
			{
				IModel_Addresses = value;
			}
		}

		/// <summary>
		/// UpdateView method.
		/// </summary>
		public override void UpdateView()
		{
            BreadCrumbLogger.GetInstance.Log( "Address verification results received" );
			PopulateAddressListBox();
		}

		/// <summary>
		/// UpdateModel method.
		/// </summary>
		public override void UpdateModel()
		{   

		}

		public void ResetMatchingAddressView()
		{
			this.lbxMatchingAddresses.Items.Clear();
		}
		#endregion

		#region Private Methods
		
        public void PopulateAddressListBox()
		{
			this.ShowCover = false;
			if( IModel_Addresses != null )
			{
				lbxMatchingAddresses.Items.Clear();
				if( IModel_Addresses.Count > 0 )
				{
					foreach( Address address in IModel_Addresses )
					{
						lbxMatchingAddresses.Items.Add( address );
					}
					if( lbxMatchingAddresses.Items.Count > 0 )
					{
						lbxMatchingAddresses.SetSelected( 0, true);
						lbxMatchingAddresses.Enabled = true;
					}
					this.lbxMatchingAddresses.ScrollAlwaysVisible = true;
				}
				else
				{
					lbxMatchingAddresses.Enabled = true;
				}
//				else
//				{
//					this.lbxMatchingAddresses.ScrollAlwaysVisible = false;
//					this.ShowCover = true;
//				}
				this.lbxMatchingAddresses.Focus();
			}
			this.Refresh();
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbxMatchingAddresses = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lbxMatchingAddresses
			// 
			this.lbxMatchingAddresses.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.lbxMatchingAddresses.IntegralHeight = false;
			this.lbxMatchingAddresses.ItemHeight = 55;
			this.lbxMatchingAddresses.Location = new System.Drawing.Point(0, 0);
			this.lbxMatchingAddresses.Name = "lbxMatchingAddresses";
			this.lbxMatchingAddresses.ScrollAlwaysVisible = true;
			this.lbxMatchingAddresses.Size = new System.Drawing.Size(450, 94);
			this.lbxMatchingAddresses.TabIndex = 0;
			this.lbxMatchingAddresses.TabStop = false;
			this.lbxMatchingAddresses.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxMatchingAddresses_DrawItem);
			this.lbxMatchingAddresses.SelectedIndexChanged += new System.EventHandler(this.lbxMatchingAddresses_SelectedIndexChanged);
			// 
			// MatchingAddressView
			// 
			this.Controls.Add(this.lbxMatchingAddresses);
			this.CoverMessage = "No addresses match your entry.  Use the address entered (if complete), or enter a" +
				" new address to verify.";
			this.CoverPadding = 30;
			this.Name = "MatchingAddressView";
			this.ShowCover = false;
			this.Size = new System.Drawing.Size(451, 94);
			this.Controls.SetChildIndex(this.lbxMatchingAddresses, 0);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public MatchingAddressView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}
		#endregion

		#region Data Elements
		private Container components = null;
		private ListBox lbxMatchingAddresses;
		private ICollection IModel_Addresses;
        private bool i_Ignoring = false;
		#endregion

		#region Constants

		#endregion
	}
}
