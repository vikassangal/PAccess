using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class ZeroPaymentReason : ReferenceValue 
    {

		#region Constructors 

        public ZeroPaymentReason( long oid, DateTime version, string description )
            : base( oid, version, description ){}

        public ZeroPaymentReason() { }

		#endregion Constructors 

		#region Properties 

        public override string Description
        {
            get
            {
                return base.Description;
            }
            set
            {

                string oldValue = base.Description;
                base.Description = value;

                if( base.Description != null && !base.Description.Equals( oldValue ) )
                    this.TrackChange( "Description", oldValue, value );

            }
        }

		#endregion Properties 

    }
}
