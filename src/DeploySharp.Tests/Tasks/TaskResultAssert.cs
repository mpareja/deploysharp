using DeploySharp.Core;
using NUnit.Framework;

namespace DeploySharp.Tests.Tasks
{
	public static class TaskResultAssert
	{
		public static void AssertNoErrors(this TaskResult result)
		{
			Assert.NotNull (result, "Task results should not be null.");

			if (result.ContainsError())
			{
				var receiver = new ResultReceiver();
				result.SendSubResultsTo(receiver);
				Assert.Fail(receiver.Message);
			}
		}

		private class ResultReceiver : ITaskResultReceiver
		{
			public ResultReceiver()
			{
				Message = "";
			}
			public string Message { get; private set; }

			public void ReceiveSuccess(string message) { }
			public void ReceiveWarning(string message)
			{
				Message += "Warning: " + message + "\n";
			}

			public void ReceiveError(string message)
			{
				Message += "ERROR: " + message + "\n";
			}
		}
	}
}