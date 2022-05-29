using System;
using System.Data;
using System.Xml;
using NHibernate.Type;

namespace PatientAccess.AppServer.Persistence.Nhibernate
{
    /// <summary>
    /// Extends the NHibernate XmlDocType class to encrypt and decrypt the xml document before saving and retreiving it
    /// </summary>
    [Serializable]
    public class EncryptedXmlType : XmlDocType
    {
        public override object Get(IDataReader rs, int index)
        {
            var document = (XmlDocument) base.Get(rs, index);
            XmlEncryptor.Decrypt( document );
            return document;
        }

        public override object Get(IDataReader rs, string name)
        {
            var document = (XmlDocument)base.Get( rs, name );
            XmlEncryptor.Decrypt( document );
            return document;
        }

        public override void Set(IDbCommand cmd, object value, int index)
        {
            var xmlDocument = ( (XmlDocument)value );
            XmlEncryptor.Encrypt( xmlDocument );
            ( (IDataParameter)cmd.Parameters[index] ).Value = ( (XmlDocument)value ).OuterXml;
        } 
    }
}