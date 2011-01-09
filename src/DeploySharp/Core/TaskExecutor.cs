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
			var instance = _locator.GetInstance (taskType);

			var ewc = instance as IExecutableWithContext;
			if (ewc != null)
				ewc.Execute (_locator.GetInstance<TaskContext> ()); ;

			var executable = instance as IExecutable;
			if (executable != null)
				executable.Execute();

			if (ewc == null && executable == null)
				throw new InvalidOperationException(taskType.Name + " type does not implement IExecutableWithContext or IExecutable.");
		}
		private readonly IServiceLocator _locator;
	}
}