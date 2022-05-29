namespace PatientAccess.Persistence.Nhibernate
{
    public interface IEncryptor
    {
        string Encrypt( string password );
        string Decrypt( string encryptedPassword );
    }
}