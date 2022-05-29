using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for YesNoFlag
    [Serializable]
    public class YesNoFlag : CodedReferenceValue, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void SetYes()
        {
            base.Code = CODE_YES;
            base.Description = DESCRIPTION_YES;
        }

        public void SetYes( string description )
        {
            base.Code = CODE_YES;
            base.Description = description;
        }

        public void SetNo()
        {
            base.Code = CODE_NO;
            base.Description = DESCRIPTION_NO;
        }
        public void SetUnable()
        {
            base.Code = CODE_UNABLE;
            base.Description = DESCRIPTION_UNABLE;
        }

        public void SetNo( string description )
        {
            base.Code = CODE_NO;
            base.Description = description;
        }

        public void SetBlank()
        {
            base.Code = CODE_BLANK;
            base.Description = CODE_BLANK;
        }

        public void SetBlank( string description )
        {
            base.Code = CODE_BLANK;
            base.Description = description;
        }

        public override object Clone()
        {
            YesNoFlag newObject = new YesNoFlag( this.Code );
            return newObject;
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            
            YesNoFlag flag = obj as YesNoFlag;
            
            if ( flag == null )
            {
                return false; 
            }
            return flag.Code == this.Code &&
                   flag.Description == this.Description;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties

        public static YesNoFlag Yes
        {
            get
            {
                return new YesNoFlag(CODE_YES);
            }
        }

        public static YesNoFlag No
        {
            get
            {
                return new YesNoFlag(CODE_NO);
            }
        }

        public static YesNoFlag Blank
        {
            get
            {
                return new YesNoFlag(CODE_BLANK);
            }
        }
        
        public bool IsYes
        {
            get
            {

                return this.Code.Equals( CODE_YES );

            }
        }

        public bool IsNo
        {
            get
            {

                return this.Code.Equals( CODE_NO );

            }
        }

        public bool IsBlank
        {
            get
            {

                return this.Code.Equals( CODE_BLANK );

            }
        }
        
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public YesNoFlag()
        {

            SetBlank();

        }

        public YesNoFlag(string str)
        {
            if( str.Equals(CODE_YES) )
            {
                this.SetYes();
            }
            else if( str.Equals(CODE_NO) )
            {
                this.SetNo();
            }
            else if (str.Equals(CODE_UNABLE))
            {
                this.SetUnable();
            }
            else
            {
                this.SetBlank();
            }
            

        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        public const string CODE_YES   = "Y";
        public const string CODE_NO    = "N";
        public const string CODE_BLANK = " ";
        public const string CODE_UNABLE = "U";

        public const string DESCRIPTION_YES = "Yes";
        public const string DESCRIPTION_NO = "No";
        public const string DESCRIPTION_UNABLE = "Unable";
        #endregion

        public static YesNoFlag GetYesNoFlagFor(string value)
        {
            YesNoFlag returnYesNoFlag = new YesNoFlag();
            switch (value.ToUpper())
            {
                case CODE_YES:
                    {
                        returnYesNoFlag.SetYes();
                        break;
                    }

                case CODE_NO:
                    {
                        returnYesNoFlag.SetNo();
                        break;
                    }
               
                case CODE_BLANK:
                    {
                        returnYesNoFlag.SetBlank(String.Empty);
                        break;
                    }
                case "":
                    {
                        returnYesNoFlag.SetBlank(String.Empty);
                        break;
                    }
            }
            return returnYesNoFlag;
        }
    }
}
