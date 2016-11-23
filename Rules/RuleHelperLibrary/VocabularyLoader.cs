using Microsoft.RuleEngine;
using System;
using System.Collections.Generic;

namespace bizilante.Rules.Helper
{
    public class VocabularyLoader
    {
        public EventHandler<RuleEventArgs> RuleEvent;

        private void DoRuleEvent(string source, string message)
        {
            DoRuleEvent(source, message, false);
        }
        private void DoRuleEvent(string source, string message, bool isError)
        {
            if (null == RuleEvent) return;
            RuleEventArgs args = new RuleEventArgs(source, message, isError);
            RuleEvent(this, args);
        }

        public void DeployVocabulary(Vocabulary[] vocabularies, string server, string database)
        {
            for (int i = 0; i < vocabularies.Length; i++)
            {
                DoRuleEvent("DeployVocabulary", string.Format("Ready to deploy Vocabulary {0} with version {1}.{2}.", new object[] { vocabularies[i].Name, vocabularies[i].CurrentVersion.MajorRevision, vocabularies[i].CurrentVersion.MinorRevision }));
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
                DoRuleEvent("DeployVocabulary", string.Format("Vocabulary {0} already exists.", new object[] { exception.VocabularyName }), true);
            }
            catch
            {
                throw;
            }
            for (int i = 0; i < vocabularies.Length; i++)
            {
                DoRuleEvent("DeployVocabulary", string.Format("Deployed Vocabulary {0} with version {1}.{2}.", new object[] { vocabularies[i].Name, vocabularies[i].CurrentVersion.MajorRevision, vocabularies[i].CurrentVersion.MinorRevision }));
            }
        }

        public Vocabulary[] LoadVocabFromFile(string filename, string vocabName)
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
        public List<VocabularyInfo> GetVocabularies()
        {
            List<VocabularyInfo> vocabularyList = new List<VocabularyInfo>();
            RuleSetDeploymentDriver driver = new RuleSetDeploymentDriver();
            try
            {
                RuleStore ruleStore = driver.GetRuleStore();
                VocabularyInfoCollection vocabularies = ruleStore.GetVocabularies(RuleStore.Filter.All);
                if (vocabularies.Count < 1) return vocabularyList;
                for (int i = 0; i < vocabularies.Count; i++)
                    vocabularyList.Add(vocabularies[i]);
            }
            catch (RuleEngineConfigurationException confEx)
            {
                DoRuleEvent("GetVocabularies", string.Format("Rule engine configuration exception: {0}", confEx.Message), true);
            }
            catch (RuleEngineArgumentNullException nullEx)
            {
                DoRuleEvent("GetVocabularies", string.Format("Rule engine argument null exception: {0}", nullEx.Message), true);
            }
            return vocabularyList;
        }
        public void ExportVocabulary(string filename, string vocabName)
        {
            RuleSetDeploymentDriver driver = new RuleSetDeploymentDriver();
            try
            {
                RuleStore ruleStore = driver.GetRuleStore();
                VocabularyInfoCollection vocabularies = ruleStore.GetVocabularies(vocabName, RuleStore.Filter.All);
                if (vocabularies.Count < 1)
                {
                    DoRuleEvent("ExportVocabulary", string.Format("No Vocabulary named {0} exists in rule store {1}", vocabName, ruleStore.Location));
                    return;
                }
                driver.ExportVocabularyToFileRuleStore(vocabularies[0], filename);
            }
            catch (RuleEngineConfigurationException confEx)
            {
                DoRuleEvent("ExportVocabulary", string.Format("Rule engine configuration exception: {0}", confEx.Message), true);
            }
            catch (RuleEngineArgumentNullException nullEx)
            {
                DoRuleEvent("ExportVocabulary", string.Format("Rule engine argument null exception: {0}", nullEx.Message), true);
            }
            DoRuleEvent("ExportVocabulary", string.Format("Vocabulary {0} saved to {1}", vocabName, filename));
        }

        public void UnDeployVocabulary(string vocabName, string serverName, string databaseName)
        {
            DoRuleEvent("UnDeployVocabulary", string.Format("Ready to undeploy Vocabulary name {0}.", new object[] { vocabName }));
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
                DoRuleEvent("UnDeployVocabulary", string.Format("Found vocabulary {0} with version {1}.{2}.", new object[] { vocabulary.Name, vocabulary.MajorRevision, vocabulary.MinorRevision }));
            DoRuleEvent("UnDeployVocabulary", string.Format("Start Undeploy...", new object[] { }));
            try
            {
                ruleStore.Remove(vocabularies);
            }
            catch
            {
                throw;
            }
            DoRuleEvent("UnDeployVocabulary", string.Format("Vocabularies with name {0} removed.", new object[] { vocabName }));
        }
    }
}

