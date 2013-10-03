﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolstagis.Web.Lifecycle;

namespace Dolstagis.Web.Views.Static
{
    public class StaticResultProcessor : ResultProcessor<StaticResult>
    {
        private IList<ResourceLocator> _locators;
        private IMimeTypes _mimeTypes;
        private Application _application;

        public StaticResultProcessor(IEnumerable<Module> modules, IMimeTypes mimeTypes, Application application)
        {
            _locators = modules.Select(x => x.StaticFiles).Where(x => x != null).ToList();
            _mimeTypes = mimeTypes;
            _application = application;
        }

        public override async Task Process(StaticResult data, Http.IHttpContext context)
        {
            var resource = _locators.Select(x => x.Get(data.Path, _application.PhysicalPath))
                .Where(x => x != null).FirstOrDefault();
            if (resource == null) Status.NotFound.Throw();
            context.Response.Status = Status.OK;
            context.Response.AddHeader("Content-Type", _mimeTypes.GetMimeType(resource.Name));
            context.Response.AddHeader("Last-Modified", resource.LastModified.ToString("R"));
            if (resource.Length.HasValue) {
                context.Response.AddHeader("Content-Length", resource.Length.Value.ToString());
            }
            using (var stream = resource.Open()) {
                await stream.CopyToAsync(context.Response.ResponseStream);
            }
        }
    }
}
