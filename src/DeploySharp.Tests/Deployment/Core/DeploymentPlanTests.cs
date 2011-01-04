using System;

using DeploySharp.Core;
using DeploySharp.Tasks;

using Moq;

using NUnit.Framework;

namespace Tests.Deployment.Core
{
	[TestFixture]
	public class DeploymentPlanTests
	{
		[SetUp]
		public void Setup()
		{
			// strict so mock will throw if RunPlan called and not expected
			_executor = new Mock<IExecuteTasks>(MockBehavior.Strict);
			_plan = new DeploymentPlan(_executor.Object);
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
			_plan.ExecuteTask<ExtractWebsiteTask>();
			_executor.Setup(x => x.ExecuteTask(typeof(ExtractWebsiteTask))).Verifiable();

			_plan.RunPlan();

			_executor.Verify();
		}

		[Test]
		public void RunningPlanWithTwoTasks_RunsBothTasksInOrder()
		{
			_plan
				.ExecuteTask<ExtractWebsiteTask>()
				.ThenExecute<AppOfflineTask>();

			var count = 0;
			var appOffline = -1;
			var extract = -1;
			_executor.Setup(x => x.ExecuteTask(typeof(ExtractWebsiteTask)))
				.Callback<Type>(t => extract = count++).Verifiable();

			_executor.Setup(x => x.ExecuteTask(typeof(AppOfflineTask)))
				.Callback<Type>(t => appOffline = count++).Verifiable();

			_plan.RunPlan();

			Assert.AreNotEqual(-1, extract, "ExtractWebsiteTask never ran!");
			Assert.AreNotEqual(-1, appOffline, "AppOfflineTask never ran!");
			Assert.AreEqual(0, extract, "ExtractWebsiteTask should have ran first!");
			Assert.AreEqual(1, appOffline, "AppOfflineTask should have ran second!");
		}

		private Mock<IExecuteTasks> _executor;
		private DeploymentPlan _plan;
	}
}