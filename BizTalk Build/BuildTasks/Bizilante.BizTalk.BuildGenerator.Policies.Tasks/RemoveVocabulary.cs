using Microsoft.Build.Utilities;

namespace bizilante.BuildGenerator.Policies.Tasks
{
    public class RemoveVocabulary : Task
    {
        private string databaseName = string.Empty;
        private string serverName = string.Empty;
        private string vocabularyName;

        public override bool Execute()
        {
            VocabularyLoader.UnDeployVocabulary(this.vocabularyName, this.serverName, this.databaseName, base.Log);
            base.Log.LogMessage("", new object[] { });
            return true;
        }

        public string DatabaseName
        {
            get
            {
                return this.databaseName;
            }
            set
            {
                this.databaseName = value;
            }
        }

        public string ServerName
        {
            get
            {
                return this.serverName;
            }
            set
            {
                this.serverName = value;
            }
        }

        public string VocabularyName
        {
            get
            {
                return this.vocabularyName;
            }
            set
            {
                this.vocabularyName = value;
            }
        }
    }
}

