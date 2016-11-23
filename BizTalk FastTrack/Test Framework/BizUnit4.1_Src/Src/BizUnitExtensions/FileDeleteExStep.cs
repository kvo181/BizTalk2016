//---------------------------------------------------------------------
// File: FileDeleteExStep.cs
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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.Extensions
{
    /// <summary>
    /// The FileDeleteStep deletes a FILE specified at a given location.
    /// The EXTENSION now includes the use of context in this step
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep AssemblyPath="BizUnit.Extensions.dll" TypeName="BizUnit.Extensions.FileDeleteExStep">
    ///		<FileToDeletePath takeFromCtx="c:\file1.xml"></FileToDeletePath>
    ///		<FileToDeletePath takeFromCtx="c:\file2.xml"></FileToDeletePath>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>FileToDeletePath</term>
    ///			<description>The location of FILE to be deleted<para>(one or more)</para></description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class FileDeleteExStep : TestStepBase
    {
        private string fileName;

        private List<string> filesToDelete = new List<string>();

        public System.Collections.Generic.List<string> FilesToDelete
        {
            get { return filesToDelete; }
            set { filesToDelete = value; }
        }

        public override void Execute(Context context)
        {

            Validate(context);
            try
            {
                foreach (string file in filesToDelete)
                {
                    fileName = file;
                    File.Delete(fileName);
                    context.LogInfo(String.Format("FileDeleteStep has deleted file: {0}", fileName));
                }
            }
            catch (Exception ex)
            {
                string myMsg = String.Format("There was a problem while trying to delete the file at {0}. This step requires an xml element or context called 'FileToDeletePath' - please check this value.", fileName);

                Exception myEx = new Exception(myMsg, ex);
                context.LogException(myEx);
                throw myEx;
            }


        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForNullReference(filesToDelete, "Files To Delete");
        }
    }
}
