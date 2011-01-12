namespace DeploySharp.Core
{
	public interface ITaskResultReceiver
	{
		void ReceiveError (string message);

		void ReceiveSuccess (string message);
	}
}