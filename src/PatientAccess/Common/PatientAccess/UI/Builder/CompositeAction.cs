using System;
using System.Collections;

namespace Extensions.UI.Builder
{
    [Serializable]
    public abstract class CompositeAction : LeafAction, IAction
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void Add( IAction anAction )
        {
            if( anAction == this )
            {
                throw new ArgumentException( "A CompositeAction cannot be added to itself", "anAction" );
            }

            if( !this.i_Constituents.Contains( anAction ) )
            {
                this.i_Constituents.Add( anAction );
            }
        }

        public IAction ActionWith( long oid )
        {
            IAction foundAction = null;

            if( this.Oid == oid )
            {
                foundAction = this;
            }
            else
            {
                foreach( IAction action in this.Constituents )
                {
                    CompositeAction composite = action as CompositeAction;
                    if( null != composite )
                    {
                        foundAction = composite.ActionWith( oid );
                        if( null != foundAction )
                        {
                            break;
                        }
                    }
                    else
                    {
                        if( action.Oid == oid )
                        {
                            foundAction = action;
                            break;
                        }
                    }
                }    
            }

            return foundAction;
        }

        public void Remove( IAction anAction )
        {
            if( this.i_Constituents.Contains( anAction ) )
            {
                this.i_Constituents.Remove( anAction );
            }
        }

        public int NumberOfAllLeafActions()
        {
            int count = 0;
            foreach( IAction action in this.Constituents )
            {
                CompositeAction composite = action as CompositeAction;
                if( composite == null )
                {
                    count++;
                }
                else
                {
                    count += composite.NumberOfAllLeafActions();
                }
            }

            return count;
        }

        public abstract override void Execute();
        #endregion

        #region Properties
        public ICollection Constituents 
        {
            get
            {
                return (ICollection)this.i_Constituents.Clone();
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        protected ICollection DirectConstituentComposites
        {
            get
            {
                ArrayList constituentComposites = new ArrayList();
                foreach( IAction action in this.Constituents )
                {
                    if( action is CompositeAction )
                    {
                        constituentComposites.Add( action );
                    }
                }

                return constituentComposites;
            }
        }
        #endregion

        #region Construction and Finalization
        public CompositeAction()
        {
        }

        public CompositeAction( object context ) : base( context )
        {
        }
        #endregion

        #region Data Elements
        private ActionsList i_Constituents = new ActionsList();
        #endregion

        #region Constants
        #endregion
    }
}
