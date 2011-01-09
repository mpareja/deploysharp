using System;
using System.Collections.Generic;
using System.Reflection;

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
			Assert.AreEqual (typeof (Task1), calls[0].DeclaringType);
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

			Assert.AreEqual (typeof (Task1), calls[0].DeclaringType, "Task1 should have ran first!");
			Assert.AreEqual (typeof (Task2), calls[1].DeclaringType, "Task2 should have ran second!");
		}

		[Test]
		public void EnableRunningPreparations()
		{
			_plan.ExecuteTask<PreparableTask>();

			_plan.RunPreparations();

			var calls = ExecuteOrderHelper.Calls();
			Assert.AreEqual (1, calls.Length);
			
			Assert.AreEqual (typeof (PreparableTask), calls[0].DeclaringType);
			Assert.AreEqual ("Prepare", calls[0].Name);
		}

		public class Task1 : IExecutable
		{
			public void Execute()
			{
				ExecuteOrderHelper.LogCall (GetType().GetMethod ("Execute"));
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