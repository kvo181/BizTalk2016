using Microsoft.Build.Utilities;
using Microsoft.RuleEngine;
using System;

namespace bizilante.BuildGenerator.Policies.Tasks
{
    public static class VocabularyLoader
    {
        public static void DeployVocabulary(Vocabulary[] vocabularies, string server, string database, TaskLoggingHelper log)
        {
            for (int i = 0; i < vocabularies.Length; i++)
            {
                log.LogMessage("Ready to deploy Vocabulary {0} with version {1}.{2}.", new object[] { vocabularies[i].Name, vocabularies[i].CurrentVersion.MajorRevision, vocabularies[i].CurrentVersion.MinorRevision });
            }
            Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver driver;
            if ((server != string.Empty) && (database != string.Empty))
            {
                driver = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver(server, database);
            }
            else
            {
                driver = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver();
            }
            RuleStore ruleStore = driver.GetRuleStore();
            try
            {
                for (int i = 0; i < vocabularies.Length; i++)
                {
                    ruleStore.Add(vocabularies[i], true);
                }
            }
            catch (RuleStoreVocabularyAlreadyPublishedException exception)
            {
                log.LogMessage("Vocabulary {0} already exists.", new object[] { exception.VocabularyName });
            }
            catch
            {
                throw;
            }
            for (int i = 0; i < vocabularies.Length; i++)
            {
                log.LogMessage("Deployed Vocabulary {0} with version {1}.{2}.", new object[] { vocabularies[i].Name, vocabularies[i].CurrentVersion.MajorRevision, vocabularies[i].CurrentVersion.MinorRevision });
            }
        }

        public static Vocabulary[] LoadVocabFromFile(string filename, string vocabName)
        {
            RuleStore store = new FileRuleStore(filename);
            VocabularyInfoCollection vocabularies = store.GetVocabularies(vocabName, RuleStore.Filter.All);
            if (vocabularies.Count < 1)
            {
                throw new ApplicationException(string.Format("EXCEPTION: No Vocabulary named {0} exists in rule store {1}", vocabName, filename));
            }
            Vocabulary[] vocabularyArray = new Vocabulary[vocabularies.Count];
            for (int i = 0; i < vocabularies.Count; i++)
            {
                vocabularyArray[i] = store.GetVocabulary(vocabularies[i]);
            }
            return vocabularyArray;
        }

        public static void UnDeployVocabulary(string vocabName, string serverName, string databaseName, TaskLoggingHelper log)
        {
            log.LogMessage("Ready to undeploy Vocabulary name {0}.", new object[] { vocabName });
            Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver driver;
            if ((databaseName != string.Empty) && (serverName != string.Empty))
            {
                driver = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver(serverName, databaseName);
            }
            else
            {
                driver = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver();
            }
            RuleStore ruleStore = driver.GetRuleStore();
            VocabularyInfoCollection vocabularies = ruleStore.GetVocabularies(vocabName, RuleStore.Filter.All);
            foreach (VocabularyInfo vocabulary in vocabularies)
                log.LogMessage("Found vocabulary {0} with version {1}.{2}.", new object[] { vocabulary.Name, vocabulary.MajorRevision, vocabulary.MinorRevision });
            log.LogMessage("Start Undeploy...", new object[] { });
            try
            {
                ruleStore.Remove(vocabularies);
            }
            catch
            {
                throw;
            }
            log.LogMessage("Vocabularies with name {0} removed.", new object[] { vocabName });
        }
    }
}

