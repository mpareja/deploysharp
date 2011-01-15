using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CommonServiceLocator.NinjectAdapter;

using DeploySharp.Core;

using Ninject;

using NUnit.Framework;

namespace DeploySharp.Tests.Deployment.Core
{
	[TestFixture]
	public class DeploymentPlanTests
	{
		[SetUp]
		public void Setup()
		{
			var locator = new NinjectServiceLocator (new StandardKernel ());
			_plan = new DeploymentPlan (new TaskBuilder (locator));

			ExecuteOrderHelper.Reset ();
		}

		[Test]
		public void RunningPlanWithoutTasks_RunsNothing()
		{
			_plan.RunPlan();
		}

		[Test]
		public void RunningPlanWithATask_RunsTheTask()
		{
			_plan.ExecuteTask<Task1>();

			_plan.RunPlan();

			AssertTaskExecutionOrder<Task1> (1);
		}

		[Test]
		public void RunningPlanWithTwoTasks_RunsBothTasksInOrder()
		{
			_plan
				.ExecuteTask<Task1>()
				.ExecuteTask<Task2>();

			_plan.RunPlan();

			AssertTaskExecutionOrder<Task1> (1);
			AssertTaskExecutionOrder<Task2> (2);
		}

		[Test]
		public void EnableRunningPreparations()
		{
			_plan.ExecuteTask<PreparableTask> ();

			_plan.RunPreparations();

			AssertTaskPreparationOrder<PreparableTask> (1);
		}

		[Test]
		public void RunningPreparationsWithFailure_AbortsPrepExecution()
		{
			_plan
				.ExecuteTask<PreparableTask>()
				.ExecuteTask<FailingPreparableTask>()
				.ExecuteTask<PreparableTask2> ();

			_plan.RunPreparations();

			AssertTaskPreparationOrder<PreparableTask> (1);
			AssertTaskPreparationOrder<FailingPreparableTask> (2);
			AssertTaskNeverPrepared<PreparableTask2>();
		}

		[Test]
		public void RunningPlanWithFailure_AbortsPlanExecution()
		{
			_plan
				.ExecuteTask<PreparableTask> ()
				.ExecuteTask<FailingPreparableTask> ()
				.ExecuteTask<PreparableTask2> ();

			_plan.RunPlan();

			AssertTaskExecutionOrder<PreparableTask> (1);
			AssertTaskExecutionOrder<FailingPreparableTask> (2);
			AssertTaskNeverExecuted<PreparableTask2> ();
		}

		[Test]
		public void EnableConfiguringATask()
		{
			_plan
				.ExecuteTask<TaskRequiringConfig>(t => t.Configured = true);

			_plan.RunPreparations();
		}

		private void AssertTaskPreparationOrder<T> (int orderNumber)
		{
			AssertOrder<T> (orderNumber, "Prepare");
		}
		private void AssertTaskExecutionOrder<T> (int orderNumber)
		{
			AssertOrder<T> (orderNumber, "Execute");
		}
		private void AssertTaskNeverPrepared<T>()
		{
			AssertTaskNeverExecuted<T> ("Prepare");
		}
		private void AssertTaskNeverExecuted<T>()
		{
			AssertTaskNeverExecuted<T> ("Execute");
		}

		private void AssertTaskNeverExecuted<T>(string methodName)
		{
			var type = typeof(T);
			var typeExecuted = (
					from c in ExecuteOrderHelper.Calls()
			        where c.Name == methodName && c.DeclaringType == type
			        select c).Any();

			Assert.IsFalse (typeExecuted, 
				string.Format("{0} should not have been called for task {1}!",
				methodName, type.Name));
		}
		private void AssertOrder<T> (int orderNumber, string methodName)
		{
			var calls = ExecuteOrderHelper.Calls ();

			Assert.IsNotNull (calls);
			Assert.That (orderNumber, Is.AtMost (calls.Length),
			             "Expected execution order number is greater than the number of actual executions.");

			var ix = orderNumber - 1; // make it zero based
			var expectedType = typeof (T);
			var actual = calls[ix];
			Assert.AreEqual (expectedType, actual.DeclaringType, 
			                 string.Format("#{0} task {2} should have been {1}!", orderNumber, expectedType.Name, methodName));
			Assert.AreEqual (methodName, actual.Name);
		}

		public class Task1 : IExecutable
		{
			public TaskResult Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
				return new TaskResult ();
			}
		}

		public class Task2 : IExecutable
		{
			public TaskResult Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
				return new TaskResult ();
			}
		}

		public class PreparableTask : IExecutable, IPreparable
		{
			public TaskResult Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
				return new TaskResult ();
			}

			public TaskResult Prepare()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Prepare"));
				return new TaskResult();
			}
		}

		public class PreparableTask2 : IExecutable, IPreparable
		{
			public TaskResult Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
				return new TaskResult ();
			}

			public TaskResult Prepare()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Prepare"));
				return new TaskResult ();
			}
		}

		public class FailingPreparableTask : IExecutable, IPreparable
		{
			public TaskResult Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
				var result = new TaskResult ();
				result.Error ("my error");
				return result;
			}

			public TaskResult Prepare()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Prepare"));
				var result = new TaskResult ();
				result.Error ("my error");
				return result;
			}
		}

		public class TaskRequiringConfig : IExecutable, IPreparable
		{
			public bool Configured = false;

			public TaskResult Execute()
			{
				if (Configured == false)
					Assert.Fail ("Expected task to be configured before being executed.");
				return new TaskResult();
			}

			public TaskResult Prepare()
			{
				if (Configured == false)
					Assert.Fail ("Expected task to be configured before being prepared.");
				return new TaskResult ();
			}
		}

		public class ExecuteOrderHelper
		{
			public ExecuteOrderHelper() { Reset(); }

			public static void LogCall(MethodInfo method)
			{
				_queue.Enqueue (method);
			}

			public static MethodInfo[] Calls()
			{
				return _queue.ToArray();
			}

			public static void Reset()
			{
				_queue = new Queue<MethodInfo> ();
			}

			private static Queue<MethodInfo> _queue;
		}

		private DeploymentPlan _plan;
	}

}