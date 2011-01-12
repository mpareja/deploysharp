using DeploySharp.Core;

namespace DeploySharp.TaskResultReceivers
{
	public class NullTaskResultReceiver : ITaskResultReceiver
	{
		public void ReceiveError(string message) { }

		public void ReceiveSuccess(string message) { }
	}
}