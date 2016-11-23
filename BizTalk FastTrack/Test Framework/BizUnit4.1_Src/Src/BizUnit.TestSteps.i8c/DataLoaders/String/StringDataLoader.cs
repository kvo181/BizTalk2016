//---------------------------------------------------------------------
// File: FileDataLoader.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2016, bizilante. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System.IO;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.DataLoaders.String
{
    /// <summary>
    /// The StringDataLoader maybe used to load a string and pass to a test 
    /// step or sub-step which accepts a dataloader. Test steps which use data loaders 
    /// benefit from increased flexibility around how they load data by de-coupling 
    /// the test step from how it loads its data.
    /// </summary>
    public class StringDataLoader : DataLoaderBase
    {
        /// <summary>
        /// The string to load
        /// </summary>
        public string Data { get; set; }

        public override Stream Load(Context context)
        {
            return StreamHelper.LoadMemoryStream(Data);
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(Data, "Data");
        }
    }
}
