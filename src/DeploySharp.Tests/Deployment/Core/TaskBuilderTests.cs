using DeploySharp.Core;

using Ninject;

using NUnit.Framework;

namespace DeploySharp.Tests.Deployment.Core
{
	[TestFixture]
	public class TaskBuilderTests
	{
		[Test]
		public void WhenGivenATaskType_CanBuildIt()
		{
			var task = new TaskBuilder ().BuildTask<TestTask> ();

			Assert.IsNotNull (task);
		}

		[Test]
		public void WhenGivenATaskTypeWithServicesInConstructor_CanBuildIt()
		{
			var kernel = new StandardKernel();
			kernel.Bind<IMyService>().To<TestMyService>();

			var builder = new TaskBuilder(kernel);
			var task = builder.BuildTask<TestTaskRequiringService> ();

			Assert.IsNotNull (task);
			Assert.IsInstanceOf(typeof(TestMyService), task.Service);
		}

		public class TestTask : IExecutable
		{
			public TaskResult Execute() { return new TaskResult(); }
		}

		public interface IMyService{}
		public class TestMyService : IMyService {}

		public class TestTaskRequiringService : IExecutable
		{
			public IMyService Service { get; set; }

			public TestTaskRequiringService(IMyService service)
			{
				Service = service;
			}

			public TaskResult Execute()
			{
				return new TaskResult();
			}
		}
	}
}