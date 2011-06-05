using System.IO;
using System.Web.Services.Protocols;

using DeploySharp.Core;
using DeploySharp.ReportWebService;

using NDepend.Helpers.FileDirectoryPath;


namespace DeploySharp.Tasks.Ssrs
{
	public class DeploySsrsReportTask : SsrsTask, IExecutable, IPreparable
	{
		public PathMate.FilePath ReportRdl { get; set; }
		public DirectoryPath DestinationPathOnServer { get; set; }
		public bool DeleteReportFirst { get; set; }

		TaskResult IPreparable.Prepare()
		{
			var results = new TaskResult();
			var rdlPath = ReportRdl.MakeAbsolute();
			if (rdlPath.FileInfo.Exists)
			{
				try
				{
					_rdl = File.ReadAllBytes (rdlPath);
				}
				catch (IOException e)
				{
					results.Error("Error while loading report file '{0}':\n{1}",
						ReportRdl, e.Message);
				}
			}
			else
			{
				results.Error("Report definition (.rdl) file not found: " + ReportRdl);
			}
			return results;
		}

		TaskResult IExecutable.Execute()
		{
			var results = new TaskResult();

			Warning[] warnings;
			var name = ReportRdl.FileNameWithoutExtension;
			var rs = GetWsClient ();
			try
			{
				rs.CreateCatalogItem ("Report", name, DestinationPathOnServer.Path,
				                      true, _rdl, null, out warnings);

				results.Success ("Report '{0}' was created successfully in server temp directory.", name);
			}
			catch (SoapException e)
			{
				results.Error ("Report '{0}' not created successfully. Error:\n{1}\n",
					name, e.Detail.InnerXml);
				return results;
			}

			if (warnings != null)
			{
				foreach (var warning in warnings)
					results.Warning (warning.Message);
			}
			return results;
		}

		private byte[] _rdl;
	}
}