namespace Peradigm.Framework.Persistence
{
    interface IPersistentModel
    {
        #region Interface Methods
        bool HasBeenSaved();
        bool HasNotBeenSaved();
        bool HasVersion( byte[] version );
        long Oid
        {
            get;
            set;
        }
        
        byte[] Version
        {
            get;
            set;
        }            
        #endregion
    }
}
