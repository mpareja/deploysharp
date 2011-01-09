using System;

using CommonServiceLocator.NinjectAdapter;

using DeploySharp.Core;

using Ninject;

using NUnit.Framework;

namespace Tests.Deployment.Core
{
	[TestFixture]
	public class TaskExecutorTests
	{
		[SetUp]
		public void Before_each_spec()
		{
			_context = new TaskContext();

			// Container will always return _context when asked for TaskContext
			var standardKernel = new StandardKernel();
			standardKernel.Bind<TaskContext>().ToConstant (_context);

			_executor = new TaskExecutor(new NinjectServiceLocator (standardKernel));
		}

		[TearDown]
		public void Teardown()
		{
			TestTask.ExecuteCalled = 0;
		}

		[Test]
		public void CanExecuteATask_WhenPassedInAType()
		{
			TestTask.ExecuteCalled = 0;

			_executor.ExecuteTask (typeof(TestTask));

			Assert.AreEqual (1, TestTask.ExecuteCalled);
		}

		[Test]
		public void Throws_IfAskedToExecuteANonExecutableTask()
		{
			Assert.Throws<InvalidOperationException> (() =>
				_executor.ExecuteTask (typeof(TaskExecutorTests)));
		}

		public class TestTask : IExecutable
		{
			public void Execute()
			{
				ExecuteCalled++;
			}

			public static int ExecuteCalled = 0;
		}

		private TaskExecutor _executor;
		private TaskContext _context;
	}
}