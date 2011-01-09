using System.Collections.Generic;
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
				.ThenExecute<Task2>();

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

		private void AssertTaskPreparationOrder<T> (int orderNumber)
		{
			AssertOrder<T> (orderNumber, "Prepare");
		}
		private void AssertTaskExecutionOrder<T> (int orderNumber)
		{
			AssertOrder<T> (orderNumber, "Execute");
		}
		private void AssertOrder<T> (int orderNumber, string methodName)
		{
			var calls = ExecuteOrderHelper.Calls ();

			Assert.IsNotNull (calls);
			Assert.That (orderNumber, Is.AtMost (calls.Length),
			             "Expected execution order number is greater than the number of actual executions.");

			var ix = orderNumber - 1; // make it zero based
			var expectedType = typeof (T);
			Assert.AreEqual (expectedType, calls[ix].DeclaringType, 
			                 string.Format("#{0} task {2} should have been {1}!", orderNumber, expectedType.Name, methodName));
			Assert.AreEqual (methodName, calls[0].Name);
		}

		public class Task1 : IExecutable
		{
			public void Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
			}
		}

		public class Task2 : IExecutable
		{
			public void Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
			}
		}

		public class PreparableTask : IExecutable, IPreparable
		{
			public void Execute()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Execute"));
			}

			public void Prepare()
			{
				ExecuteOrderHelper.LogCall (GetType ().GetMethod ("Prepare"));
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