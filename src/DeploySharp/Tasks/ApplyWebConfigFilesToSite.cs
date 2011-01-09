using DeploySharp.Core;

namespace DeploySharp.Tasks
{
	public class ApplyWebConfigFilesToSite : IExecutable
	{
		public ApplyWebConfigFilesToSite(ConfigurationRepository configRepo)
		{
			_configRepo = configRepo;
		}

		public TargetConfig Target { get; set; }

		public BuildInfo Build { get; set; }

		public void Execute()
		{
			var appPath = ExtractWebsiteTask.GetAppDirectory (Target.SiteRoot, Build);
			
			_configRepo.SaveAeWebConfigFile (Target.SiteName, appPath.GetChildDirectoryWithName ("Ae"));
			_configRepo.SaveLmsWebConfigFile (Target.SiteName, appPath.GetChildDirectoryWithName ("Lms"));
		}

		private readonly ConfigurationRepository _configRepo;
	}
}