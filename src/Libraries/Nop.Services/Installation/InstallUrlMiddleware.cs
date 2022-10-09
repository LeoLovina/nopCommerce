using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;
using Nop.Services.Security;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Represents middleware that checks whether database is installed and redirects to installation URL in otherwise
    /// </summary>
    public class InstallUrlMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public InstallUrlMiddleware(RequestDelegate next, IPermissionService permissionService)
        {
            _next = next;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context, IWebHelper webHelper)
        {
            //whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
            {
                var installUrl = $"{webHelper.GetStoreLocation()}{NopInstallationDefaults.InstallPath}";
                if (!webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    //redirect
                    context.Response.Redirect(installUrl);
                    return;
                }
            }

            // Add new permission
            //var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
            //foreach (var providerType in permissionProviders)
            //{
            //    var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
            //    await _permissionService.InstallPermissionsAsync(provider);
            //}

            //or call the next middleware in the request pipeline
            await _next(context);
        }

        #endregion
    }
}