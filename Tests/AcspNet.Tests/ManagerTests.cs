﻿//namespace AcspNet.Tests
//{
//	[TestFixture]
//	[LoadExtensionsFromAssemblyOf(typeof(ManagerTests))]
//	[LoadIndividualExtensions(typeof(ExternalExecExtensionTest), typeof(ExternalLibExtensionTest))]
//	public class ManagerTests
//	{
//		private Mock<HttpResponseBase> _httpResponse;

//		public Mock<HttpContextBase> GetTestHttpContext()
//		{
//			var context = new HttpContext(new HttpRequest("Foo", "http://localhost", ""), new HttpResponse(new StringWriter()));

//			var httpContext = new Mock<HttpContextBase>();
//			var httpRequest = new Mock<HttpRequestBase>();
//			_httpResponse = new Mock<HttpResponseBase>();
//			var httpSession = new Mock<HttpSessionStateBase>();
//			var cookieCollection = new HttpCookieCollection();

//			httpContext.SetupGet(r => r.Request).Returns(httpRequest.Object);
//			httpContext.SetupGet(r => r.Response).Returns(_httpResponse.Object);
//			httpContext.SetupGet(r => r.Session).Returns(httpSession.Object);

//			httpRequest.SetupGet(r => r.Cookies).Returns(cookieCollection);
//			httpRequest.SetupGet(r => r.QueryString).Returns(new NameValueCollection());
//			httpRequest.SetupGet(r => r.Form).Returns(new NameValueCollection());
//			httpRequest.SetupGet(r => r.PhysicalApplicationPath).Returns(@"C:\WebSites\FooSite\");
//			httpRequest.SetupGet(r => r.Url).Returns(new Uri("http://localhost"));
//			httpRequest.SetupGet(r => r.RawUrl).Returns("http://localhost/FooSite/");
//			httpRequest.SetupGet(r => r.ApplicationPath).Returns("/FooSite");

//			_httpResponse.SetupGet(r => r.Cookies).Returns(cookieCollection);
//			_httpResponse.SetupGet(r => r.Cache).Returns(new HttpCachePolicyWrapper(context.Response.Cache));

//			var sessions = new Dictionary<string, object>();
//			httpSession.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<object>()))
//				.Callback((string key, object value) =>
//						  {
//							  if (!sessions.ContainsKey(key))
//								  sessions.Add(key, value);
//						  });

//			httpSession.Setup(x => x[It.IsAny<string>()])
//				.Returns((string key) => sessions.ContainsKey(key) ? sessions[key] : null);
//			httpSession.Setup(x => x.Remove(It.IsAny<string>())).Callback((string key) => sessions.Remove(key));

//			return httpContext;
//		}

//		public IFileSystem GetTestFileSystem()
//		{
//			var files = new Dictionary<string, MockFileData>();

//			files.Add("ExtensionsData/Bar.en.xml",
//				"<?xml version=\"1.0\" encoding=\"utf-8\" ?><items><item name=\"SiteTitle\" value=\"Your site title!\" /></items>");
//			files.Add("ExtensionsData/Bar.ru.txt", "Hello text!");
//			files.Add("ExtensionsData/Empty.en.txt", "");
//			files.Add("ExtensionsData/BarDefault.en.txt", "Hello default!");

//			files.Add("ExtensionsData/StringTable.en.xml",
//				"<?xml version=\"1.0\" encoding=\"utf-8\" ?><items><item name=\"SiteTitle\" value=\"Your site title!\" /><item name=\"InfoTitle\" value=\"Information!\" /><item name=\"FooEnumFooItem1\" value=\"Foo item text\" /><item name=\"HtmlListDefaultItemLabel\" value=\"Default label\" /><item name=\"NotifyPageDataError\" value=\"Page data error!\" /></items>");
//			files.Add("ExtensionsData/StringTable.ru.xml",
//				"<?xml version=\"1.0\" encoding=\"utf-8\" ?><items><item name=\"SiteTitle\" value=\"Заголовок сайта!\" /></items>");

//			files.Add("Templates/Foo.tpl", "Hello world!!!");

//			files.Add("Templates/Index.tpl", Template.FromManifest("TestData.Index.tpl").Get());

//			files.Add("Templates/AcspNet/MessageBox/OkMessageBox.tpl", "{Title}{Message}");
//			files.Add("Templates/AcspNet/MessageBox/ErrorMessageBox.tpl", "{Title}{Message}");
//			files.Add("Templates/AcspNet/MessageBox/InfoMessageBox.tpl", "{Title}{Message}");
//			files.Add("Templates/AcspNet/MessageBox/InlineInfoMessageBox.tpl", "{Message}");
//			files.Add("Templates/AcspNet/MessageBox/InlineErrorMessageBox.tpl", "{Message}");
//			files.Add("Templates/AcspNet/MessageBox/InlineOkMessageBox.tpl", "{Message}");

