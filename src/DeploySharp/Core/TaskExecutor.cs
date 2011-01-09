using System;

using Microsoft.Practices.ServiceLocation;

namespace DeploySharp.Core
{
	public class TaskExecutor : IExecuteTasks
	{
		public TaskExecutor(IServiceLocator locator)
		{
			_locator = locator;
		}

		public void ExecuteTask(Type taskType)
		{
			var executable = _locator.GetInstance (taskType) as IExecutable;
			if (executable == null)
				throw new InvalidOperationException(taskType.Name + " type does not implement IExecutableWithContext or IExecutable.");

			executable.Execute ();
		}
		private readonly IServiceLocator _locator;
	}
}