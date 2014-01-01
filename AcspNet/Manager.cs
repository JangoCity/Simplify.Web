using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

namespace AcspNet
{
	/// <summary>
	/// ACSP.NET main class
	/// </summary>
	public sealed class Manager
	{
		/// <summary>
		///  Gets the <see cref="T:System.Web.HttpContextBase"/> object for the current HTTP request.
		/// </summary>
		public readonly HttpContextBase Context;

		/// <summary>
		/// Gets the System.Web.HttpRequest object for the current HTTP request
		/// </summary>
		public readonly HttpRequestBase Request;

		/// <summary>
		/// Gets the System.Web.HttpResponse object for the current HTTP response
		/// </summary>
		public readonly HttpResponseBase Response;

		///// <summary>
		///// Gets the connection of  HTTP query string variables
		///// </summary>
		//public readonly NameValueCollection QueryString;

		///// <summary>
		///// Gets the System.Web.HttpSessionState object for the current HTTP request
		///// </summary>
		//public readonly HttpSessionState Session;

		///// <summary>
		///// Gets the connection of HTTP post request form variables
		///// </summary>
		//public readonly NameValueCollection Form;
		
		/// <summary>
		/// Gets the current aspx page.
		/// </summary>
		public readonly Page Page;

		/// <summary>
		/// The stop watch (for web-page build measurement)
		/// </summary>
		public readonly Stopwatch StopWatch;

		/// <summary>
		/// The file system instance, to work with System.IO functions
		/// </summary>
		public IFileSystem FileSystem;

		//private const string IsNewSessionFieldName = "AcspIsNewSession";

		//private static List<ExecExtensionMetaContainer> ExecExtensionsMetaContainers = new List<ExecExtensionMetaContainer>();
		//private static List<LibExtensionMetaContainer> LibExtensionsMetaContainers = new List<LibExtensionMetaContainer>();

		private static bool IsStaticInitialized;
		private static readonly object Locker = new object();

		private static readonly Lazy<AcspNetSettings> SettingsInstance = new Lazy<AcspNetSettings>(() => new AcspNetSettings());

		private static Lazy<string> SitePhysicalPathInstance;
		
		public readonly Environment Environment;
		public readonly ExtensionsDataLoader DataLoader;
		public readonly StringTable StringTable;
		public readonly TemplateFactory TemplateFactory;
		public readonly DataCollector DataCollector;
		
		//private string _currentAction;
		//private string _currentMode;
		//private string _currentID;

		//private IList<ExecExtension> _execExtensionsList;
		private bool _isExtensionsExecutionStopped;

		//private Dictionary<string, bool> _libExtensionsIsInitializedList;
		//private IList<LibExtension> _libExtensionsList;

		//////private static string SiteUrlInstance = "";

		/// <summary>
		///Initialize ACSP .NET engine instance
		/// </summary>
		/// <param name="page">The web-site default page.</param>
		public Manager(Page page)
			: this(page, new HttpContextWrapper(HttpContext.Current), new FileSystem())
		{
		}

		/// <summary>
		/// Initialize ACSP .NET engine instance
		/// </summary>
		/// <param name="page">The web-site default page.</param>
		/// <param name="httpContext">The HTTP context.</param>
		/// <param name="fileSystem">The file system.</param>
		/// <exception cref="System.ArgumentNullException">page
		/// or
		/// httpContext</exception>
		public Manager(Page page, HttpContextBase httpContext, IFileSystem fileSystem)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			if (httpContext == null)
				throw new ArgumentNullException("httpContext");

			if (fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			StopWatch = new Stopwatch();
			StopWatch.Start();

			Page = page;
			Context = httpContext;
			FileSystem = fileSystem;
			Request = Context.Request;
			Response = Context.Response;

			//QueryString = HttpContext.Current.Request.QueryString;
			//Session = HttpContext.Current.Session;
			//Form = HttpContext.Current.Request.Form;

			if (!IsStaticInitialized)
			{
				lock (Locker)
				{
					if (!IsStaticInitialized)
					{
						SitePhysicalPathInstance = new Lazy<string>(() => Request.PhysicalApplicationPath != null
							? Request.PhysicalApplicationPath.Replace("\\", "/")
							: null);

						RouteConfig.RegisterRoutes(RouteTable.Routes, Settings);

						//CreateMetaContainers(Assembly.GetCallingAssembly());
						IsStaticInitialized = true;
					}
				}
			}

			Environment = new Environment(this);
			DataLoader = new ExtensionsDataLoader(this);
			StringTable = new StringTable(this);
			TemplateFactory = new TemplateFactory(this);
			DataCollector = new DataCollector(this);
		}

