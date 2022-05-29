using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidRelForPriInsuredChange Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidRelForPriInsuredChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidRelForPriInsuredChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidRelForPriInsuredChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidRelForPriInsuredChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidRelForPriInsuredChangeEvent = null;   
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

            if( ( account.Insurance != null ) && ( account.Patient != null ))
            {
                Coverage coverage = account.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                if((coverage != null) && (coverage.Insured != null) )
                {                   
                    RelationshipType rel = account.Patient.RelationshipWith(coverage.Insured);
                    if(rel != null)
                    {
                        if(!rel.IsValid)                                        
                        {
                   
                            if( this.FireEvents && InvalidRelForPriInsuredChangeEvent != null )
                            {
                                this.InvalidRelForPriInsuredChangeEvent(this, null);
                            }
                
                            return false ;
                        }
                        else
                        {
                            return true;
                        }      
                   
                    }
                    else
                    {
                        return true;
                    }      
                    
               
                }
                else
                {
                    return true;
                }       
          
            }
            else
            {
                return true;
            }       
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

        public InvalidRelForPriInsuredChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

