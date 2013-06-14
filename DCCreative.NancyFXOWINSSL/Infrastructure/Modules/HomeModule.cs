using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Security;

namespace DCCreative.NancyFXOWINSSL.Infrastructure.Modules {
    class HomeModule : NancyModule {
        public HomeModule() {
            this.RequiresHttps(true, 9443);
            Get["/"] = parameters => {
                return View["index"];
            };
        }
    }
}
