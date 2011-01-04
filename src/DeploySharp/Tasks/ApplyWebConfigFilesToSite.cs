using DeploySharp.Core;

using DeploySharp.Tasks;

namespace DeploySharp.Tasks
{
	public class ApplyWebConfigFilesToSite : IExecutable
	{
		public ApplyWebConfigFilesToSite(ConfigurationRepository configRepo)
		{
			_configRepo = configRepo;
		}

		public void Execute(TaskContext context)
		{
			var appPath = ExtractWebsiteTask.GetAppDirectory (context);
			
			_configRepo.SaveAeWebConfigFile (context.Target.SiteName, appPath.GetChildDirectoryWithName ("Ae"));
			_configRepo.SaveLmsWebConfigFile (context.Target.SiteName, appPath.GetChildDirectoryWithName ("Lms"));
		}

		private readonly ConfigurationRepository _configRepo;
	}
}