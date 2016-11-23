using Microsoft.VisualStudio.Shell;
using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.RegistrationAttributes
{
    public sealed class TasksPathAttribute : RegistrationAttribute
    {

        #region [ Local Fields ]

        private string _tasksPath;

        #endregion

        #region [ Constructors ]

        public TasksPathAttribute(string tasksPath)
        {
            if (tasksPath == null)
                throw new ArgumentNullException("tasksPath");

            this._tasksPath = tasksPath;
        }

        #endregion

        #region [ Public Properties ]

        public string TasksPath
        {
            get { return this._tasksPath; }
        }

        #endregion

        #region [ Override Members ]

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            Key key = context.CreateKey("TasksPath");
            key.SetValue(null, this._tasksPath);
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey("TasksPath");
        }

        #endregion

    }
}
