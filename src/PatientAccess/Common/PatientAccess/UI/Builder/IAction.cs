using Extensions.UI.Winforms;

namespace Extensions.UI.Builder
{
	public interface IAction
	{
        #region Event Handlers
        #endregion

        #region Methods
        void Execute();
        void Configure( IView aView );
        #endregion

        #region Properties
        
        long CompositeActionID
        {
            get;
            set;
        }

        object Context
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        long Oid
        {
            get;
            set;
        }
        #endregion
	}
}
