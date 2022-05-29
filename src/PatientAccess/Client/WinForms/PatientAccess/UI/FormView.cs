using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Extensions.UI.Winforms
{
    [Serializable()]
    public class FormView : Form, IView
    {
        #region Event Handlers
        public virtual void Model_ChangedListeners( object sender, string aspect, object oldValue, object newValue )
        {
            this.UpdateView();
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
        #endregion

        #region Private Properties
        private ViewImpl ViewImpl
        {
            get
            {
                return this.i_ViewImpl;
            }
        }
        #endregion

        #region Construction and Finalization
        public FormView() : base()
        {
            i_ViewImpl = new ViewImpl( this );
        }
        #endregion

        #region Data Elements
        private ViewImpl    i_ViewImpl;
        #endregion

        #region Constants
        #endregion
    }
}
