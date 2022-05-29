namespace PatientAccess.Domain
{
    public interface IEmployer
    {
        #region Event Handlers
        #endregion

        #region Methods
        IEmployer AsEmployer( );
        #endregion

        #region Properties

        string Name
        {
            get;
            set;
        }

        string NationalId
        {
            get;
            set;
        }
        string Industry
        {
            get;
            set;
        }

        ContactPoint PartyContactPoint
        {
            get;
            set;
        }

        #endregion
    }
}