using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.HelperClasses;

using PatientAccess.Rules;

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    partial class PhoneNumberControl
    {

        #region Events

        #endregion

        #region Event Handlers

       
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();

                RuleEngine.GetInstance().UnregisterEvent( typeof( PhoneNumberPrefersAreaCode ), this.Model, new EventHandler( PhoneNumberPrefersAreaCodeEventHandler ) );
                RuleEngine.GetInstance().UnregisterEvent( typeof( AreaCodeRequiresPhoneNumber ), this.Model, new EventHandler( AreaCodeRequiresPhoneNumberEventHandler ) );
            }
            base.Dispose( disposing );
        }

        #endregion       

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mtbPhoneNumber = new System.Windows.Forms.MaskedTextBox();
            this.mtbAreaCode = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // mtbPhoneNumber
            // 
            this.mtbPhoneNumber.AllowPromptAsInput = false;
            this.mtbPhoneNumber.Location = new System.Drawing.Point( 36, 3 );
            this.mtbPhoneNumber.Mask = "000-0000";
            this.mtbPhoneNumber.Name = "mtbPhoneNumber";
            this.mtbPhoneNumber.PromptChar = ' ';
            this.mtbPhoneNumber.ResetOnPrompt = false;
            this.mtbPhoneNumber.Size = new System.Drawing.Size( 52, 20 );
            this.mtbPhoneNumber.TabIndex = 2;
            this.mtbPhoneNumber.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mtbPhoneNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPhoneNumber_Validating );
            this.mtbPhoneNumber.MouseClick += new MouseEventHandler( mtbPhoneNumber_MouseClick );
            this.mtbPhoneNumber.TextChanged += new System.EventHandler(this.mtbPhoneNumber_TextChanged);
            // 
            // mtbAreaCode
            // 
            this.mtbAreaCode.AllowPromptAsInput = false;
            this.mtbAreaCode.Location = new System.Drawing.Point( 3, 3 );
            this.mtbAreaCode.Mask = "000";
            this.mtbAreaCode.Name = "mtbAreaCode";
            this.mtbAreaCode.PromptChar = ' ';
            this.mtbAreaCode.ResetOnPrompt = false;
            this.mtbAreaCode.Size = new System.Drawing.Size( 27, 20 );
            this.mtbAreaCode.TabIndex = 1;
            this.mtbAreaCode.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mtbAreaCode.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAreaCode_Validating );
            this.mtbAreaCode.KeyUp += new System.Windows.Forms.KeyEventHandler( this.mtbAreaCode_KeyUp );
            this.mtbAreaCode.MouseClick += new MouseEventHandler( mtbAreaCode_MouseClick );
            this.mtbAreaCode.TextChanged += new System.EventHandler(this.mtbAreaCode_TextChanged); 
            // 
            // PhoneNumberControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.mtbAreaCode );
            this.Controls.Add( this.mtbPhoneNumber );
            this.Name = "PhoneNumberControl";
            this.Size = new System.Drawing.Size( 94, 27 );
            this.Leave += new EventHandler( PhoneNumberControl_Leave );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        #region Data Elements
        
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MaskedTextBox mtbPhoneNumber;
        private System.Windows.Forms.MaskedTextBox mtbAreaCode;

        #endregion

        #region Constants
        #endregion
    }
}
