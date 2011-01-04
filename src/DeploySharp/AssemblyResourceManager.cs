using System;
using System.IO;
using System.Reflection;

namespace DeploySharp
{
	public interface IAssemblyResourceManager {
		void SaveToDisk (string resourceName, string targetPath);
	}

	public class AssemblyResourceManager : IAssemblyResourceManager
	{
		public AssemblyResourceManager ()
		{
			_assembly = GetType ().Assembly;
		}

		public void SaveToDisk (string resourceName, string targetPath)
		{
			var fullname = GetFullResourceName(resourceName);

			using (var inStream = _assembly.GetManifestResourceStream (fullname))
			{
				if (inStream == null)
					throw new ArgumentException ("Invalid resource name: " + fullname);

				using (var outStream = File.Create (targetPath))
				{
					var reader = new BinaryReader (inStream);
					var writer = new BinaryWriter (outStream);

					writer.Write (reader.ReadBytes ((int) reader.BaseStream.Length));
					writer.Flush ();
				}
			}
		}

		public Stream OpenResourceStream (string resourceName)
		{
			var fullname = GetFullResourceName (resourceName);

			return _assembly.GetManifestResourceStream (fullname);
		}

		private string GetFullResourceName(string resourceName)
		{
			return _assembly.GetName().Name + ".Resources." + resourceName;
		}

		private readonly Assembly _assembly;
	}
}
