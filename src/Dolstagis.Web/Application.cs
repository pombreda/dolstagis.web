﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dolstagis.Web.Http;
using Dolstagis.Web.Owin;

namespace Dolstagis.Web
{
    public class Application
    {
        private IList<Feature> _features = new List<Feature>();
        private Lazy<ApplicationContext> _context;
        private ISet<Assembly> _loadedAssemblies = new HashSet<Assembly>();

        public string VirtualPath { get; private set; }
        public string PhysicalPath { get; private set; }
        public ISettings Settings { get; private set; }
        public IDictionary<string, object> Items { get; private set; }

        public Application(string virtualPath, string physicalPath, ISettings settings)
        {
            _context = new Lazy<ApplicationContext>(CreateContext);
            VirtualPath = virtualPath;
            PhysicalPath = physicalPath;
            Settings = settings;
            Items = new Dictionary<string, object>();
            AddAllFeaturesInAssembly(this.GetType().Assembly);
        }

        private ApplicationContext CreateContext()
        {
            return new ApplicationContext(this, _features.Where(x => x.Enabled));
        }


        /// <summary>
        ///  Registers a feature with the application by type.
        /// </summary>
        /// <typeparam name="T">
        ///  They type of feature to register.
        /// </typeparam>

        public void AddFeature<T>() where T : Feature, new()
        {
            AddFeature(new T());
        }

        /// <summary>
        ///  Registers a feature with the application by instance.
        /// </summary>
        /// <param name="feature">
        ///  The feature to register.
        /// </param>

        public void AddFeature(Feature feature)
        {
            _features.Add(feature);
        }

        /// <summary>
        ///  Scan an assembly for features to add.
        /// </summary>
        /// <param name="assembly">
        ///  The assembly.
        /// </param>
        /// <remarks>
        ///  Only features with a public default constructor will be instantiated.
        ///  The order in which they are added is non-deterministic.
        /// </remarks>

        public void AddAllFeaturesInAssembly(Assembly assembly)
        {
            if (_loadedAssemblies.Contains(assembly)) return;
            _loadedAssemblies.Add(assembly);

            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            foreach (var type in types.Where(t => typeof(Feature).IsAssignableFrom(t)))
            {
                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    var feature = constructor.Invoke(null) as Feature;
                    AddFeature(feature);
                }
            }
        }


        /// <summary>
        ///  Processes a request synchronously.
        /// </summary>
        /// <param name="context">
        ///  The <see cref="IHttpContext"/> containing request and response objects.
        /// </param>

        public void ProcessRequest(IRequest request, IResponse response)
        {
            ProcessRequestAsync(request, response).Wait();
        }

        /// <summary>
        ///  Processes a request asynchronously.
        /// </summary>
        /// <param name="context">
        ///  The <see cref="IHttpContext"/> containing request and response objects.
        /// </param>
        /// <returns>
        ///  A <see cref="Task"/> instance.
        /// </returns>

        public async Task ProcessRequestAsync(IRequest request, IResponse response)
        {
            await _context.Value.ProcessRequestAsync(request, response);
        }

        /// <summary>
        ///  Gets the Owin AppFunc for this request.
        /// </summary>
        /// <returns></returns>

        public Func<IDictionary<string, object>, Task> GetAppFunc()
        {
            return async environment => {
                var request = new Request(environment);
                var response = new Response(environment);
                await ProcessRequestAsync(request, response);
            };
        }
    }
}
