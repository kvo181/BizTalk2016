using System.Collections.Generic;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk
{
    /// <summary>
    /// Used to sort the application resources to have the correct build order
    /// </summary>
    public class ApplicationResourceSorter
    {
        /// <summary>
        /// Sorts the resources
        /// </summary>
        /// <param name="resources"></param>
        public static void Sort(List<MetaDataBuildGenerator.ApplicationResource> resources)                       
        {
            Queue<MetaDataBuildGenerator.ApplicationResource> tempResources = new Queue<MetaDataBuildGenerator.ApplicationResource>();

            foreach (MetaDataBuildGenerator.ApplicationResource resource in resources)
            {
                if (resource.DependantResources.Count == 0)
                    tempResources.Enqueue(resource);
            }

            foreach (MetaDataBuildGenerator.ApplicationResource resource in resources)
            {
                AddResource(resource, resources, tempResources);
            }

            resources.Clear();
            MetaDataBuildGenerator.ApplicationResource[] resourceArray = tempResources.ToArray();
            for (int i = resourceArray.Length - 1; i >= 0; i--)
                resources.Add(resourceArray[i]);

            // Second pass: first assemblies then biztalk assemblies
            List<MetaDataBuildGenerator.ApplicationResource> nonBiztalkAssemblies =
                resources.FindAll(delegate(MetaDataBuildGenerator.ApplicationResource r) { return r.Type != MetaDataBuildGenerator.MetaData.ResourceTypes.BizTalkAssembly; });
            List<MetaDataBuildGenerator.ApplicationResource> biztalkAssemblies =
                resources.FindAll(delegate(MetaDataBuildGenerator.ApplicationResource r) { return r.Type == MetaDataBuildGenerator.MetaData.ResourceTypes.BizTalkAssembly; });

            resources.Clear();
            resources.AddRange(nonBiztalkAssemblies);
            resources.AddRange(biztalkAssemblies);
        }
        /// <summary>
        /// Adds resources 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="resources"></param>
        /// <param name="tempResources"></param>
        private static void AddResource(
            MetaDataBuildGenerator.ApplicationResource resource, 
            List<MetaDataBuildGenerator.ApplicationResource> resources, 
            Queue<MetaDataBuildGenerator.ApplicationResource> tempResources)
        {
            if (tempResources.Contains(resource))
                return;

            foreach (MetaDataBuildGenerator.ApplicationResource dependancy in resource.DependantResources)            
                AddResource(dependancy, resources, tempResources);            

            tempResources.Enqueue(resource);
        }
    }
}
