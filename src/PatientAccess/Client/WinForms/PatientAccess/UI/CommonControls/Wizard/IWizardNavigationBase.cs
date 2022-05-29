namespace PatientAccess.UI.CommonControls.Wizard
{
	/// <summary>
	/// Interface for classes that inherit from WizardNavigationBase
	/// </summary>
	
    public interface IWizardNavigationBase
	{
        void AddNavigation( string navName, string pageName );
        void AddNavigation( string navName, WizardPage aPage );
        void AddNavigation( string navName, FunctionDelegate functionDelegate );
       
        void RemoveNavigation( string navName);

        void UpdateNavigation( string navName, WizardPage aPage );
        void UpdateNavigation( string navName, string pageName );
        void UpdateNavigation( string navName, FunctionDelegate functionDelegate );
	}

    public delegate void                    FunctionDelegate();
}
