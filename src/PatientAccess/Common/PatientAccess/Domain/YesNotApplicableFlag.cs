using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for YesNotApplicableFlag
    [Serializable]
    public class YesNotApplicableFlag : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void SetYes()
        {
            base.Code = CODE_YES;
            base.Description = "Yes";
        }
        public void SetNotApplicable()
        {
            base.Code = CODE_NOTAPPLICABLE;
            base.Description = "N/A";
        }
        public void SetBlank()
        {
            base.Code = CODE_BLANK;
            base.Description = " ";
        }
       
       
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
      
        #endregion

        #region Private Properties

        public bool IsYes 
        { 

            get
            {

                return this.Code.Equals( CODE_YES );

            }

        }

        public bool IsNotApplicable
        {

            get
            {

                return this.Code.Equals( CODE_NOTAPPLICABLE );

            }

        }

        public bool IsBlank
        {

            get
            {

                return String.IsNullOrEmpty( this.Code );
            }

        }

        #endregion

        #region Construction and Finalization
        public YesNotApplicableFlag()
        {
            this.SetBlank();
        }
     
        public YesNotApplicableFlag(string initValue)
        {
            
            switch( initValue )
            {

                case(CODE_YES):
                case("YES"):
                {
                    this.SetYes();
                    break;
                }
                case(CODE_NOTAPPLICABLE):
                {
                    this.SetNotApplicable();
                    break;
                }
                default:
                {
                    this.SetBlank();
                    break;
                }

            }//switch

        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        public const string CODE_YES = "Y";
        public const string CODE_NOTAPPLICABLE = "N/A";
        public static readonly string CODE_BLANK = string.Empty;

        #endregion
    }
}
