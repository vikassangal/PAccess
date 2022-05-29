using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls
{
    public class EmploymentViewPresenter
    {
         #region Fields
        
        #endregion Fields

        #region Constructors

        public EmploymentViewPresenter( IEmploymentView view )
        {
            EmploymentView = view;
        }

        #endregion Constructors

        #region Properties

        private IEmploymentView EmploymentView { get; set; }

        #region Public Methods

        /// <summary>
        /// Set Enable/Disable EditEmployer button, Clear button and Occupation Industry Textbox based on EmploymentStatus
        /// </summary>
        
         public void SetControlStatesOnEmploymentStatus( )
        {
             if(EmploymentView.Model_Employment.Status!=null)
             {
                 switch (EmploymentView.Model_Employment.Status.Code)
                 {
                     case EmploymentStatus.NOT_EMPLOYED_CODE:
                            EmploymentView.EnableClearButton(false);
                            EmploymentView.SetControlState(false);
                         break;
                     case EmploymentStatus.RETIRED_CODE:
                            EmploymentView.EnableClearButton(true);
                            EmploymentView.SetControlState(true);
                         break;
                     case EmploymentStatus.SELF_EMPLOYED_CODE:
                            EmploymentView.EnableClearButton(true);
                            EmploymentView.SetControlState(true);
                         break;
                     default:
                         if (EmploymentView.Model_Employment.Status.Code == string.Empty)
                         {
                             EmploymentView.EnableClearButton(false);
                             EmploymentView.SetControlState(false);
                         }
                         else
                         {
                             EmploymentView.EnableClearButton(true);
                             EmploymentView.SetControlState(true);
                         }

                         break;
                 }
             }
             else
             {
                 EmploymentView.EnableClearButton(true);
                 EmploymentView.SetControlState(false);
             }
        }

        #endregion


        #endregion Properties
    }
}
