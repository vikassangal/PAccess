using System;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for TransferService.
    /// </summary>
    public class TransferService
    {
        public static bool IsTransferDateValid( MaskedEditTextBox mtbTransferDate )
        {
            SetNormalBgColor( mtbTransferDate );
            mtbTransferDate.Refresh();

            if ( mtbTransferDate.UnMaskedText.Trim() == string.Empty )
            {
                return false;
            }

            if ( mtbTransferDate.Text.Length != 10 )
            {
                SetErrBgColor( mtbTransferDate );
                MessageBox.Show( UIErrorMessages.TRANSFER_DATE_INVALID_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );

                mtbTransferDate.Focus();
                return false;
            }

            int transferMonth = Convert.ToInt32( mtbTransferDate.Text.Substring( 0, 2 ) );
            int transferDay = Convert.ToInt32( mtbTransferDate.Text.Substring( 3, 2 ) );
            int transferYear = Convert.ToInt32( mtbTransferDate.Text.Substring( 6, 4 ) );

            try
            {
                DateTime transferDate = new DateTime( transferYear, transferMonth, transferDay );

                if ( DateValidator.IsValidDate( transferDate ) == false )
                {
                    SetErrBgColor( mtbTransferDate );
                    MessageBox.Show( UIErrorMessages.TRANSFER_DATE_NOT_VALID_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );

                    mtbTransferDate.Focus();

                    return false;
                }
            }
            catch ( ArgumentOutOfRangeException )
            {
                SetErrBgColor( mtbTransferDate );
                MessageBox.Show( UIErrorMessages.TRANSFER_DATE_NOT_VALID_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );

                mtbTransferDate.Focus();

                return false;
            }

            return true;
        }

        public static bool IsFutureTransferDate( MaskedEditTextBox mtbTransferDate, TextBox mtbTransferTime,
            DateTime localDateTime, string field )
        {
            SetNormalBgColor( mtbTransferDate );
            SetNormalBgColor( mtbTransferTime );

            mtbTransferDate.Refresh();
            mtbTransferTime.Refresh();

            if ( mtbTransferDate.UnMaskedText.Trim() == string.Empty )
            {
                return false;
            }

            int transferMonth = Convert.ToInt32( mtbTransferDate.Text.Substring( 0, 2 ) );
            int transferDay = Convert.ToInt32( mtbTransferDate.Text.Substring( 3, 2 ) );
            int transferYear = Convert.ToInt32( mtbTransferDate.Text.Substring( 6, 4 ) );

            int transferHour = 0;
            int transferMinute = 0;

            if ( ( ( MaskedEditTextBox )mtbTransferTime ).UnMaskedText.Length == 4 )
            {
                transferHour = Convert.ToInt32( mtbTransferTime.Text.Substring( 0, 2 ) );
                transferMinute = Convert.ToInt32( mtbTransferTime.Text.Substring( 3, 2 ) );
            }

            try
            {   // Check the date entered is not in the future
                DateTime transferDate = new DateTime( transferYear, transferMonth, transferDay, transferHour, transferMinute, 0 );

                if ( transferDate > localDateTime )
                {
                    SetErrBgColor(field == "DATE" ? mtbTransferDate : mtbTransferTime);
                    if (field == "DATE")
                    {
                        mtbTransferDate.Focus();
                    }
                    else
                    {
                        mtbTransferTime.Focus();
                    }

                    MessageBox.Show( UIErrorMessages.TRANSFER_DATE_IN_FUTURE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );

                    return true;
                }
            }
            catch ( ArgumentOutOfRangeException )
            {
                SetErrBgColor( mtbTransferDate );
                MessageBox.Show( UIErrorMessages.TRANSFER_DATE_NOT_VALID_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return true;
            }

            return false;
        }

        public static bool IsTransferDateBeforeAdmitDate( MaskedEditTextBox mtbTransferDate, TextBox mtbTransferTime,
            DateTime admitDateTime, string field, bool forSwap )
        {
            SetNormalBgColor( mtbTransferDate );
            SetNormalBgColor( mtbTransferTime );

            mtbTransferDate.Refresh();
            mtbTransferTime.Refresh();

            if ( mtbTransferDate.UnMaskedText.Trim() == string.Empty )
            {
                return false;
            }

            int transferMonth = Convert.ToInt32( mtbTransferDate.Text.Substring( 0, 2 ) );
            int transferDay = Convert.ToInt32( mtbTransferDate.Text.Substring( 3, 2 ) );
            int transferYear = Convert.ToInt32( mtbTransferDate.Text.Substring( 6, 4 ) );
            int transferHour = 0;
            int transferMinute = 0;

            if ( ( ( MaskedEditTextBox )mtbTransferTime ).UnMaskedText.Length == 4 )
            {
                transferHour = Convert.ToInt32( mtbTransferTime.Text.Substring( 0, 2 ) );
                transferMinute = Convert.ToInt32( mtbTransferTime.Text.Substring( 3, 2 ) );
            }

            try
            {
                DateTime transferDate = new DateTime( transferYear, transferMonth, transferDay, transferHour, transferMinute, 0 );

                if (
                    ( transferHour == 0 && transferMinute == 0 && transferDate.Date <= admitDateTime.Date && transferDate != admitDateTime )
                    ||
                    ( ( transferHour != 0 || transferMinute != 0 ) && transferDate < admitDateTime )
                  )
                {
                    if ( transferDate.Date == admitDateTime.Date && transferDate.TimeOfDay < admitDateTime.TimeOfDay )
                    {
                        field = "TIME";
                    }
                    else
                    {
                        field = "DATE";
                    }
                    if ( field == "DATE" )
                    {
                        SetErrBgColor( mtbTransferDate );
                        mtbTransferDate.Focus();
                    }
                    else
                    {
                        SetErrBgColor( mtbTransferTime );
                        mtbTransferTime.Focus();
                    }

                    MessageBox.Show( forSwap
                            ? UIErrorMessages.TRANSFER_DATE_BEFORE_ADMIT_DATE_SWAP_MSG
                            : UIErrorMessages.TRANSFER_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);

                    return true;
                }

                SetNormalBgColor(field == "DATE" ? mtbTransferDate : mtbTransferTime);
            }
            catch ( ArgumentOutOfRangeException )
            {
                SetErrBgColor( mtbTransferDate );
                MessageBox.Show( UIErrorMessages.TRANSFER_DATE_NOT_VALID_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return true;
            }

            return false;
        }

        public static void SetErrBgColor( TextBox txtBox )
        {
            UIColors.SetErrorBgColor( txtBox );
        }

        public static void SetNormalBgColor( TextBox txtBox )
        {
            UIColors.SetNormalBgColor( txtBox );
            txtBox.Refresh();
        }


        public static void PopulateDefaultTransferDateTime( MaskedEditTextBox mtbTransferDate,
                                                            MaskedEditTextBox mtbTransferTime,
                                                            DateTime localDateTime )
        {
            mtbTransferDate.UnMaskedText = CommonFormatting.MaskedDateFormat( localDateTime );

            if ( localDateTime.Hour != 0 ||
                localDateTime.Minute != 0 )
            {
                mtbTransferTime.UnMaskedText = CommonFormatting.MaskedTimeFormat( localDateTime );
            }
            else
            {
                mtbTransferTime.UnMaskedText = "";
            }
        }

        public static DateTime GetLocalDateTime( int gmtOffset, int dstOffset )
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( gmtOffset, dstOffset );
        }

        public static void QueueTransfer( Account anAccount )
        {
            anAccount.Facility = User.GetCurrent().Facility;
            Activity currentActivity = anAccount.Activity;
            currentActivity.AppUser = User.GetCurrent();
            CoverageDefaults coverageDefaults = new CoverageDefaults();
            coverageDefaults.SetCoverageDefaultsForActivity( anAccount );

            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            if ( broker != null )
            {
                if (currentActivity.GetType() == typeof(TransferERToOutpatientActivity) ||
                    currentActivity.GetType() == typeof(TransferOutpatientToERActivity))
                {
                    broker.SaveMultipleTransactions(anAccount, currentActivity);
                }
                else if (currentActivity.GetType() == typeof (TransferInToOutActivity) &&
                         anAccount.RemoveOccurrenceCodes50())
                {
                    anAccount.RemoveOccurrenceCode50IfNotApplicable();
                    broker.SaveMultipleTransactions(anAccount, currentActivity);
                }
                else if (currentActivity.GetType() == typeof(TransferOutToInActivity) )
                {
                    broker.SaveMultipleTransactions(anAccount, currentActivity);
                }
                else
                {
                    broker.Save(anAccount, currentActivity);
                }
            }
        }

        public static void QueueTransfer( Account anAccountOne, Account anAccountTwo )
        {
            anAccountOne.Facility = User.GetCurrent().Facility;
            anAccountTwo.Facility = User.GetCurrent().Facility;
            Activity currentActivity = anAccountOne.Activity;
            currentActivity.AppUser = User.GetCurrent();

            CoverageDefaults coverageDefaults = new CoverageDefaults();
            coverageDefaults.SetCoverageDefaultsForActivity( anAccountOne );
            coverageDefaults.SetCoverageDefaultsForActivity( anAccountTwo );

            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            if ( broker != null )
            {
                broker.Save( anAccountOne, anAccountTwo, currentActivity );
            }
        }

        public static void ReleaseBed( Location location, Facility aFacility )
        {
            LocationBrokerProxy broker = new LocationBrokerProxy();
            broker.ReleaseReservedBed( location, aFacility );
        }

        //ICloneable is not used for simplicity.
        public static Location DeepCopyLocation( Location sourceLocation )
        {
            Location targetLocation = new Location( sourceLocation.NursingStation.Code,
                                                    sourceLocation.Room.Code,
                                                    sourceLocation.Bed.Code );

            if ( sourceLocation.Bed.Accomodation != null )
            {
                targetLocation.Bed.Accomodation = new Accomodation( PersistentModel.NEW_OID,
                                                                    PersistentModel.NEW_VERSION,
                                                                    sourceLocation.Bed.Accomodation.Description,
                                                                    sourceLocation.Bed.Accomodation.Code );
            }

            return targetLocation;
        }

        /// <summary>
        /// Deeps the copy location and accomodation.
        /// </summary>
        /// <param name="sourceLocation">The source location.</param>
        /// <param name="targetAccomodation">The target accomodation.</param>
        /// <returns></returns>
        public static Location DeepCopyLocationAndAccomodation( Location sourceLocation, Accomodation targetAccomodation )
        {

            Location targetLocation = new Location( sourceLocation.NursingStation.Code,
                                                   sourceLocation.Room.Code,
                                                   sourceLocation.Bed.Code ) 
                                                   {Bed = {Accomodation = targetAccomodation}};
            return targetLocation;
        }

        #region Data Elements
        #endregion
    }
}
