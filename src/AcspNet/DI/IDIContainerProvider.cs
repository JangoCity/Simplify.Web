﻿using System;

namespace AcspNet.DI
{
	/// <summary>
	/// Represents DI container provider
	/// </summary>
	public interface IDIContainerProvider : IHideObjectMembers
	{
		/// <summary>
		/// Resolves the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		object Resolve(Type type);

		/// <summary>
		/// Registers the specified service type with corresponding implemetation type.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="implementationType">Implementation type.</param>
		/// <param name="lifetimeType">Lifetime type of the registering service type.</param>
		void Register(Type serviceType, Type implementationType, LifetimeType lifetimeType = LifetimeType.Singleton);

		/// <summary>
		/// Registers the specified service type for resolve with delegate for service implementaion instance creation.
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="instanceCreator">The instance creator.</param>
		/// <param name="lifetimeType">Lifetime type of the registering concrete type.</param>
		void Register<TService>(Func<IDIContainerProvider, TService> instanceCreator,
			LifetimeType lifetimeType = LifetimeType.Singleton)
			where TService : class;

		/// <summary>
		/// Begins the lifetime scope.
		/// </summary>
		/// <returns></returns>
		ILifetimeScope BeginLifetimeScope();
	}
}