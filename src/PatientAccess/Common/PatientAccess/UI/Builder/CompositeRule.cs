using System;
using System.Collections;

namespace Extensions.UI.Builder
{
    [Serializable]
    public abstract class CompositeRule : LeafRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void Add( IRule aRule )
        {
            if( aRule == this )
            {
                throw new ArgumentException( "A CompositeRule cannot be added to itself", "aRule" );
            }

            if( !this.i_Constituents.Contains( aRule ) )
            {
                this.i_Constituents.Add( aRule );
            }
        }

        public IRule RuleWith( long oid )
        {
            IRule foundRule = null;

            if( this.Oid == oid )
            {
                foundRule = this;
            }
            else
            {
                foreach( IRule rule in this.Constituents )
                {
                    CompositeRule composite = rule as CompositeRule;
                    if( null != composite )
                    {
                        foundRule = composite.RuleWith( oid );
                        if( null != foundRule )
                        {
                            break;
                        }
                    }
                    else
                    {
                        if( rule.Oid == oid )
                        {
                            foundRule = rule;
                            break;
                        }
                    }
                }    
            }

            return foundRule;
        }
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
                foreach( IRule rule in this.Constituents )
                {
                    if( rule is CompositeRule )
                    {
                        constituentComposites.Add( rule );
                    }
                }

                return constituentComposites;
            }
        }
        #endregion

        #region Construction and Finalization
        public CompositeRule()
        {
        }
        #endregion

        #region Data Elements
        private ArrayList i_Constituents = new ArrayList();
        #endregion

        #region Constants
        #endregion
    }
}
