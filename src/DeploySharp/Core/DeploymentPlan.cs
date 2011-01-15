using System;
using System.Collections.Generic;

using DeploySharp.TaskResultReceivers;

namespace DeploySharp.Core
{
	public class DeploymentPlan : IDeploymentPlanDsl, IDisposable
	{
		public DeploymentPlan(TaskBuilder builder)
		{
			_builder = builder;
			_taskQueue = new Queue<object>();
		}

		public void RunPreparations()
		{
			RunPreparations(new NullTaskResultReceiver());
		}

		public void RunPreparations(ITaskResultReceiver receiver)
		{
			if (receiver == null) throw new ArgumentNullException ("receiver");

			foreach (var task in _taskQueue)
			{
				var preparable = task as IPreparable;
				if (preparable != null)
				{
					var result = preparable.Prepare ();
					result.SendSubResultsTo (receiver);
					if (result.ContainsError ())
						break;
				}
			}
		}

		public void RunPlan()
		{
			RunPlan (new NullTaskResultReceiver());
		}

		public void RunPlan(ITaskResultReceiver receiver)
		{
			if (receiver == null) throw new ArgumentNullException ("receiver");

			foreach (var task in _taskQueue)
			{
				var executable = task as IExecutable;
				if (executable != null)
				{
					var result = executable.Execute();
					result.SendSubResultsTo (receiver);
					if (result.ContainsError ())
						break;
				}
			}

			DisposeTasks();
		}

		public IDeploymentPlanDsl ExecuteTask<T>() where T : class, IExecutable
		{
			return ExecuteTask<T> (t => { });
		}

		public IDeploymentPlanDsl ExecuteTask<T>(Action<T> configure) where T : class, IExecutable
		{
			if (configure == null) throw new ArgumentNullException ("configure");

			var task = _builder.BuildTask<T>();
			if (task == null)
				throw new InvalidOperationException("Unable to build task of type: " + typeof(T).FullName);

			configure (task);

			_taskQueue.Enqueue(task);
			return this;
		}

		public void Dispose()
		{
			DisposeTasks();
		}

		private void DisposeTasks()
		{
			foreach (var task in _taskQueue)
			{
				var disposable = task as IDisposable;
				if (disposable != null)
					disposable.Dispose ();
			}
		}

		private readonly TaskBuilder _builder;
		private readonly Queue<object> _taskQueue;
	}

	public interface IDeploymentPlanDsl
	{
		IDeploymentPlanDsl ExecuteTask<T>() where T : class, IExecutable;

		IDeploymentPlanDsl ExecuteTask<T> (Action<T> configure) where T : class, IExecutable;
	}
}