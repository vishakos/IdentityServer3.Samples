﻿/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using IdentityAdmin.Configuration;
using IdentityAdmin.Core;
using WebHost.IdSvr;
using IdentityManager.Configuration;
using IdentityServer3.Core.Configuration;
using WebHost.IdMgr;
using Owin;
using Serilog;
using IdentityManager.Logging;
using IdentityManager.Core.Logging;
using WebHost.Config;

namespace WebHost
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();


            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService("AspId");
            
                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory,
                   
                   
                });
            });

            app.Map("/core", core =>
            {
                var idSvrFactory = Factory.Configure();
                idSvrFactory.ConfigureUserService("AspId");

                var options = new IdentityServerOptions
                {
                    SiteName = "IdentityServer3 - AspNetIdentity",
                    SigningCertificate = Certificate.Get(),
                    Factory = idSvrFactory,
                    RequireSsl = false
                    
                };
               
                core.UseIdentityServer(options);
            });
            app.Map("/adm", adminApp =>
            {
                var factory = new IdentityAdminServiceFactory();
                factory.Configure();
                adminApp.UseIdentityAdmin(new IdentityAdminOptions
                {
                    Factory = factory
                });
            });

        }
    }
}