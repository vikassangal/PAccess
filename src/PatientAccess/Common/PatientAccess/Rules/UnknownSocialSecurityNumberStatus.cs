using System;
using System.Linq;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    //TODO: Create XML summary comment for UnknownSocialSecurityNumberStatus
    [Serializable]
    [UsedImplicitly]
    public class UnknownSocialSecurityNumberStatus : LeafRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override void UnregisterHandlers()
        {            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
        
            var anAccount = context as Account;
            
            if(anAccount == null || anAccount.Patient == null)
            {
                return true;
            }
            
            SocialSecurityNumber ssnNumber  =  anAccount.Patient.SocialSecurityNumber;
            
            if (ssnNumber == null)
            {
                return true;
            }

            var listOfContactPoints = anAccount.Facility.ContactPoints;

            var aContactPoint = listOfContactPoints.Cast<ContactPoint>().FirstOrDefault( cp => cp != null );

            State state = null;
            
            if (aContactPoint != null && aContactPoint.Address != null)
            {
                state = aContactPoint.Address.State;
            }
            
            if(!anAccount.BillHasDropped)
            {

                if (state != null && state.IsFlorida)
                {
                    if (ssnNumber.ToString() == "777777777" && ssnNumber.SSNStatus == SocialSecurityNumberStatus.UnknownSSNStatus)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                
                else if (state != null && ( state.IsCalifornia || state.IsSouthCarolina ))
                {
                    if (ssnNumber.ToString() == "000000001" && ssnNumber.SSNStatus == SocialSecurityNumberStatus.UnknownSSNStatus)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (ssnNumber.ToString() == "999999999" && ssnNumber.SSNStatus == SocialSecurityNumberStatus.UnknownSSNStatus)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            
            else
            {
                return true;
            }
               
                    
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public UnknownSocialSecurityNumberStatus()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
