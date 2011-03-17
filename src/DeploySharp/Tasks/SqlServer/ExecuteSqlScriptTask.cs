using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using DeploySharp.Core;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace DeploySharp.Tasks
{
	public class ExecuteSqlScriptsTask : IExecutable, IPreparable, IDisposable
	{
		public string Server { get; set; }
		public string Database { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public IEnumerable<string> ScriptFiles { get; set; }

		TaskResult IPreparable.Prepare()
		{
			var results = new TaskResult ();
			if (_foundSmoAssembly == false)
			{
				results.Error ("!!!! IF SQL SERVER NOT INSTALLED, INSTALL SQL SERVER SMO REDISTRIBUTABLE !!!!");
			}

			foreach (var path in ScriptFiles)
			{
				try
				{
					// obviously not the most memory efficient - consider deferring
					var script = File.ReadAllText (path, Encoding.Default);
					_scripts.Enqueue(new ScriptFile {
						Filename = path,
						Script = script
					});
				}
				catch(Exception e)
				{
					results.Error(e, "Error reading file: {0}", path);
				}
			}

			_conn = OpenConnection (results);
			return results;
		}

		TaskResult IExecutable.Execute()
		{
			var result = new TaskResult();
			result.Success("Executing scripts on server '{0}' database '{1}'.", Server, Database);
			foreach (var scriptFile in _scripts)
			{
				try
				{
					ExecuteSql(scriptFile.Script);
					result.Success("\tExecuted script: {0}", scriptFile.Filename);
				}
				catch(Exception e)
				{
					var exception = e.InnerException ?? e;
					result.Error("Error executing script: {0}:\n{1}",
						scriptFile.Filename, exception.Message);
				}
			}
			return result;
		}

		protected void ExecuteSql(string script)
		{
			ServerConnection.ConnectionContext.ExecuteNonQuery(script);
		}

		private Server ServerConnection
		{
			get
			{
				if (_server == null)
					_server = new Server (new ServerConnection (_conn));
				return _server;
			}
		}

		private SqlConnection OpenConnection(TaskResult results)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = Database,
                UserID = Username,
                Password = Password
            };

            var conn = new SqlConnection (builder.ToString ());
            try
            {
                conn.Open ();
            	results.Success("Successfully opened connection with SQL Server.");
            }
            catch (Exception e)
            {
                results.Error (e, "Unable to open connection to SQL Server '{0}'"
                    + " database '{1}'", Server, Database);
            }
            return conn;
        }

		void IDisposable.Dispose()
		{
			if (_conn != null)
			{
				_conn.Dispose ();
				_conn = null;
			}
		}

		protected class ScriptFile
		{
			public string Filename;
			public string Script;
		}

		/* An untested attempt at checking to make sure Microsoft SMOs are installed */
		static ExecuteSqlScriptsTask()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
			{
				if (args.Name.ToUpperInvariant ().Contains ("SMO"))
					_foundSmoAssembly = false;
				return null;
			};
		}
		private static bool _foundSmoAssembly = true;

		protected Queue<ScriptFile> _scripts = new Queue<ScriptFile> ();
		private Server _server;
		private SqlConnection _conn;
	}
}
