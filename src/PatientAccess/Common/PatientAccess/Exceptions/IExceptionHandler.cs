namespace Extensions.Exceptions
{
	public interface IExceptionHandler
	{
        void HandleException( EnterpriseException exception );
	}
}
