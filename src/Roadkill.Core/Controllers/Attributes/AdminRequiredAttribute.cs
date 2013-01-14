﻿using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Roadkill.Core.Configuration;

namespace Roadkill.Core
{
	/// <summary>
	/// Represents an attribute that is used to restrict access by callers to users that are in Admin role group.
	/// </summary>
	public class AdminRequiredAttribute : AuthorizeAttribute
	{
		private IConfigurationContainer _config;
		private UserManager _userManager;

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			// Should refactor this so its in the IFilterProvider

			Roadkill.Core.Controllers.ControllerBase controller = filterContext.Controller as Roadkill.Core.Controllers.ControllerBase;
			if (controller != null)
			{
				_config = controller.Configuration;
				_userManager = controller.UserManager;
			}

			base.OnAuthorization(filterContext);
		}

		/// <summary>
		/// Provides an entry point for custom authorization checks.
		/// </summary>
		/// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
		/// <returns>
		/// true if the user is in the role name specified by the roadkill web.config adminRoleName setting or if this is blank; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="httpContext"/> parameter is null.</exception>
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			IPrincipal user = httpContext.User;
			IIdentity identity = user.Identity;

			if (!identity.IsAuthenticated)
			{
				return false;
			}

			if (string.IsNullOrEmpty(_config.ApplicationSettings.AdminRoleName))
				return true;

			if (_userManager.IsAdmin(identity.Name))
				return true;
			else
				return false;
		}
	}
}
