using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Extensions.UI.Builder;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for RuleArrayList.
    /// </summary>
    [Serializable]
    public class RuleArrayList : ArrayList
    {
        public RuleArrayList()
        {
        }

        public RuleArrayList( ICollection collection ) :
            base( collection )
        {       
        }

        public override object Clone()
        {
           RuleArrayList newAL = this.DeepCopy();
           return newAL;
        }

        private RuleArrayList DeepCopy()
        {
            BinaryFormatter formatter;
            MemoryStream stream = null;
            RuleArrayList result = null;

            if( this.Count > 0)
            {
                try
                {
                    stream = new MemoryStream();
                    formatter = new BinaryFormatter();
                    formatter.Serialize( stream, this );
                    stream.Position = 0;
                    result = formatter.Deserialize( stream ) as RuleArrayList;
                }
                catch(Exception)
                {
                    result = new RuleArrayList();

                    foreach( LeafRule lr in this)
                    {
                        result.Add( lr );
                    }
                }
                finally
                {
                    if( null != stream )
                    {
                        stream.Flush();
                        stream.Close();
                    }
                }
            }
            return result;
        }
    }
}
