﻿using System;

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
			var instance = _locator.GetInstance (taskType) as IExecutableWithContext;
			if (instance == null)
				throw new InvalidOperationException(taskType.Name + " type does not implement IExecutableWithContext.");

			instance.Execute (_locator.GetInstance<TaskContext>());;
		}
		private readonly IServiceLocator _locator;
	}
}