﻿using System;
using AcspNet.DI;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace AcspNet.DIContainerProvider.CastleWindsor
{
	/// <summary>
	/// Castle Windsor container provider implementation
	/// </summary>
	public class CastleWindsorDIProvider : IDIContainerProvider
	{
		private WindsorContainer _container;

		/// <summary>
		/// The IOC container
		/// </summary>
		public WindsorContainer Container
		{
			get
			{
				return _container ?? (_container = new WindsorContainer());
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_container = value;
			}
		}

		/// <summary>
		/// Resolves the specified service type.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <returns></returns>
		public object Resolve(Type serviceType)
		{
			return Container.Resolve(serviceType);
		}

		/// <summary>
		/// Registers the specified service type with corresponding implementation type.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="implementationType">Implementation type.</param>
		/// <param name="lifetimeType">Lifetime type of the registering services type.</param>
		public void Register(Type serviceType, Type implementationType, LifetimeType lifetimeType = LifetimeType.Singleton)
		{
			switch (lifetimeType)
			{
				case LifetimeType.Transient:
					Container.Register(Component.For(serviceType).ImplementedBy(implementationType).LifestyleTransient());
					break;

				case LifetimeType.Singleton:
					Container.Register(Component.For(serviceType).ImplementedBy(implementationType).LifestyleSingleton());
					break;

				case LifetimeType.PerLifetimeScope:
					Container.Register(Component.For(serviceType).ImplementedBy(implementationType).LifestyleScoped());
					break;
			}
		}

		/// <summary>
		/// Registers the specified provider.
		/// </summary>
		/// <typeparam name="TService">Concrete type.</typeparam>
		/// <param name="instanceCreator">The instance creator.</param>
		/// <param name="lifetimeType">Lifetime type of the registering concrete type.</param>
		public void Register<TService>(Func<IDIContainerProvider, TService> instanceCreator, LifetimeType lifetimeType = LifetimeType.Singleton)
			where TService : class
		{
			switch (lifetimeType)
			{
				case LifetimeType.Transient:
					Container.Register(Component.For<TService>().UsingFactoryMethod(c => instanceCreator(this)).LifestyleTransient());
					break;

				case LifetimeType.Singleton:
					Container.Register(Component.For<TService>().UsingFactoryMethod(c => instanceCreator(this)).LifestyleSingleton());
					break;

				case LifetimeType.PerLifetimeScope:
					Container.Register(Component.For<TService>().UsingFactoryMethod(c => instanceCreator(this)).LifestyleScoped());
					break;
			}
		}

		/// <summary>
		/// Begins the lifetime scope.
		/// </summary>
		/// <returns></returns>
		public ILifetimeScope BeginLifetimeScope()
		{
			return new CastleWindsorLifetimeScope(this);
		}
	}
}