using CommonServiceLocator.NinjectAdapter;

using DeploySharp.Core;

using Ninject;

using NUnit.Framework;

namespace DeploySharp.Tests.Deployment.Core
{
	[TestFixture]
	public class TaskBuilderTests
	{
		private TaskBuilder _builder;

		[SetUp]
		public void Before_each_spec()
		{
			var standardKernel = new StandardKernel ();

			_builder = new TaskBuilder (new NinjectServiceLocator (standardKernel));
		}

		[Test]
		public void WhenGivenATaskType_CanBuildIt()
		{
			var task = _builder.BuildTask<TestTask>();

			Assert.IsNotNull (task);
		}

		public class TestTask : IExecutable
		{
			public void Execute() { }
		}
	}
}