//			return new MockFileSystem(files, "C:/WebSites/FooSite");
//		}

//		public IFileSystem GetTestFileSystemForAsyncTesting()
//		{
//			var files = new Dictionary<string, MockFileData>();

//			for (var i = 0; i < 1000; i++)
//				files.Add("Templates/Async" + i + ".tpl",
//					"<?xml version=\"1.0\" encoding=\"utf-8\" ?><items><item name=\"SiteTitle\" value=\"Your site title!\" /></items>");

//			return new MockFileSystem(files, "C:/WebSites/FooSite");
//		}

//		public Assembly GetTestUserAssembly()
//		{
//			return Assembly.GetCallingAssembly();
//		}

//		public Manager GetTestManager(string action = null, string mode = null, string id = null)
//		{
//			var httpContext = GetTestHttpContext();
//			var routeData = AcspNetTestingHelper.GetTestRouteData();
//			var fs = GetTestFileSystem();

//			if (action != null)
//				httpContext.Object.Request.QueryString.Add("act", action);

//			if (mode != null)
//				httpContext.Object.Request.QueryString.Add("mode", mode);

//			if (id != null)
//				httpContext.Object.Request.QueryString.Add("id", id);

//			Template.FileSystem = fs;

//			return new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), GetTestUserAssembly());
//		}

//		[Test]
//		public void Manager_Initialize_ExceptionsThrownCorrectly()
//		{
//			Assert.Throws<ArgumentNullException>(() => new Manager(null));
//			Assert.Throws<ArgumentNullException>(() => new Manager(null, null, null, null, null));
//			Assert.Throws<ArgumentNullException>(() => new Manager(AcspNetTestingHelper.GetTestRouteData(), null, null, null, null));
//			Assert.Throws<ArgumentNullException>(
//				() => new Manager(AcspNetTestingHelper.GetTestRouteData(), GetTestHttpContext().Object, null, null, null));
//			Assert.Throws<ArgumentNullException>(
//				() => new Manager(AcspNetTestingHelper.GetTestRouteData(), GetTestHttpContext().Object, GetTestFileSystem(), null, null));
//			Assert.Throws<ArgumentNullException>(
//				() => new Manager(AcspNetTestingHelper.GetTestRouteData(), GetTestHttpContext().Object, GetTestFileSystem(), AcspNetTestingHelper.GetTestHttpRuntime(), null));
//		}

//		[Test]
//		public void Manager_Initialize_ParametersInitializedCorrectly()
//		{
//			var manager = GetTestManager();

//			Assert.IsNotNull(manager.Context);
//			Assert.IsNotNull(manager.Request);
//			Assert.IsNotNull(manager.RouteData);
//			Assert.IsNotNull(manager.Response);
//			Assert.IsNotNull(manager.Session);
//			Assert.IsNotNull(manager.QueryString);
//			Assert.IsNotNull(manager.Form);
//			Assert.IsNotNull(manager.StopWatch);
//			Assert.IsNotNull(Manager.Settings);
//			Assert.IsNotNull(manager.Environment);
//			Assert.IsNotNull(manager.StringTable);
//			Assert.IsNotNull(manager.DataCollector);
//			Assert.IsNotNull(manager.HtmlWrapper);
//			Assert.IsNotNull(manager.HtmlWrapper.ListsGenerator);
//			Assert.IsNotNull(manager.HtmlWrapper.MessageBox);
//			Assert.IsNotNull(manager.AuthenticationModule);
//			Assert.IsNotNull(manager.ExtensionsWrapper);
//			Assert.IsNotNull(manager.ExtensionsWrapper.MessagePage);
//			Assert.IsNotNull(manager.ExtensionsWrapper.IdProcessor);
//			Assert.IsNotNull(manager.ExtensionsWrapper.Navigator);
//			Assert.AreEqual("http://localhost/FooSite/", Manager.SiteUrl);
//			Assert.IsNotNull(manager.CurrentAction);
//			Assert.IsNotNull(manager.CurrentMode);
//		}

//		[Test]
//		public void TemplateFactory_Usage_BehaviourIsCorrect()
//		{
//			var manager = GetTestManager();

//			Assert.Throws<ArgumentNullException>(() => manager.TemplateFactory.Load(null));
//			Assert.Throws<TemplateException>(() => manager.TemplateFactory.Load("Not"));

//			var tpl = manager.TemplateFactory.Load("Foo.tpl");

//			Assert.IsNotNull(tpl);
//			Assert.AreEqual("Hello world!!!", tpl.Get());

//			manager.Environment.TemplatesMemoryCache = true;

