using Ninject.MockingKernel.Moq;

using NUnit.Framework;

namespace Tests
{
	public abstract class TestFixtureBase
	{
		public virtual void Setup() { }
		public virtual void TearDown() { }

		[SetUp]
		public void InternalSetup()
		{
			Setup();
		}

		[TearDown]
		public void InternalTearDown()
		{
			TearDown();

			if (_container != null)
			{
				_container.Dispose ();
				_container = null;
			}
		}

		public MoqMockingKernel Container
		{
			get
			{
				if(_container == null)
					_container = new MoqMockingKernel();
				return _container;
			}
		}

		private MoqMockingKernel _container;
	}
}
