using System;
using System.Collections.Generic;

namespace DeploySharp.Core
{
	public class DeploymentPlan : IDeploymentPlanDsl
	{

		public DeploymentPlan(IExecuteTasks executeTasks)
		{
			_executeTasks = executeTasks;
			_taskQueue = new Queue<Type>();
		}

		public void RunPlan()
		{
			foreach (var taskType in _taskQueue)
				_executeTasks.ExecuteTask(taskType);
		}

		public IDeploymentPlanDsl ExecuteTask<T>() where T : IExecutableWithContext
		{
			_taskQueue.Enqueue(typeof(T));
			return this;
		}

		public IDeploymentPlanDsl ThenExecute<T>() where T : IExecutableWithContext
		{
			return ExecuteTask<T>();
		}

		private readonly IExecuteTasks _executeTasks;
		private Queue<Type> _taskQueue;
	}

	public interface IDeploymentPlanDsl
	{
		IDeploymentPlanDsl ThenExecute<T>() where T : IExecutableWithContext;
	}
}