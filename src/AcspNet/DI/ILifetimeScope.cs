﻿using System;

namespace AcspNet.DI
{
	/// <summary>
	/// Represents DI Container provider lifetime scope
	/// </summary>
	public interface ILifetimeScope : IDisposable, IHideObjectMembers
	{
		/// <summary>
		/// Gets the DI container provider (shoud be used to resolve types when using scoping).
		/// </summary>
		/// <value>
		/// The DI container provider (shoud be used to resolve types when using scoping).
		/// </value>
		IDIContainerProvider Container { get; }
	}
}