using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for EmailReasonBroker.
    /// </summary>
    //TODO: Create XML summary comment for EmailReasonBroker
    [Serializable]
    public class EmailReasonBroker : MarshalByRefObject, IEmailReasonBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Get a list of EmailReason objects including oid, code and description.
        /// </summary>
        public ICollection<EmailReason> AllEmailReasons()
        {
            try
            {
                ICollection<EmailReason> emailReasonsList = new Collection<EmailReason>(EmailReasonsList);
                return emailReasonsList;
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("EmailReasonBroker failed to initialize", e, CLog);
            }
        }

        public EmailReason EmailReasonWith(string code)
        {
            EmailReason returnEmailReason = null;
            if (code == null)
            {
                throw new ArgumentNullException("Code Cannot Be Null Or Empty");
            }
            code = code.Trim().ToUpper();
            try
            {
                ICollection<EmailReason> emailReasons = this.AllEmailReasons();
                foreach (EmailReason emailReason in emailReasons)
                {
                    if (emailReason.Code.Equals(code))
                    {
                        returnEmailReason = emailReason;
                        break;
                    }
                }
                if (returnEmailReason == null)
                {
                    EmailReason emailReasonInvalid = new EmailReason(PARAM_OID, DateTime.Now, code, code);
                    emailReasonInvalid.IsValid = false;
                    returnEmailReason = emailReasonInvalid;

                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Email Reason  failed to initialize", e, CLog);
            }
            return returnEmailReason;
        }

        #endregion


        #region Data Elements
        private static readonly ILog CLog =
            LogManager.GetLogger(typeof(EmailReasonBroker));
        #endregion

        #region Constants
        private const int PARAM_OID = 0;

        private static readonly EmailReason[] EmailReasonsList =
        {
            new EmailReason(PARAM_OID, DateTime.Now, EmailReason.BLANK, EmailReason.BLANK),
            new EmailReason(PARAM_OID, DateTime.Now, EmailReason.PROVIDED_DESCRIPTION, EmailReason.PROVIDED),
            new EmailReason(PARAM_OID, DateTime.Now, EmailReason.REMOVE_DESCRIPTION, EmailReason.REMOVE),
            new EmailReason(PARAM_OID, DateTime.Now, EmailReason.DECLINED_DESCRIPTION, EmailReason.DECLINED),
            
        };

        #endregion
    }

}
