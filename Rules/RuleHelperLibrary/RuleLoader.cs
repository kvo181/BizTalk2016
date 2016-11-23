using Microsoft.RuleEngine;
using System;
using System.Collections.Generic;

namespace bizilante.Rules.Helper
{
    public class RuleLoader
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

        public void DeployRuleSet(RuleSet ruleSet, string server, string database, bool deploy)
        {
            DoRuleEvent("DeployRuleSet", string.Format("Ready to {3} Ruleset {0} with version {1}.{2}.", new object[] { ruleSet.Name, ruleSet.CurrentVersion.MajorRevision, ruleSet.CurrentVersion.MinorRevision, deploy ? "deploy" : "publish" }));
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
                ruleStore.Add(ruleSet, true);
            }
            catch (RuleStoreRuleSetAlreadyPublishedException)
            {
                DoRuleEvent("DeployRuleSet", string.Format("Ruleset {0} is already published", new object[] { ruleSet.Name }), true);
            }
            catch
            {
                throw;
            }
            if (deploy)
            {
                try
                {
                    driver.Deploy(new RuleSetInfo(ruleSet.Name, ruleSet.CurrentVersion.MajorRevision, ruleSet.CurrentVersion.MinorRevision));
                }
                catch (RuleEngineDeploymentAlreadyDeployedException)
                {
                    DoRuleEvent("DeployRuleSet", string.Format("Ruleset {0} is already deployed.", new object[] { ruleSet.Name }), true);
                }
                catch
                {
                    throw;
                }
                DoRuleEvent("DeployRuleSet", string.Format("Deployed Ruleset {0} with version {1}.{2}.", new object[] { ruleSet.Name, ruleSet.CurrentVersion.MajorRevision, ruleSet.CurrentVersion.MinorRevision }));
            }
        }

        public RuleSet LoadRuleFromFile(string filename, string rulesetName)
        {
            RuleStore store = new FileRuleStore(filename);
            RuleSetInfoCollection ruleSets = store.GetRuleSets(rulesetName, RuleStore.Filter.Latest);
            if (ruleSets.Count != 1)
            {
                throw new ApplicationException(string.Format("EXCEPTION: No Ruleset named {0} exists in rule store {1}", rulesetName, filename));
            }
            return store.GetRuleSet(ruleSets[0]);
        }
        public List<RuleSetInfo> GetRuleSets()
        {
            List<RuleSetInfo> ruleSetList = new List<RuleSetInfo>();
            RuleSetDeploymentDriver driver = new RuleSetDeploymentDriver();
            try
            {
                RuleStore ruleStore = driver.GetRuleStore();
                RuleSetInfoCollection ruleSets = ruleStore.GetRuleSets(RuleStore.Filter.All);
                if (ruleSets.Count <= 0) return ruleSetList;
                for (int i = 0; i < ruleSets.Count; i++)
                    ruleSetList.Add(ruleSets[i]);
            }
            catch (RuleEngineConfigurationException confEx)
            {
                DoRuleEvent("GetRuleSets", string.Format("Rule engine configuration exception: {0}", confEx.Message), true);
            }
            catch (RuleEngineArgumentNullException nullEx)
            {
                DoRuleEvent("GetRuleSets", string.Format("Rule engine argument null exception: {0}", nullEx.Message), true);
            }
            return ruleSetList;
        }
        public void ExportRule(string filename, string rulesetName)
        {
            RuleSetDeploymentDriver driver = new RuleSetDeploymentDriver();
            try
            {
                RuleStore ruleStore = driver.GetRuleStore();
                RuleSetInfoCollection ruleSets = ruleStore.GetRuleSets(rulesetName, RuleStore.Filter.Latest);
                if (ruleSets.Count != 1)
                {
                    DoRuleEvent("ExportRule", string.Format("No Ruleset named {0} exists in rule store {1}", rulesetName, ruleStore.Location));
                    return;
                }
                driver.ExportRuleSetToFileRuleStore(ruleSets[0], filename);
            }
            catch (RuleEngineConfigurationException confEx)
            {
                DoRuleEvent("ExportRule", string.Format("Rule engine configuration exception: {0}", confEx.Message), true);
            }
            catch (RuleEngineArgumentNullException nullEx)
            {
                DoRuleEvent("ExportRule", string.Format("Rule engine argument null exception: {0}", nullEx.Message), true);
            }
            DoRuleEvent("ExportRule", string.Format("Rule {0} saved to {1}", rulesetName, filename));
        }

        public void UnDeployRuleSet(string ruleName, string server, string database)
        {
            DoRuleEvent("UnDeployRuleSet", string.Format("Ready to undeploy Rule name {0}.", new object[] { ruleName }));
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
            RuleSetInfoCollection ruleSets = ruleStore.GetRuleSets(ruleName, RuleStore.Filter.All);
            foreach (RuleSetInfo ruleSet in ruleSets)
                DoRuleEvent("UnDeployRuleSet", string.Format("Found Ruleset {0} with version {1}.{2}.", new object[] { ruleSet.Name, ruleSet.MajorRevision, ruleSet.MinorRevision }));
            DoRuleEvent("UnDeployRuleSet", string.Format("Start Undeploy...", new object[] { }));
            bool bCont = true;
            while (bCont && ruleSets.Count > 0)
            {
                try
                {
                    driver.Undeploy(ruleSets);
                    bCont = false;
                }
                catch (RuleEngineDeploymentNotDeployedException ex)
                {
                    DoRuleEvent("UnDeployRuleSet", string.Format("Ruleset {0} with version {1}.{2} was not deployed.", new object[] { ex.RuleSetName, ex.MajorVersion, ex.MinorVersion }), true);
                    // remove from ruleSets and try again
                    for (int i = 0; i < ruleSets.Count; i++)
                    {
                        if (ruleSets[i].Name.Equals(ex.RuleSetName) &&
                            ruleSets[i].MajorRevision == ex.MajorVersion &&
                            ruleSets[i].MinorRevision == ex.MinorVersion)
                        {
                            ruleSets.RemoveAt(i);
                            break;
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            // We only need to ones published now
            ruleSets = ruleStore.GetRuleSets(ruleName, RuleStore.Filter.Published);
            foreach (RuleSetInfo ruleSet in ruleSets)
                DoRuleEvent("UnDeployRuleSet", string.Format("Found Ruleset {0} with version {1}.{2} to be removed.", new object[] { ruleSet.Name, ruleSet.MajorRevision, ruleSet.MinorRevision }));
            if (ruleSets.Count > 0)
            {
                DoRuleEvent("UnDeployRuleSet", string.Format("Start Remove...", new object[] { }));
                try
                {
                    ruleStore.Remove(ruleSets);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}

