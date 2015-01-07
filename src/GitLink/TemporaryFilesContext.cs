// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporaryFilesContext.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System;
    using System.IO;
    using Catel.Logging;

    public class TemporaryFilesContext : IDisposable
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Guid _randomGuid = Guid.NewGuid();
        private readonly string _rootDirectory;

        public TemporaryFilesContext()
        {
            _rootDirectory = Path.Combine(Path.GetTempPath(), "GitLink", _randomGuid.ToString());

            Directory.CreateDirectory(_rootDirectory);
        }

        public string GetDirectory(string relativeDirectoryName)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeDirectoryName);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }

        public string GetFile(string relativeFilePath)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeFilePath);

            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return fullPath;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Log.Info("Deleting temporary files from '{0}'", _rootDirectory);

            try
            {
                if (Directory.Exists(_rootDirectory))
                {
                    Directory.Delete(_rootDirectory, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete temporary files");
            }
        }
    }
}