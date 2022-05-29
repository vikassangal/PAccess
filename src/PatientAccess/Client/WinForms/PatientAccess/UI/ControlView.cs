using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Extensions.UI.Winforms
{
    [Serializable()]
    public class ControlView : UserControl, IView
    {
        #region Constants
        #endregion

        #region Event Handlers
        public virtual void Model_ChangedListeners( object sender, string aspect, object oldValue, object newValue )
        {
            this.UpdateView();
        }

        private void EnterEventHandler( object sender, EventArgs e )
        {
            FocusInControl = true;
            AttachAcceptButton();
        }

        private void LeaveEventHandler( object sender, EventArgs e )
        {
            FocusInControl = false;
            if( IsManagingAcceptButton && (this.ParentForm != null ) )
            {
                this.ParentForm.AcceptButton = SavedAcceptButton;
            }
        }
        
        #endregion

        #region Methods
        public virtual void UpdateModel()
        {
        }

        public virtual void UpdateView()
        {
        }

        public void EnableThemesOn( Control aControlAndItsChildren )
        {
            this.ViewImpl.EnableThemesOn( aControlAndItsChildren );
        }

        protected override void Dispose( bool disposing )
        {
            try
            {
                if( disposing && !this.InvokeRequired )
                {
                    Application.DoEvents();

                    foreach( IDisposable disposableControl in this.Controls )
                    {
                        if( disposableControl != null )
                        {
                            disposableControl.Dispose();
                        }
                    }
                }

                base.Dispose( disposing );
            }
            catch
            {
                // no error... we are trying to GC our controls
            }
        }
        #endregion

        #region Properties
        [Browsable( false )]
        public object Model
        {
            get
            {
                return this.ViewImpl.Model;
            }
            set
            {
                this.ViewImpl.Model = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the Component is currently in design mode.
        /// </summary>
        [Browsable( false )]
        protected new bool DesignMode
        {
            get
            {
                Control control = this;
                bool designMode = base.DesignMode;
 
                while( ( control.Parent != null) && ( designMode == false ) )
                {
                    control = control.Parent;
                    designMode = ( ( control.Site != null ) && control.Site.DesignMode);
                }
 
                return designMode;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the Component is currently in design mode.
        /// </summary>
        [Browsable( false )]
        public bool IsInDesignMode
        {
            get
            {
                return this.DesignMode;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the Component is currently in runtime mode.
        /// </summary>
        [Browsable( false )]
        public bool IsInRuntimeMode
        {
            get
            {
                return this.DesignMode == false;
            }
        }

        /// <summary>
        /// Gets or sets the button on the form that is clicked when the user presses the ENTER key.
        /// </summary>
        [DefaultValue( null )]
        public IButtonControl AcceptButton
        {
            get
            {
                if( this.DisableAcceptButton )
                {
                    return null;
                }
                else
                {
                    return i_AcceptButton;
                }
            }
            set
            {
                i_AcceptButton = value;

                if( this.DesignMode == false )
                {
                    this.Enter += new EventHandler( this.EnterEventHandler );
                    this.Leave += new EventHandler( this.LeaveEventHandler );
                    this.AttachAcceptButton();
                }
            }
        }

        [DefaultValue( false )]
        public bool DisableAcceptButton
        {
            private get
            {
                return i_DisableAcceptButton;
            }
            set
            {
                i_DisableAcceptButton = value;
            }
        }
        #endregion

        #region Private Methods
        internal void NotifyListeners( object model )
        {
            if( model != null )
            {
                Model m = model as Model;
                if( m != null )
                {
                    m.ChangedListeners += new Changed( Model_ChangedListeners );
                }
            }
        }

        private void AttachAcceptButton()
        {
            if( IsManagingAcceptButton && (this.ParentForm != null) )
            {
                SavedAcceptButton = this.ParentForm.AcceptButton;
                this.ParentForm.AcceptButton = AcceptButton;
            }
        }

        #endregion

        #region Private Properties
        private ViewImpl ViewImpl
        {
            get
            {
                return this.i_ViewImpl;
            }
        }

        private bool IsManagingAcceptButton
        {
            get
            {
                return ( AcceptButton != null ) || DisableAcceptButton;
            }
        }

        private IButtonControl SavedAcceptButton
        {
            get
            {
                return i_SavedAcceptButton;
            }
            set
            {
                i_SavedAcceptButton = value;
            }
        }

        private bool FocusInControl
        {
            set
            {
                i_FocusInControl = value;
            }
        }

        #endregion

        #region Construction and Finalization
        public ControlView() : base()
        {
            i_ViewImpl = new ViewImpl( this );
        }
        #endregion

        #region Data Elements
        private ViewImpl        i_ViewImpl;
        
        private IButtonControl  i_AcceptButton          = null;
        private IButtonControl  i_SavedAcceptButton     = null;

        private bool            i_DisableAcceptButton   = false;
        private bool            i_FocusInControl        = false;

        #endregion
    }
}