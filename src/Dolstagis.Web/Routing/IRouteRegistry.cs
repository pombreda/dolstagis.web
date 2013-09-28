﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolstagis.Web.Routing
{
    public interface IRouteRegistry
    {
        IList<IRouteDefinition> Routes { get; }

        bool Enabled { get; }
    }
}
