//---------------------------------------------------------------------
// File: WaitOnFileExStep.cs
// 
// Summary: 
//
// Copyright (c) http://bizunitextensions.codeplex.com. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Xml;
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.Extensions
{
    /// <summary>
    /// The WaitOnFileExStep is used to wait for a FILE to be written to a given location.
    /// It uses the file system watcher. It replaces the old step which used to simply delay
    /// for the given interval.Now when the file is detected the system returns immediately.
    /// It also loads the file name(both the full name and the simple name) into the context 
    /// (with the keynames DetectedFileFullName and DetectedFileName) and allows you to specify whether you want 
    /// to consider previously existing files which is useful in scenarios where the testing is
    /// intensive and the system races ahead of the test and places the file in the location and
    /// the step executes after the file is already available.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="BizUnit.Extensions.dll" TypeName="BizUnit.Extensions.WaitOnFileExStep">
    ///		<Path>n:\\</Path>
    ///		<FileFilter>*.xml</FileFilter>
    ///		<TimeOut>10000</TimeOut>
    ///		<IncludeOldFiles>Y</IncludeOldFiles>
    ///		<Delete>Y</Delete>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Path</term>
    ///			<description>The directory to look for the FILE</description>
    ///		</item>
    ///		<item>
    ///			<term>FileFilter</term>
    ///			<description>The FILE mask to be used to search for a FILE, e.g. *.xml</description>
    ///		</item>
    ///		<item>
    ///			<term>TimeOut</term>
    ///			<description>The time to wait for the FILE to become present in miliseconds</description>
    ///		</item>
    ///		<item>
    ///			<term>Delete</term>
    ///			<description>Do you want to delete the file when checked (Y/N)</description>
    ///		</item>
    ///		<item>
    ///		<term>IncludeOldFiles</term>
    ///		<description>should we take into account files already existing</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class WaitOnFileExStep : TestStepBase
    {
        ManualResetEvent mre;
        private string newFilePath;
        private string newFileName;

        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        private string fileFilter;

        public string FileFilter
        {
            get { return fileFilter; }
            set { fileFilter = value; }
        }
        private int timeOut;

        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }
        private string deleteFileIfFound;

        public string DeleteFileIfFound
        {
            get { return deleteFileIfFound; }
            set { deleteFileIfFound = value; }
        }
        private string includeOldFiles;

        public string IncludeOldFiles
        {
            get { return includeOldFiles; }
            set { includeOldFiles = value; }
        }

        #region "private methods"
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            newFilePath = e.FullPath;
            newFileName = e.Name;
            mre.Set();
        }

        #endregion

        public override void Execute(Context context)
        {
            if ((Directory.GetFiles(Path).Length > 0) && (IncludeOldFiles == "Y"))
            {
                // Set newFilename and newFilePath with the earliest file
                string[] items = Directory.GetFileSystemEntries(Path, "*.xml");
                foreach (string item in items)
                {
                    context.Add("DetectedFileFullName", item, true);
                    context.Add("DetectedFileName", System.IO.Path.GetFileName(item), true);
                    break;
                }

                return;
            }
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path;
            watcher.Filter = FileFilter;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = false;
            watcher.Changed += new FileSystemEventHandler(OnCreated);
            mre = new ManualResetEvent(false);

            if (!mre.WaitOne(TimeOut, false))
            {
                throw new Exception(string.Format("WaitOnFileStep timed out after {0} milisecs watching path:{1}, filter{2}", TimeOut, Path, FileFilter));
            }
            else
            {
                context.LogInfo(string.Format("WaitOnFileStep found the file: {0}", newFilePath));
                context.Add("DetectedFile", newFilePath, true);
                context.Add("DetectedFileName", newFileName, true);

            }
            if (DeleteFileIfFound == "Y")
            {
                Thread.Sleep(500); // Wait for file to be closed by creator
                File.Delete(newFilePath);
                context.LogWarning("FILE {0} HAS BEEN DELETED", newFilePath);
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(path, "Path");
            ArgumentValidation.CheckForEmptyString(fileFilter, "File Filter");
            ArgumentValidation.CheckForEmptyString(TimeOut.ToString(), "TimeOut");
            ArgumentValidation.CheckForEmptyString(includeOldFiles, "Include Old Files");
        }
   }
}
