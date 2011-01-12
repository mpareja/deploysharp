using System;

using DeploySharp.Core;

namespace DeploySharp.TaskResultReceivers
{
	public class ConsoleTaskResultReceiver : ITaskResultReceiver
	{
		public void ReceiveError(string message)
		{
			var colour = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine ("ERROR: " + message);
			Console.ForegroundColor = colour;
		}

		public void ReceiveSuccess(string message)
		{
			Console.WriteLine (message);
		}
	}
}