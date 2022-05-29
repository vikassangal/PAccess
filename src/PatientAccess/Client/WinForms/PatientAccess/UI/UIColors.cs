using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using PatientAccess.Rules;

namespace PatientAccess.UI
{
    /// <summary>
    /// Summary description for UIColors.
    /// </summary>
    public class UIColors
    {
        #region delegates        
        delegate void SetBgColorCallBack( Control ctrl );
        #endregion

        public static Color TextFieldBackgroundError        = Color.FromArgb( 255, 0, 0 );
        public static Color TextFieldBackgroundRequired     = Color.FromArgb( 250, 250, 100 );
        public static Color TextFieldBackgroundPreferred    = Color.FromArgb( 198, 233, 255 );
        private static Color TextFieldBackgroundNormal       = Color.White;
        private static Color TextFieldBackgroundDeactivated  = Color.FromArgb( 255, 0, 0 );
        public static Color FormBackColor                   = Color.FromArgb( 209, 228, 243 );

        
        /// <summary>
        /// setNormalBgColor - set the background color to 'normal' for the control passed.
        /// </summary>
        /// <param name="field"></param>
        public static void SetNormalBgColor( Control field )
        {
            if (field.InvokeRequired)
            {
                field.Invoke(new SetBgColorCallBack( SetNormalBgColor ), new object[] { field });
            }
            else
            {
                field.BackColor = Color.White;
                field.Refresh();

                RuleEngine.GetInstance().RemoveGenericError( field.Name );
            }
        }
        public static void SetNormalBgColor(UltraToolbarsManager field)
        {
            field.Appearance.BackColor = Color.White;
        }

        /// <summary>
        /// setRequiredBgColor - set the background color to the required field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        public static void SetRequiredBgColor( Control field )
        {
            if (field.InvokeRequired)
            {
                field.Invoke(new SetBgColorCallBack( SetRequiredBgColor ), new object[] { field });
            }
            else
            {
                if (field.Enabled)
                {
                    if (field.BackColor != TextFieldBackgroundError)
                    {
                        field.BackColor = TextFieldBackgroundRequired;
                        field.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// setPreferredBgColor - set the background color to the preferred field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        public static void SetPreferredBgColor( Control field )
        {
            if (field.InvokeRequired)
            {
                field.Invoke(new SetBgColorCallBack( SetPreferredBgColor ), new object[] { field }); 
            }
            else
            {
                if (field.Enabled)
                {
                    if (field.BackColor != TextFieldBackgroundError
                        && field.BackColor != TextFieldBackgroundRequired)
                    {
                        field.BackColor = TextFieldBackgroundPreferred;
                        field.Refresh();
                    }
                }
            }           
        }

        /// <summary>
        /// setErrorBgColor - set the background color to the error field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        public static void SetDeactivatedBgColor( Control field )
        {
            if (field.InvokeRequired)
            {
                field.Invoke(new SetBgColorCallBack( SetDeactivatedBgColor ), new object[] { field });
            }
            else
            {
                if (field.Enabled)
                {
                    field.BackColor = TextFieldBackgroundDeactivated;
                    field.Refresh();
                }
            }
        }
        public static void SetDeactivatedBgColor(UltraToolbarsManager field)
        {
            field.Appearance.BackColor = TextFieldBackgroundDeactivated;
        }

        public static void SetRequiredBgColor(UltraToolbarsManager field)
        {
            if (field.Enabled)
            {
                if (field.Appearance.BackColor != TextFieldBackgroundError)
                {
                    field.Appearance.BackColor = TextFieldBackgroundRequired;
                }
            }

        }

        public static void SetPreferredBgColor(UltraToolbarsManager field)
        {
            if (field.Enabled)
            {
                if (field.Appearance.BackColor != TextFieldBackgroundError
                    && field.Appearance.BackColor != TextFieldBackgroundRequired)
                {
                    field.Appearance.BackColor = TextFieldBackgroundPreferred;
                }
            }
        }

        /// <summary>
        /// setErrorBgColor - set the background color to the error field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        public static void SetErrorBgColor( Control field )
        {
            if (field.InvokeRequired)
            {
                field.Invoke(new SetBgColorCallBack( SetErrorBgColor ), new object[] { field });
            }
            else
            {
                if (field.Enabled)
                {
                    field.BackColor = TextFieldBackgroundNormal;
                    field.Refresh();
                    field.BackColor = TextFieldBackgroundError;
                    field.Refresh();

                    // add a generic action to the the failed actions collection so that we can prevent navigation

                    RuleEngine.GetInstance().AddGenericError(field.Name);
                }
            }
        }

        /// <summary>
        /// SetDisabledDarkBgColor - set the background color to the disabled controls like combobox.
        /// </summary>
        /// <param name="field"></param>
        public static void SetDisabledDarkBgColor( Control field )
        {
            if (field.InvokeRequired)
            {
                field.Invoke( new SetBgColorCallBack( SetDisabledDarkBgColor ), new object[] { field } ) ; 
            }
            else
            {
                field.BackColor = SystemColors.Control;
                field.Refresh();
            }
        }

        /// <summary>
        /// SetTabControlHeaderDisabledColor - Grey out the tab headers of a tab control.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="graphics">The Graphics surface to draw the item on.</param>
        /// <param name="bounds">The bounding rectangle for the specified tab.</param>
        public static void SetTabControlHeaderDisabledColor(Control field, Graphics graphics, Rectangle bounds )
        {
            TextRenderer.DrawText(graphics, field.Text, field.Font, bounds, SystemColors.GrayText);
        }

        /// <summary>
        /// SetTabControlHeaderEnabledColor - Reset the color of tab headers after being disabled..
        /// </summary>
        /// <param name="field"></param>
        /// <param name="graphics">The Graphics surface to draw the item on.</param>
        /// <param name="bounds">The bounding rectangle for the specified tab.</param>
        public static void SetTabControlHeaderEnabledColor(Control field, Graphics graphics, Rectangle bounds)
        {
            TextRenderer.DrawText(graphics, field.Text, field.Font, bounds, field.ForeColor);
        }

    }
}