//			Assert.IsNotNull(tpl);
//			Assert.AreEqual("Hello world!!!", manager.TemplateFactory.Load("Foo.tpl").Get());

//			Assert.IsNotNull(tpl);
//			Assert.AreEqual("Hello world!!!", manager.TemplateFactory.Load("Foo.tpl").Get());

//			// Test async operations

//			var fs = GetTestFileSystemForAsyncTesting();
//			Template.FileSystem = fs;
//			var manager1 = new Manager(AcspNetTestingHelper.GetTestRouteData(), GetTestHttpContext().Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(),
//				GetTestUserAssembly());
//			var manager2 = new Manager(AcspNetTestingHelper.GetTestRouteData(), GetTestHttpContext().Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(),
//				GetTestUserAssembly());

//			ThreadStart first = () => manager1.TemplateFactory.Load("Async1.tpl");
//			ThreadStart second = () => manager2.TemplateFactory.Load("Async1.tpl");

//			first.BeginInvoke(null, null);
//			second.BeginInvoke(null, null);
//		}

//		[Test]
//		public void MainPage_Execution_BehaviourIsCorrect()
//		{
//			RouteConfig.RegisterRoutes();
//			var manager = GetTestManager();

//			Assert.IsTrue(manager.IsNewSession);

//			manager.Run();

//			Assert.IsFalse(manager.IsNewSession);
//		}

//		[Test]
//		public void HtmlListsGenerator_Usage_BehaviourIsCorrect()
//		{
//			var manager = GetTestManager("htmlListsTest");
//			manager.Run();
//		}

//		[Test]
//		public void MessageBox_Usage_BehaviourIsCorrect()
//		{
//			var manager = GetTestManager("messageBoxTests");
//			manager.Run();
//		}

//		[Test]
//		public void AuthenticationModule_Usage_BehaviourIsCorrect()
//		{
//			var manager = GetTestManager("authenticationModuleTests");
//			manager.Run();
//		}

//		[Test]
//		public void Manager_Usage_StopExtensionsExecutionIsCorrect()
//		{
//			var manager = GetTestManager("stopExtensionsExecution");
//			manager.Run();
//		}

//		[Test]
//		public void Manager_Usage_ActionModeIdUsageIsCorrect()
//		{
//			var manager = GetTestManager("foo", "bar", "15");
//			manager.Run();
//		}

//		[Test]
//		public void Manager_Usage_ExternalLibraryExtensionUsageIsCorrect()
//		{
//			var manager = GetTestManager("foo");
//			manager.Run();
//		}

//		[Test]
//		public void Manager_Usage_GetExtensionsMetadataIsCorrect()
//		{
//			var manager = GetTestManager("getExtensionsMetadataTest");
//			manager.Run();
//		}

//		[Test]
//		public void Manager_Redirect_BehaviourIsCorrect()
//		{
//			var manager = GetTestManager();
//			manager.Run();

//			Assert.Throws<ArgumentNullException>(() => manager.Redirect(""));

//			manager.Redirect("http://localhost");
//		}

//		[Test]
//		public void Manager_Usage_MessagePageBehaviourIsCorrect()
//		{
//			var routeData = AcspNetTestingHelper.GetTestRouteData();
//			var httpContext = GetTestHttpContext();
//			var fs = GetTestFileSystem();
//			var userAssembly = GetTestUserAssembly();
//			Template.FileSystem = fs;

//			var manager = new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), userAssembly);
//			manager.Run();

//			manager.ExtensionsWrapper.MessagePage.Message = "Hello!";
//			manager.ExtensionsWrapper.MessagePage.NavigateToMessagePage();

//			httpContext.Object.Request.QueryString.Add("act", "message");
//			manager = new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), userAssembly);
//			manager.Run();

//			manager.ExtensionsWrapper.MessagePage.NavigateToMessagePage("Hello!");
//			manager = new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), userAssembly);
//			manager.Run();

//			manager.Run();
//		}

//		[Test]
//		public void Manager_Usage_ExtensionsProtectorBehaviourIsCorrect()
//		{
//			var manager = GetTestManager("extensionsProtectorTest");
//			manager.Run();
//		}

//		[Test]
//		public void MainPage_Execution_IdProcessorAjaxBehaviourIsCorrect()
//		{
//			var routeData = AcspNetTestingHelper.GetTestRouteData();
//			var httpContext = GetTestHttpContext();
//			var fs = GetTestFileSystem();
//			var userAssembly = GetTestUserAssembly();
//			Template.FileSystem = fs;

//			var manager = new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), userAssembly);
//			manager.Run();

//			Assert.IsNull(manager.ExtensionsWrapper.IdProcessor.CheckAndGetQueryStringIdAjax());

