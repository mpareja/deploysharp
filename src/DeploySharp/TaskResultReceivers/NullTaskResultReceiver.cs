using DeploySharp.Core;

namespace DeploySharp.TaskResultReceivers
{
	public class NullTaskResultReceiver : ITaskResultReceiver
	{
		public void ReceiveSuccess(string message) { }

		public void ReceiveWarning(string message) { }

		public void ReceiveError(string message) { }
	}
}