		public AcspNetSettings Settings
		{
			get
			{				
				return SettingsInstance.Value;
			}
		}

		/// <summary>
		/// Gets the web-site physical path, for example: C:\inetpub\wwwroot\YourSite
		/// </summary>
		/// <value>
		/// The site physical path.
		/// </value>
		public string SitePhysicalPath
		{
			get
			{
				return SitePhysicalPathInstance.Value;
			}
		}

		///// <summary>
		/////     Gets the web-site URL, for example: http://yoursite.com/site1/
		///// </summary>
		///// <value>
		/////     The site URL.
		///// </value>
		//public static string SiteUrl
		//{
		//	get
		//	{
		//		if (SiteUrlInstance == "")
		//		{
		//			SiteUrlInstance = string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority,
		//				HttpContext.Current.Request.ApplicationPath);

		//			if (!SiteUrlInstance.EndsWith("/"))
		//				SiteUrlInstance += "/";
		//		}

		//		return SiteUrlInstance;
		//	}
		//}

		///// <summary>
		/////     Indicating whether session was created with the current request
		///// </summary>
		//public static bool IsNewSession
		//{
		//	get { return HttpContext.Current.Session[IsNewSessionFieldName] == null; }
		//}

		///// <summary>
		/////     Gets the current web-site request action parameter (/someAction or ?act=someAction).
		///// </summary>
		///// <value>
		/////     The current action (?act=someAction).
		///// </value>
		//public string CurrentAction
		//{
		//	get
		//	{
		//		if (_currentAction != null) return _currentAction;

		//		string action;

		//		if (_currentPage.RouteData.Values.ContainsKey("action"))
		//			action = (string) _currentPage.RouteData.Values["action"];
		//		else
		//			action = HttpContext.Current.Request.QueryString["act"];

		//		_currentAction = action ?? "";

		//		return _currentAction;
		//	}
		//}

		///// <summary>
		/////     Gets the current web-site mode request parameter (/someAction/someMode/SomeID or ?act=someAction&amp;mode=somMode).
		///// </summary>
		///// <value>
		/////     The current mode (?act=someAction&amp;mode=somMode).
		///// </value>
		//public string CurrentMode
		//{
		//	get
		//	{
		//		if (_currentMode != null) return _currentMode;

		//		string mode;

		//		if (_currentPage.RouteData.Values.ContainsKey("mode"))
		//			mode = (string)_currentPage.RouteData.Values["mode"];
		//		else
		//			mode = HttpContext.Current.Request.QueryString["mode"];

		//		_currentMode = mode ?? "";

		//		return _currentMode;
		//	}
		//}

		///// <summary>
		/////     Gets the current web-site ID request parameter (/someAction/someID or ?act=someAction&amp;id=someID).
		///// </summary>
		///// <value>
		/////     The current mode (?act=someAction&amp;mode=somMode).
		///// </value>
		//public string CurrentID
		//{
		//	get
		//	{
		//		if (_currentID != null) return _currentID;

		//		string id;

		//		if (_currentPage.RouteData.Values.ContainsKey("id"))
		//			id = (string)_currentPage.RouteData.Values["id"];
		//		else
		//			id = HttpContext.Current.Request.QueryString["id"];

		//		_currentID = id ?? "";

		//		return _currentID;
		//	}
		//}


		///// <summary>
		///// Gets the current executing extensions types.
		///// </summary>
		///// <value>
		///// The current executing extensions types.
		///// </value>
		//public IList<Type> ExecExtensionsTypes { get; private set; }

		/// <summary>
		/// Stop ACSP subsequent extensions execution
		/// </summary>
		public void StopExtensionsExecution()
		{
			_isExtensionsExecutionStopped = true;
		}

		//private static void CreateMetaContainers(Assembly callingAssembly)
		//{
		//	var assemblyTypes = callingAssembly.GetTypes();

		//	var containingClass = assemblyTypes.FirstOrDefault(t => t.IsDefined(typeof (LoadExtensionsFromAssemblyOfAttribute), true)) ??
		//						  assemblyTypes.FirstOrDefault(t => t.IsDefined(typeof (LoadIndividualExtensionsAttribute), true));

		//	if (containingClass == null)
		//		throw new AcspNetException("LoadExtensionsFromAssemblyOf attribute not found in your class");

