using System;

using DeploySharp.Core;

namespace DeploySharp.TaskResultReceivers
{
	public class ConsoleTaskResultReceiver : ITaskResultReceiver
	{
		public void ReceiveSuccess(string message)
		{
			Console.WriteLine (message);
		}

		public void ReceiveWarning(string message)
		{
			Console.WriteLine ("Warning: " + message);
		}

		public void ReceiveError(string message)
		{
			var colour = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine ("ERROR: " + message);
			Console.ForegroundColor = colour;
		}
	}
}