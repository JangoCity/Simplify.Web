﻿namespace AcspNet.DI
{
	/// <summary>
	/// Provides DI container provider resolve extensions
	/// </summary>
	public static class DIContainerProviderResolveExtensions
	{
		/// <summary>
		/// Resolves the specifed type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="provider">The DI provider.</param>
		/// <returns></returns>
		public static object Resolve<T>(this IDIContainerProvider provider)
		{
			return provider.Resolve(typeof (T));
		}
	}
}