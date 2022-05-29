namespace PatientAccess.UI.NewEmployersManagement
{
    internal sealed class NullCommand : ICommand
    {
        #region ICommand Members

        public void ExecuteUIAction() {}

        public void UndoUIAction() {}

        public void CommitChanges() {}

        #endregion
    }
}