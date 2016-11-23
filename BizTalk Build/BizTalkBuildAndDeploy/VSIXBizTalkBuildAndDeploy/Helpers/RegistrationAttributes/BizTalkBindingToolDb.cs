using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXBizTalkBuildAndDeploy.Helpers.RegistrationAttributes
{
    public sealed class BizTalkBindingToolDbAttribute : RegistrationAttribute
    {

        #region [ Local Fields ]

        private bool _generateBindings;
        private string _bizTalkBindingToolDbServer;
        private string _bizTalkBindingToolDbDatabase;

        #endregion

        #region [ Constructors ]

        public BizTalkBindingToolDbAttribute(bool generateBindings, string bizTalkBindingToolDbServer, string bizTalkBindingToolDbDatabase)
        {
            if (generateBindings)
            {
                if (bizTalkBindingToolDbServer == null)
                    throw new ArgumentNullException("bizTalkBindingToolDbServer");
                if (bizTalkBindingToolDbDatabase == null)
                    throw new ArgumentNullException("bizTalkBindingToolDbDatabase");
            }

            this._generateBindings = generateBindings;
            this._bizTalkBindingToolDbServer = bizTalkBindingToolDbServer;
            this._bizTalkBindingToolDbDatabase = bizTalkBindingToolDbDatabase;
        }

        #endregion

        #region [ Public Properties ]

        public bool GenerateBindings
        {
            get { return this._generateBindings; }
        }
        public string BizTalkBindingToolDbServer
        {
            get { return this._bizTalkBindingToolDbServer; }
        }
        public string BizTalkBindingToolDbDatabase
        {
            get { return this._bizTalkBindingToolDbDatabase; }
        }

        #endregion

        #region [ Override Members ]

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            Key key = context.CreateKey("BizTalkBindingToolDb");
            key.SetValue("GenerateBindings", this._generateBindings);
            key.SetValue("Server", this._bizTalkBindingToolDbServer);
            key.SetValue("Database", this._bizTalkBindingToolDbDatabase);
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey("BizTalkBindingToolDb");
        }

        #endregion

    }
}
