﻿namespace AcspNet.DI.DIContainerProvider.DryIoc
{
	/// <summary>
	/// DryIoc DI provider lifetime scope implemetation
	/// </summary>
	public class DryIocLifetimeScope : ILifetimeScope
	{
		private readonly DryIocDIProvider _currentScopeProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="DryIocLifetimeScope"/> class.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public DryIocLifetimeScope(DryIocDIProvider provider)
		{
			Container = provider;

			_currentScopeProvider = new DryIocDIProvider { Container = provider.Container.OpenScope() };
		}

		/// <summary>
		/// Gets the DI container provider (shoud be user to resolve types when using scoping).
		/// </summary>
		/// <value>
		/// The DI container provider (shoud be user to resolve types when using scoping).
		/// </value>
		public IDIContainerProvider Container { get; private set; }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_currentScopeProvider.Container.Dispose();
		}
	}
}