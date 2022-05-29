using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Clinic_4Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidClinic_4CodeChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidClinic_4CodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidClinic_4CodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidClinic_4CodeChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidClinic_4CodeChangeEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;

                                    
            if( account.DischargeDate != DateTime.MinValue )
            {
                return true;
            }

            if( ( account != null ) &&  (account.Clinics  != null )
                &&  (account.Clinics.Count >0  ))
            {
                
                HospitalClinic clinic =  (HospitalClinic)account.Clinics[3] ;
                if (clinic != null)
                {

                    if((account.Activity.GetType().Equals( typeof( PreRegistrationActivity)))||
                        (account.Activity.GetType().Equals( typeof( PostMSERegistrationActivity)))                      
                        )
                    {
                        if(!(clinic.IsValid))
                          
                        {
                            if( this.FireEvents && InvalidClinic_4CodeChangeEvent != null )
                            {
                                this.InvalidClinic_4CodeChangeEvent(this, null);
                            }
            
                            return false ;
                        }
                    }
                    else
                        if(account.Activity.GetType().Equals( typeof( RegistrationActivity))
                        && 
                        ((account.KindOfVisit.Code == VisitType.OUTPATIENT ) ||
                        (account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT ) ||
                        (account.KindOfVisit.Code == VisitType.RECURRING_PATIENT ) ||
                        (account.KindOfVisit.Code == VisitType.NON_PATIENT ) )
                        )
                    {
                        if(!(clinic.IsValid))
                          
                        {
                            if( this.FireEvents && InvalidClinic_4CodeChangeEvent != null )
                            {
                                this.InvalidClinic_4CodeChangeEvent(this, null);
                            }
            
                            return false ;
                        }

                    }
                    else
                        if(account.Activity.GetType().Equals( typeof( MaintenanceActivity))
                        && (account.KindOfVisit.Code == VisitType.PREREG_PATIENT))
                        
                    {
                        if(!(clinic.IsValid))
                          
                        {
                            if( this.FireEvents && InvalidClinic_4CodeChangeEvent != null )
                            {
                                this.InvalidClinic_4CodeChangeEvent(this, null);
                            }
            
                            return false ;
                        }

                    }
                    else
                        if(account.Activity.GetType().Equals( typeof( MaintenanceActivity))
                        && 
                        ( ((account.KindOfVisit.Code == VisitType.OUTPATIENT ) ||
                        (account.KindOfVisit.Code == VisitType.RECURRING_PATIENT ) ||
                        (account.KindOfVisit.Code == VisitType.NON_PATIENT ) ) ||

                        ((account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT ) &&
                        ((account.FinancialClass == null)||
                        ((account.FinancialClass != null)&&
                        (!account.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE )))))))                   
                        
                    {
                        if(!(clinic.IsValid))
                          
                        {
                            if( this.FireEvents && InvalidClinic_4CodeChangeEvent != null )
                            {
                                this.InvalidClinic_4CodeChangeEvent(this, null);
                            }
            
                            return false ;
                        }

                    }

                }
                   
            }
                
            
            return true;
        }


        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public InvalidClinic_4CodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


