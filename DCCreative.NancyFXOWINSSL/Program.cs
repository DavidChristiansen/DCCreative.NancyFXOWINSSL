using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCCreative.NancyFXOWINSSL.Infrastructure;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Services;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using Owin;

namespace DCCreative.NancyFXOWINSSL {
    class Program {
        static void Main(string[] args) {
            var serviceInitialiser = new ServiceInitialiser();
            serviceInitialiser.Init();
        }
    }

    internal class ServiceInitialiser {
        private MyApplication _application;

        public void Init() {
            _application = new MyApplication();
            IServiceProvider serviceProvider = DefaultServices.Create(defaultServiceProvider => defaultServiceProvider.AddInstance<MyApplication>(this._application));
            using (WebApplication.Start<Startup>(serviceProvider)) {
                Console.WriteLine("Running on http://127.0.0.1:9100 and http://127.0.0.1:9443");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }

    internal class Startup {
        private readonly MyApplication _application;

        public Startup(MyApplication application) {
            _application = application;
        }

        public void Configuration(IAppBuilder builder) {
            builder.UseNancy(_application.BootStrapper);
            var hostAddresses = new List<IDictionary<string, object>>();
            Dictionary<string, object> httpHostAddress = new Dictionary<string, object>() {
                {"scheme", "http"},
                {"host", "+"},
                {"port", "9100"},
                {"path", string.Empty},
            };
            hostAddresses.Add(httpHostAddress);
            Dictionary<string, object> httpsHostAddress = new Dictionary<string, object>() {
                {"scheme", "https"},
                {"host", "+"},
                {"port", "9443"},
                {"path", string.Empty},
            };
            hostAddresses.Add(httpsHostAddress);
            builder.Properties["host.Addresses"] = hostAddresses;
        }
    }

    internal class MyApplication {
        private INancyBootstrapper _bootStrapper;

        public MyApplication() {
            _bootStrapper = new MyApplicationBootStrapper();
        }

        public INancyBootstrapper BootStrapper {
            get { return _bootStrapper; }
            set { _bootStrapper = value; }
        }
    }

    internal class MyApplicationBootStrapper : DefaultNancyBootstrapper {

        public MyApplicationBootStrapper() {
            var assembly = this.GetType().Assembly;
            ResourceViewLocationProvider.RootNamespaces.Add(assembly, "DCCreative.NancyFXOWINSSL.Web.Views");
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration {
            get { return new DiagnosticsConfiguration { Password = @"P@ssw0rd" }; }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            base.ApplicationStartup(container, pipelines);
            var assembly = this.GetType().Assembly;
            this.Conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("Images", assembly, "Web/images"));
            this.Conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("js", assembly, "Web/js"));
            this.Conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("CSS", assembly, "Web/css"));
            this.Conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("Views", assembly, "Web/Views"));
        }
       
        protected override NancyInternalConfiguration InternalConfiguration {
            get {
                return NancyInternalConfiguration.WithOverrides(this.OnConfigurationBuilder);
            }
        }
        protected override IEnumerable<ModuleRegistration> Modules {
            get {
                return GetType().Assembly.GetTypes().Where(type => type.BaseType == typeof(NancyModule)).Select(type => new ModuleRegistration(type));
            }
        }

        private void OnConfigurationBuilder(NancyInternalConfiguration x) {
            x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
        }
    }
}
