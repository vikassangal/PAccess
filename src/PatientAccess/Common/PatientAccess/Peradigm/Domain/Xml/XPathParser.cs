using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Peradigm.Framework.Domain.Xml
{
	/// <summary>
	/// Summary description for XPathParser.
	/// </summary>
	[Serializable]
	public class XPathParser : object
	{
		#region Constants
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		public string ValueAt( string expression )
		{
		    string attribute = String.Empty;
		    XPathNodeIterator nodeIterator = this.NodeIteratorFor( expression );
    		
            nodeIterator.MoveNext();
		    attribute = nodeIterator.Current.Value;
		    return attribute;
		}

        public ICollection ValuesMatching( string expression )
        {
            ArrayList matchingValues = new ArrayList();
            XPathNodeIterator nodeIterator = this.NodeIteratorFor( expression );
            while( nodeIterator.MoveNext() )
            {
                matchingValues.Add( nodeIterator.Current.Value );
            }
            return matchingValues;
        }

		#endregion

		#region Properties
		#endregion

		#region Private Methods
        private XPathNodeIterator NodeIteratorFor( string expression )
        {
            return this.Navigator.Select( expression );
        }
		#endregion

		#region Private Properties
		private XPathNavigator Navigator
		{
			get
			{
				return i_Navigator;
			}
			set
			{
				i_Navigator = value;
			}
		}
		#endregion

		#region Construction and Finalization
		public XPathParser( XmlDocument document )
		{
			this.Navigator = document.CreateNavigator();
		}
		#endregion

		#region Data Elements
		private XPathNavigator i_Navigator;
		#endregion
	}

}
