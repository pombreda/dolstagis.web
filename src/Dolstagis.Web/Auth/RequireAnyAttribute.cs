﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolstagis.Web.Auth
{
    /// <summary>
    ///  Requires the user to be in at least one of the specified roles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireAnyAttribute : Attribute, IRequirement
    {
        private string[] _requiredRoles;

        public RequireAnyAttribute(params string[] requiredRoles)
        {
            _requiredRoles = requiredRoles;
        }

        public bool IsDenied(RequestContext context)
        {
            return context.User == null ||
                !_requiredRoles.Any(x => context.User.IsInRole(x));
        }
    }
}