		//	var batchExtensionsAttributes = containingClass.GetCustomAttributes(typeof (LoadExtensionsFromAssemblyOfAttribute), false);
		//	var individualExtensionsAttributes = containingClass.GetCustomAttributes(typeof (LoadIndividualExtensionsAttribute), false);

		//	if (batchExtensionsAttributes.Length <= 1 && individualExtensionsAttributes.Length <= 1)
		//	{
		//		if (batchExtensionsAttributes.Length == 1)
		//			LoadExtensionsFromAssemblyOf(((LoadExtensionsFromAssemblyOfAttribute) batchExtensionsAttributes[0]).Types);

		//		if (individualExtensionsAttributes.Length == 1)
		//			LoadIndividualExtensions(((LoadIndividualExtensionsAttribute)individualExtensionsAttributes[0]).Types);

		//		SortLibraryExtensionsMetaContainers();
		//		SortExecExtensionsMetaContainers();
		//	}
		//	else if (batchExtensionsAttributes.Length > 1)
		//		throw new Exception("Multiple LoadExtensionsFromAssemblyOf attributes found");
		//	else if (individualExtensionsAttributes.Length > 1)
		//		throw new Exception("Multiple LoadIndividualExtensions attributes found");
		//}

		//private static void LoadExtensionsFromAssemblyOf(params Type[] types)
		//{
		//	foreach (var assemblyTypes in types.Select(classType => Assembly.GetAssembly(classType).GetTypes()))
		//	{
		//		foreach (var t in assemblyTypes.Where(t => t.BaseType != null && t.BaseType.FullName == "AcspNet.LibExtension"))
		//			AddLibExtensionMetaContainer(t);

		//		foreach (var t in assemblyTypes.Where(t => t.BaseType != null && t.BaseType.FullName == "AcspNet.ExecExtension"))
		//			AddExecExtensionMetaContainer(t);
		//	}
		//}

		//private static void LoadIndividualExtensions(params Type[] types)
		//{
		//	foreach (var t in types.Where(t => t.BaseType != null && t.BaseType.FullName == "AcspNet.LibExtension").Where(t => LibExtensionsMetaContainers.All(x => x.ExtensionType != t)))
		//		AddLibExtensionMetaContainer(t);

		//	foreach (var t in types.Where(t => t.BaseType != null && t.BaseType.FullName == "AcspNet.ExecExtension").Where(t => ExecExtensionsMetaContainers.All(x => x.ExtensionType != t)))
		//		AddExecExtensionMetaContainer(t);
		//}

		//private static void AddLibExtensionMetaContainer(Type extensionType)
		//{
		//	LibExtensionsMetaContainers.Add(new LibExtensionMetaContainer(CreateExtensionMetaContainer(extensionType)));
		//}

		//private static void AddExecExtensionMetaContainer(Type extensionType)
		//{
		//	var action = "";
		//	var mode = "";
		//	var runType = RunType.OnAction;

		//	var attributes = extensionType.GetCustomAttributes(typeof (ActionAttribute), false);

		//	if (attributes.Length > 0)
		//		action = ((ActionAttribute) attributes[0]).Action;

		//	attributes = extensionType.GetCustomAttributes(typeof (ModeAttribute), false);

		//	if (attributes.Length > 0)
		//		mode = ((ModeAttribute) attributes[0]).Mode;

		//	attributes = extensionType.GetCustomAttributes(typeof (RunTypeAttribute), false);

		//	if (attributes.Length > 0)
		//		runType = ((RunTypeAttribute) attributes[0]).RunType;

		//	ExecExtensionsMetaContainers.Add(new ExecExtensionMetaContainer(CreateExtensionMetaContainer(extensionType), action, mode, runType));
		//}

		//private static ExtensionMetaContainer CreateExtensionMetaContainer(Type extensionType)
		//{
		//	var priority = 0;
		//	var version = "";

		//	var attributes = extensionType.GetCustomAttributes(typeof (PriorityAttribute), false);

		//	if (attributes.Length > 0)
		//		priority = ((PriorityAttribute) attributes[0]).Priority;

		//	attributes = extensionType.GetCustomAttributes(typeof (VersionAttribute), false);

		//	if (attributes.Length > 0)
		//		version = ((VersionAttribute) attributes[0]).Version;

		//	return new ExtensionMetaContainer(extensionType, priority, version);
		//}

		//private static void SortLibraryExtensionsMetaContainers()
		//{
		//	LibExtensionsMetaContainers = LibExtensionsMetaContainers.OrderBy(x => x.Priority).ToList();
		//}

		//private static void SortExecExtensionsMetaContainers()
		//{
		//	ExecExtensionsMetaContainers = ExecExtensionsMetaContainers.OrderBy(x => x.Priority).ToList();
		//}