//			_httpResponse.Verify(x => x.Write(It.IsAny<string>()), Times.Exactly(2));

//			httpContext.Object.Request.QueryString.Add("id", "asd");
//			Assert.IsNull(manager.ExtensionsWrapper.IdProcessor.CheckAndGetQueryStringIdAjax());

//			_httpResponse.Verify(x => x.Write(It.IsAny<string>()), Times.Exactly(3));

//			httpContext.Object.Request.QueryString["id"] = "26";
//			Assert.AreEqual(26, manager.ExtensionsWrapper.IdProcessor.CheckAndGetQueryStringIdAjax());

//			_httpResponse.Verify(x => x.Write(It.IsAny<string>()), Times.Exactly(3));
//		}

//		[Test]
//		public void MainPage_Execution_IdProcessorBehaviourIsCorrect()
//		{
//			var routeData = AcspNetTestingHelper.GetTestRouteData();
//			var httpContext = GetTestHttpContext();
//			var fs = GetTestFileSystem();
//			var userAssembly = GetTestUserAssembly();
//			Template.FileSystem = fs;

//			var manager = new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), userAssembly);
//			manager.Run();

//			Assert.IsNull(manager.ExtensionsWrapper.IdProcessor.CheckAndGetQueryStringID());

//			httpContext.Object.Request.QueryString.Add("id", "asd");
//			Assert.IsNull(manager.ExtensionsWrapper.IdProcessor.CheckAndGetQueryStringID());

//			httpContext.Object.Request.QueryString["id"] = "26";
//			Assert.AreEqual(26, manager.ExtensionsWrapper.IdProcessor.CheckAndGetQueryStringID());
//		}

//		[Test]
//		public void MainPage_Execution_IdProcessorFormBehaviourIsCorrect()
//		{
//			var routeData = AcspNetTestingHelper.GetTestRouteData();
//			var httpContext = GetTestHttpContext();
//			var fs = GetTestFileSystem();
//			var userAssembly = GetTestUserAssembly();
//			Template.FileSystem = fs;

//			var manager = new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), userAssembly);
//			manager.Run();

//			Assert.IsNull(manager.ExtensionsWrapper.IdProcessor.CheckAndGetFormID());

//			httpContext.Object.Request.Form.Add("id", "asd");
//			Assert.IsNull(manager.ExtensionsWrapper.IdProcessor.CheckAndGetFormID());

//			httpContext.Object.Request.Form["id"] = "26";
//			Assert.AreEqual(26, manager.ExtensionsWrapper.IdProcessor.CheckAndGetFormID());
//		}

//		[Test]
//		public void Manager_Execution_IsRouteDataParsingCorrect()
//		{
//			var routeData = AcspNetTestingHelper.GetTestRouteData();
//			var httpContext = GetTestHttpContext();
//			var fs = GetTestFileSystem();
//			var userAssembly = GetTestUserAssembly();
//			Template.FileSystem = fs;

//			routeData.Values.Add("action", "routeAction");
//			routeData.Values.Add("mode", "routeMode");
//			routeData.Values.Add("id", "routeID");

//			var manager = new Manager(routeData, httpContext.Object, fs, AcspNetTestingHelper.GetTestHttpRuntime(), userAssembly);

//			Assert.AreEqual("routeAction", manager.CurrentAction);
//			Assert.AreEqual("routeMode", manager.CurrentMode);
//			Assert.AreEqual("routeID", manager.CurrentID);
//		}

//		[Test]
//		public void Navigator_NavigateToPreviosPage_IsCorrect()
//		{
//			var manager = GetTestManager("navigatorTest");

//			manager.ExtensionsWrapper.Navigator.NavigateToPreviousPage();

//			Assert.IsNotNull(manager.ExtensionsWrapper.Navigator.PreviousNavigatedUrl);
//		}

//		[Test]
//		public void Navigator_NavigateToPreviosPageWithBookmark_IsCorrect()
//		{
//			var manager = GetTestManager("navigatorTest");

//			manager.ExtensionsWrapper.Navigator.NavigateToPreviousPageWithBookmark("foo");
//		}

//		[Test]
//		public void Navigator_NavigateToRedirectLink_IsCorrect()
//		{
//			var manager = GetTestManager("navigatorTest");

//			manager.ExtensionsWrapper.Navigator.RedirectLink = "foo";
//			manager.ExtensionsWrapper.Navigator.NavigateToRedirectLink();
//		}

//		[Test]
//		public void Navigator_SetRedirectLinkToCurrentPage_IsCorrect()
//		{
//			var manager = GetTestManager("navigatorTest");

//			manager.ExtensionsWrapper.Navigator.SetRedirectLinkToCurrentPage();
//		}
//	}
//}
