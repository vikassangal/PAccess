using System;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.DiagnosisViews.Specialized;
using PatientAccess.UI.Specialized;

namespace PatientAccess.UI.Factories
{
    /// <summary>
    /// The ViewFactory is responsible for the creation of any PatientAccess
    /// view class. Creation of the view is first delegated to any matching
    /// rules
    /// </summary>
    public class ViewFactory
    {
		#region Fields 

        /// <summary>
        /// Singleton instance of this class
        /// </summary>
        private static volatile ViewFactory _instance = new ViewFactory();
        private static volatile object _patientAccessViewInstanceLock = new object();

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Hides the default contructor to force the usage of
        /// the Instance property
        /// </summary>
        internal ViewFactory(){}

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets the singlton instance of this factory
        /// </summary>
        /// <value>The instance.</value>
        public static ViewFactory Instance
        {
            get
            {
                return _instance;
            }
        }
        /// <summary>
        /// Gets or sets the patient access view instance.
        /// </summary>
        /// <value>The patient access view instance.</value>
        private static PatientAccessView PatientAccessViewInstance { get; set; }


        /// <summary>
        /// Gets the patient access view instance lock.
        /// </summary>
        /// <value>The patient access view instance lock.</value>
        private static object PatientAccessViewInstanceLock
        {
            get
            {
                return _patientAccessViewInstanceLock;
            }
        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Creates a view of type T using optional context information
        /// </summary>
        /// <typeparam name="T">Type of view to create (can be base type)</typeparam>
        /// <returns>a new view of type T</returns>
        public T CreateView<T>() where T : Control, new()
        {
            // Create the default view, we'll eat the overhead if it is changed
            // by a later handler
            T newView = null;

            // Handle specialized cases
            HandleViewCreationForClinicalTrialFacility(ref newView);
            // more handers here ...

            // Build default if no handler built a view
            if (newView == null)
            {
                newView = new T();
            }
            
            return newView;
        }


        /// <summary>
        /// Handles the view creation for clinical trial facility.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newView">The new view.</param>
        /// <remarks>This implementation seems pretty sub-par :(</remarks>
        private static void HandleViewCreationForClinicalTrialFacility<T>( ref T newView ) where T:Control
        {
            Facility facility = User.GetCurrent().Facility;

            if( facility == null )
            {
                return;
            }

            Type requestedType = typeof( T );
   
            if( requestedType.Equals( typeof( DiagnosisView ) ) && 
                facility.HasExtendedProperty( ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED ) )
            {
                newView = 
                    new ClinicalTrialsBoardDiagnosisView() as T;
            }
            else if( requestedType.Equals( typeof( PatientAccessView ) ) )
            {
                // Singleton aspect of the view makes this a little clunky
                if (facility.HasExtendedProperty(ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED))
                {
                    newView = 
                        CreatePatientAccessView<ClinicalTrialsBoardPatientAccessView>() as T;
                }
                else
                {
                    newView = 
                        CreatePatientAccessView<PatientAccessView>() as T;
                }
            }
        }


        /// <summary>
        /// Creates the clinical trails board patient access view.
        /// </summary>
        /// <returns></returns>
        private static PatientAccessView CreatePatientAccessView<T>() where T:PatientAccessView, new()
        {
            lock( PatientAccessViewInstanceLock )
            {
                if( PatientAccessViewInstance == null || PatientAccessViewInstance.IsDisposed )
                {
                    PatientAccessViewInstance = new T();
                }
            }

            return PatientAccessViewInstance;
        }

		#endregion Methods 
    }
}