using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CommonControls
{
    public partial class PhoneNumberControl : ControlView
    {
        #region Events

        public event EventHandler AreaCodeChanged;
        public event EventHandler PhoneNumberChanged;
        public event EventHandler PhoneNumberTextChanged;
     
        #endregion

        #region Event Handlers


        void PhoneNumberControl_Leave( object sender, EventArgs e )
        {
            Model.AreaCode = mtbAreaCode.Text.Trim();
            Model.Number = mtbPhoneNumber.Text.Trim();
        }

        private void mtbAreaCode_Validating( object sender, CancelEventArgs e )
        {
            if( !i_RulesRegistered )
            {
                RegisterRules();
            }

            UIColors.SetNormalBgColor( mtbAreaCode );
            AreaCode = mtbAreaCode.Text.Trim();
            if ( AreaCode.Length > 0 && AreaCode.Length != 3 )
            {   // Prevent cursor from advancing to the next control
                e.Cancel = true;
                UIColors.SetErrorBgColor( mtbAreaCode );
                MessageBox.Show( UIErrorMessages.AREA_CODE_INVALID, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAreaCode.Focus();
                return;
            }
            else
            {
               
                Model.AreaCode = (string)AreaCode.Trim().Clone();

                RunRules();

                if( AreaCodeChanged != null )
                {
                    AreaCodeChanged( this, null );
                }
            }            
        }

        private void mtbPhoneNumber_Validating( object sender, CancelEventArgs e )
        {
            if( !i_RulesRegistered )
            {
                RegisterRules();
            }

            UIColors.SetNormalBgColor( mtbPhoneNumber );
            PhoneNumber = mtbPhoneNumber.Text.Trim();
            if( mtbPhoneNumber.Text.Trim().Length > 0 && mtbPhoneNumber.Text.Trim().Length != 7 )
            {   // Prevent cursor from advancing to the next control
                e.Cancel = true;
                UIColors.SetErrorBgColor( mtbPhoneNumber );
                MessageBox.Show( UIErrorMessages.PHONE_NUMBER_INVALID, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return;
            }
            else
            {
                
                Model.Number = (string)PhoneNumber.Trim().Clone();

                RunRules();

                if( PhoneNumberChanged != null )
                {
                    PhoneNumberChanged( this, null );
                }
            }


        }

        void mtbPhoneNumber_MouseClick( object sender, MouseEventArgs e )
        {
            if( string.IsNullOrEmpty( mtbPhoneNumber.Text ) )
            {
                mtbPhoneNumber.SelectAll();
            }
        }

        void mtbAreaCode_MouseClick( object sender, MouseEventArgs e )
        {
            if( string.IsNullOrEmpty( mtbAreaCode.Text ) )
            {
                mtbAreaCode.SelectAll();
            }
        }

        private void mtbAreaCode_KeyUp( object sender, KeyEventArgs e )
        {
            if( mtbAreaCode.Text.Length == 3 && mtbAreaCode.SelectionLength != 3 )
            {
                mtbPhoneNumber.Focus();
            }
        }

        private void PhoneNumberPrefersAreaCodeEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if( aControl == this )
            {
                UIColors.SetPreferredBgColor( mtbAreaCode );
            }
        }

        private void AreaCodeRequiresPhoneNumberEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if( aControl == this )
            {
                UIColors.SetRequiredBgColor( mtbPhoneNumber );
            }
        }

        private void mtbPhoneNumber_TextChanged(object sender, EventArgs e)
        {
            if (PhoneNumberTextChanged != null)
            {
                PhoneNumberTextChanged(this, null);
            }
        }

        private void mtbAreaCode_TextChanged(object sender, EventArgs e)
        {
            if (PhoneNumberTextChanged != null)
            {
                PhoneNumberTextChanged(this, null);
            }
        }

        #endregion

        #region Methods


        public bool HasError()
        {
            return mtbAreaCode.BackColor == UIColors.TextFieldBackgroundError
                   || mtbPhoneNumber.BackColor == UIColors.TextFieldBackgroundError;
        }

        public bool HasPhoneNumberError()
        {
            return mtbPhoneNumber.BackColor == UIColors.TextFieldBackgroundError;
        }

        public bool HasAreaCodeError()
        {
            return mtbAreaCode.BackColor == UIColors.TextFieldBackgroundError;
        }

        public void ClearText()
        {
            mtbAreaCode.Text = string.Empty;
            mtbPhoneNumber.Text = string.Empty;
        }

        private void RegisterRules()
        {
            i_RulesRegistered = true;

            RuleEngine.GetInstance().RegisterEvent( typeof( PhoneNumberPrefersAreaCode ), Model, new EventHandler( PhoneNumberPrefersAreaCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AreaCodeRequiresPhoneNumber ), Model, new EventHandler( AreaCodeRequiresPhoneNumberEventHandler ) );
        }

        public void RunRules()
        {
            if( !i_RulesRegistered )
            {
                RegisterRules();
            }

            UIColors.SetNormalBgColor( mtbAreaCode );
            UIColors.SetNormalBgColor( mtbPhoneNumber );

            RuleEngine.GetInstance().EvaluateRule( typeof( AreaCodeRequiresPhoneNumber ), Model, this );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhoneNumberPrefersAreaCode ), Model, this );
        }

        public void ToggleEnabled( bool toggleTo )
        {
            mtbAreaCode.Enabled        = toggleTo;
            mtbPhoneNumber.Enabled     = toggleTo;
        }



        public void FocusAreaCode()
        {
            mtbAreaCode.Focus();
        }

        public void FocusPhoneNumber()
        {
            mtbPhoneNumber.Focus();
        }



        public void SetVisible( bool isVisible )
        {
            Visible = isVisible;
        }

        public void SetAreaCodeRequiredColor()
        {
            UIColors.SetRequiredBgColor( mtbAreaCode );
        }

        public void SetAreaCodePreferredColor()
        {
            UIColors.SetPreferredBgColor( mtbAreaCode );
        }


        public void SetPhoneNumberRequiredColor()
        {
            UIColors.SetRequiredBgColor( mtbPhoneNumber );
        }
        public void SetPhoneNumberErrorColor()
        {
            UIColors.SetErrorBgColor(mtbPhoneNumber);
            UIColors.SetErrorBgColor(mtbAreaCode);
        }

        public void SetNumberErrorColor()
        {
            UIColors.SetErrorBgColor(mtbPhoneNumber); 
        }

        public void SetPhoneNumberPreferredColor()
        {
            UIColors.SetPreferredBgColor( mtbPhoneNumber );
        }


        public void SetNormalColor()
        {
            UIColors.SetNormalBgColor( mtbAreaCode );
            UIColors.SetNormalBgColor( mtbPhoneNumber );
        }

        #endregion

        #region Properties

        public new PhoneNumber Model
        {
            get
            {
                return i_PhoneNumber;
            }
            set
            {
                i_PhoneNumber = value ?? new PhoneNumber();

                if (i_PhoneNumber.AreaCode == null)
                {
                    i_PhoneNumber.AreaCode = string.Empty;
                }

                if (i_PhoneNumber.Number == null)
                {
                    i_PhoneNumber.Number = string.Empty;
                }

                mtbAreaCode.Text = i_PhoneNumber.AreaCode;
                mtbPhoneNumber.Text = i_PhoneNumber.Number;
            }
        }

        public string AreaCode
        {
            get
            {
                return mtbAreaCode.Text;
            }
            set
            {
                mtbAreaCode.Text = value;

                if( mtbAreaCode.Text == "0" )
                {
                    mtbAreaCode.Text = String.Empty;
                }
            }
        }

        public string PhoneNumber
        {
            get
            {
                return mtbPhoneNumber.Text;
            }
            set
            {
                mtbPhoneNumber.Text = value;

                if( mtbPhoneNumber.Text == "0" )
                {
                    mtbPhoneNumber.Text = String.Empty;
                }
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PhoneNumberControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements

        private PhoneNumber                     i_PhoneNumber = new PhoneNumber();
        private bool                            i_RulesRegistered;

        #endregion

        #region Constants
        #endregion
        
    }
}
