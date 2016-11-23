using System;
using System.IO;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Task to ackup the BRE
    /// </summary>
    public class BackupBRE : BaseBRETask 
    {
        /// <summary>
        /// Task execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (!Directory.Exists(FolderPath))
                throw new ApplicationException("The folder does not exist");

            var folder = Path.GetFullPath(FolderPath);
            Console.WriteLine("Backup Folder: " + folder);
            var filePath = folder + @"\" + Guid.NewGuid();
            var mgr = CreateManager();
            mgr.ExportAll(filePath);
            return true;
        }

        /// <summary>
        /// This is the folder where the backup will be created
        /// </summary>
        [Required]
        public string FolderPath { get; set; }
    }
}
