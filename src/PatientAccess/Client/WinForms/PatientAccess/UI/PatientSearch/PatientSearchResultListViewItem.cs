using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.PatientSearch
{

    public class PatientSearchResultListViewItem : ListViewItem
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
        public PatientSearchResultListViewItem( PatientSearchResult patientSearchResult )
        {

            Debug.Assert( patientSearchResult != null );

            this.ImageIndex = IMAGE_INDEX_FOR_BLANK;
            
            this.SearchResult = patientSearchResult;
            
            this.BuildToolTipsFor( patientSearchResult );

            this.SubItems.Add( patientSearchResult.Name.AsFormattedName() );
            this.SubItems.Add( patientSearchResult.Gender.Description );
            this.SubItems.Add( patientSearchResult.DateOfBirth.ToString( "MM/dd/yyyy" ) );
            this.SubItems.Add( new SocialSecurityNumber( patientSearchResult.SocialSecurityNumber ).AsFormattedString() );
            this.SubItems.Add( patientSearchResult.MedicalRecordNumber.ToString() );
            this.SubItems.Add( patientSearchResult.Address.OneLineAddressLabel() );

        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets the search result.
        /// </summary>
        /// <value>The search result.</value>
        private PatientSearchResult SearchResult
        {

            get
            {

                return this.Tag as PatientSearchResult;

            }
            set
            {

                this.Tag = value;

            }

        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Builds the alias tool tip for.
        /// </summary>
        /// <param name="patientSearchResult">The patient search result.</param>
        /// <returns></returns>
        private string BuildAliasToolTipFor( PatientSearchResult patientSearchResult ) 
        {

            StringBuilder stringBuilder = 
                new StringBuilder( "Alias Name: " );
            int numberOfAliases = 
                patientSearchResult.AkaNames.Count;
            List<Name> listOfAliases = 
                patientSearchResult.AkaNames;

            foreach( Name aliasName in listOfAliases )
            {

                stringBuilder.Append( aliasName.AsFormattedName().TrimEnd() );
                
                // Add a delimiter if this is not the end of the list
                if( numberOfAliases-- > 1)
                {

                    stringBuilder.Append( "; " );

                }//if
                        
            }//foreach

            return stringBuilder.ToString();

        }

        /// <summary>
        /// Builds the tool tips for.
        /// </summary>
        /// <param name="patientSearchResult">The patient search result.</param>
        private void BuildToolTipsFor( PatientSearchResult patientSearchResult )
        {

            List<Name> aliases = patientSearchResult.AkaNames;

            if( aliases.Count > 0 )
            {

                if( patientSearchResult.Name.TypeOfName == TypeOfName.Alias )
                {

                    this.ImageIndex = IMAGE_INDEX_FOR_ALIAS;
                    this.ToolTipText = 
                        string.Format( "Real Name: {0}", 
                                       aliases[0].AsFormattedName().TrimEnd() );

                }//if
                else
                {

                    this.ImageIndex = IMAGE_INDEX_FOR_REAL_NAME;                    
                    this.ToolTipText = this.BuildAliasToolTipFor( patientSearchResult );

                } //else

            }//if
        }

		#endregion Methods 

    }

}
