﻿using System;
using System.Linq;
using Dolstagis.Web.Auth;
using Dolstagis.Web.Lifecycle;
using Dolstagis.Web.ModelBinding;
using Dolstagis.Web.Sessions;
using Dolstagis.Web.Static;
using Dolstagis.Web.Views;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace Dolstagis.Web
{
    internal class CoreServices : Registry
    {
        public CoreServices()
        {
            For<IMimeTypes>().Singleton().Use<MimeTypes>();

            For<IRequestContextBuilder>().Use<RequestContextBuilder>();
            For<IRequestProcessor>().Use<RequestProcessor>();
            For<IExceptionHandler>().Use<ExceptionHandler>();
            For<ISessionStore>().Singleton().Use<InMemorySessionStore>();
            For<IAuthenticator>().Singleton().Use<SessionAuthenticator>();
            For<ILoginHandler>().Use<LoginHandler>();
            For<IModelBinder>().Singleton().Use<DefaultModelBinder>();

            For<IResultProcessor>().Singleton().Add<StaticResultProcessor>()
                .Ctor<IResourceResolver>().Is(ctx => new ResourceResolver
                    (ResourceType.StaticFiles, ctx.GetAllInstances<ResourceMapping>())
                );
            For<IResultProcessor>().Singleton().Add<ViewResultProcessor>();
            For<IResultProcessor>().Singleton().Add<JsonResultProcessor>();
            For<IResultProcessor>().Singleton().Add<ContentResultProcessor>();
            For<IResultProcessor>().Singleton().Add<HeadResultProcessor>();

            For<ViewRegistry>().Singleton().Use<ViewRegistry>()
                .Ctor<IResourceResolver>().Is(ctx => new ResourceResolver
                    (ResourceType.Views, ctx.GetAllInstances<ResourceMapping>())
                );

            this.Scan(x =>
            {
                x.AssemblyContainingType<IConverter>();
                x.With(new SingletonConvention<IConverter>());
                x.AddAllTypesOf<IConverter>();
            });
        }

        private class SingletonConvention<TPluginFamily> : IRegistrationConvention
        {
            public void Process(Type type, Registry registry)
            {
                if (!type.IsConcrete() || !type.CanBeCreated() ||
                    !type.AllInterfaces().Contains(typeof(TPluginFamily))) return;

                registry.For(typeof(TPluginFamily)).Singleton().Use(type);
            }
        }
    }
}
