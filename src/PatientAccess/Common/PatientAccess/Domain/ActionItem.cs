namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for ActionItem.
	/// </summary>
	public class ActionItem
	{
		public ActionItem()
		{
		}

        public bool CanBeAppliedTo
        {
            get
            {
                return this.i_CanBeAppliedTo;
            }
            set
            {
                this.i_CanBeAppliedTo = value;
            }
        }

        public bool ShouldStopProcessing
        {
            get
            {
                return this.i_ShouldStopProcessing;
            }
            set
            {
                this.i_ShouldStopProcessing = value;
            }
        }

        private bool i_CanBeAppliedTo;
        private bool i_ShouldStopProcessing;
	}
}
