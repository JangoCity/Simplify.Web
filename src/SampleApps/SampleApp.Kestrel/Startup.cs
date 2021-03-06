﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Simplify.Web.Owin;

namespace SampleApp.Kestrel
{
	public class Startup
	{
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSimplifyWeb();
		}
	}
}