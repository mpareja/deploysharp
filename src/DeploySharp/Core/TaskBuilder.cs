using Microsoft.Practices.ServiceLocation;

namespace DeploySharp.Core
{
	public class TaskBuilder
	{
		public TaskBuilder(IServiceLocator locator)
		{
			_locator = locator;
		}

		public T BuildTask<T>()
		{
			return _locator.GetInstance<T>();
		}

		private readonly IServiceLocator _locator;
	}
}