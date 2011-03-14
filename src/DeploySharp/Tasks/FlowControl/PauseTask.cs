using System;
using DeploySharp.Core;

namespace DeploySharp.Tasks
{
	public class PauseTask : IExecutable
	{
		public string Message { get; set; }
		private bool _pauseEnabled = true;
		public bool PauseEnabled
		{
			get { return _pauseEnabled; }
			set { _pauseEnabled = value; }
		}

		public TaskResult Execute()
		{
			if (PauseEnabled)
			{
				if (string.IsNullOrEmpty (Message) == false)
					Console.WriteLine (Message);

				Console.WriteLine ("Press enter to continue. (CTRL-C to abort)");
				Console.Read();
			}
			return new TaskResult();
		}
	}
}