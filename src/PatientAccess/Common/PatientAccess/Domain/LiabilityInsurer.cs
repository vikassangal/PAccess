using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LiabilityInsurer : ICloneable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void CollectRecommendation( MSPRecommendation recommendation )
        {
            bool question1Failed = true;
            bool question2Failed = true;
            bool question3Failed = true;

            if( recommendation.MSPVersion ==  VERSION_1 )
            {
                // UC231 - Condition 5
                // What type of accident caused the illness/injury? = Automobile or NonAutomobile

                if( NonWorkRelated.Code.Equals( "N" ) || AccidentType.Description.Equals( String.Empty ) )
                {
                    question1Failed = true;
                    question2Failed = true;
                    if( recommendation.FirstSetOfQuestionsFailed )
                    {
                        recommendation.IsMedicareRecommended = true;
                    }
                }
                else if( AccidentType.Description.Equals( "Auto" ) ||
                    AccidentType.Description.Equals( "Non-automobile" ) )
                {
                    question1Failed = false;
                    recommendation.IsMedicareRecommended = false;
                    if( recommendation.FirstSetOfQuestionsFailed )
                    {
                        recommendation.ContributingConditions.Clear();
                    }
                    recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                        "What type of accident caused the illness/injury?", AccidentType.Description ) );
                }
                else if( AccidentType != null && AccidentType.Description.Equals( "Other" ) )
                {
                    if( recommendation.FirstSetOfQuestionsFailed )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "What type of accident caused the illness/injury?", "Other" ) );
                    }
                    // UC231 - Condition 6
                    // Was another party responsible for this accident? = Yes

                    if( AnotherPartyResponsibility != null && AnotherPartyResponsibility.Code.Equals( "Y" ) )
                    {
                        question2Failed = false;
                        recommendation.IsMedicareRecommended = false;
                        if( recommendation.FirstSetOfQuestionsFailed )
                        {
                            recommendation.ContributingConditions.Clear();
                        }
                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "Was another party responsible for this accident?", "Yes" ) );
                    }
                    else if( AnotherPartyResponsibility != null && AnotherPartyResponsibility.Code.Equals( "N" ) )
                    {
                        if( recommendation.FirstSetOfQuestionsFailed )
                        {
                            recommendation.IsMedicareRecommended = true;
                            recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                                "Was another party responsible for this accident?", "No" ) );
                        }
                    }
                    else
                    {
                        question2Failed = true;
                    }
                }
                else
                {
                    question2Failed = true;
                    question1Failed = true;
                }

                if( question1Failed && question2Failed )
                {
                    recommendation.SecondSetOfQuestionsFailed = true;
                }
                else
                {
                    recommendation.SecondSetOfQuestionsFailed = false;
                }
            }
                
            // MSP Version 2

            else if( recommendation.MSPVersion == VERSION_2 )
            {                
                recommendation.SecondSetOfQuestionsFailed   = false;

                if( this.NonWorkRelated.Code == YesNoFlag.CODE_YES )
                {
                    question1Failed = false;

                    // Condition 5

                    if( this.NoFaultInsuranceAvailable.Code == YesNoFlag.CODE_YES )
                    {
                        question2Failed = false;
                        recommendation.IsMedicareRecommended        = false;

                        if( recommendation.FirstSetOfQuestionsFailed )
                        {
                            recommendation.ContributingConditions.Clear();
                        }

                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "Was illness/injury due to a non-work related accident?", "Yes"));
                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "Is no-fault insurance available?", "Yes" ) );                                                
                    }

                    // Condition 6

                    if( this.LiabilityInsuranceAvailable.Code == YesNoFlag.CODE_YES )
                    {
                        question3Failed = false;

                        if( recommendation.FirstSetOfQuestionsFailed 
                            && question2Failed )
                        {
                            recommendation.ContributingConditions.Clear();
                        }

                        if( question2Failed )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                                "Was illness/injury due to a non-work related accident?", "Yes"));
                        }
                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "Is Liability insurance available?", "Yes" ) );

                        recommendation.IsMedicareRecommended        = false;
                    }                        
                }


                if( question2Failed && question3Failed )
                {
                    if( this.NonWorkRelated.Code == YesNoFlag.CODE_YES )
                    {
                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "Was illness/injury due to a non-work related accident?", "Yes"));

                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "Is Liability insurance available?", "No" ) );                

                        recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                            "Is no-fault insurance available?", "No" ) );
                    }
                    else
                    {
                        if( recommendation.FirstSetOfQuestionsFailed )
                        {
                            recommendation.AddCondition( new ContributingCondition( "Liability Insurer",
                                "Was illness/injury due to a non-work related accident?", "No"));
                        }
                    }
                    
                    recommendation.SecondSetOfQuestionsFailed = true;                    
                }
            }
        }

        public object Clone()
        {
            LiabilityInsurer newObject = new LiabilityInsurer();

            SetYesNoState( newObject.NonWorkRelated, this.NonWorkRelated );
            SetYesNoState( newObject.AnotherPartyResponsibility, this.AnotherPartyResponsibility );
            newObject.AccidentDate = this.AccidentDate;

            if( this.AccidentType != null )
            {
                newObject.i_AccidentType = (TypeOfAccident) this.i_AccidentType.Clone();
            }
            else
            {
                newObject.i_AccidentType = null;
            }
            return newObject;
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            LiabilityInsurer li = obj as LiabilityInsurer;
            int comparison = DateTime.Compare( this.AccidentDate, li.AccidentDate );
            bool dateComparison = (comparison == 0);

            bool accident = (this.AccidentType == null && li.AccidentType == null ? true : 
                             this.AccidentType == null && li.AccidentType != null ? false :
                             this.AccidentType != null && li.AccidentType == null ? false :
                             this.AccidentType.Equals( li.AccidentType ));

            bool result = (dateComparison &&
                           accident &&
                           (this.NonWorkRelated.Equals( li.NonWorkRelated )) && 
                           (this.AnotherPartyResponsibility.Equals( li.AnotherPartyResponsibility )));
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties

        public YesNoFlag LiabilityInsuranceAvailable
        {
            get
            {
                return this.i_LiabilityInsuranceAvailable;
            }
            set
            {
                this.i_LiabilityInsuranceAvailable = value;
            }
        }
        
        public YesNoFlag NoFaultInsuranceAvailable
        {
            get
            {
                return this.i_NoFaultInsuranceAvailable;
            }
            set
            {
                this.i_NoFaultInsuranceAvailable = value;
            }
        }

        public YesNoFlag NonWorkRelated
        {   // Was illness/injury due to a non-work related accident?
            get
            {
                return this.i_NonWorkRelated;
            }
            set
            {
                this.i_NonWorkRelated = value;
            }
        }

        public DateTime AccidentDate
        {
            get
            {
                return this.i_AccidentDate;
            }
            set
            {
                this.i_AccidentDate = value;
            }
        }

        public TypeOfAccident AccidentType
        {   // What type of accident caused the illness/injury?
            get
            {
                return this.i_AccidentType;
            }
            set
            {
                this.i_AccidentType = value;
            }
        }

        public YesNoFlag AnotherPartyResponsibility
        {   // Was another party responsible for this accident?
            get
            {
                return this.i_AnotherPartyResponsibility;
            }
            set
            {
                this.i_AnotherPartyResponsibility = value;
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
        public LiabilityInsurer()
        {
        }
        #endregion

        #region Data Elements

        private DateTime                        i_AccidentDate;
        private TypeOfAccident                  i_AccidentType                  = new TypeOfAccident();

        private YesNoFlag                       i_AnotherPartyResponsibility    = new YesNoFlag();
        private YesNoFlag                       i_NonWorkRelated                = new YesNoFlag();
        private YesNoFlag                       i_LiabilityInsuranceAvailable   = new YesNoFlag();
        private YesNoFlag                       i_NoFaultInsuranceAvailable     = new YesNoFlag();

        #endregion

        #region Constants

        private const int                                                   VERSION_1 = 1;
        private const int                                                   VERSION_2 = 2;

        #endregion
    }
}
