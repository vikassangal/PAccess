namespace Extensions.UI.Builder
{
	/// <summary>
	/// Interface defined for rules that may be applied to SalaryActions.
	/// These rules can be load by specific applications at run time to
	/// avoid coupling the SalaryAction with a specific set of rules at
	/// design time.
	/// </summary>
	public interface IRule
	{
        #region Methods
        /// <summary>
        /// Force the Context object to conform to the constraints defined within the rule. This may change the state of the Context object (for instance to adjust values back into an acceptable range). 
        /// </summary>
        /// <param name="Context">
        /// Context onto which the rule will be applied.
        /// </param>
        void ApplyTo( object Context );
        
        /// <summary>
        /// Evaluate a block of code (the business rule or logic) and return true or false.
        /// </summary>
        /// <param name="Context">
        /// An object of arbitrary complexity used to evaluate the rule logic. 
        /// </param>
        /// <returns>
        /// True or false based on the evaluation of the rule logic. The rule should return true to Enable controls and false to Disable controls.
        /// </returns>
        bool CanBeAppliedTo( object Context );
        
        /// <summary>
        /// Determines if this rule should prevent the UIBuilder from processing any further rules. The UIBuilder is not required to send this message during the build process.
        /// </summary>
        /// <returns>
        /// True if the UIBuilder should stop processing subsequent rules.
        /// </returns>
        bool ShouldStopProcessing();
        #endregion

        #region Properties
        long Oid
        {
            get;
            set;
        }
        #endregion
	}
}
