namespace PatientAccess.UI.NewEmployersManagement
{
    internal interface ICommand
    {
        void ExecuteUIAction();
        void UndoUIAction();
        void CommitChanges();
    }
}