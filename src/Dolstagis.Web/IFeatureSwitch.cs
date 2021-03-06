﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolstagis.Web.Http;

namespace Dolstagis.Web
{
    /// <summary>
    ///  Indicates whether a feature is enabled or disabled for a particular
    ///  request.
    /// </summary>

    public interface IFeatureSwitch
    {
        /// <summary>
        ///  Queries the given request to determine whether or not this feature
        ///  is enabled for this request.
        /// </summary>
        /// <param name="request">
        ///  The current request.
        /// </param>
        /// <returns>
        ///  true if the feature is enabled for this request, otherwise false.
        /// </returns>

        Task<bool> IsEnabledForRequest(IRequest request);
    }
}
