using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RBVCAFusNoteFormatter.
	/// </summary>
	//TODO: Create XML summary comment for CRFEPFusNoteFormatter
    [Serializable]
    [UsedImplicitly]
    public class CREFPFusNoteFormatter : FusFormatterStrategy
    {

		#region Constants 

        private const string FORMATTED_STRING = "Patient refused to pay";

		#endregion Constants 

		#region Constructors 

        public CREFPFusNoteFormatter()
        {
        }

		#endregion Constructors 

		#region Methods 

        public override IList  Format()
        {
            ArrayList messages = new ArrayList();
            messages.Add( FORMATTED_STRING );
            return messages;
        }

		#endregion Methods 

    }
}
