using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Extensions.UI.Builder;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for RuleHashtable.
    /// </summary>
    [Serializable]
    public class RuleHashtable : Hashtable
    {
        public RuleHashtable()
        {
        }

        protected RuleHashtable(SerializationInfo info, StreamingContext context)
            :base(info,context){}

        public override object Clone()
        {            
            RuleHashtable newHT = this.DeepCopy(); 

            return newHT;
        }


        private RuleHashtable DeepCopy()
        {
            BinaryFormatter formatter;
            MemoryStream stream = null;
            RuleHashtable result = null;

            if( this.Count > 0 )
            {
                try
                {
                    stream = new MemoryStream();
                    formatter = new BinaryFormatter();
                    formatter.Serialize( stream, this );
                    stream.Position = 0;
                    result = formatter.Deserialize( stream ) as RuleHashtable;
                }
                catch( Exception )
                {
                    result = new RuleHashtable();

                    foreach( LeafRule lr in this.Values )
                    {
                        result.Add( lr.GetType(), lr );
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
            else
            {
                result = new RuleHashtable();     // OTD 36378
            }

            return result;
        }
    }
}
