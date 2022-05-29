using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SpecialProgram : IRecommendationCollector, ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void CollectRecommendation( MSPRecommendation recommendation )
        {
            bool question1Failed = true; // and Question 2 failed included
            bool question3Failed = true;  
            bool question4Failed = true;  
            bool question5Failed = true;

            // UC231 - Condition 1
            // Are you receiving Black Lung Benefits? = Yes AND
            // is the reason for today’s visit related to Black Lung = Yes

            if( BlackLungBenefits.Code.Equals( "Y" ) && VisitForBlackLung.Code.Equals( "Y" ) )
            {
                question1Failed = false;
                recommendation.IsMedicareRecommended = false;
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Are you receiving Black Lung (BL) benefits?", "Yes" ) );
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Is the reason for today's visit related to Black Lung?", "Yes" ) );
            }
            // UC231 - Condition 2
            // Are the services to be paid by a government program such as a research grant? = Yes

            if( GovernmentProgram.Code.Equals( "Y" ) )
            {
                question3Failed = false;
                recommendation.IsMedicareRecommended = false;
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Are the services to be paid by a government program such as a " +
                    "research grant?", "Yes" ) );
            }
            // UC231 - Condition 3
            // Has the Dept of Veterans Affairs authorized and agreed to pay for care at this facility? = Yes

            if( DVAAuthorized.Code.Equals( "Y" ) )
            {
                question4Failed = false;
                recommendation.IsMedicareRecommended = false;
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Has the Department of Verteran Affairs (DVA) authorized and agreed " +
                    "to pay for care at this facility?", "Yes" ) );
            }
            // UC231 - Condition 4
            // Was the illness/injury due to a work-related accident/condition? = Yes

            if( WorkRelated.Code.Equals( "Y" ) )
            {
                question5Failed = false;
                recommendation.IsMedicareRecommended = false;
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Was the illness/injury due to a work-related accident/condition?", "Yes" ) );
            }
            if( question1Failed && question3Failed && question4Failed && question5Failed )
            {
                if( BlackLungBenefits.Code == YesNoFlag.CODE_YES )
                {
                    recommendation.AddCondition( new ContributingCondition( "Special Program",
                        "Are you receiving Black Lung (BL) benefits?", "Yes" ) );
                }
                else if (BlackLungBenefits.Code == YesNoFlag.CODE_NO )
                {
                    recommendation.AddCondition( new ContributingCondition( "Special Program",
                        "Are you receiving Black Lung (BL) benefits?", "No" ) );
                }
                
                if( VisitForBlackLung.Code == YesNoFlag.CODE_NO )
                {
                    recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Is the reason for today's visit related to Black Lung?", "No" ) );
                }
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Are the services to be paid by a government program such as a " +
                    "research grant?", "No" ) );
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Has the Department of Verteran Affairs (DVA) authorized and agreed " +
                    "to pay for care at this facility?", "No" ) );
                recommendation.AddCondition( new ContributingCondition( "Special Program",
                    "Was the illness/injury due to a work-related accident/condition?", "No" ) );

                recommendation.FirstSetOfQuestionsFailed = true;
            }
            else
            {
                recommendation.FirstSetOfQuestionsFailed = false;
            }
        }

        public object Clone()
        {
            SpecialProgram newObject = new SpecialProgram();
            SetYesNoState( newObject.BlackLungBenefits, this.BlackLungBenefits );
            SetYesNoState( newObject.VisitForBlackLung, this.VisitForBlackLung );
            SetYesNoState( newObject.GovernmentProgram, this.GovernmentProgram );
            SetYesNoState( newObject.DVAAuthorized, this.DVAAuthorized );
            SetYesNoState( newObject.WorkRelated, this.WorkRelated );
            newObject.BLBenefitsStartDate = this.BLBenefitsStartDate;
            return newObject;
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            SpecialProgram sp = obj as SpecialProgram;
            int comparison = DateTime.Compare( this.BLBenefitsStartDate, sp.BLBenefitsStartDate );
            bool dateComparison = (comparison == 0);

            bool result = (this.BlackLungBenefits.Equals( sp.BlackLungBenefits ) &&
                           this.VisitForBlackLung.Equals( sp.VisitForBlackLung ) &&
                           this.GovernmentProgram.Equals( sp.GovernmentProgram ) &&
                           this.DVAAuthorized.Equals( sp.DVAAuthorized ) &&
                           dateComparison &&
                           this.WorkRelated.Equals( sp.WorkRelated ));
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties
        public YesNoFlag BlackLungBenefits
        {   // Are you receiving Black Lung (BL) benefits?
            get
            {
                return this.i_BlankLungBenefits;
            }
            set
            {
                this.i_BlankLungBenefits = value;
            }
        }

        public YesNoFlag VisitForBlackLung
        {   // Is the reason for today's visit due to black lung?
            get
            {
                return this.i_visitForBlackLung;
            }
            set
            {
                this.i_visitForBlackLung = value;
            }
        }

        public DateTime BLBenefitsStartDate
        {   // Black lung benefits started on date
            get
            {
                return this.i_BLBenefitsStartDate;
            }
            set
            {
                this.i_BLBenefitsStartDate = value;
            }
        }

        public YesNoFlag GovernmentProgram
        {   // Are the services to be paid by a government program?
            get
            {
                return this.i_GovernmentProgram;
            }
            set
            {
                 this.i_GovernmentProgram = value;
            }
        }

        public YesNoFlag DVAAuthorized
        {   // Has the DVA agreed to pay for care?
            get
            {
                return this.i_DVAAuthorized;
            }
            set
            {
                this.i_DVAAuthorized = value;
            }
        }

        public YesNoFlag WorkRelated
        {   // Was illness/injury due to work-related accident/condition?
            get
            {
                return this.i_WorkRelated;
            }
            set
            {
                this.i_WorkRelated = value;
            }
        }
        #endregion

        #region Private Methods
        private void SetYesNoState( YesNoFlag newObj, YesNoFlag oldObj )
        {
            if( oldObj.Code.Equals( "Y" ) )
            {
                newObj.SetYes();
            }
            else if( oldObj.Code.Equals( "N" ) )
            {
                newObj.SetNo();
            }
            else if( oldObj.Code.Equals( " " ) )
            {
                newObj.SetBlank();
            }
        }
        #endregion

        #region Construction and Finalization
        public SpecialProgram()
        {
        }
        #endregion

        #region Data Elements
        private DateTime  i_BLBenefitsStartDate;
        private YesNoFlag i_BlankLungBenefits = new YesNoFlag();
        private YesNoFlag i_visitForBlackLung = new YesNoFlag();
        private YesNoFlag i_GovernmentProgram = new YesNoFlag();
        private YesNoFlag i_DVAAuthorized = new YesNoFlag();
        private YesNoFlag i_WorkRelated = new YesNoFlag();
        #endregion

        #region Constants
        #endregion
    }
}