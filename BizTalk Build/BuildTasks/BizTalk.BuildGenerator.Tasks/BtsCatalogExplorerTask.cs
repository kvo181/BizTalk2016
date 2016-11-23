using System;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// This is a base class for tasks which want to use the BtsCatalog
    /// </summary>
    public class BtsCatalogExplorerTask : Task, IDisposable
    {
        #region Fields

        public BtsCatalogExplorerTask()
        {
            Catalog = new BtsCatalogExplorer();
        }

        #endregion

        /// <summary>
        /// Overrides the exewcute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            return true;
        }

        #region Properties

        /// <summary>
        /// The catalog
        /// </summary>
        protected BtsCatalogExplorer Catalog { get; set; }

        /// <summary>
        /// This is the optional application name that msbuild can provide
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// This is the connection string for the message box that should be supplied by the msbuild file
        /// </summary>
        [Required]
        public string MessageBoxConnection { get; set; }

        #endregion

        /// <summary>
        /// Disposable implementation
        /// </summary>
        /// <param name="canDispose"></param>
        protected virtual void Dispose(bool canDispose)
        {
            if (!canDispose) return;
            Catalog.Dispose();
            GC.SuppressFinalize(this);
        }

        #region IDisposable Members

        /// <summary>
        /// Implemented to adhere to code analysis
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}