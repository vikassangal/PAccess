using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

namespace PatientAccess.Persistence.Nhibernate
{
    public class AesEncryptor : IEncryptor
    {
        private byte[] myBytes;

        public AesEncryptor()
        {
            var encryptionSection = (NameValueCollection)ConfigurationManager.GetSection( ServerConfigurationConstants.EncryptionSectionName );

            EncryptionKey = encryptionSection[ServerConfigurationConstants.EncryptionKeyName];
        }

        public string EncryptionKey { get; set; }

        public string Encrypt( string password )
        {
            var bytes = GetBytes();

            using ( var memoryStream = new MemoryStream() )
            using ( var algorithm = new AesCryptoServiceProvider() )
            using ( var encryptor = algorithm.CreateEncryptor( bytes, bytes ) )
            using ( var cryptoStream = new CryptoStream( memoryStream, encryptor, CryptoStreamMode.Write ) )
            using ( var writer = new StreamWriter( cryptoStream ) )
            {
                writer.Write( password );
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String( memoryStream.GetBuffer(), 0, (int)memoryStream.Length );
            }
        }

        private byte[] GetBytes()
        {
            if ( myBytes == null )
                myBytes = Convert.FromBase64String( EncryptionKey );
            return myBytes;
        }

        public string Decrypt( string encryptedPassword )
        {
            var bytes = GetBytes();
            using ( var memoryStream = new MemoryStream( Convert.FromBase64String( encryptedPassword ) ) )
            using ( var algorithm = Aes.Create() )
            using ( var decryptor = algorithm.CreateDecryptor( bytes, bytes ) )
            using ( var cryptoStream = new CryptoStream( memoryStream, decryptor, CryptoStreamMode.Read ) )
            {
                using ( var reader = new StreamReader( cryptoStream ) )
                {
                    return reader.ReadToEnd();
                }
            }
        }

    }
}
