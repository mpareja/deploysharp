using DeploySharp.Core;
using DeploySharp.Tasks;

using DeploySharp;
using DeploySharp.Infrastructure;

using Moq;

using NDepend.Helpers.FileDirectoryPath;

using Ninject;

using NUnit.Framework;

namespace Tests.Deployment.Tasks
{
	[TestFixture]
	public class TaskTests : TestFixtureBase
	{
		[Test]
		public void ExtractWebsiteTask_ExtractsAllFiles_ToApplicationDirectoryBasedOnBuildNumber()
		{
			var task = Container.Get<ExtractWebsiteTask>();

			task.Execute (new TaskContext
			{
				Target = new TargetConfig ("beta.example.org", new DirectoryPathAbsolute (@"d:\domains\beta.example.org")),
				Build = new BuildInfo ("2.14", "2010.12.8.1")
			});

			Container.GetMock<IExtractZip>().Verify (z => z.ExtractAllFiles ("website.zip",
				@"d:\domains\beta.example.org-2.14.2010.12.8.1"));
		}

		[Test]
		public void AppOfflineTask_CopiesAppOffline_ToCorrectLocation()
		{
			var task = Container.Get<AppOfflineTask>();

			Container.GetMock<IWebServer>().Setup (server => server.GetLmsDirFor (It.IsAny<string>()))
				.Returns (new DirectoryPathAbsolute (@"d:\sites\beta.example.com-2.14.2010.10.11.1\Lms"));
			Container.GetMock<IWebServer> ().Setup (server => server.GetAeDirFor (It.IsAny<string> ()))
				.Returns (new DirectoryPathAbsolute (@"d:\sites\beta.example.com-2.14.2010.10.11.1\Ae"));

			task.Execute (new TaskContext {
				Target = new TargetConfig ("beta.example.org", new DirectoryPathAbsolute (@"d:\sites\beta.example.com"))
			});

			var resourceMan = Container.GetMock<IAssemblyResourceManager> ();
			resourceMan.Verify (x => x.SaveToDisk ("App_Offline.htm",
				@"d:\sites\beta.example.com-2.14.2010.10.11.1\Ae\App_Offline.htm"));
			resourceMan.Verify (x => x.SaveToDisk ("App_Offline.htm",
				@"d:\sites\beta.example.com-2.14.2010.10.11.1\Lms\App_Offline.htm"));
		}

		[Test]
		public void ApplyWebConfigFilesToSite_CopiesLmsWebConfig_ToCorrectLocation()
		{
			var repoMock = new Mock<ConfigurationRepository>();
			var task = new ApplyWebConfigFilesToSite (repoMock.Object);

			task.Execute (new TaskContext {
				Target = new TargetConfig ("beta.example.com", new DirectoryPathAbsolute (@"d:\sites\beta.example.com")),
				Build = new BuildInfo ("2.14", "2")
			});

			repoMock.Verify (x => x.SaveLmsWebConfigFile (
				"beta.example.com", new DirectoryPathAbsolute(@"d:\sites\beta.example.com-2.14.2\Lms")));
		}

		[Test]
		public void ApplyWebConfigFilesToSite_CopiesAeWebConfig_ToCorrectLocation()
		{
			var repoMock = new Mock<ConfigurationRepository> ();
			var task = new ApplyWebConfigFilesToSite (repoMock.Object);

			task.Execute (new TaskContext
			{
				Target = new TargetConfig ("beta.example.com", new DirectoryPathAbsolute (@"d:\sites\beta.example.com")),
				Build = new BuildInfo ("2.14", "2")
			});

			repoMock.Verify (x => x.SaveAeWebConfigFile (
				"beta.example.com", new DirectoryPathAbsolute (@"d:\sites\beta.example.com-2.14.2\Ae")));
		}
	}
}
