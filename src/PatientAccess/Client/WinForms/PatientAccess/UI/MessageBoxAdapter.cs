using System;
using System.Windows.Forms;

namespace PatientAccess.UI
{
    public class MessageBoxAdapter : IMessageBoxAdapter
    {
        public DialogResult ShowMessageBox( string messageText )
        {
            return MessageBox.Show( messageText );
        }

        public DialogResult ShowMessageBox( string messageText, string caption, MessageBoxButtons buttons, MessageBoxIcon icon )
        {
            return MessageBox.Show( messageText, caption, buttons, icon );
        }

        public DialogResult ShowMessageBox( string messageText, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton )
        {
            return MessageBox.Show( messageText, caption, buttons, icon, defaultButton );
        }

        public MessageBoxAdapterResult ShowMessageBox( string messageText, string caption, MessageBoxAdapterButtons buttons, MessageBoxAdapterIcon icon, MessageBoxAdapterDefaultButton defaultButton )
        {
            MessageBoxButtons winFormsButtons = ToWinFroms( buttons );

            MessageBoxIcon winFormsIcon = ToWinFormsIcon( icon );
            MessageBoxDefaultButton winFormsDefaultButton = ToWinFormsDefaultButton(defaultButton);
            var winResult = MessageBox.Show( messageText, caption, winFormsButtons, winFormsIcon, winFormsDefaultButton );

            return ToWinFormsDialogResult( winResult );
        }

        public DialogResult Show( string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton )
        {
            return MessageBox.Show( text, caption, buttons, icon, defaultButton );
        }

        private MessageBoxDefaultButton ToWinFormsDefaultButton ( MessageBoxAdapterDefaultButton defaultButton )
        {
            switch ( defaultButton )
            {
                case MessageBoxAdapterDefaultButton.Button1:
                    return MessageBoxDefaultButton.Button1;
                case MessageBoxAdapterDefaultButton.Button2:
                    return MessageBoxDefaultButton.Button2;
                case MessageBoxAdapterDefaultButton.Button3:
                    return MessageBoxDefaultButton.Button3;
                default:
                    throw new ArgumentOutOfRangeException( "defaultButton" );
            }
        }

        private MessageBoxIcon ToWinFormsIcon ( MessageBoxAdapterIcon icon )
        {
            switch ( icon )
            {
                case MessageBoxAdapterIcon.None:
                    return MessageBoxIcon.None;
                case MessageBoxAdapterIcon.Error:
                    return MessageBoxIcon.Error;
                case MessageBoxAdapterIcon.Question:
                    return MessageBoxIcon.Question;
                case MessageBoxAdapterIcon.Exclamation:
                    return MessageBoxIcon.Exclamation;
                case MessageBoxAdapterIcon.Asterisk:
                    return MessageBoxIcon.Asterisk;
                default:
                    throw new ArgumentOutOfRangeException( "icon" );
            }
        }

        private MessageBoxButtons ToWinFroms( MessageBoxAdapterButtons buttons )
        {
            switch ( buttons )
            {
                case MessageBoxAdapterButtons.OK:
                    return MessageBoxButtons.OK;
                case MessageBoxAdapterButtons.OKCancel:
                    return MessageBoxButtons.OKCancel;
                case MessageBoxAdapterButtons.AbortRetryIgnore:
                    return MessageBoxButtons.AbortRetryIgnore;
                case MessageBoxAdapterButtons.YesNoCancel:
                    return MessageBoxButtons.YesNoCancel;
                case MessageBoxAdapterButtons.YesNo:
                    return MessageBoxButtons.YesNo;
                case MessageBoxAdapterButtons.RetryCancel:
                    return MessageBoxButtons.RetryCancel;
                default:
                    throw new ArgumentOutOfRangeException( "buttons" );
            }
        }

        private MessageBoxAdapterResult ToWinFormsDialogResult( DialogResult dialogResult )
        {
            switch ( dialogResult )
            {
                case DialogResult.None:
                    return MessageBoxAdapterResult.None;
                case DialogResult.OK:
                    return MessageBoxAdapterResult.OK;
                case DialogResult.Cancel:
                    return MessageBoxAdapterResult.Cancel;
                case DialogResult.Abort:
                    return MessageBoxAdapterResult.Abort;
                case DialogResult.Retry:
                    return MessageBoxAdapterResult.Retry;
                case DialogResult.Ignore:
                    return MessageBoxAdapterResult.Ignore;
                case DialogResult.Yes:
                    return MessageBoxAdapterResult.Yes;
                case DialogResult.No:
                    return MessageBoxAdapterResult.No;
                default:
                    throw new ArgumentOutOfRangeException( "dialogResult" );
            }
        }
    }


}