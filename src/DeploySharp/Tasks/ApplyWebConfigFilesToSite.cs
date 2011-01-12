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

		public TaskResult Execute()
		{
			var appPath = ExtractWebsiteTask.GetAppDirectory (Target.SiteRoot, Build);
			
			_configRepo.SaveAeWebConfigFile (Target.SiteName, appPath.GetChildDirectoryWithName ("Ae"));
			_configRepo.SaveLmsWebConfigFile (Target.SiteName, appPath.GetChildDirectoryWithName ("Lms"));

			var result = new TaskResult();
			result.Success ("Finished applying web.config files to site");
			return result;
		}

		private readonly ConfigurationRepository _configRepo;
	}
}