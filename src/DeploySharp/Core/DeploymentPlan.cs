using System;
using System.Collections.Generic;

namespace DeploySharp.Core
{
	public class DeploymentPlan : IDeploymentPlanDsl
	{
		public DeploymentPlan(ITaskBuilder builder)
		{
			_builder = builder;
			_taskQueue = new Queue<object>();
		}

		public void RunPreparations()
		{
			foreach (var task in _taskQueue)
			{
				var preparable = task as IPreparable;
				if (preparable != null)
					preparable.Prepare ();
			}
		}

		public void RunPlan()
		{
			foreach (var task in _taskQueue)
			{
				var executable = task as IExecutable;
				if (executable != null)
					executable.Execute();
			}
		}

		public IDeploymentPlanDsl ExecuteTask<T>() where T : class, IExecutable
		{
			var task = _builder.BuildTask<T>();
			if (task == null)
				throw new InvalidOperationException("Unable to build task of type: " + typeof(T).FullName);

			_taskQueue.Enqueue(task);
			return this;
		}

		public IDeploymentPlanDsl ThenExecute<T>() where T : class, IExecutable
		{
			return ExecuteTask<T>();
		}

		private readonly ITaskBuilder _builder;
		private Queue<object> _taskQueue;
	}

	public interface IDeploymentPlanDsl
	{
		IDeploymentPlanDsl ThenExecute<T>() where T : class, IExecutable;
	}
}