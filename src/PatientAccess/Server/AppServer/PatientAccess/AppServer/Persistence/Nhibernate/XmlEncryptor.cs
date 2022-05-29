using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace PatientAccess.AppServer.Persistence.Nhibernate
{
    public static class XmlEncryptor
    {
        private const bool EncryptElementContentsOnly = true;
        private const string KeyName = "rsaKey";
        private const string EncryptionElementId = "EncryptedElement";
        private const string KeyContainerName = "PatientAccessKeys";

        private static RSACryptoServiceProvider GetRsaProvider()
        {
            var cspParams = new CspParameters
                                {
                                    KeyContainerName = KeyContainerName,
                                    
                                    //this flag is needed so that the asp.net account can access the key container
                                    //http://blogs.msdn.com/b/alejacma/archive/2007/12/03/rsacryptoserviceprovider-fails-when-used-with-asp-net.aspx
                                    Flags = CspProviderFlags.UseMachineKeyStore
                                };

            return new RSACryptoServiceProvider( cspParams );
        }

        public static void Encrypt( XmlDocument doc )
        {
            if ( doc == null )
                throw new ArgumentNullException( "doc" );


            AesManaged sessionKey = null;
            try
            {
                /////////////////////////////////////////////////////////////////
                // Create a new instance of the EncryptedXml class and use it
                // to encrypt the XmlElement with the a new random symmetric key
                // 

                // Create a 256 bit AES key.
                sessionKey = new AesManaged { KeySize = 256 };

                var eXml = new EncryptedXml();

                byte[] encryptedElement = eXml.EncryptData( doc.DocumentElement, sessionKey, EncryptElementContentsOnly );


                /////////////////////////////////////////////////////////////////
                // Construct an EncryptedData object and populate it with the 
                // desired encryption information.
                // 

                var edElement = new EncryptedData
                                    {
                                        Type = EncryptedXml.XmlEncElementUrl,
                                        Id = EncryptionElementId,
                                        EncryptionMethod = new EncryptionMethod( EncryptedXml.XmlEncAES256Url )
                                    };


                /////////////////////////////////////////////////////////////////
                // Create an EncryptionMethod element so that the receiver knows 
                // which algorithm to use for decryption.
                //

                // Encrypt the session key and add it to an EncryptedKey element.

                var rsaProvider = GetRsaProvider();
                var ek = new EncryptedKey();
                byte[] encryptedKey = EncryptedXml.EncryptKey( sessionKey.Key, rsaProvider, false );
                ek.CipherData = new CipherData( encryptedKey );
                ek.EncryptionMethod = new EncryptionMethod( EncryptedXml.XmlEncRSA15Url );


                /////////////////////////////////////////////////////////////////
                // Create a new DataReference element for the KeyInfo element.This 
                // optional element specifies which EncryptedData uses this key. 
                // An XML document can have multiple EncryptedData elements that use
                // different keys.
                //

                DataReference dRef = new DataReference();
                // Specify the EncryptedData URI.
                dRef.Uri = "#" + EncryptionElementId;
                // Add the DataReference to the EncryptedKey.
                ek.AddReference( dRef );
                // Add the encrypted key to the EncryptedData object.
                edElement.KeyInfo.AddClause( new KeyInfoEncryptedKey( ek ) );
                // Set the KeyInfo element to specify the name of the RSA key.
                // Create a new KeyInfo element.
                edElement.KeyInfo = new KeyInfo();
                // Create a new KeyInfoName element.
                KeyInfoName kin = new KeyInfoName();
                // Specify a name for the key.
                kin.Value = KeyName;
                // Add the KeyInfoName element to the EncryptedKey object.
                ek.KeyInfo.AddClause( kin );
                // Add the encrypted key to the EncryptedData object.
                edElement.KeyInfo.AddClause( new KeyInfoEncryptedKey( ek ) );


                /////////////////////////////////////////////////////////////////
                // Add the encrypted element data to the EncryptedData object.
                //

                edElement.CipherData.CipherValue = encryptedElement;
                // Replace the element from the original XmlDocument object with 
                // the EncryptedData element           
                EncryptedXml.ReplaceElement( doc.DocumentElement, edElement, EncryptElementContentsOnly );
            }
            catch ( Exception e )
            {
                // re-throw the exception.
                throw e;
            }
            finally
            {
                if ( sessionKey != null )
                {
                    sessionKey.Clear();
                }
            }
        }

        public static void Decrypt( XmlDocument Doc )
        {
            //////////////////////////////////////////////////////////////////////
            // Check the arguments
            //
            var rsaProvider = GetRsaProvider();
            if ( Doc == null )
                throw new ArgumentNullException( "Doc" );


            //////////////////////////////////////////////////////////////////////
            // Create a new EncryptedXml object to decrypt data
            //

            var exml = new EncryptedXml( Doc );
            // Add a key-name mapping.
            // This method can only decrypt documents
            // that present the specified key name.
            exml.AddKeyNameMapping( KeyName, rsaProvider );
            // Decrypt the element.
            exml.DecryptDocument();
        }
    }
}