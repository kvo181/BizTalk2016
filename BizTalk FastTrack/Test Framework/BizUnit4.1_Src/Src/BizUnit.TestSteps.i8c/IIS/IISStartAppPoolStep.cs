﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.IIS
{
    /// <summary>
    /// The IISStartAppPoolStep allows us to Start an application pool.
    /// </summary>
    public class IisStartAppPoolStep : TestStepBase
    {
        ///<summary>
        /// Name of the WebServer 
        /// [Optional]: when empty we assume the local server.
        ///</summary>
        public string ServerName { get; set; }
        /// <summary>
        /// Username used to connect to the remote webserver
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password used to connect to the remote webserver
        /// </summary>
        public string Password { get; set; }
        ///<summary>
        /// Name of the application pool
        ///</summary>
        public string AppPoolName { get; set; }

        public override void Execute(Context context)
        {
            context.LogInfo("Start app pool: '{0}' on web server: '{1}'", AppPoolName, ServerName);

            ConnectionOptions options = null;
            ManagementScope scope = null;

            if (null != Username)
            {
                context.LogInfo("Connect with user: '{0}' and password: '{1}'", Username, Password);
                options = new ConnectionOptions()
                              {
                                  Username = Username,
                                  Password = Password,
                                  EnablePrivileges = true,
                                  Authentication = AuthenticationLevel.PacketPrivacy
                              };
            }
            else if (null != ServerName)
            {
                context.LogInfo("Connect with impersonate");
                options = new ConnectionOptions()
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true,
                    Authentication = AuthenticationLevel.PacketPrivacy
                };
            }

            var path = !string.IsNullOrEmpty(ServerName) ? 
                new ManagementPath(string.Format("\\\\{0}\\root\\WebAdministration:ApplicationPool.Name='{1}'", ServerName, AppPoolName)) : 
                new ManagementPath(string.Format("root\\WebAdministration:ApplicationPool.Name='{0}'", AppPoolName));

            context.LogInfo("Path = '{0}'", path.Path);

            if (null != options)
                scope = new ManagementScope(path, options);
            else
                scope = new ManagementScope(path);
            scope.Connect();

            int stateValue = -1;
            var opt = new ObjectGetOptions();
            var classInstance = new ManagementObject(scope, path, opt);
            var outState = classInstance.InvokeMethod("GetState", null).ToString();
            if (!int.TryParse(outState, out stateValue))
            {
                context.LogInfo("Invalid Application Pool State = '{0}'", outState);
                return;
            }
            context.LogInfo("Application Pool State = '{0}'", IISHelper.GetFriendlyApplicationPoolState(stateValue));
            switch (IISHelper.GetFriendlyApplicationPoolState(stateValue))
            {
                case "Stopped":
                    var outParams = classInstance.InvokeMethod("Start", null, null);
                    context.LogInfo("Application Pool has been started");
                    break;
                case "Started":
                    context.LogInfo("Application Pool already started");
                    break;
                default:
                    context.LogInfo("Application Pool could not be started");
                    break;
            }

        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(AppPoolName))
                throw new ArgumentNullException("AppPoolName is null or empty");
            if (!string.IsNullOrEmpty(Username) &&
                string.IsNullOrEmpty(Password))
                throw new ArgumentNullException("Password is null or empty");
        }
    }
}
