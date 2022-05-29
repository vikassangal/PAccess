using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{

    /// <summary>
    /// Summary description for TitleOnlyFusNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class TitleOnlyFusNoteFormatter : FusFormatterStrategy
    {

		#region Methods 

        public override IList Format()
        {

            ArrayList messages = new ArrayList();
            return messages;

        }

		#endregion Methods 

    }

}