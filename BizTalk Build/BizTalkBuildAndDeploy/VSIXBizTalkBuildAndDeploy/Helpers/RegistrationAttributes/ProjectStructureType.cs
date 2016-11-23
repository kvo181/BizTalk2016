using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXBizTalkBuildAndDeploy.Helpers.RegistrationAttributes
{
    public sealed class ProjectStructureTypeAttribute : RegistrationAttribute
    {

        #region [ Local Fields ]

        private string _projectStructureType;

        #endregion

        #region [ Constructors ]

        public ProjectStructureTypeAttribute(string projectStructureType)
        {
            if (projectStructureType == null)
                throw new ArgumentNullException("projectStructureType");

            this._projectStructureType = projectStructureType;
        }

        #endregion

        #region [ Public Properties ]

        public string ProjectStructureType
        {
            get { return this._projectStructureType; }
        }

        #endregion

        #region [ Override Members ]

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            Key key = context.CreateKey("ProjectStructureType");
            key.SetValue(null, this._projectStructureType);
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey("ProjectStructureType");
        }

        #endregion

    }
}
