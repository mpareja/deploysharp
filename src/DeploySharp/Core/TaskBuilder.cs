using System;

namespace DeploySharp.Core
{
	public class TaskBuilder
	{
		public TaskBuilder()
		{
			_provider = null;
		}
		public TaskBuilder(IServiceProvider provider)
		{
			_provider = provider;
		}

		public T BuildTask<T>()
		{
			if (_provider != null)
			{
				return (T) _provider.GetService(typeof(T));
			}
			return (T) Activator.CreateInstance(typeof (T));
		}

		private readonly IServiceProvider _provider;
	}
}