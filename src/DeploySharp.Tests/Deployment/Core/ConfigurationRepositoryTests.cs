using System.IO;

using DeploySharp.Core;
using DeploySharp.Infrastructure;

using Shouldly;

using NDepend.Helpers.FileDirectoryPath;

using NUnit.Framework;

namespace Tests.Deployment.Core
{
	[TestFixture]
	public class ConfigurationRepositoryTests
	{
		[SetUp]
		public void Before_each_spec()
		{
			// place dummy config files in repo
			var path = KnownPaths.ApplicationPath.Path + @"\Configuration\Web Configuration Files\beta.example.org";
			Directory.CreateDirectory (path + @"\LMS");
			Directory.CreateDirectory (path + @"\AE");

			File.WriteAllText (path + @"\LMS\Web.config", "LmsConfigFile");
			File.WriteAllText (path + @"\AE\Web.config", "AeConfigFile");
		}

		[TearDown]
		public void Teardown()
		{
			// remove dummy repo files
			Directory.Delete (KnownPaths.ApplicationPath.Path + @"\Configuration", true);
		}

		[Test]
		public void can_save_LMS_Web_config_file_to_specified_location()
		{
			using (var tempDir = new TempDirectoryPathAbsolute())
			{
				var lmsAppRoot = tempDir.AsDir;
				var repo = new ConfigurationRepository();
				repo.SaveLmsWebConfigFile ("beta.example.org", lmsAppRoot);

				var webconfig = lmsAppRoot.GetChildFileWithName ("Web.config");

				webconfig.Exists.ShouldBe (true);
				Assert.AreEqual ("LmsConfigFile", File.ReadAllText (webconfig.Path));
			}
		}

		[Test]
		public void can_save_AE_Web_config_file_to_specified_location()
		{
			using (var tempDir = new TempDirectoryPathAbsolute())
			{
				var aeAppRoot = tempDir.AsDir;
				var repo = new ConfigurationRepository ();
				repo.SaveAeWebConfigFile ("beta.example.org", aeAppRoot);

				var webconfig = aeAppRoot.GetChildFileWithName ("Web.config");

				webconfig.Exists.ShouldBe (true);
				Assert.AreEqual ("AeConfigFile", File.ReadAllText (webconfig.Path));
			}
		}
	}
}