namespace DeploySharp.Core
{
	public interface ITaskResultReceiver
	{
		void ReceiveSuccess (string message);

		void ReceiveWarning (string message);

		void ReceiveError(string message);
	}
}