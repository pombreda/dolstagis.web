﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolstagis.Web.Logging
{
    public interface ILoggingProvider
    {
        Logger CreateLogger(Type type);
    }
}
