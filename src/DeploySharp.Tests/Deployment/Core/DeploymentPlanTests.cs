using System;
using System.Collections.Generic;

using CommonServiceLocator.NinjectAdapter;

using DeploySharp.Core;
using DeploySharp.Tasks;

using Ninject;

using NUnit.Framework;

namespace Tests.Deployment.Core
{
	[TestFixture]
	public class DeploymentPlanTests : TestFixtureBase
	{
		public override void Setup()
		{
			var locator = new NinjectServiceLocator (new StandardKernel ());
			_plan = new DeploymentPlan (new TaskBuilder (locator));

			ExecuteOrderHelper.Reset ();
		}

		[Test]
		public void can_create()
		{
			Assert.IsNotNull(_plan);
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

			var calls = ExecuteOrderHelper.Calls ();
			Assert.AreEqual (1, calls.Length);
			Assert.AreEqual (typeof(Task1), calls[0]);
		}

		[Test]
		public void RunningPlanWithTwoTasks_RunsBothTasksInOrder()
		{
			_plan
				.ExecuteTask<Task1>()
				.ThenExecute<Task2>();

			_plan.RunPlan();

			var calls = ExecuteOrderHelper.Calls();
			Assert.AreEqual (2, calls.Length, "Should have executed 2 tasks.");

			Assert.AreEqual (typeof (Task1), calls[0], "Task1 should have ran first!");
			Assert.AreEqual (typeof (Task2), calls[1], "Task2 should have ran second!");
		}

		public class Task1 : IExecutable
		{
			public void Execute()
			{
				ExecuteOrderHelper.LogCall (GetType());
			}
		}

		public class Task2 : Task1 { }

		public class ExecuteOrderHelper
		{
			public ExecuteOrderHelper() { Reset(); }

			public static void LogCall(Type type)
			{
				_queue.Enqueue (type);
			}

			public static Type[] Calls()
			{
				return _queue.ToArray();
			}

			public static void Reset()
			{
				_queue = new Queue<Type>();
			}

			private static Queue<Type> _queue;
		}

		private DeploymentPlan _plan;
	}
}