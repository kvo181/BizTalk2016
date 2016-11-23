using System.Collections.Generic;
using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    /// <summary>
    /// Rules consist out of an optional vocabulary and mandatory policy.
    /// </summary>
    [Serializable]
    public class Rules
    {
        public List<Vocabulary> Vocabularies { get; set; }
        public List<Policy> Policies { get; set; }

        /// <summary>
        /// List of unique vocabulary names (to remove)
        /// </summary>
        public List<string> VocabularieNames 
        {
            get
            {
                if (null == Vocabularies) return null;
                List<string> vocabularieNames = new List<string>();
                foreach (Vocabulary vocabulary in Vocabularies)
                {
                    if (!vocabularieNames.Contains(vocabulary.Name))
                        vocabularieNames.Add(vocabulary.Name);
                }
                return vocabularieNames;
            }
        }
        /// <summary>
        /// List of unique policy names (to undeploy and remove)
        /// </summary>
        public List<string> PolicyNames 
        {
            get
            {
                if (null == Policies) return null;
                List<string> policyNames = new List<string>();
                foreach (Policy policy in Policies)
                {
                    if (!policyNames.Contains(policy.Name))
                        policyNames.Add(policy.Name);
                }
                return policyNames;
            }
        }
    }
}