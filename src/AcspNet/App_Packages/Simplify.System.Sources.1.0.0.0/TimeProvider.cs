﻿using System;

namespace Simplify.Web.App_Packages.Simplify.System.Sources._1._0._0._0
{
	/// <summary>
	/// Provides Time ambient context
	/// </summary>
	internal static class TimeProvider
	{
		private static ITimeProvider _currentInstance;

		/// <summary>
		/// Gets or sets the current time provider.
		/// </summary>
		/// <value>
		/// The current time provider.
		/// </value>
		/// <exception cref="ArgumentNullException">value</exception>
		public static ITimeProvider Current
		{
			get
			{
				return _currentInstance ?? (_currentInstance = new SystemTimeProvider());
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_currentInstance = value;
			}
		}
	}
}
