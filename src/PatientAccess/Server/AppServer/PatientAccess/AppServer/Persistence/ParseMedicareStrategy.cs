using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{

    class ParseMedicareStrategy : ParseStrategy
    {

        #region Non-Public Properties
        
        
        private DateTime DateOfLastBillingActivity
        {
            get
            {
                DateTime result = DateTime.MinValue;

                result = GetDateOfLastBillingActivity();

                return result;
            }
        }

        private MedicareConstraints MedicareConstraints
        {

            get
            {

                return i_MedicareConstraints;

            }

        }

        private YesNoFlag PartACoverage
        {
            get
            {
                if( GetCoverageDate( PART_A ) != DateTime.MinValue )
                {
                    return new YesNoFlag( "Y" );
                }
                
                return new YesNoFlag( "N" );
            }
        }

        private DateTime PartACoverageEffectiveDate
        {
            get
            {
                return GetCoverageDate( PART_A );
            }
        }

        private YesNoFlag PartBCoverage
        {
            get
            {
                if( GetCoverageDate( PART_B ) != DateTime.MinValue )
                {
                    return new YesNoFlag( "Y" );
                }
                
                return new YesNoFlag( "N" );
            }
        }

        private DateTime PartBCoverageEffectiveDate
        {
            get
            {
                return GetCoverageDate( PART_B );
            }
        }

        private YesNoFlag PatientHasMedicareHMOCoverage
        {
            get
            {
                if( GetPatientHasHMOCoverage() )
                {
                    return new YesNoFlag( "Y" );
                }
                
                return new YesNoFlag( "N" );
            }
        }

        private YesNoFlag PatientIsPartOfHospiceProgram
        {
            get
            {
                YesNoFlag result = new YesNoFlag();
                XmlNode node = XmlResponseDocument.SelectSingleNode( PATIENT_ISPARTOF_HOSPICE_PROGRAM );
                if( NodeHasInnerText( node ) )
                {
                    result = new YesNoFlag( node.InnerText );
                }
                return result;
            }
        }

        private float RemainingPartADeductible
        {
            get
            {
                float result = 0;
                XmlNode node = XmlResponseDocument.SelectSingleNode( REMAINING_PART_A_DEDUCTIBLE );
                if( NodeHasInnerText( node ) )
                {
                    result = float.Parse( node.InnerText );
                }
                return result;
            }
        }

        private float RemainingPartBDeductible
        {
            get
            {
                float result = 0;
                XmlNode node = XmlResponseDocument.SelectSingleNode( REMAINING_PART_B_DEDUCTIBLE );
                if( NodeHasInnerText( node ) )
                {
                    result = float.Parse( node.InnerText );
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the payor id regular expression.
        /// </summary>
        /// <value>The payor id regular expression.</value>
        /// <remarks>
        /// Always null for Medicare
        /// </remarks>
        protected override Regex PayorIdRegularExpression
        {

            get
            {

                return null;

            }

        }


        /// <summary>
        /// Gets the remaining benefit period.
        /// </summary>
        /// <value>The remaining benefit period.</value>
        private int RemainingBenefitPeriod
        {
            get
            {
                int result = 0;

                result = GetHospitalDaysRemaining();

                return result;
            }
        }


        /// <summary>
        /// Gets the remaining co insurance.
        /// </summary>
        /// <value>The remaining co insurance.</value>
        private int RemainingCoInsurance
        {
            get
            {
                int result = 0;

                result = GetCoInsuranceDaysRemaining();

                return result;
            }
        }

        /// <summary>
        /// Gets the remaining life time reserve.
        /// </summary>
        /// <value>The remaining life time reserve.</value>
        private int RemainingLifeTimeReserve
        {
            get
            {
                int result = 0;

                result = GetReserveLifetimeDaysRemaining();

                return result;
            }
        }

        /// <summary>
        /// Gets the remaining SNC co insurance.
        /// </summary>
        /// <value>The remaining SNC co insurance.</value>
        private int RemainingSNFCoInsurance
        {
            get
            {
                int result = 0;

                result = GetCoInsSNFDaysRemaining();

                return result;
            }
        }


        /// <summary>
        /// Gets the remaining SNF.
        /// </summary>
        /// <value>The remaining SNF.</value>
        private int RemainingSNF
        {
            get
            {
                int result = 0;

                result = GetSNFDaysRemaining();

                return result;
            }
        }

        #endregion

        #region Public Methods

        public override void Execute()
        {

            if( TheBenefitsValidationResponse == null )
            {
                throw new ArgumentNullException( "BenefitsValidationResponse",
                    "BenefitsValidationResponse can not be null" );
            }

            base.Execute();

            PopulateAndAddConstraintsFromXml();

        }

        #endregion

        #region Non-Public Methods

        internal void AddBenefits( BenefitsInformation benefitsInfo )
        {
            Benefits.Add( benefitsInfo );
        }

        internal int GetCoInsSNFDaysRemaining()
        {

            ArrayList matches = GetCoverages( ELIG_OR_BEN_CO_INSURANCE, PART_A, SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                TIME_PERIOD_QUALIFIER_REMAINING, QUANTITY_QUALIFIER_DAYS );

            if( matches.Count == 1 )
            {
                return int.Parse( ( (BenefitsInformation)matches[0] ).Quantity );
            }
            if( matches.Count == 0 )
            {
                matches = GetCoverages( ELIG_OR_BEN_CO_INSURANCE, PART_A, SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                                        TIME_PERIOD_QUALIFIER_REMAINING, QUANTITY_QUALIFIER_DAYS );

                if( matches.Count == 1 )
                {
                    return int.Parse( ( (BenefitsInformation)matches[0] ).Quantity );
                }
                if( matches.Count == 0 )
                {
                    // Defect 5160 fix 
                    return GetCoPaymentSNFDaysRemaining();
                }
            }

            string prevQuantity = "None";
            bool blnSame = true;

            foreach( BenefitsInformation bi in matches )
            {
                if( prevQuantity != "None" )
                {
                    if( prevQuantity != bi.Quantity )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevQuantity = bi.Quantity;
                }
            }

            if( blnSame && prevQuantity != "None" )
            {
                return int.Parse( prevQuantity );
            }
            
            return -1;
        }

        
        /// <summary>
        /// Defect 5160 fix - If SNF days was being returned as CoPayment SNF days and not 
        /// Co-Insurance SNF days in the 271 response, and if the Parsing logic is only 
        /// looking for the Co-Insurance code, then it will miss the SNF days value that
        /// is being returned with a Co-Payment code. Tiffany wants the SNF days parsed out
        /// whether they are Co-Insurance or Co-Payment SNF days being returned in the response.
        /// </summary>
        /// <returns></returns>
        private int GetCoPaymentSNFDaysRemaining()
        {

        
            ArrayList matches = GetCoverages( ELIG_OR_BEN_CO_PAYMENT, PART_A, SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                TIME_PERIOD_QUALIFIER_REMAINING, QUANTITY_QUALIFIER_DAYS );

            if ( matches.Count == 1 )
            {
                return int.Parse( ( ( BenefitsInformation )matches[0] ).Quantity );
            }
            if ( matches.Count == 0 )
            {
       
                matches = GetCoverages( ELIG_OR_BEN_CO_PAYMENT, PART_A, SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                                        TIME_PERIOD_QUALIFIER_REMAINING, QUANTITY_QUALIFIER_DAYS );

                if ( matches.Count == 1 )
                {
                    return int.Parse( ( ( BenefitsInformation )matches[0] ).Quantity );
                }
                if ( matches.Count == 0 )
                {
                    return -1;
                }
            }

            string prevQuantity = "None";
            bool blnSame = true;

            foreach ( BenefitsInformation bi in matches )
            {
                if ( prevQuantity != "None" )
                {
                    if ( prevQuantity != bi.Quantity )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevQuantity = bi.Quantity;
                }
            }

            if ( blnSame && prevQuantity != "None" )
            {
                return int.Parse( prevQuantity );
            }
            
            return -1;
        }

        private int GetCoInsuranceDaysRemaining()
        {
            ArrayList matches = GetCoverages( ELIG_OR_BEN_CO_INSURANCE, PART_A, SERVICE_TYPE_CODE_HOSPITAL_IP,
                TIME_PERIOD_QUALIFIER_REMAINING, QUANTITY_QUALIFIER_DAYS );

            if( matches.Count == 1 )
            {
                return int.Parse( ( (BenefitsInformation)matches[0] ).Quantity );
            }
            if( matches.Count == 0 )
            {
                matches = GetCoverages( ELIG_OR_BEN_CO_INSURANCE, PART_A, SERVICE_TYPE_CODE_HOSPITAL,
                                        TIME_PERIOD_QUALIFIER_REMAINING, QUANTITY_QUALIFIER_DAYS );

                if( matches.Count == 1 )
                {
                    return int.Parse( ( (BenefitsInformation)matches[0] ).Quantity );
                }
                if( matches.Count == 0 )
                {
                    return -1;
                }
            }

            string prevQuantity = "None";
            bool blnSame = true;

            foreach( BenefitsInformation bi in matches )
            {
                if( prevQuantity != "None" )
                {
                    if( prevQuantity != bi.Quantity )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevQuantity = bi.Quantity;
                }
            }

            if( blnSame && prevQuantity != "None" )
            {
                return int.Parse( prevQuantity );
            }
            
            return -1;
        }

        private DateTime GetCoverageDate( string partAorB )
        {
            // Parm partAorB will either be MA for Part A or MB for Part B

            ArrayList matches = GetCoverages( ELIG_OR_BEN_ACTIVE_COVERAGE, partAorB, DATE_TIME_QUALIFIER_START_DATE );
            DateTime aDate = DateTime.MinValue;

            if( matches.Count == 1 )
            {
                try
                {

                    string strDate = ( (BenefitsInformation)matches[0] ).DateTimePeriod;
                    string formattedDate = strDate.Substring( 4, 2 ) + "/" + strDate.Substring( 6, 2 ) + "/" + strDate.Substring( 0, 4 );
                    aDate = DateTime.Parse( formattedDate );
                }
                catch
                {
                }

                return aDate;
            }
            if( matches.Count == 0 )
            {
                matches.Clear();

                matches = GetCoverages( ELIG_OR_BEN_ACTIVE_COVERAGE, partAorB, DATE_TIME_QUALIFIER_DATE_RANGE );

                aDate = DateTime.MinValue;

                if( matches.Count == 1 )
                {
                    try
                    {

                        string strDate = ( (BenefitsInformation)matches[0] ).DateTimePeriod;
                        string formattedDate = strDate.Substring( 4, 2 ) + "/" + strDate.Substring( 6, 2 ) + "/" + strDate.Substring( 0, 4 );
                        aDate = DateTime.Parse( formattedDate );
                    }
                    catch
                    {
                    }

                    return aDate;
                }
                if( matches.Count == 0 )
                {
                    return aDate;
                }
            }

            // if we make it here, we have > 1 match... compare the values to see if they are all the same

            string prevDate = "None";
            bool blnSame = true;

            foreach( BenefitsInformation bi in matches )
            {
                if( prevDate != "None" )
                {
                    if( prevDate != bi.DateTimePeriod )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevDate = bi.DateTimePeriod;
                }
            }

            if( blnSame )
            {
                try
                {
                    string formattedDate = prevDate.Substring( 4, 2 ) + "/" + prevDate.Substring( 6, 2 ) + "/" + prevDate.Substring( 4 );
                    aDate = DateTime.Parse( formattedDate );
                }
                catch
                {
                }
                return aDate;
            }
            
            return aDate;
        }

        private DateTime GetDateOfLastBillingActivity()
        {

            DateTime aDate = DateTime.MinValue;

            ArrayList matches = GetCoverages( ELIG_OR_BEN_BEN_DESCRIPTION, PART_A, DATE_TIME_QUALIFIER_194 );

            if( matches.Count == 1 )
            {
                try
                {

                    string strDate = ( (BenefitsInformation)matches[0] ).DateTimePeriod;
                    string formattedDate = strDate.Substring( 4, 2 ) + "/" + strDate.Substring( 6, 2 ) + "/" + strDate.Substring( 0, 4 );
                    aDate = DateTime.Parse( formattedDate );
                }
                catch
                {
                }

                return aDate;
            }
            if( matches.Count == 0 )
            {
                return aDate;
            }

            // if we make it here, we have > 1 match... compare the values to see if they are all the same

            string prevDate = "None";
            bool blnSame = true;

            foreach( BenefitsInformation bi in matches )
            {
                if( prevDate != "None" )
                {
                    if( prevDate != bi.DateTimePeriod )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevDate = bi.DateTimePeriod;
                }
            }

            if( blnSame )
            {
                try
                {
                    string formattedDate = prevDate.Substring( 4, 2 ) + "/" + prevDate.Substring( 6, 2 ) + "/" + prevDate.Substring( 4 );
                    aDate = DateTime.Parse( formattedDate );
                }
                catch
                {
                }
                return aDate;
            }
            
            return aDate;
        }

        private int GetHospitalDaysRemaining()
        {
            return ParseCoveragesFor(c_HospitalDaysRemainingParseRules);
        }

        private int GetSNFDaysRemaining()
        {
            return ParseCoveragesFor(c_SnfDaysRemainingParseRules);
        }


        internal int ParseCoveragesFor( string[,] snfOrHospitalDaysRemaining )
        {
            int returnValue = -1;
            ArrayList matches = null;
            for (int ruleRowIndex = 0; ruleRowIndex < snfOrHospitalDaysRemaining.GetLength(0); ruleRowIndex++)
            {
                matches = GetCoverages(
                    snfOrHospitalDaysRemaining[ruleRowIndex, 0],
                    snfOrHospitalDaysRemaining[ruleRowIndex, 1],
                    snfOrHospitalDaysRemaining[ruleRowIndex, 2],
                    snfOrHospitalDaysRemaining[ruleRowIndex, 3],
                    snfOrHospitalDaysRemaining[ruleRowIndex, 4]);

                if ( 0 != matches.Count ) break;
            }

            if ( matches == null ) return returnValue;

            string prevQuantity = "None";
            bool blnSame = true;
            switch ( matches.Count )
            {
                case 0:
                    c_Logger.Warn( "Unable to parse coverage field." ) ;
                    break;
                case 1:
                    if ( !Int32.TryParse( ( ( BenefitsInformation )matches[0]).Quantity, out returnValue ) )
                    {                       
                        c_Logger.Warn( "Failed to Parse Benefits Information." ) ; 
                    }
                    break;
                default:
                    foreach (BenefitsInformation bi in matches)
                    {
                        if (prevQuantity != "None")
                        {
                            if (prevQuantity != bi.Quantity)
                            {
                                blnSame = false;
                                break;
                            }
                        }
                        else
                        {
                            prevQuantity = bi.Quantity;
                        }
                    }

                    if (blnSame && prevQuantity != "None")
                    {
                        returnValue = int.Parse(prevQuantity);
                    }
                    break;
            }

            return returnValue;
        }

        private bool GetPatientHasHMOCoverage()
        {
            string hmoCoveragePresent = string.Empty;


            // pull HM and HN services for the Active (1) and Other (R) eligibility groups, qualified with a term date

            ArrayList matches = GetCoverages( ELIG_OR_BEN_ACTIVE_COVERAGE, INS_TYPE_HMO_MC_RISK, DATE_TIME_QUALIFIER_END_DATE );

            if( matches.Count > 0 )
            {
                if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier == DATE_TIME_QUALIFIER_END_DATE )
                {
                    if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod ) ) >= DateTime.Today )
                    {
                        hmoCoveragePresent = "true";
                    }
                    else
                    {
                        hmoCoveragePresent = "false";
                    }

                }
                else if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier2 == DATE_TIME_QUALIFIER_END_DATE )
                {
                    if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod2 ) ) >= DateTime.Today )
                    {
                        hmoCoveragePresent = "true";
                    }
                    else
                    {
                        hmoCoveragePresent = "false";
                    }

                }
            }
            else
            {
                // pull HM's
                matches = GetCoverages( ELIG_OR_BEN_ACTIVE_COVERAGE, INS_TYPE_HMO, DATE_TIME_QUALIFIER_END_DATE );

                if( matches.Count > 0 )
                {
                    if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier == DATE_TIME_QUALIFIER_END_DATE )
                    {
                        if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod ) ) >= DateTime.Today )
                        {
                            hmoCoveragePresent = "true";
                        }
                        else
                        {
                            hmoCoveragePresent = "false";
                        }

                    }
                    else if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier2 == DATE_TIME_QUALIFIER_END_DATE )
                    {
                        if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod2 ) ) >= DateTime.Today )
                        {
                            hmoCoveragePresent = "true";
                        }
                        else
                        {
                            hmoCoveragePresent = "false";
                        }

                    }
                }
                else
                {
                    matches = GetCoverages( ELIG_OR_BEN_OTHER_PAYOR, INS_TYPE_HMO_MC_RISK, DATE_TIME_QUALIFIER_END_DATE );

                    if( matches.Count > 0 )
                    {
                        if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier == DATE_TIME_QUALIFIER_END_DATE )
                        {
                            if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod ) ) >= DateTime.Today )
                            {
                                hmoCoveragePresent = "true";
                            }
                            else
                            {
                                hmoCoveragePresent = "false";
                            }

                        }
                        else if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier2 == DATE_TIME_QUALIFIER_END_DATE )
                        {
                            if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod2 ) ) >= DateTime.Today )
                            {
                                hmoCoveragePresent = "true";
                            }
                            else
                            {
                                hmoCoveragePresent = "false";
                            }

                        }
                    }
                    else
                    {
                        // pull HM's
                        matches = GetCoverages( ELIG_OR_BEN_OTHER_PAYOR, INS_TYPE_HMO, DATE_TIME_QUALIFIER_END_DATE );

                        if( matches.Count > 0 )
                        {
                            if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier == DATE_TIME_QUALIFIER_END_DATE )
                            {
                                if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod ) ) >= DateTime.Today )
                                {
                                    hmoCoveragePresent = "true";
                                }
                                else
                                {
                                    hmoCoveragePresent = "false";
                                }

                            }
                            else if( ( (BenefitsInformation)matches[0] ).DateTimeQualifier2 == DATE_TIME_QUALIFIER_END_DATE )
                            {
                                if( DateTime.Parse( FormatDate( ( (BenefitsInformation)matches[0] ).DateTimePeriod2 ) ) >= DateTime.Today )
                                {
                                    hmoCoveragePresent = "true";
                                }
                                else
                                {
                                    hmoCoveragePresent = "false";
                                }
                            }
                        }
                    }
                }
            }

            // no matches found with term dates... check for unqualified matches for HN, HM for both Active(1) and Other(R)

            if( hmoCoveragePresent == string.Empty )
            {
                matches = GetCoverages( ELIG_OR_BEN_ACTIVE_COVERAGE, INS_TYPE_HMO_MC_RISK );

                if( matches.Count > 0 )
                {
                    hmoCoveragePresent = "true";
                }
                else
                {
                    // pull HM's
                    matches = GetCoverages( ELIG_OR_BEN_ACTIVE_COVERAGE, INS_TYPE_HMO );

                    if( matches.Count > 0 )
                    {
                        hmoCoveragePresent = "true";
                    }
                    else
                    {
                        matches = GetCoverages( ELIG_OR_BEN_OTHER_PAYOR, INS_TYPE_HMO_MC_RISK );

                        if( matches.Count > 0 )
                        {
                            hmoCoveragePresent = "true";
                        }
                        else
                        {
                            // pull unqualfied HM's

                            matches = GetCoverages( ELIG_OR_BEN_OTHER_PAYOR, INS_TYPE_HMO );

                            if( matches.Count > 0 )
                            {
                                hmoCoveragePresent = "true";
                            }
                            else
                            {
                                hmoCoveragePresent = "false";
                            }
                        }
                    }
                }
            }

            if( hmoCoveragePresent == "true" )
            {
                return true;
            }
            
            return false;
        }

        private int GetReserveLifetimeDaysRemaining()
        {
            ArrayList matches = GetCoverages( ELIG_OR_BEN_RESERVE, PART_A, SERVICE_TYPE_CODE_HOSPITAL_IP,
                TIME_PERIOD_QUALIFIER_LIFETIME_REMAINING, QUANTITY_QUALIFIER_DAYS );

            if( matches.Count == 1 )
            {
                return int.Parse( ( (BenefitsInformation)matches[0] ).Quantity );
            }
            if( matches.Count == 0 )
            {
                matches = GetCoverages( ELIG_OR_BEN_RESERVE, PART_A, SERVICE_TYPE_CODE_HOSPITAL,
                                        TIME_PERIOD_QUALIFIER_LIFETIME_REMAINING, QUANTITY_QUALIFIER_LIFETIME_RESERVE_ACTUAL );

                if( matches.Count == 1 )
                {
                    return int.Parse( ( (BenefitsInformation)matches[0] ).Quantity );
                }
                if( matches.Count == 0 )
                {
                    return -1;
                }
            }

            string prevQuantity = "None";
            bool blnSame = true;

            foreach( BenefitsInformation bi in matches )
            {
                if( prevQuantity != "None" )
                {
                    if( prevQuantity != bi.Quantity )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevQuantity = bi.Quantity;
                }
            }

            if( blnSame && prevQuantity != "None" )
            {
                return int.Parse( prevQuantity );
            }
            
            return -1;
        }

        private void PopulateAndAddConstraintsFromXml()
        {

            if( !MedicareConstraints.PartACoverage.Equals( PartACoverage ) )
            {
                MedicareConstraints.PartACoverage = PartACoverage;
                MedicareConstraints.ForceChangedStatusFor( "PartACoverage" );
            }
            
            if( !MedicareConstraints.PartACoverageEffectiveDate.Equals( PartACoverageEffectiveDate ) )
            {
                MedicareConstraints.PartACoverageEffectiveDate = PartACoverageEffectiveDate;
                MedicareConstraints.ForceChangedStatusFor( "PartACoverageEffectiveDate" );
            }

            if( !MedicareConstraints.PartBCoverage.Equals( PartBCoverage ) )
            {
                MedicareConstraints.PartBCoverage = PartBCoverage;
                MedicareConstraints.ForceChangedStatusFor( "PartBCoverage" );
            }

            if( !MedicareConstraints.PartBCoverageEffectiveDate.Equals( PartBCoverageEffectiveDate ) )
            {
                MedicareConstraints.PartBCoverageEffectiveDate = PartBCoverageEffectiveDate;
                MedicareConstraints.ForceChangedStatusFor( "PartBCoverageEffectiveDate" );
            }

            if( !MedicareConstraints.PatientHasMedicareHMOCoverage.Equals( PatientHasMedicareHMOCoverage ) )
            {
                MedicareConstraints.PatientHasMedicareHMOCoverage = PatientHasMedicareHMOCoverage;
                MedicareConstraints.ForceChangedStatusFor( "PatientHasMedicareHMOCoverage" );
            }

            if( !MedicareConstraints.PatientIsPartOfHospiceProgram.Equals( PatientIsPartOfHospiceProgram ) )
            {
                MedicareConstraints.PatientIsPartOfHospiceProgram = PatientIsPartOfHospiceProgram;
                MedicareConstraints.ForceChangedStatusFor( "PatientIsPartOfHospiceProgram" );
            }

            if( !MedicareConstraints.RemainingBenefitPeriod.Equals( RemainingBenefitPeriod ) )
            {
                MedicareConstraints.RemainingBenefitPeriod = RemainingBenefitPeriod;
                MedicareConstraints.ForceChangedStatusFor( "RemainingBenefitPeriod" );
            }

            if( !MedicareConstraints.RemainingCoInsurance.Equals( RemainingCoInsurance ) )
            {
                MedicareConstraints.RemainingCoInsurance = RemainingCoInsurance;
                MedicareConstraints.ForceChangedStatusFor( "RemainingCoInsurance" );            
            }

            if( !MedicareConstraints.RemainingLifeTimeReserve.Equals( RemainingLifeTimeReserve ) )
            {
                MedicareConstraints.RemainingLifeTimeReserve = RemainingLifeTimeReserve;
                MedicareConstraints.ForceChangedStatusFor( "RemainingLifeTimeReserve" );
            }

            if( !MedicareConstraints.RemainingSNFCoInsurance.Equals( RemainingSNFCoInsurance ) )
            {
                MedicareConstraints.RemainingSNFCoInsurance = RemainingSNFCoInsurance;
                MedicareConstraints.ForceChangedStatusFor( "RemainingSNFCoInsurance" );
            }

            if( !MedicareConstraints.RemainingSNF.Equals( RemainingSNF ) )
            {
                MedicareConstraints.RemainingSNF = RemainingSNF;
                MedicareConstraints.ForceChangedStatusFor( "RemainingSNF" );
            }

            if( !MedicareConstraints.RemainingPartADeductible.Equals( RemainingPartADeductible ) )
            {
                MedicareConstraints.RemainingPartADeductible = RemainingPartADeductible;
                MedicareConstraints.ForceChangedStatusFor( "RemainingPartADeductible" );
            }

            if( !MedicareConstraints.RemainingPartBDeductible.Equals( RemainingPartBDeductible ) )
            {
                MedicareConstraints.RemainingPartBDeductible = RemainingPartBDeductible;
                MedicareConstraints.ForceChangedStatusFor( "RemainingPartBDeductible" );
            }

            if( !MedicareConstraints.DateOfLastBillingActivity.Equals( DateOfLastBillingActivity ) )
            {
                MedicareConstraints.DateOfLastBillingActivity = DateOfLastBillingActivity;
                MedicareConstraints.ForceChangedStatusFor( "DateOfLastBillingActivity" );
            }

            TheBenefitsValidationResponse.CoverageConstraintsCollection.Add( MedicareConstraints );
        }

        #endregion

        #region Constants

        const string DATE_TIME_QUALIFIER_194 = "194";
        const string DATE_TIME_QUALIFIER_DATE_RANGE = "307,291";
        const string DATE_TIME_QUALIFIER_START_DATE = "356";
        const string DATE_TIME_QUALIFIER_END_DATE = "357";
        const string ELIG_OR_BEN_ACTIVE_COVERAGE_LIMITATIONS = "F";
        const string ELIG_OR_BEN_ACTIVE_COVERAGE = "1";
        const string ELIG_OR_BEN_BEN_DESCRIPTION = "D";
        const string ELIG_OR_BEN_CO_INSURANCE = "A";
        const string ELIG_OR_BEN_CO_PAYMENT = "B";
        const string ELIG_OR_BEN_RESERVE = "K";
        const string ELIG_OR_BEN_OTHER_PAYOR = "R";
        const string INS_TYPE_HMO_MC_RISK = "HN";
        const string INS_TYPE_HMO = "HM";
        const string PART_A = "MA";
        const string PART_B = "MB";
        const string PATIENT_ISPARTOF_HOSPICE_PROGRAM = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[ServiceTypeCode='45' and InsuranceTypeCode='MA']";
        const string REMAINING_PART_A_DEDUCTIBLE = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='C' and TimePeriodQualifier='29' and InsuranceTypeCode='MA']/MonetaryAmount";
        const string REMAINING_PART_B_DEDUCTIBLE = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='C' and TimePeriodQualifier='29' and InsuranceTypeCode='MB']/MonetaryAmount";
        const string QUANTITY_QUALIFIER_DAYS = "DY";
        const string QUANTITY_QUALIFIER_LIFETIME_RESERVE_ACTUAL = "LA";
        const string SERVICE_TYPE_CODE_HOSPITAL_IP = "48";
        const string SERVICE_TYPE_CODE_HOSPITAL = "47";
        const string SERVICE_TYPE_CODE_SKILLED_NURSING_CARE = "AG";
        const string TIME_PERIOD_QUALIFIER_LIFETIME_REMAINING = "33";
        const string TIME_PERIOD_QUALIFIER_REMAINING = "29";

        #endregion

        #region Fields

        MedicareConstraints i_MedicareConstraints = new MedicareConstraints();
        private static readonly ILog c_Logger = LogManager.GetLogger(typeof(ParseMedicareStrategy));

        static string[,] c_SnfDaysRemainingParseRules =
            new string[2, 5] {                                                                          
                                  {   ELIG_OR_BEN_ACTIVE_COVERAGE_LIMITATIONS, 
                                      PART_A, 
                                      SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                                      TIME_PERIOD_QUALIFIER_REMAINING, 
                                      QUANTITY_QUALIFIER_DAYS 
                                  },

                                  {   ELIG_OR_BEN_ACTIVE_COVERAGE, 
                                      PART_A, 
                                      SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                                      TIME_PERIOD_QUALIFIER_REMAINING, 
                                      QUANTITY_QUALIFIER_DAYS 
                                  }
                                 };

        static string[,] c_HospitalDaysRemainingParseRules =
            new string[2, 5] {                                                                          
                                  {   
                                      ELIG_OR_BEN_ACTIVE_COVERAGE_LIMITATIONS, 
                                      PART_A, 
                                      SERVICE_TYPE_CODE_HOSPITAL_IP,
                                      TIME_PERIOD_QUALIFIER_REMAINING, 
                                      QUANTITY_QUALIFIER_DAYS
                                  },

                                  {   
                                      ELIG_OR_BEN_ACTIVE_COVERAGE_LIMITATIONS, 
                                      PART_A, 
                                      SERVICE_TYPE_CODE_HOSPITAL,
                                      TIME_PERIOD_QUALIFIER_REMAINING, 
                                      QUANTITY_QUALIFIER_DAYS                                  
                                  }
                                 };

        #endregion

    }

}