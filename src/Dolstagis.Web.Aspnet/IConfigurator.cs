﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolstagis.Web.Aspnet
{
    public interface IConfigurator
    {
        void Configure(Application application);
    }
}
