using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties; 

namespace PatientAccess.UI.PatientSearch
{
    public class EMPIPatientSearchResultListViewItem : ListViewItem
    {
        #region Constants

        private const int IMAGE_INDEX_FOR_ALIAS = 1;
        private const int IMAGE_INDEX_FOR_BLANK = -1;
        private const int IMAGE_INDEX_FOR_REAL_NAME = 0;

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientSearchResultListViewItem"/> class.
        /// </summary>
        /// <param name="patientSearchResult">The patient search result.</param>
        public EMPIPatientSearchResultListViewItem(PatientSearchResult patientSearchResult)
        {

            Debug.Assert(patientSearchResult != null);
            ImageIndex = IMAGE_INDEX_FOR_BLANK;
            if (patientSearchResult != null)
            {
                BuildToolTipsFor(patientSearchResult);

                SubItems.Add(patientSearchResult.EMPIScore.ToString().PadLeft(3, ' '));
                SubItems.Add(patientSearchResult.Name.AsFormattedName());
                SubItems.Add(patientSearchResult.Gender.Description);
                SubItems.Add(patientSearchResult.DateOfBirth.ToString("MM/dd/yyyy"));
                SubItems.Add(new SocialSecurityNumber(patientSearchResult.SocialSecurityNumber).AsFormattedString());
                SubItems.Add(patientSearchResult.MedicalRecordNumber.ToString());
                SubItems.Add(patientSearchResult.HspCode.PadLeft(3, ' ').PadRight(3, ' '));
                SubItems.Add(patientSearchResult.Address.OneLineAddressLabel());
                
            }
        }

        #endregion Constructors
 
        #region Methods

        /// <summary>
        /// Builds the alias tool tip for.
        /// </summary>
        /// <param name="patientSearchResult">The patient search result.</param>
        /// <returns></returns>
        private string BuildAliasToolTipFor(PatientSearchResult patientSearchResult)
        {

            var stringBuilder =
                new StringBuilder("Alias Name: ");
            int numberOfAliases =
                patientSearchResult.AkaNames.Count;
            List<Name> listOfAliases =
                patientSearchResult.AkaNames;

            foreach (Name aliasName in listOfAliases)
            {

                stringBuilder.Append(aliasName.AsFormattedName().TrimEnd());

                // Add a delimiter if this is not the end of the list
                if (numberOfAliases-- > 1)
                {

                    stringBuilder.Append("; ");

                }//if

            }//foreach

            return stringBuilder.ToString();

        }

        /// <summary>
        /// Builds the tool tips for.
        /// </summary>
        /// <param name="patientSearchResult">The patient search result.</param>
        private void BuildToolTipsFor(PatientSearchResult patientSearchResult)
        {

            List<Name> aliases = patientSearchResult.AkaNames;

            if (aliases.Count > 0)
            {

                if (patientSearchResult.Name.TypeOfName == TypeOfName.Alias)
                {

                    ImageIndex = IMAGE_INDEX_FOR_ALIAS;
                    ToolTipText =
                        String.Format("Real Name: {0}",
                                       aliases[0].AsFormattedName().TrimEnd());

                }//if
                else
                {

                    ImageIndex = IMAGE_INDEX_FOR_REAL_NAME;
                    ToolTipText = BuildAliasToolTipFor(patientSearchResult);

                } //else

            }//if
        }

        #endregion Methods
    }

}
