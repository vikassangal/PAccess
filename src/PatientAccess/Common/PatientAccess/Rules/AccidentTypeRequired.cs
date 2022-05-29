using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for AccidentTypeRequired.
	/// </summary>
	//TODO: Create XML summary comment for AccidentTypeRequired
	[Serializable]
	[UsedImplicitly]
    public class AccidentTypeRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler AccidentTypeRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.AccidentTypeRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.AccidentTypeRequiredEvent -= eventHandler;
			return true;
		}
                                
        public override void UnregisterHandlers()
        {
            this.AccidentTypeRequiredEvent = null;  
        }

		public override bool CanBeAppliedTo(object context)
		{
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 			
            
            if( ((Account)context).Diagnosis.Condition.GetType().Name == "Accident" )
			{
				Accident accident = ( Accident )((Account)context).Diagnosis.Condition;
				if( accident == null || 
                    accident.Kind == null ||
					accident.Kind.Description == null ||
					accident.Kind.Description == String.Empty )
				{
					if( this.FireEvents && AccidentTypeRequiredEvent != null )
					{
						this.AccidentTypeRequiredEvent(this, null);
					}
            
					return false;
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
			return false;
		}
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public AccidentTypeRequired()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
