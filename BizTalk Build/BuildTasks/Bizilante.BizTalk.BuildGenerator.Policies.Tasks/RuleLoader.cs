using Microsoft.Build.Utilities;
using Microsoft.RuleEngine;
using System;

namespace bizilante.BuildGenerator.Policies.Tasks
{
    public static class RuleLoader
    {
        public static void DeployRuleSet(RuleSet ruleSet, string server, string database, TaskLoggingHelper log, bool deploy)
        {
            log.LogMessage("Ready to {3} Ruleset {0} with version {1}.{2}.", new object[] { ruleSet.Name, ruleSet.CurrentVersion.MajorRevision, ruleSet.CurrentVersion.MinorRevision, deploy ? "deploy" : "publish" });
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
                log.LogMessage("Ruleset {0} is already published", new object[] { ruleSet.Name });
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
                    log.LogMessage("Ruleset {0} is already deployed.", new object[] { ruleSet.Name });
                }
                catch
                {
                    throw;
                }
                log.LogMessage("Deployed Ruleset {0} with version {1}.{2}.", new object[] { ruleSet.Name, ruleSet.CurrentVersion.MajorRevision, ruleSet.CurrentVersion.MinorRevision });
            }
        }

        public static RuleSet LoadRuleFromFile(string filename, string rulesetName)
        {
            RuleStore store = new FileRuleStore(filename);
            RuleSetInfoCollection ruleSets = store.GetRuleSets(rulesetName, RuleStore.Filter.Latest);
            if (ruleSets.Count != 1)
            {
                throw new ApplicationException(string.Format("EXCEPTION: No Ruleset named {0} exists in rule store {1}", rulesetName, filename));
            }
            return store.GetRuleSet(ruleSets[0]);
        }

        public static void UnDeployRuleSet(string ruleName, string server, string database, TaskLoggingHelper log)
        {
            log.LogMessage("Ready to undeploy Rule name {0}.", new object[] { ruleName });
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
                log.LogMessage("Found Ruleset {0} with version {1}.{2}.", new object[] { ruleSet.Name, ruleSet.MajorRevision, ruleSet.MinorRevision });
            log.LogMessage("Start Undeploy...", new object[] { });
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
                    log.LogMessage("Ruleset {0} with version {1}.{2} was not deployed.", new object[] { ex.RuleSetName, ex.MajorVersion, ex.MinorVersion });
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
                log.LogMessage("Found Ruleset {0} with version {1}.{2} to be removed.", new object[] { ruleSet.Name, ruleSet.MajorRevision, ruleSet.MinorRevision });
            if (ruleSets.Count > 0)
            {
                log.LogMessage("Start Remove...", new object[] { });
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