		///// <summary>
		///// Run ACSP engine
		///// </summary>
		//public void Run()
		//{
			//	CreateLibraryExtensionsInstances();
			//	InitializeLibraryExtensions();

			//	CreateExecutableExtensionsInstances();
			//	RunExecutableExtensions();

			//	Session.Add(IsNewSessionFieldName, "true");
		//}

		//private void CreateLibraryExtensionsInstances()
		//{
		//	_libExtensionsList = new List<LibExtension>(LibExtensionsMetaContainers.Count);
		//	_libExtensionsIsInitializedList = new Dictionary<string, bool>(LibExtensionsMetaContainers.Count);

		//	foreach (var container in LibExtensionsMetaContainers)
		//	{
		//		var newInstance = (LibExtension) Activator.CreateInstance(container.ExtensionType);
		//		newInstance.ManagerInstance = this;

		//		_libExtensionsList.Add(newInstance);
		//		_libExtensionsIsInitializedList.Add(container.ExtensionType.Name, false);
		//	}
		//}

		//private void InitializeLibraryExtensions()
		//{
		//	foreach (var extension in _libExtensionsList)
		//	{
		//		extension.Initialize();
		//		_libExtensionsIsInitializedList[extension.GetType().Name] = true;
		//	}
		//}

		//private void CreateExecutableExtensionsInstances()
		//{
		//	_execExtensionsList = new List<ExecExtension>(ExecExtensionsMetaContainers.Count);
		//	ExecExtensionsTypes = new List<Type>(ExecExtensionsMetaContainers.Count);

		//	foreach (var container in ExecExtensionsMetaContainers)
		//	{
		//		if ((CurrentAction != "" || CurrentMode != "" || container.RunType != RunType.MainPage) &&
		//			(!String.Equals(container.Action, CurrentAction, StringComparison.CurrentCultureIgnoreCase) ||
		//			 !String.Equals(container.Mode, CurrentMode, StringComparison.CurrentCultureIgnoreCase)) &&
		//			(container.Action != "" || container.RunType != RunType.OnAction)) continue;

		//		var extension = (ExecExtension)Activator.CreateInstance(container.ExtensionType);
		//		extension.ManagerInstance = this;

		//		_execExtensionsList.Add(extension);
		//		ExecExtensionsTypes.Add(extension.GetType());
		//	}
		//}

		//private void RunExecutableExtensions()
		//{
		//	if (_execExtensionsList.Count <= 0) return;

		//	foreach (var extension in _execExtensionsList)
		//	{
		//		if (_isExtensionsExecutionStopped)
		//			return;

		//		extension.Invoke();
		//	}
		//}

		///// <summary>
		/////     Gets library extension instance
		///// </summary>
		///// <typeparam name="T">Library extension instance to get</typeparam>
		///// <returns>Library extension</returns>
		//public T Get<T>()
		//	where T : LibExtension
		//{
		//	foreach (var t in _libExtensionsList)
		//	{
		//		var currentType = t.GetType();

		//		if (currentType != typeof (T))
		//			continue;

		//		if (_libExtensionsIsInitializedList[currentType.Name] == false)
		//			throw new AcspNetException("Attempt to call not initialized library extension '" + t.GetType() + "'");

		//		return t as T;
		//	}

		//	throw new AcspNetException("Extension not found: " + typeof(T).FullName);
		//}

		///// <summary>
		///// Gets current action/mode URL in formal like ?act={0}&amp;mode={1}.
		///// </summary>
		///// <returns></returns>
		//public string GetActionModeUrl()
		//{
		//	return string.Format("?act={0}&amp;mode={1}", CurrentAction, CurrentMode);
		//}

		///// <summary>
		/////     Redirects a client to a new URL
		///// </summary>
		//public void Redirect(string url)
		//{
		//	StopExtensionsExecution();
		//	Response.Redirect(url, false);
		//}

		///// <summary>
		///// Get currently loaded executable extensions meta-data
		///// </summary>
		///// <returns></returns>
		//public static IList<ExecExtensionMetaContainer> GetExecExtensionsMetaData()
		//{
		//	return ExecExtensionsMetaContainers.ToArray();
		//}

		///// <summary>
		///// Gets the library extensions meta data.
		///// </summary>
		///// <returns></returns>
		//public static IList<LibExtensionMetaContainer> GetLibExtensionsMetaData()
		//{
		//	return LibExtensionsMetaContainers.ToArray();
		//}
	}
}