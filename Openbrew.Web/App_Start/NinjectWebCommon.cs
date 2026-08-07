using System.Reflection;
using System.IO;
using System.Linq;
using Openbrew.Web.Controllers;
using ctorx.Core.Mapping;
using ctorx.Core.Ninject;
using Openbrew.Web.Core.Data;
using FluentValidation.Mvc;
using StackExchange.Exceptional;
using StackExchange.Exceptional.Stores;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Openbrew.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Openbrew.Web.App_Start.NinjectWebCommon), "Stop")]

namespace Openbrew.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MvcHandler.DisableMvcResponseHeader = true;

            FluentValidationModelValidatorProvider.Configure(provider =>
            {
                provider.ValidatorFactory = new ViewModelValidatorFactory();
            });

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new CaseInsensitiveRazorViewEngine());

            Database.SetInitializer<BrewgrContext>(null);
            var connectionString = Environment.GetEnvironmentVariable("OPENBREW_CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("Brewgr_ConnectionString");
            ErrorStore.Setup("Brewgr.com", new SQLErrorStore(connectionString));

            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

	            KernelPersister.Set(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
			kernel.Load(new Dependencies());
			kernel.Load(new AutoMapperModule(Assembly.GetAssembly(typeof(BrewgrController))));


        }
    }

    internal sealed class CaseInsensitiveRazorViewEngine : RazorViewEngine
    {
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            if (base.FileExists(controllerContext, virtualPath))
            {
                return true;
            }

            var physicalPath = controllerContext.HttpContext.Server.MapPath(virtualPath);
            return this.ResolveCaseInsensitivePath(physicalPath) != null;
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return base.CreateView(controllerContext, this.CanonicalizeVirtualPath(controllerContext, viewPath), this.CanonicalizeVirtualPath(controllerContext, masterPath));
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return base.CreatePartialView(controllerContext, this.CanonicalizeVirtualPath(controllerContext, partialPath));
        }

        string CanonicalizeVirtualPath(ControllerContext controllerContext, string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
            {
                return virtualPath;
            }

            var physicalPath = controllerContext.HttpContext.Server.MapPath(virtualPath);
            var resolvedPhysicalPath = this.ResolveCaseInsensitivePath(physicalPath);
            if (resolvedPhysicalPath == null)
            {
                return virtualPath;
            }

            var appRoot = Path.GetFullPath(controllerContext.HttpContext.Server.MapPath("~/")).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var fullResolved = Path.GetFullPath(resolvedPhysicalPath);
            if (!fullResolved.StartsWith(appRoot, StringComparison.OrdinalIgnoreCase))
            {
                return virtualPath;
            }

            var relative = fullResolved.Substring(appRoot.Length).Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
            return "~/" + relative;
        }

        string ResolveCaseInsensitivePath(string physicalPath)
        {
            if (File.Exists(physicalPath) || Directory.Exists(physicalPath))
            {
                return physicalPath;
            }

            var root = Path.GetPathRoot(physicalPath);
            if (string.IsNullOrWhiteSpace(root))
            {
                root = physicalPath.StartsWith(Path.DirectorySeparatorChar.ToString()) ? Path.DirectorySeparatorChar.ToString() : string.Empty;
            }

            var current = root;
            var remaining = physicalPath.Substring(root.Length).Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in remaining)
            {
                if (!Directory.Exists(current))
                {
                    return null;
                }

                var match = Directory.EnumerateFileSystemEntries(current)
                    .FirstOrDefault(entry => string.Equals(Path.GetFileName(entry), part, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    return null;
                }

                current = match;
            }

            return File.Exists(current) || Directory.Exists(current) ? current : null;
        }
    }
}
