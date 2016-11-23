using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.RuleEngine;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Class containing methods for managing the BRE
    /// </summary>
    public class BREManager
    {
        private readonly RuleSetDeploymentDriver _driver;

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public BREManager()
        {
            _driver = new RuleSetDeploymentDriver();
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="databaseServer"></param>
        /// <param name="databaseName"></param>
        public BREManager(string databaseServer, string databaseName)
        {
            _driver = new RuleSetDeploymentDriver(databaseServer, databaseName);
        }
        #endregion

        /// <summary>
        /// Imports and publishes a vocabulary or policy file
        /// </summary>
        /// <param name="filePath"></param>
        public void Import(string filePath)
        {           			
            _driver.ImportAndPublishFileRuleStore(filePath);			
        }
        /// <summary>
        /// Imports all vocabulary and policy files
        /// </summary>
        /// <param name="filePaths"></param>
        public void Import(List<string> filePaths)
        {
            filePaths.ForEach(Import);
        }
        /// <summary>
        /// Exports all rules to a temporary location
        /// </summary>
        /// <param name="filePath"></param>
        public void ExportAll(string filePath)
        {
            var ruleStore = _driver.GetRuleStore();
            var exportRuleStore = new FileRuleStore(filePath);

            foreach(VocabularyInfo vocabularyInfo in ruleStore.GetVocabularies(RuleStore.Filter.All))
            {
                exportRuleStore.Add(ruleStore.GetVocabulary(vocabularyInfo));
            }
            foreach(RuleSetInfo ruleSetInfo in ruleStore.GetRuleSets(RuleStore.Filter.All))
            {
                exportRuleStore.Add(ruleStore.GetRuleSet(ruleSetInfo));
            }                
        }
        /// <summary>
        /// Removes all versions of a policy or vocabulary
        /// </summary>
        /// <param name="objectType">Should be RuleSetInfo or VocabularyInfo</param>
        /// <param name="name"></param>        
        public void Remove(RulesEngineObjectType objectType, string name)
        {
            var ruleStore = _driver.GetRuleStore();
            switch (objectType)
            {
                case RulesEngineObjectType.VocabularyInfo:
                    for (var index = 0; index < ruleStore.GetVocabularies(RuleStore.Filter.All).Count; index++)
                    {
                        var vocabularyInfo = ruleStore.GetVocabularies(RuleStore.Filter.All)[index];
                        if (vocabularyInfo.Name == name)
                            ruleStore.Remove(vocabularyInfo);
                    }
                    break;
                case RulesEngineObjectType.RuleSetInfo:
                    for (var index = 0; index < ruleStore.GetRuleSets(RuleStore.Filter.All).Count; index++)
                    {
                        var ruleSetInfo = ruleStore.GetRuleSets(RuleStore.Filter.All)[index];
                        if (ruleSetInfo.Name == name)
                            ruleStore.Remove(ruleSetInfo);
                    }
                    break;
                default:
                    throw new ApplicationException("Invalid object type");
            }
        }

        /// <summary>
        /// Removes a single entity from the rule store
        /// </summary>
        /// <param name="objectType">Should be RuleSetInfo or VocabularyInfo</param>
        /// <param name="name"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        public void Remove(RulesEngineObjectType objectType, string name, int major, int minor)
        {
            var sqlRuleStore = _driver.GetRuleStore();
            switch (objectType)
            {
                case RulesEngineObjectType.VocabularyInfo:
                    sqlRuleStore.Remove(new VocabularyInfo(name, major, minor));
                    break;
                case RulesEngineObjectType.RuleSetInfo:
                    sqlRuleStore.Remove(new RuleSetInfo(name, major, minor));
                    break;
                default:
                    throw new ApplicationException("Invalid object type");
            }
        }

        /// <summary>
        /// Clears the business rules store
        /// </summary>
        public void RemoveAll()
        {
            var ruleStore = _driver.GetRuleStore();
            foreach (RuleSetInfo ruleSetInfo in ruleStore.GetRuleSets(RuleStore.Filter.All))
            {
                ruleStore.Remove(ruleSetInfo);
            }
            foreach(VocabularyInfo vocabulary in ruleStore.GetVocabularies(RuleStore.Filter.All))
            {
                ruleStore.Remove(vocabulary);
            }
        }
        /// <summary>
        /// Undeploys a RuleSet
        /// </summary>
        /// <param name="name"></param>        
        public void UndeployRuleSet(string name)
        {
            var ruleStore = _driver.GetRuleStore();
            foreach (var ruleSetInfo in
                ruleStore.GetRuleSets(RuleStore.Filter.All).Cast<RuleSetInfo>().Where(ruleSetInfo => ruleSetInfo.Name == name))
            {
                _driver.Undeploy(ruleSetInfo);
            }
        }

        /// <summary>
        /// Undeploys a RuleSet
        /// </summary>
        /// <param name="name"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        public void UndeployRuleSet(string name, int major, int minor)
        {            
            _driver.Undeploy(new RuleSetInfo(name, major, minor));            
        }
        /// <summary>
        /// Deploys a rule set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        public void DeployRuleSet(string name, int major, int minor)
        {            
            _driver.Deploy(new RuleSetInfo(name, major, minor));
        }
        /// <summary>
        /// Deploys all versions of a rules policy
        /// </summary>
        /// <param name="name"></param>
        public void DeployRuleSet(string name)
        {
            var ruleStore = _driver.GetRuleStore();
            foreach (var ruleSetInfo in
                ruleStore.GetRuleSets(RuleStore.Filter.All).Cast<RuleSetInfo>().Where(ruleSetInfo => ruleSetInfo.Name == name))
            {
                DeployRuleSet(ruleSetInfo.Name, ruleSetInfo.MajorRevision, ruleSetInfo.MinorRevision);
            }
        }
    }
    /// <summary>
    /// Type of object to be removed from the rules engine
    /// </summary>
    public enum RulesEngineObjectType
    {
        VocabularyInfo,
        RuleSetInfo
    }
}

