﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;

namespace Dolstagis.Web.Aspnet
{
    public class HttpRequestHandler : HttpTaskAsyncHandler
    {
        private static Application _application;

        public override bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            EnsureInit();
            var requestContext = new HttpRequestContext(new HttpContextWrapper(context));
            await _application.ProcessRequestAsync(requestContext);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void EnsureInit()
        {
            var ignores = new string[] {
                "Microsoft.",
                "mscorlib,", 
                "System.",
                "System,",
                "IronPython",
                "IronRuby",
                "CR_ExtUnitTest",
                "CR_VSTest",
                "DevExpress.CodeRush"
            };

            if (_application != null) return;
            _application = new Application();
            _application.Init();
            foreach (var assembly in BuildManager.GetReferencedAssemblies().Cast<Assembly>()) {
                if (!assembly.IsDynamic && !ignores.Any
                    (x => assembly.FullName.StartsWith(x, StringComparison.InvariantCulture))) {
                        _application.AddAllModulesInAssembly(assembly);
                }
            }
        }
    }
}
